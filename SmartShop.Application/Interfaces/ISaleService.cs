using SmartShop.Application.DTOs;

namespace SmartShop.Application.Interfaces;

public interface ISaleService
{
    Task CreateAsync(CreateSaleDto dto);
}
