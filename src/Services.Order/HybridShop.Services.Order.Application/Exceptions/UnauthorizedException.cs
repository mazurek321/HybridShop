namespace HybridShop.Services.Order.Application.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()  
        : base($"You dont have permission to do that.")
    {}
}