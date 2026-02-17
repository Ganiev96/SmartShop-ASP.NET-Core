using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Entities;

namespace SmartShop.Application.Services;

public class ProductService : IProductService
{
    private const int MaxPageSize = 100;

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

    public async Task<PagedResult<Product>> GetPagedAsync(int page = 1, int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var query = _context.Products.AsNoTracking().OrderBy(p => p.Name);
        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
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
