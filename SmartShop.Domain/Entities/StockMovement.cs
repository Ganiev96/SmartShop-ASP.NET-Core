using SmartShop.Domain.Common;
using SmartShop.Domain.Enums;

namespace SmartShop.Domain.Entities;

public class StockMovement : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public int ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public StockMovementType Type { get; set; }

    public int Quantity { get; set; }

    public string Source { get; set; } = string.Empty; // "Sale" or "Purchase"

    public int SourceId { get; set; }
}
