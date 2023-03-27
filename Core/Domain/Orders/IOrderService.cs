using DataLayer.Entities.Orders;

namespace Core.Domain.Orders
{
    public interface ITestService
    {
        Task<Order> GetSignedInUsersActiveOrder();
    }
}
