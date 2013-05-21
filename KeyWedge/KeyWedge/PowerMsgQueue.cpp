//PowerMsgQueue.cpp

#include "PowerMsgQueue.h"
#include "./common/nclog.h"

HANDLE hExitPwrMsgThreadEvent;
TCHAR szExitPwrMsgThreadEventName[MAX_PATH] = L"szExitPwrMsgThreadEventName";

HANDLE hPwrMsgThreadHasStoppedEvent;
TCHAR szPwrMsgThreadHasStoppedEventName[MAX_PATH] = L"szPwrMsgThreadHasStoppedEventName";

HANDLE hPwrMsgQueue;
HANDLE hPwrMsgQueueThread;

HWND hwndMain;

char*  logDateTimeA(){
	static char cDateTimeStr[64];
	TCHAR str[64];
	wsprintf(str, L"%s", logDateTime());
	wcstombs(cDateTimeStr, str, wcslen(str));
	return cDateTimeStr;
}
//
//int InitTransitionsPowerNotification(void)
//{
//	int iRet=0;
//	nclog(L"\tInitTransitionsPowerNotification()...\n");
//	MSGQUEUEOPTIONS mqoQueueOptions;
//	mqoQueueOptions.dwSize = sizeof(mqoQueueOptions); 
//	mqoQueueOptions.dwFlags = MSGQUEUE_NOPRECOMMIT;
//	mqoQueueOptions.dwMaxMessages = 0;
//	mqoQueueOptions.cbMaxMessage = sizeof(POWER_BROADCAST) + MAX_NAMELEN;
//	mqoQueueOptions.bReadAccess = TRUE;
//
//	hTransitionsStatusQueue = CreateMsgQueue(NULL, &mqoQueueOptions);
//	if(hTransitionsStatusQueue==NULL){
//		nclog(L"\tCreateMsgQueue failed.\n");
//		iRet=-1;
//	}
//	else{
//		if(RequestPowerNotifications(hTransitionsStatusQueue, PBT_TRANSITION)==0){ //PBT_TRANSITION  PBT_RESUME
//			nclog(L"\tRequestPowerNotifications failed.\n");
//			iRet=-2;
//		}
//	}
//	nclog(L"\tInitTransitionsPowerNotification() done with ret=%i\n", iRet);
//	return iRet;
//}

void dumpPowerFlags(POWER_BROADCAST* ppb, TCHAR* szPPB){
	wsprintf(szPPB, L"");
	if(ppb->Flags & POWER_STATE_ON)
		wcscat(szPPB, L"ON|");
	if(ppb->Flags & POWER_STATE_OFF)
		wcscat(szPPB, L"OFF|");
	if(ppb->Flags & POWER_STATE_BOOT)
		wcscat(szPPB, L"ON|");
	if(ppb->Flags & POWER_STATE_CRITICAL)
		wcscat(szPPB, L"CRITICAL|");
	if(ppb->Flags & POWER_STATE_IDLE)
		wcscat(szPPB, L"IDLE|");
	if(ppb->Flags & POWER_STATE_PASSWORD)
		wcscat(szPPB, L"PASSWORD|");
#if _WIN32_WCE > 0x501
	if(ppb->Flags & POWER_STATE_BACKLIGHTON)
		wcscat(szPPB, L"BACKLIGHTON|");
#endif
	if(ppb->Flags & POWER_STATE_SUSPEND)
		wcscat(szPPB, L"SUSPEND|");
	if(ppb->Flags & POWER_STATE_UNATTENDED)
		wcscat(szPPB, L"UNATTENDED|");
	if(ppb->Flags & POWER_STATE_RESET)
		wcscat(szPPB, L"RESET|");
	if(ppb->Flags & POWER_STATE_USERIDLE)
		wcscat(szPPB, L"USERIDLE|");
	wcscat(szPPB, L"\0");
}

