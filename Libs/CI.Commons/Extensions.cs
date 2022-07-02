using Hero;
using Hero.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Ride;
using System.Collections.Generic;
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

        public static ICollection<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int limit)
        {
            source.ThrowIfNull("source");
            ICollection<IEnumerable<T>> cols = new HashSet<IEnumerable<T>>();
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    cols.Add(Partition(enumerator, limit).ToArray());
                }
            }
            return cols;
        }
        private static IEnumerable<T> Partition<T>(IEnumerator<T> source, int limit)
        {
            yield return source.Current;
            limit--;
            for (int i = 0; i < limit && source.MoveNext(); i++)
                yield return source.Current;
        }
    }
}