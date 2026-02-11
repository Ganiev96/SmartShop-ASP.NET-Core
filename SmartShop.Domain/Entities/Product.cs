using SmartShop.Domain.Common;

namespace SmartShop.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public decimal PurchasePrice { get; set; }

    public decimal SalePrice { get; set; }

    public int QuantityInStock { get; set; }

    public int MinimumStockLevel { get; set; }

    public bool IsActive { get; set; } = true;
}
