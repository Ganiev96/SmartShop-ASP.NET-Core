using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Entities;

namespace SmartShop.Application.Services;

public class ProductService : IProductService
{
    private readonly IAppDbContext _context;
    private readonly IAuditService _auditService;
    private readonly ICurrentUserService _currentUserService;

    public ProductService(IAppDbContext context,
                      IAuditService auditService,
                      ICurrentUserService currentUserService)
    {
        _context = context;
        _auditService = auditService;
        _currentUserService = currentUserService;
    }


    public async Task<Product> CreateAsync(CreateProductDto dto)
    {
        if (dto.SalePrice < dto.PurchasePrice)
            throw new Exception("Sale price cannot be lower than purchase price.");
        if (_currentUserService.ShopId == null)
            throw new UnauthorizedAccessException("ShopId not found for current user");

        var product = new Product
        {
            Name = dto.Name,
            SKU = dto.SKU,
            PurchasePrice = dto.PurchasePrice,
            SalePrice = dto.SalePrice,
            MinimumStockLevel = dto.MinimumStockLevel,
            QuantityInStock = 0,
            ShopId = _currentUserService.ShopId.Value
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("Create", "Product");

        return product;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await GetByIdAsync(id);
        if (product == null)
            throw new Exception("Product not found");

        product.IsDeleted = true;
        await _context.SaveChangesAsync();
        await _auditService.LogAsync("SoftDelete", "Product");
    }
}
