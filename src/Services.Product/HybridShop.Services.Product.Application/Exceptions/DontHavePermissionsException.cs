namespace HybridShop.Services.Product.Application.Exceptions;
public class DontHavePermissionsException : Exception
{
    public DontHavePermissionsException() 
        : base($"You dont have permissions to do that.")
    {}
}