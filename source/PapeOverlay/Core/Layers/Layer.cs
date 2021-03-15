using GameOverlay.Drawing;

using PapeOverlay.Core.Exceptions;
using PapeOverlay.Core.Input;
using SharpDX;
using System;
using System.Collections.Generic;

namespace PapeOverlay.Core.Layers
{
    public enum TransformType
    {
        Layer,
        World
    }

    /// <summary>
    /// Base class for any graphical element, any subclass must override the drawSelf (draw) method and isPointInside (raycast).
    /// Contains a Transform which holds the transformation of a single layer and it's matrices to draw correctly on the screen.
    /// Any layer instance can have it's own custom properties by using GetState/SetState methods or indexer method [string propertyName]
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// Event called before the global OnUpdate occours inside the OverlayApplication
        /// </summary>
        public event Action<float, long, float> OnUpdateSelf = delegate { };

        /// <summary>
        /// Data provider used to get directX wrapped resources from the WindowOverlay indirectly
        /// </summary>
        protected DataLayerProvider dataProvider;
        protected Transform transform;
        private readonly Dictionary<string, object> state;

        /// <summary>
        /// Layer name, it's not unique
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is layer visible, if false no draw is done, update it's still called
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Is layer selected, if true a selection shape should appear on screen
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Layer Transform, contains any information about space and coordinates
        /// </summary>
        public Transform Transform => transform;


        /// <summary>
        /// Custom property of this instance
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Value binded to this property name</returns>
        public object this[string propertyName]
        {
            get => GetState(propertyName);
            set => SetState(propertyName, value);
        }

        public Layer()
        {
            state = new Dictionary<string, object>(0);
            IsVisible = true;
            Name = "Layer0";

            transform = new Transform(this);
        }

        /// <summary>
        /// Get the value binded to the propertyName
        /// </summary>
        /// <typeparam name="T">Type converted</typeparam>
        /// <param name="propertyName">Property name</param>
        /// <returns>Value binded to the propertyName as T</returns>
        public T GetState<T>(string propertyName)
        {
            if (!state.ContainsKey(propertyName))
                throw new PropertyNotFoundException(propertyName, this);

            return (T)state[propertyName];
        }

        /// <summary>
        /// Get the value binded to the propertyName
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <returns>Value binded to the propertyName</returns>
        public object GetState(string propertyName)
        {
            if (!state.ContainsKey(propertyName))
                throw new PropertyNotFoundException(propertyName, this);

            return state[propertyName];
        }

        /// <summary>
        /// Set the value binded to the propertyName
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="value">Value</param>
        public void SetState(string propertyName, object value)
        {
            if (!state.ContainsKey(propertyName))
                state.Add(propertyName, value);
            else
                state[propertyName] = value;
        }

        /// <summary>
        /// Get child by name
        /// </summary>
        /// <typeparam name="T">Child specific type</typeparam>
        /// <param name="name">Child name</param>
        /// <returns>Child</returns>
        public T GetChildByName<T>(string name) where T : Layer
        {
            return GetChildByFunc<T>(layer => layer.Name == name);
        }

        /// <summary>
        /// Get child by criterias
        /// </summary>
        /// <typeparam name="T">Child specific type</typeparam>
        /// <param name="func">Child criteria</param>
        /// <returns>Child</returns>
        public T GetChildByFunc<T>(Func<T, bool> func) where T : Layer
        {
            return transform.RunSingleResultQueryDFS(func, true);
        }

        /// <summary>
        /// Get children by name
        /// </summary>
        /// <typeparam name="T">Children specific type</typeparam>
        /// <param name="name">Children name</param>
        /// <returns>Children</returns>
        public List<T> GetChildrenByName<T>(string name) where T : Layer
        {
            return GetChildrenByFunc<T>(layer => layer.Name == name);
        }

        /// <summary>
        /// Get children by criterias
        /// </summary>
        /// <typeparam name="T">Children specific type</typeparam>
        /// <param name="func">Child criteria</param>
        /// <returns>Children</returns>
        public List<T> GetChildrenByFunc<T>(Func<T, bool> func) where T : Layer
        {
            return transform.RunMultipleResultQueryDFS(func, true);
        }

        internal virtual void OnInit(DataLayerProvider dataLayerProvider)
        {
            dataProvider = dataLayerProvider;
        }

        internal virtual void OnStartup(DataLayerProvider dataLayerProvider)
        {
            LayerHelper.RunLayerAction(this, layer => layer.OnInit(dataLayerProvider));
        }

