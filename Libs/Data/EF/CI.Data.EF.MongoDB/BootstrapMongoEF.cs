using CI.Data.EF.MongoDB.Contexts;
using CI.Data.EF.MongoDB.Interfaces;
using CI.Data.Entity;
using Hero.Core.Interfaces;
using Hero.IoC;
using MongoDB.Bson.Serialization.Conventions;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BT = MongoDB.Bson.BsonType;

namespace CI.Data.EF.MongoDB
{
    public class BootstrapMongoEF : IBootstrapLoader<IBuilderIoC>
    {
        internal const string TABLE_MAPS = "tableMaps";

        private void MongoDBConvetion()
        {
            var convention = new ConventionPack
            {
                new EnumRepresentationConvention(BT.String),
                new CamelCaseElementNameConvention()
            };
            ConventionRegistry.Register("All", convention, f => true);
        }

        public Task Run(IBuilderIoC container)
        {
            container.Register<IMongoContext, MongoContext>(ScopeIoC.Lifetime);
            container.RegisterAsImplement<DbFactoryES>(ScopeIoC.Lifetime);
            container.RegisterAsImplement<UoW>(ScopeIoC.Lifetime);

            // Register repository
            container
                .RegisterAsImplement<RepositoryBase<User>>(ScopeIoC.Lifetime)
                .RegisterAsImplement<RepositoryBase<MeetingEvent>>(ScopeIoC.Lifetime)
                ;

            container.RegisterGeneric(typeof(MongoTable<>), typeof(IMongoTable<>));

            container.RegisterAsSelf<ConcurrentBag<string>>(TABLE_MAPS, ScopeIoC.Singleton);

            MongoDBConvetion();

            container.Register(fn =>
            {
                return new MongoTabelRunning();
            }, ScopeIoC.Singleton);

            return Task.FromResult(0);
        }

        public Task Stop()
        {
            return Task.FromResult(0);
        }
    }
}