//***************************************************************************
// Function Name: PowerNotificationThread
//
// Purpose: listens for power change notifications
//
DWORD PowerMsgsThread(LPVOID pVoid)
{
    // size of a POWER_BROADCAST message
    DWORD cbPowerMsgSize = sizeof POWER_BROADCAST + (MAX_PATH * sizeof TCHAR);

    // Initialize our MSGQUEUEOPTIONS structure
    MSGQUEUEOPTIONS mqo;
    mqo.dwSize = sizeof(MSGQUEUEOPTIONS); 
    mqo.dwFlags = MSGQUEUE_NOPRECOMMIT;
    mqo.dwMaxMessages = 4;
    mqo.cbMaxMessage = cbPowerMsgSize;
    mqo.bReadAccess = TRUE;              
                                         
    // Create a message queue to receive power notifications
    HANDLE hPowerMsgQ = CreateMsgQueue(NULL, &mqo);
    if (NULL == hPowerMsgQ) 
    {
        RETAILMSG(1, (L"CreateMsgQueue failed: %x\n", GetLastError()));
        goto Error;
    }

    // Request power notifications 
    HANDLE hPowerNotifications = RequestPowerNotifications(hPowerMsgQ,
                                                           PBT_TRANSITION | 
                                                           PBT_RESUME | 
                                                           PBT_POWERINFOCHANGE);
    if (NULL == hPowerNotifications) 
    {
        nclog(L"RequestPowerNotifications failed: %x\n", GetLastError());
        goto Error;
    }

	//create exit thread handle event
	hExitPwrMsgThreadEvent = CreateEvent(NULL, FALSE, FALSE, szExitPwrMsgThreadEventName);
	hPwrMsgThreadHasStoppedEvent = CreateEvent(NULL, FALSE, FALSE, szPwrMsgThreadHasStoppedEventName);

    HANDLE rgHandles[2] = {0};
    rgHandles[0] = hPowerMsgQ;	//will be signaled on power changes
    rgHandles[1] = hExitPwrMsgThreadEvent;	//used to signal the thread to exit
	int iPost=0;

    // Wait for a power notification or for the app to exit
    while(WaitForMultipleObjects(2, rgHandles, FALSE, INFINITE) == WAIT_OBJECT_0)
    {
        DWORD cbRead;
        DWORD dwFlags;
        POWER_BROADCAST *ppb = (POWER_BROADCAST*) new BYTE[cbPowerMsgSize];
        TCHAR* szPPB = new TCHAR[MAX_PATH];  
		memset(szPPB, 0, MAX_PATH * 2);
		TCHAR* szPBtype=new TCHAR[MAX_PATH];
		memset(szPBtype, 0, MAX_PATH * 2);
		TCHAR szOut[MAX_PATH];
        // loop through in case there is more than 1 msg 
        while(ReadMsgQueue(hPowerMsgQ, ppb, cbPowerMsgSize, &cbRead, 
                           0, &dwFlags))
        {
			wsprintf(szPPB, L"");
			wsprintf(szPBtype, L"");
			nclog(L"ReadMsgQueue: %s\n", ppb->SystemPowerState);
            switch (ppb->Message)
            {
				case PBT_POWERSTATUSCHANGE:
                    nclog(L"Power Notification Message: PBT_POWERSTATUSCHANGE\n");
					wsprintf(szPBtype, L"change: ");
					//Add2Log(L"Power Notification Message: PBT_POWERSTATUSCHANGE\n",TRUE);
					break;
				case PBT_SUSPENDKEYPRESSED:
                    nclog(L"Power Notification Message: PBT_SUSPENDKEYPRESSED\n");
					wsprintf(szPBtype, L"keypress: ");
					//Add2Log(L"Power Notification Message: PBT_SUSPENDKEYPRESSED\n",TRUE);
					break;
                case PBT_TRANSITION:
                    nclog(L"Power Notification Message: PBT_TRANSITION\n");
					//Add2Log(L"Power Notification Message: PBT_TRANSITION\n",TRUE);
                    nclog(L"Flags: %lx\n", ppb->Flags);
                    nclog(L"Length: %d\n", ppb->Length);
					wsprintf(szPBtype, L"trans.: ");
/*
Flags: 12010000
Length: 6
trans.: ON|PASSWORD|BACKLIGHTON|
*/
					//				 0x10000000				0x00010000		 0x02000000
					if( ((ppb->Flags & POWER_STATE_PASSWORD) == POWER_STATE_PASSWORD) ||
						((ppb->Flags & POWER_STATE_ON) == POWER_STATE_ON) 
#if _WIN32_WCE > 0x501
						||
						((ppb->Flags & POWER_STATE_BACKLIGHTON) == POWER_STATE_BACKLIGHTON) 
#endif
						)
					{
						nclog(L"PwrMsgQueue: got 'ON|PASSWORD|BACKLIGHTON'...\n");
						//send a message to main window
						iPost = PostMessage(hwndMain, WM_USER_RESUMECOMM, 0, 0);
						nclog(L"PostMessage WM_USER_RESUMECOMM returned %i\n", iPost);
					}
                    break;

                case PBT_RESUME:
                    nclog(L"Power Notification Message: PBT_RESUME\n");
					//Add2Log(L"Power Notification Message: PBT_RESUME\n",TRUE);
					wsprintf(szPBtype, L"resume: ");
					//send a message to main window
					iPost = PostMessage(hwndMain, WM_USER_RESUMECOMM, 0, 0);
					nclog(L"PostMessage WM_USER_RESUMECOMM returned %i\n", iPost);
					nclog(L"Power: PBT_RESUME\n");

                    break;

                case PBT_POWERINFOCHANGE:
                {
                    nclog(L"Power Notification Message: PBT_POWERINFOCHANGE\n");
					//Add2Log(L"Power Notification Message: PBT_POWERINFOCHANGE\n", TRUE);
                    // PBT_POWERINFOCHANGE message embeds a 
                    // POWER_BROADCAST_POWER_INFO structure into the 
                    // SystemPowerState field
					wsprintf(szPBtype, L"info: ");

                    //PPOWER_BROADCAST_POWER_INFO ppbpi =
                    //    (PPOWER_BROADCAST_POWER_INFO) ppb->SystemPowerState;
                    //if (ppbpi) 
                    //{
                    //    RETAILMSG(1,(L"Length: %d\n", ppb->Length));
                    //    RETAILMSG(1,(L"BatteryLifeTime = %d\n",ppbpi->dwBatteryLifeTime));
                    //    RETAILMSG(1,(L"BatterFullLifeTime = %d\n",
                    //                 ppbpi->dwBatteryFullLifeTime));
                    //    RETAILMSG(1,(L"BackupBatteryLifeTime = %d\n",
                    //                 ppbpi->dwBackupBatteryLifeTime));
                    //    RETAILMSG(1,(L"BackupBatteryFullLifeTime = %d\n",
                    //                 ppbpi->dwBackupBatteryFullLifeTime));
                    //    RETAILMSG(1,(L"ACLineStatus = %d\n",ppbpi->bACLineStatus));
                    //    RETAILMSG(1,(L"BatteryFlag = %d\n",ppbpi->bBatteryFlag));
                    //    RETAILMSG(1,(L"BatteryLifePercent = %d\n",
                    //                 ppbpi->bBatteryLifePercent));
                    //    RETAILMSG(1,(L"BackupBatteryFlag = %d\n",
                    //                 ppbpi->bBackupBatteryFlag));
                    //    RETAILMSG(1,(L"BackupBatteryLifePercent = %d\n",
                    //                 ppbpi->bBackupBatteryLifePercent));
                    //}
                    break;
                }

                default:
					nclog(L"Unknown PwrMsg: 0x%08x", ppb->Message);
                    break;
            }
            //dumpBattery();
			dumpPowerFlags(ppb, szPPB);
			wsprintf(szOut, L"%s%s\n", szPBtype, szPPB);
			nclog(szOut);
			//dumpBattery(szOut);
        }
        delete[] ppb;
		delete(szPPB);
		delete(szPBtype);
    }
	SetEvent(hPwrMsgThreadHasStoppedEvent);
Error:
    if (hPowerNotifications)
        StopPowerNotifications(hPowerNotifications);

    if (hPowerMsgQ)
        CloseMsgQueue(hPowerMsgQ);
	CloseHandle(hExitPwrMsgThreadEvent); //no more needed
    return NULL;
}

