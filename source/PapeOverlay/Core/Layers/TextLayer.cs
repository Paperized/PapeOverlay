using GameOverlay.Drawing;
using GameOverlay.Drawing.Utils;

namespace PapeOverlay.Core.Layers
{ 

    public class TextLayer : Layer
    {
        public string Text { get; set; }
        public float FontSize { get; set; }
        private FontWrapper Font { get; set; }
        private BrushWrapper Brush { get; set; }
        private BrushWrapper BackgroundBrush { get; set; }
        public BoxContainer BoxContainer { get; set; }

        public TextLayer()
        {
            Font = new FontWrapper();
            Brush = new BrushWrapper();
            BackgroundBrush = new BrushWrapper();
        }

        internal override void OnInit(DataLayerProvider dataLayerProvider)
        {
            base.OnInit(dataLayerProvider);

            if (Font.fontName != null)
                Font.font = dataLayerProvider.GetFontByName(Font.fontName);
            if (Brush.brushName != null)
                Brush.brush = dataLayerProvider.GetBrushByName(Brush.brushName);
            if (BackgroundBrush.brushName != null)
                BackgroundBrush.brush = dataLayerProvider.GetBrushByName(BackgroundBrush.brushName);
        }

        public void SetBackgroundBrushColorName(string name)
        {
            BackgroundBrush.brushName = name;
        }

        public void SetBrushColorName(string name)
        {
            Brush.brushName = name;
        }

        public void SetFontName(string name)
        {
            Font.fontName = name;
        }

        protected override void DrawSelf(Graphics gfx)
        {
            BoxContainer box = BoxContainer;
            gfx.DrawText(Font.font, FontSize, Brush.brush, Point.Zero, Text, ref box);
            BoxContainer = box;
        }

        protected override void DrawSelected(Graphics gfx)
        {
            gfx.DrawRectangle(dataProvider.SelectedBrush, BoxContainer.GetCenteredRectangle(Point.Zero), 4);
        }

        public override bool IsPointInside(Point point)
        {
            TransformationMatrix modelMatrix = transform.CalculateModelMatrix();
            modelMatrix.Invert();
            Point transformed = TransformationMatrix.TransformPoint(modelMatrix, point);
            return BoxContainer.GetCenteredRectangle(Point.Zero).IsPointInside(transformed);
        }
    }
}
