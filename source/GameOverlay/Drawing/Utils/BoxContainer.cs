using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOverlay.Drawing.Utils
{
    public struct BoxContainer
    {
        public bool isCustomSize;
        public float customWidth;
        public float customHeight;

        public BoxContainer(bool isCustomSize = false) : this(0, isCustomSize) { }

        public BoxContainer(float size, bool isCustomSize = false) : this(size, size, isCustomSize) { }

        public BoxContainer(float customWidth, float customHeight, bool isCustomSize = false)
        {
            this.customWidth = customWidth;
            this.customHeight = customHeight;
            this.isCustomSize = isCustomSize;
        }

        public Rectangle GetCenteredRectangle(Point position)
        {
            return Rectangle.CenteredRectangle(position, customWidth, customHeight);
        }

        public Rectangle GetRectangle(Point position)
        {
            return new Rectangle(position.X, position.Y, position.X + customWidth, position.Y + customHeight);
        }
    }
}
