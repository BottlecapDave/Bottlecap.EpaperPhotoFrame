using Bottlecap.EPaper.Services.ImageProviders;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace Bottlecap.EPaper.GoogleCloudStorage.Services
{
    public class GoogleCloudImageProvider : IImageProvider
    {
        private readonly string _bucketName;

        private readonly GoogleCredential _credentials;

        public GoogleCloudImageProvider(string bucketName, GoogleCredential credentials)
        {
            _bucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
            _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        public async Task<Stream> GetImageAsync()
        {
            using (var client = StorageClient.Create(_credentials))
            {
                var items = await client.ListObjectsAsync(_bucketName).ReadPageAsync(50);
                var itemIndex = new Random().Next(items.Count());
                var item = items.ToList()[itemIndex];

                var memoryStream = new MemoryStream();
                await client.DownloadObjectAsync(item, memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}