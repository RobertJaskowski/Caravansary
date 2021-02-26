#include "DLib.h"
#include <windows.h>
#include <strsafe.h>

#pragma comment( lib, "user32.lib") 
#pragma comment( lib, "gdi32.lib")

// typedef struct _MYHOOKDATA 
// { 
//     int nType; 
//     HOOKPROC hkprc; 
//     HHOOK hhook; 
// } MYHOOKDATA; 
 

// MYHOOKDATA myhookdata;


//my test
// typedef bool (CALLBACK* CSHARPCALLMETHOD)(int code, WPARAM wParam, LPARAM lParam);

// //this in c#?
// // bool __stdcall HookCallTest(int nCode, WPARAM wParam, LPARAM lParam){

// // }

Callback call;

double TestDelegate(Callback func)
{
    call = func;
    return func(25.0);
}


HHOOK _hook;
KBDLLHOOKSTRUCT kbdStruct;

 
LRESULT __stdcall HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
{
        call(4.0);

	if (nCode >= 0)
	{
		if (wParam == WM_KEYDOWN)
		{
			kbdStruct = *((KBDLLHOOKSTRUCT*)lParam);
			if (kbdStruct.vkCode == VK_F1)
			{
                call(3.0);
			}
            call(2.0);
		}
	}
 
	return CallNextHookEx(_hook, nCode, wParam, lParam);
}
 

void SetHook()
{
	
	if (!(_hook = SetWindowsHookEx(WH_CBT, HookCallback, GetModuleHandle(NULL), 0)))
	{

	}
}

bool test(){

    SetHook();
    return true;
    // myhookdata.nType = WH_CBT;
    // myhookdata.hkprc = CBTProc;
    // myhookdata.hhook = SetWindowsHookExA(13,)

    // hhk = SetWindowsHookEx(WH_KEYBOARD, keyboardProc, hModule, NULL);

    // SetHook();

// static HINSTANCE dllHandle;
// dllHandle = LoadLibrary(TEXT("keyDLL.dll"));
}

// LRESULT CALLBACK LowLevelKeyboardProc(
//   _In_ int    nCode,
//   _In_ WPARAM wParam,
//   _In_ LPARAM lParam
// );

// HHOOK SetWindowsHookExA(
//   int       idHook,
//   HOOKPROC  lpfn,
//   HINSTANCE hmod,
//   DWORD     dwThreadId
// );

// LRESULT WINAPI CBTProc(int, WPARAM, LPARAM);
// typedef LRESULT (CALLBACK* HOOKPROC)(int code, WPARAM wParam, LPARAM lParam);


// LRESULT CALLBACK CBTProc(int nCode, WPARAM wParam, LPARAM lParam) 
// { 
    
  
    
//     return CallNextHookEx(_hook, nCode, wParam, lParam); 
// } 


 
void ReleaseHook()
{
	UnhookWindowsHookEx(_hook);
}
 