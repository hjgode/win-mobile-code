#ifndef __OEMIOCTL_H__
#define __OEMIOCTL_H__

/* Copyright © 2000-2001 Intel Corp.  */
/*++

Module Name:  $Workfile: oemioctl.h $

Abstract:  
 Contains definitions specific to Intel XScale 
 Microarchitecture OEMIOcontrol calls.
 
--*/

#include <winioctl.h>

#define OAL_VERSION "1.00"
#define CORE_OEM_IOCTL_BASE						0x800
#define PLATFORM_ITC_IOCTL_BASE					0x8B0
#define PLATFORM_OEM_IOCTL_BASE					0xC00
#define PLATFORM_PMU_IOCTL_BASE					0xFA0

//----------------------------------------------------------------------------
//
// These IOCTL codes allow the platform to be reset
//
//----------------------------------------------------------------------------

#define IOCTL_HAL_WARMBOOT		 			CTL_CODE( FILE_DEVICE_HAL, CORE_OEM_IOCTL_BASE + 4,  METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HAL_COLDBOOT		 			CTL_CODE( FILE_DEVICE_HAL, CORE_OEM_IOCTL_BASE + 5,  METHOD_BUFFERED, FILE_ANY_ACCESS)

//other controls

#define IOCTL_HAL_GET_RESET_INFO			CTL_CODE( FILE_DEVICE_HAL, CORE_OEM_IOCTL_BASE + 23, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HAL_GET_BOOT_DEVICE			CTL_CODE( FILE_DEVICE_HAL, CORE_OEM_IOCTL_BASE + 22, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HAL_GET_BOOTLOADER_VERINFO	CTL_CODE( FILE_DEVICE_HAL, CORE_OEM_IOCTL_BASE + 1,  METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HAL_GET_OAL_VERINFO			CTL_CODE( FILE_DEVICE_HAL, CORE_OEM_IOCTL_BASE + 0,  METHOD_BUFFERED, FILE_ANY_ACCESS)

#define HAL_BOOT_DEVICE_UNKNOWN			0
#define HAL_BOOT_DEVICE_ROM_XIP			1
#define HAL_BOOT_DEVICE_ROM				2
#define HAL_BOOT_DEVICE_PCMCIA_ATA		3
#define HAL_BOOT_DEVICE_PCMCIA_LINEAR	4
#define HAL_BOOT_DEVICE_IDE_ATA			5
#define HAL_BOOT_DEVICE_IDE_ATAPI		6
#define HAL_BOOT_DEVICE_ETHERNET		7
#define HAL_BOOT_DEVICE_80211b			8
#define HAL_BOOT_DEVICE_SDMMC			9

#define HAL_RESET_TYPE_UNKNOWN			0
#define HAL_RESET_REASON_HARDWARE		1
#define HAL_RESET_REASON_SOFTWARE		2
#define HAL_RESET_REASON_WATCHDOG		4
#define HAL_RESET_BATT_FAULT			8
#define HAL_RESET_VDD_FAULT				16

#define HAL_OBJECT_STORE_STATE_UNKNOWN	0
#define HAL_OBJECT_STORE_STATE_CLEAR	1

typedef struct VersionInfo
{
	int				cboemverinfo;			// sizeof (tagOemVerInfo);
	unsigned short	verinfover;			// version number of version info structure
	char			sig[8];					// 
	char			id;						// 'B' = boot loader, 'N' = CE image
	char			tgtcustomer[24];		// _TGTCUSTOMER -    customer name
	char			tgtplat[24];			// _TGTPLAT - 	     platform name
	char			tgtplatversion[8];		// _TGTPLATVERSION - platform version
	char			tgtcputype[8];			// _TGTCPUTYPE -     CPU type
	char			tgtcpu[12];				// _TGTCPU -         CPU name
	char			tgtcoreversion[8];		// 
	char			date[12];				// __DATE__ -        build date
	char			time[12];				// __TIME__ -        build time

} VERSIONINFO, *PVERSIONINFO;


typedef struct {
	DWORD ResetReason;		// type of the most recent reset
	DWORD ObjectStoreState;	// state of the object store
} HAL_RESET_INFO, * PHAL_RESET_INFO;

