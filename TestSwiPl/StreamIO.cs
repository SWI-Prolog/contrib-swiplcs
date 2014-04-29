/*********************************************************
* 
*  Author:        Uwe Lesta
*  Copyright (C): 2008-2014, Uwe Lesta SBS-Softwaresysteme GmbH
*
*  Unit-Tests for the interface from C# to Swi-Prolog - SwiPlCs
*
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 2.1 of the License, or (at your option) any later version.
*
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*  Lesser General Public License for more details.
*
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*
*********************************************************/

using System;
using SbsSW.SwiPlCs;
// using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs.Streams;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace TestSwiPl
{

	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI-Prolog interface in CSharp
	/// </summary>

    [TestClass]
    public class TestStreamIo : BasePlInit
    {

        #region StreamWrite_doc
        static string _testString;

        static long Swrite(IntPtr handle, string buffer, long buffersize)
        {
            string s = buffer.Substring(0, (int)buffersize);
            _testString = s;
            return buffersize;
        }

        [TestMethod]
        public void StreamWrite()
        {
            // NOTE: the Swrite function is only called if you flush the output or send a newline character
            const string validationString = "Hello .net world äöüß"; // The last 4 characters are German umlauts.
            PlQuery.PlCall("assert( (test_write :- writeln('" + validationString + "'), flush_output) )");
            var wf = new DelegateStreamWriteFunction(Swrite);
            PlEngine.SetStreamFunctionWrite(PlStreamType.Output, wf);
            PlQuery.PlCall("test_write");
            Assert.AreEqual(validationString+"\r\n", _testString);
        }
        #endregion StreamWrite_doc



        #region StreamRead_doc

	    private const string ValidationStringRead = "hello_dotnet_world_äöüß."; // The last 4 character are German umlauts.

	    static internal long Sread(IntPtr handle, IntPtr buffer, long buffersize)
        {
            const string s = ValidationStringRead + "\n";
            byte[] array = System.Text.Encoding.Unicode.GetBytes(s);
            System.Runtime.InteropServices.Marshal.Copy(array, 0, buffer, array.Length);
            return array.Length;
        }


        [TestMethod]
        public void StreamRead()
        {
            var rf = new DelegateStreamReadFunction(Sread);
            PlEngine.SetStreamFunctionRead(PlStreamType.Input, rf);
            // NOTE: read/1 needs a dot ('.') at the end
            PlQuery.PlCall("assert( (test_read(A) :- read(A)) )");
            PlTerm t = PlQuery.PlCallQuery("test_read(A)");
            Assert.AreEqual(ValidationStringRead, t.ToString()+".");
        }
        #endregion StreamRead_doc



    } // test class StreamIO
}
