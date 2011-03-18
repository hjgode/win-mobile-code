//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.BluetoothSocketAddress
//		Copyright (C) 2004, OpenNETCF.org
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
using System.Net;

namespace OpenNETCF.Net.Bluetooth {
    /// <summary>
    /// Summary description for BluetoothSocketAddress.
    /// </summary>
    public class BluetoothSocketAddress {
        //USHORT addressFamily; //2
        //bt_addr btAddr; //6
        //GUID serviceClassId; //16
        //ULONG port; //4

        private byte[] m_data;
        public const int Length = 40;
        private const ushort AF_BTH = 32;

        public BluetoothSocketAddress() {
            m_data = new byte[Length];
            BitConverter.GetBytes(AF_BTH).CopyTo(m_data, 0);
        }

        public BluetoothSocketAddress(byte[] data) {
            if (data.Length == Length) {
                m_data = data;
            }
            else {
                throw new ArgumentOutOfRangeException("Data length not expected");
            }
        }

        public byte[] ToByteArray() {
            return m_data;
        }

        public override string ToString() {
            string result = "";

            result += m_data[13].ToString("X") + ":";
            result += m_data[12].ToString("X") + ":";
            result += m_data[11].ToString("X") + ":";
            result += m_data[10].ToString("X") + ":";
            result += m_data[9].ToString("X") + ":";
            result += m_data[8].ToString("X");

            return result;
        }


        public byte[] Address {
            get {
                byte[] addr = new byte[6];
                Buffer.BlockCopy(m_data, 8, addr, 0, 6);
                return addr;
                //return BitConverter.ToInt64(m_data, 0);
            }
        }
    }
}
