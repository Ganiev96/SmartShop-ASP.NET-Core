using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Entities;
using SmartShop.Domain.Enums;

namespace SmartShop.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IAppDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public PurchaseService(IAppDbContext context,
                        IAuditService auditService,
                        ICurrentUserService currentUserService)
    {
        _context = context;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }

    public async Task CreateAsync(CreatePurchaseDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var purchase = new Purchase
            {
                ShopId = _currentUserService.ShopId!.Value,
                SupplierName = dto.SupplierName,
                TotalAmount = 0
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    throw new Exception("Product not found");
                }

                var subTotal = item.Quantity * item.UnitPrice;
                totalAmount += subTotal;

                var purchaseItem = new PurchaseItem
                {
                    PurchaseId = purchase.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = subTotal
                };

                _context.PurchaseItems.Add(purchaseItem);

                product.QuantityInStock += item.Quantity;

                _context.StockMovements.Add(new StockMovement
                {
                    ShopId = _currentUserService.ShopId!.Value,
                    ProductId = product.Id,
                    Type = StockMovementType.In,
                    Quantity = item.Quantity,
                    Source = "Purchase",
                    SourceId = purchase.Id
                });
            }

            purchase.TotalAmount = totalAmount;

            _context.CashTransactions.Add(new CashTransaction
            {
                ShopId = _currentUserService.ShopId!.Value,
                Type = CashTransactionType.Expense,
                Amount = totalAmount,
                ReferenceType = "Purchase",
                ReferenceId = purchase.Id
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            await _auditService.LogAsync("Create", "Purchase");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
