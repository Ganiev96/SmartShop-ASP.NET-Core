using SmartShop.Application.DTOs;
using SmartShop.Domain.Entities;

namespace SmartShop.Application.Interfaces;

public interface IProductService
{
    Task<Product> CreateAsync(CreateProductDto dto);
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
}
