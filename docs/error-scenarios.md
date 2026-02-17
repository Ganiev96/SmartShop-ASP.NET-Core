# Error Scenario Demo Notes

## Scenarios
1. Sale with insufficient stock -> `BusinessException("Not enough stock.")`
2. Sale with inactive product -> `BusinessException`
3. Product create where `SalePrice < PurchasePrice` -> validation exception
4. Unknown product in purchase/sale -> exception + error middleware redirect

## UX expectation
- User should be redirected to `/Home/Error` with user-readable message.
- Validation errors should remain in form with field-level hints.
