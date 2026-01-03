using SailsEnergy.Domain.Common;
using SailsEnergy.Domain.Events;
using SailsEnergy.Domain.Exceptions;

namespace SailsEnergy.Domain.Entities;

public class Tariff : AuditableEntity
{
    public Guid GangId { get; private set; }
    public decimal PricePerKwh { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public DateTimeOffset EffectiveFrom { get; private set; }
    public Guid SetByUserId { get; private set; }

    private Tariff() { }

    public static Tariff Create(
        Guid gangId,
        decimal pricePerKwh,
        string currency,
        Guid setByUserId)
    {
        ValidationException.ThrowIfNullOrWhiteSpace(currency, nameof(currency));

        if (pricePerKwh < 0)
            throw new ValidationException(nameof(pricePerKwh), "Price cannot be negative.");

        var tariff = new Tariff
        {
            GangId = gangId,
            PricePerKwh = pricePerKwh,
            Currency = currency.ToUpperInvariant(),
            EffectiveFrom = DateTimeOffset.UtcNow,
            SetByUserId = setByUserId
        };

        tariff.SetCreated(setByUserId);

        tariff.AddDomainEvent(new TariffChangedEvent(gangId, tariff.Id, pricePerKwh, currency));

        return tariff;
    }
}
