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

using System.Globalization;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestSwiPl
{
	
    [TestClass]
    public class TestPlTerm : BasePlInit
    {
        
        #region Unify
        [TestMethod]
		public void Unify()
		{
			var t1 = new PlTerm("x");
            var t2 = new PlTerm("x");
            var t3 = new PlTerm("y");
			Assert.IsTrue(t1.Unify(t2));
			Assert.IsTrue(t2.Unify(t1));
			Assert.IsFalse(t1.Unify(t3));
			Assert.IsFalse(t3.Unify(t1));
		}

        [TestMethod]
		public void UnifyTerm()
		{
            var t1 = new PlTerm("x(a)");
            var t2 = new PlTerm("x(a)");
            var t3 = new PlTerm("y(a)");
			Assert.IsTrue(t1.Unify(t2));
			Assert.IsTrue(t2.Unify(t1));
			Assert.IsFalse(t1.Unify(t3));
			Assert.IsFalse(t3.Unify(t1));
		}

        [TestMethod]
		public void UnifyTermVar()
		{
            var t1 = new PlTerm("x(A)");
            var t2 = new PlTerm("x(A)");
            var t3 = new PlTerm("y(A)");
			Assert.IsTrue(t1.Unify(t2));
			Assert.IsTrue(t2.Unify(t1));
			Assert.IsFalse(t1.Unify(t3));
			Assert.IsFalse(t3.Unify(t1));
		}

        [TestMethod]
        public void EqualSign()
        {
            var t1 = new PlTerm("x(A)");
            var t2 = t1;
            var t1O = new PrivateObject(t1);
            var t2O = new PrivateObject(t2);
            Assert.AreEqual(t1O.GetProperty("TermRef"), t2O.GetProperty("TermRef"));
        }



        [TestMethod]
		public void UnifyTermVar1()
		{
            var t1 = new PlTerm("x(A)");
            var t2 = new PlTerm("x(1)");
			Assert.IsTrue(t1.Unify(t2));
			Assert.AreEqual("x(1)", t1.ToString());
		}

        [TestMethod]
        #region UnifyTermVar_doc
        public void UnifyTermVar_doc()
		{
			var t1 = new PlTerm("x(A, 2)");
			var t2 = new PlTerm("x(1, B)");
			Assert.IsTrue(t1.Unify(t2));
			Assert.AreEqual("x(1,2)", t1.ToString());
			Assert.AreEqual("x(1,2)", t2.ToString());
        }
        #endregion UnifyTermVar_doc

        [TestMethod]
		public void UnifyInt()
		{
            var t1 = new PlTerm("123");
            var t2 = new PlTerm("123");
			Assert.IsTrue(t1.Unify(t2));
		}
        [TestMethod]
		public void UnifyIntVar()
		{
            var t1 = new PlTerm("123");
            var t2 = new PlTerm("A");
			Assert.IsTrue(t1.Unify(t2));
			Assert.AreEqual("123", t2.ToString());
		}

        [TestMethod]
		public void UnifyListTuple()
		{
            var list = new PlTerm("[1, (a, b), 2, (c, d)]");
			int idx = 0;
            foreach (var t in list)
			{
				//Console.WriteLine(t.ToString());	// testausgabe
				if (t.IsCompound && t.Arity == 2)
				{
                    var v = new PlTermV(2);
                    var c = PlTerm.PlCompound(",", v);
					t.Unify(c);
					if (idx == 0)
					{
						Assert.AreEqual("a", v[0].ToString(), "a");
						Assert.AreEqual("b", v[1].ToString(), "b");
					} else if (idx == 1)
					{
						Assert.AreEqual("c", v[0].ToString(), "c");
						Assert.AreEqual("d", v[1].ToString(), "d");
					}
					idx++;
				}
			}
		}

		#endregion

        #region cast operators

        [TestMethod]
        public void cast_double()
        {
            var t = new PlTerm(1.2);
            var d = (double) t;
            Assert.AreEqual(1.2, d);
        }

        [TestMethod]
        public void cast_double_from_string()
        {
            var t = new PlTerm("1.2");
            var d = (double)t;
            Assert.AreEqual(1.2, d);
        }

        [TestMethod]
        [ExpectedException(typeof(PlException))]
        public void cast_double_exception()
        {
            var t = new PlTerm("1.2r");
            var d = (double)t;          // trow an exception
            Assert.AreEqual(1.2, d);    // never executed
        }

        #endregion cast operators

        [TestMethod]
        public void equals_long()
        {
            var t = new PlTerm(12);
            Assert.IsTrue(12 == t);
            Assert.IsTrue(t == 12);
        }

        [TestMethod]
        public void equals_string()
        {
            var t = new PlTerm("12");
            Assert.IsTrue(12 == t);
            Assert.IsTrue(t == 12);
            Assert.IsTrue("12" == t);
            Assert.IsTrue(t == "12");
        }

        #region special methods


        [TestMethod]
        public void plterm_PlString_len()
        {
            var t = PlTerm.PlString("abc", 9);
            Assert.AreEqual("abc", t.ToString());
        }

        [TestMethod]
        public void plterm_PlString_len_2()
        {
            var t = PlTerm.PlString("abcde", 2);
            Assert.AreEqual("ab", t.ToString());
        }

        [TestMethod]
        public void plterm_PlCharList()
        {
            var t = PlTerm.PlCharList("abc");
            Assert.AreEqual("[a,b,c]", t.ToString());
        }


        // lists

        [TestMethod]
        public void list_GetEnumerator()
        {
            const string mm = "abc";
            int i = 0;
            var t = new PlTerm("[a,b,c]");
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, t.ToListString());
            foreach (var x in t)
                Assert.AreEqual(mm[i++].ToString(CultureInfo.InvariantCulture), x.ToString());
        }

        [TestMethod]
        public void list_GetEnumerator_2()
        {
            const string mm = "abc";
            int i = 0;
            var t = new PlTerm("[a,b,c]");
            var e = t.GetEnumerator();
            while (e.MoveNext())
                Assert.AreEqual(mm[i++].ToString(CultureInfo.InvariantCulture), e.Current.ToString());
        }

        [TestMethod]
        public void plterm_IsInitialized()
        {
            var t = PlTerm.PlVar();
            Assert.IsTrue(t.IsInitialized);
        }



        #endregion special methods


        #region Equals and GetHashCode
        [TestMethod]
        public void plterm_Equals_term()
        {
            var t = new PlTerm(12);
            var t2 = new PlTerm("12");
            Assert.IsTrue(t.Equals(t2));
        }

        [TestMethod]
        public void plterm_Equals_int()
        {
            var t = new PlTerm(12);
            Object t2 = 12;
            Assert.IsTrue(t.Equals(t2));
        }

        [TestMethod]
        public void plterm_Equals_int_false()
        {
            var t = new PlTerm(12);
            Object t2 = new Int32();
            Assert.IsFalse(t.Equals(t2));
        }

        [TestMethod]
        public void plterm_GetHashCode()
        {
            var t = PlTerm.PlVar();
            var t2 = PlTerm.PlVar();
            t.Unify(t2);
            Assert.AreNotEqual(t.GetHashCode(), t2.GetHashCode());
        }
        
        #endregion Equals and GetHashCode

        #region ToString()

        [TestMethod]
		public void ToString_var()
		{
            var t = PlTerm.PlVar();
			Assert.IsTrue(t.ToString().StartsWith("_L"), "Start not with _L - " + t);
		}

        [TestMethod]
		public void ToString_var2()
		{
            var t = new PlTerm("A");
            Assert.IsTrue(t.IsVar);
            Assert.AreEqual("_L", t.ToString().Substring(0, 2));
		}

        [TestMethod]
		public void ToString_atom()
		{
            var t = new PlTerm("atomic");
            Assert.IsTrue(t.IsAtom);
			Assert.AreEqual("atomic", t.ToString());
		}

        [TestMethod]
		public void ToString_pl_string()
		{
            var t = PlTerm.PlString("string");
            Assert.IsTrue(t.IsString);
            Assert.AreEqual("string", t.ToString());
		}

        [TestMethod]
        public void ToString_Compound()
        {
            var t = new PlTerm("a(i)");
            Assert.IsTrue(t.IsCompound);
            Assert.AreEqual("a(i)", t.ToString());
        }

        [TestMethod]
        public void ToString_is_float()
        {
            var t = new PlTerm(1.2);
            Assert.IsTrue(t.IsFloat);
            Assert.AreEqual("1.2", t.ToString());
        }

        [TestMethod]
        public void ToString_is_int()
        {
            var t = new PlTerm(12);
            Assert.IsTrue(t.IsInteger);
            Assert.AreEqual("12", t.ToString());
        }

        [TestMethod]
        public void ToString_is_list()
        {
            var t = new PlTerm("[1,2,3]");
            Assert.IsTrue(t.IsList);
            Assert.AreEqual("[1,2,3]", t.ToString());
        }

        [TestMethod]
        public void ToString_is_ground()
        {
            var t = new PlTerm("a(i)");
            Assert.IsTrue(t.IsGround);
        }

        [TestMethod]
        public void ToString_is_not_ground()
        {
            var t = new PlTerm("a(I)");
            Assert.IsFalse(t.IsGround);
        }

	    [TestMethod]
        public void ToString_Compound_string()
        {
            // bugfix reported by batu.akan@mdh.se mail from 13.07.2009
            //  write_term(A, [quoted(true)]) % do the trick
            var term = new PlTerm("cs_speak('hello world')");
            string s = term.ToStringCanonical();
            Assert.AreEqual("cs_speak('hello world')", s);
        }

        [TestMethod]
        public void ToStringCanonical_space()
        {
            var term = new PlTerm("'a b'");
            string s = term.ToStringCanonical();
            Assert.AreEqual("'a b'", s);
        }

        [TestMethod]
        public void ToStringCanonical_UpperCaseAtom()
        {
            var term = new PlTerm("'Atom'");
            string s = term.ToStringCanonical();
            Assert.AreEqual("'Atom'", s);
        }

		#endregion

		#region Arity

        [TestMethod]
		public void Arity_list()
		{
			var t = new PlTerm("[1,2,3]");
			Assert.AreEqual(2, t.Arity);
		}

        [TestMethod]
		public void Arity_Tuple()
		{
			var t = new PlTerm("(a,b)");
			Assert.AreEqual(2, t.Arity);
		}

        [TestMethod]
		public void Arity_Triple()
		{
			var t = new PlTerm("(a,b,c)");
			Assert.AreEqual(2, t.Arity);
		}

        [TestMethod]
		public void Arity_named_Triple()
		{
			var t = new PlTerm("x(a,b,c)");
			Assert.AreEqual(3, t.Arity);
		}

		#endregion

        #region PlTerm.Compound

        [TestMethod]
        public void PlTerm_PlCompound_termV()
        {
            var tv = new PlTermV(new PlTerm("a"), new PlTerm("b"), new PlTerm("c"));
            var t = PlTerm.PlCompound("foo", tv);
            Assert.AreEqual("foo(a,b,c)", t.ToString());
        }

        [TestMethod]
        public void PlTerm_PlCompound_1()
        {
            var t = PlTerm.PlCompound("foo", new PlTerm("a"));
            Assert.AreEqual("foo(a)", t.ToString());
        }

        [TestMethod]
        public void PlTerm_PlCompound_2()
        {
            var t = PlTerm.PlCompound("foo", new PlTerm("a"), new PlTerm("b"));
            Assert.AreEqual("foo(a,b)", t.ToString());
        }

        [TestMethod]
        public void PlTerm_PlCompound_3()
        {
            var t = PlTerm.PlCompound("foo", new PlTerm("a"), new PlTerm("b"), new PlTerm("c"));
            Assert.AreEqual("foo(a,b,c)", t.ToString());
        }


        [TestMethod]
        // NOTE: Microsoft_BUG: Exception message text is not compared
        [ExpectedException(typeof(NotSupportedException), "Work only for compound terms!")]
        public void PlTermIndexer_exception_atom()
        {
            var t = new PlTerm("foo");
            Assert.AreEqual("foo", t[0].ToString());
        }
        [TestMethod]
        [ExpectedException(typeof(NotSupportedException))]
        public void PlTermIndexer_exception_var()
        {
            var t = new PlTerm("X");
            Assert.AreEqual("X", t[0].ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PlTermIndexer_exception_range()
        {
            var t = new PlTerm("foo(bar)");
            Assert.AreEqual("foo(bar)", t[-1].ToString());
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PlTermIndexer_exception_range_2()
        {
            var t = new PlTerm("foo(bar)");
            Assert.AreEqual("bar", t[2].ToString());
        }


        [TestMethod]
        public void PlTermIndexer1()
        {
            var t = new PlTerm("foo(bar)");
            Assert.AreEqual("foo", t[0].ToString());
            Assert.AreEqual("bar", t[1].ToString());
        }
        
        #region PlTerm_indexer_doc
        [TestMethod]
        public void PlTermIndexer2()
        {
            var t = new PlTerm("foo(bar, x(y))");
            Assert.AreEqual("foo", t[0].ToString());
            Assert.AreEqual("bar", t[1].ToString());
            Assert.AreEqual("x(y)", t[2].ToString());
        }

        [TestMethod]
        public void PlTermIndexer_list1()
        {
            var t = new PlTerm("[a,b,c]");
            Assert.AreEqual(".", t[0].ToString());
            Assert.AreEqual("a", t[1].ToString());
            Assert.AreEqual("[b,c]", t[2].ToString());
        }
        #endregion PlTerm_indexer_doc

        #endregion PlTerm.Compound

    }
}
