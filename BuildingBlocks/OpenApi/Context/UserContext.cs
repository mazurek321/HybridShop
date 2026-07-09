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
}