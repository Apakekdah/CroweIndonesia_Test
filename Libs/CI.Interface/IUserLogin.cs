using CI.Model.Models;
using System.Collections.Generic;

namespace CI.Interface
{
    public interface IUserLogin
    {
        AuthenticateResponse Login(AuthenticateUser authenticate, IDictionary<string, string> properties);
    }
}