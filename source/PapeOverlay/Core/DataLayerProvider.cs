using GameOverlay.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeOverlay.Core
{
    public class DataLayerProvider
    {
        private readonly Dictionary<string, SolidBrush> brushesLoaded;
        private readonly Dictionary<string, Font> fontsLoaded;
        private readonly Dictionary<string, AbstractImageContainer> imagesLoaded;
        internal SolidBrush SelectedBrush { get; set; }
        public bool recreatingResources = false;

        public DataLayerProvider(Dictionary<string, SolidBrush> brushesLoaded, Dictionary<string, Font> fontsLoaded,
                Dictionary<string, AbstractImageContainer> imagesLoaded)
        {
            this.brushesLoaded = brushesLoaded;
            this.fontsLoaded = fontsLoaded;
            this.imagesLoaded = imagesLoaded;
        }

        public SolidBrush GetBrushByName(string name)
        {
            if (name == null || !brushesLoaded.ContainsKey(name))
                return null;

            return brushesLoaded[name];
        }

        public Font GetFontByName(string name)
        {
            if (name == null || !fontsLoaded.ContainsKey(name))
                return null;

            return fontsLoaded[name];
        }

        public T GetImageByName<T>(string name) where T : AbstractImage
        {
            if (name == null || !imagesLoaded.ContainsKey(name))
                return null;

            return imagesLoaded[name].As<ImageContainer<T>>().GetImage();
        }
    }
}
