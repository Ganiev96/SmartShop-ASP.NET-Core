using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Entities;
using SmartShop.Domain.Enums;
using SmartShop.Infrastructure.Persistence;

namespace SmartShop.Application.Services;

public class PurchaseService : IPurchaseService
{
    private readonly AppDbContext _context;
    private readonly IAuditService _auditService;

    public PurchaseService(AppDbContext context,
                        IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task CreateAsync(CreatePurchaseDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var purchase = new Purchase
            {
                SupplierName = dto.SupplierName,
                TotalAmount = 0
            };

            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();

            decimal totalAmount = 0;

            foreach (var item in dto.Items)
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == item.ProductId);

                if (product == null)
                    throw new Exception("Product not found");

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

                // STOCK INCREASE
                product.QuantityInStock += item.Quantity;

                // STOCK MOVEMENT
                _context.StockMovements.Add(new StockMovement
                {
                    ProductId = product.Id,
                    Type = StockMovementType.In,
                    Quantity = item.Quantity,
                    Source = "Purchase",
                    SourceId = purchase.Id
                });
            }

            purchase.TotalAmount = totalAmount;

            // CASH TRANSACTION
            _context.CashTransactions.Add(new CashTransaction
            {
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
