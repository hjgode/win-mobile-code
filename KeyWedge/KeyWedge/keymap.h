//keymap.h

#include <winuser.h>
#pragma once

#ifndef _KEYMAP_
#define _KEYMAP_
	#define MAX_BUFSIZE 1000

	#define KTAB_SIZE 0xFF

	//some undefined VK values
	#define VK_UNDEF 0x00
	#define VK_NUL VK_UNDEF
	#define VK_SOH VK_UNDEF
	#define VK_STX VK_UNDEF
	#define VK_ETX VK_UNDEF
	#define VK_EOT VK_UNDEF
	#define VK_ENQ VK_UNDEF
	#define VK_ACK VK_UNDEF
	#define VK_BEL VK_UNDEF
	#define VK_VT VK_UNDEF
	#define VK_FF VK_UNDEF 
	#define VK_SO VK_UNDEF
	#define VK_SI VK_UNDEF
	#define VK_DLE VK_UNDEF
	#define VK_DC1 VK_UNDEF
	#define VK_DC2 VK_UNDEF
	#define VK_DC3 VK_UNDEF
	#define VK_DC4 VK_UNDEF
	#define VK_NAK VK_UNDEF
	#define VK_SYN VK_UNDEF
	#define VK_ETB VK_UNDEF
	#define VK_CAN VK_UNDEF
	#define VK_EM VK_UNDEF
	#define VK_SUB VK_UNDEF
	#define VK_FS VK_UNDEF
	#define VK_GS VK_UNDEF
	#define VK_RS VK_UNDEF
	#define VK_US VK_UNDEF

	// some more undefined vk values
	#define VK_0 	0x30
	#define VK_1 	0x31
	#define VK_2 	0x32
	#define VK_3 	0x33
	#define VK_4 	0x34
	#define VK_5 	0x35
	#define VK_6 	0x36
	#define VK_7 	0x37
	#define VK_8 	0x38
	#define VK_9 	0x39
	#define VK_A 	0x41
	#define VK_B 	0x42
	#define VK_C 	0x43
	#define VK_D 	0x44
	#define VK_E 	0x45
	#define VK_F 	0x46
	#define VK_G 	0x47
	#define VK_H 	0x48
	#define VK_I 	0x49
	#define VK_J 	0x4A
	#define VK_K 	0x4B
	#define VK_L 	0x4C
	#define VK_M 	0x4D
	#define VK_N 	0x4E
	#define VK_O 	0x4F
	#define VK_P 	0x50
	#define VK_Q 	0x51
	#define VK_R 	0x52
	#define VK_S 	0x53
	#define VK_T 	0x54
	#define VK_U 	0x55
	#define VK_V 	0x56
	#define VK_W 	0x57
	#define VK_X 	0x58
	#define VK_Y 	0x59
	#define VK_Z 	0x5A

	// the struct used to save a ASCII <-> VK_ table
	struct KTABLE{
		 byte kByte[1];
		 char txt[15];
		 byte kVKval; //the VK value to send for kChar/kByte
		 bool kShift;
	};

	typedef KTABLE* pKTABLE;
	
	extern struct KTABLE vkTable[];

#endif //_KEYMAP_

