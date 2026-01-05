using System.Collections.Generic;

namespace JellyfinIASubs.Models
{
    public sealed class AiSubtitleRequest
    {
        public string MediaPath { get; set; } = string.Empty;

        public string Language { get; set; } = string.Empty;

        public string Model { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;

        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
