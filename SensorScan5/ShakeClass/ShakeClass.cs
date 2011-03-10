using System;
namespace ShakeDetection
{
    /// <summary>
    /// Main abstract class to implement various Shake detector classes
    /// </summary>
	public abstract class ShakeClass:IShake,IDisposable
    {
        #region Properties
        private string _name = "ShakeClass";
        /// <summary>
        /// used to identify multiple classes that inherited this class
        /// </summary>
		public string name{
            get{return _name;}
            set { _name = value; }
        }
        /// <summary>
        /// generic constructor
        /// </summary>
        /// <param name="s"></param>
		public ShakeClass (String s)
		{
			this._name=s;
		}
        public ShakeClass()
        {
            this._name = "ShakeClass";
        }
        private bool _logEnabled = false;
        public bool logEnabled
        {
            get { return _logEnabled; }
            set { _logEnabled = value; }
        }
        private bool _logToFile = false;
        /// <summary>
        /// enable logging to file
        /// </summary>
        public bool logToFile
        {
            get { return _logToFile; }
            set { _logToFile = value; }
        }
        #endregion
        /// <summary>
        /// implemented for completeness
        /// </summary>
        public void Dispose(){
        }

        #region Events
        /// <summary>
        /// the shake detected event handler
        /// </summary>
        public event ShakeDetectedEventHandler ShakeDetected;
        /// <summary>
        /// the shake detected delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bre"></param>
        public delegate void ShakeDetectedEventHandler(object sender, ShakeEventArgs bre);
        private void ShakeDetectedEvent(object mySender, ShakeEventArgs myArgs)
        {
            if (this.ShakeDetected == null)
                return;
            this.ShakeDetected(this, myArgs);
        }
        /// <summary>
        /// this eventhandler should be fired by the derived classes to signal a shake detection
        /// </summary>
        /// <param name="gv"></param>
        public void OnShakeDetected(GVector gv)
        {
            ShakeDetectedEvent(this, new ShakeEventArgs(gv));
        }

        #endregion
        #region Methods
        /// <summary>
        /// implement this with the actual vector and use OnShakeEvent to signal a 'Shake'
        /// </summary>
        /// <param name="gv"></param>
		public abstract void addValues(GVector gv);
        #endregion
        #region Logging
        public void Logger(string s)
        {
            if (!_logEnabled)
                return;
            System.Diagnostics.Debug.WriteLine(_name + ": " + s);
            if (_logToFile)
            {
                string sFileName = @"\" + _name + ".log.txt";
                System.IO.TextWriter tw = new System.IO.StreamWriter(sFileName, true);
                //tw.WriteLine(X.ToString() + "," + Y.ToString() + "," + Z.ToString() + "," + AngleX.ToString() + "," + AngleY.ToString());
                tw.WriteLine(_name + ": " + s);
                tw.Close();
            }
        }
        #endregion

	}
}

