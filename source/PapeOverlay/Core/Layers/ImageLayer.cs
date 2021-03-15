using GameOverlay.Drawing;
using GameOverlay.Drawing.Utils;
using System.Collections.Generic;

namespace PapeOverlay.Core.Layers
{
    public abstract class AbstractImageLayer<T> : Layer where T : AbstractImage
    {
        public float Opacity { get; set; }
        public bool LinearScale { get; set; }
        public Point OriginalSize => new Point(Image.image.Width, Image.image.Height);
        public Rectangle OriginalRectangle => Rectangle.CenteredRectangle(transform.Position, Image.image.Width, Image.image.Height);
        public BoxContainer BoxContainer { get; set; }
        protected ImageWrapper<T> Image { get; set; }

        public AbstractImageLayer()
        {
            Opacity = 1;
            LinearScale = true;
            Image = new ImageWrapper<T>();
        }

        internal override void OnInit(DataLayerProvider dataLayerProvider)
        {
            base.OnInit(dataLayerProvider);

            Image.image = dataLayerProvider.GetImageByName<T>(Image.imageName);
            if (Image == null)
                throw new System.Exception("Image field cannot be null in " + GetType().Name + ", Initialize it in your constructor");

            if(!BoxContainer.isCustomSize)
            {
                Point size = OriginalSize;
                BoxContainer container = BoxContainer;
                container.customWidth = size.X;
                container.customHeight = size.Y;
                BoxContainer = container;
            }
        }

        public void SetImageName(string name)
        {
            Image.imageName = name;
        }

        protected override void DrawSelected(Graphics gfx)
        {
            const float stroke = 3;
            Rectangle biggerRectangle = BoxContainer.GetCenteredRectangle(Point.Zero);
            gfx.DrawRectangle(dataProvider.SelectedBrush, biggerRectangle, stroke);
        }

        public override bool IsPointInside(Point point)
        {
            TransformationMatrix modelMatrix = transform.CalculateModelMatrix();
            modelMatrix.Invert();
            Point transformed = TransformationMatrix.TransformPoint(modelMatrix, point);
            return BoxContainer.GetCenteredRectangle(Point.Zero).IsPointInside(transformed);
        }
    }

    public class ImageLayer : AbstractImageLayer<Image>
    {
        public ImageLayer() : base()
        {

        }

        protected override void DrawSelf(Graphics gfx)
        {
            gfx.DrawImage(Image.image, BoxContainer.GetCenteredRectangle(Point.Zero), Opacity, LinearScale);
        }
    }

    public class GIFImageLayer : AbstractImageLayer<GIFImage>
    {
        private int currentFrameIndex;
        private float internalTimer;
        public List<FrameImage> Frames { get; private set; }
        private FrameImage CurrentFrame => Frames[currentFrameIndex];

        public GIFImageLayer() : base()
        {
            OnUpdateSelf += OnFrameUpdate;
        }

        ~GIFImageLayer()
        {
            OnUpdateSelf -= OnFrameUpdate;
        }

        internal override void OnInit(DataLayerProvider dataLayerProvider)
        {
            base.OnInit(dataLayerProvider);

            currentFrameIndex = 0;
            Frames = Image.image.frameImages;
        }

        protected void OnFrameUpdate(float deltaTime, long frameCount, float frameTime)
        {
            internalTimer += deltaTime;
            if (internalTimer >= CurrentFrame.Delay)
            {
                internalTimer -= CurrentFrame.Delay;
                if (currentFrameIndex < Frames.Count - 1)
                    currentFrameIndex += 1;
                else
                    currentFrameIndex = 0;
            }
        }

        protected override void DrawSelf(Graphics gfx)
        {
            gfx.DrawImageGIF(currentFrameIndex, Image.image, Point.Zero, Opacity);
        }
    }
}
