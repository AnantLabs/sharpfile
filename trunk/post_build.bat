@ECHO OFF

if not exist "%1Tools\7z.exe" goto end
ECHO Compressing bin...
"%1Tools\7z.exe" u -tzip -mx9 "%2SharpFile_bin.zip" "%2ChangeLog.txt" "%2SharpFile.exe" "%2gpl.txt" "%2Readme.txt" "%2thanks.txt" "%2ICSharpCode.SharpZipLib.dll" "%2Common.dll" "%2WeifenLuo.WinFormsUI.Docking.dll" "%2SharpFile.Infrastructure.dll" "%2Plugins\"
ECHO Compressing source...
"%1Tools\7z.exe" u -tzip -mx9 "%2SharpFile_src.zip" -r "%1*.*" -x!"%1bin\" -x!"%1obj\" -x!"%1.svn\" -x!"%1_svn\" -x!"%1*.user" -x!"%1*.NoLoad" -x!"%1*.suo" -x!"%1todo.*" -x!"%1*.patch"
:end