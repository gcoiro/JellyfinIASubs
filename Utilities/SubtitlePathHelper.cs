using System.IO;
using System.Text.RegularExpressions;

namespace JellyfinIASubs.Utilities
{
    public static class SubtitlePathHelper
    {
        private static readonly Regex SafeLanguage = new("[^a-zA-Z0-9_-]", RegexOptions.Compiled);

        public static string Build(string mediaPath, string language, bool overwrite)
        {
            var safe = SafeLanguage.Replace(language ?? string.Empty, string.Empty);
            if (string.IsNullOrWhiteSpace(safe))
            {
                safe = "und";
            }

            var outputPath = Path.ChangeExtension(mediaPath, $"{safe}.ai.srt");
            if (!overwrite && File.Exists(outputPath))
            {
                return outputPath;
            }

            return outputPath;
        }
    }
}
