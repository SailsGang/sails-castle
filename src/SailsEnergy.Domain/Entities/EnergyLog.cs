using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Domain.Entities;

public class EnergyLog : SoftDeletableEntity
{
    private static readonly TimeSpan _editWindow = TimeSpan.FromMinutes(5);

    public Guid GangId { get; private set; }
    public Guid GangCarId { get; private set; }
    public Guid PeriodId { get; private set; }
    public Guid LoggedByUserId { get; private set; }
    public decimal EnergyKwh { get; private set; }
    public DateTimeOffset ChargingDate { get; private set; }
    public string? Notes { get; private set; }
    public Guid AppliedTariffId { get; private set; }

    private EnergyLog() { }

    public static EnergyLog Create(
        Guid gangId,
        Guid gangCarId,
        Guid periodId,
        Guid loggedByUserId,
        decimal energyKwh,
        DateTimeOffset chargingDate,
        Guid appliedTariffId,
        string? notes,
        Guid createdBy)
    {
        if (energyKwh <= 0)
            throw new ValidationException(nameof(energyKwh), "Energy must be greater than zero.");

        var log = new EnergyLog
        {
            GangId = gangId,
            GangCarId = gangCarId,
            PeriodId = periodId,
            LoggedByUserId = loggedByUserId,
            EnergyKwh = energyKwh,
            ChargingDate = chargingDate,
            AppliedTariffId = appliedTariffId,
            Notes = notes
        };

        log.SetCreated(createdBy);

        log.AddDomainEvent(new EnergyLoggedEvent(gangId, periodId, log.Id, energyKwh));

        return log;
    }

    public void SetEnergyKwh(decimal energyKwh, Guid updatedBy)
    {
        EnsureWithinEditWindow();

        if (energyKwh <= 0)
            throw new ValidationException(nameof(energyKwh), "Energy must be greater than zero.");

        EnergyKwh = energyKwh;
        SetUpdated(updatedBy);
    }

    public void SetNotes(string? notes, Guid updatedBy)
    {
        EnsureWithinEditWindow();

        Notes = notes;
        SetUpdated(updatedBy);
    }

    public void SetChargingDate(DateTimeOffset chargingDate, Guid updatedBy)
    {
        EnsureWithinEditWindow();

        ChargingDate = chargingDate;
        SetUpdated(updatedBy);
    }

    private void EnsureWithinEditWindow()
    {
        if (DateTimeOffset.UtcNow - CreatedAt > _editWindow)
            throw new BusinessRuleException("EDIT_WINDOW_EXPIRED", "Energy logs can only be modified within 5 minutes of creation.");
    }
}
