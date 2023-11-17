@echo off

if "%1"=="" (
  echo Usage: %0 project_name
  exit /b 1
)

set project=%1
set coverage_report_folder=coverage_reports

rem Run the test command and capture the output to a file
set log_file=%project%.log
dotnet test %project% /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura > %log_file% 2>&1

rem Extract the path of the latest coverage file from the log file
set latest_coverage_file=
for /f "delims=" %%a in ('type %log_file% ^| findstr /c:"Generating report 'Cobertura'" /c:"  Total tests: "') do (
  set line=%%a
  set latest_coverage_file=!line:*'Cobertura' =!
)
if defined latest_coverage_file (
  rem Generate the report using the latest coverage file
  reportgenerator "-reports:%latest_coverage_file%" "-targetdir:%coverage_report_folder%" "-reporttypes:Html;Cobertura"
  echo Coverage report generated in %coverage_report_folder%
) else (
  echo No coverage data found
)
