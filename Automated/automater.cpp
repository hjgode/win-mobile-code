//autometer.cpp
#include "stdafx.h"
#include "automater.h"
#include "minwin.h"

HWND hWndMsgWin=NULL;


void automater::initMsgWin(){
	RECT rect;
	GetWindowRect(GetDesktopWindow(), &rect);
	rect.bottom=rect.top;
	rect.top=0;
	startWin(&rect);
	Sleep(1000);
	hWndMsgWin=FindWindow(myclass,NULL);
	if(hWndMsgWin)
		PostMessage(hWndMsgWin, WM_UPDATEWIN, (WPARAM)L'*', 0);
}

automater::automater(){
	wsprintf(mszTitle,L"");
	wsprintf(mszClass, L"");
	getMetrics(&mScreenW, &mScreenH);
}

automater::automater(HWND hWnd){
	mhWnd=hWnd;
	TCHAR szTxt[MAX_PATH];
	wsprintf(szTxt, L"hallo");
	int len=0;
	len = GetWindowText(mhWnd, mszTitle, MAX_PATH);
	len = GetClassName(mhWnd, mszClass, MAX_PATH);

	getMetrics(&mScreenW, &mScreenH);
	initMsgWin();
}

automater::automater(TCHAR *szClass, TCHAR *szTitle){
	wsprintf(mszClass, L"%s", szClass);
	wsprintf(mszTitle, L"%s", szTitle);

	mhWnd=FindWindow(mszClass,mszTitle);
	getMetrics(&mScreenW, &mScreenH);
	initMsgWin();
}

automater::~automater(){
	stopApp=TRUE;
	SetEvent(stopHandle);
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

void automater::updateMessage(TCHAR* szTxt){
	HWND hwndMsg=FindWindow(myclass, NULL);
	if(hwndMsg!=NULL){
		MYREC mRec;
		wcsncpy(mRec.s1, szTxt, 80);
		MyCDS.cbData=sizeof(MYREC);
		MyCDS.lpData=&mRec;
		MyCDS.dwData=1;
		
		SendMessage(hwndMsg, WM_COPYDATA, 0, (LPARAM) (LPVOID) &MyCDS);
	}
}

BOOL automater::DoClickAt(clickPoint* cp){
	if(!testWindow()){
		return FALSE;
	}

	TCHAR* szTxt=(TCHAR*)malloc(sizeof(TCHAR)*80);
	memset(szTxt,0,sizeof(TCHAR)*80);

	int dx = (int)((65535 / mScreenW) * cp->x); //Screen.PrimaryScreen.Bounds.Width
	int dy = (int)((65535 / mScreenH) * cp->y); //Screen.PrimaryScreen.Bounds.Height

	wsprintf(szTxt,L"Action: %s at %i/%i\n", cp->name, cp->x, cp->y);
	DEBUGMSG(1, (szTxt));
	updateMessage(szTxt);
	/*
	HWND hwndMsg=FindWindow(myclass, NULL);
	if(hwndMsg!=NULL){
		MYREC mRec;
		wcsncpy(mRec.s1, szTxt, 80);
		MyCDS.cbData=sizeof(MYREC);
		MyCDS.lpData=&mRec;
		MyCDS.dwData=1;
		
		SendMessage(hwndMsg, WM_COPYDATA, 0, (LPARAM) (LPVOID) &MyCDS);
	}
	*/
	mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	Sleep(5);
	mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	
	free(szTxt);
	return TRUE;
}

BOOL automater::DoEnterText(TCHAR* text){
	if(!testWindow()){
		return FALSE;
	}
	TCHAR sMsg[128];
	wsprintf(sMsg, L"entering data: %s", text);
	updateMessage(sMsg);

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

	TCHAR sMsg[128];
	wsprintf(sMsg, L"entering data: %s", text);
	updateMessage(sMsg);

	char* textA;
	textA = (char*)malloc((wcslen(text)+1)*sizeof(TCHAR));
	int num = wcstombs(textA, text, wcslen(text)+1);
	byte* pByte=(byte*) textA; //set pointer to first char
	int cnt=0;
	//while (pByte!=NULL && pByte!=0){
	while(cnt<num){
//		SendMessage(hWnd, WM_CHAR, *pByte, 0x40000000);	//with bit 30 set, key pressed
		Sleep(1);
		SendMessage(mhWnd, WM_CHAR, *pByte, 0x80000000);	//with bit 31 set, key released
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

BOOL automater::exec(actions act, clickPoint* cP, TCHAR *msg, BYTE bVKey, int iDelay){
	BOOL bRet=TRUE;
	switch (act){
		case click:
			bRet = DoClickAt(cP);
			break;
		case type:
			bRet = DoSendTextMsg(msg);
			break;
		case keybd:
			bRet = DoSendKeyEvent(bVKey);
			break;
		case delay:
			visualDelay(iDelay);
			break;
		default:
			bRet=FALSE;
			break;
	}
	return bRet;
}

BOOL automater::exec(actions act, VOID* param){
	BOOL bRet=TRUE;
	if(act == click){
		clickPoint* cP=(clickPoint*)param;
		bRet = DoClickAt(cP);
	}
	else if(act == type){
		TCHAR* msg=(TCHAR*)param;
		bRet = DoSendTextMsg(msg);
	}
	else if(act == keybd){
		BYTE bVKey=(BYTE)param;
		bRet = DoSendKeyEvent(bVKey);
	}
	else if(act == delay){
		int iDelay=(int)param;
		visualDelay(iDelay);
	}
	else{
		bRet=FALSE;
	}
	return bRet;
}