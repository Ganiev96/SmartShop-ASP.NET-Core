using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Common;
using SmartShop.Domain.Entities;
using SmartShop.Infrastructure.Identity;
using System.Linq.Expressions;


namespace SmartShop.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
{
    private readonly ICurrentUserService _currentUserService;
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

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p =>
                !_currentUserService.ShopId.HasValue ||
                p.ShopId == _currentUserService.ShopId.Value);

        modelBuilder.Entity<Purchase>()
            .HasQueryFilter(p =>
                !_currentUserService.ShopId.HasValue ||
                p.ShopId == _currentUserService.ShopId.Value);

        modelBuilder.Entity<Sale>()
            .HasQueryFilter(p =>
                !_currentUserService.ShopId.HasValue ||
                p.ShopId == _currentUserService.ShopId.Value);
    }


}
