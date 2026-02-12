using SmartShop.Domain.Common;

namespace SmartShop.Domain.Entities;

public class Purchase : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public string SupplierName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public ICollection<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();
}
