using System.Collections.Generic;

namespace CI.Model.Models
{
    public class AuthenticateUser
    {
        public string User { get; set; }
        public string Password { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}