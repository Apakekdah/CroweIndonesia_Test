using CI.Data.EF.MongoDB.Contexts;
using CI.Data.Entity;

namespace CI.Data.EF.MongoDB.Configure
{
    class UserDataConfig : MongoTableConfigurator<User>
    {
    }
}