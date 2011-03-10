using System;
using System.Collections.Generic;
using System.Text;

namespace ShakeDetection
{
	// http://mark.mymonster.nl/2010/10/24/shake-that-windows-phone-7-and-detect-it/
	public class ShakeClass9:ShakeClass
	{
		#region Properties
		private bool firstcall=true; //helper var as GVector is not Nullable

        private Queue<double> myDeltaG;
        private Queue<GVector> myGVqueue;
        private int _queueSize = 20;

        private double myGmin;
        public double _Gmin
        {
            set { myGmin = value; }
        }
        private double myTmin;
        public double _Tmin
        {
            set { myTmin = value; }
        }

        #endregion
        #region Methods
        /// <summary>
        /// shake detection compairing difference of last acceleration and current accel.
        /// if diffs of acceleration pairs exceeds min treshold and the number of shakes
        /// exceeds 1 then the ShakeDetected event will be fired
        /// </summary>
        public ShakeClass9 ()
		{
			base.name="ShakeClass9";
            init();
		}
		public ShakeClass9 (string s)
		{
			base.name=s;
            init();
		}
        private void init()
        {
            myDeltaG = new Queue<double>(_queueSize);
            myGVqueue = new Queue<GVector>(_queueSize);
            myGmin = 0.9d;
            myTmin = 15; //equals 0.75 seconds if samples per seconds is 20
        }
		public override void addValues (GVector gv)
		{
			GVector reading=gv;
            myGVqueue.Enqueue(gv);
            if (!firstcall)// _lastReading != null)
            {
                if (myDeltaG.Count == _queueSize)
                {
                    myDeltaG.Dequeue();
                }
                if (myGVqueue.Count == _queueSize)
                {
                    myGVqueue.Dequeue();
                }

                //get and store delta
                GVector gvOld = myGVqueue.Peek();
                double deltaG = Math.Abs(gv.Length - gvOld.Length);
                myDeltaG.Enqueue(deltaG);
                //get average
                double sum = 0;
                foreach (double d in myDeltaG)
                {
                    sum += d;
                }
                double DeltaAverage = sum / myDeltaG.Count;
                //rapid movement detection
                if (DeltaAverage > myGmin)
                    if (myDeltaG.Count > myTmin)
                        OnShakeDetected(gv);
            }
            else
            {
                myDeltaG.Enqueue(0.9d); //enqueu first value
                firstcall = false;
            }
		}
        #endregion
    }
}

