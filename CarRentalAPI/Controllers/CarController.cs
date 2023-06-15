using Core.ControllerModels.Car;
using Core.Domain.Cars;
using Core.Infrastructure.Files;
using Duende.Bff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[BffApi]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IFileService _fileService;
    private readonly ILogger<CarController> _logger;

    public CarController(ICarService carService, ILogger<CarController> logger, IFileService fileService)
    {
        _carService = carService;
        _logger = logger;
        _fileService = fileService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCarRequestModel model)
    {
        try
        {
            var car = await _carService.CreateCarAsync(model);
            return Ok(car);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Creating car failed cause:");
            return BadRequest();
        }
    }

    [HttpPost("{carId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddProfilePicToCar([FromRoute] int carId, [FromForm] AddProfilePicToCarRequestModel model)
    {
        try
        {
            await _fileService.SaveCarProfilePickAsync(carId, model.File);
            return Ok(new
            {
                Success = true,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Adding car picture failed cause:");
            return BadRequest();
        }
    }

    [HttpDelete("{carId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProfilePicToCar(int carId)
    {
        try
        {
            await _fileService.DeleteCarProfilePickAsync(carId);
            return Ok(new
            {
                Success = true,
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Adding car picture failed cause:");
            return BadRequest();
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCars()
    {
        try
        {
            var cars = _carService.GetCars();
            return Ok(cars);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Getting cars failed cause:");
            return BadRequest();
        }
    }
}
