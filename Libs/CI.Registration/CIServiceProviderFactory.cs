using Hero.Cache;
using Hero.Core.Interfaces;
using Hero.IoC;
using Hero.IoC.Autofac;
using Hero.IoC.Autofac.LoggerModule;
using Hero.Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RI = Ride.Interfaces;
using RS = Ride.Security;

namespace CI
{
    public static class CIServiceProviderFactory
    {
        public static IHostBuilder UseCIServiceProviderFactory(this IHostBuilder host)
        {
            return host.UseServiceProviderFactory(new IoCServiceProviderFactory());
        }

        public static void RegisteringCIServicesDevelopment(this IBuilderIoC builder, IConfiguration configuration)
        {
            RegisteringCIServices(builder, configuration);
        }

        public static void RegisteringCIServicesProduction(this IBuilderIoC builder, IConfiguration configuration)
        {
            RegisteringCIServices(builder, configuration);
        }

        static void RegisteringCIServices(this IBuilderIoC builder, IConfiguration configuration)
        {
            builder.RegisterModule<LogModule>();

            builder.RegisterLog<NLogIt>();

            var registerAsms = new[] {
                    // Api registration
                    typeof(Data.Business.BootstrapBusiness).Assembly,
                    typeof(Data.EF.MongoDB.BootstrapMongoEF).Assembly,
                    typeof(Mappers.BootstrapMapper).Assembly,
                    typeof(API.Handlers.BootstrapApiHandlers).Assembly,
                    typeof(Mappers.BootstrapJWT).Assembly,
                    typeof(Seeder.BootstrapSeeder).Assembly,
                };

            builder.RegisterAllBootstrapLoaderBuilder(registerAsms);

            builder.Register<RI.ISecurityContext, RS.HttpSecurityContext>(ScopeIoC.Lifetime);

            builder.Register<ICache, InMemoryCache>(ScopeIoC.Singleton);
        }
    }
}