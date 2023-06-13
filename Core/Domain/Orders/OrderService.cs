using Core.ControllerModels.Order;
using Core.Domain.Helpers;
using Core.Domain.StripePayments.Interfaces;
using Core.Domain.User;
using DataLayer;
using DataLayer.Entities.Cars;
using DataLayer.Entities.Orders;
using DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe;

namespace Core.Domain.Orders;

public class OrderService : IOrderService
{
    private readonly IUsersService _userService;
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly ILogger<OrderService> _logger;
    private readonly IStripeInvoiceService _stripeInvoiceService;

    public OrderService(IUsersService userService, ApplicationDbContext applicationDbContext, ILogger<OrderService> logger, IStripeInvoiceService stripeInvoiceService)
    {
        _userService = userService;
        _applicationDbContext = applicationDbContext;
        _logger = logger;
        _stripeInvoiceService = stripeInvoiceService;
    }

    public async Task<OrderResponseModel> CreateOrder(CreateOrderRequestModel model, string clientMail)
    {
        var signedInUser = await _userService.GetUserByMailAsync(clientMail);
        var car = await _applicationDbContext.FindAsync<Car>(model.CarId);
        if (car == null)
        {
            throw new Exception("car couldnt be found in db");
        }
        var order = new Order
        {
            Customer = signedInUser,
            CreatedAt = DateTime.UtcNow,
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Car = car
        };
        var dbOrder = await _applicationDbContext.AddAsync(order);
        await _applicationDbContext.SaveChangesAsync();

        var price = await GetStripePriceOfOrder(car);
        var result = OrderHelper.OrderResponseModelFromOrder(dbOrder.Entity, (long)price);
        return result;

    }

    public async Task<List<OrderResponseModel>> GetCustomersOrders(string loggedinUserMail)
    {
        var signedInUser = await _userService.GetUserByMailAsync(loggedinUserMail);
        var orders = await _applicationDbContext.Orders.Where(x => x.Customer == signedInUser).Include(x=>x.Car).ToListAsync();
        var result = new List<OrderResponseModel>();
        foreach (var order in orders)
        {
            if (order is not null)
            {
                var price = await GetStripePriceOfOrder(order.Car);
                result.Add(OrderHelper.OrderResponseModelFromOrder(order, price));
            }
        }
        return result;
    }

    public void PayOrder(int orderId, string clientMail)
    {
        var order = _applicationDbContext.Orders.Include(x => x.Car).Include(x => x.Payments).FirstOrDefault(x => x.Id == orderId);
        if (order is null)
        {
            throw new Exception("Order doesnt exist");
        }

        var signedInUser = _userService.GetUserByMailAsync(clientMail).Result;
        var subscription = _applicationDbContext.StripeSubscriptions.FirstOrDefault(x =>
            x.StripeSubscriptionStatus == DataLayer.Entities.User.StripeSubscriptionStatus.active
            && x.ApplicationUser == signedInUser);
        if (subscription is null
            || subscription.StripeSubscriptionId is null
            || subscription.StripeCustomerId is null
            || subscription.StripeSubscriptionStatus != DataLayer.Entities.User.StripeSubscriptionStatus.active)
        {
            throw new Exception("User deosnt have active suybscription.");
        }

        var priceId = order.Car.StripePriceId;

        var invoice = _stripeInvoiceService.CreateInvoiceForSubscription(priceId,
                                                                         subscription.StripeSubscriptionId,
                                                                         subscription.StripeCustomerId);

        if (order.Payments is null)
        {
            order.Payments = new List<StripeInvoice> { invoice };
        }
        else
        {
            order.Payments.Add(invoice);
        }
        _applicationDbContext.SaveChanges();
    }

    private async Task<long> GetStripePriceOfOrder(Car car)
    {
        var stripePriceService = new PriceService();
        var stripePriceObj = await stripePriceService.GetAsync(car.StripePriceId);
        var price = stripePriceObj.UnitAmount;
        if (price is null)
        {
            throw new Exception("price couldnt be retrieved from stripe");
        }
        return (long)price/100;
    }
}
