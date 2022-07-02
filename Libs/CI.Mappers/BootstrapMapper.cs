using CI.MapperRegistration;
using AutoMapper;
using Hero;
using Hero.Core.Interfaces;
using Hero.IoC;
using Ride.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CI.Mappers
{
    public class BootstrapMapper : IBootstrapLoader<IBuilderIoC>
    {
        public Task Run(IBuilderIoC container)
        {
            container.Register(c =>
            {
                ICollection<Profile> profiles = new HashSet<Profile>
                {
                    new ApiMapperProfile()
                };

                if (c.IsRegistered<IMapperConfigurator>())
                {
                    foreach (var p in c.GetInstance<IEnumerable<IMapperConfigurator>>())
                    {
                        var rm = new AISMapper();
                        p.SetMapper(rm);
                        profiles.Add(rm);
                    }
                }

                var mapConfig = new MapperConfiguration(mc =>
                {
                    profiles.Each(p => mc.AddProfile(p));
                });

                return mapConfig.CreateMapper();
            }, ScopeIoC.Singleton);

            container.Register<IMappingObject, MappingObject>(ScopeIoC.Singleton);
            return Task.FromResult(0);
        }

        public Task Stop()
        {
            return Task.FromResult(0);
        }

        internal class AISMapper : Profile
        {

        }
    }
}