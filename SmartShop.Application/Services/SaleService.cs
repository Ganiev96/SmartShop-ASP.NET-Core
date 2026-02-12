using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Common;
using SmartShop.Domain.Entities;
using SmartShop.Domain.Enums;

namespace SmartShop.Application.Services;

public class SaleService : ISaleService
{
    private readonly IAppDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public SaleService(IAppDbContext context,
                   IAuditService auditService,
                   ICurrentUserService currentUserService)
    {
        _context = context;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }

    public async Task CreateAsync(CreateSaleDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var sale = new Sale
            {
                ShopId = _currentUserService.ShopId!.Value,
                TotalAmount = 0,
                TotalProfit = 0
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();

            decimal totalAmount = 0;
            decimal totalProfit = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new BusinessException("Product not found.");

                if (!product.IsActive)
                    throw new BusinessException("Inactive product cannot be sold.");

                if (product.QuantityInStock < item.Quantity)
                    throw new BusinessException("Not enough stock.");

                var unitPrice = product.SalePrice;
                var purchaseSnapshot = product.PurchasePrice;

                var profit = (unitPrice - purchaseSnapshot) * item.Quantity;
                var subTotal = unitPrice * item.Quantity;

                totalAmount += subTotal;
                totalProfit += profit;

                var saleItem = new SaleItem
                {
                    SaleId = sale.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    PurchaseSnapshotPrice = purchaseSnapshot,
                    Profit = profit
                };

                _context.SaleItems.Add(saleItem);

                // STOCK DECREASE
                product.QuantityInStock -= item.Quantity;

                // STOCK MOVEMENT
                _context.StockMovements.Add(new StockMovement
                {ShopId = _currentUserService.ShopId!.Value,
                    ProductId = product.Id,
                    Type = StockMovementType.Out,
                    Quantity = item.Quantity,
                    Source = "Sale",
                    SourceId = sale.Id
                });
            }

            sale.TotalAmount = totalAmount;
            sale.TotalProfit = totalProfit;

            // CASH TRANSACTION (Income)
            _context.CashTransactions.Add(new CashTransaction
            {ShopId = _currentUserService.ShopId!.Value,
                Type = CashTransactionType.Income,
                Amount = totalAmount,
                ReferenceType = "Sale",
                ReferenceId = sale.Id
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            await _auditService.LogAsync("Create", "Sale");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
