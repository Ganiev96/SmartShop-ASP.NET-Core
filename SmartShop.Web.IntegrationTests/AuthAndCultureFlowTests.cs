using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SmartShop.Web.IntegrationTests;

public class AuthAndCultureFlowTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthAndCultureFlowTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProductIndex_Should_RedirectToLogin_WhenAnonymous()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Product");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task CultureSet_Should_WriteCultureCookie()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        var response = await client.GetAsync("/Culture/Set?culture=en&returnUrl=%2F");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains(".AspNetCore.Culture", response.Headers.TryGetValues("Set-Cookie", out var values)
            ? string.Join(";", values)
            : string.Empty);
    }
}
