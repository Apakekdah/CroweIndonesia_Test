using CI.Interface;
using CI.JWT.Models;
using CI.Model.Models;
using Hero;
using Hero.IoC;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CI.JWT
{
    public class UserAuthorization : IUserLogin, IUserLogoff
    {
        private readonly IDisposableIoC life;
        private readonly JwtConfig jwtCfg;

        public UserAuthorization(IDisposableIoC life)
        {
            this.life = life;
            jwtCfg = life.GetInstance<JwtConfig>();
        }

        public AuthenticateResponse Login(AuthenticateUser authenticate, IDictionary<string, string> properties)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(jwtCfg.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(GetClaims(authenticate.User, properties)),
                Expires = DateTime.UtcNow.AddMinutes(jwtCfg.ExpiredMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticateResponse
            {
                User = authenticate.User,
                CreateAt = DateTime.UtcNow,
                ExpiredAt = tokenDescriptor.Expires,
                Token = tokenHandler.WriteToken(token)
            };
        }

        private IEnumerable<Claim> GetClaims(string userName, IDictionary<string, string> properties)
        {
            yield return new Claim(ClaimTypes.Name, userName);

            if (!properties.IsNull())
            {
                foreach (var kvp in properties)
                {
                    yield return new Claim(kvp.Key, kvp.Value);
                }
            }
        }

        public void LogOff(string token)
        {
            throw new NotImplementedException();
        }
    }
}