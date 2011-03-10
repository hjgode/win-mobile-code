using System;
using System.Collections.Generic;
using System.Text;

namespace Movedetection
{
    /// <summary>
    /// chalenge: recognize a gesture pattern
    /// Detect movement (Accelerometer_WBSN.pdf http://www.google.co.uk/url?sa=t&source=web&cd=6&ved=0CDwQFjAF&url=http%3A%2F%2Fciteseerx.ist.psu.edu%2Fviewdoc%2Fdownload%3Fdoi%3D10.1.1.120.296%26rep%3Drep1%26type%3Dpdf&ei=wTMHTf_NL8e3hQf-oZXtBw&usg=AFQjCNGgZnlYRvBzKg4uJRveSwVHhxIa-g&sig2=URThwmeDoZF2JdCMj2Babg)
    /// 1. calc the actual acceleration: Length=sqrt(x*x+y*y+z*z)
    /// 2. get the diff to the previous DeltaAcceleration: LengthNow - LengthPrevious
    /// 3. get the average of deltas for about 1 sec (should be 20 measurements): SUM(DeltaAccelerations)/sps; //sps=samples per second
    /// </summary>
    class MovementClass1:MovementClass
    {

        public MovementClass1(string s)
        {
            base.name = s;
            this.setTreshold(.2d, 0.5d);
            myQueue = new LimitedQueue<GMVector>(_queueLength);

            myAverages = new LimitedQueue<gRMSAverage>(_queueLength);

            //need at least 3 seconds recording size, but only record crossing values
            myCrossUPs = new LimitedQueue<gRMSAverage>(_queueLength);
            myCrossDOWNs = new LimitedQueue<gRMSAverage>(_queueLength);

            myGmin = 0.2d; //was 0.9d but we get a deltaAverage of about around 0.14
            myTmin = 10; // 15 is equal to 0.75 seconds if samples per seconds is 20

            _treshCountRMS = 2000;  //should be 3000 for 60 samples in 3 seconds at 20 samples/second,
                                 //BUT we only see ~2 samples per second

            basicLogger("Movement1 Class\r\nx\ty\tz\ttick\tdeltaG");
        }

        public override void setTreshold(double high, double low)
        {
            myGmin = high;
            myRMSmin = low;
        }

        public override void addValues(GMVector gv)
        {
            GMVector gvOld;
            if (myQueue.Count>0)// !_firstCall)
            {
                //_samplesPerSecond=
                //calc samples per second
                if (!_samplesPerSecondsCalulated)
                {
                    if (myQueue.Count > 10)
                    {
                        GMVector[] myQueueArray = myQueue.ToArray();
                        ulong iSamplesCount = 0;
                        ulong iTickSum = 0;
                        ulong tickDiff = 0;
                        for (int c = myQueueArray.Length - 1; c > 1; c--)
                        {
                            iSamplesCount++;
                            tickDiff = myQueueArray[c].Ticks - myQueueArray[c - 1].Ticks;
                            iTickSum += tickDiff;
                        }
                        _samplesPerSecond = (uint)(1000 / (iTickSum / iSamplesCount)); // number of samples per second, ticks stored as milliseconds
                        _samplesPerSecondsCalulated = true;
                    }
                }
                //get and store delta
                gvOld = myQueue.Peek();
                //calculate deltaG for current and last sample
                double deltaG = Math.Abs(gv.Length-gvOld.Length);

                //save for later use
                gRMSAverage currentRMS=new gRMSAverage(deltaG, gv.Ticks);
                myAverages.Enqueue(currentRMS);

                basicLogger(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", 
                    gv.X, gv.Y, gv.Z, 
                    gv.Ticks,
                    deltaG));

                /* Theory:
                 * RSD: Rapid Shake Detection
                 * acceleration change is the sum of the delta G between consecutive samples divided
                 * by the number of samples per second
                 * Drastic Movement
                 *  average acceleration change exceeds about 0.9G per sample (50ms period)
                 *  calculated over the last .75 seconds
                 * Sustained Movement
                 *  more fequently exceed an acceleration of about 0.5G (0.5G acceleration change per sample)
                 *  exceed 0.5G value within time frame for non-consecutive samples (cross counter)
                 *  time frame is set to reset after 60 non-consecutive (about 3 seconds at 50ms per sample) samples below 0.5G
                */

                //test for condition1
                bool bCondition1=false;
                if (myAverages.Count > 1)
                {
                    bCondition1 = condition1(currentRMS);
                }

                //test for condition2
                bool bCondition2=false;
                if (myAverages.Count > 1)
                {
                    gRMSAverage lastRMS = myAverages.Peek();
                    bCondition2 = condition2(currentRMS, lastRMS);
                }

                //===============================================================================================
                //start when queue is filled
                if (myQueue.Count >= 20)
                {
                    //rapid shake detection (RSD)
                    //Condition1: have at least an average of 0.9G for at least .75 second
                    if(bCondition1 & bCondition2)
                        OnMoveDetected(gv);
                }//start with filled queue
            }
            //else
            //    _firstCall = false;

            myQueue.Enqueue(gv);

            gvOld = gv;
        }

