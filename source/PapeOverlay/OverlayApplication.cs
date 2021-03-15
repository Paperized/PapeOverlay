using GameOverlay.Drawing;
using PapeOverlay.Core;
using PapeOverlay.Core.Layers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace PapeOverlay
{
    /// <summary>
    /// Generic message from an application overlay containing a title and a message.
    /// </summary>
    public class ApplicationEvent
    {
        public string title;
        public string message;
    }

    /// <summary>
    /// Level of severity of an ApplicationError
    /// </summary>
    public enum ApplicationErrorSeverity
    {
        Low,
        Medium,
        High,
        Fatal
    }

    /// <summary>
    /// Error message from and application overlay inheredit from ApplicationEvent and contains a level of severity.
    /// </summary>
    public class ApplicationError : ApplicationEvent
    {
        public ApplicationErrorSeverity severity;
    }

    /// <summary>
    /// Application configuration, it's used to set some configuration for an application, it's saved on disk on application close and loaded on application open.
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// Author name
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string DescriptionOverlay { get; set; }

        /// <summary>
        /// Destination window, this should match the window title you want to make the overlay, if the name doesn't match the first application which contains
        /// this string is taken.
        /// </summary>
        public string DestinationWindow { get; set; }
        public string Version { get; set; }

        /// <summary>
        /// Custom property you want to save and retrieve from you application
        /// </summary>
        public Dictionary<string, object> CustomProperties { get; set; }

        public object this[string property]
        {
            get => CustomProperties[property];
            set => CustomProperties[property] = value;
        }
    }

    /// <summary>
    /// Base class for any overlay application, already contains a minimum amount of logic about loading/saving AppConfig and memory management on close.
    /// Provides some overridable methods and event to send notification via event.
    /// </summary>
    public abstract class OverlayApplication
    {
        /// <summary>
        /// Event invoked when a generic event occured
        /// </summary>
        public event Action<OverlayApplication, ApplicationEvent> OnEvent = delegate { };

        /// <summary>
        /// Event invoked when an error event occured
        /// </summary>
        public event Action<OverlayApplication, ApplicationError> OnError = delegate { };

        /// <summary>
        /// Window Overlay used by this OverlayApplication
        /// </summary>
        protected WindowOverlay WindowOverlay { get; private set; }

        /// <summary>
        /// Current app configuration in memory, saved on disk OnClosing
        /// </summary>
        protected AppConfig AppConfig { get; private set; }

        /// <summary>
        /// Has the overlay loaded
        /// </summary>
        public bool IsStarted { get; private set; }

        /// <summary>
        /// Overlay path that contains the configuration and assets
        /// </summary>
        public string OverlayPath { get; private set; }

        /// <summary>
        /// JSON Configuration path
        /// </summary>
        public string ConfigPath => OverlayPath + "/appConfig.json";

        /// <summary>
        /// Assets folder path
        /// </summary>
        public string AssetsPath => OverlayPath + "/Assets/";

        /// <summary>
        /// Root layer for this application
        /// </summary>
        protected Layer RootLayer => WindowOverlay.RootLayer;

        /// <summary>
        /// Create an Application with a given overlayPath, also loads the configuration found inside the folder in memory.
        /// </summary>
        /// <param name="overlayPath">Overlay path</param>
        public OverlayApplication(string overlayPath)
        {
            OverlayPath = overlayPath;
            LoadConfig();
        }

        /// <summary>
        /// Gives a default config for your application if it doesnt exists on disk
        /// </summary>
        /// <returns>Default configuration</returns>
        public abstract AppConfig DefaultConfig();

        /// <summary>
        /// Create and initialize the overlay, loads the engine and bind the various events to this application. At the end of this method InitializeComponents.
        /// Use Start to start the graphical interface
        /// </summary>
        public void InitializeOverlay()
        {
            try
            {
                WindowOverlay = new WindowOverlay(GetWindowHandle(AppConfig.DestinationWindow));
                WindowOverlay.OnSetupLayers += InternalOnCreateLayers;
                WindowOverlay.OnUpdateLayers += OnUpdateOverlay;
                WindowOverlay.OnWindowSizeChanged += OnWindowSizeChange;
            }
            catch (Exception ex)
            {
                OnError.Invoke(this, new ApplicationError() {
                    title = "InitializeOverlay",
                    message = ex.Message,
                    severity = ApplicationErrorSeverity.Fatal
                });

                return;
            }

            InitializeComponents(WindowOverlay);
            OnEvent.Invoke(this, new ApplicationEvent()
            {
                title = "Startup",
                message = "Overlay initialized successfully"
            });
        }

        /// <summary>
        /// Starts the overlay if it's not already started
        /// </summary>
        public void Start()
        {
            if (IsStarted)
                return;

            IsStarted = true;
            WindowOverlay.Start();
        }

        /// <summary>
        /// Load the configuration from disk, if it doesn't exists create a default one and save it.
        /// </summary>
        private void LoadConfig()
        {
            string jsonConfig;

            if (File.Exists("./appConfig.json"))
            {
                jsonConfig = File.ReadAllText(ConfigPath);
                AppConfig = JsonSerializer.Deserialize<AppConfig>(jsonConfig);
            }
            else
            {
                AppConfig = DefaultConfig();
                SaveConfig();
            }
        }

        /// <summary>
        /// Save the current configuration from memory to disk.
        /// </summary>
        private void SaveConfig()
        {
            string jsonConfig = JsonSerializer.Serialize(AppConfig);
            File.WriteAllText(ConfigPath, jsonConfig);
        }

        /// <summary>
        /// Create any resource you need before creating your layers that uses them.
        /// Current resources available: Brushes, Fonts, Images (.JPG, .PNG, ...., .GIF)
        /// </summary>
        /// <param name="windowOverlay">Current window overlay</param>
        protected abstract void InitializeComponents(WindowOverlay windowOverlay);

        /// <summary>
        /// Internal create layers that calls the real OnCreateLayers
        /// </summary>
        /// <param name="windowOverlay">Current window overlay</param>
        private void InternalOnCreateLayers(WindowOverlay windowOverlay)
        {
            OnCreateLayers();
        }

        /// <summary>
        /// Create and initialize any layer you need, right now it's the only place you can create layers.
        /// </summary>
        protected abstract void OnCreateLayers();

        /// <summary>
        /// Called every frame with three arguments. Use this method to update your layers.
        /// </summary>
        /// <param name="deltaTime">Time passed to previous frame</param>
        /// <param name="frameCount">Current frame count</param>
        /// <param name="frameTime">This frame time</param>
        protected virtual void OnUpdateOverlay(float deltaTime, long frameCount, float frameTime) { }

        /// <summary>
        /// Called every time the target window change it's size
        /// </summary>
        /// <param name="newSize">New window size</param>
        protected virtual void OnWindowSizeChange(Point newSize) { }

        /// <summary>
        /// Close this overlay application and release every resources
        /// </summary>
        public void Close()
        {
            OnClosing();
        }

        /// <summary>
        /// Unbind every event, release memory used and save the current configuration to disk.
        /// </summary>
        protected virtual void OnClosing()
        {
            WindowOverlay.OnSetupLayers -= InternalOnCreateLayers;
            WindowOverlay.OnUpdateLayers -= OnUpdateOverlay;
            WindowOverlay.OnWindowSizeChanged -= OnWindowSizeChange;
            WindowOverlay.End();
            SaveConfig();
            OnEvent.Invoke(this, new ApplicationEvent()
            {
                title = "Closing",
                message = "Overlay closing!"
            });
        }

        /// <summary>
        /// Util to get an handle by window title
        /// </summary>
        /// <param name="windowName">Window name the title must contain</param>
        /// <returns>Handle</returns>
        private IntPtr GetWindowHandle(string windowName)
        {
            foreach(Process process in Process.GetProcesses())
            {
                if (process.MainWindowTitle.Contains(windowName))
                    return process.MainWindowHandle;
            }

            return IntPtr.Zero;
        }
    }
}
