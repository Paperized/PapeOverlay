using GameOverlay.Drawing;
using PapeOverlay.Core.Interfaces;

namespace PapeOverlay.Core.Layers
{
    public class BrushWrapper
    {
        public string brushName;
        public IBrush brush;
    }

    public class FontWrapper
    {
        public string fontName;
        public Font font;
    }

    public class ImageWrapper<T> : IImageContainer<T> where T : AbstractImage
    {
        public string imageName;
        public T image;

        public T GetImage() => image;
        public void SetImage(T image) => this.image = image;
    }
}
