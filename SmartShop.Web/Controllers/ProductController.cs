using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;

namespace SmartShop.Web.Controllers;

[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 20)
    {
        var products = await _productService.GetPagedAsync(page, pageSize);
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        await _productService.CreateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Owner")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
