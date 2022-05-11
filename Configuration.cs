namespace Blog;

public static class Configuration
{
    public static string JwtKey { get; set; } = "TFAuQmxvZy5iYTY4MWE0YS02NDdmLTQ5MGUtYTBiMi0yOTU3NWJiMWU3NDU=";

    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
