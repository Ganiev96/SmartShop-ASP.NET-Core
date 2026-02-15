using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;

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

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Products = await _productService.GetAllAsync();

        return View(new CreateSaleDto
        {
            Items = new List<SaleItemDto>
            {
                new SaleItemDto()
            }
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateSaleDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Products = await _productService.GetAllAsync();
            return View(dto);
        }

        await _saleService.CreateAsync(dto);

        return RedirectToAction(nameof(Index));
    }
}
