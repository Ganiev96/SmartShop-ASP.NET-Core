namespace SmartShop.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreateProductDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string SKU { get; set; } = string.Empty;

    [Range(0.01, 999999999)]
    public decimal PurchasePrice { get; set; }

    [Range(0.01, 999999999)]
    public decimal SalePrice { get; set; }

    [Range(0, int.MaxValue)]
    public int MinimumStockLevel { get; set; }
}
