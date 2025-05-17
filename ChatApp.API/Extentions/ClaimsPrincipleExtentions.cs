using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ChatApp.API.Extentions
{
    public static class ClaimsPrincipleExtentions
    {
        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user.FindFirstValue(ClaimTypes.Name) ?? throw new Exception("Cann't Get UserName !");
        }

        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            return Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                throw new Exception("Cann't Get UserId !"));
        }
    }
}
