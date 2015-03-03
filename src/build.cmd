@echo off

:CheckOS
IF EXIST "%PROGRAMFILES(X86)%" (GOTO 64BIT) ELSE (GOTO 32BIT)

:64BIT
"%programfiles(x86)%\MSBuild\14.0\Bin\msbuild.exe" Krawlr.sln /m:4 /p:Configuration=Release /p:Platform="Any CPU"
GOTO END

:32BIT
"%programfiles%\MSBuild\14.0\Bin\msbuild.exe" Krawlr.sln /m:4 /p:Configuration=Release /p:Platform="Any CPU"
GOTO END

:END
pause