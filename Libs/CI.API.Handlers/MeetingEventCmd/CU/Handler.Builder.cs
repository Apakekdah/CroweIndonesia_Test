﻿using CI.Commands.API;
using CI.Model.Models;
using Ride.Handlers.Handlers;
using System;

namespace CI.API.Handlers.MeetingEventCmd.CU
{
    public partial class Handler
    {
        public const string DEFAULT_NAME = "MeetingEventUpdateDelete";

        public static Builder CreateBuilder()
        {
            return new Builder(DEFAULT_NAME, c => new Handler(c));
        }

        public class Builder : BuilderBase<Builder>
        {
            private readonly string name;
            private readonly Func<Config, Handler> factory;

            public Builder(string name, Func<Config, Handler> factory)
            {
                this.name = name;
                this.factory = factory;
            }

            protected override string DefaultName => name;

            public override CommandHandlerBase<MeetingEventCommandCU, MeetingEvent> Build()
            {
                return factory(GetConfig());
            }
        }
    }
}