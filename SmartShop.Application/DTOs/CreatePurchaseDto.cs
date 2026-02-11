public class CreatePurchaseDto
{
    public string SupplierName { get; set; } = string.Empty;
    public List<PurchaseItemDto> Items { get; set; } = new();
}
