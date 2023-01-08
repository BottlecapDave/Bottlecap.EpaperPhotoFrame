using Bottlecap.EPaper.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bottlecap.EPaper.GoogleCloudStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly IEpaperImageService _service;

    private readonly ApiConfig _config;

    public ImageController(IEpaperImageService imageService, ApiConfig config)
    {
        _service = imageService;
        _config = config;
    }

    [HttpGet()]
    public async Task Get([FromHeader] string apiKey, [FromQuery] int height, [FromQuery] int width)
    {
        if (apiKey != _config.ApiKey)
        {
            HttpContext.Response.StatusCode = 403;
            return;
        }

        var result = await _service.GetImageAsync(new ImageQuery()
        {
            Height = height,
            Width = width
        });
        HttpContext.Response.StatusCode = 200;
        await HttpContext.Response.BodyWriter.WriteAsync(result);
    }
}
