namespace SailsEnergy.Application.Settings;

/// <summary>
/// Configuration settings for energy log behavior
/// </summary>
public class EnergyLogSettings
{
    public const string SectionName = "EnergyLog";
    
    /// <summary>
    /// Number of minutes after creation that an energy log can be edited
    /// </summary>
    public int EditWindowMinutes { get; set; } = 5;
}
