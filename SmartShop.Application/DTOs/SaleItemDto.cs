namespace SmartShop.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class SaleItemDto
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
