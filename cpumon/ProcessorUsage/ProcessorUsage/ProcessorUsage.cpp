// ProcessorUsage.cpp : main source file for ProcessorUsage.exe
//

#include "stdafx.h"

#include "resourceppc.h"

#include "ProcessorUsageDialog.h"

CAppModule _Module;

int WINAPI _tWinMain(HINSTANCE hInstance, HINSTANCE /*hPrevInstance*/, LPTSTR lpstrCmdLine, int nCmdShow)
{
    HRESULT hRes = CProcessorUsageDialog::ActivatePreviousInstance(hInstance, lpstrCmdLine);

    if(FAILED(hRes) || S_FALSE == hRes)
    {
        return hRes;
    }

    hRes = ::CoInitializeEx(NULL, COINIT_MULTITHREADED);
    ATLASSERT(SUCCEEDED(hRes));

    AtlInitCommonControls(ICC_DATE_CLASSES);
    SHInitExtraControls();

    hRes = _Module.Init(NULL, hInstance);
    ATLASSERT(SUCCEEDED(hRes));

    int nRet = CProcessorUsageDialog::AppRun(lpstrCmdLine, nCmdShow);

    _Module.Term();
    ::CoUninitialize();

    return nRet;
}

