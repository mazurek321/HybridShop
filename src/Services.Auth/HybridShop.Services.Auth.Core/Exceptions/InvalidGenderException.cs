using HybridShop.Services.Auth.Core.Models;

namespace HybridShop.Services.Auth.Core.Exceptions;

public class InvalidGenderException : Exception
{
    public InvalidGenderException() 
        : base($"This gender is not in our system. Use M of F.")
    {}
}