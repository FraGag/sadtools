@echo off

REM Run this batch file from a command prompt in which the PATH environment
REM variable is set to contain the location of xsd.exe. xsd.exe is part of the
REM Windows SDK, which is installed as part of Visual Studio. Therefore, it
REM should be available from the Visual Studio Command Prompt.

REM Make sure to set the working directory to the directory that contains the
REM batch file before running it, otherwise SADXPCModule.xsd will not be found.

xsd.exe /nologo SADXPCModule.xsd /c /n:SonicRetro.SonicAdventure.SADXPCDecompiler
