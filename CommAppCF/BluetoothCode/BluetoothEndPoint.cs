#region Using

using System;
using System.Net;
using System.Net.Sockets;

#endregion

namespace OpenNETCF.Net.Bluetooth {
    /// <summary>
    /// BluetoothEndPoint 
    /// </summary>
    public class BluetoothEndPoint : EndPoint{
        private static int m_addressoffset;
        private static int m_defaultport;
        private BluetoothAddress m_id;
        private int m_port;
        private static int m_portoffset;
        private static int m_salength;
        private Guid m_service;
        private static int m_serviceoffset;
        public const int MaxPort = 0xffff;
        public const int MinPort = 1;

        public BluetoothAddress Address {
            get {
                return this.m_id;
            }
            set {
                this.m_id = value;
            }
        }

        public override AddressFamily AddressFamily {
            get {
                return (AddressFamily)0x20;
            }
        }

        public int Port {
            get {
                return this.m_port;
            }
            set {
                this.m_port = value;
            }
        }

        public Guid Service {
            get {
                return this.m_service;
            }
            set {
                this.m_service = value;
            }
        }


        static BluetoothEndPoint() {
            m_addressoffset = 8;
            m_serviceoffset = 0x10;
            m_portoffset = 0x20;
            m_salength = 40;
        }


        public BluetoothEndPoint(BluetoothAddress address, Guid service)
            : this(address, service, m_defaultport) {
        }

        public BluetoothEndPoint(BluetoothAddress address, Guid service, int port) {
            this.m_id = address;
            this.m_service = service;
            this.m_port = port;
        }

        public override EndPoint Create(SocketAddress socketAddress) {
            int index;
            if (socketAddress == null) {
                throw new ArgumentNullException("socketAddress");
            }
            if (socketAddress[0] != 0x20) {
                return base.Create(socketAddress);
            }
            byte[] address = new byte[6];
            for (index = 0; index < 6; index++) {
                address[index] = socketAddress[m_addressoffset + index];
            }
            byte[] b = new byte[0x10];
            for (index = 0; index < 0x10; index++) {
                b[index] = socketAddress[m_serviceoffset + index];
            }
            byte[] buffer3 = new byte[4];
            for (index = 0; index < 4; index++) {
                buffer3[index] = socketAddress[m_portoffset + index];
            }
            return new BluetoothEndPoint(new BluetoothAddress(address), new Guid(b), BitConverter.ToInt32(buffer3, 0));
        }

        public override bool Equals(object obj) {
            BluetoothEndPoint point = obj as BluetoothEndPoint;
            if (point == null) {
                return base.Equals(obj);
            }
            if (this.Address.Equals(point.Address)) {
                return this.Service.Equals(point.Service);
            }
            return false;
        }

        public override int GetHashCode() {
            return this.Address.GetHashCode();
        }

        public override SocketAddress Serialize() {
            SocketAddress address = new SocketAddress((AddressFamily)0x20, m_salength);
            address[0] = 0x20;
            if (this.m_id != null) {
                byte[] buffer = this.m_id.ToByteArray();
                for (int i = 0; i < 6; i++) {
                    address[i + m_addressoffset] = buffer[i];
                }
            }
            if (this.m_service != Guid.Empty) {
                byte[] buffer2 = this.m_service.ToByteArray();
                for (int j = 0; j < 0x10; j++) {
                    address[j + m_serviceoffset] = buffer2[j];
                }
            }
            byte[] bytes = BitConverter.GetBytes(this.m_port);
            for (int k = 0; k < 4; k++) {
                address[k + m_portoffset] = bytes[k];
            }
            return address;
        }

    }
}
