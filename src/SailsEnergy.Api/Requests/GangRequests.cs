namespace SailsEnergy.Api.Requests;

public record UpdateGangRequest(string? Name, string? Description);
public record AddMemberRequest(Guid UserId);
public record ChangeMemberRoleRequest(string Role);
public record AddCarToGangRequest(Guid CarId);
public record AddCarToGangResponse(Guid GangCarId);
