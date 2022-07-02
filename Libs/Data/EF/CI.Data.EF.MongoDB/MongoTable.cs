using MongoDB.Driver;
using CI.Data.EF.MongoDB.Interfaces;

namespace CI.Data.EF.MongoDB
{
    class MongoTable<T> : IMongoTable<T>
        where T : class
    {
        public MongoTable(IClientSessionHandle session, IMongoCollection<T> collection)
        {
            Session = session;
            Collection = collection;
        }

        public IClientSessionHandle Session { get; }
        public IMongoCollection<T> Collection { get; }
    }
}