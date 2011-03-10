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
        Landing on a table, the values returned by the SensorEvent for the phone should be :
           1. 0 m/s2 along x axis
           2. 0 m/s2 along y axis
           3. 9,80665 m/s2 along z axis
     */

    public class ShakeClass1: ShakeClass
    {
        #region Properties
        private double _shakeTreshold = 4; //default was 8
        public double shakeTreshold
        {
            get { return _shakeTreshold; }
            set { _shakeTreshold = value; }
        }
        private int _sensor_cache_size = 10;

        private Queue<double> _X_Cache;
        private Queue<double> _Y_Cache;
        private Queue<double> _Z_Cache;

        #endregion
        #region Methods
        /// <summary>
        /// This shake detection code uses three queues to fill them with x,y and z values
        /// if the new added value exceeds the average of the queue by a treshold, the
        /// shakedDetected event will be fired
        /// </summary>
        /// <param name="name"></param>
        public ShakeClass1(string name)
        {
            base.name = name;
            _X_Cache=new Queue<double>(_sensor_cache_size);
            _Y_Cache=new Queue<double>(_sensor_cache_size);
            _Z_Cache=new Queue<double>(_sensor_cache_size);
        }
        /// <summary>
        /// this function uses every direction for its own shake detection
        /// </summary>
        /// <param name="xNew"></param>
        /// <param name="yNew"></param>
        /// <param name="zNew"></param>
        public override void addValues(GVector gv){
            //code is from android where 9.8m/s² is normal
            //cn50 gives 0.98m/s² so we multiply the CN50 values with 10
            gv = gv.Scale(10);

            addToSensorCache(_X_Cache, gv.X);
            addToSensorCache(_Y_Cache, gv.Y);
            addToSensorCache(_Z_Cache, gv.Z);

            this.Logger(gv.ToString());
            if(isShaking(_X_Cache, gv.X) ||
                isShaking(_Y_Cache, gv.Y) ||
                isShaking(_Z_Cache, gv.Z) )
            {
                this.OnShakeDetected(gv);
            }
        }
       
        private void addToSensorCache(Queue<double> cache, double currentValue)
        {
            if (cache.Count > _sensor_cache_size)
                cache.Dequeue();
            //add only positive values
            cache.Enqueue(Math.Abs(currentValue));
        }

        private bool isShaking(Queue<double> cache, double currentValue)
        {
            double average = 0;
            foreach (double d in cache)
            {
                average += d;
            }
            average = average / cache.Count;
            this.Logger(String.Format("ShakeClass1: isShaking: average: {0} current: {1} treshold:{2} diff:{3}", average, currentValue, _shakeTreshold, Math.Abs(average - currentValue)));
            return Math.Abs(average - Math.Abs(currentValue)) > _shakeTreshold;
        }
        #endregion
    }
}
