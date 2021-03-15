using GameOverlay.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PapeOverlay.Core
{
    public interface IWindowOverlay
    {
        void SetHandle(IntPtr targetWindow);
        void SetVisibility(bool isVisible);
        void Start();
        void End();
        void Pause();
        void Resume();
        bool SetBrush(string name, Color color);
        bool SetBrush(string name, int r, int g, int b, int a = 255);
        bool SetBrush(string name, float r, float g, float b, float a = 1);
        bool SetFont(string name, string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false);
        bool SetImage(string name, string path);
    }
}
