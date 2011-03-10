using System;
using System.Collections.Generic;
using System.Text;

namespace ShakeDetection
{
    /// <summary>
    /// very simple shake detect function
    /// it just checks if acceleration is greater than treshold
    /// </summary>
    public class ShakeClass3 : ShakeClass
    {
        #region Methods
        public ShakeClass3(string name)
        {
            base.name = name;
        }
        public override void addValues(GVector gv)
        {
            //calc the acceleration or the longest vector
            double accel = gv.Length; //Math.Sqrt(x * x + y * y + z * z);
            this.Logger(gv.ToString());
            if (accel > _shakeTreshold)
            {
                this.Logger("Shake detected");
                this.OnShakeDetected(gv); //fire the event in the base class
            }
        }
        #endregion
        #region Properties
        private double _shakeTreshold = 1.2;
        public double shakeTreshold
        {
            get { return _shakeTreshold; }
            set { _shakeTreshold = value; }
        }
        #endregion
    }
}