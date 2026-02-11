using SmartShop.Domain.Common;
using SmartShop.Domain.Enums;

namespace SmartShop.Domain.Entities;

public class CashTransaction : BaseEntity
{
    public CashTransactionType Type { get; set; }

    public decimal Amount { get; set; }

    public string ReferenceType { get; set; } = string.Empty; // "Sale" or "Purchase"

    public int ReferenceId { get; set; }
}
