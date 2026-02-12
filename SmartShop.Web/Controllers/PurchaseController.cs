using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        ViewBag.Products = await _productService.GetAllAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(int productId, int quantity, decimal unitPrice)
    {
        var dto = new CreatePurchaseDto
        {
            SupplierName = "Test Supplier",
            Items = new List<PurchaseItemDto>
            {
                new PurchaseItemDto
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                }
            }
        };

        await _purchaseService.CreateAsync(dto);

        return RedirectToAction(nameof(Index));
    }
}
