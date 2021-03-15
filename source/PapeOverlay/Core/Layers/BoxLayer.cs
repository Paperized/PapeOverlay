using GameOverlay.Drawing;

namespace PapeOverlay.Core.Layers
{
    public class BoxLayer : Layer
    {
        private float width, height;

        public bool IsDashed { get; set; }
        public bool IsFilled { get; set; }
        public float Radius { get; set; }
        public float Stroke { get; set; }
        public float Width { get => width; set { width = value; RefreshRectangle(); }}
        public float Height { get => height; set { height = value; RefreshRectangle(); } }
        public Rectangle Rectangle { get; private set; }
        private BrushWrapper Brush { get; set; }
        private BrushWrapper StrokeBrush { get; set; }

        public BoxLayer()
        {
            Brush = new BrushWrapper();
            StrokeBrush = new BrushWrapper();
        }

        internal override void OnInit(DataLayerProvider dataLayerProvider)
        {
            base.OnInit(dataLayerProvider);

            if (Brush.brushName != null)
                Brush.brush = dataLayerProvider.GetBrushByName(Brush.brushName);
            if (StrokeBrush.brushName != null)
                StrokeBrush.brush = dataLayerProvider.GetBrushByName(StrokeBrush.brushName);

            RefreshRectangle();
        }

        public void SetBrushColorName(string name)
        {
            Brush.brushName = name;
        }

        public void SetStrokeBrushColorName(string name)
        {
            StrokeBrush.brushName = name;
        }

        public override bool IsPointInside(Point point)
        {
            TransformationMatrix modelMatrix = transform.CalculateModelMatrix();
            modelMatrix.Invert();
            Point transformed = TransformationMatrix.TransformPoint(modelMatrix, point);
            return Rectangle.IsPointInside(transformed);
        }

        protected override void DrawSelf(Graphics gfx)
        {
            //fill rect -> box2d
            //rect -> rectangle
            if(!IsDashed)
            {
                if (IsFilled)
                {
                    if (Radius > 0)
                    {
                        gfx.FillRoundedRectangle(Brush.brush, Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom, Radius);
                        gfx.DrawRoundedRectangle(StrokeBrush.brush, Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom, Radius, Stroke);
                    }
                    else
                    {
                        gfx.DrawBox2D(StrokeBrush.brush, Brush.brush, Rectangle, Stroke);
                    }
                }
                else
                {
                    if (Radius > 0)
                    {
                        gfx.DrawRoundedRectangle(StrokeBrush.brush, Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom, Radius, Stroke);
                    }
                    else
                    {
                        gfx.DrawRectangle(StrokeBrush.brush, Rectangle, Stroke);
                    }
                }
            }
            else
            {
                if (IsFilled)
                {
                    if (Radius > 0)
                    {
                        gfx.FillRoundedRectangle(Brush.brush, Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom, Radius);
                        gfx.DashedRoundedRectangle(StrokeBrush.brush, Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom, Radius, Stroke);
                    }
                    else
                    {
                        gfx.FillRectangle(Brush.brush, Rectangle);
                        gfx.DashedRectangle(StrokeBrush.brush, Rectangle, Stroke);
                    }
                }
                else
                {
                    if (Radius > 0)
                    {
                        gfx.DrawRoundedRectangle(StrokeBrush.brush, Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom, Radius, Stroke);
                    }
                    else
                    {
                        gfx.DashedRectangle(StrokeBrush.brush, Rectangle, Stroke);
                    }
                }
            }
        }

        protected override void DrawSelected(Graphics gfx)
        {
            float halfStroke = Stroke / 2;

            if(Radius > 0)
            {
                gfx.DrawRoundedRectangle(dataProvider.SelectedBrush, Rectangle.Left - halfStroke, Rectangle.Top - halfStroke, Rectangle.Right + halfStroke, Rectangle.Bottom + halfStroke, Radius, 4);
            }
            else
            {
                gfx.DrawRectangle(dataProvider.SelectedBrush, Rectangle.Left - halfStroke, Rectangle.Top - halfStroke, Rectangle.Right + halfStroke, Rectangle.Bottom + halfStroke, 4);
            }
        }

        protected void RefreshRectangle()
        {
            float halfWidth = Width / 2;
            float halfHeight = Height / 2;
            Rectangle = new Rectangle(- halfWidth, - halfHeight, + halfWidth, + halfHeight);
        }
    }
}
