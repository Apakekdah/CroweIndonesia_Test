using Hero.Core.Interfaces;
using Hero.IoC;
using Ride.Handlers;
using Ride.Handlers.Filters;
using System.Threading.Tasks;

namespace CI.API.Handlers
{
    public class BootstrapApiHandlers : IBootstrapLoader<IBuilderIoC>
    {
        public Task Run(IBuilderIoC container)
        {
            // Register Authentication Filter 
            container.RegisterAssemblyTypes(RegistrationTypeIoC.AsImplement, null, ScopeIoC.Singleton, true, new[]
            {
                typeof(IAuthenticationFilter),

            }, typeof(BootstrapApiHandlers).Assembly);

            // Register Pre Invoke Filter
            container.RegisterAssemblyTypes(RegistrationTypeIoC.AsLook, null, ScopeIoC.Singleton, true, new[]
            {
                typeof(IPreInvocationFilter<>),

            }, typeof(BootstrapApiHandlers).Assembly);

            /// ############## MeetingEvent ##############
            container.Register(c => MeetingEventCmd.CU.Handler.CreateBuilder()
                .WithLife(c).Build().CreateInvoker(c), ScopeIoC.Singleton);
            container.Register(c => MeetingEventCmd.D.Handler.CreateBuilder()
                .WithLife(c).Build().CreateInvoker(c), ScopeIoC.Singleton);
            container.Register(c => MeetingEventCmd.Read.Handler.CreateBuilder()
                .WithLife(c).Build().CreateInvoker(c), ScopeIoC.Singleton);

            /// ############## User ##############
            container.Register(c => UserCmd.CUD.Handler.CreateBuilder()
                .WithLife(c).Build().CreateInvoker(c), ScopeIoC.Singleton);

            /// ############## Authorization ##############
            container.Register(c => AuthenticateCmd.Login.Handler.CreateBuilder()
                .WithMaxFailedLogin(3).WithLife(c).Build().CreateInvoker(c), ScopeIoC.Singleton);

            return Task.FromResult(0);
        }

        public Task Stop()
        {
            return Task.FromResult(0);
        }
    }
}