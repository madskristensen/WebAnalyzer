@echo off

if exist %~dp0..\src\WebLinter\Node\node_modules.7z goto:done

pushd %~dp0..\src\WebLinter\Node

echo Installing packages...
call npm install babel-eslint --no-optional --quiet > nul
call npm install coffeelint --no-optional --quiet > nul
call npm install csslint --no-optional --quiet > nul
call npm install eslint --no-optional --quiet > nul
call npm install eslint-config-defaults --no-optional --quite > nul
call npm install eslint-plugin-filenames --no-optional --quiet > nul
call npm install eslint-plugin-react --no-optional --quiet > nul
call npm install tslint --no-optional --quiet > nul


echo Deleting unneeded files and folders...
del /s /q *.html > nul
del /s /q *.markdown > nul
del /s /q *.md > nul
del /s /q *.npmignore > nul
del /s /q *.patch > nul
del /s /q *.txt > nul
del /s /q *.yml > nul
del /s /q .coffeelintignore > nul
del /s /q .editorconfig > nul
del /s /q .eslintrc > nul
del /s /q .gitattributes > nul
del /s /q .gitmodules > nul
del /s /q .jscsrc > nul
del /s /q .jshintrc > nul
del /s /q .lint > nul
del /s /q .lintignore > nul
del /s /q cakefile > nul
del /s /q CHANGELOG > nul
del /s /q CHANGES > nul
del /s /q CNAME > nul
del /s /q coffeelint.json > nul
del /s /q example.js > nul
del /s /q generate-* > nul
del /s /q gruntfile.js > nul
del /s /q gulpfile.* > nul
del /s /q makefile.* > nul
del /s /q README > nul

for /d /r . %%d in (benchmark)  do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (bench)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (doc)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (docs)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (example)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (examples)   do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (images)     do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (man)        do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (media)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (scripts)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (test)       do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tests)      do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (testing)    do @if exist "%%d" rd /s /q "%%d" > nul
for /d /r . %%d in (tst)        do @if exist "%%d" rd /s /q "%%d" > nul

echo Compressing artifacts and cleans up...
%~dp07z.exe a -r -mx9 node_modules.7z node_modules > nul
rmdir /S /Q node_modules > nul


:done
echo Done
pushd "%~dp0"