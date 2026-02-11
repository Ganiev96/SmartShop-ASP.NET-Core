using SmartShop.Application.DTOs;

namespace SmartShop.Application.Interfaces;

public interface IPurchaseService
{
    Task CreateAsync(CreatePurchaseDto dto);
}
