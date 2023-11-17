@echo off

if [%1]==[] goto USAGE
set target=%1

call Publish %target% DotNetToolbox.CommandLineBuilder 7.0.0
goto :eof

:USAGE
echo Usage:
echo PublishAll ^<local^|remote^>
echo;
