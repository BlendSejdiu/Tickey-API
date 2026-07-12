using System.Security.Claims;

namespace Tickey.Sevices;

public class AuthService(IHttpContextAccessor _httpContextAccessor)
{
    private ClaimsPrincipal? User =>_httpContextAccessor.HttpContext?.User;

    public Guid GetUserId()
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(userIdClaim, out var userId)? userId : Guid.Empty;
    }

    public string GetUserRole()
    {
        return User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    public bool IsAdmin()
    {
        return GetUserRole().Equals("Admin", StringComparison.OrdinalIgnoreCase);
    }

}
