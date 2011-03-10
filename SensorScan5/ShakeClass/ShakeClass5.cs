using System;
using System.Collections.Generic;
using System.Text;


namespace ShakeDetection
{
    public class ShakeClass5 : ShakeClass
    {
        #region Methods
        /// <summary>
        /// simple shake detect function
        /// it just checks if acceleration between two readings is greater than treshold
        /// </summary>
        public ShakeClass5(string s)
        {
            base.name = s;
        }
        public override void addValues(GVector gv)
        {
            //private static int SHAKE_THRESHOLD = 800;

            long lastUpdate = 0;
            long curTime = System.DateTime.Now.Millisecond;
            // only allow one update every 100ms.
            if ((curTime - lastUpdate) > 100)
            {
                long diffTime = (curTime - lastUpdate);
                lastUpdate = curTime;

                double speed = Math.Abs(gv.X + gv.Y + gv.Z - last_x - last_y - last_z) / diffTime * 10000;

                //adjust for CN50
                speed = speed / 10;
                if (speed > _shakeTreshold)
                {
                    this.Logger("Shake detected");
                    this.OnShakeDetected(gv);
                }
                else
                {
                    //System.Diagnostics.Debug.WriteLine("sensor : shake speed: " + speed.ToString());
                }
                gvLast = gv;
                last_x = gv.X;
                last_y = gv.Y;
                last_z = gv.Z;
            }
            else
                this.Logger("sensor : timespan to low ");
        }
        #endregion
        #region Properties
        // http://www.codeshogun.com/blog/category/mobile/
        static double last_x = 0;
        static double last_y = 0;
        static double last_z = 0;
        static GVector gvLast = new GVector(0, 0, 0);
        private double _shakeTreshold = 800;
        public double shakeTreshold
        {
            get { return _shakeTreshold; }
            set { _shakeTreshold = value; }
        }

        #endregion
        
    }
}