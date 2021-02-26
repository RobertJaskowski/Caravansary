g++ -c -DBUILD_MY_DLL DLib.cpp
g++ -shared -o DLib.dll DLib.o -Wl,--out-implib,libDLib_lib.a