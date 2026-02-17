# Localization Coverage Checklist

## Resource structure
- [x] `Resources/SharedResource.en.resx`
- [x] `Resources/SharedResource.uz.resx`
- [x] `SharedResource.cs` marker class

## UI coverage
- [x] Layout/nav labels localized
- [x] Home/Privacy localized
- [x] Product/Purchase/Sale/Dashboard labels localized
- [x] Auth actions (Login/Register/Logout) localized
- [x] Confirmation strings localized (`DeleteConfirm`)

## Runtime behavior
- [x] `RequestLocalizationOptions` configured
- [x] Cookie + query + header providers enabled
- [x] Culture switch endpoint exists (`/Culture/Set`)

## Follow-up
- [ ] Add localized validation/error messages in DataAnnotations
- [ ] Add currency format by culture
- [ ] Add automated localization key parity test