        /// <summary>
        /// Check if mouse hover layer
        /// </summary>
        /// <returns>is hovered</returns>
        public virtual bool IsMouseHover()
        {
            return IsPointInside(InputManager.GetMouseCoordinatesToWindow());
        }

        /// <summary>
        /// Check if mouse button down inside layer
        /// </summary>
        /// <param name="mouseButton">Mouse button</param>
        /// <returns>is button down</returns>
        public virtual bool IsMouseDown(MouseButton mouseButton)
        {
            if (InputManager.IsMouseButtonDown(mouseButton))
                return IsPointInside(InputManager.GetMouseCoordinatesToWindow());

            return false;
        }

        /// <summary>
        /// Check if mouse button up inside layer
        /// </summary>
        /// <param name="mouseButton">Mouse button</param>
        /// <returns>is button up</returns>
        public virtual bool IsMouseUp(MouseButton mouseButton)
        {
            if (InputManager.IsMouseButtonUp(mouseButton))
                return IsPointInside(InputManager.GetMouseCoordinatesToWindow());

            return false;
        }

        /// <summary>
        /// Check if a point is inside layer
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns>is inside layer</returns>
        public virtual bool IsPointInside(Point point) { return false; }

        internal void Update(float delta, long frameCount, float frameTime)
        {
            LayerHelper.RunLayerAction(this, layer => layer.OnUpdateSelf.Invoke(delta, frameCount, frameTime));
        }

        /// <summary>
        /// Called once per frame, here the layer should be drawn
        /// </summary>
        /// <param name="gfx">Graphics component</param>
        protected virtual void DrawSelf(Graphics gfx) { }

        /// <summary>
        /// Called once per frame, here the layer selected graphic should be drawn
        /// </summary>
        /// <param name="gfx">Graphics component</param>
        protected virtual void DrawSelected(Graphics gfx) { }

        internal void Draw(Graphics gfx)
        {
            LayerHelper.RunLayerAction(this, layer =>
            {
                if (!layer.IsVisible)
                    return;

                Transform currTrasf = layer.transform;
                if (currTrasf.matrixNeedsUpdate)
                {
                    currTrasf.RecalculateLocalMatrix();
                }

                currTrasf.modelMatrix = currTrasf.Parent != null ? currTrasf.localMatrix * currTrasf.Parent.modelMatrix : currTrasf.localMatrix;

                gfx.TransformStart(currTrasf.modelMatrix);
                layer.DrawSelf(gfx);
                if (layer.IsSelected)
                    layer.DrawSelected(gfx);
                gfx.TransformEnd();
            });
        }

        /// <summary>
        /// Raycast that checks for any layer that contains this point as input
        /// </summary>
        /// <param name="position">Point input</param>
        /// <returns>List of layers hitted</returns>
        public List<Layer> RaycastAll(Point position) => RaycastAll<Layer>(position);

        /// <summary>
        /// Raycast that checks for the most visible layer that contains this point as input
        /// </summary>
        /// <param name="position">Point input</param>
        /// <returns>Layer hitted</returns>
        public Layer Raycast(Point position) => Raycast<Layer>(position);

        /// <summary>
        /// Raycast that checks or the most visible layer that contains this point as input
        /// </summary>
        /// <typeparam name="T">Layer specific type</typeparam>
        /// <param name="position">Point input</param>
        /// <returns>Layer hitted</returns>
        public T Raycast<T>(Point position) where T : Layer
        {
            List<T> layers = transform.RunMultipleResultQueryDFS<T>(_ => true);
            for(int i = layers.Count - 1; i >= 0; i--)
            {
                T curr = layers[i];
                if (curr.IsPointInside(position))
                    return curr;
            }

            return null;
        }

        /// <summary>
        /// Raycast that checks for any layer that contains this point as input
        /// </summary>
        /// <typeparam name="T">Layers specific type</typeparam>
        /// <param name="position">Point input</param>
        /// <returns>List of layers hitted</returns>
        public List<T> RaycastAll<T>(Point position) where T : Layer
        {
            List<T> layers = transform.RunMultipleResultQueryDFS<T>(_ => true);
            for (int i = layers.Count - 1; i >= 0; i--)
            {
                T curr = layers[i];
                if (!curr.IsPointInside(position))
                    layers.Remove(curr);
            }

            return layers;
        }
    }
}
