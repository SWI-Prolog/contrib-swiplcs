
using System;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs.Streams;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;		// für List_ToList sample


namespace TestSwiPl
{

	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI-Prolog interface in CSharp
	/// </summary>

    [TestClass()]
    public class T_StreamIO : BasePlInit
    {

        #region StreamWrite_doc
        static string test_string;

        static long Swrite(IntPtr handle, string buffer, long buffersize)
        {
            string s = buffer.Substring(0, (int)buffersize);
            test_string = s;
            return buffersize;
        }

        [TestMethod]
        public void StreamWrite()
        {
            // NOTE: the Swrite function is only called if you flush the output or send a newline character
            string ref_string = "Hello .net world äöüß";        // The last 4 characters are German umlauts.
            PlQuery.PlCall("assert( (test_write :- writeln('" + ref_string + "'), flush_output) )");
            DelegateStreamWriteFunction wf = new DelegateStreamWriteFunction(Swrite);
            PlEngine.SetStreamFunctionWrite(PlStreamType.Output, wf);
            PlQuery.PlCall("test_write");
            Assert.AreEqual(ref_string+"\r\n", test_string);
        }
        #endregion StreamWrite_doc



        #region StreamRead_doc
        static string ref_string_read = "hello_dotnet_world_äöüß.";     // The last 4 character are German umlauts.

        static internal long Sread(IntPtr handle, System.IntPtr buffer, long buffersize)
        {
            string s = ref_string_read + "\n";
            byte[] array = System.Text.Encoding.Unicode.GetBytes(s);
            System.Runtime.InteropServices.Marshal.Copy(array, 0, buffer, array.Length);
            return array.Length;
        }


        [TestMethod]
        public void StreamRead()
        {
            DelegateStreamReadFunction rf = new DelegateStreamReadFunction(Sread);
            PlEngine.SetStreamFunctionRead(PlStreamType.Input, rf);
            // NOTE: read/1 needs a dot ('.') at the end
            PlQuery.PlCall("assert( (test_read(A) :- read(A)) )");
            PlTerm t = PlQuery.PlCallQuery("test_read(A)");
            Assert.AreEqual(ref_string_read, t.ToString()+".");
        }
        #endregion StreamRead_doc



    } // test class StreamIO
}
