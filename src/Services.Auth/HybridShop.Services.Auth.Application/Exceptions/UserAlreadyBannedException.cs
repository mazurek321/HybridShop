namespace HybridShop.Services.Auth.Application.Exceptions;

public class UserAlreadyBannedException : Exception
{
    public UserAlreadyBannedException() 
        : base($"This user is already banned.")
    {}
}