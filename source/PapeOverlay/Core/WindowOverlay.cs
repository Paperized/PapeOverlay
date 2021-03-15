using GameOverlay.Drawing;
using GameOverlay.Windows;
using PapeOverlay.Core.Interfaces;
using PapeOverlay.Core.Layers;
using System;
using System.Collections.Generic;

namespace PapeOverlay.Core
{
    public class WindowOverlay : IWindowOverlay, IDisposable
    {
        internal event Action<float, long, float> OnUpdateLayers = delegate { };
        internal event Action<WindowOverlay> OnSetupLayers = delegate { };
        internal event Action<Point> OnWindowSizeChanged = delegate { };

        public Layer RootLayer { get; set; }

        private readonly Dictionary<string, BrushData> brushesAvailable;
        private readonly Dictionary<string, FontData> fontsAvailable;
        private readonly Dictionary<string, ImageData> imagesAvailable;

        private readonly Dictionary<string, SolidBrush> brushesLoaded;
        private readonly Dictionary<string, Font> fontsLoaded;
        private readonly Dictionary<string, AbstractImageContainer> imagesLoaded;

        private readonly Graphics gfx;
        private readonly StickyWindow window;
        private readonly DataLayerProvider layerProvider;

        public Point GetWindowSize() => new Point(window.Width, window.Height);

        private bool alreadySetupped = false;

        public WindowOverlay(IntPtr targetWindow)
        {
            Input.InputManager.WindowOverlay = this;

            RootLayer = new Layer()
            {
                Name = "Root"
            };

            brushesAvailable = new Dictionary<string, BrushData>();
            fontsAvailable = new Dictionary<string, FontData>();
            imagesAvailable = new Dictionary<string, ImageData>();

            brushesLoaded = new Dictionary<string, SolidBrush>();
            fontsLoaded = new Dictionary<string, Font>();
            imagesLoaded = new Dictionary<string, AbstractImageContainer>();

            gfx = new Graphics()
            {
                MeasureFPS = true,
                PerPrimitiveAntiAliasing = true,
                TextAntiAliasing = true
            };

            window = new StickyWindow(targetWindow, gfx)
            {
                FPS = 60,
                IsTopmost = true,
                IsVisible = true,
                BypassTopmost = true,
                AttachToClientArea = true
            };

            layerProvider = new DataLayerProvider(brushesLoaded, fontsLoaded, imagesLoaded);
        }

        private void Window_SizeChanged(object sender, OverlaySizeEventArgs e)
        {
            if(!alreadySetupped)
            {
                alreadySetupped = true;
                OnSetupLayers.Invoke(this);
                InitializeLayerStructure();
            }
            else
            {
                OnWindowSizeChanged.Invoke(new Point(e.Width, e.Height));
            }
        }

        public void Start()
        {
            window.SetupGraphics += Window_SetupGraphics;
            window.DrawGraphics += Window_DrawGraphics;
            window.DestroyGraphics += Window_DestroyGraphics;
            window.SizeChanged += Window_SizeChanged;

            window.Create();
        }

        public void End()
        {
            window.SetupGraphics -= Window_SetupGraphics;
            window.DrawGraphics -= Window_DrawGraphics;
            window.DestroyGraphics -= Window_DestroyGraphics;
            window.SizeChanged -= Window_SizeChanged;

            DisposeResources();
            gfx.Dispose();
            window.Dispose();
        }

        private void Window_SetupGraphics(object sender, SetupGraphicsEventArgs e)
        {
            var gfx = e.Graphics;

            if (e.RecreateResources)
            {
                DisposeResources();
            }

            foreach (var pair in brushesAvailable)
            {
                brushesLoaded[pair.Key] = gfx.CreateSolidBrush(pair.Value.color);
            }

            foreach (var pair in fontsAvailable)
            {
                var fontData = pair.Value;
                fontsLoaded[pair.Key] = gfx.CreateFont(fontData.fontFamilyName, fontData.size, fontData.bold, fontData.italic, fontData.wordWrapping);
            }

            foreach (var pair in imagesAvailable)
            {
                if(pair.Value.path.EndsWith(".gif"))
                {
                    GIFImage gif = gfx.CreateImageGIF(pair.Value.path);
                    if(gif.useGlobalBackgroundColor)
                    {
                        gif.bgBrush = gfx.CreateSolidBrush(gif.globalBackgroundColor);
                        brushesLoaded[gif.GetHashCode().ToString()] = (SolidBrush)gif.bgBrush;
                    }

                    imagesLoaded[pair.Key] = new ImageContainer<GIFImage>(gif);

                }
                else
                {
                    imagesLoaded[pair.Key] = new ImageContainer<Image>(gfx.CreateImage(pair.Value.path));
                }
            }

            layerProvider.SelectedBrush = gfx.CreateSolidBrush(Color.Red);
            brushesLoaded[layerProvider.GetHashCode().ToString()] = layerProvider.SelectedBrush;
        }

