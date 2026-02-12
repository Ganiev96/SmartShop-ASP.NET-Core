using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Enums;

namespace SmartShop.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _context;

    public DashboardService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardDto> GetAsync()
    {
        var totalRevenue = await _context.CashTransactions
            .Where(c => c.Type == CashTransactionType.Income)
            .SumAsync(c => (decimal?)c.Amount) ?? 0;

        var totalExpense = await _context.CashTransactions
            .Where(c => c.Type == CashTransactionType.Expense)
            .SumAsync(c => (decimal?)c.Amount) ?? 0;

        var totalProfit = await _context.Sales
            .SumAsync(s => (decimal?)s.TotalProfit) ?? 0;

        var stockValue = await _context.Products
            .SumAsync(p => (decimal?)(p.QuantityInStock * p.PurchasePrice)) ?? 0;

        var lowStockCount = await _context.Products
            .CountAsync(p => p.QuantityInStock <= p.MinimumStockLevel);

        return new DashboardDto
        {
            TotalRevenue = totalRevenue,
            TotalExpense = totalExpense,
            TotalProfit = totalProfit,
            StockValue = stockValue,
            LowStockCount = lowStockCount
        };
    }
}