//
// Struct for CPU ID info
//
#define IOCTL_GET_CPU_ID CTL_CODE(FILE_DEVICE_HAL, PLATFORM_PMU_IOCTL_BASE+1, METHOD_BUFFERED, FILE_ANY_ACCESS)

typedef struct {
    unsigned long CPUId;
} CPUIdInfo, *PCPUIdInfo;


#define IOCTL_HAL_NMSD_READ_PARM 	CTL_CODE(FILE_DEVICE_HAL, PLATFORM_ITC_IOCTL_BASE + 1, METHOD_BUFFERED, FILE_ANY_ACCESS)

// Norand standard parameter ID's
#define APM_PARM_ETHERNET_ID		1	// Ethernet ID 6 bytes + 1 byte CRC.
#define APM_PARM_ETHERNET_LTH		6	// Ethernet ID length
#define APM_PARM_MANF_DATE			3	// Manf date. BCD: yymmdd.
#define APM_PARM_PEN_ALIGN_ID 		19	// Pen alignment id: 8 bytes at the moment.
#define APM_PARM_PEN_ALIGN_LTH		8	// Pen alignment length

#define IOCTL_HAL_ITC_READ_PARM_BYTE 	CTL_CODE(FILE_DEVICE_HAL, PLATFORM_ITC_IOCTL_BASE + 28, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HAL_ITC_READ_PARM 	CTL_CODE(FILE_DEVICE_HAL, PLATFORM_ITC_IOCTL_BASE + 1, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define ITC_NVPARM_ETHERNET_ID		1	// Ethernet ID 6 bytes + 1 byte CRC.
#define ITC_NVPARM_SERIAL_NUM		2
#define ITC_NVPARM_MANF_DATE		3	// Manf date. BCD: yymmdd.
#define ITC_NVPARM_SERVICE_DATE		4
#define ITC_NVPARM_DISPLAY_TYPE		5
#define ITC_NVPARM_EDG_IP			6
#define ITC_NVPARM_EDBG_SUBNET		7
#define ITC_NVPARM_ECN				8
#define ITC_NVPARM_CONTRAST			9
#define ITC_NVPARM_MCODE			10
#define ITC_IRQS					11
#define ITC_NVPARM_VERSION_NUMBER	14

	#define VN_CLASS_KBD			1
	#define VN_CLASS_ASIC			2
	#define VN_CLASS_BOOTSTRAP		3

#define ITC_NVPARM_SOFTWARE_FLAGS	15
#define ITC_NVPARM_INTERMEC_SOFTWARE_CONTENT 16
#define ITC_RTC_RESTORE_ENABLE		17
#define ITC_NVPARM_RTC_RESTORE		17
#define ITC_NVPARM_WAKEUP_MASK		18
#define ITC_NVPARM_PHYSICAL_MEMORY	19
#define ITC_NVPARM_ANTENNA_DIVERSITY	20
#define ITC_NVPARM_WAN_RI				21
#define ITC_NVPARM_PERIPH_FLAGS		22
#define ITC_NVPARM_SOFT_ENABLE_FLAGS	23
#define ITC_NVPARM_BOOT_DEVICE		24
#define ITC_NVPARM_MIXER_DATA       25

#define ITC_NVPARM_INTERMEC_DATACOLLECTION_HW	26
#define ITC_NVPARM_INTERMEC_DATACOLLECTION_SW	27
#define ITC_NVPARM_WAN_INSTALLED	28
#define ITC_NVPARM_WAN_FREQUENCY	29
#define ITC_NVPARM_WAN_RADIOTYPE	30
#define ITC_NVPARM_80211_INSTALLED	31
#define ITC_NVPARM_80211_RADIOTYPE	32
#define ITC_NVPARM_BLUETOOTH_INSTALLED	33
#define ITC_NVPARM_SERIAL2_INSTALLED	34
#define ITC_NVPARM_VIBRATE_INSTALLED	35
#define ITC_NVPARM_LAN9000_INSTALLED	36
#define ITC_NVPARM_SIM_PROTECT_HW_INSTALLED	37
#define ITC_NVPARM_SIM_PROTECT_SW_INSTALLED	38
#define ITC_NVPARM_MANU_FLAGS 39
#define ITC_NVPARM_EXTENDED_FLAGS 40

