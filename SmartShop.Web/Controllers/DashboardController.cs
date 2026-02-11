using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartShop.Application.Interfaces;

namespace SmartShop.Web.Controllers;

[Authorize(Roles = "Owner")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var data = await _dashboardService.GetAsync();
        return View(data);
    }
}
