using PapeOverlay.Core.Layers;
using System;

namespace PapeOverlay.Core.Exceptions
{
    public class PropertyNotFoundException : Exception
    {
        public Layer layer;
        public string PropertyName { get; set; }

        public PropertyNotFoundException(string propertyName, Layer layer) : this(propertyName, layer, null)
        {
            
        }

        public PropertyNotFoundException(string propertyName, Layer layer, Exception inner)
            : base($"There is no {propertyName} property in \"{layer.Name}\" Layer!", inner)
        {

        }
    }
}
