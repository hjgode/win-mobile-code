using System;

namespace ShakeDetection
{
	public interface IShake
	{
//		event EventHandler<ShakeEventArgs> ShakeEventHandler;
        void OnShakeDetected(GVector gv);
		void addValues(GVector gv);
	}

}

