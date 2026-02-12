namespace SmartShop.Application.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    Guid? ShopId { get; }
}
