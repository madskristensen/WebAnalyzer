7z.exe x -y node.7z
7z.exe x -y node_modules.7z

del /q 7z.dll
del /q 7z.exe
del /q node.7z
del /q node_modules.7z
del /q prepare.cmd