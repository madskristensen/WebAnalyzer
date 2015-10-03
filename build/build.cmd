@echo off

if exist %~dp0..\src\WebLinter\Node\node_modules.7z goto:done

pushd %~dp0..\src\WebLinter\Node

echo Installing packages...
call npm install tslint --no-optional --quiet > nul
call npm install coffeelint --no-optional --quiet > nul
call npm install csslint --no-optional --quiet > nul
call npm install eslint --no-optional --quiet > nul
call npm install eslint-plugin-react --no-optional --quiet > nul


echo Deleting unneeded files and folders...
del /s /q *.md > nul
del /s /q *.markdown > nul
del /s /q *.html > nul
del /s /q *.txt > nul
del /s /q *.old > nul
del /s /q *.patch > nul
del /s /q *.yml > nul
del /s /q *.npmignore > nul
del /s /q makefile.* > nul
del /s /q generate-* > nul
del /s /q .jshintrc > nul
del /s /q .jscsrc > nul
del /s /q LICENSE > nul
del /s /q README > nul
del /s /q CHANGELOG > nul
del /s /q CNAME > nul

for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > nul

echo Compressing artifacts and cleans up...
echo %~dp07z.exe
"%~dp07z.exe" a -r -mx9 node_modules.7z node_modules > nul
rmdir /S /Q node_modules > nul


:done
echo Done
pushd "%~dp0"