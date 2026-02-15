using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SmartShop.Application.Interfaces;
using SmartShop.Domain.Common;
using SmartShop.Domain.Entities;
using SmartShop.Infrastructure.Identity;
using System.Linq.Expressions;

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
        ApplyGlobalQueryFilters(modelBuilder);
    }

    private void ApplyGlobalQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var filter = BuildFilterExpression(entityType);
            if (filter != null)
            {
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }

    private LambdaExpression? BuildFilterExpression(IMutableEntityType entityType)
    {
        var clrType = entityType.ClrType;
        var parameter = Expression.Parameter(clrType, "e");
        Expression? body = null;

        if (entityType.FindProperty(nameof(BaseEntity.IsDeleted))?.ClrType == typeof(bool))
        {
            var isDeletedProperty = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(bool) },
                parameter,
                Expression.Constant(nameof(BaseEntity.IsDeleted)));

            var isNotDeleted = Expression.Equal(isDeletedProperty, Expression.Constant(false));
            body = isNotDeleted;
        }

        if (entityType.FindProperty("ShopId")?.ClrType == typeof(Guid))
        {
            var shopIdProperty = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new[] { typeof(Guid) },
                parameter,
                Expression.Constant("ShopId"));

            var currentShopId = Expression.Property(
                Expression.Constant(this),
                nameof(CurrentShopId));

            var hasValue = Expression.Property(currentShopId, nameof(Nullable<Guid>.HasValue));
            var value = Expression.Property(currentShopId, nameof(Nullable<Guid>.Value));

            var tenantFilter = Expression.OrElse(
                Expression.Not(hasValue),
                Expression.Equal(shopIdProperty, value));

            body = body == null ? tenantFilter : Expression.AndAlso(body, tenantFilter);
        }

        return body == null ? null : Expression.Lambda(body, parameter);
    }
}
