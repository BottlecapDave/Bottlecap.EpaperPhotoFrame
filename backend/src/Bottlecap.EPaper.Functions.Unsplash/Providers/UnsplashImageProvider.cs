
using Bottlecap.EPaper.Services.ImageProviders;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<Stream> GetImageAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Client-ID {_accessKey}");

                // Get random photo
                var rawResult = await client.GetStringAsync($"{UNSPLASH_URL}?query={_query}&orientation={_orientation}");
                var result = JsonConvert.DeserializeObject<UnsplashPhoto>(rawResult);

                // Download image
                return await client.GetStreamAsync(result.Urls.Small);
            }
        }
    }
}
