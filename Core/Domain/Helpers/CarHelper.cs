﻿using Core.ControllerModels.Car;
using DataLayer.Entities.Cars;

namespace Core.Domain.Helpers
{
    public static class CarHelper
    {
        public static CarDTO GetCarDTOFromDbObject(Car car)
        {
            var unavailable = new List<DateTime>();
            if (car.Orders is not null)
            {
                foreach (var order in car.Orders)
                {
                    foreach (var day in Utils.EachDay(order.StartDate, order.EndDate))
                    {
                        unavailable.Add(day);
                    }
                }
            }


            var carDTO = new CarDTO
            {
                Id = car.Id,
                Name = car.Name,
                Unavailable = unavailable,
                PictureUrl = car.ProfilePic is not null? car.ProfilePic.RelativePath : ""
            };
            return carDTO;
        }
    }
}
