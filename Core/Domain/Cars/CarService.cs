using Core.ControllerModels.Car;
using Core.Domain.Helpers;
using DataLayer;
using DataLayer.Entities.Cars;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Domain.Cars;

public class CarService : ICarService
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<CarService> _logger;

    public CarService(ApplicationDbContext applicationDbContext, ILogger<CarService> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }

    public async Task<CarDTO> CreateCarAsync(CreateCarRequestModel model)
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
        return CarHelper.GetCarDTOFromDbObject(car);
    }

    public List<CarDTO> GetCars()
    {
        var cars = _applicationDbContext.Cars.Include(x=>x.Orders).ToList();
        var carsList = new List<CarDTO>();
        foreach (var car in cars)
        {
            carsList.Add(CarHelper.GetCarDTOFromDbObject(car));
        }
        return carsList;
    }
}
