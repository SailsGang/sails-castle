using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.Exceptions;
using SailsEnergy.Domain.ValueObjects;

namespace SailsEnergy.Domain.Entities;

public class Period : AuditableEntity
{
    public Guid GangId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }
    public PeriodStatus Status { get; private set; }
    public Guid? ClosedByUserId { get; private set; }

    private Period() { }

    public static Period Create(Guid gangId, Guid createdBy)
    {
        var period = new Period
        {
            GangId = gangId,
            StartedAt = DateTimeOffset.UtcNow,
            Status = PeriodStatus.Active
        };

        period.SetCreated(createdBy);

        return period;
    }

    public void Close(Guid closedByUserId)
    {
        if (Status == PeriodStatus.Closed)
            throw new BusinessRuleException("PERIOD_ALREADY_CLOSED", "Period is already closed.");

        Status = PeriodStatus.Closed;
        ClosedAt = DateTimeOffset.UtcNow;
        ClosedByUserId = closedByUserId;
        SetUpdated(closedByUserId);
        AddDomainEvent(new PeriodClosedEvent(GangId, Id, closedByUserId));
    }
}
