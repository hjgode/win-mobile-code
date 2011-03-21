//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.RadioMode
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
    /// Determine all the possible modes of operation of the Bluetooth radio.
    /// </summary>
    public enum RadioMode {
        /// <summary>
        /// Bluetooth is disabled on the device.
        /// </summary>
        PowerOff,
        /// <summary>
        /// Bluetooth is connectable but your device cannot be discovered by other devices.
        /// </summary>
        Connectable,
        /// <summary>
        /// Bluetooth is activated and fully discoverable.
        /// </summary>
        Discoverable,
    }
}