//DWORD WINAPI TransitionsUpdateThread(LPVOID pvarg)
//{
//	writefile(L"Thread starting...\n");
//	DWORD dwRet=0;
//
//	HANDLE hWaitHandles[2];
//	DWORD dwWaitStatus;
//	DWORD dwBytesRead;
//	DWORD dwFlags;
//	DWORD dwStateNumber = 0;
//	DWORD dwOnCount = 0;
//	FILE *fp;
//
//    union 
//	{
//        UCHAR buf[sizeof(POWER_BROADCAST) + MAX_NAMELEN];
//        POWER_BROADCAST powerBroadcast;
//    } qData;
//
//	if((dwRet = InitTransitionsPowerNotification()) != 0){
//		writefile(L"InitTransitionsPowerNotification failed\nEnding Thread.");
//		ExitThread(dwRet);
//	}
//	//hPwrMsgThreadHasStoppedEvent = CreateEvent(NULL, FALSE, FALSE, NULL);
//	hPwrMsgThreadHasStoppedEvent = CreateEvent(NULL, FALSE, FALSE, szPwrMsgThreadHasStoppedEventName);
//	if(hPwrMsgThreadHasStoppedEvent==NULL){
//		writefile(L"CreateEvent: hPwrMsgThreadHasStoppedEvent failed\nEnding Thread.");
//		ExitThread(GetLastError());
//	}
//
//	hExitPwrMsgThreadEvent = CreateEvent(NULL, FALSE, FALSE, szExitPwrMsgThreadEventName );
//	if(hExitPwrMsgThreadEvent==NULL){
//		writefile(L"CreateEvent: hExitPwrMsgThreadEvent failed\nEnding Thread.");
//		ExitThread(GetLastError());
//	}
//
//	hWaitHandles[0] = hPwrMsgQueue;
//	hWaitHandles[1] = hExitPwrMsgThreadEvent;
//
//	writefile(L"Starting thread loop...\nFurther thread infos write to '\\power.txt'\n");
//	BOOL bRunThread=TRUE;
//
//	do
//	{
//		dwWaitStatus = WaitForMultipleObjects(2, hWaitHandles, FALSE, INFINITE);
//		if (dwWaitStatus == WAIT_OBJECT_0)
//		{
//			if (ReadMsgQueue(hPwrMsgQueue, &qData, sizeof(qData), &dwBytesRead, INFINITE, &dwFlags))
//			{
//				fp = fopen("\\power.txt", "a+");
//				fprintf(fp, logDateTimeA()); fprintf(fp, "\t");
//				char cChar[64]; sprintf(cChar, "0x%08x\t", (qData.powerBroadcast).Flags);
//				fprintf(fp, cChar);
//
//				if ((qData.powerBroadcast).Flags & POWER_STATE_ON)
//				{
//					/* 
//					will be done now by BattSaveWakeUp.exe itself 
//					*/
//					//if(!_bTestMode){
//					//	if(clearTimedEvent()==0)
//					//		fprintf(fp, "Power: POWER_STATE_ON, Timed event cleared\n");
//					//	else
//					//		fprintf(fp, "Power: POWER_STATE_ON, Timed event clear failed\n");
//					//}
//				}
//
//				if ((qData.powerBroadcast).Flags & POWER_STATE_SUSPEND)
//				{
//					fprintf(fp, "Power: POWER_STATE_SUSPEND\n");
//					//if(addTimedEvent()==0)
//						//fprintf(fp, "Power: POWER_STATE_SUSPEND with WakeUp event set\n");
//					//else
//					//	fprintf(fp, "Power: POWER_STATE_SUSPEND with WakeUp event set FAILED\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_IDLE)
//				{
//					fprintf(fp, "Power: POWER_STATE_IDLE\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_OFF)
//				{
//					fprintf(fp, "Power: POWER_STATE_OFF\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_UNATTENDED)
//				{
//					fprintf(fp, "Power: POWER_STATE_UNATTENDED\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_USERIDLE)
//				{
//					fprintf(fp, "Power: POWER_STATE_USERIDLE\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_BOOT)
//				{
//					fprintf(fp, "Power: POWER_STATE_BOOT\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_CRITICAL)
//				{
//					fprintf(fp, "Power: POWER_STATE_CRITICAL\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_PASSWORD)
//				{
//					//send a message to main window
//					int iPost = PostMessage(hwndMain, WM_USER_RESUMECOMM, 0, 0);
//					writefile(L"PostMessage WM_USER_RESUMECOMM returned %i\n", iPost);
//					fprintf(fp, "Power: POWER_STATE_PASSWORD\n");
//				}
//				if ((qData.powerBroadcast).Flags & POWER_STATE_RESET)
//				{
//					fprintf(fp, "Power: POWER_STATE_RESET\n");
//				}
//				fflush(fp);
//				fclose(fp);
//			}
//		}
//		if (dwWaitStatus == WAIT_OBJECT_0 + 1)
//		{
//			fprintf(fp, "PowerThread exit requested\n");
//			bRunThread=FALSE;
//		}
//		else
//		{
//			break;
//		}
//	}while (bRunThread);
//	writefile(L"...Thread loop ended\n");
//	CloseHandle(hExitPwrMsgThreadEvent);
//	SetEvent(hPwrMsgThreadHasStoppedEvent);
//	DEBUGMSG(1, (L"Thread stopped\n"));
//	writefile(L"Thread stopped\n");
//	return(0);
//}
//
//int startMsgThread(){
//	hPwrMsgQueueThread = CreateThread(NULL, 0, &TransitionsUpdateThread, NULL, 0, 0);
//	if(hPwrMsgQueueThread!=NULL){
//		SetThreadPriority(hPwrMsgQueueThread, THREAD_PRIORITY_ABOVE_NORMAL);
//		return 0;
//	}
//	else 
//		return -1;
//}

