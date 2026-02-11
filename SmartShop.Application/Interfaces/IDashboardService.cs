using SmartShop.Application.DTOs;

namespace SmartShop.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetAsync();
}
