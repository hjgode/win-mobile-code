// Automated.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"

/*
START screen is Herausgebeauftrag
screen title "HTML5 Browser"
button: ulds_scannen = 230,210
input: 1st input box = 230,180 (double click + backspace to mark and delete all existing text?)
type: barcode_text = AKE00006LH
button: uebernehmen = 240, 350
button: abbrechen = 240, 440
delete: uld_delete1 = 436, 242
delete2: uld_delete2 = 404, 242
button: abbrechen 
--back at START
*/
TCHAR* sz_winTitle = L"HTML5 Browser";
//TCHAR* sz_winTitle = L"IntermecBrowser";
TCHAR* sz_winClass = NULL;
bool activateWindow(TCHAR* sz_Class, TCHAR* sz_Title);

TCHAR* sz_ScanData = L"AKE00006LH";

int x=430;
int y=590;

int dx = (int)((65535 / 480) * x); //Screen.PrimaryScreen.Bounds.Width
int dy = (int)((65535 / 640) * y); //Screen.PrimaryScreen.Bounds.Height

class clickPoint{
public:
	int x;
	int y;
	TCHAR* name;
	clickPoint(int i1, int i2){
		x=i1;
		y=i2;
	}
	clickPoint(int i1, int i2, TCHAR* n){
		x=i1;
		y=i2;
		name=(TCHAR*) malloc((32+1)+sizeof(TCHAR));
		wcsncpy(name, n, 32);
	}
	~clickPoint(void){
		free(name);
	}
};

void DoClickAt(int x, int y){
	BOOL bUseTouch=false;
	int dx = (int)((65535 / 480) * x); //Screen.PrimaryScreen.Bounds.Width
	int dy = (int)((65535 / 640) * y); //Screen.PrimaryScreen.Bounds.Height

	if (bUseTouch){
		mouse_event(MOUSEEVENTF_TOUCH, dx, dy, 0, 0);
		Sleep(1);
	}else{
		mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
		Sleep(10);
		mouse_event(MOUSEEVENTF_LEFTUP , 0, 0, 0, 0);
		Sleep(5);
	}
}

int screenW, screenH;

void GetMetrics(int* width, int* height){
	int screenX = GetSystemMetrics(SM_CXFULLSCREEN);
	int screenY = GetSystemMetrics(SM_CYFULLSCREEN);
	*width=screenX;
	*height=screenY;
}

