using System;
namespace Movedetection
{
    public class MovementEventArgs : EventArgs
    {
        /* HACK: fields are typically private, but making this internal so it
         * can be accessed from other classes. In practice should use properties.
         */
        internal GMVector _gmvector;
        public MovementEventArgs()
        {
        }
        public MovementEventArgs(GMVector gmvector)
            : base()
        {
            this._gmvector = gmvector;
        }
    }
}

