using System.Security.Claims;
using Microsoft.AspNetCore.Http;


namespace BuildingBlocks.OpenApi.Context;

public class UserContext(IHttpContextAccessor httpContextAccessor) : IUserContext
{
    public Guid Id
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            return Guid.Parse(userId);
        }
    }

    public string Role
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var role = user.FindFirst(ClaimTypes.Role)?.Value 
                       ?? user.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(role))
            {
                throw new UnauthorizedAccessException("User role claim is missing.");
            }

            return role;
        }
    }

    public string Email
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
            {
                throw new UnauthorizedAccessException("User is not authenticated.");
            }

            var email = user.FindFirst(ClaimTypes.Email)?.Value 
                       ?? user.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                throw new UnauthorizedAccessException("User email claim is missing.");
            }

            return email;
        }
    }
}