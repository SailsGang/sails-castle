using SailsEnergy.Domain.Common;

namespace SailsEnergy.Domain.Entities;

public class GangCar : AuditableEntity
{
    public Guid GangId { get; private set; }
    public Guid CarId { get; private set; }
    public Guid AddedByMemberId { get; private set; }
    public bool IsActive { get; private set; }

    private GangCar() { }

    public static GangCar Create(Guid gangId, Guid carId, Guid addedByMemberId, Guid createdBy)
    {
        var gangCar = new GangCar
        {
            GangId = gangId,
            CarId = carId,
            AddedByMemberId = addedByMemberId,
            IsActive = true
        };

        gangCar.SetCreated(createdBy);

        return gangCar;
    }

    public void Deactivate(Guid updatedBy)
    {
        IsActive = false;
        SetUpdated(updatedBy);
    }

    public void Reactivate(Guid updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }
}
