@echo off
if [%1]==[] goto usage
if [%2]==[] goto usage
set project=%1
set version=%2

@echo [92m%project% package...[0m

cd source\%project%

@echo [93mRemove existing packages...[0m
rmdir /Q /S %userprofile%\.nuget\packages\%project%\%version%
nuget delete %project% %version% -source c:\nuget\packages -noninteractive

@echo [93mBuild project...[0m
dotnet build -c Release

@echo [93mPack project...[0m
dotnet pack -c Release

@echo [93mPublish package...[0m
nuget add bin\Release\%project%.%version%.nupkg -source c:\nuget\packages

cd ..\..

@echo [92mDone...[0m
goto :eof

:usage
@echo Usage: %0 ^<project^> ^<version^>
exit /B 1