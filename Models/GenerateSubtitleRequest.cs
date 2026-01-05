using System;

namespace JellyfinIASubs.Models
{
    public sealed class GenerateSubtitleRequest
    {
        public Guid ItemId { get; set; }

        public string Language { get; set; } = string.Empty;
    }
}
