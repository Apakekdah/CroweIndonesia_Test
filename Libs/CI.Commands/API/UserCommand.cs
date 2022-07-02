using System.ComponentModel.DataAnnotations;

namespace CI.Commands.API
{
    public class UserCommand : BaseCommand
    {
        [Required]
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
    }
}