using MediaBrowser.Model.Plugins;

namespace JellyfinIASubs
{
    public enum AiProvider
    {
        Local,
        OpenAi,
        AzureOpenAi,
        Aws,
        Custom
    }

    public sealed class PluginConfiguration : BasePluginConfiguration
    {
        public AiProvider Provider { get; set; } = AiProvider.Local;

        public string BaseUrl { get; set; } = "http://localhost:8080";

        public string GeneratePath { get; set; } = "/v1/subtitles";

        public string ApiKey { get; set; } = string.Empty;

        public bool UseBearerAuth { get; set; } = true;

        public string Model { get; set; } = "";

        public string DefaultLanguage { get; set; } = "en";

        public int TimeoutSeconds { get; set; } = 180;

        public bool OverwriteExisting { get; set; } = false;

        public LocalProviderSettings Local { get; set; } = new();

        public OpenAiProviderSettings OpenAi { get; set; } = new();

        public AzureOpenAiProviderSettings AzureOpenAi { get; set; } = new();

        public AwsProviderSettings Aws { get; set; } = new();
    }

    public sealed class LocalProviderSettings
    {
        public string Host { get; set; } = "http://localhost:8080";

        public string Token { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
    }

    public sealed class OpenAiProviderSettings
    {
        public string ApiKey { get; set; } = string.Empty;

        public string Model { get; set; } = "gpt-4o-mini-transcribe";
    }

    public sealed class AzureOpenAiProviderSettings
    {
        public string Endpoint { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;

        public string Deployment { get; set; } = string.Empty;
    }

    public sealed class AwsProviderSettings
    {
        public string AccessKeyId { get; set; } = string.Empty;

        public string SecretAccessKey { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;
    }
}
