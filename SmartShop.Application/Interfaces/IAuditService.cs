namespace SmartShop.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityName);
}
