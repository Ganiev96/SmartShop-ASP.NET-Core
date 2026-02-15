using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Entities;
using SmartShop.Infrastructure.Identity;

namespace SmartShop.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
{
    private readonly ICurrentUserService _currentUserService;
    private Guid? CurrentShopId => _currentUserService.ShopId;

    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUserService)
        : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Purchase> Purchases => Set<Purchase>();
    public DbSet<PurchaseItem> PurchaseItems => Set<PurchaseItem>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<CashTransaction> CashTransactions => Set<CashTransaction>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplySoftDeleteAndTenantFilters(modelBuilder);
    }

    private void ApplySoftDeleteAndTenantFilters(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.ShopId == CurrentShopId));

        modelBuilder.Entity<Purchase>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.ShopId == CurrentShopId));

        modelBuilder.Entity<Sale>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.ShopId == CurrentShopId));

        // Related entities also get filters to avoid EF warning 10622
        modelBuilder.Entity<PurchaseItem>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.Purchase.ShopId == CurrentShopId));

        modelBuilder.Entity<SaleItem>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.Sale.ShopId == CurrentShopId));

        modelBuilder.Entity<StockMovement>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.ShopId == CurrentShopId));

        modelBuilder.Entity<CashTransaction>()
            .HasQueryFilter(e =>
                !e.IsDeleted &&
                (!CurrentShopId.HasValue || e.ShopId == CurrentShopId));
    }
}
