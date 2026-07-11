namespace HybridShop.Services.Auth.Application.Exceptions;

public class UserAlreadyDeletedException : Exception
{
    public UserAlreadyDeletedException() 
        : base($"User is already deleted.")
    {}
}