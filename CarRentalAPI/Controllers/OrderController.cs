﻿using Microsoft.AspNetCore.Mvc;
using Core.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Core.ControllerModels.Order;
using Duende.Bff;
using IdentityModel;

namespace CarRentalAPI.Controllers;


[Route("api/[controller]")]
[BffApi]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IOrderService orderService, ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> Create(CreateOrderRequestModel model)
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(JwtClaimTypes.Email);
            var order = await _orderService.CreateOrder(model, loggedinUserMail);
            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetOrdes()
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(JwtClaimTypes.Email);
            List< OrderResponseModel> orders = await _orderService.GetCustomersOrders(loggedinUserMail);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{orderId}")]
    [Authorize(Policy = "ViewOwnOrdersPolicy", Roles = "Customer")]
    public async Task<IActionResult> PayOrder(int orderId)
    {
        try
        {
            var loggedinUserMail = HttpContext.User.FindFirstValue(JwtClaimTypes.Email);
            _orderService.PayOrder(orderId, loggedinUserMail);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return BadRequest(ex.Message);

        }
    }
}
