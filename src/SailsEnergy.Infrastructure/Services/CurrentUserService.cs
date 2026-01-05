using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(claim, out var id) ? id : null;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? UserName => User?.FindFirst(ClaimTypes.Name)?.Value;

    public IReadOnlyList<string> Roles => User?.FindAll(ClaimTypes.Role)
        .Select(c => c.Value)
        .ToList() ?? [];

    public bool IsAuthenticated => UserId.HasValue;
}
