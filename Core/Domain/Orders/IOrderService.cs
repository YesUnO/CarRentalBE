using DataLayer.Entities.Orders;
using DTO;

namespace Core.Domain.Orders
{
    public interface IOrderService
    {
        Task<Order> GetSignedInUsersActiveOrder();
        Task<bool> CreateOrder(OrderDTO model);
    }
}
