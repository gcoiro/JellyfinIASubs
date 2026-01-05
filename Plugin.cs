using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace JellyfinIASubs
{
    public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public static Plugin? Instance { get; private set; }

        public override string Name => "Jellyfin AI Subs";

        public override Guid Id => new Guid("c6a0d0f7-59f0-4b08-9f86-5f7f3a0b2f6a");

        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            var assemblyName = GetType().Assembly.GetName().Name;
            return new[]
            {
                new PluginPageInfo
                {
                    Name = "config",
                    DisplayName = "AI Subtitles",
                    EmbeddedResourcePath = $"{assemblyName}.Configuration.configPage.html",
                    EnableInMainMenu = true,
                    MenuSection = "Plugins",
                    MenuIcon = "settings"
                }
            };
        }
    }
}
