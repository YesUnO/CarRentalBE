using Core.ControllerModels.Order;
using DataLayer.Entities.Orders;
using DTO;

namespace Core.Domain.Orders;

public interface IOrderService
{
    Task<OrderResponseModel> CreateOrder(OrderDTO model, string clientMail);
    Task<List<OrderResponseModel>> GetCustomersOrders(string loggedinUserMail);
    void PayOrder(int orderId, string clientMail);
}
