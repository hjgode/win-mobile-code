// SetBTprinter.cpp : Defines the entry point for the DLL application.
//

#include "stdafx.h"
#include <windows.h>
//#include <commctrl.h>

// SetBTPrinter4dll.cpp : Defines the entry point for the DLL application.
//
/*
	history
		version
		1.1		replaced all DEBUGMSG by nclog
				added nclog.h and nclog.cpp
				changed log text for ITC_SetDefaultDevice()
				addede iResult=-3 before while loop
				added iResult=0 for found device below "if(wcsstr(uDeviceName, uBDA)!=NULL)"
*/
#pragma comment (user, "SetBTprinter.DLL version 1.1")

//#include "stdafx.h"
#include "SetBTprinter.h"

#include "nclog.h"

const TCHAR* szXmlFileName = L"\\Windows\\pswdm0c.xml";
const LPTSTR szDeviceType = TEXT("Printer");
const LPTSTR szDeviceSubtype = NULL;

const TCHAR* csFP = L"NEW\r\n1 BARFONT ON\r\n2 BARFONT \"Swiss 721 BT\", 6\r\n10 PRPOS 10,10\r\n20 PRBOX 430,340,15\r\n50 PRPOS 75,27060 BARTYPE \"CODE39\"70 PRBAR \"ABC\"80 PRPOS 25,220\r\n90 FONT \"Swiss 721 BT\", 6\r\n100 PRTXT \"My FIRST label\"\r\n200 PRINTFEED\r\n300 END\r\nRUN\r\n";

const TCHAR* csESCP = L"Intermec Developer Conference 2005\n\n Multiblue Sample Order\n\nProduct     Quantity     Price\n______________________________\n Delicious Soda     123     $0.49\n\n\n\n\n\n";

