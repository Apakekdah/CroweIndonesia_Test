using Hero.IoC;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace CI.Registration.MW
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var life = (IDisposableIoC)context.RequestServices.GetService(typeof(IDisposableIoC));
            using (var scope = life.New)
            {
                var log = scope.GetLogger(GetType());

                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    log.Error("Error while invoke next delegate", ex);

                    var response = context.Response;
                    response.ContentType = "application/json";

                    response.StatusCode = (int)HttpStatusCode.InternalServerError;

                    var result = Hero.HeroSerializer.Serializer.Serialize(new { Json = ex?.Message });

                    await response.WriteAsync(result);
                }
            }
        }
    }
}