namespace JellyfinIASubs.Models
{
    public sealed class AiEndpointOptions
    {
        public string BaseUrl { get; set; } = string.Empty;

        public string GeneratePath { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public bool UseBearerAuth { get; set; } = true;

        public string Model { get; set; } = string.Empty;

        public int TimeoutSeconds { get; set; } = 180;
    }
}
