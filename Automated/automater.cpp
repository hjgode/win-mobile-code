//autometer.cpp
#include "stdafx.h"
#include "automater.h"

automater::automater(){
	mszTitle=NULL;
	mszClass=NULL;
	getMetrics(&mScreenW, &mScreenH);
}

automater::automater(HWND hWnd){
	mhWnd=hWnd;
	getMetrics(&mScreenW, &mScreenH);
}

automater::automater(TCHAR *szClass, TCHAR *szTitle){
	mszClass=(TCHAR*)malloc(wcslen(szClass)*sizeof(TCHAR)+2);
	mszTitle=(TCHAR*)malloc(wcslen(szClass)*sizeof(TCHAR)+2);
	mhWnd=FindWindow(mszClass,mszTitle);
	getMetrics(&mScreenW, &mScreenH);
}

automater::~automater(){
	free (mszClass);
	free (mszTitle);
}

BOOL automater::testWindow(){
	if(FindWindow(mszClass,mszTitle)==NULL)
		return FALSE;
	else
		return TRUE;
}

void automater::visualDelay(int delay){
	DEBUGMSG(1, (L"\nDELAY: "));
	for (int i=1; i<=delay; i++){
		DEBUGMSG(1, (L"."));
		Sleep(i*1000);
	}
}

void automater::getMetrics(int* width, int* height){
	int screenX = GetSystemMetrics(SM_CXFULLSCREEN);
	int screenY = GetSystemMetrics(SM_CYFULLSCREEN);
	*width=screenX;
	*height=screenY;
}

BOOL automater::DoClickAt(clickPoint* cp){

	if(!testWindow()){
		return FALSE;
	}

	int dx = (int)((65535 / mScreenW) * cp->x); //Screen.PrimaryScreen.Bounds.Width
	int dy = (int)((65535 / mScreenH) * cp->y); //Screen.PrimaryScreen.Bounds.Height

	DEBUGMSG(1, (L"Action: %s at %i/%i\n", cp->name, cp->x, cp->y));

	mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	Sleep(5);
	mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	return TRUE;
}

BOOL automater::DoEnterText(TCHAR* text){
	if(!testWindow()){
		return FALSE;
	}
	char* textA;
	textA = (char*)malloc((wcslen(text)+1)*sizeof(TCHAR));
	int num = wcstombs(textA, text, wcslen(text)+1);
	byte* pByte=(byte*) textA; //set pointer to first char
	int cnt=0;
	//while (pByte!=NULL && pByte!=0){
	while(cnt<num){
		keybd_event(*pByte, 0x00, 0x00, 0);
		Sleep(1);
		keybd_event(*pByte, 0x00, KEYEVENTF_KEYUP, 0x00);
		pByte++;
		cnt++;
	}
	free (textA);
	return TRUE;
}

BOOL automater::DoSendTextMsg(TCHAR* text){
	if(!testWindow()){
		return FALSE;
	}
	HWND hWnd=FindWindow(mszClass, mszTitle);
	if(hWnd==NULL)
		return FALSE;

	char* textA;
	textA = (char*)malloc((wcslen(text)+1)*sizeof(TCHAR));
	int num = wcstombs(textA, text, wcslen(text)+1);
	byte* pByte=(byte*) textA; //set pointer to first char
	int cnt=0;
	//while (pByte!=NULL && pByte!=0){
	while(cnt<num){
//		SendMessage(hWnd, WM_CHAR, *pByte, 0x40000000);	//with bit 30 set, key pressed
		Sleep(1);
		SendMessage(hWnd, WM_CHAR, *pByte, 0x80000000);	//with bit 31 set, key released
		Sleep(1);
		pByte++;
		cnt++;
	}
	free (textA);
	return TRUE;
}

BOOL automater::DoSendTab(){
	if(!testWindow()){
		return FALSE;
	}
	keybd_event(0x09, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(0x09, 0x00, KEYEVENTF_KEYUP, 0x00);
	return TRUE;
}

BOOL automater::DoSendSpace(){
	if(!testWindow()){
		return FALSE;
	}
	keybd_event(0x20, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(0x20, 0x00, KEYEVENTF_KEYUP, 0x00);
	return TRUE;
}

BOOL automater::DoSendEnter(){
	if(!testWindow()){
		return FALSE;
	}
	keybd_event(VK_RETURN, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(VK_RETURN, 0x00, KEYEVENTF_KEYUP, 0x00);
	return TRUE;
}

BOOL automater::DoSendKeyEvent(BYTE vkKey){
	if(!testWindow()){
		return FALSE;
	}
	keybd_event(vkKey, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(vkKey, 0x00, KEYEVENTF_KEYUP, 0x00);
	return TRUE;
}

/*
	clickPointData *clickPoints[] = { 
		//new clickPoint(240,460, L"ULDs scannen"), 
		new clickPoint(240,340, L"Einzel-ULD scannen"), 
		//new clickPoint(230,180, L"Edit Box"), 
		new clickPoint(100,220, L"ULD Edit Box"), 
		//new clickPoint(240,350, L"Übernehmen"), //Übernehmen
		//three times cursor down
		new clickPoint(240,440, L"Zur Übersicht"), //Abbrechen
		new clickPoint(430,380, L"Delete icon"), //delete symbol
		new clickPoint(395,405, L"Confirm Delete icon")  //confirm delete symbol
	};
*/