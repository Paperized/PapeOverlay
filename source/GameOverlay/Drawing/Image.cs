using System;
using System.IO;

using SharpDX.WIC;
using SharpDX.Direct2D1;

using GameOverlay.Drawing.Imaging;

using Bitmap = SharpDX.Direct2D1.Bitmap;
using System.Collections.Generic;

namespace GameOverlay.Drawing
{
    /// <summary>
    /// Represents an Image which can be drawn using a Graphics surface.
    /// </summary>
    public abstract class AbstractImage : IDisposable
    {
        internal static readonly ImagingFactory ImageFactory = new ImagingFactory();

        /// <summary>
        /// Gets the width of this Image
        /// </summary>
        public abstract float Width { get; set; }

        /// <summary>
        /// Gets the height of this Image
        /// </summary>
        public abstract float Height { get; set; }

        /// <summary>
        /// Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        ~AbstractImage() => Dispose(false);


        protected bool disposedValue = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by this Image.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected abstract void Dispose(bool disposing);
    }

    public class Image : AbstractImage
    {
        /// <summary>
        /// The SharpDX Bitmap
        /// </summary>
        public Bitmap Bitmap;

        /// <summary>
        /// Gets the width of this Image
        /// </summary>
        public override float Width { get => Bitmap.PixelSize.Width; set => new NotImplementedException(); }

        /// <summary>
        /// Gets the height of this Image
        /// </summary>
        public override float Height { get => Bitmap.PixelSize.Height; set => new NotImplementedException(); }

        private Image()
        {
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a byte[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="bytes">A byte[] containing image data.</param>
        public Image(RenderTarget device, byte[] bytes)
            => Bitmap = LoadBitmapFromMemory(device, bytes);

        /// <summary>
        /// Initializes a new Image for the given device by using a file on disk.
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="path">The path to an image file on disk.</param>
        public Image(RenderTarget device, string path)
            => Bitmap = LoadBitmapFromFile(device, path);

        /// <summary>
        /// Initializes a new Image for the given device by using a byte[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="bytes">A byte[] containing image data.</param>
        public Image(Graphics device, byte[] bytes) : this(device.GetRenderTarget(), bytes)
        {
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a file on disk.
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="path">The path to an image file on disk.</param>
        public Image(Graphics device, string path) : this(device.GetRenderTarget(), path)
        {
        }

        /// <summary>
        /// Returns a value indicating whether this instance and a specified <see cref="T:System.Object" /> represent the same type and value.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="obj" /> is a Image and equal to this instance; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is Image image)
            {
                return image.Bitmap.NativePointer == Bitmap.NativePointer;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Image represent the same value.
        /// </summary>
        /// <param name="value">An object to compare to this instance.</param>
        /// <returns><see langword="true" /> if <paramref name="value" /> is equal to this instance; otherwise, <see langword="false" />.</returns>
        public bool Equals(Image value)
        {
            return value != null
                && value.Bitmap.NativePointer == Bitmap.NativePointer;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return OverrideHelper.HashCodes(
                Bitmap.NativePointer.GetHashCode());
        }

        /// <summary>
        /// Converts this Image instance to a human-readable string.
        /// </summary>
        /// <returns>A string representation of this Image.</returns>
        public override string ToString()
        {
            return OverrideHelper.ToString(
                "Image", "Bitmap",
                "Width", Width.ToString(),
                "Height", Height.ToString(),
                "PixelFormat", Bitmap.PixelFormat.Format.ToString());
        }

        #region IDisposable Support

        /// <summary>
        /// Releases all resources used by this Image.
        /// </summary>
        /// <param name="disposing">A Boolean value indicating whether this is called from the destructor.</param>
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                Bitmap?.Dispose();

                disposedValue = true;
            }
        }
        #endregion

        /// <summary>
        /// Converts an Image to a SharpDX Bitmap.
        /// </summary>
        /// <param name="image">The Image object.</param>
        public static implicit operator Bitmap(Image image)
        {
            if (image == null) throw new ArgumentNullException(nameof(image));

            return image.Bitmap;
        }

        /// <summary>
        /// Returns a value indicating whether two specified instances of Image represent the same value.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns> <see langword="true" /> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <see langword="false" />.</returns>
        public static bool Equals(AbstractImage left, AbstractImage right)
        {
            return left?.Equals(right) == true;
        }

        private static Bitmap LoadBitmapFromMemory(RenderTarget device, byte[] bytes)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentOutOfRangeException(nameof(bytes));

            Bitmap bmp = null;
            MemoryStream stream = null;
            BitmapDecoder decoder = null;
            FormatConverter converter = null;

            try
            {
                stream = new MemoryStream(bytes);
                decoder = new BitmapDecoder(ImageFactory, stream, DecodeOptions.CacheOnDemand);
                bmp = ImageDecoder.Decode(device, decoder);

                decoder.Dispose();
                stream.Dispose();

                return bmp;
            }
            catch
            {
                if (converter?.IsDisposed == false) converter.Dispose();
                if (decoder?.IsDisposed == false) decoder.Dispose();
                if (stream != null) TryCatch(() => stream.Dispose());
                if (bmp?.IsDisposed == false) bmp.Dispose();

                throw;
            }
        }

        private static Bitmap LoadBitmapFromFile(RenderTarget device, string path) => LoadBitmapFromMemory(device, File.ReadAllBytes(path));

        private static void TryCatch(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            try
            {
                action();
            }
            catch { }
        }
    }

