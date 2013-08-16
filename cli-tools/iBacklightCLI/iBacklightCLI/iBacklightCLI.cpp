// iBacklightCLI.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

DWORD maxBrightness=100;

int setBacklightState(BOOL bOnOff);

DWORD getMaxBrightness(){
	DWORD dwB;
	HRESULT hRes = ITCGetMaxScreenBrightness(&dwB);
	if( ITC_ISERROR (hRes) )
		return -1;	//failed
	maxBrightness=dwB;
	return maxBrightness;
}

int getBacklight(){
	DWORD dwBrightness;
	HRESULT hRes = ITCGetScreenBrightness(&dwBrightness);
	if( ITC_ISERROR (hRes) )
		return -1;	//failed

	hRes = ITCGetScreenBrightnessAcDc(0xDC, &dwBrightness);
	if( ITC_ISERROR (hRes) )
		DEBUGMSG(1,(L"DC brightness get not supported\n"));
	else
		DEBUGMSG(1,(L"DC brightness = %i\n", dwBrightness));

	hRes = ITCGetScreenBrightnessAcDc(0xAC, &dwBrightness);
	if( ITC_ISERROR (hRes) )
		DEBUGMSG(1,(L"AC brightness get not supported\n"));
	else
		DEBUGMSG(1,(L"AC brightness = %i\n", dwBrightness));

	return dwBrightness;
}

int setBacklight(DWORD dwBrightness){
	HRESULT hRes = ITCSetScreenBrightness(dwBrightness);
	if( ITC_ISERROR (hRes) )
		return -1;	//failed

	hRes = ITCSetScreenBrightnessAcDc(0xAC, dwBrightness);
	if( ITC_ISERROR (hRes) )
		DEBUGMSG(1,(L"AC brightness setting not supported\n"));
	hRes = ITCSetScreenBrightnessAcDc(0xDC, dwBrightness);
	if( ITC_ISERROR (hRes) )
		DEBUGMSG(1,(L"DC brightness setting not supported\n"));

	setBacklightState(TRUE);
	return 0;
}

int setBacklightState(BOOL bOnOff){
	HRESULT hRes = ITCSetScreenLight(bOnOff, TRUE);
	if( ITC_ISERROR (hRes) )
		return -1;	//failed
	return 0;
}

int getBacklightState(){
	BOOL bOnOff=FALSE;
	HRESULT hRes = ITCGetScreenLight(&bOnOff);
	if( ITC_ISERROR (hRes) )
		return -1;	//failed
	if(bOnOff)
		return 1;
	else
		return 0;
}

int _tmain(int argc, _TCHAR* argv[])
{
	getMaxBrightness();
	DEBUGMSG(1,(L"Max brightness = %i\n", maxBrightness));
	int iRet = 0;

	if(argc==2){
		//changes
		if(wcsicmp(argv[1], L"on")==0)
			setBacklightState(TRUE);
		else if(wcsicmp(argv[1], L"off")==0)
			setBacklightState(FALSE);
		else if(wcsicmp(argv[1], L"max")==0)
			setBacklight(maxBrightness);
		else if(wcsicmp(argv[1], L"min")==0)
			setBacklight((DWORD)1);
		//queries
		else if(wcsicmp(argv[1], L"state")==0)
			return (getBacklightState());
		else if(wcsicmp(argv[1], L"level")==0)
			return getBacklight();
		else{
			//change
			DWORD dwLevel = _wtoi(argv[1]);
			if(dwLevel>0 && dwLevel<=maxBrightness){
				setBacklight(dwLevel);
				return getBacklight();
			}
			else if(dwLevel>maxBrightness)
				return -2;
		}
	}

	return iRet;
}

