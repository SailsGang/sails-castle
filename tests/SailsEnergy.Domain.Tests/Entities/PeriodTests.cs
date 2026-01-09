namespace SailsEnergy.Domain.Tests.Entities;

using SailsEnergy.Domain.Entities;
using Events;
using Exceptions;
using ValueObjects;

public class PeriodTests
{
    private readonly Guid _gangId = Guid.NewGuid();
    private readonly Guid _userId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldSucceed()
    {
        // Act
        var period = Period.Create(_gangId, _userId);

        // Assert
        period.GangId.Should().Be(_gangId);
        period.Status.Should().Be(PeriodStatus.Active);
        period.StartedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        period.ClosedAt.Should().BeNull();
        period.ClosedByUserId.Should().BeNull();
        period.Id.Should().NotBeEmpty();
        period.CreatedBy.Should().Be(_userId);
    }

    [Fact]
    public void Close_ShouldCloseActivePeriod()
    {
        // Arrange
        var period = Period.Create(_gangId, _userId);
        var closingUser = Guid.NewGuid();

        // Act
        period.Close(closingUser);

        // Assert
        period.Status.Should().Be(PeriodStatus.Closed);
        period.ClosedAt.Should().NotBeNull();
        period.ClosedAt.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromSeconds(1));
        period.ClosedByUserId.Should().Be(closingUser);
        period.UpdatedBy.Should().Be(closingUser);
    }

    [Fact]
    public void Close_ShouldRaisePeriodClosedEvent()
    {
        // Arrange
        var period = Period.Create(_gangId, _userId);
        period.ClearDomainEvents(); // Clear creation events

        // Act
        period.Close(_userId);

        // Assert
        period.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<PeriodClosedEvent>()
            .Which.Should().Match<PeriodClosedEvent>(e =>
                e.GangId == _gangId &&
                e.PeriodId == period.Id &&
                e.ClosedByUserId == _userId);
    }

    [Fact]
    public void Close_AlreadyClosedPeriod_ShouldThrow()
    {
        // Arrange
        var period = Period.Create(_gangId, _userId);
        period.Close(_userId);

        // Act & Assert
        var act = () => period.Close(Guid.NewGuid());
        act.Should().Throw<BusinessRuleException>()
           .Where(e => e.Code == "PERIOD_ALREADY_CLOSED");
    }
}
