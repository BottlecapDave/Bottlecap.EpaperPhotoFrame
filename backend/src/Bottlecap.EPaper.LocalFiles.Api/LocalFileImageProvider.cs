using Bottlecap.EPaper.Services;
using Bottlecap.EPaper.Services.ImageProviders;

namespace Bottlecap.EPaper.LocalFiles.Services
{
    public class LocalFileImageProvider : IImageProvider
    {
        private readonly string _fileDirectory;

        private readonly string _cacheFileDirectory;

        public LocalFileImageProvider(string fileDirectory)
        {
            _fileDirectory = fileDirectory;
            _cacheFileDirectory = Path.Join(_fileDirectory, "_cache");
            if (Directory.Exists(_cacheFileDirectory) == false)
            {
                Directory.CreateDirectory(_cacheFileDirectory);
            }
        }

        /// <summary>
        /// Retrieve the id of the image to retrieve.
        /// </summary>
        /// <param name="query">The query to restrict the retrieved image id to</param>
        /// <returns>The id of the chosed image</returns>
        public Task<string> GetImageIdAsync(ImageQuery query)
        {
            var files = Directory.GetFiles(_fileDirectory);
            var fileIndex = DateTime.UtcNow.DayOfYear % files.Count();
            var file = files[fileIndex];
            return Task.FromResult(Path.GetFileName(file));
        }

        /// <summary>
        /// Retrieve the image that matches the specified query, if available.
        /// </summary>
        /// <param name="id">The id of the image to retrieve</param>
        /// <param name="query">The query the image must match</param>
        /// <returns>The stream of the image; Null otherwise.</returns>
        public async Task<Stream?> GetImageAsync(string id, ImageQuery query)
        {
            var filepath = GetCachedFilepath(id, query);
            if (File.Exists(filepath) == false)
            {
                return null;
            }

            var memoryStream = new MemoryStream();
            var fileContent = await File.ReadAllBytesAsync(filepath);
            await memoryStream.WriteAsync(fileContent, 0, fileContent.Length);
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Retrieve the content of the image.
        /// </summary>
        /// <param name="id">The id of the image to retrieve</param>
        /// <returns>The stream of the image</returns>
        public async Task<Stream> GetImageAsync(string id)
        {
            var memoryStream = new MemoryStream();
            var fileContent = await File.ReadAllBytesAsync(Path.Join(_fileDirectory, id));
            await memoryStream.WriteAsync(fileContent, 0, fileContent.Length);
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// Save the provided image based on the provided id and image query
        /// </summary>
        /// <param name="content">The content of the image to save</param>
        /// <param name="id">The id of the original image</param>
        /// <param name="query">The query that was used to restrict the content</param>
        public async Task SaveImageAsync(Stream content, string id, ImageQuery query)
        {
            using (var memoryStream = new MemoryStream())
            {
                content.CopyTo(memoryStream);
                memoryStream.Position = 0;
                await File.WriteAllBytesAsync(GetCachedFilepath(id, query), memoryStream.ToArray());
            }
        }

        private string GetCachedFilepath(string id, ImageQuery query)
        {
            return Path.Join(_cacheFileDirectory, $"{id}_{query.Width}_{query.Height}");
        }
    }
}