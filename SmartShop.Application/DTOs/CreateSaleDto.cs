namespace SmartShop.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateSaleDto
{
    [Required]
    [MinLength(1)]
    public List<SaleItemDto> Items { get; set; } = new();
}
