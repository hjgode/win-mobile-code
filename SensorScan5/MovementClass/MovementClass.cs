using System;
namespace Movedetection
{
    /// <summary>
    /// Main abstract class to implement various Shake detector classes
    /// </summary>
	public abstract class MovementClass:IMovement,IDisposable
    {
        #region Properties
        private string _name = "MovementClass";
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
		public MovementClass (String s)
		{
			this._name=s;
		}
        public MovementClass()
        {
            this._name = "MovementClass";
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
        /// the move detected event handler
        /// </summary>
        public event MoveDetectedEventHandler MoveDetected;
        /// <summary>
        /// the shake detected delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bre"></param>
        public delegate void MoveDetectedEventHandler(object sender, MovementEventArgs bre);
        private void MoveDetectedEvent(object mySender, MovementEventArgs myArgs)
        {
            if (this.MoveDetected == null)
                return;
            this.MoveDetected(this, myArgs);// this.movementEventArgs);
        }
        /// <summary>
        /// this eventhandler should be fired by the derived classes to signal a shake detection
        /// </summary>
        /// <param name="gv"></param>
        public void OnMoveDetected(GMVector gv)
        {
            MoveDetectedEvent(this, new MovementEventArgs(gv));
        }
        //========================================================================================
        /// <summary>
        /// the shake detected event handler
        /// </summary>
        public event IdleDetectedEventHandler IdleDetected;
        /// <summary>
        /// the shake detected delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="bre"></param>
        public delegate void IdleDetectedEventHandler(object sender, MovementEventArgs bre);
        private void IdleDetectedEvent(object mySender, MovementEventArgs myArgs)
        {
            if (this.IdleDetected==null)
                return;
            this.IdleDetected(this, myArgs ); //this.idleEventArgs);
        }
        /// <summary>
        /// this eventhandler should be fired by the derived classes to signal a shake detection
        /// </summary>
        /// <param name="gv"></param>
        public void OnIdleDetected(GMVector gv)
        {
            IdleDetectedEvent(this, new MovementEventArgs(gv));
        }
        #endregion
        #region Methods
        /// <summary>
        /// implement this with the actual vector and use OnShakeEvent to signal a 'Shake'
        /// </summary>
        /// <param name="gv"></param>
		public abstract void addValues(GMVector gv);
        public abstract void setTreshold(double high, double low);
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

