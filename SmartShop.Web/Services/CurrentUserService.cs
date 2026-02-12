using System.Security.Claims;
using SmartShop.Application.Interfaces;

namespace SmartShop.Web.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

    public Guid? ShopId
    {
        get
        {
            var shopIdClaim = _httpContextAccessor.HttpContext?
                .User?
                .FindFirst("ShopId")?.Value;

            return shopIdClaim != null ? Guid.Parse(shopIdClaim) : null;
        }
    }

}
