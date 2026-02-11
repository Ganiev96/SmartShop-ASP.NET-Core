using SmartShop.Domain.Common;

namespace SmartShop.Domain.Entities;

public class SaleItem : BaseEntity
{
    public int SaleId { get; set; }

    public Sale Sale { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal PurchaseSnapshotPrice { get; set; }

    public decimal Profit { get; set; }
}
