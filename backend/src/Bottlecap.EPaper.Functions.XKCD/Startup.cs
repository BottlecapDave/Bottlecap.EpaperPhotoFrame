using Bottlecap.EPaper.Functions.XKCD.Providers;
using Bottlecap.EPaper.Services;
using Bottlecap.EPaper.Services.ImageProviders;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Bottlecap.EPaper.Functions.XKCD
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
            services
                .AddSingleton<IImageProvider, XKCDImageProvider>()
            .AddSingleton<IEpaperImageService, EPaperImageService>();
    }
}
