using SailsEnergy.Domain.Common;

namespace SailsEnergy.Domain.Entities;

public class GangCar : AuditableEntity
{
    public Guid GangId { get; private set; }
    public Guid CarId { get; private set; }
    public bool IsActive { get; private set; }

    private GangCar() { }

    public static GangCar Create(Guid gangId, Guid carId, Guid createdBy)
    {
        var gangCar = new GangCar
        {
            GangId = gangId,
            CarId = carId,
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

    public void Activate(Guid updatedBy)
    {
        IsActive = true;
        SetUpdated(updatedBy);
    }
}
