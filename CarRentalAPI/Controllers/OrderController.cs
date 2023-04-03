using Microsoft.AspNetCore.Mvc;
using Core.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DTO;

namespace CarRentalAPI.Controllers;


[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Customer")]
    public async Task<IActionResult> Create(OrderDTO model)
    {
        await _orderService.CreateOrder(model);
        return Ok();
    }
}