    public enum FrameImageDisposal
    {
        Unspecified,
        NotDispose,
        RestoreBackground,
        RestorePrevious
    }

    public class FrameImage
    {
        public Bitmap Frame;
        public float Width { get; set; }
        public float Height { get; set; }
        public Point TopLeft;
        public float Delay;
        public FrameImageDisposal frameDisposal;

        public bool useLocalBackgroundColor;
        public Color localBackgroundColor = Color.Transparent;

        public Rectangle Rectangle => new Rectangle(TopLeft.X, TopLeft.Y, TopLeft.X + Width, TopLeft.Y + Height);
    }

    public class GIFImage : AbstractImage
    {
        public IBrush bgBrush;

        public Color globalBackgroundColor = Color.Transparent;

        public List<FrameImage> frameImages = new List<FrameImage>();

        public override float Width { get; set; }

        public override float Height { get; set; }

        public bool useGlobalBackgroundColor;

        private GIFImage()
        {

        }

        ~GIFImage()
        {
            //bgBrush.Dispose();
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a byte[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="bytes">A byte[] containing image data.</param>
        public GIFImage(RenderTarget device, byte[] bytes)
            => LoadGIFFromMemory(device, bytes);

        /// <summary>
        /// Initializes a new Image for the given device by using a file on disk.
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="path">The path to an image file on disk.</param>
        public GIFImage(RenderTarget device, string path)
            => LoadGIFFromMemory(device, path);

        /// <summary>
        /// Initializes a new Image for the given device by using a byte[].
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="bytes">A byte[] containing image data.</param>
        public GIFImage(Graphics device, byte[] bytes) : this(device.GetRenderTarget(), bytes)
        {
        }

        /// <summary>
        /// Initializes a new Image for the given device by using a file on disk.
        /// </summary>
        /// <param name="device">The Graphics device.</param>
        /// <param name="path">The path to an image file on disk.</param>
        public GIFImage(Graphics device, string path) : this(device.GetRenderTarget(), path)
        {
        }

        private void LoadGIFFromMemory(RenderTarget device, byte[] bytes)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Length == 0) throw new ArgumentOutOfRangeException(nameof(bytes));

            Bitmap bmp = null;
            MemoryStream stream = null;
            BitmapDecoder decoder = null;
            FormatConverter converter = null;

            try
            {
                stream = new MemoryStream(bytes);
                decoder = new BitmapDecoder(ImageFactory, stream, DecodeOptions.CacheOnDemand);
                ImageDecoder.DecodeGIF(device, decoder, this);

                decoder.Dispose();
                stream.Dispose();
            }
            catch
            {
                if (converter?.IsDisposed == false) converter.Dispose();
                if (decoder?.IsDisposed == false) decoder.Dispose();
                if (stream != null) TryCatch(() => stream.Dispose());
                if (bmp?.IsDisposed == false) bmp.Dispose();

                throw;
            }
        }

        private void LoadGIFFromMemory(RenderTarget device, string path) => LoadGIFFromMemory(device, File.ReadAllBytes(path));

        private static void TryCatch(Action action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            try
            {
                action();
            }
            catch { }
        }


        protected override void Dispose(bool disposing)
        {
            if(!disposedValue)
            {
                foreach (var frame in frameImages) frame?.Frame?.Dispose();

                disposedValue = true;
            }
        }
    }
}
