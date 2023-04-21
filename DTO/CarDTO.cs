﻿
namespace DTO;

public class CarDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<DateTime> Unavailable { get; set; }
    public decimal MileageAtPurchase { get; set; }
    public decimal PurchasePrice { get; set; }

}
