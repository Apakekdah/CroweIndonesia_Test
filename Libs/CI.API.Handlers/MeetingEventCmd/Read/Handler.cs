using CI.Commands.API;
using CI.Data.Business.Repositories;
using CI.Data.Entity;
using Hero;
using Hero.IoC;
using Ride.Handlers.Handlers;
using Ride.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Handlers.MeetingEventCmd.Read
{
    public partial class Handler : CommandHandlerBase<MeetingEventCommandRA, IEnumerable<Model.Models.MeetingEvent>>
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public Handler(Config config) :
            base(config)
        {
            life = config.Life;
            map = life.GetInstance<IMappingObject>();
        }

        public override Task<bool> Validate(MeetingEventCommandRA command)
        {
            //var user = User.GetActiveUser();
            //if (user.IsNullOrEmptyOrWhitespace())
            //{
            //    throw new NullReferenceException("Can't found active user in session");
            //}

            //if (command.PageSize < 1)
            //    command.PageSize = Extensions.DefaultPageSize;
            //if (command.Page < 1)
            //    command.Page = 1;

            return Task.FromResult(true);
        }

        public override async Task<IEnumerable<Model.Models.MeetingEvent>> Execute(MeetingEventCommandRA command, CancellationToken cancellation)
        {
            var activeUser = User.GetActiveUser();

            using (var scope = life.New)
            {
                //var bllUser = scope.GetInstance<Users>();
                //if (!await bllUser.IsUserOk(activeUser))
                //{
                //    throw new Exception("Active user is not valid");
                //}

                var bll = scope.GetInstance<MeetingEvents>();

                MeetingEvent meetingEvent;
                IEnumerable<MeetingEvent> meetingEvents = null;

                switch (command.CommandProcessor)
                {
                    case Commands.CommandProcessor.GetAll:
                        meetingEvents = await bll.GetAllLimit();
                        break;
                    case Commands.CommandProcessor.GetOne:
                        meetingEvent = await bll.GetByMeetingEventID(command.Id);
                        if (!meetingEvent.IsNull())
                        {
                            meetingEvents = new[] { meetingEvent };
                        }
                        break;
                    default:
                        throw new Exception($"Unsupported Processing '{command.CommandProcessor}' in command {Name}");
                }

                if (meetingEvents.IsNull())
                    return null;

                return map.Get<List<Model.Models.MeetingEvent>>(meetingEvents);
            }
        }
    }
}