namespace HybridShop.Services.Auth.Application.Exceptions;

public class UserIsNotBannedException : Exception
{
    public UserIsNotBannedException() 
        : base($"This user is not banned.")
    {}
}