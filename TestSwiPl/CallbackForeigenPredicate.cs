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
using System.Linq;
using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Callback;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;		// für List_ToList sample

namespace TestSwiPl
{

	/// <summary>
	/// These Testcases are samples how to use C# methods as a callback predicate from prolog
	/// </summary>

    [TestClass]
    public class TestCallbackForeigenPredicate : BasePlInit
	{


		#region Deterministic

        [TestMethod]
        #region t_in_out_doc
        public void t_in_out()
        {
            Delegate replaceDelegate = new DelegateParameter2(atom_replace);
            PlEngine.RegisterForeign(replaceDelegate);
            for (int i = 1; i < 10; i++)
            {
                var arg = new PlTermV(new PlTerm("test_f"), PlTerm.PlVar());
                PlQuery.PlCall("atom_replace", arg);
                Assert.AreEqual("test_xx_f", arg[1].ToString(), "atom_replace failed!");
            }
        }
        public static bool atom_replace(PlTerm atomIn, PlTerm atomOut)
        {
            return atomOut.Unify(atomIn.ToString().Replace("_", "_xx_"));
        }
        #endregion t_in_out_doc



        #region t_creating_a_list_doc
        [TestMethod]
        public void t_creating_a_list()
        {
            Delegate d = new DelegateParameter1(create_list);
            PlEngine.RegisterForeign(d);
            for (int i = 1; i < 10; i++)
            {
                PlTerm t = PlQuery.PlCallQuery("create_list(L)");
                Assert.AreEqual("[a,b,c]", t.ToString(), "create_list failed!");
            }
        }
        public static bool create_list(PlTerm list)
        {
            return list.Unify(new PlTerm("[a,b,c]"));
        }
        #endregion t_creating_a_list_doc


        [TestMethod]
        public void t_compound_term()
        {
            Delegate d = new DelegateParameter3(compound_term);
            PlEngine.RegisterForeign(d);
            PlTerm t = PlQuery.PlCallQuery("compound_term(test(P, schmerz, arm), 1, PART)");
            Assert.IsTrue(t.IsVar, "1 create_list failed!");
            t = PlQuery.PlCallQuery("compound_term(test(P, schmerz, arm), 2, PART)");
            Assert.AreEqual("schmerz", t.ToString(), "2 create_list failed!");
            t = PlQuery.PlCallQuery("compound_term(test(P, schmerz, arm), 3, PART)");
            Assert.AreEqual("arm", t.ToString(), "3 create_list failed!");
        }
        public static bool compound_term(PlTerm compound, PlTerm number, PlTerm l)
        {
            bool isComp = compound.IsCompound;
            int araty = compound.Arity;
            string name = compound.Name;
            System.Diagnostics.Debug.Print("compound_term {0} name={1} arity={2}", isComp, name, araty);

            using (new PlFrame())
            {
                PlTerm list = PlQuery.PlCallQuery(compound + " =.. L");

                // conversion is necessary because indexing the prolog list (list) is based on the 
                // order of internal term references which might be *not* the order which we expect here.
                List<PlTerm> tl = Enumerable.ToList(list);

                l.Unify(tl[(int)number]);
            }

            return true;
        }

        [TestMethod]
        public void t_compound_term_arg()
        {
            Delegate d = new DelegateParameter3(compound_term_arg);
            PlEngine.RegisterForeign(d);
            PlTerm t = PlQuery.PlCallQuery("compound_term_arg(test(P, schmerz, arm), 1, PART)");
            Assert.IsTrue(t.IsVar, "1 create_list failed!");
            t = PlQuery.PlCallQuery("compound_term_arg(test(P, schmerz, arm), 2, PART)");
            Assert.AreEqual("schmerz", t.ToString(), "2 create_list failed!");
            t = PlQuery.PlCallQuery("compound_term_arg(test(P, schmerz, arm), 3, PART)");
            Assert.AreEqual("arm", t.ToString(), "3 create_list failed!");
        }
        public static bool compound_term_arg(PlTerm compound, PlTerm number, PlTerm l)
        {
            return PlQuery.PlCall("arg", new PlTermV(number, compound, l));
        }


        [TestMethod]
        public void t_succeed_and_fail()
        {
            Delegate d = new DelegateParameter1(odd);
            PlEngine.RegisterForeign(d);
            Assert.IsFalse(PlQuery.PlCall("odd(as)"));
            Assert.IsFalse(PlQuery.PlCall("odd(4)"));
            Assert.IsTrue(PlQuery.PlCall("odd(5)"));
        }
// ReSharper disable once InconsistentNaming
        public static bool odd(PlTerm term)
        {
            if(term.IsInteger)
            {
                bool isOdd = 1 == ((int)term) % 2;
                return isOdd;
            }
            return false;
        }



