
using Bottlecap.EPaper.Services.ImageProviders;
using Microsoft.SyndicationFeed;
using Microsoft.SyndicationFeed.Rss;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Bottlecap.EPaper.Functions.Unsplash.Providers
{
    public class UnsplashImageProvider : IImageProvider
    {
        public const string UNSPLASH_URL = "https://api.unsplash.com/photos/random";

        private readonly string _accessKey;
        private readonly string _query;
        private readonly string _orientation;

        public UnsplashImageProvider(string accessKey, string query, string orientation)
        {
            _accessKey = accessKey;
            _query = query;
            _orientation = orientation;
        }

        public async Task<byte[]> GetImageAsync()
        {
            WebClient client = new WebClient();
            client.Headers.Add("Authorization", $"Client-ID {_accessKey}");

            // Get random photo
            var rawResult = await client.DownloadStringTaskAsync($"{UNSPLASH_URL}?query={_query}&orientation={_orientation}");
            var result = JsonConvert.DeserializeObject<UnsplashPhoto>(rawResult);

            // Download image
            return await client.DownloadDataTaskAsync(result.Urls.Small);
        }
    }
}
