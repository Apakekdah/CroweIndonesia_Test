using CI.Commands.API;
using CI.Data.Business.Repositories;
using Hero;
using Hero.IoC;
using Ride.Handlers.Handlers;
using Ride.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Handlers.UserCmd.CUD
{
    public partial class Handler : CommandHandlerBase<UserCommand, Model.Models.User>
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public Handler(Config config) :
            base(config)
        {
            life = config.Life;
            map = life.GetInstance<IMappingObject>();
        }

        public override async Task<bool> Validate(UserCommand command)
        {
            using (var scope = life.New)
            {
                var bll = scope.GetInstance<Users>();

                switch (command.CommandProcessor)
                {
                    case Commands.CommandProcessor.Add:
                        if (await bll.Exists(command.UserID))
                        {
                            throw new Exception($"UserID '{command.UserID}' already exists");
                        }
                        goto case Commands.CommandProcessor.Edit;
                    case Commands.CommandProcessor.Edit:
                        if (command.Password.IsNullOrEmpty())
                        {
                            throw new Exception($"Password can't be empty");
                        }
                        if (command.Name.IsNullOrEmptyOrWhitespace())
                        {
                            throw new Exception($"Name can't be left blank or empty space");
                        }
                        break;
                }
            }

            return true;
        }

        public override async Task<Model.Models.User> Execute(UserCommand command, CancellationToken cancellation)
        {
            using (var scope = life.New)
            {
                var bll = scope.GetInstance<Users>();

                Data.Entity.User userEntity;

                switch (command.CommandProcessor)
                {
                    case Commands.CommandProcessor.Add:
                        userEntity = new Data.Entity.User
                        {
                            UserID = command.UserID,
                            Name = command.Name,
                            Password = command.Password,
                            IsActive = true
                        };
                        break;
                    case Commands.CommandProcessor.Delete:
                    case Commands.CommandProcessor.Edit:
                        userEntity = await bll.GetUser(command.UserID);
                        break;
                    default:
                        throw new Exception($"Unsupported Processing '{command.CommandProcessor}' in command {Name}");
                }

                if (userEntity.IsNull())
                {
                    throw new Exception($"Failed to read data for '{command.CommandProcessor}' with Id '{command.UserID}'");
                }

                userEntity.Name = command.Name;
                userEntity.Password = command.Password;

                switch (command.CommandProcessor)
                {
                    case Commands.CommandProcessor.Add:
                        await bll.Add(userEntity);
                        break;
                    case Commands.CommandProcessor.Delete:
                        await bll.Delete(userEntity);
                        break;
                    default:
                        await bll.Update(userEntity);
                        break;
                }

                var changes = await bll.Commit();

                if (changes < 1)
                    throw new Exception($"Failed to save processing '{command.CommandProcessor}' with ID '{command.UserID}'");

                return map.Get<Model.Models.User>(userEntity);
            }
        }
    }
}