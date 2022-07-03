using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CI.API.OPFilters
{
    public class AuthorizeOPFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType?.GetCustomAttributes().OfType<AuthorizeAttribute>().Any() == true)
            {
                operation.Responses.Add(StatusCodes.Status401Unauthorized.ToString(), new OpenApiResponse() { Description = "Unauthorize" });
                operation.Responses.Add(StatusCodes.Status403Forbidden.ToString(), new OpenApiResponse() { Description = "Forbidden" });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                    //Id = "oauth2"
                                }
                            },
                            new List<string>
                            {
                                //"api"
                            }
                        }
                    }
                };
            }
        }
    }
}
