using CI.Data.Entity;
using Hero.Business;
using Hero.Core.Interfaces;
using System.Threading.Tasks;

namespace CI.Data.Business.Repositories
{
    public class Users : BusinessClassBaseAsync<User>
    {
        public Users(IRepositoryAsync<User> repository, IUnitOfWorkAsync unitOfWork) : base(repository, unitOfWork)
        {
        }

        public Task<User> GetUser(string id)
        {
            return Get(c => c.UserID == id && c.IsActive);
        }

        public async Task<bool> IsUserOk(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            return await Exists(c => c.UserID == id && c.IsActive);
        }
    }
}