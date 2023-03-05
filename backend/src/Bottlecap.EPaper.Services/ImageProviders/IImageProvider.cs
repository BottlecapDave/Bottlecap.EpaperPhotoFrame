using System.IO;
using System.Threading.Tasks;

namespace Bottlecap.EPaper.Services.ImageProviders
{
    public interface IImageProvider
    {
        /// <summary>
        /// Retrieve the id of the image to retrieve.
        /// </summary>
        /// <param name="query">The query to restrict the retrieved image id to</param>
        /// <returns>The id of the chosed image</returns>
        Task<string> GetImageIdAsync(ImageQuery query);

        /// <summary>
        /// Retrieve the image that matches the specified query, if available.
        /// </summary>
        /// <param name="id">The id of the image to retrieve</param>
        /// <param name="query">The query the image must match</param>
        /// <returns>The stream of the image; Null otherwise.</returns>
        Task<Stream> GetImageAsync(string id, ImageQuery query);

        /// <summary>
        /// Retrieve the content of the image.
        /// </summary>
        /// <param name="id">The id of the image to retrieve</param>
        /// <returns>The stream of the image</returns>
        Task<Stream> GetImageAsync(string id);

        /// <summary>
        /// Save the provided image based on the provided id and image query
        /// </summary>
        /// <param name="content">The content of the image to save</param>
        /// <param name="id">The id of the original image</param>
        /// <param name="query">The query that was used to restrict the content</param>
        Task SaveImageAsync(Stream content, string id, ImageQuery query);
    }
}
