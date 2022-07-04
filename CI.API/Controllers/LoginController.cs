using CI.Commands.API;
using CI.Model.Models;
using Hero.Core.Interfaces;
using Hero.IoC;
using Microsoft.AspNetCore.Mvc;
using Ride.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class LoginController
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public LoginController(IDisposableIoC life)
        {
            this.life = life;
            map = life.GetInstance<IMappingObject>();
        }

        [HttpPost]
        public async Task<IActionResult> Login(AuthenticateUser model, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<AuthenticateCommand, AuthenticateResponse>>();
            using (var cmd = map.Get<AuthenticateCommand>(model))
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpGet("GetToken")]
        public async Task<IActionResult> GetDefaultUser(CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<AuthenticateCommand, AuthenticateResponse>>();
            using (var cmd = new AuthenticateCommand
            {
                User = "Rudi",
                Password = "123"
            })
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        //[HttpDelete("{userId}")]
        //public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellation)
        //{
        //    var invoker = Life.GetInstance<ICommandInvoker<UserCommand, User>>();
        //    using (var cmd = new UserCommand { UserID = userId, CommandProcessor = Commands.CommandProcessor.Delete })
        //    {
        //        return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
        //    }
        //}
    }
}
