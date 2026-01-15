namespace SailsEnergy.Application.Settings;

/// <summary>
/// Configuration settings for gang behavior
/// </summary>
public class GangSettings
{
    public const string SectionName = "Gang";
    
    /// <summary>
    /// Maximum number of members allowed per gang
    /// </summary>
    public int MaxMembersPerGang { get; set; } = 50;
    
    /// <summary>
    /// Maximum number of cars allowed per gang
    /// </summary>
    public int MaxCarsPerGang { get; set; } = 25;
}
