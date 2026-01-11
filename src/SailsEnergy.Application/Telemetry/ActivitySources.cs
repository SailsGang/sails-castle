using System.Diagnostics;

namespace SailsEnergy.Application.Telemetry;

public static class ActivitySources
{
    public static readonly ActivitySource Gangs = new("SailsEnergy.Gangs");
    public static readonly ActivitySource Cars = new("SailsEnergy.Cars");
    public static readonly ActivitySource EnergyLogs = new("SailsEnergy.EnergyLogs");
    public static readonly ActivitySource Members = new("SailsEnergy.Members");
    public static readonly ActivitySource Auth = new("SailsEnergy.Auth");
    public static readonly ActivitySource Periods = new("SailsEnergy.Periods");
}
