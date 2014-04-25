All test can be run by RunTests.bat
	
	mstest /testmetadata:..\swi-pl-cs.vsmdi  /noisolation

It is unclear why running all tests in visual studio nondetermistical fail
( VS running the tests in a own application domain )

After running RunTests.bat you'll find the results in e.g. TestResults\Lesta_LESTA2 2011-10-13 22_53_22.trx.
Drop the file into VS.



"Hacking MSTest out of Visual Studio" from http://blog.foxxtrot.net/2010/02/hacking-mstest-out-of-visual-studio.html
Open up Regedit on your development box, and point it to HKLM\SOFTWARE\Microsoft\VisualStudio\9.0\Licenses, 
and you’ll see a list of keys (probably only one, but it could be more than one). Export the keys you see 
under this hive, and then import them into your CI server, and it should automagically unlock the features 
you were missing on CI, check the /help output to be sure. Note: On a 64-bit system, 
the Hive is HKLM\Software\Wow6432Node\Microsoft\VisualStudio\9.0\Licenses. Just make sure you export and 
import to and from the correct location, if there is a difference in bit-width between your two boxes.

Now, you should have all the MSTest features you had on your development box on your CI box, and you 
didn’t have to do a full install of Visual Studio in CI.

