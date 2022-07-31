using System.IO;
using System.Threading.Tasks;

namespace Bottlecap.EPaper.Services.ImageProviders
{
    public interface IImageProvider
    {
        Task<Stream> GetImageAsync();
    }
}
