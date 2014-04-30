
using System;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;		// für List_ToList sample

namespace TestSwiPl
{

    public class TreadWork
    {
        static public void CallPl()
        {
            String[] empty_param = { "-q" };  
            System.Diagnostics.Trace.WriteLine("MT1:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            PlEngine.Initialize(empty_param);
            PlTerm t = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
            PlEngine.PlCleanup();
        }
    }


	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI prolog interface in CSharp
    /// NOTE: all test methods must initialize PlEngine
	/// </summary>

    [TestClass()]
    public class T_PlEngine
	{

 
        /// <summary>
        /// Sample from the documentation PlFrame
        /// </summary>
        [TestMethod]
        #region demo_consult_pl_file_by_param
        public void Demo_consult_pl_file_by_param()
		{
            string[] ref_values = { "gloria", "melanie", "ayala" };
            Console.WriteLine("Demo_consult_pl_file_by_param");

            // Build a prolog source file (skip this step if you already have one :-)
            string filename = Path.GetTempFileName();
            StreamWriter sw = File.CreateText(filename);
            sw.WriteLine("father(martin, inka).");
            sw.WriteLine("father(uwe, gloria).");
            sw.WriteLine("father(uwe, melanie).");
            sw.WriteLine("father(uwe, ayala).");
            sw.Close();

            // build the parameterstring to Initialize PlEngine with the generated file
            String[] param = { "-q", "-f", filename };
            try
            {
                PlEngine.Initialize(param);
                Console.WriteLine("all child's from uwe:");
                using (PlQuery q = new PlQuery("father(uwe, Child)"))
                {
                    int idx = 0;
                    foreach (PlQueryVariables v in q.SolutionVariables)
                    {
                        Console.WriteLine(v["Child"].ToString());
                        Assert.AreEqual(ref_values[idx++], v["Child"].ToString());
                    }
                }
            }
            catch (PlException e)
            {
                Console.WriteLine(e.MessagePl);
                Console.WriteLine(e.Message);
            }
            finally
            {
                PlEngine.PlCleanup();
            }
        } // Demo_consult_pl_file_by_param
        #endregion demo_consult_pl_file_by_param

        static private String[] empty_param = { "-q" };  // suppressing informational and banner messages

	    [TestMethod]
        public void MT_demo_consult_pl_file_by_param()
	    {
            Demo_consult_pl_file_by_param();
	    }

	    [TestMethod]
        public void MT_0()
        {
            System.Diagnostics.Trace.WriteLine("MT0:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            PlEngine.Initialize(empty_param);
            PlTerm t = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
            PlEngine.PlCleanup();

            PlEngine.Initialize(empty_param);
            PlTerm t2 = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t2.PlType, "type enum");
            PlEngine.PlCleanup();

            PlEngine.Initialize(empty_param);
            PlTerm t3 = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
            PlEngine.PlCleanup();

            PlEngine.Initialize(empty_param);
            PlEngine.PlCleanup();
        }

        [TestMethod]
        public void MT_0_tread()
        {
            System.Threading.Thread myThread = new System.Threading.Thread(TreadWork.CallPl);
            System.Threading.Thread myThread2 = new System.Threading.Thread(TreadWork.CallPl);
            System.Threading.Thread myThread3 = new System.Threading.Thread(TreadWork.CallPl);
            myThread.Start();
            System.Diagnostics.Trace.WriteLine("eins");
            System.Threading.Thread.Sleep(100);
            myThread2.Start();
            System.Diagnostics.Trace.WriteLine("zwei");
            System.Threading.Thread.Sleep(100);
            myThread3.Start();
            System.Diagnostics.Trace.WriteLine("drei");
            System.Threading.Thread.Sleep(100);
        }

        [TestMethod]
        [TestCategory("mt")]
        public void MT_1()
        {
            System.Diagnostics.Trace.WriteLine("MT1:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            PlEngine.Initialize(empty_param);
            PlTerm t = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
            PlEngine.PlCleanup();
        }
        [TestMethod]
        [TestCategory("mt")]
        public void MT_2()
        {
            System.Diagnostics.Trace.WriteLine("MT2:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            PlEngine.Initialize(empty_param);
            PlTerm t = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
            PlEngine.PlCleanup();
        }
        [TestMethod]
        [TestCategory("mt")]
        public void MT_3()
        {
            System.Diagnostics.Trace.WriteLine("MT3:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
            PlEngine.Initialize(empty_param);
            PlTerm t = PlTerm.PlVar();
            Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
            PlEngine.PlCleanup();
        }

    } // test class T_PlEngine
}
