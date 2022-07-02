namespace CI.JWT.Models
{
    public class JwtConfig
    {
        public static readonly string SectionName = "JwtConfig";

        public string Key { get; set; }
        public int ExpiredMinutes { get; set; } = 240;
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}