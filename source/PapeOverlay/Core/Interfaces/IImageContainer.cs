using GameOverlay.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeOverlay.Core.Interfaces
{
    public interface IImageContainer<T> where T : AbstractImage
    {
        T GetImage();
        void SetImage(T image);
    }
}
