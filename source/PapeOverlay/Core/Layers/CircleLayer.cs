using GameOverlay.Drawing;
using System;

namespace PapeOverlay.Core.Layers
{
    public enum LineType
    {
        Line,
        Outline,
        Dashed
    }

    public class CircleLayer : Layer
    {
        public LineType LineType { get; set; }
        public bool IsFilled { get; set; }
        public float Radius { get; set; }
        public float Stroke { get; set; }
        private BrushWrapper Brush { get; set; }
        private BrushWrapper FillBrush { get; set; }

        public CircleLayer()
        {
            Brush = new BrushWrapper();
            FillBrush = new BrushWrapper();
        }

        internal override void OnInit(DataLayerProvider dataLayerProvider)
        {
            base.OnInit(dataLayerProvider);

            if(Brush.brushName != null)
                Brush.brush = dataLayerProvider.GetBrushByName(Brush.brushName);
            if(FillBrush.brushName != null)
                FillBrush.brush = dataLayerProvider.GetBrushByName(FillBrush.brushName);
        }

        public void SetBrushColorName(string name)
        {
            Brush.brushName = name;
        }

        public void SetFillBrushColorName(string name)
        {
            FillBrush.brushName = name;
        }

        protected override void DrawSelf(Graphics gfx)
        {
            switch(LineType)
            {
                case LineType.Outline:
                    if (IsFilled)
                        gfx.OutlineFillCircle(Brush.brush, FillBrush.brush, Point.Zero, Radius, Stroke);
                    else
                        gfx.DrawCircle(Brush.brush, Point.Zero, Radius, Stroke);
                    break;
                case LineType.Line:
                    if (IsFilled)
                        gfx.FillCircle(FillBrush.brush, Point.Zero, Radius);
                    else
                        gfx.DrawCircle(Brush.brush, Point.Zero, Radius, Stroke);
                    break;
                case LineType.Dashed:
                    gfx.DashedCircle(Brush.brush, Point.Zero, Radius, Stroke);
                    break;
            }
        }

        protected override void DrawSelected(Graphics gfx)
        {
            gfx.DrawCircle(dataProvider.SelectedBrush, Point.Zero, Radius + Stroke / 2, 4);
        }

        public override bool IsPointInside(Point point)
        {
            TransformationMatrix modelMatrix = transform.CalculateModelMatrix();
            modelMatrix.Invert();
            Point transformed = TransformationMatrix.TransformPoint(modelMatrix, point);
            return Math.Pow(transformed.X, 2) + Math.Pow(transformed.Y, 2) <= Math.Pow(Radius, 2);
        }
    }
}
