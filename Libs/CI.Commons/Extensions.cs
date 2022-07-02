using Hero;
using Hero.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Ride;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;

namespace CI
{
    public static class Extensions
    {
        public readonly static int DefaultPageSize = 20;

        public static IActionResult ToContentJson<T>(this ICommandResult<T> result)
        {
            if (result.IsNull())
                return new NoContentResult();

            var json = result.ToJson();
            result.Dispose();
            return new ContentResult
            {
                Content = json,
                ContentType = "application/json",
                StatusCode = HttpStatusCode.OK.GetHashCode()
            };
        }

        public static string GetActiveUser(this IPrincipal principal)
        {
            var claim = ((ClaimsIdentity)principal.Identity)?.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Name));
            return claim?.Value;
        }
    }
}