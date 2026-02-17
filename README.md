# SmartShop ASP.NET Core

SmartShop — kichik va o'rta biznes uchun ombor, xarid va sotuv jarayonlarini boshqarish tizimi.

## Maqsad
- Mahsulotlar katalogini boshqarish.
- Xarid va sotuv oqimida stock/cash harakatini avtomatlashtirish.
- Multi-tenant (shop-based) va role-based accessni qo'llash.

## Texnologiyalar
- .NET 8, ASP.NET Core MVC + Razor Pages
- Entity Framework Core + PostgreSQL (Npgsql)
- ASP.NET Core Identity
- Bootstrap 5, jQuery validation
- Localization (`uz`, `en`) `.resx` fayllar orqali

## Arxitektura
Loyiha Clean-ish layering bilan tashkil qilingan:
- `SmartShop.Domain` — entity va business primitive'lar
- `SmartShop.Application` — service, DTO va interface'lar
- `SmartShop.Infrastructure` — EF Core DbContext, identity, DI
- `SmartShop.Web` — MVC UI, middleware, localization

```mermaid
flowchart LR
  Web[SmartShop.Web] --> App[SmartShop.Application]
  App --> Domain[SmartShop.Domain]
  Infra[SmartShop.Infrastructure] --> App
  Infra --> Domain
  Web --> Infra

## Ishga tushirish (local)
  appsettings.json ichida ConnectionStrings:DefaultConnection ni sozlang.
  Migrationni qo'llang.
  Web loyihani ishga tushiring.
  Eslatma: development uchun seed user'lar Seed:* config orqali override qilinadi.

## Environment variables / config
  ConnectionStrings__DefaultConnection
  Seed__AdminEmail
  Seed__AdminPassword
  Seed__SecondAdminEmail
  Seed__SecondAdminPassword

## Testlar
  SmartShop.Application.Tests — service unit testlar
  SmartShop.Web.IntegrationTests — auth/culture flow integration testlar

## Demo
  Demo screenshot: repo ichiga keyinroq qo'shiladi.

## Roadmap
  + Localization (uz/en) va culture switch.
  + CSRF hardening (AutoValidateAntiforgeryToken, POST delete).
  + Product pagination va batch query optimizatsiyasi.
    Purchase/Sale multi-row dynamic form UX.
    Swagger (agar API endpointlar alohida ajratilsa).
    Redis distributed cache.

## Known issues
  CI/CD pipeline hali qo'shilmagan.
  Kod coverage badge hozircha placeholder.
