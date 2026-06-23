using System.Security.Claims;
using OrderManager.WebApi.Interfaces;

namespace OrderManager.WebApi.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userIdClaim = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");

            return userId;
        }
    }
}