//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.ClassOfDevice
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
    [Flags()]
    public enum ClassOfDevice : int {
        MAJOR_SERVICE_CLASS_INFORMATION = 0x800000,
        MAJOR_SERVICE_CLASS_TELEPHONY = 0x400000,
        MAJOR_SERVICE_CLASS_AUDIO = 0x200000,
        MAJOR_SERVICE_CLASS_OBEX = 0x100000,
        MAJOR_SERVICE_CLASS_CAPTURE = 0x080000,
        MAJOR_SERVICE_CLASS_RENDERING = 0x040000,
        MAJOR_SERVICE_CLASS_NETWORK = 0x020000,
        MAJOR_SERVICE_CLASS_LIMITED_DISC = 0x002000,

        BTH_COD_MAJOR_DEVICE_CLASS_MISC = 0x000000,
        BTH_COD_MAJOR_DEVICE_CLASS_COMPUTER = 0x000100,
        BTH_COD_MAJOR_DEVICE_CLASS_PHONE = 0x000200,
        BTH_COD_MAJOR_DEVICE_CLASS_LAP = 0x000300,
        BTH_COD_MAJOR_DEVICE_CLASS_AUDIO = 0x000400,
        BTH_COD_MAJOR_DEVICE_CLASS_PERIPHERAL = 0x000500,
        BTH_COD_MAJOR_DEVICE_CLASS_UNCLASSIFIED = 0x001f00,

        BTH_COD_MINOR_COMPUTER_UNCLASSIFIED = 0x000000,
        BTH_COD_MINOR_COMPUTER_DESKTOP = 0x000004,
        BTH_COD_MINOR_COMPUTER_SERVER = 0x000008,
        BTH_COD_MINOR_COMPUTER_LAPTOP = 0x00000c,
        BTH_COD_MINOR_COMPUTER_HANDHELD = 0x000010,
        BTH_COD_MINOR_COMPUTER_PDA = 0x000014,

        BTH_COD_MINOR_PHONE_UNCLASSIFIED = 0x000000,
        BTH_COD_MINOR_PHONE_CELL = 0x000004,
        BTH_COD_MINOR_PHONE_CORDLESS = 0x000008,
        BTH_COD_MINOR_PHONE_SMART = 0x00000c,
        BTH_COD_MINOR_PHONE_WIRED = 0x000010,

        BTH_COD_MINOR_LAP_AVAILABLE = 0x000000,
        BTH_COD_MINOR_LAP_1_17 = 0x000004,
        BTH_COD_MINOR_LAP_17_33 = 0x000008,
        BTH_COD_MINOR_LAP_33_50 = 0x00000c,
        BTH_COD_MINOR_LAP_50_67 = 0x000010,
        BTH_COD_MINOR_LAP_67_83 = 0x000014,
        BTH_COD_MINOR_LAP_83_99 = 0x000018,
        BTH_COD_MINOR_LAP_NO_SERVICE = 0x00001c,

        BTH_COD_MINOR_AUDIO_UNCLASSIFIED = 0x000000,
        BTH_COD_MINOR_AUDIO_HEADSET = 0x000004,
    }
}