        private void Window_DrawGraphics(object sender, DrawGraphicsEventArgs e)
        {
            float delta = e.DeltaTime / 1000f;
            float frameTime = e.FrameTime / 1000f;
            RootLayer.Update(delta, e.FrameTime, frameTime);
            OnUpdateLayers.Invoke(delta, e.FrameCount, frameTime);

            var gfx = e.Graphics;
            gfx.BeginScene();
            gfx.ClearScene();
            RootLayer.Draw(gfx);
            gfx.EndScene();
        }

        private void Window_DestroyGraphics(object sender, DestroyGraphicsEventArgs e)
        {
            DisposeResources();
        }

        private void InitializeLayerStructure()
        {
            RootLayer.OnStartup(layerProvider);
        }

        #region CACHE RESOURCES DATA
        public bool SetBrush(string name, Color color)
        {
            if (!brushesAvailable.ContainsKey(name))
            {
                brushesAvailable.Add(name, null);
            }

            brushesAvailable[name] = new BrushData(color);
            return true;
        }

        public bool SetBrush(string name, int r, int g, int b, int a = 255)
        {
            return SetBrush(name, new Color(r, g, b, a));
        }

        public bool SetBrush(string name, float r, float g, float b, float a = 1)
        {
            return SetBrush(name, new Color(r, g, b, a));
        }

        public bool SetFont(string name, string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            if (!fontsAvailable.ContainsKey(name))
            {
                fontsAvailable.Add(name, null);
            }

            fontsAvailable[name] = new FontData(fontFamilyName, size, bold, italic, wordWrapping);
            return true;
        }

        public bool SetImage(string name, string path)
        {
            if (!imagesAvailable.ContainsKey(name))
            {
                imagesAvailable.Add(name, null);
            }

            imagesAvailable[name] = new ImageData(path);
            return true;
        }
        #endregion

        #region LOAD GRAPHICAL RESOURCES
        private bool ContainsBrushColor(Color color, out SolidBrush solidBrush)
        {
            foreach (var brush in brushesLoaded.Values)
            {
                if (brush != null && brush.Color.Equals(color))
                {
                    solidBrush = brush;
                    return true;
                }
            }

            solidBrush = null;
            return false;
        }

        private bool IsFontEquals(Font font, string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            return font.FontFamilyName.Equals(fontFamilyName) &&
                    font.FontSize.Equals(size) &&
                    font.Bold.Equals(bold) &&
                    font.Italic.Equals(italic) &&
                    font.WordWeapping.Equals(wordWrapping);
        }

        private bool ContainsFont(out Font font, string fontFamilyName, float size, bool bold = false, bool italic = false, bool wordWrapping = false)
        {
            foreach (var currFont in fontsLoaded.Values)
            {
                if (currFont != null && IsFontEquals(currFont, fontFamilyName, size, bold, italic, wordWrapping))
                {
                    font = currFont;
                    return true;
                }
            }

            font = null;
            return false;
        }
        #endregion

        public void Pause()
        {
            window.Pause();
        }

        public void Resume()
        {
            window.Unpause();
        }

        public void SetHandle(IntPtr targetWindow)
        {
            window.ParentWindowHandle = targetWindow;
        }

        public void SetVisibility(bool isVisible)
        {
            window.IsVisible = isVisible;
        }

        public Point ScreenPointToWindow(Point screenCoordinates)
        {
            return new Point(screenCoordinates.X - window.X, screenCoordinates.Y - window.Y);
        }

        public bool IsScreenPointInsideWindow(Point screenCoordinates)
        {
            return screenCoordinates.IsInsideRectangle(window.RectangleWindow);
        }

        private void DisposeResources()
        {
            foreach (var brush in brushesLoaded) brush.Value?.Dispose();
            foreach (var font in fontsLoaded) font.Value?.Dispose();
            foreach (var image in imagesLoaded) image.Value?.Dispose();
        }

        public void Dispose()
        {
            End();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
