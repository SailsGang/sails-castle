using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SailsEnergy.Infrastructure.Identity;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        // Apply EF Core migrations
        var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();

        // Seed Identity roles (for users)
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await SeedRolesAsync(roleManager, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager, ILogger logger)
    {
        // Identity roles for user access levels
        // (Gang member roles are handled by MemberRole enum in Domain)
        string[] roles = ["Admin", "User"];

        foreach (var roleName in roles)
        {
            try
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;

                var result = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));

                if (result.Succeeded) logger.LogInformation("Created role: {RoleName}", roleName);
            }
            catch (DbUpdateException)
            {
                logger.LogDebug("Role {RoleName} already exists (concurrent creation)", roleName);
            }
        }
    }
}
