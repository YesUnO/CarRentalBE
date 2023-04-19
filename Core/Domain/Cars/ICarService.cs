using DTO;

namespace Core.Domain.Cars;

public interface ICarService
{
    Task<bool> CreateCarAsync(CarDTO model);
    List<CarDTO> GetCars();
}
