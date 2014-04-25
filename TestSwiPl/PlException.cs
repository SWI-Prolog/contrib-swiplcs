
using System;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;		// für List_ToList sample

namespace TestSwiPl
{

	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI prolog interface in CSharp
	/// </summary>

    [TestClass()]
    public class T_PlException : BasePlInit
	{
        /// <summary>
        /// Sample from the documentation PlFrame
        /// </summary>
        [TestMethod]
        public void komplexPlException()
		{
            int size = 4;
            int index = 6;
            PlTerm term = PlTerm.PlCompound("error",
                            new PlTermV(PlTerm.PlCompound("domain_error",
                                                    new PlTermV(PlTerm.PlCompound("argv", new PlTermV(size)), new PlTerm(index))
			                                ),
	        		        PlTerm.PlVar())
			                );
            PlException ex = new PlException(term);
            //Assert.AreEqual("Domain error: `argv(_G1, _G2, _G3, _G4)' expected, found `6'", ex.Message);
            // since swi-prolog version 5.7.6
            // Assert.AreEqual("Domain error: `argv(_G60, _G61, _G62, _G63)' expected, found `6'", ex.Message);
            // since swi-prolog version 5.7.10
            // Assert.AreEqual("Domain error: `argv(_G1, _G2, _G3, _G4)' expected, found `6'", ex.Message);
            // since swi-prolog version 5.9.10
           
#if _PL_X64
            Assert.AreEqual("Domain error: `argv(_G53,_G54,_G55,_G56)' expected, found `6'", ex.Message);
#else
            Assert.AreEqual("Domain error: `argv(_G1,_G2,_G3,_G4)' expected, found `6'", ex.Message);
#endif

        }
        
        
        #region prolog throw c# catch Samples _doc

        [TestMethod]
        #region prolog_exception_sample_doc
        public void prolog_exception_sample()
        {
            string exception_text = "test_exception";
            Assert.IsTrue(PlQuery.PlCall("assert( (test_throw :- throw(" + exception_text + ")) )"));
            try
            {
                Assert.IsTrue(PlQuery.PlCall("test_throw"));
            }
            catch (PlException ex)
            {
                Assert.AreEqual(exception_text, ex.Term.ToString());
                Assert.AreEqual("Unknown message: " + exception_text, ex.Message);
            }
        }
        #endregion prolog_exception_sample_doc

        [TestMethod]
        #region prolog_type_exception_sample_doc
        public void prolog_type_exception_sample()
        {
            try
            {
                Assert.IsTrue(PlQuery.PlCall("sumlist([1,error],L)"));
            }
            catch (PlTypeException ex)
            {
                Assert.AreEqual("is/2: Arithmetic: `error/0' is not a function", ex.Message);
            }
        }
        #endregion prolog_type_exception_sample_doc

        [TestMethod]
        #region prolog_domain_exception_sample_doc
        public void prolog_domain_exception_sample()
        {
            try
            {
                Assert.IsTrue(PlQuery.PlCall("open(temp_kill, nonsens, F)"));
            }
            catch (PlDomainException ex)
            {
                Assert.AreEqual("open/3: Domain error: `io_mode' expected, found `nonsens'", ex.Message);
            }
        }
        #endregion prolog_domain_exception_sample_doc


        #endregion prolog throw c# catch


        //[TestMethod]
        //[Ignore]
        public void t_prolog_init_exception()
        {
            PlEngine.PlCleanup();
            Assert.IsFalse(PlEngine.IsInitialized);
            // this throw a PlLibException
            String[] param = { "-q", "-g", "member(A,[a," };  // -q suppressing informational and banner messages
            // TODO i'll need something that throw a PlException on initialitzation
            try
            {
                PlEngine.Initialize(param);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
            Assert.IsTrue(PlEngine.IsInitialized);
        }



        #region prolog throw c# catch

        [TestMethod]
        [ExpectedException(typeof(PlException), "Unknown message: error")]
        public void t_prolog_exception()
        {
            Assert.IsTrue(PlQuery.PlCall("assert( (test_throw :- throw(error)) )"));
            PlQuery.PlCall("test_throw");
        }

        [TestMethod]
        [ExpectedException(typeof(PlTypeException), "is/2: Arithmetic: `error/0' is not a function")]
        public void t_prolog_exception_2()
        {
            PlQuery.PlCall("sumlist([1,error],L)");
        }

        [TestMethod]
        [ExpectedException(typeof(PlDomainException), "open/3: Domain error: `io_mode' expected, found `nonsens'")]
        public void t_prolog_exception_3()
        {
            PlQuery.PlCall("open(temp_kill, nonsens, F)");
        }


        [TestMethod]
        [ExpectedException(typeof(PlTypeException), "`list' expected, found `[a,b,c]'")]
        public void TestException_in_a_query_2()
        {
            PlQuery plq = new PlQuery("atomic_list_concat(L, A)");
            Assert.IsTrue(plq.Variables["L"].Unify("[a,b,c]"));
            //Assert.IsTrue(plq.Variables["L"].Unify(new PlTerm("[a,b,c]")));
            foreach (PlQueryVariables vars in plq.SolutionVariables)
            {
                Assert.AreEqual("abc", vars["A1"].ToString());
            }
        }


        #endregion prolog throw c# catch


    } // test class T_PlException
}
