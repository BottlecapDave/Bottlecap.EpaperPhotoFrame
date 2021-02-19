using System.Threading.Tasks;

namespace Bottlecap.EPaper.Services
{
    public interface IEpaperImageService
    {
        Task<byte[]> GetImageAsync(ImageQuery query);
    }
}
