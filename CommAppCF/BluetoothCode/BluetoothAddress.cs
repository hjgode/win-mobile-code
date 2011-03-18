#region Using

using System;

#endregion

namespace OpenNETCF.Net.Bluetooth {
    /// <summary>
    /// BluetoothAddress 
    /// </summary>
    public sealed class BluetoothAddress : IComparable {

        private byte[] data;
        public const int Giac = 0x9e8b33;
        internal const int IacFirst = 0x9e8b00;
        internal const int IacLast = 0x9e8b3f;
        public const int Liac = 0x9e8b00;
        public static readonly BluetoothAddress None = new BluetoothAddress();

        static BluetoothAddress() {
            None = new BluetoothAddress();
        }

        internal BluetoothAddress() {
            this.data = new byte[8];
        }

        public BluetoothAddress(byte[] address) : this() {
            if (address.Length != 6) {
                throw new ArgumentException("address");
            }
            Buffer.BlockCopy(address, 0, this.data, 0, 6);
        }
        public BluetoothAddress(long address) : this() {
            BitConverter.GetBytes(address).CopyTo(this.data, 0);
        }

        #region IComparable 

        int IComparable.CompareTo(object obj) {
            BluetoothAddress address = obj as BluetoothAddress;
            if (address != null) {
                return this.ToInt64().CompareTo(address.ToInt64());
            }
            return -1;

        }
        #endregion

        public long ToInt64() {
            return BitConverter.ToInt64(this.data, 0);
        }

        public byte[] ToByteArray() {
            return this.data;
        }



    }
}
