namespace Chatter.Security.API.Options
{
    public class CorsOptions
    {
        public const string ConfigurationSectionName = "Cors";
        public const string PolicyName = "ChatterCors";
        public const string AllowAllOption = "*";

        public string[]? AllowedOrigins { get; set; }

        public string[]? AllowedHeaders { get; set; }

        public string[]? AllowedMethods { get; set; }
    }
}
