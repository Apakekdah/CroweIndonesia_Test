using CI.Data.Business.Repositories;
using CI.Data.Entity;
using Hero.Core.Commons;
using Hero.Core.Interfaces;
using Hero.IoC;
using System.Threading.Tasks;

namespace CI.Seeder
{
    class UserSeeder : Disposable, IAutoStartService
    {
        private readonly IDisposableIoC life;

        public UserSeeder(IDisposableIoC life)
        {
            this.life = life.New;
        }

        public int Priority => 0;

        protected override void DisposeCore()
        {
            life.Dispose();
            base.DisposeCore();
        }

        public async Task DoWork()
        {
            var createBy = GetType().Name;

            var bll = life.GetInstance<Users>();

            User user = new User
            {
                UserID = "Rudi",
                Name = "Rudi",
                Password = "123",
                IsActive = true
            };
            if (!await bll.Exists(user.UserID))
            {
                await bll.Add(user);
            }

            user = new User
            {
                UserID = "Atlas",
                Name = "Atlas",
                Password = "secret",
                IsActive = true
            };
            if (!await bll.Exists(user.UserID))
            {
                await bll.Add(user);
            }

            await bll.Commit();
        }
    }
}