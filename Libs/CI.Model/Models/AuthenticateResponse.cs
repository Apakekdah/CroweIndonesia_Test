using System;

namespace CI.Model.Models
{
    public class AuthenticateResponse
    {
        public string Token { get; set; }
        public string User { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
}