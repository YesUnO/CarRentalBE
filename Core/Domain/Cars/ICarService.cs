using Core.ControllerModels.Car;

namespace Core.Domain.Cars;

public interface ICarService
{
    Task<CarDTO> CreateCarAsync(CreateCarRequestModel model);
    List<CarDTO> GetCars();
}
