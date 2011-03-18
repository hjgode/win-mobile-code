//==========================================================================================
//
//		OpenNETCF.Net.Bluetooth.HardwareStatus
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
    /// Specifies the current status of the Bluetooth hardware.
    /// </summary>
    public enum HardwareStatus : int {
        /// <summary>
        /// Status cannot be determined.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Bluetooth radio not present.
        /// </summary>
        NotPresent = 1,
        /// <summary>
        /// Bluetooth radio is in the process of starting up.
        /// </summary>
        Initializing = 2,
        /// <summary>
        /// Bluetooth radio is active.
        /// </summary>
        Running = 3,
        /// <summary>
        /// Bluetooth radio is in the process of shutting down.
        /// </summary>
        Shutdown = 4,
        /// <summary>
        /// Bluetooth radio is in an error state.
        /// </summary>
        Error = 5,
    }
}
