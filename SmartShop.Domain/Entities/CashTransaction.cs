using SmartShop.Domain.Common;
using SmartShop.Domain.Enums;

namespace SmartShop.Domain.Entities;

public class CashTransaction : BaseEntity
{
    public Guid ShopId { get; set; }
    public Shop Shop { get; set; } = null!;

    public CashTransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string ReferenceType { get; set; } = string.Empty; // "Sale" or "Purchase"

    public int ReferenceId { get; set; }
}
