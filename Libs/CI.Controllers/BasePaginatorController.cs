#undef USE_SESSION

using CI.Commons;
using CI.Interface;
using Hero;
using Hero.Core.Interfaces;
using Hero.IoC;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace CI
{
    public class BasePaginatorController : ControllerBase
    {
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
            PaginationHelper pagination = new PaginationHelper();

            ICommandResult<IEnumerable<TResult>> commandResult = null;
            ICache cache = null;
            string key = null;
            int start = 0;

            if (await TryUpdateModelAsync(pagination))
            {
                if (pagination.Page == 0)
                    pagination.Page = 1;

                if (pagination.PageSize == 0)
                    pagination.PageSize = DefaultPageSize;

#if USE_SESSION
                if ((HttpContext.Session != null) && !pagination.IsReset)
#else
                if (!pagination.IsReset)
#endif
                {
#if USE_SESSION
                    key = string.Concat(invoker.Name, "-", HttpContext.Session.Id);
#else
                    key = string.Concat(invoker.Name, "-", User.GetActiveUser());
#endif
                    var life = HttpContext.RequestServices.GetService(typeof(IDisposableIoC)) as IDisposableIoC;
                    if (!life.IsNull())
                    {
                        if (life.IsRegistered<ICache>())
                        {
                            cache = life.GetInstance<ICache>();

                            if (pagination.IsReset)
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
            }
            else
            {
                pagination.Page = 1;
                pagination.PageSize = Extensions.DefaultPageSize;
                pagination.Reset = bool.TrueString;
            }

            start = ((pagination.Page < 1) ? 0 : pagination.Page - 1) * pagination.PageSize;

            if (commandResult.IsNull() || pagination.IsReset)
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
                    var paging = commandResult.Result.Skip(start).Take(pagination.PageSize).ToArray().AsEnumerable();
                    commandResult = new CommandResultWithCount<IEnumerable<TResult>>(true, paging, commandResult.Result.Count())
                    {
                        Page = pagination.Page,
                        PageSize = pagination.PageSize
                    };
                }
            }

            return commandResult.ToContentJson();
        }
    }

    class PaginationHelper : IPaginator
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string Reset { get; set; }
        public bool IsReset => (Reset.IsNullOrEmptyOrWhitespace() ? false : Regex.IsMatch(Reset, "\\b(yes|on|true|1)\\b", RegexOptions.IgnoreCase | RegexOptions.Multiline));
    }
}