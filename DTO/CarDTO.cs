﻿
namespace DTO;

public class CarDTO
{
    public string Name { get; set; }
    public List<DateTime> Unavailable { get; set; }
    public decimal MileageAtPurchase { get; set; }
    public decimal PurchasePrice { get; set; }

}
