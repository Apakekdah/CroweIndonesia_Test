using Hero.Core.Interfaces;
using Hero.IoC;
using System.Threading.Tasks;

namespace CI.Seeder
{
    public class BootstrapSeeder : IBootstrapLoader<IBuilderIoC>
    {
        public Task Run(IBuilderIoC container)
        {
            //container.RegisterAsImplement<MeetingEventSeeder>();
            //container.reg

            var asm = typeof(FakeClass).Assembly;
            container.RegisterAssemblyTypes(RegistrationTypeIoC.AsImplement,
                new[] { typeof(IAutoStartService) }, ScopeIoC.Lifetime,
                new[] { asm });


            return Task.FromResult(0);
        }

        public Task Stop()
        {
            return Task.FromResult(0);
        }
    }

    class FakeClass
    {

    }
}