//test func for pswdmc0
// registerPrinter(TCHAR* szBDA)
// register and activate the BT device with BDA szBDA
// performs a device discovery and binds the device
// the registry has the BTPort entry as WPort not WPPort as in help doc
// returns 
//		0 for success
//		-1 for error in ITC_InitializeDeviceUtility
//		-2 for failure with ITC_DiscoverDevices
//		-3 for if unable to find device in range
//		-4 ITC_SetActiveDevice failed
//		-5 ITC_SetDefaultDevice failed
//		-6 ITC_SaveDeviceUtilityChanges failed
//		-7 ITC_CloseDeviceUtility failed
//		-8 ITC_AddDevice failed
int registerPrinter(TCHAR* szBDA){
	int iResult = 0; //return code, assume no error
	TCHAR uDeviceName[MAX_PATH];	//for upper case compare
	TCHAR uBDA[MAX_PATH];			//for upper case compare

	wchar_t fillChar = '\0';
	TCHAR deviceName[MAX_PATH];			//var for ITC_GetDiscoveredDevice
	TCHAR strSelectedDevice[MAX_PATH];		//var to be used for Registering

	nclog(L"registerPrinter: ITC_InitializeDeviceUtility()\r\n");
	int iRes = ITC_InitializeDeviceUtility(szXmlFileName);
	if(iRes==0){
		ITC_DISCOVERPROC lpDiscoveryComplete=NULL;
		DWORD lParam = 0;
		nclog(L"registerPrinter: ITC_DiscoverDevices()...\r\n");
		iRes = ITC_DiscoverDevices(szDeviceType, szDeviceSubtype, lpDiscoveryComplete, lParam);
		nclog(L"registerPrinter: ITC_DiscoverDevices() finished\r\n");
		if(iRes==0){
			iResult=-3; //assume device not found, v1.1
			BOOL bFirst = TRUE;
			while(ITC_GetDiscoveredDevice(deviceName, MAX_PATH, bFirst)==ERROR_SUCCESS){	//find first device
				//cDeviceName=deviceName;
				nclog(L"registerPrinter: testing '%s'\r\n", deviceName);
				//is the BDA in there?
				wcscpy(uBDA, szBDA); _wcsupr(uBDA); //all upper case
				wcscpy(uDeviceName, deviceName); _wcsupr(uDeviceName); //all upper case
				//is this the searched printer
				if(wcsstr(uDeviceName, uBDA)!=NULL){
					iResult=0; //found device, v1.1
					nclog(L"registerPrinter: found match '%s'\r\n", deviceName);				
					//found the printer in question, now set this as default
					wsprintf(strSelectedDevice, L"%s", deviceName);

					// Determine if the computer needs to bond with the device.
					nclog(L"registerPrinter: ITC_IsBondWithDeviceNeeded for %s...\r\n", strSelectedDevice);
					if (ITC_IsBondWithDeviceNeeded(strSelectedDevice))
					{
						// TODO: You should show a warning message suggesting
						// that bonding should not be performed in a public 
						// place, because an attacker could eavesdrop and 
						// steal the Passkey.

						// TODO: If you do not want to use the default Passkey 
						// prompt for the Bluetooth stack, make sure a Passkey
						// is set for the device. This sample just allows the
						// default Passkey prompt so it does not use this code.
                        TCHAR strPasskey[MAX_PATH];
                        iRes = ITC_GetDeviceProperty(strSelectedDevice, TEXT("Passkey"), strPasskey, MAX_PATH);
                        nclog(L"registerPrinter: ITC_GetDeviceProperty for passkey returned %i\r\n", iRes);
                        if (strPasskey==NULL || wcslen(strPasskey)==0)
                        {
                            // TODO: If all of your devices have a common
                            // Passkey, you could hardcode a value here.
                            // Otherwise, you could prompt the user for 
                            // the Passkey.
                            wsprintf(strPasskey, L"");
                            // Store the Passkey.
                            iRes = ITC_SetDeviceProperty(strSelectedDevice, TEXT("Passkey"), strPasskey);
                             nclog(L"registerPrinter: ITC_SetDeviceProperty setting empty passkey returned %i\r\n", iRes);
                        }

                        // Bond the computer and the device.
                        // TODO: Because radio connections can sometimes fail,
                        // you should build in retry logic when BondWithDevice
                        // fails. This sample just tries once.
                        iRes = ITC_BondWithDevice(strSelectedDevice);
                        if(iRes!=0)
                             nclog(L"registerPrinter: ITC_BondWithDevice failed with %i\r\n",iRes);
                        else
                             nclog(L"registerPrinter: ITC_BondWithDevice OK\r\n");

					}
					else{
						nclog(L"registerPrinter: ITC_IsBondWithDeviceNeeded not needed\r\n");
					}

					iRes = ITC_AddDevice(szDeviceType, /*szDeviceSubtype*/ NULL, strSelectedDevice);
					if (iRes == ERROR_SUCCESS){
						nclog(L"registerPrinter: ITC_AddDevice for '%s' OK\r\n", strSelectedDevice);
					}
					else{
						nclog(L"registerPrinter: ITC_AddDevice for '%s' failed with error %i\r\n", strSelectedDevice, iRes);
						iResult = -8;
					}

					// Set up a port to use the selected device. Refer to the BtPort element in the XML file.
					// Call ITC_SetActiveDevice to set up a port for accessing the device. Call ITC_SetActiveDevice before 
					// each attempt to access the device because the port is removed if the computer is rebooted.


					iRes = ITC_SetActiveDevice(szDeviceType, /*szDeviceSubtype*/ NULL, strSelectedDevice);
					if (iRes == ERROR_SUCCESS){
						nclog(L"registerPrinter: ITC_SetActiveDevice to '%s' OK\r\n", strSelectedDevice);
					}
					else{
						nclog(L"registerPrinter: ITC_SetActiveDevice to '%s' failed with error %i\r\n", strSelectedDevice, iRes);
						iResult = -4;
					}
					// Set the selected device as the default.
					iRes = ITC_SetDefaultDevice(szDeviceType, /*szDeviceSubtype*/ NULL, strSelectedDevice);
					if (iRes == ERROR_SUCCESS)
						nclog(L"registerPrinter: ITC_SetDefaultDevice to '%s' OK\r\n", strSelectedDevice);
					else{
						nclog(L"registerPrinter: ITC_SetDefaultDevice to '%s' failed with error %i\r\n", strSelectedDevice, iRes);
						iResult=-5;
					}
					nclog(L"...leaving registerPrinter\r\n");
					break;
				}// if BDA found
				bFirst=FALSE;
				_wcsset(deviceName, fillChar);
			}//while
			nclog(L"registerPrinter: Leaving loop\r\n");
		}//ITC_DiscoverDevices
		else{
			nclog(L"registerPrinter: ITC_DiscoverDevices() failed\r\n");
			iResult=-2;
		}

		//save changes to utility
		if(iRes = ITC_SaveDeviceUtilityChanges()==0)
			nclog(L"registerPrinter: saved changes to '%s'\r\n", szXmlFileName);
		else{
			nclog(L"registerPrinter: save to '%s' failed with error %i\r\n", szXmlFileName, iRes);
			iResult=-6;
		}

		//close device utility
		if(iRes = ITC_CloseDeviceUtility()==0)
			nclog(L"registerPrinter: ITC_CloseDeviceUtility() OK\r\n", szXmlFileName);
		else{
			nclog(L"registerPrinter: ITC_CloseDeviceUtility() failed with error %i\r\n", iRes);
			iResult=-7;
		}

		nclog(L"registerPrinter: ITC_InitializeDeviceUtility() finished\r\n");
	}
	else
	{
		nclog(L"registerPrinter: ITC_InitializeDeviceUtility() failed\r\n");
		return -1;
	}//ITC_InitializeDeviceUtility

	return iResult;
}

