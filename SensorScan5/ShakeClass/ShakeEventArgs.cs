using System;
namespace ShakeDetection
{
    public class ShakeEventArgs : EventArgs
    {
        /* HACK: fields are typically private, but making this internal so it
         * can be accessed from other classes. In practice should use properties.
         */
        internal GVector _gvector;
        public ShakeEventArgs()
        {
        }
        public ShakeEventArgs(GVector gvector)
            : base()
        {
            this._gvector = gvector;
        }
    }
}

