using Bottlecap.EPaper.Functions.Unsplash.Providers;
using Bottlecap.EPaper.Services;
using Bottlecap.EPaper.Services.ImageProviders;
using Google.Cloud.Functions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Bottlecap.EPaper.Functions.Unsplash
{
    public class Startup : FunctionsStartup
    {
        public override void ConfigureServices(WebHostBuilderContext context, IServiceCollection services) =>
            services
                .AddSingleton<IImageProvider>((serviceProvider) => {
                    string accessToken = Environment.GetEnvironmentVariable("UNSPLASH_ACCESS_TOKEN");
                    string query = Environment.GetEnvironmentVariable("UNSPLASH_QUERY");
                    string orientation = Environment.GetEnvironmentVariable("UNSPLASH_ORIENTATION");
                    if (String.IsNullOrEmpty(accessToken))
                    {
                        throw new SystemException("UNSPLASH_ACCESS_TOKEN is not set");
                    }
                    else if (String.IsNullOrEmpty(query))
                    {
                        throw new SystemException("UNSPLASH_QUERY is not set");
                    }
                    else if (String.IsNullOrEmpty(orientation))
                    {
                        throw new SystemException("UNSPLASH_ORIENTATION is not set");
                    }

                    return new UnsplashImageProvider(accessToken, query, orientation);
                })
            .AddSingleton<IEpaperImageService, EPaperImageService>();
    }
}