int startMsgThread(){
	DWORD rc=0;
	hPwrMsgQueueThread = CreateThread(NULL, 0, &PowerMsgsThread, NULL, 0, &rc);
	if(hPwrMsgQueueThread!=NULL){
		SetThreadPriority(hPwrMsgQueueThread, THREAD_PRIORITY_ABOVE_NORMAL);
		nclog(L"Started MessageThread with id=0x%x", rc);
		return 0;
	}
	else 
		return -1;
}

int stopMsgThread(){
	int iResult=0;
	hPwrMsgThreadHasStoppedEvent = CreateEvent(NULL, FALSE, FALSE, szPwrMsgThreadHasStoppedEventName); //hopefully an exisiting event as thread is started
	if(GetLastError()==ERROR_ALREADY_EXISTS){
		nclog(L"stopMsgThread: using existing hPwrMsgThreadHasStoppedEvent handle to wait for PwrMsgThread End...\n");
		if(hExitPwrMsgThreadEvent!=NULL){
			nclog(L"stopMsgThread: using existing hExitPwrMsgThreadEvent handle to signal Exit to PwrMsgThread...\n");
			PulseEvent(hExitPwrMsgThreadEvent);

			nclog(L"stopMsgThread: Waiting for thread end...\n");
			
			DWORD dwStat = WaitForSingleObject(hPwrMsgThreadHasStoppedEvent, INFINITE);
			switch(dwStat){
				case WAIT_OBJECT_0:
					nclog(L"stopMsgThread: PwrMsgThread signaled it has ended\n");
					iResult = 1;
					break;
				case WAIT_ABANDONED:
					RETAILMSG(1, (L"stopMsgThread: WaitForSingleObject abondoned!\n"));
					iResult = -1;
					break;
				case WAIT_FAILED:
					RETAILMSG(1, (L"stopMsgThread: WaitForSingleObject failed!\n"));
					iResult = -2;
					break;
			}
		}
		else
			RETAILMSG(1, (L"stopMsgThread: No hExitPwrMsgThreadEvent handle found! PwrMsgThread running?\n"));
	}
	else
		RETAILMSG(1, (L"stopMsgThread: hPwrMsgThreadHasStoppedEvent not found! PwrMsgThread running?\n"));
	CloseHandle(hExitPwrMsgThreadEvent);
	CloseHandle(hPwrMsgQueueThread);
	CloseHandle(hPwrMsgThreadHasStoppedEvent);
	return iResult;
}

//int stopMsgThread(){
//	int iResult=0;
//	hPwrMsgThreadHasStoppedEvent = CreateEvent(NULL, FALSE, FALSE, szPwrMsgThreadHasStoppedEventName); //hopefully an exisiting event as thread is started
//	PulseEvent(hExitPwrMsgThreadEvent);
//
//	nclog(L"Waiting for thread end...\n");
//	DWORD dwStat = WaitForSingleObject(hPwrMsgThreadHasStoppedEvent, INFINITE);
//	switch(dwStat){
//		case WAIT_OBJECT_0:
//			iResult = 1;
//			break;
//		case WAIT_ABANDONED:
//			iResult = -1;
//			break;
//		case WAIT_FAILED:
//			iResult = -2;
//			break;
//	}
//	return iResult;
//}