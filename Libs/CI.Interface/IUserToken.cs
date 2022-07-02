using CI.Model.Models;
using System.Collections.Generic;

namespace CI.Interface
{
    public interface IUserToken
    {
        AuthenticateResponse RefreshToken(string oldToken, IDictionary<string, object> properties);
    }
}