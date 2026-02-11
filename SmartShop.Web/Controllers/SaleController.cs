using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;

namespace SmartShop.Web.Controllers;

[Authorize]
public class SaleController : Controller
{
    private readonly ISaleService _saleService;
    private readonly IProductService _productService;

    public SaleController(ISaleService saleService,
                          IProductService productService)
    {
        _saleService = saleService;
        _productService = productService;
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Products = await _productService.GetAllAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int productId, int quantity)
    {
        var dto = new CreateSaleDto
        {
            Items = new List<SaleItemDto>
            {
                new SaleItemDto
                {
                    ProductId = productId,
                    Quantity = quantity
                }
            }
        };

        await _saleService.CreateAsync(dto);


        return RedirectToAction("Index", "Product");
    }
}
