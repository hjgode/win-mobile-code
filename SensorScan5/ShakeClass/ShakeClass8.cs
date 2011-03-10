using System;
using System.Collections.Generic;
using System.Text;

namespace ShakeDetection
{
	// http://mark.mymonster.nl/2010/10/24/shake-that-windows-phone-7-and-detect-it/
	public class ShakeClass8:ShakeClass
	{
		#region Properties
	    public double ShakeThreshold = 0.7;
		private bool firstcall=true; //helper var as GVector is not Nullable
	    private GVector _lastReading;
	    private int _shakeCount;
	    private bool _shaking;
		#endregion
        #region Methods
        /// <summary>
        /// shake detection compairing difference of last acceleration and current accel.
        /// if diffs of acceleration pairs exceeds min treshold and the number of shakes
        /// exceeds 1 then the ShakeDetected event will be fired
        /// </summary>
        public ShakeClass8 ()
		{
			base.name="ShakeClass8";
		}
		public ShakeClass8 (string s)
		{
			base.name=s;
		}
		public override void addValues (GVector gv)
		{
			GVector reading=gv;
		    if (!firstcall)// _lastReading != null)
            {
                if (!_shaking && CheckForShake(_lastReading, reading, ShakeThreshold) && _shakeCount >= 1)
                {
                    //We are shaking
                    _shaking = true;
                    _shakeCount = 0;
					OnShakeDetected(gv);
                    
                }
                else if (CheckForShake(_lastReading, reading, ShakeThreshold))
                {
                    _shakeCount++;
                }
                else if (!CheckForShake(_lastReading, reading, 0.2))
                {
                    _shakeCount = 0;
                    _shaking = false;
                }
            }
            _lastReading = reading;
			firstcall=false;

		}
	    private static bool CheckForShake(GVector last, GVector current,
	                                        double threshold)
	    {
	        double deltaX = Math.Abs((last.X - current.X));
	        double deltaY = Math.Abs((last.Y - current.Y));
	        double deltaZ = Math.Abs((last.Z - current.Z));
	 
	        return (deltaX > threshold && deltaY > threshold) ||
	                (deltaX > threshold && deltaZ > threshold) ||
	                (deltaY > threshold && deltaZ > threshold);
        }
        #endregion
    }
}

