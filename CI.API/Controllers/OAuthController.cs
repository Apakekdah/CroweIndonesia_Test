﻿using CI.Commands.API;
using CI.Model.Models;
using Hero.Core.Interfaces;
using Hero.IoC;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Ride.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    [EnableCors]
    public class OAuthController : ControllerBase
    {
        private readonly IDisposableIoC life;
        private readonly IMappingObject map;

        public OAuthController(IDisposableIoC life)
        {
            this.life = life;
            map = life.GetInstance<IMappingObject>();
        }

        [HttpPost("authorize")]

        [HttpPost]
        public async Task<IActionResult> Login(CancellationToken cancellation)
        {
            var authValue = Request.Headers["Authorization"];
            if (authValue.Count < 1)
            {
                return BadRequest();
            }
            OAuthForm oForm = new OAuthForm();
            await TryUpdateModelAsync(oForm);
            OAuthFake oauth = null;
            var splits = authValue.ToString().Split(' ', 2, System.StringSplitOptions.RemoveEmptyEntries);
            if (splits.Length == 2)
            {
                var basicInfo = splits[1];
                var fromBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(basicInfo));
                var userInfo = fromBase64.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
                if (userInfo.Length == 2)
                {
                    IEnumerable<string> roles = (oForm.scope ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    var invoker = life.GetInstance<ICommandInvoker<AuthenticateCommand, AuthenticateResponse>>();
                    using (var cmd = new AuthenticateCommand
                    {
                        User = WebUtility.UrlDecode(userInfo[0]),
                        Password = WebUtility.UrlDecode(userInfo[1]),
                        Roles = roles
                    })
                    {
                        var result = (await invoker.Invoke(cmd, cancellation));
                        if (result.Success)
                        {
                            var now = DateTime.Now;
                            var expIn = result.Result.ExpiredAt ?? now.AddDays(1);

                            oauth = new OAuthFake();
                            oauth.access_token = result.Result.Token;
                            oauth.expires_in = (int)(expIn - now).TotalSeconds;
                            oauth.scope = oForm.scope ?? "";
                        }
                        else
                        {
                            return Unauthorized();
                        }
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return BadRequest();
            }
            return Ok(oauth);
        }
    }

    class OAuthFake
    {
        public string access_token { get; set; }
        public string token_type { get; set; } = "Bearer";
        public int expires_in { get; set; }
        //public string refresh_token { get; set; }
        public string scope { get; set; }
    }

    class OAuthForm
    {
        public string grant_type { get; set; }
        public string scope { get; set; }
    }
}