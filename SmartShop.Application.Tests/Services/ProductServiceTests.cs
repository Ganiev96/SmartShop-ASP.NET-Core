using Microsoft.EntityFrameworkCore;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;
using SmartShop.Application.Services;
using SmartShop.Infrastructure.Persistence;
using Xunit;

namespace SmartShop.Application.Tests.Services;

public class ProductServiceTests
{
    [Fact]
    public async Task GetPagedAsync_Should_ReturnRequestedPageAndSortedItems()
    {
        var context = CreateDbContext();
        var audit = new FakeAuditService();
        var currentUser = new FakeCurrentUserService(Guid.NewGuid(), "user-1");

        context.Products.AddRange(
            new SmartShop.Domain.Entities.Product { Name = "C", SKU = "C-1", PurchasePrice = 1, SalePrice = 2, ShopId = currentUser.ShopId!.Value },
            new SmartShop.Domain.Entities.Product { Name = "A", SKU = "A-1", PurchasePrice = 1, SalePrice = 2, ShopId = currentUser.ShopId!.Value },
            new SmartShop.Domain.Entities.Product { Name = "B", SKU = "B-1", PurchasePrice = 1, SalePrice = 2, ShopId = currentUser.ShopId!.Value });
        await context.SaveChangesAsync();

        var service = new ProductService(context, audit, currentUser);

        var page = await service.GetPagedAsync(page: 2, pageSize: 1);

        Assert.Equal(2, page.Page);
        Assert.Equal(1, page.PageSize);
        Assert.Equal(3, page.TotalCount);
        Assert.Single(page.Items);
        Assert.Equal("B", page.Items[0].Name);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options, new FakeCurrentUserService(null, null));
    }

    private sealed class FakeCurrentUserService(Guid? shopId, string? userId) : ICurrentUserService
    {
        public string? UserId { get; } = userId;
        public Guid? ShopId { get; } = shopId;
    }

    private sealed class FakeAuditService : IAuditService
    {
        public Task LogAsync(string action, string entityName) => Task.CompletedTask;
    }
}
