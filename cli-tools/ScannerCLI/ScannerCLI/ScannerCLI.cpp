// ScannerCLI.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

INT32 hScan=NULL;
DWORD iTimeOut=1000;

int doScanBarcode(){
	int iRet=0;

	HRESULT hRes = ITCSCAN_Open(&hScan, L"default");
	if(ITC_ISERROR(hRes))
		return -1;
	hRes = ITCSCAN_SetScannerEnable(hScan, TRUE);
	if(ITC_ISERROR(hRes))
		return -2;

	READ_DATA_STRUCT ReadDataBlock;
	BYTE* pDataBytes = (BYTE*)malloc(2048);
	ReadDataBlock.dwTimeout=iTimeOut;
	ReadDataBlock.rgbDataBuffer = pDataBytes; 
	ReadDataBlock.dwDataBufferSize = 2048;
	ReadDataBlock.dwBytesReturned = 0;
	ReadDataBlock.iDataType=0;
	ReadDataBlock.iSymbology=0;

	hRes = ITCSCAN_SetTriggerScanner(hScan, TRUE);
	if(ITC_ISERROR(hRes))
	{
		iRet=-3;
		goto scanexit;
	}

	hRes = ITCSCAN_SyncRead(hScan, &ReadDataBlock);

	if(hRes==E_ITCADC_OPERATION_TIMED_OUT)
	{
		DEBUGMSG(1, (L"Barcode read timed out with no barcode read\n"));
		iRet=0;
		goto scanexit;
	}

	if(ITC_ISERROR(hRes)){
		iRet = -4;
		goto scanexit;
	}

	TCHAR* tStr = (TCHAR*)malloc(4096);
	mbstowcs(tStr, (char*)ReadDataBlock.rgbDataBuffer, ReadDataBlock.dwBytesReturned);

	DEBUGMSG(1, (L"Barcode read= '%s'\n", tStr));
	delete(tStr);
	delete(ReadDataBlock.rgbDataBuffer);

	iRet=1;

scanexit:
	//unset trigger
	hRes = ITCSCAN_SetTriggerScanner(hScan, FALSE);
	if(ITC_ISERROR(hRes))
		return -5;
	hRes = ITCSCAN_Close(hScan);
	if(ITC_ISERROR(hRes))
		return -6;

	return iRet;
}

int _tmain(int argc, _TCHAR* argv[])
{
	if(argc==2){
		int i = _wtoi(argv[1]);
		if(i>0 && i<10)
			iTimeOut=i * 1000;
	}

	int iResult = doScanBarcode();
	DEBUGMSG(1, (L"doScanBarcode() = %i\n", iResult));
	return iResult;

	return 0;
}

