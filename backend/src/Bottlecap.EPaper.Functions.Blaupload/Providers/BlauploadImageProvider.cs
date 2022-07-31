using Bottlecap.EPaper.Services.ImageProviders;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bottlecap.EPaper.Functions.Blaupload.Providers
{
    public class BlauploadImageProvider : IImageProvider
    {
        private readonly string _baseUrl;
        private readonly string _password;

        public BlauploadImageProvider(string baseUrl, string password)
        {
            _baseUrl = baseUrl;
            _password = password;
        }

        public async Task<Stream> GetImageAsync()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add(HttpRequestHeader.Cookie.ToString(), $"password={_password}");
                var itemsResponse = await client.GetStringAsync(new Uri($"{_baseUrl}/?last=50&format=json"));
                var items = JsonConvert.DeserializeObject<BlauploadItem[]>(itemsResponse);
                items = items.Where(x => x.type == "image2").ToArray();

                return await client.GetStreamAsync($"{_baseUrl}/{items[new Random().Next(items.Length)].filename}");
            }
        }
    }
}
