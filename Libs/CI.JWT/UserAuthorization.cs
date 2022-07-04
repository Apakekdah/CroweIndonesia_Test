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
            var expireIn = DateTime.UtcNow.AddMinutes(jwtCfg.ExpiredMinutes);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = expireIn,
                Subject = new ClaimsIdentity(GetClaims(authenticate.User, authenticate.Roles, expireIn, properties)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenValue = tokenHandler.WriteToken(token);

            return new AuthenticateResponse
            {
                User = authenticate.User,
                CreateAt = DateTime.UtcNow,
                ExpiredAt = tokenDescriptor.Expires,
                Token = tokenValue
            };
        }

        private IEnumerable<Claim> GetClaims(string userName, IEnumerable<string> roles, DateTime expireIn, IDictionary<string, string> properties)
        {
            yield return new Claim(ClaimTypes.Name, userName);
            yield return new Claim(ClaimTypes.Expiration, expireIn.ToString(), typeof(DateTime).Name);

            if (!roles.IsNullOrEmpty())
            {
                foreach (var role in roles)
                {
                    yield return new Claim(ClaimTypes.Role, role);
                }
            }

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