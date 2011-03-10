using System;

namespace ShakeDetection
{
    public class ShakeClass7:ShakeClass
    {
        #region Methods
        /// <summary>
        /// shake detection using a list of directions
        /// if  acceleration is > treshold and current direction is != actual direction and
        /// number of detected shakes is > min number of shakes then 
        /// the ShakeDetected event will be fired
        /// </summary>
        /// <param name="s"></param>
        public ShakeClass7(string s) 
        {
            _minimumShakes=1;
            _shakeRecordList = new ShakeRecord[_minimumShakes];
            base.name = s;
        }
        public ShakeClass7(string s, int minShakes)
        {
            _minimumShakes = minShakes;
            _shakeRecordList = new ShakeRecord[minShakes];
        }

        public override void addValues(GVector gv)
		{
            //use new GVector 16 segmented direction
            GVector.Direction direction = gv.direction;

            double accel = gv.Length;// Math.Sqrt(e.X * e.X + e.Y * e.Y);// + e.Z*e.Z);
            //Does the currenet acceleration vector meet the minimum magnitude that we
            //care about?
            if (accel > MinimumAccelerationMagnitudeSquared)
            {
                //If the shake detected is in the same direction as the last one then ignore it
                //if ((direction & _shakeRecordList[_shakeRecordIndex].ShakeDirection) != GVector.Direction.None)// Direction.None)
                if((direction == _shakeRecordList[_shakeRecordIndex].ShakeDirection))
                    return;
                ShakeRecord record = new ShakeRecord();
                record.EventTime = DateTime.Now;
                record.ShakeDirection = direction;
                _shakeRecordIndex = (_shakeRecordIndex + 1)%_minimumShakes;
                _shakeRecordList[_shakeRecordIndex] = record;

                 CheckForShakes(gv);

            }
        }

        private void CheckForShakes(GVector gv)
        {
            int startIndex = (_shakeRecordIndex - 1);
            if (startIndex < 0) startIndex = _minimumShakes - 1;
            int endIndex = _shakeRecordIndex;
            if ((_shakeRecordList[endIndex].EventTime.Subtract(_shakeRecordList[startIndex].EventTime)) <= MinimumShakeTime)
            {
                Logger("Shake detected");
                OnShakeDetected(gv);
            }
        }
#endregion
        #region Properties
        private int _minimumShakes;
        ShakeRecord[] _shakeRecordList;
        private int _shakeRecordIndex = 0;
        private const double MinimumAccelerationMagnitude = 1.1;
        private const double MinimumAccelerationMagnitudeSquared = MinimumAccelerationMagnitude*MinimumAccelerationMagnitude;
        private static readonly TimeSpan MinimumShakeTime = TimeSpan.FromMilliseconds(500);

        private struct ShakeRecord
        {
            //public Direction ShakeDirection;
            public GVector.Direction ShakeDirection;
            public DateTime EventTime;
        }
        #endregion
    }
}
