//automater.h

#include "stdafx.h"

		//nested class
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


class automater{
	private:
		HWND mhWnd;
		BOOL testWindow();
		int mScreenW;
		int mScreenH;
		TCHAR* mszClass;
		TCHAR* mszTitle;
		void getMetrics(int* width, int* height);
	public:
		automater(HWND hWnd);
		automater(TCHAR* szClass, TCHAR* szTitle);
		~automater();
		automater();

		void visualDelay(int d);
		BOOL DoClickAt(clickPoint* cp);
		BOOL DoEnterText(TCHAR* text);
		BOOL DoSendTextMsg(TCHAR* text);
		BOOL DoSendKeyEvent(BYTE vkKey);
		BOOL DoSendEnter();
		BOOL DoSendSpace();
		BOOL DoSendTab();
};