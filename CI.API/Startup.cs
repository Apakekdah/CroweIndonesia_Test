using CI.API.OPFilters;
using CI.Registration.MW;
using Hero.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CI.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment appEnv)
        {
            Configuration = configuration;
            Environment = appEnv;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                  .AddJsonOptions(opt =>
                  {
                      opt.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                      opt.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                      opt.JsonSerializerOptions.IgnoreNullValues = true;
                      opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                      opt.JsonSerializerOptions.Converters.Add(Hero.Core.Serializer.TextJson.Converter.DateTimeJsonConverter.Create());
                      opt.JsonSerializerOptions.Converters.Add(Hero.Core.Serializer.TextJson.Converter.DateTimeNullJsonConverter.Create());
                      opt.JsonSerializerOptions.Converters.Add(Hero.Core.Serializer.TextJson.Converter.TimeSpanJsonConverter.Create());
                      opt.JsonSerializerOptions.Converters.Add(Hero.Core.Serializer.TextJson.Converter.TimeSpanNullJsonConverter.Create());
                      opt.JsonSerializerOptions.Converters.Add(Hero.Core.Serializer.TextJson.Converter.ExceptionConverter.Create());
                      opt.JsonSerializerOptions.Converters.Add(Hero.Core.Serializer.TextJson.Converter.DictionaryObjectConverter.Create());
                      opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                  });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Crowe Indonesia API", Version = "v1" });
                c.ResolveConflictingActions(x => x.First());

                var identityUri = Configuration["identityServerHost"];
                var tokenUrl = new Uri(new Uri(identityUri), "api/oauth");
                //var oauthUrl = new Uri("../api/oauth", UriKind.Relative);

                c.AddSecurityDefinition("oauth2", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
                    Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
                    {
                        ClientCredentials = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
                        {
                            TokenUrl = tokenUrl,
                            Scopes = new Dictionary<string, string>
                            {
                                { "Atlas", "Atlas. Full Access" },
                                { "Weather", "Forecase wheather" },
                                { "Admin", "Adminstration controller" }
                            }
                        }
                    }
                });

                c.OperationFilter<AuthorizeOPFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseCors();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "ci-sdk";
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "Crowe Indonesia API v1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "api_default",
                    pattern: "api/{controller=WeatherForecast}/{action=Get}/{id?}");
            });
        }

        public void ConfigureContainer(IBuilderIoC builder)
        {
            if (Environment.IsDevelopment())
            {
                builder.RegisteringCIServicesDevelopment(Configuration);
            }
            else
            {
                builder.RegisteringCIServicesProduction(Configuration);
            }
        }
    }
}
