using SmartShop.Application.Interfaces;
using SmartShop.Domain.Entities;
using SmartShop.Infrastructure.Persistence;

namespace SmartShop.Application.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditService(AppDbContext context,
                        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogAsync(string action, string entityName)
    {
        var log = new AuditLog
        {
            UserId = _currentUser.UserId ?? "System",
            Action = action,
            EntityName = entityName
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
