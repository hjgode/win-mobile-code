using System;
using System.Collections.Generic;
using System.Text;


namespace ShakeDetection
{
    public class ShakeClass6 : ShakeClass
    {
        /// <summary>
        /// shake detection using vector difference (distance) and counters
        /// </summary>
        /// <param name="s"></param>
        public ShakeClass6(string s)
        {
            base.name = s;
        }
        #region Properties
        private int DetectWindowCount = 0;
        private int ResetWindowCount = 0;
        /// <summary>
        /// how many shakes will trigger a shake event
        /// </summary>
        public int detectwindow=3;
        /// <summary>
        /// how many shakes will reset the shake counting
        /// </summary>
        public int resetwindow = 2;

        private double _shakeTreshold = 1.2;
        /// <summary>
        /// what will be counted as fast shake
        /// </summary>
        public double shakeTreshold
        {
            get { return _shakeTreshold; }
            set { _shakeTreshold = value; }
        }
        private GVector gvOld;
        private int FirstTimeEntryFlag=0;
        #endregion
        #region Methods
        public override void addValues(GVector gvNew)
        {
            if (FirstTimeEntryFlag != 0)
            {
                //Set new values to old
                gvOld = gvNew;

                //calculate Euclidean distance between old and data points
                double EuclideanDistance = gvNew.EuclideanDistance(gvOld);// Math.Sqrt(Math.Pow(X - Xold, 2) + Math.Pow(Y - Yold, 2) + Math.Pow(Z - Zold, 2));

                //set shake to true if distance between data points is greater than the defined threshold 
                if (EuclideanDistance > _shakeTreshold)
                {
                    //System.Diagnostics.Debug.WriteLine("DetectShake : shake detected w/ speed: " + EuclideanDistance.ToString());
                    DetectWindowCount++;
                    if (DetectWindowCount > detectwindow) { 
                        DetectWindowCount = 0; 
                        ResetWindowCount = 0;
                        Logger("Shake detected");
                        OnShakeDetected(gvNew); 
                    }
                }
                else
                {
                    //movement to low
                    //System.Diagnostics.Debug.WriteLine("DetectShake : speed to low: " + EuclideanDistance.ToString());
                    ResetWindowCount++;
                    if (ResetWindowCount > resetwindow) {
                        DetectWindowCount = 0; 
                        ResetWindowCount = 0;
                        Logger("Windows counts reset");
                        return; //no shake 
                    }
                }

            }
            //no longer the first run.
            FirstTimeEntryFlag = 1;
        }
        #endregion
    }
}