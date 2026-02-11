using SmartShop.Domain.Common;

namespace SmartShop.Domain.Entities;

public class PurchaseItem : BaseEntity
{
    public int PurchaseId { get; set; }

    public Purchase Purchase { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal SubTotal { get; set; }
}
