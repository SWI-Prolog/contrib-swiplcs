echo off

call "%ProgramFiles%\Microsoft Visual Studio 10.0\Common7\Tools\vsvars32"
echo
echo to test a special TestCategory use switch  /category:"mt"  
echo see "MSTest.exe Command-Line Options" at 
echo http://msdn.microsoft.com/de-de/library/ms182489.aspx#testsettings
echo for more options
echo

rem mstest /testcontainer:bin\Debug\TestSwiPl.dll rem /resultsfile:RunTestResult.trx /category:"mt"   /noisolation
rem mstest /testcontainer:bin\Debug\TestSwiPl.dll   /testmetadata:..\swi-pl-cs.vsmdi    /noisolation

rem quite_version 
rem mstest /testmetadata:..\swi-pl-cs.vsmdi  /noisolation /usestderr >nul

mstest /testmetadata:..\swi-pl-cs.vsmdi  /noisolation 

pause