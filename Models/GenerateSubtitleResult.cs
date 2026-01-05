using System;

namespace JellyfinIASubs.Models
{
    public sealed class GenerateSubtitleResult
    {
        public Guid ItemId { get; set; }

        public string Language { get; set; } = string.Empty;

        public string SubtitlePath { get; set; } = string.Empty;

        public string Provider { get; set; } = string.Empty;
    }
}
