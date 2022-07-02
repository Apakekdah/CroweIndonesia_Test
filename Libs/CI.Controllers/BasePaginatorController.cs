#undef USE_SESSION

using Hero;
using Hero.Core.Commons;
using Hero.Core.Interfaces;
using Hero.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CI
{
    public class BasePaginatorController : ControllerBase
    {
        protected const string PAGE_NAME = "Page";
        protected const string PAGE_SIZE_NAME = "PageSize";
        protected const string PAGE_RESET_NAME = "Reset";

        public BasePaginatorController() : this(Extensions.DefaultPageSize) { }
        public BasePaginatorController(int defaultPageSize)
        {
            DefaultPageSize = defaultPageSize;
        }

        protected int DefaultPageSize { get; set; }

        protected async Task<IActionResult> CreatePaginationInvoker<TInvoker, TCommand, TResult>(TInvoker invoker, TCommand command, CancellationToken cancellation)
            where TInvoker : ICommandInvoker<TCommand, IEnumerable<TResult>>
            where TCommand : ICommand
        {
            var query = Request.Query;


            int page, pageSize;
            bool reset;

            if (query.TryGetValue(PAGE_NAME, out StringValues sValue))
            {
                int.TryParse(sValue.ToString(), out page);
            }
            else
            {
                page = 1;
            }

            if (query.TryGetValue(PAGE_SIZE_NAME, out sValue))
            {
                int.TryParse(sValue.ToString(), out pageSize);
            }
            else
            {
                pageSize = DefaultPageSize;
            }

            if (query.TryGetValue(PAGE_RESET_NAME, out sValue))
            {
                reset = Regex.IsMatch(sValue, "\\b(yes|on|true|1)\\b", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            }
            else
            {
                reset = true;
            }

            ICommandResult<IEnumerable<TResult>> commandResult = null;
            ICache cache = null;
            string key = null;

            int start = ((page < 1) ? 0 : page - 1) * pageSize;
#if USE_SESSION
            if (HttpContext.Session != null)
#endif
            {
#if USE_SESSION
                key = string.Concat(invoker.Name, "-", HttpContext.Session.Id);
#else
                key = invoker.Name;
#endif
                var life = HttpContext.RequestServices.GetService(typeof(IDisposableIoC)) as IDisposableIoC;
                if (!life.IsNull())
                {
                    if (life.IsRegistered<ICache>())
                    {
                        cache = life.GetInstance<ICache>();
                        IDictionary<string, (string key, string value)> dictionary = new ConcurrentDictionary<string, (string key, string value)>();

                        if (reset)
                        {
                            await cache.Remove(key);
                        }
                        else
                        {
                            commandResult = (await cache.Get(key)) as ICommandResult<IEnumerable<TResult>>;
                        }
                    }
                }
            }

            if (commandResult.IsNull() || reset)
            {
                commandResult = await invoker.Invoke(command, cancellation);

                if (commandResult.Success && commandResult.Result.Any() && !cache.IsNull())
                {
                    await cache.Add(key, commandResult, TimeSpan.FromMinutes(5));
                }
            }

            if (commandResult != null)
            {
                if (commandResult.Success && commandResult.Result.Any())
                {
                    var paging = commandResult.Result.Skip(start).Take(pageSize).ToArray().AsEnumerable();
                    commandResult = new CommandResult<IEnumerable<TResult>>(true, paging);
                }
            }

            return commandResult.ToContentJson();
        }
    }
}