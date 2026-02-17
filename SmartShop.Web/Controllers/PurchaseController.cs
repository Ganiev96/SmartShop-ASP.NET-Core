using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShop.Application.DTOs;
using SmartShop.Application.Interfaces;

[Authorize]
public class PurchaseController : Controller
{
    private readonly IPurchaseService _purchaseService;
    private readonly IProductService _productService;

    public PurchaseController(IPurchaseService purchaseService,
                              IProductService productService)
    {
        _purchaseService = purchaseService;
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Products = (await _productService.GetPagedAsync(page: 1, pageSize: 200)).Items;

        return View(new CreatePurchaseDto
        {
            SupplierName = "Test Supplier",
            Items = new List<PurchaseItemDto>
        {
            new PurchaseItemDto()
        }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Products = (await _productService.GetPagedAsync(page: 1, pageSize: 200)).Items;
            return View(dto);
        }

        await _purchaseService.CreateAsync(dto);

        return RedirectToAction(nameof(Index));
    }

}
