//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.SdpQueryUuidUnion
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
    /// Summary description for SdpQueryUuidUnion.
    /// </summary>
    public class SdpQueryUuidUnion {
        private byte[] m_data;

        public SdpQueryUuidUnion() {
            m_data = new byte[18];
        }

        public Guid Uuid {
            get {
                byte[] guidbytes = new byte[16];

                Buffer.BlockCopy(m_data, 0, guidbytes, 0, 16);

                return new Guid(guidbytes);
            }
        }

        /// <summary>
        /// Uuid Type.
        /// </summary>
        public short UuidType {
            get {
                return BitConverter.ToInt16(m_data, 16);
            }
            /*set
            {
                BitConverter.GetBytes(value).CopyTo(m_data, 16);
            }*/
        }
    }
}
