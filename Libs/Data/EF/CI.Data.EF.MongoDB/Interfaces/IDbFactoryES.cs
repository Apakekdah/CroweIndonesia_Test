using Hero.Core.Interfaces;

namespace CI.Data.EF.MongoDB.Interfaces
{
    interface IDbFactoryES : IDatabaseFactory<IMongoContext>
    {
    }
}