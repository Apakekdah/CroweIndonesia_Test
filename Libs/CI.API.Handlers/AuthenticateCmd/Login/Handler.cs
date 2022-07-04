using CI.Commands.API;
using CI.Data.Business.Repositories;
using CI.Interface;
using Hero;
using Hero.IoC;
using Ride.Handlers.Handlers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Handlers.AuthenticateCmd.Login
{
    public partial class Handler : CommandHandlerBase<AuthenticateCommand, Model.Models.AuthenticateResponse>
    {
        private readonly IDisposableIoC life;
        private readonly int maxFailed;
        private readonly IUserLogin login;

        public Handler(HandlerConfig config) :
            base(config)
        {
            life = config.Life;
            maxFailed = config.MaxFailed;
            login = life.GetInstance<IUserLogin>();
        }

        public override async Task<Model.Models.AuthenticateResponse> Execute(AuthenticateCommand command, CancellationToken cancellation)
        {
            using (var scope = life.New)
            {
                var bll = scope.GetInstance<Users>();

                var userEntity = await bll.GetById(command.User);

                if (userEntity.IsNull())
                    throw new Exception($"User '{command.User}' not exists");
                else if (!userEntity.IsActive)
                    throw new Exception($"User '{command.User}' not active");
                else if (!command.Password.Equals(userEntity.Password))
                {
                    if (userEntity.Counter > maxFailed)
                    {
                        throw new Exception($"User '{command.User}' is blocked");
                    }

                    try
                    {
                        userEntity.Counter++;

                        await bll.Update(userEntity);

                        await bll.Commit();
                    }
                    catch { }

                    throw new Exception($"User '{command.User}' invalid password");
                }

                try
                {
                    userEntity.Counter = 0;

                    await bll.Update(userEntity);

                    await bll.Commit();
                }
                catch { }

                return login.Login(new Model.Models.AuthenticateUser
                {
                    User = command.User,
                    Password = command.Password,
                    Roles = command.Roles
                }, new Dictionary<string, string>());
            }
        }
    }
}