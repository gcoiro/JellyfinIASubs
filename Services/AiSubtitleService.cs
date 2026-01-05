using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JellyfinIASubs.Models;
using JellyfinIASubs.Utilities;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace JellyfinIASubs.Services
{
    public sealed class AiSubtitleService
    {
        private readonly ILibraryManager _libraryManager;
        private readonly ILogger _logger;
        private readonly AiSubtitleClient _client = new();

        public AiSubtitleService(ILibraryManager libraryManager, ILogger logger)
        {
            _libraryManager = libraryManager;
            _logger = logger;
        }

        public async Task<GenerateSubtitleResult> GenerateAsync(GenerateSubtitleRequest request, CancellationToken cancellationToken)
        {
            var config = Plugin.Instance?.Configuration ?? new PluginConfiguration();
            var language = string.IsNullOrWhiteSpace(request.Language) ? config.DefaultLanguage : request.Language;
            var endpointOptions = BuildEndpointOptions(config);

            var item = _libraryManager.GetItemById(request.ItemId);
            if (item == null || string.IsNullOrWhiteSpace(item.Path))
            {
                throw new InvalidOperationException("Media item not found or has no path");
            }

            var outputPath = SubtitlePathHelper.Build(item.Path, language, config.OverwriteExisting);
            if (!config.OverwriteExisting && File.Exists(outputPath))
            {
                return new GenerateSubtitleResult
                {
                    ItemId = request.ItemId,
                    Language = language,
                    SubtitlePath = outputPath,
                    Provider = "existing"
                };
            }

            var aiRequest = new AiSubtitleRequest
            {
                MediaPath = item.Path,
                Language = language,
                Model = endpointOptions.Model,
                Provider = config.Provider.ToString(),
                Metadata = BuildMetadata(item)
            };

            var response = await _client.GenerateAsync(aiRequest, endpointOptions, cancellationToken).ConfigureAwait(false);

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? string.Empty);
            await File.WriteAllTextAsync(outputPath, response.Srt, new UTF8Encoding(false), cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("Generated AI subtitles for {Path} -> {Output}", item.Path, outputPath);

            return new GenerateSubtitleResult
            {
                ItemId = request.ItemId,
                Language = language,
                SubtitlePath = outputPath,
                Provider = response.Provider
            };
        }

        private static AiEndpointOptions BuildEndpointOptions(PluginConfiguration config)
        {
            var options = new AiEndpointOptions
            {
                BaseUrl = config.BaseUrl ?? string.Empty,
                GeneratePath = config.GeneratePath ?? string.Empty,
                ApiKey = config.ApiKey ?? string.Empty,
                UseBearerAuth = config.UseBearerAuth,
                Model = config.Model ?? string.Empty,
                TimeoutSeconds = config.TimeoutSeconds
            };

            switch (config.Provider)
            {
                case AiProvider.Local:
                    options.BaseUrl = string.IsNullOrWhiteSpace(config.Local.Host) ? options.BaseUrl : config.Local.Host;
                    options.ApiKey = config.Local.Token ?? string.Empty;
                    options.Model = string.IsNullOrWhiteSpace(config.Local.Model) ? options.Model : config.Local.Model;
                    break;
                case AiProvider.OpenAi:
                    options.ApiKey = config.OpenAi.ApiKey ?? string.Empty;
                    options.Model = string.IsNullOrWhiteSpace(config.OpenAi.Model) ? options.Model : config.OpenAi.Model;
                    break;
                case AiProvider.AzureOpenAi:
                    options.ApiKey = config.AzureOpenAi.ApiKey ?? string.Empty;
                    options.Model = string.IsNullOrWhiteSpace(config.AzureOpenAi.Deployment) ? options.Model : config.AzureOpenAi.Deployment;
                    break;
                case AiProvider.Aws:
                    options.ApiKey = config.Aws.SecretAccessKey ?? string.Empty;
                    options.Model = string.IsNullOrWhiteSpace(config.Aws.Model) ? options.Model : config.Aws.Model;
                    break;
                case AiProvider.Custom:
                default:
                    break;
            }

            if (string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                throw new InvalidOperationException("AI subtitle endpoint is not configured for the selected provider.");
            }

            return options;
        }

        private static Dictionary<string, string> BuildMetadata(MediaBrowser.Controller.Entities.BaseItem item)
        {
            var metadata = new Dictionary<string, string>
            {
                ["name"] = item.Name ?? string.Empty,
                ["type"] = item.GetType().Name,
                ["premiereDate"] = item.PremiereDate?.ToString("O") ?? string.Empty
            };

            if (!string.IsNullOrWhiteSpace(item.ProductionYear?.ToString()))
            {
                metadata["productionYear"] = item.ProductionYear?.ToString() ?? string.Empty;
            }

            return metadata;
        }
    }
}
