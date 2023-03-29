using Core.Domain.User;
using DataLayer;
using DataLayer.Entities.Cars;
using DataLayer.Entities.Orders;
using DTO;

namespace Core.Domain.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _applicationDbContext;

        public OrderService(IUserService userService, ApplicationDbContext applicationDbContext)
        {
            _userService = userService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<bool> CreateOrder(OrderDTO model)
        {
            var signedInUser = await _userService.GetSignedInUserAsync();
            var car = await _applicationDbContext.FindAsync<Car>(model.CarId);
            if (car == null)
            {
                return false;
            }
            var order = new Order
            {
                Customer = signedInUser,
                CreatedAt= DateTime.UtcNow,
                Car = car
            };
            await _applicationDbContext.AddAsync(order);
            await _applicationDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Order> GetSignedInUsersActiveOrder()
        {
            var signedInUser = await _userService.GetSignedInUserAsync();
            var activeOrder = _applicationDbContext.Orders.FirstOrDefault(x => x.Customer == signedInUser);
            throw new NotImplementedException();
        }
    }
}
