#ifndef DLIB
#define DLIB

#include <string>
#include <iostream>
#include <cstdlib>
#include <cstdio>
#include <cmath>

using namespace std;

#ifdef __cplusplus
    extern "C" {
    #endif

#ifdef BUILD_MY_DLL
    #define SHARED_LIB __declspec(dllexport)
#else
    #define SHARED_LIB __declspec(dllimport)
#endif


bool SHARED_LIB test();

typedef double (__stdcall * Callback)(double);

double SHARED_LIB __stdcall  TestDelegate(Callback func);



#ifdef __cplusplus
    }
#endif

#endif