#define ITC_DEVID_MASK					0x7F	// scanner device id's are stored in the lower 7 dev id bits
#define ITC_DEVID_S6ENGINE_MASK		0x80	// the s6 engine flag is stored in the high dev id bit
#define ITC_DEVID_SCANHW_NONE			0x00
#define ITC_DEVID_OEM2D_IMAGER		0x01
#define ITC_DEVID_INTERMEC2D_IMAGER	0x02
#define ITC_DEVID_SE900_LASER			0x03
#define ITC_DEVID_SE900HS_LASER		0x04
#define ITC_DEVID_INTERMEC_E1210		0x05
#define ITC_DEVID_SCANHW_HIGHEST    0x05  // Set to highest defined value
#define ITC_DEVID_SCANHW_MAX        0x7F	// This is the max value for scanner id's

#define ITC_DEVID_WANRADIO_NONE				0x00
#define ITC_DEVID_WANRADIO_SIERRA_SB555	0x01 // CDMA
#define ITC_DEVID_WANRADIO_XIRCOM_GEM3503	0x02 // GSM/GPRS
#define ITC_DEVID_WANRADIO_SIEMENS_MC45	0x03 // GSM/GPRS
#define ITC_DEVID_WANRADIO_UHF418_COM4 0x04 // periph board uart
#define ITC_DEVID_WANRADIO_UHF418_COM2 0x05 // cotulla uart
#define ITC_DEVID_WANRADIO_MAX				0x0F

#define ITC_DEVID_80211RADIO_NONE			0x00
#define ITC_DEVID_80211RADIO_INTEL_2011B	0x01
#define ITC_DEVID_80211RADIO_MAX				0x0F

#define IOCTL_HAL_ITC_READ_SYSPARM	CTL_CODE(FILE_DEVICE_HAL, PLATFORM_ITC_IOCTL_BASE + 4, METHOD_BUFFERED, FILE_ANY_ACCESS)
#define IOCTL_HAL_ITC_WRITE_SYSPARM	CTL_CODE(FILE_DEVICE_HAL, PLATFORM_ITC_IOCTL_BASE + 5, METHOD_BUFFERED, FILE_ANY_ACCESS)

#define ITC_REGISTRY_LOCATION		1
#define	ITC_REGISTRY_SAVE_ENABLE	2
#define ITC_DOCK_SWITCH				3
	#define DOCK_MODEM					0x40	// Sets the switch to "0" (modem)
	#define DOCK_SERIAL					0		// Sets the switch to "1" (serial)

#define ITC_WAKEUP_MASK					4
	#define 	SCANNER_TRIGGER		1
	#define 	SCANNER_LEFT		2
	#define 	SCANNER_RIGHT		4
	#define 	GOLD_A1				8
	#define 	GOLD_A2				0x10
#define ITC_AMBIENT_KEYBOARD			5
#define ITC_AMBIENT_FRONTLIGHT			6
#define ITC_COMMIT_AMBIENT_KEYBOARD		7
#define ITC_COMMIT_AMBIENT_FRONTLIGHT	8
#define ITC_AMBIENT_KEYBOARD_EEPROM		9
#define ITC_AMBIENT_FRONTLIGHT_EEPROM	10
#define ITC_A2D							11
#define ITC_GET_SYSPARM_AUDIO           12
#define ITC_SET_SYSPARM_AUDIO			13
#define ITC_GET_SYS_PASSWORD			14
#define ITC_SET_SYS_PASSWORD			15
#define ITC_ENABLE_SYS_PASSWORD			16
#define ITC_IS_SYS_PASSWORD_ENABLED		17

	#define MAIN_BATT_A2D			1
	#define EXT_POWER_A2D			2
	#define CHG_CURR_A2D			3
	#define HEADSET_A2D				4
	#define AMBIENT_A2D				5

#define	IOCTL_HAL_KEYBOARD_WEDGE	CTL_CODE(FILE_DEVICE_HAL, PLATFORM_ITC_IOCTL_BASE + 29, METHOD_BUFFERED, FILE_ANY_ACCESS)
#endif