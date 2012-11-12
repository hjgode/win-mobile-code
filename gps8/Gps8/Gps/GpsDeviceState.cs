//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft end-user
// license agreement (EULA) under which you licensed this SOFTWARE PRODUCT.
// If you did not accept the terms of the EULA, you are not authorized to use
// this source code. For a copy of the EULA, please see the LICENSE.RTF on your
// install media.
//
#region Using directives

using System;
using System.Runtime.InteropServices;

#endregion

public enum GpsServiceState : int
{
    Off = 0,
    On = 1,
    StartingUp = 2, 
    ShuttingDown = 3,
    Unloading = 4,
    Uninitialized = 5,
    Unknown = -1
}

namespace Microsoft.WindowsMobile.Samples.Location
{

    [StructLayout(LayoutKind.Sequential)]
    internal struct FileTime
    {
        int dwLowDateTime;
        int dwHighDateTime;
    }

    /// <summary>
    /// GpsDeviceState holds the state of the gps device and the friendly name if the 
    /// gps supports them.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class GpsDeviceState
    {
        public static int GpsMaxFriendlyName = 64;
        public static int GpsDeviceStructureSize = 216;

        int serviceState = 0;
        /// <summary>
        /// State of the GPS Intermediate Driver service
        /// </summary>
        public GpsServiceState ServiceState
        {
            get {return (GpsServiceState)serviceState;}
        }

        int deviceState = 0;
        /// <summary>
        /// Status of the actual GPS device driver.
        /// </summary>
        public GpsServiceState DeviceState
        {
            get {return (GpsServiceState)deviceState;}
        }

        string friendlyName = "";
        /// <summary>
        /// Friendly name of the real GPS device we are currently using.
        /// </summary>
        public string FriendlyName
        {
            get {return friendlyName;}
        }
        /// <summary>
        /// String with GPSDriverPrefix
        /// </summary>
        string m_GPSDriverPrefix = "";
        public string GPSDriverPrefix
        {
            get { return m_GPSDriverPrefix; }
        }
        /// <summary>
        /// String with GPSDriverPrefix;
        /// </summary>
        string m_GPSMultiplexPrefix = "";
        public string GPSMultiplexPrefix
        {
            get { return m_GPSMultiplexPrefix ; }
        }
        /// <summary>
        /// Constructor of GpsDeviceState.  It copies values from the native pointer 
        /// passed in. 
        /// </summary>
        /// <param name="pGpsDevice">Native pointer to memory that contains
        /// the GPS_DEVICE data</param>
        public GpsDeviceState(IntPtr pGpsDevice)
        {
            // make sure our pointer is valid
            if (pGpsDevice == IntPtr.Zero)
            {
                throw new ArgumentException();
            }

            // read in the service state which starts at offset 8
            serviceState = Marshal.ReadInt32(pGpsDevice, 8);
            // read in the device state which starts at offset 12
            deviceState = Marshal.ReadInt32(pGpsDevice, 12);

#region GPS_DEVICE struct
            //
            // GPS_DEVICE contains information about the device driver and the
            // service itself and is returned on a call to GPSGetDeviceState().
            // States are indicated with SERVICE_STATE_XXX flags defined in service.h
            // 
            //#define GPS_MAX_SATELLITES      12
            //#define GPS_MAX_PREFIX_NAME     16
            //#define GPS_MAX_FRIENDLY_NAME   64
            //typedef struct _GPS_DEVICE {
            //    DWORD    dwVersion;                             0-3    // Current version of GPSID client is using.
            //    DWORD    dwSize;                                4-7    // sizeof this structure
            //    DWORD    dwServiceState;                        8-11    // State of the GPS Intermediate Driver service.  
            //    DWORD    dwDeviceState;                        12-15     // Status of the actual GPS device driver.
            //    FILETIME ftLastDataReceived;                   16-19,20-23     // Last time that the actual GPS device sent information to the intermediate driver.
            //    WCHAR    szGPSDriverPrefix[GPS_MAX_PREFIX_NAME];    24-55 (16x2byte)// Prefix name we are using to communicate to the base GPS driver
            //    WCHAR    szGPSMultiplexPrefix[GPS_MAX_PREFIX_NAME]; 56-87 // Prefix name that GPS Intermediate Driver Multiplexer is running on
            //    WCHAR    szGPSFriendlyName[GPS_MAX_FRIENDLY_NAME];  88-215 (62x2)    // Friendly name real GPS device we are currently using
            //} *PGPS_DEVICE, GPS_DEVICE;
#endregion

            // the friendly name starts at offset 88
            IntPtr pFriendlyName = (IntPtr)(pGpsDevice.ToInt32() + 88);
            // marshal the native string into our gpsFriendlyName
            friendlyName = Marshal.PtrToStringUni(pFriendlyName);

            //
            IntPtr pGPSDriverPrefix = (IntPtr)(pGpsDevice.ToInt32() + 24);
            m_GPSDriverPrefix = Marshal.PtrToStringUni(pGPSDriverPrefix);

            IntPtr pGPSMultiplexPrefix = (IntPtr)(pGpsDevice.ToInt32() + 56);
            m_GPSMultiplexPrefix = Marshal.PtrToStringUni(pGPSMultiplexPrefix); ;
        }
    }
}
