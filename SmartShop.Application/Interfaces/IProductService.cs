using SmartShop.Application.DTOs;
using SmartShop.Domain.Entities;

namespace SmartShop.Application.Interfaces;

public interface IProductService
{
    Task<Product> CreateAsync(CreateProductDto dto);
    Task<PagedResult<Product>> GetPagedAsync(int page = 1, int pageSize = 20);
    Task<Product?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}
