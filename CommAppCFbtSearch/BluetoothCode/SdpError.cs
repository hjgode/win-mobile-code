//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.Sdp.SdpError
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

namespace OpenNETCF.Net.Bluetooth {

    public enum SdpError : short {
        INVALID_SDP_VERSION = 0x0001,
        INVALID_RECORD_HANDLE = 0x0002,
        INVALID_REQUEST_SYNTAX = 0x0003,
        INVALID_PDU_SIZE = 0x0004,
        INVALID_CONTINUATION_STATE = 0x0005,
        INSUFFICIENT_RESOURCES = 0x0006,
    }

}
