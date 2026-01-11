namespace SailsEnergy.Api.Requests;

public record UpdateEnergyLogRequest(decimal? EnergyKwh, DateTimeOffset? ChargingDate, string? Notes);
