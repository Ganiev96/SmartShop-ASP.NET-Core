using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Enums;

namespace SmartShop.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMemoryCache _cache;

    public DashboardService(IAppDbContext context, ICurrentUserService currentUser, IMemoryCache cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<DashboardDto> GetAsync()
    {
        var cacheKey = $"dashboard:{_currentUser.ShopId?.ToString() ?? "anonymous"}";

        if (_cache.TryGetValue(cacheKey, out DashboardDto? cached) && cached is not null)
        {
            return cached;
        }

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

        var result = new DashboardDto
        {
            TotalRevenue = totalRevenue,
            TotalExpense = totalExpense,
            TotalProfit = totalProfit,
            StockValue = stockValue,
            LowStockCount = lowStockCount
        };

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(1));

        return result;
    }
}
