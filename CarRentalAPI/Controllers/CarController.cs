using Core.ControllerModels.Car;
using Core.Domain.Cars;
using Core.Infrastructure.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> AddProfilePicToCar([FromRoute] int carId, [FromForm] AddProfilePicToCarRequestModel model)
    {
        try
        {
            await _fileService.SaveCarProfilePickAsync(carId, model.File);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Adding car picture failed cause:");
            return BadRequest();
        }
    }

    [HttpGet]
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
