using SailsEnergy.Domain.Common;

namespace SailsEnergy.Domain.Entities;

public class GangCar : SoftDeletableEntity
{
    public Guid GangId { get; private set; }
    public Guid CarId { get; private set; }

    private GangCar() { }

    public static GangCar Create(Guid gangId, Guid carId, Guid createdBy)
    {
        var gangCar = new GangCar
        {
            GangId = gangId,
            CarId = carId
        };

        gangCar.SetCreated(createdBy);

        return gangCar;
    }
}
