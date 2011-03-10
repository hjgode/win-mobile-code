using System;
using System.Collections.Generic;
using System.Text;

namespace Movedetection
{
    public interface IMovement
    {
        //		event EventHandler<ShakeEventArgs> ShakeEventHandler;
        void OnIdleDetected(GMVector gv);
        void OnMoveDetected(GMVector gv);
        void addValues(GMVector gv);
    }

}