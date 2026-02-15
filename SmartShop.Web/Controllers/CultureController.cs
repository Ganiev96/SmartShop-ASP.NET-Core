using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace SmartShop.Web.Controllers
{
    public class CultureController : Controller
    {
        private readonly RequestLocalizationOptions _localizationOptions;

        public CultureController(IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _localizationOptions = localizationOptions.Value;
        }

        public IActionResult Set(string? culture, string? returnUrl)
        {
            var supportedCultures = (_localizationOptions.SupportedUICultures ?? [])
                .Select(x => x.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var defaultCulture = _localizationOptions.DefaultRequestCulture?.UICulture.Name ?? "uz";

            if (string.IsNullOrWhiteSpace(culture) || !supportedCultures.Contains(culture))
            {
                culture = defaultCulture;
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    HttpOnly = true,
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Secure = HttpContext.Request.IsHttps
                });

            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Content("~/");
            }

            return LocalRedirect(returnUrl!);
        }
    }
}
