namespace Blogue;

public static class Configuration
{
    public static string JwtKey = "FaBLuCJoNhbGciOiJIUzI1NiIsInR5cCI6IkpXsCJ9";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "api_IlTevUM/z-e3NwCV/un/unWg==";
    public static SmtpConfiguration Smtp = new();
    
    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; } = 25;
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}