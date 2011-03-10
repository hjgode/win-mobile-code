using System;
using System.Collections.Generic;
using System.Text;


namespace ShakeDetection
{
    /// <summary>
    /// simple shake detect only checks accel>treshold but with a low cut filtered value
    /// </summary>
    public class ShakeClass4 : ShakeClass
    {
        #region Methods
        public ShakeClass4(string s)
        {
            base.name = s;
        }
        public override void addValues(GVector gv1)
        {
            
            //calc the acceleration or the longest vector
            double accel = gv1.Length;// Math.Sqrt(x * x + y * y + z * z);

            //we are not interested in directions
            accel = Math.Abs(accel);

            //a low cut filter
            double delta = Math.Abs(accel - accelLast);
            accel = accel * 0.9f + delta; 
            
            this.Logger(gv1.ToString());
            
            if (accel < _shakeTreshold &&  accelLast>_shakeTreshold)
            {
                this.OnShakeDetected(gv1);
                this.Logger("Shake detected: " + gv1.ToString());
            }
            accelLast = accel;

        }
        #endregion
        #region Properties
        private static double accelLast;
        private double _shakeTreshold = 1.2;
        public double shakeTreshold
        {
            get { return _shakeTreshold; }
            set { _shakeTreshold = value; }
        }
        #endregion
    }
}