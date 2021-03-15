using System;
using System.Collections.Generic;

using SharpDX.Direct2D1;
using SharpDX.WIC;

using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace GameOverlay.Drawing.Imaging
{
	internal static class ImageDecoder
	{
		private static readonly Guid[] _floatingPointFormats = new Guid[]
		{
			PixelFormat.Format128bppRGBAFloat,
			PixelFormat.Format128bppRGBAFixedPoint,
			PixelFormat.Format128bppPRGBAFloat,
			PixelFormat.Format128bppRGBFloat,
			PixelFormat.Format128bppRGBFixedPoint,
			PixelFormat.Format96bppRGBFixedPoint,
			PixelFormat.Format96bppRGBFloat,
			PixelFormat.Format64bppBGRAFixedPoint,
			PixelFormat.Format64bppRGBAFixedPoint,
			PixelFormat.Format64bppRGBFixedPoint,
			PixelFormat.Format48bppRGBFixedPoint,
			PixelFormat.Format48bppBGRFixedPoint,
			PixelFormat.Format32bppGrayFixedPoint,
			PixelFormat.Format32bppGrayFloat,
			PixelFormat.Format16bppGrayFixedPoint
		};

		// PixelFormat sorted in a best compatibility and best color accuracy order
		private static readonly Guid[] _standardPixelFormats = new Guid[]
		{
			PixelFormat.Format144bpp8ChannelsAlpha,
			PixelFormat.Format128bpp8Channels,
			PixelFormat.Format128bpp7ChannelsAlpha,
			PixelFormat.Format112bpp7Channels,
			PixelFormat.Format112bpp6ChannelsAlpha,
			PixelFormat.Format96bpp6Channels,
			PixelFormat.Format96bpp5ChannelsAlpha,
			PixelFormat.Format80bpp5Channels,
			PixelFormat.Format80bppCMYKAlpha,
			PixelFormat.Format80bpp4ChannelsAlpha,
			PixelFormat.Format72bpp8ChannelsAlpha,
			PixelFormat.Format64bppBGRA,
			PixelFormat.Format64bppRGBA,
			PixelFormat.Format64bppPBGRA,
			PixelFormat.Format64bppPRGBA,
			PixelFormat.Format64bpp8Channels,
			PixelFormat.Format64bpp4Channels,
			PixelFormat.Format64bppRGBAHalf,
			PixelFormat.Format64bppPRGBAHalf,
			PixelFormat.Format64bpp7ChannelsAlpha,
			PixelFormat.Format64bpp3ChannelsAlpha,
			PixelFormat.Format64bppRGB,
			PixelFormat.Format64bppCMYK,
			PixelFormat.Format64bppRGBHalf,
			PixelFormat.Format56bpp7Channels,
			PixelFormat.Format56bpp6ChannelsAlpha,
			PixelFormat.Format48bpp6Channels,
			PixelFormat.Format48bppRGB,
			PixelFormat.Format48bppBGR,
			PixelFormat.Format48bpp3Channels,
			PixelFormat.Format48bppRGBHalf,
			PixelFormat.Format48bpp5ChannelsAlpha,
			PixelFormat.Format40bpp5Channels,
			PixelFormat.Format40bppCMYKAlpha,
			PixelFormat.Format40bpp4ChannelsAlpha,
			PixelFormat.Format32bppBGRA,
			PixelFormat.Format32bppRGBA,
			PixelFormat.Format32bppPBGRA,
			PixelFormat.Format32bppPRGBA,
			PixelFormat.Format32bppRGBA1010102,
			PixelFormat.Format32bppRGBA1010102XR,
			PixelFormat.Format32bppCMYK,
			PixelFormat.Format32bpp4Channels,
			PixelFormat.Format32bpp3ChannelsAlpha,
			PixelFormat.Format32bppBGR,
			PixelFormat.Format32bppRGB,
			PixelFormat.Format32bppRGBE,
			PixelFormat.Format32bppBGR101010,
			PixelFormat.Format24bppBGR,
			PixelFormat.Format24bppRGB,
			PixelFormat.Format24bpp3Channels,
			PixelFormat.Format16bppBGR555,
			PixelFormat.Format16bppBGR565,
			PixelFormat.Format16bppBGRA5551,
			PixelFormat.Format16bppGray,
			PixelFormat.Format16bppGrayHalf,
			PixelFormat.Format16bppCbCr,
			PixelFormat.Format16bppYQuantizedDctCoefficients,
			PixelFormat.Format16bppCbQuantizedDctCoefficients,
			PixelFormat.Format16bppCrQuantizedDctCoefficients,
			PixelFormat.Format8bppIndexed,
			PixelFormat.Format8bppAlpha,
			PixelFormat.Format8bppY,
			PixelFormat.Format8bppCb,
			PixelFormat.Format8bppCr,
			PixelFormat.Format8bppGray
		};

		private static readonly Guid[] _uncommonFormats = new Guid[]
		{
			PixelFormat.Format4bppIndexed,
			PixelFormat.Format2bppIndexed,
			PixelFormat.Format1bppIndexed,
			PixelFormat.Format4bppGray,
			PixelFormat.Format2bppGray,
			PixelFormat.FormatDontCare,
			PixelFormat.FormatBlackWhite
		};

		private static IEnumerable<Guid> _pixelFormatEnumerator
		{
			get
			{
				foreach (var format in _standardPixelFormats)
				{
					yield return format;
				}

				foreach (var format in _floatingPointFormats)
				{
					yield return format;
				}

				foreach (var format in _uncommonFormats)
				{
					yield return format;
				}
			}
		}

		private static void TryCatch(Action action)
		{
			if (action == null) throw new ArgumentNullException(nameof(action));

			try
			{
				action();
			}
			catch { }
		}

        public static Bitmap Decode(RenderTarget device, BitmapDecoder decoder)
        {
            var frame = decoder.GetFrame(0);
            var converter = new FormatConverter(AbstractImage.ImageFactory);

            converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);
            var bmp = Bitmap.FromWicBitmap(device, converter);

            converter.Dispose();
            frame.Dispose();
            return bmp;
        }

        public static void DecodeGIF(RenderTarget device, BitmapDecoder decoder, GIFImage output)
        {
            ////////// GLOBAL METADATA //////////////////////////////////////////////////
            var globalMeta = decoder.MetadataQueryReader;
            output.Width = Convert.ToInt32(globalMeta.TryGetMetadataByName("/logscrdesc/Width"));
            output.Height = Convert.ToInt32(globalMeta.TryGetMetadataByName("/logscrdesc/Height"));
            output.useGlobalBackgroundColor = Convert.ToBoolean(globalMeta.TryGetMetadataByName("/logscrdesc/GlobalColorTableFlag"));
            if(output.useGlobalBackgroundColor)
            {
                var globalColorIndex = Convert.ToInt32(globalMeta.TryGetMetadataByName("/logscrdesc/BackgroundColorIndex"));
                Palette colorTable = new Palette(AbstractImage.ImageFactory);
                decoder.CopyPalette(colorTable);

                int colorInt = colorTable.GetColors<int>()[globalColorIndex];
                byte[] colorRGBA = BitConverter.GetBytes(colorInt);
                output.globalBackgroundColor = new Color(colorRGBA[0], colorRGBA[1], colorRGBA[2], colorRGBA[3]);

                colorTable.Dispose();
            }

            ///////////////////////// FRAMES DECODE /////////////////////////////////////
            for (int i = 0; i < decoder.FrameCount; i++)
            {
                BitmapFrameDecode frame = decoder.GetFrame(i);
                var converter = new FormatConverter(AbstractImage.ImageFactory);

                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppPRGBA);
                var bmp = Bitmap.FromWicBitmap(device, converter);

                var localMeta = frame.MetadataQueryReader;
                float delay = Convert.ToSingle(localMeta.TryGetMetadataByName("/grctlext/Delay")) / 100;
                int width = Convert.ToInt32(localMeta.TryGetMetadataByName("/imgdesc/Width"));
                int height = Convert.ToInt32(localMeta.TryGetMetadataByName("/imgdesc/Height"));
                Point topLeft = new Point(Convert.ToInt32(localMeta.TryGetMetadataByName("/imgdesc/Left")),
                                            Convert.ToInt32(localMeta.TryGetMetadataByName("/imgdesc/Top")));
                bool useLocalBg = Convert.ToBoolean(localMeta.TryGetMetadataByName("/imgdesc/LocalColorTableFlag"));
                FrameImageDisposal disposal = (FrameImageDisposal)Convert.ToInt32(localMeta.TryGetMetadataByName("/grctlext/Disposal"));

                output.frameImages.Add(new FrameImage()
                {
                    Frame = bmp,
                    Delay = delay,
                    Width = width,
                    Height = height,
                    TopLeft = topLeft,
                    useLocalBackgroundColor = useLocalBg,
                    frameDisposal = disposal
                });

                frame.Dispose();
                converter.Dispose();
            }
        }
    }
}
