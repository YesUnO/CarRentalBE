using Core.ControllerModels.Order;
using DataLayer.Entities.Orders;
using DTO;

namespace Core.Domain.Orders;

public interface IOrderService
{
    Task<CreateOrderResponseModel> CreateOrder(OrderDTO model, string clientMail);
    void PayOrder(int orderId, string clientMail);
}
