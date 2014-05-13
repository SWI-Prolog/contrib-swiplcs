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
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestSwiPl
{
    [TestClass]
    public class LinqSwiPl : BasePlInit
    {

        private readonly string[] _abc = { "a", "b", "c" };

        [TestMethod]
        #region Test_multi_goal_ToList_doc
        public void Test_multi_goal_ToList()
        {
            var results = from n in new PlQuery("L=[a,b,c], member(A, L)").ToList() 
                          select new { A = (string)n["A"] };
            int i = 0;
            foreach (var t in results)
                Assert.AreEqual(_abc[i++], t.A);
        }
        #endregion Test_multi_goal_ToList_doc

        [TestMethod]
        #region compound_query_SolutionVariables_doc
        public void TestCompoundQuery()
        {
            string[] refValues = { "gloria", "melanie", "ayala" };
            using (new PlFrame())
            {
                PlQuery.PlCall("assert(father(uwe, gloria))");
                PlQuery.PlCall("assert(father(uwe, melanie))");
                PlQuery.PlCall("assert(father(uwe, ayala))");
                var plq = new PlQuery("father(P,C), atomic_list_concat([P,' is_father_of ',C], L)");
                int i = 0;
                foreach (PlQueryVariables vars in plq.SolutionVariables)
                {
                    Assert.AreEqual("uwe", (string)vars["P"]);
                    Assert.AreEqual(refValues[i++], (string)vars["C"]);
                }
            }
        }

        #endregion compound_query_SolutionVariables_doc

        [TestMethod]
        #region compound_query_ToList_doc
        public void TestCompoundQueryToList()
        {
            string[] refValues = { "gloria", "melanie", "ayala" };
            using (new PlFrame())
            {
                PlQuery.PlCall("assert(father(uwe, gloria))");
                PlQuery.PlCall("assert(father(uwe, melanie))");
                PlQuery.PlCall("assert(father(uwe, ayala))");
                var plq = new PlQuery("father(P,C), atomic_list_concat([P,' is_father_of ',C], L)").ToList();
                int i = 0;
                foreach (PlQueryVariables vars in plq)
                {
                    Assert.AreEqual("uwe", (string)vars["P"]);
                    Assert.AreEqual(refValues[i++], (string)vars["C"]);
                }
            }
        }

        #endregion compound_query_ToList_doc

        [TestMethod]
        #region compound_query_with_variables_doc
        public void TestCompoundQueryWithVariables()
        {
            string[] refValues = { "x_a", "x_b", "x_c" };
            using (new PlFrame())
            {
                var plq = new PlQuery("member(X, [a,b,c]), atomic_list_concat([P, X], L)");
                plq.Variables["P"].Unify("x_");
                int i = 0;
                foreach (PlQueryVariables vars in plq.SolutionVariables)
                {
                    Assert.AreEqual("x_", (string)vars["P"]);
                    Assert.AreEqual(refValues[i++], (string)vars["L"]);
                }
            }
        }

        #endregion compound_query_with_variables_doc

        [TestMethod]
        #region compound_nested_query_with_variables_doc
        public void TestCompoundNestedQueryWithVariables()
        {
            string[] refValues = { "x_a", "x_b", "x_c" };
            string[] refValuesInner = { "a1", "a2", "b1", "b2", "c1", "c2" };
            int innerIdx = 0;
            using (new PlFrame())
            {
                var plq = new PlQuery("member(X, [a,b,c]), atomic_list_concat([P, X], L)");
                plq.Variables["P"].Unify("x_");
                int i = 0;
                foreach (PlQueryVariables vars in plq.SolutionVariables)
                {
                    Assert.AreEqual("x_", (string)vars["P"]);
                    Assert.AreEqual(refValues[i++], (string)vars["L"]);
                    var q = new PlQuery("member(X, [1,2]), atomic_list_concat([P, X], L)");
                    q.Variables["P"].Unify(plq.Variables["X"]);
                    var results = from n in q.ToList() select new { L = n["L"].ToString() };
                    foreach (var v in results)
                    {
                        Assert.AreEqual(refValuesInner[innerIdx++], v.L);
                    }
                }
            }
        }
        #endregion compound_nested_query_with_variables_doc

        [TestMethod]
        #region compound_nested_query_with_variables_2_doc
        public void TestCompoundNestedQueryWithVariables2()
        {
            string[] refValuesInner = { "_gap_abc1", "_gap_abc2", "a_gap_bc1", "a_gap_bc2", "ab_gap_c1", "ab_gap_c2", "abc_gap_1", "abc_gap_2" };
            int innerIdx = 0;
            using (new PlFrame())
            {
                try
                {
                    var outerQuery = new PlQuery("append(A,B,C), atomic_list_concat(A, A1), atomic_list_concat(B, B1)");
                    outerQuery.Variables["C"].Unify(new PlTerm("[a,b,c]"));
                    var innerQuery = new PlQuery("member(Count, [1,2]), atomic_list_concat([X, Y, Z, Count], ATOM)");
                    innerQuery.Variables["X"].Unify(outerQuery.Variables["A1"]);
                    innerQuery.Variables["Y"].Unify("_gap_");
                    innerQuery.Variables["Z"].Unify(outerQuery.Variables["B1"]);
                    foreach (PlQueryVariables vars in outerQuery.SolutionVariables)
                    {
                        foreach (PlQueryVariables varsInner in innerQuery.SolutionVariables)
                        {
                            Assert.AreEqual(refValuesInner[innerIdx++], varsInner["ATOM"].ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    Assert.IsFalse(true);
                }
            }
        }
        #endregion compound_nested_query_with_variables_2_doc

        [TestMethod]
        #region compound_nested_query_with_variables_3_doc
        public void TestCompoundNestedQueryWithVariables3()
        {
            string[] refValuesInner = { "_gap_abc1", "_gap_abc2", "a_gap_bc1", "a_gap_bc2", "ab_gap_c1", "ab_gap_c2", "abc_gap_1", "abc_gap_2" };
            int innerIdx = 0;
            using (new PlFrame())
            {
                try
                {
                    var outerQuery = new PlQuery("append(A,B,C), atomic_list_concat(A, A1), atomic_list_concat(B, B1)");
                    outerQuery.Variables["C"].Unify(new PlTerm("[a,b,c]"));
                    var innerQuery = new PlQuery("member(Count, [1,2]), atomic_list_concat([X, Y, Z, Count], ATOM)");
                    innerQuery.Variables["X"].Unify(outerQuery.Variables["A1"]);
                    innerQuery.Variables["Y"].Unify("_gap_");
                    innerQuery.Variables["Z"].Unify(outerQuery.Variables["B1"]);
                    foreach (PlQueryVariables vars in outerQuery.SolutionVariables)
                    {
                        var results = from n in innerQuery.ToList() 
                            select new { L = n["ATOM"].ToString(), X = n["X"].ToString(), Y = n["Z"].ToString() };
                        foreach (var v in results)
                        {
                            Assert.AreEqual(refValuesInner[innerIdx++], v.L);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    Assert.IsFalse(true);
                }
            }
        }
        #endregion compound_nested_query_with_variables_3_doc


        [TestMethod]
        public void TestCompoundWithVariables2()
        {
            string[] refValuesInner = { "_gap_abc", "a_gap_bc", "ab_gap_c", "abc_gap_"};
            int innerIdx = 0;
            using (new PlFrame())
            {
                try
                {
                    var plq = new PlQuery("YY=[a,b,c], append(A,B,C), atomic_list_concat(A, A1), atomic_list_concat(B, B1)");
                    //Assert.IsTrue(plq.Variables["C"].Unify("[a,b,c]"));
                    Assert.IsTrue(plq.Variables["C"].Unify(new PlTerm("[a,b,c]")));
                    // plq.Free();
                    foreach (PlQueryVariables vars in plq.SolutionVariables)
                    {
                        Assert.AreEqual(refValuesInner[innerIdx++], vars["A1"].ToString() + "_gap_" + vars["B1"].ToString());
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Trace.WriteLine(ex.Message);
                    Assert.IsFalse(true);
                }
            }
        }
        
        

        [TestMethod]
        public void Query_test_arity0()
        {
            using (new PlFrame())
            {
                var result = new PlQuery("listing").ToList();
                Assert.IsTrue(result.Count == 0);
            }
        }


        [TestMethod]
        public void TestCompoundQuery_test()
        {
            using (new PlFrame())
            {
                var results = from n in new PlQuery("P=uwe").ToList() select n;
                foreach (PlQueryVariables vars in results)
                {
                    Assert.AreEqual("uwe", (string)vars["P"]);
                }
            }
        }
        [TestMethod]
        public void TestCompoundQuery_test_2()
        {
            using (new PlFrame())
            {
                var results = from n in new PlQuery("P=uwe, member(A,[a,b])").ToList() select n;
                foreach (PlQueryVariables vars in results)
                {
                    Assert.AreEqual("uwe", (string)vars["P"]);
                }
            }
        }
        [TestMethod]
        public void TestCompoundQuery_test_3()
        {
            using (new PlFrame())
            {
                var results = from n in new PlQuery("P=uwe, C=test, atomic_list_concat([P,' is_father_of ',C], L)").ToList() select n;
                foreach (PlQueryVariables vars in results)
                {
                    Assert.AreEqual("uwe", (string)vars["P"]);
                    Assert.AreEqual("test", (string)vars["C"]);
                    Assert.AreEqual("uwe is_father_of test", (string)vars["L"]);
                }
            }

        }
        [TestMethod]
        public void TestCompoundQuery_test_4()
        {
            string[] refValues = { "gloria", "melanie" };
            using (new PlFrame())
            {
                PlQuery.PlCall("assert(father(uwe, gloria))");
                PlQuery.PlCall("assert(father(uwe, melanie))");
                PlQuery.PlCall("assert(father(jan, swi_prolog))");
                var results = from n in new PlQuery("P=uwe, father(P, C), atomic_list_concat([P,' is_father_of ',C], L)").ToList() select n;
                int i = 0;
                foreach (PlQueryVariables vars in results)
                {
                    Assert.AreEqual("uwe", (string)vars["P"]);
                    Assert.AreEqual(refValues[i], (string)vars["C"]);
                    Assert.AreEqual("uwe is_father_of " + refValues[i], (string)vars["L"]);
                    i++;
                }
                Assert.AreEqual(2, i);

            }
        }

        // TODO wieder gäöngig machen
        // see http://msdn.microsoft.com/en-us/library/bb546207.aspx
        // and  http://codethings.blogspot.de/2012/10/testing-private-members-with.html
        //[TestMethod]
        //public void TestMethod0()
        //{
        //    uintptr_t r;
        //    PlTerm t = new PlTerm("test_atom");
        //    r = LibPl.PL_record(t.TermRef);
        //    PlTerm tx = PlTerm.PlVar();
        //    LibPl.PL_recorded(r, tx.TermRef);
        //    Assert.AreEqual("test_atom", tx.ToString());
        //    LibPl.PL_erase(r);
        //}


        [TestMethod]
        public void TestMethod_simple_x()
        {
            var results = from n in new PlQuery("member(A, [a,b,c])").ToList() select new { A = n["A"].ToString() };
            // check
            int i = 0;
            foreach (var s in results)
                Assert.AreEqual(_abc[i++], s.A);
        }

        [TestMethod]
        public void TestMethod_simple()
        {
            var results = from n in new PlQuery("member(A, [a,b,c])").ToList() select n;
            // check
            int i = 0;
            foreach (var t in results)
                Assert.AreEqual(_abc[i++], t["A"].ToString());
        }
        [TestMethod]
        public void TestMethod_simple2()
        {
            var results = from n in new PlQuery("member(A, [a,b,c])").ToList() select n;
            // check
            int i = 0;
            foreach (var t in results)
                Assert.AreEqual(_abc[i++], t["A"].ToString());
        }

        [TestMethod]
        public void TestMethod_simple_inline()
        {
            var results = from n in new PlQuery("member(A, [a,b,c])").ToList() select n;
            // check
            int i = 0;
            foreach (PlQueryVariables t in results)
                Assert.AreEqual(_abc[i++], t["A"].ToString());
        }


        [TestMethod]
        public void Test_multi_goal_2()
        {
            var results = from n in new PlQuery("L=[a,b,c], member(A, L)").ToList() select n;
            // check
            int i = 0;
            foreach (PlQueryVariables t in results)
            {
                Assert.AreEqual(_abc[i++], t["A"].ToString());
                Assert.IsTrue(t["L"].IsList);
                Assert.AreEqual("[a,b,c]", t["L"].ToString());
            }
        }


        [TestMethod]
        public void TestMethod_2()
        {
            var q = new PlQuery("member(A, [a,b,c])");
            var results = from n in q.ToList() select n["A"].ToString();
            // check
            int i = 0;
            foreach (string t in results)
            {
                Assert.AreEqual(_abc[i++], t);
            }
        }

        [TestMethod]
        public void TestMethod_simple_annonymusType_1()
        {
            var results = from n in new PlQuery("member(A, [a,b,c])").ToList() select new { upper = n["A"].ToString().ToUpper(), lower = n["A"].ToString().ToLower() };
            // check
            int i = 0;
            foreach (var t in results)
                Assert.AreEqual(_abc[i++], t.lower);
            i = 0;
            foreach (var t in results)
                Assert.AreEqual(_abc[i++].ToUpper(), t.upper);
        }


        /*
        [TestMethod]
        [Ignore]
        // onli to test some things 
        public void TestMethod_ohne_linq()
        {
            string mm = "abc";
            PlQueryQ q = new PlQueryQ("findall(x(A,B), (member(A, [a,b,c]), (A==a;A==c),B=q), Bag)");
            // check
            int i = 0;
            foreach (PlTermV t in q.Solutions)
            {
                Assert.AreEqual(mm[i++].ToString(), t[0].ToString());
            }

        }
         */

    } // LinqSwiPl 
}