BOOL DoClickAt(clickPoint* cp){

	if(!activateWindow(sz_winClass, sz_winTitle)){
		return FALSE;
	}

	int dx = (int)((65535 / screenW) * cp->x); //Screen.PrimaryScreen.Bounds.Width
	int dy = (int)((65535 / screenH) * cp->y); //Screen.PrimaryScreen.Bounds.Height

	DEBUGMSG(1, (L"Action: %s at %i/%i\n", cp->name, cp->x, cp->y));

	mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	Sleep(5);
	mouse_event(MOUSEEVENTF_LEFTUP | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	return TRUE;
}

BOOL DoEnterText(TCHAR* text){
	if(!activateWindow(sz_winClass, sz_winTitle)){
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

void DoSendTab(){
	keybd_event(0x09, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(0x09, 0x00, KEYEVENTF_KEYUP, 0x00);
}

void DoSendSpace(){
	keybd_event(0x20, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(0x20, 0x00, KEYEVENTF_KEYUP, 0x00);
}

void DoSendEnter(){
	keybd_event(VK_RETURN, 0x00, 0x00, 0);
	Sleep(1);
	keybd_event(VK_RETURN, 0x00, KEYEVENTF_KEYUP, 0x00);
}

clickPoint *clickPoints[] = { 
	new clickPoint(230,210, L"ULDs scannen"), 
	new clickPoint(230,180, L"Edit Box"), 
	new clickPoint(240,350, L"Übernehmen"), //Übernehmen
	new clickPoint(240,440, L"Abbrechen"), //Abbrechen
	new clickPoint(436,242, L"Delete icon"), //delete symbol
	new clickPoint(404,240, L"Confirm Delete icon")  //confirm delete symbol
};



const WORD WM_UPDATESIGNAL = WM_USER + 1704;
static LRESULT CALLBACK TaskWndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam); //CallBAck for TaskBar Hook
static HWND g_hWnd = NULL; 
static WNDPROC g_fnProcTask = NULL; 
//----------------------------------------------------------------------
//HOOK Into TaskBar WndProc
//
BOOL HookWindow(TCHAR* szTitle) 
{ 
	// 
	// Already hooked? 
	// 
	if(g_fnProcTask) 
		return FALSE; 
	g_hWnd = FindWindow(NULL, szTitle); 
	if(g_hWnd) { 
		//g_hWndMain = g_hWnd; // GetSafeHwnd(); 
		g_fnProcTask = (WNDPROC)GetWindowLong(g_hWnd, GWL_WNDPROC); 
		SetWindowLong(g_hWnd, GWL_WNDPROC, (LONG)TaskWndProc); 
	} 
	return g_hWnd != NULL; 
} 

//----------------------------------------------------------------------
BOOL UnhookWindow() 
{ 
	// 
	// Already freed? 
	// 
	if(!g_fnProcTask) 
		return FALSE; 
	SetWindowLong(g_hWnd, GWL_WNDPROC, (LONG)g_fnProcTask); 
	g_fnProcTask = NULL; 
	return TRUE; 
} 

// TaskWndProc // 
// Handles the WM_LBUTTONUP message // 
LRESULT TaskWndProc(HWND hWnd, UINT msg, WPARAM wParam, LPARAM lParam) 
{ 
	DEBUGMSG(1, (L"msg!\n"));

	if(msg == WM_UPDATESIGNAL) { 
		DEBUGMSG(1, (L"WM_UPDATESIGNAL\n"));
		RECT rc; 
		POINT pt; 
		rc.left = 240 - 26; 
		rc.top = 0; 
		rc.bottom = 26; 
		rc.right = 240; 
		pt.x = LOWORD(lParam); 
		pt.y = HIWORD(lParam); 
		if(PtInRect(&rc, pt)) { 
			//PostMessage(g_hWnd, WM_CLOSE, 0, 0); 
			//simply do nothing
			return true;
			return CallWindowProc( g_fnProcTask, hWnd, WM_MOUSEMOVE, 0, MAKELPARAM(200, 0)); 
		} 
	} 
	return CallWindowProc(g_fnProcTask, hWnd, msg, wParam, lParam); 
}

/*
	liveview:
	click inside 384,540:470,626 ie at 427:583
*/
DWORD clickOK = MAKELONG(430,590);
//DWORD clickOK = MAKELONG(590,430);

HWND findFiveMobileLiveView(){

	//getProcList(); //build list of processes

	HWND hWnd=FindWindow(NULL, L"ImagerCapture");	//will only find top level windows
	/*
	wsprintf(szFindTitle, L"ImagerCapture");
	hwndFiveMobileLiveView=NULL;
	//start find
	EnumWindows(procEnumWindows, 0);
	hwnd=hwndFiveMobileLiveView;
	*/

	return hWnd;
}

void visualDelay(int delay){
	for (int i=1; i<=delay; i++){
		DEBUGMSG(1, (L"."));
		Sleep(i*1000);
	}
}


void clickOK1(){
	mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_ABSOLUTE , dx, dy, 0, 0);
	Sleep(1);
	mouse_event(MOUSEEVENTF_LEFTUP , 0, 0, 0, 0);
	Sleep(1);
}

// refereshing thread
bool bRunThread=true;
DWORD threadID=0;
HANDLE hThread = NULL;
ULONG uTestCount=0;
ULONG getStatus(){
	return uTestCount;
}

void logTime(){
	SYSTEMTIME st;
	GetLocalTime(&st);
	DEBUGMSG(1, (L"%04i%02i%02i %02i:%02i:%02i.%02i ", st.wYear, st.wMonth, st.wDay,
		st.wHour, st.wMinute, st.wSecond, st.wMilliseconds));
}

bool activateWindow(TCHAR* sz_Class, TCHAR* sz_Title){
	HWND hWnd = FindWindow(sz_Class, sz_Title);
	if(hWnd==NULL)
		if(sz_Class!=NULL && sz_winTitle==NULL)
			DEBUGMSG(1, (L"Window '%s' NOT FOUND\n", sz_Class));
		else if(sz_Class==NULL && sz_winTitle!=NULL)
			DEBUGMSG(1, (L"Window '%s' NOT FOUND\n", sz_Title));
		else if(sz_Class!=NULL && sz_winTitle!=NULL)
			DEBUGMSG(1, (L"Window '%s'/'%s' NOT FOUND\n", sz_Class, sz_Title));
		else{
			DEBUGMSG(1, (L"Window NULL NOT FOUND\n"));
			return false;
		}
	else{
		SetForegroundWindow(hWnd);
		return true;
	}
	return false;
}

int _tmain(int argc, _TCHAR* argv[])
{	
	GetMetrics(&screenW, &screenH);
	HWND hWnd = FindWindow(NULL, sz_winTitle);
	hWnd = FindWindow(sz_winTitle, NULL); //Intermec Browser CLASS
	if (hWnd==NULL){
		DEBUGMSG(1, (L"'%s' running?\n", sz_winTitle));
		return -1;	
	}

	//if(HookWindow(L"ImagerCapture")){
	//	DEBUGMSG(1, (L"hook OK\n"));
	//	if(PostMessage(hWnd, WM_UPDATESIGNAL, (LPARAM)10, 0))
	//		DEBUGMSG(1, (L"post OK\n"));
	//	else
	//		DEBUGMSG(1, (L"post failed\n"));
	//}
	//else
	//	DEBUGMSG(1, (L"hook failed\n"));

	//START
	SetForegroundWindow(hWnd);
	BOOL useMouse=true;

	int maxTestCount = 100;

	for(int iX=0; iX<maxTestCount; iX++){
		logTime();
		if(!activateWindow(sz_winClass, sz_winTitle)){
			return -2;
		}
		DEBUGMSG(1, (L"test round %i\n", iX));
		if(useMouse){
			//DoClickAt(clickPoints[0]->x, clickPoints[0]->y);	//click ULDs scannen
			if(!DoClickAt(clickPoints[0]))
				return -3;
			//DEBUGMSG(1, (L"Action: %s\n", clickPoints[0]->name));
			visualDelay(4);
			if(!DoClickAt(clickPoints[1]))	//click inside first edit box
				return -3;
			DEBUGMSG(1, (L"Action: %s\n", clickPoints[1]->name));
			visualDelay(1);
			DEBUGMSG(1, (L"Action: enter data %s\n", sz_ScanData));
			if(!DoEnterText(sz_ScanData))							//enter data in edit box
				return -3;
			visualDelay(1);
			//DEBUGMSG(1, (L"Action: %s\n", clickPoints[2]->name));
			if(!DoClickAt(clickPoints[2]))	//click Übernehmen
				return -3;
			
			//DoSendSpace();	//to ensure Übernehmen is clicked
			visualDelay(4);
			if(!DoClickAt(clickPoints[4]))	//click delete icon
				return -3;
			visualDelay(3);
			//DEBUGMSG(1, (L"Action: %s\n", clickPoints[5]->name));
			if(!DoClickAt(clickPoints[5]))	//click delete confirm
				return -3;
			visualDelay(6);
			//Back at START
		}else{
			//START, we are on Herausgabeauftrag
			//execute [ULDs scannen] by pressing RETURN two times!!
			DEBUGMSG(1, (L"Action: [ULDs scannen]\n"));
			DoSendEnter(); DoSendEnter();
			visualDelay(4);

			DEBUGMSG(1, (L"Action: enter data %s\n", sz_ScanData));
			DoEnterText(sz_ScanData);							//enter data in edit box
			visualDelay(1);
			DEBUGMSG(1, (L"Action: ->[Übernehmen] (1)\n"));

			//now data entered and second empty row
			DoSendTab();	//to move onto [Übernehmen]
			visualDelay(1);
			DEBUGMSG(1, (L"Action: ->[Übernehmen] (2)\n"));
			DoSendSpace();	//execute [Übernehmen]
			visualDelay(2); //focus on  delete symbol

			DEBUGMSG(1, (L"Action: [Delete]\n"));
			DoSendSpace();	//focus on DELETE
			visualDelay(1);	//exec delete
			DEBUGMSG(1, (L"Action: [Confirm] (1)\n"));
			DoSendTab();	//focus on rect around confirm symbol
			DEBUGMSG(1, (L"Action: [Confirm] (2)\n"));
			DoSendTab();	//focus on confirm
			DEBUGMSG(1, (L"Action: [Confirm] (3)\n"));
			DoSendSpace();	//exec confirm
			//back at Herausgabeauftrag
			visualDelay(6);
		}
	}

	return 2;

	for(int i=0; i<130; i++){
		//we start in LiveView and Click brings us to PreView
		DEBUGMSG(1, (L"ClickCount = %i: LiveView:", i+1));
		visualDelay(1);
		clickOK1();
		DEBUGMSG(1, (L"PreView:"));
		visualDelay(3);
		clickOK1();
		DEBUGMSG(1, (L"\n"));
		
		//PostMessage(hWnd, WM_UPDATESIGNAL, (LPARAM)i, 0);
	}

	UnhookWindow();
	return 0;

}

