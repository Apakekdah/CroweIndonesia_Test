using Hero.Core.Interfaces;
using Hero.IoC;
using System.Threading.Tasks;

namespace CI.Data.Business
{
    public class BootstrapBusiness : IBootstrapLoader<IBuilderIoC>
    {
        public Task Run(IBuilderIoC container)
        {
            var asm = typeof(FakeClass).Assembly;
            container.RegisterAssemblyTypes(RegistrationTypeIoC.AsLook,
                new[] { typeof(Hero.Business.IBusinessClassAsync<>) }, ScopeIoC.Lifetime,
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