using CI.Commands.API;
using CI.Model.Domain;
using CI.Model.Models;
using Hero.Core.Interfaces;
using Hero.IoC;
using Microsoft.AspNetCore.Mvc;
using Ride.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Atlas")]
    public class MeetingEventController : BasePaginatorController
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public MeetingEventController(IDisposableIoC life)
        {
            this.life = life;
            map = life.GetInstance<IMappingObject>();
        }

        [HttpPost]
        public async Task<IActionResult> Post(MeetingEventDomain model, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<MeetingEventCommandCU, MeetingEvent>>();
            using (var cmd = map.Get<MeetingEventCommandCU>(model, (src, dest) => dest.CommandProcessor = Commands.CommandProcessor.Add))
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(MeetingEventDomain model, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<MeetingEventCommandCU, MeetingEvent>>();
            using (var cmd = map.Get<MeetingEventCommandCU>(model, (src, dest) => dest.CommandProcessor = Commands.CommandProcessor.Edit))
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<MeetingEventCommandD, bool>>();
            using (var cmd = new MeetingEventCommandD { Id = id })
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<MeetingEventCommandRA, IEnumerable<MeetingEventDomain>>>();
            using (var cmd = new MeetingEventCommandRA { CommandProcessor = Commands.CommandProcessor.GetOne, Id = id })
            {
                return (await invoker.Invoke(cmd, cancellation)).ToContentJson();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page, [FromQuery] int pageSize, CancellationToken cancellation)
        {
            var invoker = life.GetInstance<ICommandInvoker<MeetingEventCommandRA, IEnumerable<MeetingEvent>>>();
            using (var cmd = new MeetingEventCommandRA())
            {
                return await CreatePaginationInvoker<ICommandInvoker<MeetingEventCommandRA, IEnumerable<MeetingEvent>>, MeetingEventCommandRA, MeetingEvent>(invoker, cmd, cancellation);
            }
        }
    }
}
