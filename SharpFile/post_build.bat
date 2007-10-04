if exist "%2tmp" goto tmp_exists
mkdir "%2tmp"

:tmp_exists
if not exist "C:\Programming\ILMerge\ILMerge.exe" goto no_ilmerge_exists
"C:\Programming\ILMerge\ILMerge.exe" /out:"%2tmp\SharpFile.exe" "%2SharpFile.exe" "%2Common.dll" "%2SharpFile.Infrastructure.dll" "%2SharpFile.IO.dll"
del "%2Common.dll"
del "%2SharpFile.Infrastructure.dll" 
del "%2SharpFile.IO.dll"
move /Y "%2tmp\SharpFile.exe" "%2SharpFile.exe"

:no_ilmerge_exists
rmdir /S /Q "%2tmp"

if not exist "c:\utility\7-zip\7z.exe" goto end
"c:\utility\7-zip\7z.exe" u -tzip -mx9 "%2SharpFile_bin.zip" "%2ChangeLog.txt" "%2SharpFile.exe" "%2gpl.txt" "%2Readme.txt" "%2*.xml"
"c:\utility\7-zip\7z.exe" u -tzip -mx9 "%2SharpFile_src.zip" -r "%1*.*" -x!"%1bin\" -x!"%1obj\" -x!"%1.svn\" -x!"%1_svn\" -x!"%1*.user" -x!"%1*.NoLoad" -x!"%1*.suo"
:end
