//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.PortEmuParams
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
    /// Summary description for PortEmuParams.
    /// </summary>
    public class PortEmuParams {
        /*int channel;
        int flocal;
        //BD_ADDR device 48bits;
        uint LAP; 
        //uint UAP; 
        ushort  NAP; 

        int imtu;
        int iminmtu;
        int imaxmtu;
        int isendquota;
        int irecvquota;
        Guid uuidService;
        uint uiportflags;*/

        byte[] m_data;

        public PortEmuParams() {
            m_data = new byte[50];

            BitConverter.GetBytes((int)1).CopyTo(m_data, 46);
        }

        public byte[] ToByteArray() {
            return m_data;
        }

        public bool Local {
            get {
                return Convert.ToBoolean(BitConverter.ToInt32(m_data, 4));
            }
            set {
                int val = 0;
                if (value) {
                    val = -1;
                }
                BitConverter.GetBytes(val).CopyTo(m_data, 4);
            }
        }

        public byte[] Address {
            get {
                byte[] addrbytes = new byte[6];
                Buffer.BlockCopy(m_data, 8, addrbytes, 0, 6);
                return addrbytes;
            }
            set {
                Buffer.BlockCopy(value, 0, m_data, 8, 6);
            }
        }
    }
}
