#region Using 

using System;
using System.Net.Sockets;

#endregion

namespace OpenNETCF.Net.Bluetooth {
    /// <summary>
    /// BluetoothSocket 
    /// </summary>
    public class BluetoothSocket  : Socket{
        public BluetoothSocket() : base((AddressFamily)0x20, SocketType.Stream, ProtocolType.Ggp) {
        }
    }
}

  
