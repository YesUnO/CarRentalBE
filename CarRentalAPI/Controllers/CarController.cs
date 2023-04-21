using Core.ControllerModels.Car;
using Core.Domain.Cars;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CarController : ControllerBase
{
    private readonly ICarService _carService;

    public CarController(ICarService carService)
    {
        _carService = carService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<IActionResult> Create(CreateCarRequestModel model)
    {
        var car = await _carService.CreateCarAsync(model);
        return Ok(car);
    }

    [HttpGet]
    public async Task<IActionResult> GetCars()
    {
        var cars = _carService.GetCars();
        return Ok(cars);
    }
}
