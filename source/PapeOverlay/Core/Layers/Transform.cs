using GameOverlay.Drawing;
using SharpDX;
using System;
using System.Collections.Generic;

namespace PapeOverlay.Core.Layers
{
    public class Transform
    {
        public event Action<Transform> OnZOrderChanged = delegate { };
        public event Action<Transform> OnPositionChanged = delegate { };

        internal TransformationMatrix localMatrix;
        internal TransformationMatrix modelMatrix;
        internal bool matrixNeedsUpdate;
        private Transform parent;
        private Point relativePosition;
        private Point position;
        private Point scale;
        private float rotation;
        private float radRotation;
        private float zOrder;
        private readonly List<Transform> childrenElements;

        public Layer Owner { get; }
        public Transform Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (parent != value)
                {
                    if (parent != null)
                    {
                        parent.OnPositionChanged -= OnParentPositionChanged;
                        parent.OnChildRemoved(this);
                    }

                    parent = value;
                    parent.OnPositionChanged += OnParentPositionChanged;
                    Position = position;
                    parent.OnChildAdded(this);
                }
            }
        }
        public float ZOrder
        {
            get => zOrder;
            set
            {
                if (zOrder != value)
                {
                    zOrder = value;
                    OnZOrderChanged.Invoke(this);
                }
            }
        }
        public Point RelativePosition
        {
            get => relativePosition;
            set
            {
                relativePosition = value;
                if (parent != null)
                    position = relativePosition + parent.position;
                else
                    position = relativePosition;

                matrixNeedsUpdate = true;
                OnPositionChanged.Invoke(this);
            }
        }

        public Point Position
        {
            get => position;
            set
            {
                position = value;
                if (parent != null)
                    relativePosition = position - parent.position;
                else
                    relativePosition = position;

                matrixNeedsUpdate = true;
                OnPositionChanged.Invoke(this);
            }
        }

        public float Rotation
        {
            get => rotation;
            set
            {
                rotation = value % 360;
                radRotation = MathUtil.DegreesToRadians(rotation);
                matrixNeedsUpdate = true;
            }

        }
        public Point Scale
        {
            get => scale;
            set
            {
                if (scale == value)
                    return;

                scale = value;
                matrixNeedsUpdate = true;
            }
        }

        public Transform(Layer owner)
        {
            Scale = new Point(1, 1);
            localMatrix = TransformationMatrix.Identity;
            modelMatrix = TransformationMatrix.Identity;
            matrixNeedsUpdate = true;
            childrenElements = new List<Transform>();

            Owner = owner;
        }

        protected void OnChildAdded(Transform child)
        {
            childrenElements.Add(child);
            childrenElements.Sort((x, y) => y.ZOrder.CompareTo(x.ZOrder));

            child.OnZOrderChanged += OnChildZOrderChanged;
        }

        protected void OnChildRemoved(Transform child)
        {
            childrenElements.Remove(child);

            child.OnZOrderChanged += OnChildZOrderChanged;
        }

        protected void OnChildZOrderChanged(Transform child)
        {
            childrenElements.Sort((x, y) => x.ZOrder.CompareTo(y.ZOrder));
        }

        protected void OnParentPositionChanged(Transform parent)
        {
            // Set to the current value and update the absolute position based on parent position
            RelativePosition = relativePosition;
        }

        /*
        public void Translate(Point amount, TransformType relativeTo)
        {
            if (relativeTo == TransformType.World)
            {
                
                //var matrix = CalculateModelMatrix();
                //matrix.Invert();
                //amount = TransformationMatrix.TransformPoint(matrix, amount + Position); 
                //Still works relative to the layer, it does not move by world coordinates
            }
            else
                RelativePosition += amount;
        }*/

        /// <summary>
        /// Get the first layer starting from this layer by criterias
        /// </summary>
        /// <typeparam name="T">Layer specific type</typeparam>
        /// <param name="action">Layer criteria</param>
        /// <param name="excludeSelf">Exclude current layer</param>
        /// <returns>Layer with given criteria</returns>
        public T RunSingleResultQueryDFS<T>(Func<T, bool> action, bool excludeSelf = false) where T : Layer
        {
            Stack<Layer> queue = new Stack<Layer>();
            if (excludeSelf)
                for (int i = childrenElements.Count - 1; i >= 0; i--)
                    queue.Push(childrenElements[i].Owner);
            else
                queue.Push(Owner);

            while (queue.Count != 0)
            {
                Layer currLayer = queue.Pop();
                if (currLayer is T layerConverted && action.Invoke(layerConverted))
                    return layerConverted;

                for (int i = currLayer.Transform.childrenElements.Count - 1; i >= 0; i--)
                    queue.Push(currLayer.Transform.childrenElements[i].Owner);
            }

            return null;
        }

        /// <summary>
        /// Get a list of layers starting from this layer by criterias
        /// </summary>
        /// <typeparam name="T">Layer specific type</typeparam>
        /// <param name="action">Layer criteria</param>
        /// <param name="excludeSelf">Exclude current layer</param>
        /// <returns>List of Layers with given criteria</returns>
        public List<T> RunMultipleResultQueryDFS<T>(Func<T, bool> action, bool excludeSelf = false) where T : Layer
        {
            List<T> result = new List<T>();
            Stack<Layer> queue = new Stack<Layer>();
            if (excludeSelf)
                for (int i = childrenElements.Count - 1; i >= 0; i--)
                    queue.Push(childrenElements[i].Owner);
            else
                queue.Push(Owner);

            while (queue.Count != 0)
            {
                Layer currLayer = queue.Pop();
                if (currLayer is T layerConverted && action.Invoke(layerConverted))
                    result.Add(layerConverted);

                for (int i = currLayer.Transform.childrenElements.Count - 1; i >= 0; i--)
                    queue.Push(currLayer.Transform.childrenElements[i].Owner);
            }

            return result;
        }

        internal TransformationMatrix CalculateModelMatrix()
        {
            if (matrixNeedsUpdate)
                RecalculateLocalMatrix();

            TransformationMatrix transformationMatrix = localMatrix;
            Transform curr = Parent;

            while (curr != null)
            {
                if (curr.matrixNeedsUpdate)
                    curr.RecalculateLocalMatrix();

                transformationMatrix *= curr.localMatrix;
                curr = curr.Parent;
            }

            modelMatrix = transformationMatrix;
            return transformationMatrix;
        }

        internal void RecalculateLocalMatrix()
        {
            localMatrix = TransformationMatrix.Scaling(Scale.X, Scale.Y) * TransformationMatrix.Rotation(radRotation)
                                       * TransformationMatrix.Translation(RelativePosition);

            matrixNeedsUpdate = false;
        }

        /// <summary>
        /// Get children
        /// </summary>
        /// <returns>CHildren list</returns>
        public List<Transform> GetChildren()
        {
            return new List<Transform>(childrenElements);
        }
    }
}
