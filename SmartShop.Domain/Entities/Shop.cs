using SmartShop.Domain.Common;

namespace SmartShop.Domain.Entities;

public class Shop
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
    public ICollection<Purchase> Purchases { get; set; } = new List<Purchase>();
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public ICollection<CashTransaction> CashTransactions { get; set; } = new List<CashTransaction>();
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
