using DataLayer;
using DataLayer.Entities.Cars;
using DTO;

namespace Core.Domain.Cars;

public class CarService : ICarService
{
    private readonly ApplicationDbContext _applicationDbContext;

    public CarService(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<bool> CreateCarAsync(CarDTO model)
    {
        var car = new Car
        {
            MileageAtPurchase = model.MileageAtPurchase,
            CurrentMileage = model.MileageAtPurchase,
            CarState = CarState.Parked,
            CarServiceState = CarServiceState.Fine,
            StripePriceId = "",
            PurchasePrice = model.PurchasePrice,
            Name = model.Name,
            PurchasedAt = DateTime.UtcNow,
        };
        await _applicationDbContext.AddAsync(car);
        await _applicationDbContext.SaveChangesAsync();
        return true;
    }
}
