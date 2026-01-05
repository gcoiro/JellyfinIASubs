# Jellyfin AI Subs

Generate subtitles for Jellyfin media items by calling your own AI model endpoint.

## What this plugin does
- Adds an API endpoint to request subtitle generation for a media item.
- Sends the media file path to your AI service along with language and metadata.
- Writes an SRT file next to the media file (e.g. `Movie.en.ai.srt`).

## Configuration
Edit plugin config once installed:
- `Provider`: which AI backend to use (`Local`, `OpenAi`, `AzureOpenAi`, `Aws`, `Custom`)
- `DefaultLanguage`: used when request language is empty
- `TimeoutSeconds`: HTTP timeout
- `OverwriteExisting`: overwrite existing `.ai.srt`

Common (used by `Custom` or advanced scenarios):
- `BaseUrl`: AI service base URL (example: `http://localhost:8080`)
- `GeneratePath`: relative path for generation (example: `/v1/subtitles`)
- `ApiKey`: optional key sent as `Authorization`
- `UseBearerAuth`: if true, sends `Authorization: Bearer {ApiKey}`
- `Model`: model name, optional

Local provider:
- `Local.Host`: local service URL (example: `http://192.168.1.50:8080`)
- `Local.Token`: optional token
- `Local.Model`: optional model name

OpenAI provider:
- `OpenAi.ApiKey`
- `OpenAi.Model`
Note: set `BaseUrl` to your OpenAI-compatible subtitle gateway if you're not using `Local`.

Azure OpenAI provider:
- `AzureOpenAi.Endpoint`
- `AzureOpenAi.ApiKey`
- `AzureOpenAi.Deployment`
Note: set `BaseUrl` to your Azure-compatible subtitle gateway if you're not using `Local`.

AWS provider:
- `Aws.AccessKeyId`
- `Aws.SecretAccessKey`
- `Aws.Region`
- `Aws.Model`
Note: set `BaseUrl` to your AWS-compatible subtitle gateway if you're not using `Local`.

## Installation (any Jellyfin instance)
You can install the plugin either from a ZIP release or by building from source.

### Option A: Install from a ZIP release
1. Download the latest plugin ZIP (contains the `.dll`) from your release page.
2. In Jellyfin, go to `Dashboard` -> `Plugins`.
3. Choose `Catalog` -> `...` -> `Install from URL`, and paste the direct URL to the ZIP.
4. Restart Jellyfin.
5. Go to `Dashboard` -> `Plugins` -> `My Plugins` and confirm `Jellyfin AI Subs` is listed.

### Option B: Build and install from source
1. Build the plugin:
   - `dotnet publish JellyfinIASubs.csproj -c Release -o out`
2. Copy the output DLLs into the Jellyfin plugin folder:
   - Linux: `/var/lib/jellyfin/plugins/Jellyfin.AISubtitles/`
   - Windows: `%ProgramData%\\Jellyfin\\Server\\plugins\\Jellyfin.AISubtitles\\`
   - Docker: `/config/plugins/Jellyfin.AISubtitles/`
3. Restart Jellyfin.
4. Confirm the plugin appears in `Dashboard` -> `Plugins`.

### Notes for Docker/Kubernetes
- The plugin directory is inside your Jellyfin `/config` volume at `/config/plugins/`.
- If you run with a non-root user, ensure ownership matches the Jellyfin user.
- Do not copy Jellyfin server assemblies (like `MediaBrowser.*.dll`) into the plugin folder.

## API
`POST /AISubs/Generate`

Request:
```
{
  "itemId": "<guid>",
  "language": "es"
}
```

Response:
```
{
  "itemId": "<guid>",
  "language": "es",
  "subtitlePath": "C:\\Media\\Movie.es.ai.srt",
  "provider": "my-ai"
}
```

## AI service contract
The plugin expects your AI service to accept:
```
{
  "mediaPath": "C:\\Media\\Movie.mkv",
  "language": "es",
  "model": "",
  "metadata": {
    "name": "Movie",
    "type": "Movie",
    "premiereDate": "2024-01-01T00:00:00.0000000Z",
    "productionYear": "2024"
  }
}
```

And return:
```
{
  "srt": "1\n00:00:00,000 --> 00:00:01,000\nHola\n",
  "provider": "my-ai"
}
```

## Notes
- Your AI service must be able to access the media path.
- This plugin does not extract audio; the AI service is responsible for reading/transcoding the file.
- Add a UI configuration page if you want in-server config editing.
