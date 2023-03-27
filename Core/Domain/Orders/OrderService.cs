using Core.Domain.User;
using DataLayer;
using DataLayer.Entities.Orders;

namespace Core.Domain.Orders
{
    public class TestService : ITestService
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _applicationDbContext;

        public TestService(IUserService userService, ApplicationDbContext applicationDbContext)
        {
            _userService = userService;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Order> GetSignedInUsersActiveOrder()
        {
            var signedInUser = await _userService.GetSignedInUser();
            var activeOrder = _applicationDbContext.Orders.FirstOrDefault(x => x.Customer == signedInUser);
            throw new NotImplementedException();
        }
    }
}
