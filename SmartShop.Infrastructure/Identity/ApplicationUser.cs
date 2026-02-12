using Microsoft.AspNetCore.Identity;
using SmartShop.Domain.Entities;

namespace SmartShop.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsDeleted { get; set; } = false;

    public Guid ShopId { get; set; }

    public Shop Shop { get; set; } = null!;
}
