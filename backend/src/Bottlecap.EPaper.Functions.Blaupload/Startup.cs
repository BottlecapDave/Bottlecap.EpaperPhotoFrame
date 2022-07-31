using Bottlecap.EPaper.Functions.Blaupload.Providers;
using Bottlecap.EPaper.Services;
using Bottlecap.EPaper.Services.ImageProviders;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bottlecap.EPaper.Functions.Blaupload
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
            services
                .AddSingleton<IImageProvider>((serviceProvider) => {
                    string blauploadBaseUrl = Environment.GetEnvironmentVariable("BLAUPLOAD_BASE_URL");
                    string blauploadToken = Environment.GetEnvironmentVariable("BLAUPLOAD_TOKEN");
                    if (String.IsNullOrEmpty(blauploadBaseUrl))
                    {
                        throw new SystemException("BLAUPLOAD_BASE_URL is not set");
                    }
                    else if (String.IsNullOrEmpty(blauploadToken))
                    {
                        throw new SystemException("BLAUPLOAD_TOKEN is not set");
                    }

                    return new BlauploadImageProvider(blauploadBaseUrl, blauploadToken);
                })
            .AddSingleton<IEpaperImageService, EPaperImageService>();
    }
}
