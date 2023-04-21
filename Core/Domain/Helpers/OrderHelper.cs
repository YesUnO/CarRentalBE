using Core.ControllerModels.Order;
using DataLayer.Entities.Orders;

namespace Core.Domain.Helpers
{
    public static class OrderHelper
    {
        public static CreateOrderResponseModel CreateOrderResponseModelFromOrder(Order order, long price)
        {
            var dayCount = (order.EndDate - order.StartDate).Days;
            return new CreateOrderResponseModel
            {
                Car  = CarHelper.GetCarDTOFromDbObject(order.Car) ,
                EndDate= order.EndDate,
                StartDate= order.StartDate,
                Id= order.Id,
                Price = dayCount*price
            };
        }
    }
}
