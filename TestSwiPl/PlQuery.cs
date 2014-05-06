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

using SbsSW.SwiPlCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestSwiPl
{

	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI prolog interface in CSharp
	/// </summary>
	
	[TestClass]
	public class TestPlQuery : BasePlInit
	{

        private readonly string[] _abc = { "a", "b", "c" };

		#region Query
        [TestMethod]
        public void query_exception()
        {
            try
            {
                using (var q = new PlQuery("member(A,[a,b,c]"))
                {
                    Assert.IsTrue(q.NextSolution());
                    Assert.AreEqual("a", q.Args[0].ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        [TestMethod]
        [ExpectedException(typeof(SbsSW.SwiPlCs.Exceptions.PlException), "sunexcpected end .NET BUG: message not verified")]
        public void query_exception2()
        {
            using (var q = new PlQuery("member(A,[a,b,c]"))
            {
                Assert.IsTrue(q.NextSolution());
                Assert.AreEqual("a", q.Args[0].ToString());
            }
        }

        [TestMethod]
        public void query_1()
        {
            using (var q = new PlQuery("member", new PlTermV(new PlTerm("A"), new PlTerm("[a,b]"))))
            {
                Assert.IsTrue(q.NextSolution());
                Assert.AreEqual("a", q.Args[0].ToString());
            }
        }
        [TestMethod]
        public void query_2()
        {
            var q = new PlQuery("member", new PlTermV(new PlTerm("A"), new PlTerm("[a,b]")));
            {
                Assert.IsTrue(q.NextSolution());
                Assert.AreEqual("a", q.Args[0].ToString());
            }
        }
        [TestMethod]
        public void query_3()
        {
            var q = new PlQuery("member", new PlTermV(new PlTerm("A"), new PlTerm("[a,b]")));
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("a", q.Args[0].ToString());
        }


        [TestMethod]
		public void queryNextSolutionLoop_arg_term_codelist()
		{
			var av = new PlTermV(2);
            av[1] = PlTerm.PlCodeList("abc");
			var q = new PlQuery("member", av);
			int i = 0;
			while (q.NextSolution())
			{
				Assert.AreEqual(_abc[i++][0], (int)av[0]);
			}
		}


        [TestMethod]
		public void queryNextSolutionLoop_arg_term_listbuilded()
		{
			var av = new PlTermV(2);

            var l = PlTerm.PlTail(av[1]);
            var ta = new PlTerm("a");
			var tb = new PlTerm("b");
			var tc = new PlTerm("c");
			l.Append(ta);
			l.Append(tb);
			l.Append(tc);
			l.Close();

			var q = new PlQuery("member", av);
			int i = 0;
			while (q.NextSolution())
			{
				Assert.AreEqual(_abc[i++], av[0].ToString());
			}
		}

        [TestMethod]
		public void queryNextSolutionLoop_arg_term_StringList()
		{
			var av = new PlTermV(2);

			av[1] = new PlTerm("[a,b,c]");

			var q = new PlQuery("member", av);
			int i = 0;
			while (q.NextSolution())
			{
				Assert.AreEqual(_abc[i++], av[0].ToString());
			}
		}


        [TestMethod]
		public void queryNextSolutionLoop_1()
		{
            var tv = new PlTermV(new PlTerm("A"), new PlTerm("[a,b,c]"));
			var q = new PlQuery("member", tv);
			int i = 0;
			while (q.NextSolution())
			{
				Assert.AreEqual(_abc[i++], q.Args[0].ToString());
			}
		}


        [TestMethod]
		public void QueryNextSolutionLoop()
		{
			var q = new PlQuery("member", new PlTermV(new PlTerm("A"), new PlTerm("[a,b,c]")));
			int i = 0;
			while (q.NextSolution())
			{
				Assert.AreEqual(_abc[i++], q.Args[0].ToString());
			}
		}

        [TestMethod]
		public void QueryForeach()
		{
			var q = new PlQuery("member", new PlTermV(new PlTerm("A"), new PlTerm("[a,b,c]")));
			int i = 0;
			foreach (PlTermV s in q.Solutions)
			{
				Assert.AreEqual(_abc[i++], s[0].ToString());
			}
		}
		#endregion


		#region query build by String


        [TestMethod]
        public void anonymous_variable_1()
        {
            PlQuery.PlCall("assert(a(a, 1))");
            PlQuery.PlCall("assert(a(b, 2))");
            var q = new PlQuery("a(_, _)");
            Assert.IsTrue(q.NextSolution());
            Assert.IsTrue(q.NextSolution());
            Assert.IsFalse(q.NextSolution());
        }

        [TestMethod]
        public void anonymous_variable_2()
        {
            PlQuery.PlCall("assert(a(a, 1))");
            PlQuery.PlCall("assert(a(b, 2))");
            var q = new PlQuery("a(_, Y)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("a", q.Args[0].ToString());
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("b", q.Args[0].ToString());
            Assert.IsFalse(q.NextSolution());
        }

        [TestMethod]
        public void anonymous_variable_3()
        {
            PlQuery.PlCall("assert(a(a, 1, x))");
            PlQuery.PlCall("assert(a(b, 2, y))");
            var q = new PlQuery("a(_, _, Y)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("a", q.Args[0].ToString());
            Assert.AreEqual("1", q.Args[1].ToString());
            Assert.AreEqual("x", q.Args[2].ToString());
            Assert.AreEqual(1, q.VariableNames.Count);
            Assert.AreEqual("Y", q.VariableNames[0]);
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("b", q.Args[0].ToString());
            Assert.IsFalse(q.NextSolution());
        }



        [TestMethod]
        public void QueryString()
        {
            var q = new PlQuery("member(A, [a,b])");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("a", q.Args[0].ToString());
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("b", q.Args[0].ToString());
            Assert.IsFalse(q.NextSolution());
        }

        [TestMethod]
        public void QueryStringOp()
        {
            PlQuery.PlCall("op(700, xfy, ~)");
            var q = new PlQuery("T = (a ~ b)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("a~b", q.Args[0].ToString());
        }


        [TestMethod]
        #region queryString2_doc
        public void QueryString2()
        {
            var q = new PlQuery("append(A, B, [a,b,c])");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("[]", q.Args[0].ToString());
            Assert.AreEqual("[a,b,c]", q.Args[1].ToString());
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("[a]", q.Args[0].ToString());
            Assert.AreEqual("[b,c]", q.Args[1].ToString());
        }
        #endregion queryString2_doc

        [TestMethod]
        #region queryStringNamed_doc
        public void QueryStringNamed()
        {
            var q = new PlQuery("append(A, B, [a,b,c])");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("[]", q.Variables["A"].ToString());
            Assert.AreEqual("[a,b,c]", q.Variables["B"].ToString());
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual("[a]", q.Variables["A"].ToString());
            Assert.AreEqual("[b,c]", q.Variables["B"].ToString());
        }
        #endregion queryStringNamed_doc

        [TestMethod]
        #region queryStringForeach_doc
        public void QueryStringForeach()
		{
            string[] mm = { "aa", "bb", "cc" };
            var q = new PlQuery("member(A, [aa, bb, cc])");
			int i = 0;
			foreach (PlTermV s in q.Solutions)
			{
				Assert.AreEqual(mm[i++], s[0].ToString());
			}
            // or with named variables
            i = 0;
            foreach (PlQueryVariables s in q.SolutionVariables)
            {
                Assert.AreEqual(mm[i++], s["A"].ToString());
            }
        }
        #endregion queryStringForeach_doc

        #endregion


        #region PlCall

        [TestMethod]
        public void PlCallNoVariableTrue()
        {
            bool result = PlQuery.PlCall("is_list([a,b,c])");
            Assert.IsTrue(result);
        }
        [TestMethod]
        public void PlCallNoVariableFail()
        {
            bool result = PlQuery.PlCall("is_list(123)");
            Assert.IsFalse(result);
        }

        #endregion


        #region PlCallQuery
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PlCallQueryNoVariableException()
        {
            PlQuery.PlCallQuery("member(a, [a,b,c])");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void PlCallQueryTooMannyVariableException()
        {
            PlQuery.PlCallQuery("member(A, X)");
        }


        [TestMethod]
        public void PlCallQuery0()
        {
            var t = PlQuery.PlCallQuery("member(A, [a,b,X])");
            Assert.IsTrue(t.IsAtom);
            Assert.AreEqual("a", t.ToString());
        }
        
        [TestMethod]
        public void PlCallQuery1()
        {
            #region PlCallQuery_direct_1_doc
            PlTerm t = PlQuery.PlCallQuery("A = [a,b,c]");
            Assert.IsTrue(t.IsList);
            #endregion PlCallQuery_direct_1_doc
            int i = 0;
            foreach (PlTerm s in t)
            {
                Assert.AreEqual(_abc[i++], s.ToString());
            }
        }
        [TestMethod]
        public void PlCallQuery2()
        {
            #region PlCallQuery_direct_2_doc
            PlTerm t = PlQuery.PlCallQuery("atom_concat(a, b, X)");
            Assert.AreEqual("ab", t.ToString());
            #endregion PlCallQuery_direct_2_doc
        }
        [TestMethod]
        public void PlCallQuery3()
        {
            #region PlCallQuery_direct_3_doc
            PlTerm t = PlQuery.PlCallQuery("working_directory(O, O)");
            Assert.IsTrue(t.ToString().EndsWith("contrib-swiplcs/testswipl/bin/debug/"), "Path is not correct:" + t.ToString());
            #endregion PlCallQuery_direct_3_doc
        }
        #endregion



        #region compound queries

        private void build_pred()
        {
            Assert.IsTrue(PlQuery.PlCall("assert( (test(comp(X,Y)) :- member(Z,[1,2,3]), atomic_list_concat([X,Z],Y) ) )"));
        }

        [TestMethod]
        #region PlCallQueryCompound_string_doc
        public void PlCallQueryCompound_string()
        {
            string[] mm = { "comp(aa,aa1)", "comp(aa,aa2)", "comp(aa,aa3)" };
            build_pred();   // create: test(comp(X,Y)) :- member(Z,[1,2,3]), atomic_list_concat([X,Z],Y).
            var q = new PlQuery("test(comp(aa,X))");
            int i = 0;
            foreach (PlTermV s in q.Solutions)
            {
                Assert.AreEqual(mm[i++], s[0].ToString());
            }
        }
        #endregion PlCallQueryCompound_string_doc

        [TestMethod]
        #region PlCallQueryCompoundNamed_string_doc
        public void PlCallQueryCompoundNamed_string()
        {
            string[] mm = { "aa1", "aa2", "aa3" };
            build_pred();   // create: test(comp(X,Y)) :- member(Z,[1,2,3]), atomic_list_concat([X,Z],Y).
            var q = new PlQuery("test(comp(aa,X))");
            int i = 0;
            foreach (PlQueryVariables v in q.SolutionVariables)
            {
                Assert.AreEqual(mm[i++], v["X"].ToString());
            }
        }
        #endregion PlCallQueryCompoundNamed_string_doc

        [TestMethod]
        #region PlCallQueryCompound_termv_doc
        public void PlCallQueryCompound_termv()
        {
            string[] mm = { "aa1", "aa2", "aa3" };
            build_pred();   // create: test(comp(X,Y)) :- member(Z,[1,2,3]), atomic_list_concat([X,Z],Y).
            PlTerm var1 = PlTerm.PlVar();
            PlTerm comp = PlTerm.PlCompound("comp", new PlTerm("aa"), var1);
            using (var q = new PlQuery("test", new PlTermV(comp)))
            {
                int i = 0;
                foreach (PlTermV s in q.Solutions)
                {
                    Assert.AreEqual(mm[i++], var1.ToString());
                    Assert.AreEqual(comp.ToString(), s[0].ToString());
                }
            }
        }
        #endregion PlCallQueryCompound_termv_doc


        #endregion


        #region PL_query

        [TestMethod]
        #region get_prolog_version_number_doc
        public void Pl_query_version()
        {
            long v = PlQuery.Query(PlQuerySwitch.Version);
            Assert.AreEqual(60605, v, "SWI-Prolog version number ");
        }
        #endregion get_prolog_version_number_doc

        #endregion PL_query


        [TestMethod]
        public void PlQuery_2() // von robert
        {
            #region explicit_query_dispose_doc
            const string strRef = "a;e;";
            PlQuery.PlCall("assert(n('" + strRef + "'))");
            var q = new PlQuery("n(X)");
            Assert.IsTrue(q.NextSolution());
            Assert.AreEqual(strRef, q.Variables["X"].ToString());
            var q2 = new PlQuery("n('" + strRef + "')");
            Assert.IsTrue(q2.NextSolution());
            Assert.AreEqual(strRef, q.Variables["X"].ToString());
            q2.Dispose();
            q.Dispose();
            #endregion explicit_query_dispose_doc
        }


        [TestMethod]
        public void T1()
        {
            //<COMPOUND> sqrt(8,Var1)
            var p1V = new PlTermV(2);
            PlTerm var1 = PlTerm.PlVar();
            p1V[0] = new PlTerm(8);
            p1V[1] = var1;
            PlTerm p1 = PlTerm.PlCompound("sqrt", p1V);

            //<COMPOUND> floor(Var1, Var2) 
            var p2V = new PlTermV(2);
            var var2 = PlTerm.PlVar();
            p2V[0] = var1;
            p2V[1] = var2;
            var p2 = PlTerm.PlCompound("floor", p2V);

            //<GROUPING> p3 = (sqrt(8, Var1), floor(Va1, Var2))
            var p3 = PlTerm.PlCompound(",", p1, p2);
            var pPred = new PlTermV(1);
            pPred[0] = p3;


            System.Diagnostics.Trace.WriteLine(p3.ToStringCanonical()); 



            //Prolog Equivalent woudl be:
            //P3 = (sqrt(8, Var1), floor(Var1, Var2)), call(P3).
            using (var q = new PlQuery("call", pPred))
            {
                while (q.NextSolution())
                {
                    //Process output of variables here
                    Console.WriteLine(var2.ToString()); //Var2 should print out 2
                    System.Diagnostics.Trace.WriteLine(var2.ToString()); //Var2 should print out 2
                }
            }
        } // t1


    }
}
