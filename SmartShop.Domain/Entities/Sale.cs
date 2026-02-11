using SmartShop.Domain.Common;

namespace SmartShop.Domain.Entities;

public class Sale : BaseEntity
{
    public decimal TotalAmount { get; set; }

    public decimal TotalProfit { get; set; }

    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();
}
