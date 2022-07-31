using Bottlecap.EPaper.Services.ImageProviders;
using Google.Cloud.Storage.V1;

namespace Bottlecap.EPaper.GoogleCloudStorage.Services
{
    public class GoogleCloudImageProvider : IImageProvider
    {
        private readonly string _bucketName;

        public GoogleCloudImageProvider(string bucketName)
        {
            _bucketName = bucketName;
        }

        public async Task<Stream> GetImageAsync()
        {
            using (var client = StorageClient.Create())
            {
                var items = await client.ListObjectsAsync(_bucketName).ReadPageAsync(50);
                var itemIndex = new Random().Next(items.Count());
                var item = items.ToList()[itemIndex];

                var memoryStream = new MemoryStream();
                await client.DownloadObjectAsync(item, memoryStream);
                return memoryStream;
            }
        }
    }
}