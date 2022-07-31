using Bottlecap.EPaper.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bottlecap.EPaper.GoogleCloudStorage.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    private readonly IEpaperImageService _service;

    public ImageController(IEpaperImageService imageService)
    {
        _service = imageService;
    }

    [HttpGet()]
    public async Task Get([FromQuery] int height, [FromQuery] int width)
    {
        var result = await _service.GetImageAsync(new ImageQuery()
        {
            Height = height,
            Width = width
        });
        HttpContext.Response.StatusCode = 200;
        await HttpContext.Response.BodyWriter.WriteAsync(result);
    }
}
