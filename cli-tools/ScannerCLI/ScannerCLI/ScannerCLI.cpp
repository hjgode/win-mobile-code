// ScannerCLI.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

INT32 hScan=NULL;

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
	ReadDataBlock.dwTimeout=1000;
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
	return doScanBarcode();

	return 0;
}

