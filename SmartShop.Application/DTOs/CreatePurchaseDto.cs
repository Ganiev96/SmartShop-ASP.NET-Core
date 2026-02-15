namespace SmartShop.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public class CreatePurchaseDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string SupplierName { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<PurchaseItemDto> Items { get; set; } = new();
}
