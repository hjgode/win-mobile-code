#include "pwdialog.h"

//defined in //#include "..\common\locktaskbar.h"
extern BOOL ShowSIP(bool show);

//the password entered in password dialog
TCHAR pw[]=L"................";

// Mesage handler for the Password box.
LRESULT CALLBACK PasswordProc(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{


	RECT rt, rt1;
	int DlgWidth, DlgHeight;	// dialog width and height in pixel units
	int NewPosX, NewPosY;

	switch (message)
	{
		case WM_INITDIALOG:
			// trying to center the About dialog
			if (GetWindowRect(hDlg, &rt1)) {
				GetClientRect(GetParent(hDlg), &rt);
				DlgWidth	= rt1.right - rt1.left;
				DlgHeight	= rt1.bottom - rt1.top ;
				NewPosX		= (rt.right - rt.left - DlgWidth)/2;
				NewPosY		= (rt.bottom - rt.top - DlgHeight)/2;
				
				// if the About box is larger than the physical screen 
				// if (NewPosX < 0) NewPosX = 0;
				// if (NewPosY < 0) NewPosY = 0;
				//align the dialog top left
				NewPosX = 0;
				NewPosY = 0;
				SetWindowPos(hDlg, 0, NewPosX, NewPosY,
					0, 0, SWP_NOZORDER | SWP_NOSIZE);
				//show the sip
				ShowSIP(true);
				
			}
			return TRUE;

		case WM_COMMAND:
			if ((LOWORD(wParam) == IDOK) || (LOWORD(wParam) == IDCANCEL))
			{
				//hide the sip
				ShowSIP(false);
                // Get text from edit control.
				GetDlgItemText (hDlg, IDC_PWTEXT, pw, sizeof (pw));
				if (wcscmp(pw, L"52401") == 0)
					EndDialog(hDlg, IDOK);
				else
					EndDialog(hDlg, IDCANCEL); //LOWORD(wParam));
				return TRUE;
			}
			if (LOWORD(wParam) == IDCANCEL)
			{
				EndDialog(hDlg, IDCANCEL); //LOWORD(wParam));
				return TRUE;
			}
			break;
	}
    return FALSE;
}
