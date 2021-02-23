using Bottlecap.EPaper.Services.ImageProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bottlecap.EPaper.Services
{
    public class EPaperImageService : IEpaperImageService
    {
        private readonly IImageProvider _imageProvider;

        public EPaperImageService(IImageProvider imageProvider)
        {
            _imageProvider = imageProvider;
        }

        public async Task<byte[]> GetImageAsync(ImageQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException(nameof(query));
            }

            var originalImage = await _imageProvider.GetImageAsync();
            using (var image = Image.Load(originalImage))
            {
                image.Mutate(x => x.Resize(new ResizeOptions()
                {
                    Mode = ResizeMode.Max,
                    Size = new Size()
                    {
                        Width = query.Width,
                        Height = query.Height
                    }
                }).Grayscale().BinaryDither(new SixLabors.ImageSharp.Processing.Processors.Dithering.OrderedDither(100)));

                using (var convertedImageStream = new MemoryStream())
                {
                    image.Save(convertedImageStream, new BmpEncoder());
                    return convertedImageStream.ToArray();
                }
            }
        }
    }
}
