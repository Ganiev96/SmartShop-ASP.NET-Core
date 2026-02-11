using Microsoft.AspNetCore.Identity;

namespace SmartShop.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public bool IsDeleted { get; set; } = false;
}