        /* Theory:
         * RSD: Rapid Shake Detection
         * acceleration change is the sum of the delta G between consecutive samples divided
         * by the number of samples per second
         * Drastic Movement (condition1)
         *  average acceleration change exceeds about 0.9G per sample (50ms period)
         *  calculated over the last .75 seconds
         */
        /// <summary>
        /// deltaG above 0.9G for at least .75 seconds
        /// </summary>
        /// <returns></returns>
        private bool condition1(gRMSAverage currentRMS)
        {
            if (!_samplesPerSecondsCalulated)
                return false;

            bool bRet1 = false;
            //get average of values of last .75 seconds, tick diff is about 10ms between samples
            double sum = 0;
            int iSamples = 0;
            
            foreach (gRMSAverage gRMS in myAverages)
            {
                if (currentRMS.tick - gRMS.tick < 75 * 1000) //the ticks saved are milliseconds, .75seconds=75000 milliseconds
                {
                    sum += gRMS.deltaG;
                    iSamples++;
                }
            }
            //sum of deltas divided by samples per second
            double DeltaAverage = sum / _samplesPerSecond;// (iSamples * 4 / 3);

            Avg_deltaG_rms = DeltaAverage;

            //call back for information, the caller reads _Avg_deltaG_rms
            OnIdleDetected(new GMVector(0, 0, 0));

            //how many samples exceed the Gmin treshold
            int iCountG = 0;
            foreach (GMVector g in myQueue)
            {
                if (g.Length > DeltaAverage)
                    iCountG++;
            }
            //Condition1: have at least an average of 0.9G for at least .75 second
            if (DeltaAverage > myGmin){
                if (iCountG > myTmin)
                {
                    //delete all entries, RESET
                    myQueue.Clear();
                    //_firstCall = true;

                    //OnMoveDetected(gv);
                    bRet1 = true;
                }
            }
            return bRet1;
        }

        /// <summary>
        /// deltaG above .5G for more than 3 seconds and 
        /// without deltaG below .5G within last 3 seconds
        /// </summary>
        /// <param name="currentRMS"></param>
        /// <returns></returns>
        private bool condition2(gRMSAverage currentRMS, gRMSAverage lastRMS){

            if (!_samplesPerSecondsCalulated)
                return false;

            bool bRet = false;
            crossDirection crossDir = crossDirection.unknown;
            if (currentRMS.deltaG > myRMSmin && lastRMS.deltaG < myRMSmin)
            {
                //crossUp
                crossDir = crossDirection.crossUP;
            }
            else if (currentRMS.deltaG < myRMSmin && lastRMS.deltaG > myRMSmin)
            {
                //crossDown
                crossDir = crossDirection.crossDown;
            }

            if (myCrossDOWNs.Count > 0)
            {
                gRMSAverage lastRMSup = myCrossDOWNs.Peek();
                if (crossDir == crossDirection.crossUP && (currentRMS.tick - lastRMSup.tick) >= _treshCountRMS)
                {
                    if (myCrossUPs.Count > 0)
                    {
                        gRMSAverage lastRMSdown = myCrossDOWNs.Peek();
                        if ((currentRMS.tick - lastRMSdown.tick) > _treshCountRMS)
                            bRet = true;
                    }
                }
            }
            //enqueue current values
            if (crossDir == crossDirection.crossUP)
                myCrossUPs.Enqueue(currentRMS);
            if (crossDir == crossDirection.crossDown)
                myCrossDOWNs.Enqueue(currentRMS);
            return bRet;
        }
#region fields
        private uint _samplesPerSecond = 20;
        private bool _samplesPerSecondsCalulated = false;

        private enum crossDirection : int
        {
            unknown = 0,
            crossUP = 1,
            crossDown = -1,
        }

        private class gRMSAverage
        {
            private double _deltaG;
            public double deltaG
            {
                get { return _deltaG; }
            }
            private ulong _tick;
            public ulong tick
            {
                get { return _tick; }
            }
            public gRMSAverage(double delta, ulong tick)
            {
                _deltaG = delta;
                _tick = tick;
            }
        }

        //private static bool _firstCall = true;
        
        /// <summary>
        /// store a list of acceleration data
        /// </summary>
        private LimitedQueue<GMVector> myQueue;
        
        /// <summary>
        /// store a list of relevant delta acceleration data and ticks
        /// </summary>
        private LimitedQueue<gRMSAverage> myAverages;
        
        /// <summary>
        /// queue to record deltaG averages crossing from deltaG from <.5G to >.5G
        /// </summary>
        private LimitedQueue<gRMSAverage> myCrossUPs;

        /// <summary>
        /// queue to record deltaG averages crossing from deltaG from >.5G to <.5G
        /// </summary>
        private LimitedQueue<gRMSAverage> myCrossDOWNs;
    
        private int _queueLength = 20; //20 samples per second will be great

        /// <summary>
        /// how long should the 0.5 treshold be met
        /// </summary>
        private uint _treshCountRMS = 3000; //should be normally 3 seconds
        public uint treshCountRMS
        {
            get { return _treshCountRMS; }
        }
        
        /// <summary>
        /// minimum G to exceed for shake detection
        /// </summary>
        private double myGmin=0.9;
        public double _Gmin
        {
            set { myGmin = value; }
        }

        /// <summary>
        /// minimum G to exceed for sustained movement detection
        /// </summary>
        private double myRMSmin = 0.5;
        public double _RMSmin
        {
            get { return myRMSmin; }
            set { myRMSmin = value; }
        }

        private double myTmin;
        public double _Tmin
        {
            set { myTmin = value; }
        }

        private double Avg_deltaG_rms;
        public double _Avg_deltaG_rms
        {
            get { return Avg_deltaG_rms; }
        }
#endregion
        public static void basicLogger(string s)
        {
            string sFileName = @"\basicxyz.log.txt";
            System.IO.TextWriter tw = new System.IO.StreamWriter(sFileName, true);
            //tw.WriteLine(X.ToString() + "," + Y.ToString() + "," + Z.ToString() + "," + AngleX.ToString() + "," + AngleY.ToString());
            tw.WriteLine(s);
            tw.Flush();
            tw.Close();
        }

    }
}
