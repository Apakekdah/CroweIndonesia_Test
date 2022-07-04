using CI.Commands.API;
using CI.Data.Business.Repositories;
using CI.Data.Entity;
using Hero;
using Hero.IoC;
using Ride.Handlers.Handlers;
using Ride.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Handlers.MeetingEventCmd.D
{
    public partial class Handler : CommandHandlerBase<MeetingEventCommandD, bool>
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public Handler(Config config) :
            base(config)
        {
            life = config.Life;
            map = life.GetInstance<IMappingObject>();
        }

        public override Task<bool> Validate(MeetingEventCommandD command)
        {
            //var user = User.GetActiveUser();
            //if (user.IsNullOrEmptyOrWhitespace())
            //{
            //    throw new NullReferenceException("Can't found active user in session");
            //}
            return Task.FromResult(true);
        }

        public override async Task<bool> Execute(MeetingEventCommandD command, CancellationToken cancellation)
        {
            //var activeUser = User.GetActiveUser();

            using (var scope = life.New)
            {
                //var bllUser = scope.GetInstance<Users>();
                //if (!await bllUser.IsUserOk(activeUser))
                //{
                //    throw new Exception("Active user is not valid");
                //}

                var bll = scope.GetInstance<MeetingEvents>();

                MeetingEvent meetingEvent = await bll.GetByMeetingEventID(command.Id);

                if (meetingEvent.IsNull())
                {
                    throw new Exception($"Failed to read data for '{command.CommandProcessor}' with Id '{command.Id}'");
                }
                else if (!meetingEvent.IsActive)
                {
                    throw new Exception($"Meeting Id '{command.Id}' already mark as an inactive");
                }

                meetingEvent.IsActive = false;

                await bll.Update(meetingEvent);

                var changes = await bll.Commit();

                if (changes > 0)
                    return true;

                return false;
            }
        }
    }
}