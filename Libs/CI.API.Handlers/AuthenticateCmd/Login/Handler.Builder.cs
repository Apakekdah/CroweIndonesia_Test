using CI.Commands.API;
using Ride.Handlers.Handlers;
using System;

namespace CI.API.Handlers.AuthenticateCmd.Login
{
    public partial class Handler
    {
        public const string DEFAULT_NAME = "UserCRUD";

        public static Builder CreateBuilder()
        {
            return new Builder(DEFAULT_NAME, c => new Handler(c));
        }

        public class Builder : BuilderBase<Builder>
        {
            private readonly string name;
            private readonly Func<HandlerConfig, Handler> factory;
            private int maxFailed;

            public Builder(string name, Func<HandlerConfig, Handler> factory)
            {
                this.name = name;
                this.factory = factory;
            }

            protected override string DefaultName => name;

            public override CommandHandlerBase<AuthenticateCommand, Model.Models.AuthenticateResponse> Build()
            {
                return factory(GetConfig());
            }

            public Builder WithMaxFailedLogin(int maxFailed)
            {
                this.maxFailed = maxFailed;
                return this;
            }


            new private HandlerConfig GetConfig()
            {
                return new HandlerConfig(Name ?? DefaultName)
                {
                    Life = Life,
                    MaxFailed = maxFailed,
                    ExecutionFilters = ExecutionFilters,
                    MaxRetries = MaxRetries,
                    ResultFilter = ResultFilter,
                };
            }
        }
    }
}