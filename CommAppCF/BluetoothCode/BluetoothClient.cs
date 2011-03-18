//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.BluetoothClient
//		Copyright (C) 2003-2004, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using System.Collections;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace OpenNETCF.Net.Bluetooth {
    /// <summary>
    /// Contains functionality to control the state of the Bluetooth hardware on a Smart Device.
    /// </summary>
    /// <remarks>This class currently only supports devices which use the Microsoft Bluetooth stack such as the Orange SPV E200, devices which use the WidComm stack will not work.</remarks>
    public class BluetoothClient {
        /*add by qiuq*/
        private bool authenticate;
        private bool encrypt;
        private short queryLength;
        private Socket socket;

        /// <summary>
        /// Creates a new instance of BluetoothClient.
        /// </summary>
        public BluetoothClient() {
            //want to initialise winsock 2.2
            short ver = 0x0202;

            byte[] data = new byte[512];
            int result = WSAStartup(ver, data);

            //MessageBox.Show(System.Text.Encoding.ASCII.GetString(data, 4, 257));

            if (result != 0) {
                throw new Exception(result.ToString());
            }
            /*add by qiuq*/
            try {
                this.socket = new BluetoothSocket();
            }
            catch (SocketException) {
                throw new PlatformNotSupportedException("Bluetooth.NET is not supported on this device.");
            }

        }
        /// <summary>
        /// Creates a new instance of BluetoothClient.(add by qiuq)
        /// </summary>
        /// <param name="s"></param>
        internal BluetoothClient(Socket s) {
            this.authenticate = false;
            this.encrypt = false;
            this.queryLength = 0x10;
            this.socket = s;
        }

        public void CloseSocket() {
            if ((this.socket != null) && this.socket.Connected) {
                this.socket.Close();
            }
        }

        public void Connect(BluetoothEndPoint ep) {
            this.CloseSocket();
            if (this.socket == null) {
                this.socket = new BluetoothSocket();
            }
            this.socket.Connect(ep);
        }

        public NetworkStream GetStream() {
            if (this.socket == null) {
                throw new ObjectDisposedException("Client", "The BluetoothClient has been closed.");
            }
            return new NetworkStream(this.socket);
        }



        /// <summary>
        /// Gets or Sets the current mode of operation of the Bluetooth radio.
        /// </summary>
        /// <remarks>This setting will be persisted when the device is reset.
        /// An Icon will be displayed in the tray on the Home screen and the device will emit a blue LED when Bluetooth is enabled.</remarks>
        public static RadioMode RadioMode {
            get {
                RadioMode mode = 0;
                //get the mode
                int result = BthGetMode(ref mode);

                //if successful return retrieved value
                if (result != 0) {
                    throw new ExternalException("Error getting Bluetooth mode");

                }

                //return setting
                return mode;
            }
            set {
                //set the status
                int result = BthSetMode(value);

                //check for error
                if (result != 0) {
                    throw new ExternalException("Error setting Bluetooth mode");
                }
            }
        }

        /// <summary>
        /// Discovers accessible Bluetooth devices and returns their names and addresses.
        /// </summary>
        /// <returns>An array of BluetoothDeviceInfo objects describing the devices discovered.</returns>
        public BluetoothDeviceInfo[] DiscoverDevices() {
            ArrayList al = new ArrayList();
            try {
                int handle = 0;
                byte[] queryset = new byte[1024];
                BitConverter.GetBytes((int)60).CopyTo(queryset, 0);
                BitConverter.GetBytes((int)16).CopyTo(queryset, 20);

                int bufferlen = 1024;

                int lookupresult = 0;

                //start looking for Bluetooth devices
                lookupresult = BthNsLookupServiceBegin(queryset, LookupFlags.Containers, ref handle);

                while (lookupresult != -1) {

                    lookupresult = BthNsLookupServiceNext(handle, LookupFlags.ReturnAddr | LookupFlags.ReturnName, ref bufferlen, queryset);
                    if (lookupresult != -1) {
                        //pointer to outputbuffer
                        int bufferptr = BitConverter.ToInt32(queryset, 48);
                        //remote socket address
                        int sockaddrptr = Marshal.ReadInt32((IntPtr)bufferptr, 8);
                        //remote socket len
                        int sockaddrlen = Marshal.ReadInt32((IntPtr)bufferptr, 12);

                        BluetoothSocketAddress btsa = new BluetoothSocketAddress();

                        Marshal.Copy((IntPtr)sockaddrptr, btsa.ToByteArray(), 0, BluetoothSocketAddress.Length);

                        //new deviceinfo
                        BluetoothDeviceInfo newdevice = new BluetoothDeviceInfo(btsa.Address, Marshal.PtrToStringUni((IntPtr)BitConverter.ToInt32(queryset, 4)));

                        //add to discovered list
                        al.Add(newdevice);

                    }
                }

                //stop looking
                lookupresult = BthNsLookupServiceEnd(handle);

                //return results
                
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("Exception in DiscoverDevices(): " + ex.Message);
            }
            return (BluetoothDeviceInfo[])al.ToArray(typeof(BluetoothDeviceInfo));
        }

        public void Close() {
            int result = WSACleanup();

            if (result != 0) {
                throw new Exception(result.ToString());
            }
        }

        public static int ConnectSerial(int portindex, PortEmuParams pep) {
            return RegisterDevice("COM", portindex, "btd.dll", pep.ToByteArray());
        }

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int RegisterDevice(
            string lpszType,
            int dwIndex,
            string lpszLib,
            byte[] dwInfo);

        [DllImport("coredll.dll")]
        private static extern bool DeregisterDevice(
            int handle);

        [DllImport("BthUtil.dll")]
        private static extern int BthSetMode(
            RadioMode dwMode);

        [DllImport("BthUtil.dll")]
        private static extern int BthGetMode(
            ref RadioMode dwMode);


        [DllImport("ws2.dll")]
        private static extern int WSAStartup(
            short versionrequested,
            byte[] wsadata);
        [DllImport("ws2.dll")]
        private static extern int WSACleanup();


        [DllImport("Btdrt.dll")]
        private static extern int BthNsLookupServiceBegin(
            byte[] pQuerySet,
            LookupFlags dwFlags,
            ref int lphLookup);

        [DllImport("Btdrt.dll")]
        private static extern int BthNsLookupServiceNext(
            int hLookup,
            LookupFlags dwFlags,
            ref int lpdwBufferLength,
            byte[] pResults);

        [DllImport("Btdrt.dll")]
        private static extern int BthNsLookupServiceEnd(
            int hLookup);

        #region Hardware Status
        /// <summary>
        /// Returns the current status of the Bluetooth radio hardware.
        /// </summary>
        /// <value>A member of the <see cref="OpenNETCF.Net.Bluetooth.HardwareStatus"/> enumeration.</value>
        public static HardwareStatus HardwareStatus {
            get {
                HardwareStatus status = 0;
                int result = BthGetHardwareStatus(ref status);

                if (result != 0) {
                    throw new ExternalException("Error retrieving Bluetooth hardware status");
                }
                return status;
            }
        }
        [DllImport("Btdrt.dll")]
        private static extern int BthGetHardwareStatus(ref HardwareStatus pistatus);

        #endregion

        #region LocalAddress
        /// <summary>
        /// Returns the address of the local Bluetooth device.
        /// </summary>
        public static BluetoothSocketAddress LocalAddress {
            get {
                BluetoothSocketAddress bsa = new BluetoothSocketAddress();

                int result = BthReadLocalAddr(bsa.ToByteArray());

                if (result != 0) {
                    throw new ExternalException("Error retrieving local Bluetooth address");
                }

                return bsa;
            }
        }
        [DllImport("Btdrt.dll")]
        private static extern int BthReadLocalAddr(byte[] pba);

        #endregion

        [DllImport("Btdrt.dll")]
        private static extern int BthReadLocalVersion(
            ref ushort phci_version,
            ref ushort phci_revision,
            ref ushort plmp_version,
            ref ushort plmp_subversion,
            ref ushort pmanufacturer,
            ref ushort plmp_features);

    }



    public enum LookupFlags : uint {
        Containers = 0x0002,
        ReturnName = 0x0010,
        ReturnAddr = 0x0100,
        ReturnBlob = 0x0200,
    }


}
