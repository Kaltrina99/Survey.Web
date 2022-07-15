using System;
using System.Linq;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Mvc.Razor
{
    public static class UserHasPermissionExtension
    {
        public static bool UserHasPermission(this ClaimsPrincipal principal, string permission)
        {
            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }
            var claim = principal.FindAll(x => x.Type.Equals("Permission")).Select(x => x.Value).Any(x => x.Equals(permission));
            return claim;
        }
    }
}
