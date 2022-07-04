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
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Atlas,Admin")]
    public class UserController
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public UserController(IDisposableIoC life)
        {
            this.life = life;
            map = life.GetInstance<IMappingObject>();
        }

        [HttpPost]
        public async Task<IActionResult> PostUser(User model, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<UserCommand, User>>();
            using (var cmd = map.Get<UserCommand>(model, (src, dest) => dest.CommandProcessor = Commands.CommandProcessor.Add))
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(User model, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<UserCommand, User>>();
            using (var cmd = map.Get<UserCommand>(model, (src, dest) => dest.CommandProcessor = Commands.CommandProcessor.Add))
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<UserCommand, User>>();
            using (var cmd = new UserCommand { UserID = userId, CommandProcessor = Commands.CommandProcessor.Delete })
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }
    }
}