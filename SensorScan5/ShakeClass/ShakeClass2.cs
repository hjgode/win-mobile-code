using System;
using System.Collections.Generic;
using System.Text;

namespace ShakeDetection
{
    //http://www.google.com/codesearch/p?hl=en#L1VPiqAn_3g/trunk/simple/src/com/google/devtools/simple/runtime/components/impl/android/AccelerometerSensorImpl.java&q=Shaking%20package:http://simple\.googlecode\.com&sa=N&cd=1&ct=rc
    /**
     * <p>From the Android documentation:
     * <p>"All values are in SI units (m/s^2) and measure the acceleration applied
     * to the phone minus the force of gravity.
     * <br>values[0]: Acceleration minus Gx on the x-axis
     * <br>values[1]: Acceleration minus Gy on the y-axis
     * <br>values[2]: Acceleration minus Gz on the z-axis "
        Landing on a table, the values returned by the SensorEvent for the android phone should be :
           1. 0 m/s2 along x axis
           2. 0 m/s2 along y axis
           3. 9,80665 m/s2 along z axis
     */

    public class ShakeClass2 : ShakeClass
    {
        #region Methods
        /// <summary>
        /// This shake detection uses the current and last vector
        /// if the acceleration is above treshold and within a timeframe the 
        /// ShakeDetected event will be fired
        /// </summary>
        /// <param name="name"></param>
        public ShakeClass2(string name)
        {
            base.name = name;
        }
        public override void addValues(GVector gv)
        {
            now = DateTime.Now;
            //was written for android which uses a normal value of 9.81m/s²
            //cn50 comes with 1/10 G's
            gv = gv.Scale(10);
            double x = gv.X;
            double y = gv.Y;
            double z = gv.Z;
            double force;

            if (lastUpdate.Equals(0))
            {
                this.Logger("ignored due to no update");
                lastUpdate = now;
                lastShake = now;
                lastX = x;
                lastY = y;
                lastZ = z;
            }
            else
            {
                this.Logger("testing for shake");
                timeDiff = now - lastUpdate;
                this.Logger("timediff: "+timeDiff.ToString());
                if (!timeDiff.Equals(0))
                {
                    force = Math.Abs(x + y + z - lastX - lastY - lastZ)
                                / timeDiff.TotalSeconds;
                    
                    this.Logger("force: "+force.ToString() + "treshold: " +_shakeTreshold.ToString());
                    if (force > _shakeTreshold)
                    {
                        if (now - lastShake >= interval)
                        {
                            // trigger shake event
                            this.OnShakeDetected(gv);
                        }
                        lastShake = now;
                    }
                    lastX = x;
                    lastY = y;
                    lastZ = z;
                    lastUpdate = now;
                }
            }
        }
        #endregion
        //http://blog.androgames.net/85/android-accelerometer-tutorial/
        #region Properties
        private DateTime now, lastUpdate, lastShake;
        private TimeSpan timeDiff, interval = TimeSpan.FromSeconds(2);
        double lastX, lastY, lastZ;
        private double _shakeTreshold = 1.2;
        public double shakeTreshold
        {
            get { return _shakeTreshold; }
            set { _shakeTreshold = value; }
        }
        #endregion
    }
}