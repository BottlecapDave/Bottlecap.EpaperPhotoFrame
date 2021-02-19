using Bottlecap.EPaper.Services;
using Google.Cloud.Functions.Framework;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bottlecap.EPaper.Functions.XKCD
{
    [FunctionsStartup(typeof(Startup))]
    public class EPaperImageGet : IHttpFunction
    {
        private readonly IEpaperImageService _service;

        public EPaperImageGet(IEpaperImageService imageService)
        {
            _service = imageService;
        }

        public async Task HandleAsync(HttpContext context)
        {
            var query = new ImageQuery();
            query.Height = this.GetQueryInt(context, "height", 480);
            query.Width = this.GetQueryInt(context, "width", 800);

            var result = await _service.GetImageAsync(query);
            await context.Response.BodyWriter.WriteAsync(result);
        }

        private int GetQueryInt(HttpContext context, string key, int defaultValue)
        {
            if (context.Request.Query.TryGetValue("height", out var rawQueryValue) &&
                Int32.TryParse(rawQueryValue, out var queryValue))
            {
                return queryValue;
            }

            return defaultValue;
        }
    }
}
