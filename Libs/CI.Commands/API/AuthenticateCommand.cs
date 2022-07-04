using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CI.Commands.API
{
    public class AuthenticateCommand : BaseCommand
    {
        [Required]
        public string User { get; set; }
        [Required]
        public string Password { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}