        [TestMethod]
        public void t_modifying_a_list()
        {
            Delegate d = new DelegateParameter2(modify_list);
            PlEngine.RegisterForeign(d);
            for (int i = 1; i < 10; i++)
            {
                PlTerm t = PlQuery.PlCallQuery("modify_list([a,b,c], L)");
                Assert.AreEqual("[aa,bb,cc]", t.ToString(), "modify_list failed!");
            }
        }
        public static bool modify_list(PlTerm listIn, PlTerm listOut)
        {
            using (new PlFrame())
            {
                var list = new PlTerm("[]");
                Assert.IsTrue(listIn.IsList);
                foreach (PlTerm elem in PlTerm.PlTail(listIn))
                {
                    list.Add(new PlTerm(elem.ToString() + elem.ToString()));
                }
                listOut.Unify(list);
            }
            return true;
        }



        [TestMethod]
        public void t_callback_with_subQuery()
        {
            Delegate d = new DelegateParameter1(assert_fact);
            PlEngine.RegisterForeign(d);

            Assert.IsTrue(PlQuery.PlCall("assert( (test(Result) :- member(X,[1,2,3]), assert_fact(X), findall(E, fact(E), Result)) )"));

            int count = 1;
            var q = new PlQuery("test(L)");
            foreach (PlTermV s in q.Solutions)
            {
                switch (count)
                {
                    case 1: Assert.AreEqual("[1]", s[0].ToString()); break;
                    case 2: Assert.AreEqual("[1,2]", s[0].ToString()); break;
                    case 3: Assert.AreEqual("[1,2,3]", s[0].ToString()); break;
                }
                //Console.WriteLine(s[0].ToString());
                count++;
            }
        }
        public static bool assert_fact(PlTerm termIn)
        {
            return PlQuery.PlCall("assert(fact(" + termIn.ToString() + "))");// ? 1 : 0;
        }

        #endregion


        #region Nondeterministic
        //[Ignore]
        //[TestMethod]
        public void t_backtrack()
        {
            Delegate d = new DelegateParameterBacktrack(my_member);
            PlEngine.RegisterForeign(d);
            for (int i = 1; i < 10; i++)
            {
                PlTerm t = PlQuery.PlCallQuery("my_member(X, [a,b,c])");
                Assert.AreEqual("abc", t.ToString(), "my_concat_atom failed!");
            }
        }
        public static int my_member(PlTerm termOut, PlTerm termIn, IntPtr controlHandle)
        {
            //switch( PL_foreign_control(handle) )
            //{ 
            //    case PL_FIRST_CALL:
            //    ctxt = malloc(sizeof(struct context));
            //    ...
            //    PL_retry_address(ctxt);
            //case PL_REDO:
            //    ctxt = PL_foreign_context_address(handle);
            //    ...
            //    PL_retry_address(ctxt);
            //case PL_CUTTED:
            //    ctxt = PL_foreign_context_address(handle);
            //    ...
            //    free(ctxt);
            //    PL_succeed;
            //}
            return 1;
        }

        #endregion


        #region VarArgs

        [TestMethod]
        public void t_varargs_single()
        {
// ReSharper disable once CSharpWarnings::CS0618
            Delegate d = new DelegateParameterVarArgs(my_concat_atom);
            PlEngine.RegisterForeign("my_concat_atom", 4, d);
            for (int i = 1; i < 10; i++)
            {
                PlTerm t = PlQuery.PlCallQuery("my_concat_atom(a,b,c, X)");
                Assert.AreEqual("abc", t.ToString(), "my_concat_atom failed!");
            }
        }



        [TestMethod]
        #region t_varargs_doc
        public void t_varargs()
        {
            Delegate d = new DelegateParameterVarArgs(my_concat_atom);
            PlEngine.RegisterForeign("my_concat_atom", 4, d);
            PlEngine.RegisterForeign("my_concat_atom", 7, d);
            for (int i = 1; i < 10; i++)
            {
                PlTerm t = PlQuery.PlCallQuery("my_concat_atom(a,b,c, X)");
                Assert.AreEqual("abc", t.ToString(), "my_concat_atom failed!");
                t = PlQuery.PlCallQuery("my_concat_atom(a,b,c,d,e,f, X)");
                Assert.AreEqual("abcdef", t.ToString(), "my_concat_atom failed!");
            }
        }
        public static bool my_concat_atom(PlTermV term1)
        {
            System.Diagnostics.Debug.Print("my_concat_atom called with term - {0}", term1.ToString());

            int arity = term1.Size;
            string sRet = "";
            PlTerm termOut = term1[arity -1];

            for (int i = 0; i < arity-1; i++)
            {
                sRet += term1[i].ToString();
            }
            termOut.Unify(sRet);
            return true;
        }
        #endregion t_varargs_doc


        #endregion

    }
}
