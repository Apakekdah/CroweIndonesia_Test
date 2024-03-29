﻿using CI.Commands.API;
using CI.Data.Business.Repositories;
using CI.Data.Entity;
using Hero;
using Hero.IoC;
using Ride.Handlers.Handlers;
using Ride.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Handlers.MeetingEventCmd.CU
{
    public partial class Handler : CommandHandlerBase<MeetingEventCommandCU, Model.Models.MeetingEvent>
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public Handler(Config config) :
            base(config)
        {
            life = config.Life;
            map = life.GetInstance<IMappingObject>();
        }

        public override Task<bool> Validate(MeetingEventCommandCU command)
        {
            //var activeUser = User.GetActiveUser();
            //if (activeUser.IsNullOrEmptyOrWhitespace())
            //{
            //    throw new NullReferenceException("Can't found active user in session");
            //}

            if (command.CommandProcessor == Commands.CommandProcessor.Edit)
            {
                if (command.Id.IsNullOrEmptyOrWhitespace())
                {
                    throw new NullReferenceException("Tender ID is required");
                }
            }
            return Task.FromResult(true);
        }

        public override async Task<Model.Models.MeetingEvent> Execute(MeetingEventCommandCU command, CancellationToken cancellation)
        {
            var activeUser = User.GetActiveUser();

            // Simple to show log
            Log.Debug($"User executed : {activeUser}, Process : {command.CommandProcessor}, Name : {command.Name}");

            using (var scope = life.New)
            {
                //var bllUser = scope.GetInstance<Users>();
                //if (!await bllUser.IsUserOk(activeUser))
                //{
                //    throw new Exception("Active user is not valid");
                //}

                var bll = scope.GetInstance<MeetingEvents>();

                MeetingEvent meetingEvent;

                switch (command.CommandProcessor)
                {
                    case Commands.CommandProcessor.Add:
                        meetingEvent = new MeetingEvent()
                        {
                            Id = Guid.NewGuid().ToString(),
                            IsActive = true
                        };
                        break;
                    case Commands.CommandProcessor.Edit:
                        meetingEvent = await bll.GetByMeetingEventID(command.Id);
                        break;
                    default:
                        throw new Exception($"Unsupported Processing '{command.CommandProcessor}' in command {Name}");
                }

                if (meetingEvent.IsNull())
                {
                    throw new Exception($"Failed to read data for '{command.CommandProcessor}' with Id '{command.Id}'");
                }

                meetingEvent.Name = command.Name;
                meetingEvent.Description = command.Description ?? command.Name;
                meetingEvent.StartDate = command.StartDate;
                meetingEvent.EndDate = command.EndDate;
                meetingEvent.IsActive = command.IsActive;

                if (command.CommandProcessor == Commands.CommandProcessor.Add)
                {
                    meetingEvent.CreateBy = command.CreateBy;
                    meetingEvent.CreateDate = command.CreateDate;
                }

                meetingEvent.ModifyBy = command.CreateBy;
                meetingEvent.ModifyDate = command.CreateDate;

                switch (command.CommandProcessor)
                {
                    case Commands.CommandProcessor.Add:
                        await bll.Add(meetingEvent);
                        break;
                    default:
                        await bll.Update(meetingEvent);
                        break;
                }

                var changes = await bll.Commit();

                if (changes > 0)
                {
                    return map.Get<Model.Models.MeetingEvent>(meetingEvent);
                }
                return null;
            }
        }
    }
}