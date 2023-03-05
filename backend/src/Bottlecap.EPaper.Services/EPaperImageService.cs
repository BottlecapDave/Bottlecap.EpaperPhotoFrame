using Bottlecap.EPaper.Services.ImageProviders;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
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

            var imageId = await _imageProvider.GetImageIdAsync(query);

            using (var cachedImage = await _imageProvider.GetImageAsync(imageId, query))
            {
                if (cachedImage != null)
                {
                    using (var convertedImageStream = new MemoryStream())
                    {
                        cachedImage.CopyTo(convertedImageStream);
                        convertedImageStream.Position = 0;
                        return convertedImageStream.ToArray();
                    }
                }
            }

            using (var originalImage = await _imageProvider.GetImageAsync(imageId))
            {
                using (var image = Image.Load(originalImage))
                {
                    image.Mutate(x => x.Resize(new ResizeOptions()
                    {
                        Mode = ResizeMode.Pad,
                        Size = new Size()
                        {
                            Width = query.Width,
                            Height = query.Height
                        }
                    }).Grayscale().BinaryDither(new SixLabors.ImageSharp.Processing.Processors.Dithering.OrderedDither(100)));

                    using (var convertedImageStream = new MemoryStream())
                    {
                        image.Save(convertedImageStream, new BmpEncoder());
                        convertedImageStream.Position = 0;
                        await _imageProvider.SaveImageAsync(convertedImageStream, imageId, query);
                        return convertedImageStream.ToArray();
                    }
                }
            }
        }
    }
}
