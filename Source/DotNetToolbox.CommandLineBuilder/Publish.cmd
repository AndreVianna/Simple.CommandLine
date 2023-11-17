@echo off

if [%1]==[] goto USAGE
set target=%1

cd ..
call Publish %target% DotNetToolbox.CommandLineBuilder 7.0.0-rc1
cd DotNetToolbox.CommandLineBuilder
goto :eof

:USAGE
echo Usage:
echo Publish ^<local^|remote^>
echo;

