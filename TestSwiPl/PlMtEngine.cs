
using System;
using SbsSW.SwiPlCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;		// für List_ToList sample

namespace TestSwiPl
{

	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI prolog interface in CSharp
	/// </summary>

    [TestClass()]
    public class t_PlMtEngine : BasePlInit
	{

        [TestMethod]
        public void Test_PlMtEngine_desroy()
        {
            PlMtEngine mple1 = new PlMtEngine();
            PlQuery q1 = null;
            mple1.PlSetEngine();
            q1 = new PlQuery("member(A, [a,b,c])");
            mple1.PlDetachEngine();
            q1.Dispose();
            mple1.Free();
        }

        //[TestMethod]
        //[Ignore]
        public void Test_PlMtEngine_desroy_exception()
        {
            PlMtEngine mple1 = new PlMtEngine();
            PlQuery q1 = null;
            mple1.PlSetEngine();
            q1 = new PlQuery("member(A, [a,b,c])");
            mple1.PlDetachEngine();
            mple1.Free();
            q1.Dispose();
        }

        [TestMethod]
        public void Test_Muliptle_engines()
		{

            // test case :

            // initialise
            // create thread and do 
            // pl_create_engine
            // loop
            // pl_set_engine
            // do somthing in prolog
            // pl_detache_engine

            // create 2. thread and do 
            // pl_create_engine
            // loop
            // pl_set_engine
            // do somthing in prolog
            // pl_detache_engine
            // pl_destroy_engine
            // create 3. thread and do 
            // pl_create_engine
            // pl_set_engine
            // do somthing in prolog
            // pl_detache_engine
            // pl_destroy_engine

            // the first Thread
            PlMtEngine mple1 = new PlMtEngine();
            PlQuery q1 = null;
            // the second Thread
            PlMtEngine mple2 = new PlMtEngine();
            PlQuery q2 = null;
            string[] testResult = {"a", "b", "c"};

            mple1.PlSetEngine();
            q1 = new PlQuery("member(A, [a,b,c])");
            Assert.IsTrue(q1.NextSolution());
            Assert.AreEqual(testResult[0], q1.Args[0].ToString());
            Assert.IsTrue(q1.NextSolution());
            Assert.AreEqual(testResult[1], q1.Args[0].ToString());
            mple1.PlDetachEngine();

            mple2.PlSetEngine();
            q2 = new PlQuery("member(A, [a,b,c])");
            Assert.IsTrue(q2.NextSolution());
            Assert.AreEqual(testResult[0], q2.Args[0].ToString());
            mple2.PlDetachEngine();

            mple1.PlSetEngine();
            Assert.IsTrue(q1.NextSolution());
            Assert.AreEqual(testResult[2], q1.Args[0].ToString());
            q1.Dispose();
            mple1.PlDetachEngine();

            mple2.PlSetEngine();
            Assert.IsTrue(q2.NextSolution());
            Assert.AreEqual(testResult[1], q2.Args[0].ToString());
            //q2.Free();
            mple2.PlDetachEngine();

         //   mple1.Free();
         //   mple2.Free();
        }


    } // class t_PlMtEngine 
} // namespace TestSwiPl
