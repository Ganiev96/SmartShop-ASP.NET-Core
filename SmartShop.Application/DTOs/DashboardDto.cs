namespace SmartShop.Application.DTOs;

public class DashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal TotalProfit { get; set; }
    public decimal StockValue { get; set; }
    public int LowStockCount { get; set; }
}
