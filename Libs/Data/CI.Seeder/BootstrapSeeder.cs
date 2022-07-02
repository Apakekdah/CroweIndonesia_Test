using Hero.Core.Interfaces;
using Hero.IoC;
using System.Threading.Tasks;

namespace CI.Seeder
{
    public class BootstrapSeeder : IBootstrapLoader<IBuilderIoC>
    {
        public Task Run(IBuilderIoC container)
        {
            container.RegisterAsImplement<MeetingEventSeeder>();

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