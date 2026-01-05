using System;
using System.Threading;
using System.Threading.Tasks;
using JellyfinIASubs.Models;
using JellyfinIASubs.Services;
using MediaBrowser.Controller.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JellyfinIASubs.Controllers
{
    [ApiController]
    [Route("AISubs")]
    public sealed class AiSubtitleController : ControllerBase
    {
        private readonly AiSubtitleService _service;

        public AiSubtitleController(ILibraryManager libraryManager, ILogger<AiSubtitleController> logger)
        {
            _service = new AiSubtitleService(libraryManager, logger);
        }

        [HttpPost("Generate")]
        public async Task<ActionResult<GenerateSubtitleResult>> Generate([FromBody] GenerateSubtitleRequest request, CancellationToken cancellationToken)
        {
            if (request == null || request.ItemId == Guid.Empty)
            {
                return BadRequest("itemId is required");
            }

            var result = await _service.GenerateAsync(request, cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
