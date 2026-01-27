using System.Security.Claims;

namespace FINISHARK.Extensions
{
    public static class ClaimsExtensions
    {
        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty;
        }
    }
}
