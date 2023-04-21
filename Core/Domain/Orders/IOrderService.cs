using DataLayer.Entities.Orders;
using DTO;

namespace Core.Domain.Orders;

public interface IOrderService
{
    Task<Order> GetSignedInUsersActiveOrder();
    Task<bool> CreateOrder(OrderDTO model, string clientMail);
    void PayOrder(int orderId, string clientMail);
}
