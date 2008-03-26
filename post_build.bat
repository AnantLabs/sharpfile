@ECHO OFF
SET sgen_path=C:\Program Files\Microsoft SDKs\Windows\v6.0A\Bin\sgen.exe
SET ilmerge_path=c:\programming\ILMerge\ILMerge.exe

if exist "%2tmp" goto tmp_exists
ECHO Create the temporary directory
mkdir "%2tmp"

:tmp_exists
if not exist "%ilmerge_path%" goto no_ilmerge_exists
ECHO Merging assemblies...
"%ilmerge_path%" /out:"%2tmp\SharpFile.exe" "%2SharpFile.exe" "%2Common.dll"
ECHO Delete assemblies that were merged
del "%2Common.dll"
ECHO Move merged assembly from the temporary directory to the regular output directory
move /Y "%2tmp\SharpFile.exe" "%2SharpFile.exe"
ECHO Make sure our version of settings.config is valid so overwrite any version that was previously here
copy /Y "%1Infrastructure\Resources\ilmerge_settings.config" "%2settings.config"
ECHO Remove the temporary directory
rmdir /S /Q "%2tmp"
goto ilmerge_exists

:no_ilmerge_exists
echo ILMerge could not be found.

:ilmerge_exists
if not exist "%sgen_path%" goto display_sgen_message
ECHO Generating serializers...
"%sgen_path%" /a:%2SharpFile.exe /t:SharpFile.Infrastructure.Settings /force
goto compress_files

:display_sgen_message
echo Sgen could not be found.
goto compress_files

:compress_files
if not exist "%1Tools\7z.exe" goto end
ECHO Compressing bin...
"%1Tools\7z.exe" u -tzip -mx9 "%2SharpFile_bin.zip" "%2ChangeLog.txt" "%2SharpFile.exe" "%2gpl.txt" "%2Readme.txt" "%2*.xml" "%2thanks.txt" "%2ICSharpCode.SharpZipLib.dll" "%2SharpFile.XmlSerializers.dll"
ECHO Compressing source...
"%1Tools\7z.exe" u -tzip -mx9 "%2SharpFile_src.zip" -r "%1*.*" -x!"%1bin\" -x!"%1obj\" -x!"%1.svn\" -x!"%1_svn\" -x!"%1*.user" -x!"%1*.NoLoad" -x!"%1*.suo" -x!"%1todo.*" -x!"%1*.patch"
:end