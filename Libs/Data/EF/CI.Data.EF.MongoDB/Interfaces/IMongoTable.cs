using MongoDB.Driver;

namespace CI.Data.EF.MongoDB.Interfaces
{
    interface IMongoTable<T> where T : class
    {
        IClientSessionHandle Session { get; }
        IMongoCollection<T> Collection { get; }
    }
}