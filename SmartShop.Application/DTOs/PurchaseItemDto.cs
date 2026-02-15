namespace SmartShop.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class PurchaseItemDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(0.01, 999999999)]
    public decimal UnitPrice { get; set; }
}
