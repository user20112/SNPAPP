using Emgu.CV;
using Emgu.CV.Structure;
using SkiaSharp;
using SkiaSharp.Views.UWP;
using System.Drawing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Graphics.DirectX.Direct3D11;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace ResumeApp.UWP.InterfaceImplimentations
{
    internal class Convert
    {
        public static SKImage Direct3dToSKImage(IDirect3DSurface surface)
        {
            var task = SoftwareBitmap.CreateCopyFromSurfaceAsync(surface);
            while (task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            using SoftwareBitmap bitmap = task.GetResults();
            SKImage image = SoftwareBitmapToSKImage(bitmap);
            return image;
        }

        public static SKImage ImageToSKImage(Image<Bgr, byte> image)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(image.Width, image.Height));
            Rectangle ROI = image.ROI;
            SKColor[] pixels = bitmap.Pixels;
            byte[,,] ImageBytes = image.Data;
            int rowOffset;
            for (int y = 0; y < image.Height; y++)
            {
                rowOffset = (y * image.Width);
                for (int x = 0; x < image.Width; x++)
                {
                    pixels[x + rowOffset] = new SKColor(ImageBytes[y + ROI.Top, x + ROI.Left, 2], ImageBytes[y + ROI.Top, x + ROI.Left, 1], ImageBytes[y + ROI.Top, x + ROI.Left, 0]);
                }
            }
            bitmap.Pixels = pixels;
            return SKImage.FromBitmap(bitmap);
        }

        public static SKImage ImageToSKImage(Image<Gray, byte> image)
        {
            SKBitmap bitmap = new SKBitmap(new SKImageInfo(image.Width, image.Height));
            Rectangle ROI = image.ROI;
            SKColor[] pixels = bitmap.Pixels;
            byte[,,] ImageBytes = image.Data;
            int rowOffset;
            for (int y = 0; y < image.Height; y++)
            {
                rowOffset = (y * image.Width);
                for (int x = 0; x < image.Width; x++)
                {
                    pixels[x + rowOffset] = new SKColor(ImageBytes[y + ROI.Top, x + ROI.Left, 0], ImageBytes[y + ROI.Top, x + ROI.Left, 0], ImageBytes[y + ROI.Top, x + ROI.Left, 0]);
                }
            }
            bitmap.Pixels = pixels;
            return SKImage.FromBitmap(bitmap);
        }

        public static Image<Bgr, byte> SKImageToImage(SKImage image)
        {
            SKBitmap bitmap = SKBitmap.FromImage(image);
            SKColor[] pixels = bitmap.Pixels;
            Image<Bgr, byte> Image = new Image<Bgr, byte>(image.Width, image.Height);
            byte[,,] ImageBytes = Image.Data;
            int rowOffset;
            for (int y = 0; y < image.Height; y++)
            {
                rowOffset = (y * image.Width);
                for (int x = 0; x < image.Width; x++)
                {
                    ImageBytes[y, x, 0] = pixels[x + rowOffset].Blue;
                    ImageBytes[y, x, 1] = pixels[x + rowOffset].Green;
                    ImageBytes[y, x, 2] = pixels[x + rowOffset].Red;
                }
            }
            Image.Data = ImageBytes;
            return Image;
        }

        public static SKImage SoftwareBitmapToSKImage(SoftwareBitmap bitmap)
        {
            using InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            var task = BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, randomAccessStream);
            while (task.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            BitmapEncoder encoder = task.GetResults();
            encoder.SetSoftwareBitmap(SoftwareBitmap.Convert(bitmap, BitmapPixelFormat.Rgba8));
            try
            {
                var Task = encoder.FlushAsync();
                while (Task.Status == AsyncStatus.Started)
                    Thread.Sleep(50);
            }
            catch
            {
            }
            byte[] array = new byte[randomAccessStream.Size];
            var ReadTask = randomAccessStream.ReadAsync(array.AsBuffer(), (uint)randomAccessStream.Size, InputStreamOptions.None);
            while (ReadTask.Status == AsyncStatus.Started)
                Thread.Sleep(50);
            return SKImage.FromEncodedData(array);
        }

        public static SKImage WriteableBitmapToSKImage(WriteableBitmap bitmap)
        {
            return bitmap.ToSKImage();
        }

        internal static Image<Gray, byte> SKImageToGrayImage(SKImage image)
        {
            SKBitmap bitmap = SKBitmap.FromImage(image);
            SKColor[] pixels = bitmap.Pixels;
            Image<Gray, byte> Image = new Image<Gray, byte>(image.Width, image.Height);
            byte[,,] ImageBytes = Image.Data;
            int rowOffset;
            for (int y = 0; y < image.Height; y++)
            {
                rowOffset = (y * image.Width);
                for (int x = 0; x < image.Width; x++)
                {
                    ImageBytes[y, x, 0] = (byte)((pixels[x + rowOffset].Blue + pixels[x + rowOffset].Red + pixels[x + rowOffset].Green) / 3);
                }
            }
            Image.Data = ImageBytes;
            return Image;
        }
    }
}