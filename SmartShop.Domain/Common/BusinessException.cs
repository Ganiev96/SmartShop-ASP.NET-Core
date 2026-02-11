namespace SmartShop.Domain.Common;

public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }
}
