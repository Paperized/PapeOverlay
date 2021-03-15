using System;
using System.Collections.Generic;

namespace PapeOverlay.Core.Layers
{
    public static class LayerHelper
    {
        public static void RunLayerAction(Layer root, Action<Layer> action)
        {
            Stack<Layer> queue = new Stack<Layer>();
            queue.Push(root);

            while (queue.Count != 0)
            {
                Layer currLayer = queue.Pop();
                action.Invoke(currLayer);

                List<Transform> children = currLayer.Transform.GetChildren();
                for (int i = children.Count - 1; i >= 0; i--)
                    queue.Push(children[i].Owner);
            }
        }
    }
}