int doTestPrintESCP(TCHAR* szComPort) 
{
	int iRet = 0;
	nclog(_T("doTestPrint: printing...\r\n"));

	HANDLE	hPort;
	DWORD	dwNumBytesWritten;
//	TCHAR buf[1024+1];		// Printers expect regular chars but they'll throw away the nulls in the unicode

	hPort = CreateFile(szComPort, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);   
	
	if(hPort == INVALID_HANDLE_VALUE) 
	{
		iRet=GetLastError();
		nclog(_T("doTestPrint: Unable to connect to printer at '%s', err=%i\r\n"), szComPort, iRet);
		nclog(_T("doTestPrint: ...exit printing\r\n"));
		//AfxMessageBox(_T("Unable to connect to printer.");
		return iRet;
	}

	nclog(_T("doTestPrint: connected...\r\n"));

	//TEST with FingerPrint
	if (!WriteFile(hPort, csESCP, wcslen(csESCP) * sizeof(TCHAR), &dwNumBytesWritten, NULL)) {
		iRet = GetLastError();
		nclog(_T("doTestPrint: Print error: %i\r\n"), iRet);
		CloseHandle(hPort);
		return iRet;
	}
	//	AfxMessageBox(_T("Print error");
	
	nclog(_T("doTestPrint: WriteFile did %i bytes\r\n"), dwNumBytesWritten);

	Sleep(2000);			// Allow time to finish printing before closing the handle. Workaround for VM data flush issue
	CloseHandle(hPort);
	nclog(_T("doTestPrint: ...finished printing\r\n"));

	return 0;
}

int doTestPrintFP(TCHAR* szComPort) 
{
	nclog(L"doTestPrint: printing to '%s'...\r\n", szComPort);
	int iRet=0;

	HANDLE	hPort;
	DWORD	dwNumBytesWritten;
//	TCHAR buf[1024+1];		// Printers expect regular chars but they'll throw away the nulls in the unicode

	hPort = CreateFile(szComPort, GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, 0, NULL);   
	
	if(hPort == INVALID_HANDLE_VALUE) 
	{
		iRet=GetLastError();
		nclog(_T("doTestPrint: Unable to connect to printer at '%s', err=%i\r\n"), szComPort, iRet);
		nclog(_T("doTestPrint: ...exit printing\r\n"));
		return iRet;
	}

	nclog(_T("doTestPrint: connected...\r\n"));

	//TEST with FingerPrint
	if (!WriteFile(hPort, csFP, wcslen(csFP) * sizeof(TCHAR), &dwNumBytesWritten, NULL)) {
		iRet = GetLastError();
		nclog(_T("doTestPrint: Print error: %i\r\n"), iRet);
		CloseHandle(hPort);
		return iRet;
	//	AfxMessageBox(_T("Print error");
	}
	
	nclog(_T("doTestPrint: WriteFile did %i bytes\r\n"), dwNumBytesWritten);

	Sleep(2000);			// Allow time to finish printing before closing the handle. Workaround for VM data flush issue
	CloseHandle(hPort);
	nclog(_T("doTestPrint: ...finished printing\r\n"));

	return 0;
}

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call){
	case DLL_PROCESS_ATTACH:
		nclogEnable(FALSE);	//enable or disable logging
		nclog(L"SetBTPrinter DllMain: DLL_PROCESS_ATTACH\n");
		return TRUE;
	case DLL_THREAD_ATTACH:
		nclog(L"SetBTPrinter DllMain: DLL_THREAD_ATTACH\n");
		return TRUE;
	case DLL_THREAD_DETACH:
		nclog(L"SetBTPrinter DllMain: DLL_THREAD_DETACH\n");
		return TRUE;
	case DLL_PROCESS_DETACH:
		nclog(L"SetBTPrinter DllMain: DLL_PROCESS_DETACH\n");
		return TRUE;
	default:
		nclog(L"SetBTPrinter DllMain: default\n");
		return TRUE;

	}
}


