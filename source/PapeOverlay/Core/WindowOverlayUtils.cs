using GameOverlay.Drawing;
using PapeOverlay.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeOverlay.Core
{
    public abstract class AbstractImageContainer : IDisposable
    {
        public T As<T>() where T : AbstractImageContainer
        {
            return (T)this;
        }

        public abstract void Dispose();
    }

    public class ImageContainer<T> : AbstractImageContainer, IImageContainer<T> where T : AbstractImage
    {
        private T image;

        public ImageContainer() { }
        public ImageContainer(T image) => this.image = image;

        public override void Dispose()
        {
            image.Dispose();
        }

        public T GetImage()
        {
            return image;
        }

        public void SetImage(T image)
        {
            this.image = image;
        }
    }

    public class BrushData
    {
        public Color color;

        public BrushData(Color color = default)
        {
            this.color = color;
        }

        public bool CompareWithColor(Color color)
        {
            return this.color.Equals(color);
        }
    }

    public class FontData
    {
        public string fontFamilyName;
        public float size;
        public bool bold;
        public bool italic;
        public bool wordWrapping;

        public FontData(string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            this.fontFamilyName = fontFamilyName;
            this.size = size;
            this.bold = bold;
            this.italic = italic;
            this.wordWrapping = wordWrapping;
        }

        public bool CompareWithFontData(string fontFamilyName, float size, bool bold, bool italic, bool wordWrapping)
        {
            return fontFamilyName.Equals(fontFamilyName) &&
                    size.Equals(size) &&
                    bold.Equals(bold) &&
                    italic.Equals(italic) &&
                    wordWrapping.Equals(wordWrapping);
        }
    }

    public class ImageData
    {
        public string path;

        public ImageData(string path)
        {
            this.path = path;
        }

        public bool CompareWithPath(string path)
        {
            return this.path.Equals(path);
        }
    }
}
