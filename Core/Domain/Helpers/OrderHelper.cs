using Core.ControllerModels.Order;
using DataLayer.Entities.Orders;

namespace Core.Domain.Helpers
{
    public static class OrderHelper
    {
        public static OrderResponseModel OrderResponseModelFromOrder(Order order, long price)
        {
            var dayCount = (order.EndDate - order.StartDate).Days +1;
            return new OrderResponseModel
            {
                Car  = CarHelper.GetCarDTOFromDbObject(order.Car) ,
                EndDate= order.EndDate,
                StartDate= order.StartDate,
                Id= order.Id,
                Price = dayCount*price,
                Paid = order.HasBeenPayed
            };
        }
    }
}
