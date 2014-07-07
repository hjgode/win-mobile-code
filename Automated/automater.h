//automater.h

#include "stdafx.h"

enum actions{
		click,
		type,
		keybd,
		delay
	};

//nested class
class clickPoint{
	public:
		actions action;
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


class automater{
	private:
		HWND mhWnd;
		BOOL testWindow();
		int mScreenW;
		int mScreenH;
		TCHAR mszClass[MAX_PATH];
		TCHAR mszTitle[MAX_PATH];
		void getMetrics(int* width, int* height);
		void initMsgWin();
	public:
		automater(HWND hWnd);
		automater(TCHAR *szClass, TCHAR *szTitle);
		~automater();
		automater();

		void updateMessage(TCHAR* text);
		void visualDelay(int d);
		BOOL DoClickAt(clickPoint* cp);
		BOOL DoEnterText(TCHAR* text);
		BOOL DoSendTextMsg(TCHAR* text);
		BOOL DoSendKeyEvent(BYTE vkKey);
		BOOL DoSendEnter();
		BOOL DoSendSpace();
		BOOL DoSendTab();
		BOOL exec (actions act, clickPoint* cP, TCHAR* msg, BYTE bVKey, int iDelay);
		BOOL exec (actions act, VOID* param);
};

