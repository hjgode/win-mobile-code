//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.BluetoothDeviceInfo
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

namespace OpenNETCF.Net.Bluetooth {
    /// <summary>
    /// Summary description for BluetoothDeviceInfo.
    /// </summary>
    public class BluetoothDeviceInfo {
        private byte[] m_id;
        private string m_name;

        internal BluetoothDeviceInfo(byte[] id, string name) {
            m_id = id;
            m_name = name;
        }

        /// <summary>
        /// Gets the device identifier.
        /// </summary>
        public byte[] DeviceID {
            get {
                return m_id;
            }
        }

        /// <summary>
        /// Gets a name of a device.
        /// </summary>
        public string DeviceName {
            get {
                return m_name;
            }
        }
    }
}
