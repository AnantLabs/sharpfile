if exist "%2tmp" goto tmp_exists
mkdir "%2tmp"

:tmp_exists
if not exist "%1Tools\ILMerge.exe" goto no_ilmerge_exists
"%1Tools\ILMerge.exe" /out:"%2tmp\SharpFile.exe" "%2SharpFile.exe" "%2Common.dll" "%2SharpFile.Infrastructure.dll" "%2SharpFile.IO.dll" "%2SharpFile.UI.dll"
del "%2Common.dll"
del "%2SharpFile.Infrastructure.dll" 
del "%2SharpFile.IO.dll"
del "%2SharpFile.UI.dll"
move /Y "%2tmp\SharpFile.exe" "%2SharpFile.exe"
copy /Y "%1SharpFile.Infrastructure\Resources\ilmerge_settings.config" "%2settings.config"
rmdir /S /Q "%2tmp"

if not exist "%1Tools\sgen.exe" goto display_sgen_message
"%1Tools\sgen.exe" /a:%2SharpFile.exe
goto compress_files

:no_ilmerge_exists
if not exist "%1Tools\sgen.exe" goto display_sgen_message
"%1Tools\sgen.exe" /a:%2SharpFile.Infrastructure.dll
goto compress_files

:display_sgen_message
echo It looks like you might need to set some PATH system enivronment variables. 
echo You can do that with the batch file: C:\Program Files\Microsoft.NET\SDK\v2.0 64bit\Bin\sdkvars.bat
goto compress_files

:compress_files
if not exist "%1Tools\7z.exe" goto end
"%1Tools\7z.exe" u -tzip -mx9 "%2SharpFile_bin.zip" "%2ChangeLog.txt" "%2SharpFile.exe" "%2gpl.txt" "%2Readme.txt" "%2*.xml" "%2thanks.txt"
"%1Tools\7z.exe" u -tzip -mx9 "%2SharpFile_src.zip" -r "%1*.*" -x!"%1bin\" -x!"%1obj\" -x!"%1.svn\" -x!"%1_svn\" -x!"%1*.user" -x!"%1*.NoLoad" -x!"%1*.suo" -x!"%1todo.*"
:end