rem winrar must be installed at c:\programme\winrar\winrar
rem Sandcastle Help File Builder (shfb) must be installed
rem 
rem rar sample at http://daily-it.blogspot.com/2007/11/rar-command-line-with-real-world.html

setlocal 
	call "%VS110COMNTOOLS%vsvars32.bat"
	devenv ..\SWIplcs_git.sln /Build Release /Project SwiPlCs
	devenv ..\SWIplcs_git.sln /Build Release64 /Project SwiPlCs
endlocal

set DEST=Download\SwiPlCs_1.1.60605.0.zip
set SOURCEPATH=..\SwiPlCs\bin\Release
set SOURCEPATH64=..\SwiPlCs\bin\Release64


%windir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe  ..\SwiPlCs\help\SwiCsPl.shfbproj

attrib %DEST%
del %DEST%
copy %SOURCEPATH64%\SwiPlCs.dll %SOURCEPATH%\SwiPlCs64.dll
c:\programme\winrar\winrar a %DEST% -m5 -ep -afzip ..\SwiPlCs\ChangeLog.txt %SOURCEPATH%\SwiPlCs.dll %SOURCEPATH%\SwiPlCs64.dll %SOURCEPATH%\SwiPlCs.xml Generated\SwiCsPlDocumentation.chm

pause

