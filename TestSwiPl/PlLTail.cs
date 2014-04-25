
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
    public class t_PLTail : BasePlInit
	{



		#region Test where somethigng is wrog with


        [TestMethod]
		public void ListAppendAout()
		{
			PlTerm t1 = new PlTerm("[x,y,z]");
			PlTerm t2 = new PlTerm("ende");
            PlTerm l2 = PlTerm.PlTail(t1);

			l2.Close();

			Console.WriteLine(l2.ToString());

			l2.Append(t2);

			Console.WriteLine(t2.ToString());
			Console.WriteLine(l2.ToString());
		}

        //[TestMethod]
		//[Ignore]
		//public void ListCloseString()
		//{
		//    PlTail l2 = new PlTail("[x,y,z]");
		//    Assert.AreEqual(0, l2.Close(), "close");
		//    Assert.AreEqual(3, list_length(l2), "list_length");
		//}


        //[TestMethod]
		//[Ignore]
		//public void ListAppendBase()
		//{
		//    PlTerm t1 = new PlTerm("[x,y,z]");
		//    PlTail l2 = new PlTail(t1);
		//    Assert.IsTrue(l2.Append(new PlTerm("ende")), "Append return false");
		//    Assert.AreEqual(4, list_length(l2));
		//}


        //[TestMethod]
		//[Ignore]
		//public void ListAppend()
		//{
		//    PlTerm l1 = new PlTerm("[a,b,c,d]");
		//    PlTail l2 = new PlTail("[x,y,z]");
		//    Assert.IsTrue(l2.Append(new PlTerm("ende")), "Append return false");
		//    Assert.AreEqual(4, list_length(l2));
		//}
		#endregion


		#region constructor

        [TestMethod]
        [ExpectedException(typeof(SbsSW.DesignByContract.PreconditionException))]
		public void Create_list_from_atom()
		{
			PlTerm t1 = new PlTerm("atom");
            PlTerm l2 = PlTerm.PlTail(t1);		// should throw PlTypeException
		}

        [TestMethod]
		public void Create_List()
		{
			PlTerm t1 = new PlTerm("[a,b,c,d]");
			bool result = PlQuery.PlCall("is_list", new PlTermV(t1));
			Assert.IsTrue(result);
			// test is_list
			Assert.IsFalse(PlQuery.PlCall("is_list", new PlTermV(new PlTerm("AA"))));
		}
        [TestMethod]
		public void Create_ListFromPlTerm()
		{
			PlTerm t1 = new PlTerm("[a,b,c,d]");
            PlTerm l1 = PlTerm.PlTail(t1);
			Assert.IsTrue(PlQuery.PlCall("is_list", new PlTermV(l1)));
		}

        [TestMethod]
        public void Create_ListFromString()
        {
            PlTerm l1 = new PlTerm("[a,b,c,d]");
            Assert.IsTrue(PlQuery.PlCall("is_list", new PlTermV(l1)));
        }

        [TestMethod]
        public void Create_ListFromString2()
        {
            Assert.IsTrue(PlQuery.PlCall("is_list", new PlTermV(new PlTerm("[a,b,c,d]"))));
        }

		#endregion



		#region constructing Lists
        [TestMethod]
        // doc c++ interface - 4.11 The class PlTail
        #region List_Append_from_doc
        public void List_Append()
		{
            PlTerm t = PlTerm.PlVar();
            PlTerm l = PlTerm.PlTail(t);

			Assert.IsTrue(l.Append(new PlTerm("one")), "append one");
			Assert.IsTrue(l.Append(new PlTerm("two")), "append two");
			Assert.IsTrue(l.Append(new PlTerm("three")), "append three");

			Assert.AreEqual(1, l.Close());

			Assert.AreEqual(3, list_length(t));
            Assert.AreEqual("[one,two,three]", t.ToString());
        }
        #endregion List_Append_from_doc


        [TestMethod]
		public void List_Append_compare_term()
		{
			// doc c++ interface - 4.11 The class PlTail
//            PlTerm t = new PlTerm();
            PlTerm t = PlTerm.PlVar();
            PlTerm l = PlTerm.PlTail(t);
			l.Append(new PlTerm("a"));
			l.Append(new PlTerm("b"));
			l.Append(new PlTerm("c"));
			l.Close();

			PlTerm t2 = new PlTerm("[a,b,c]");
			Assert.IsTrue(t2 == t, "equal = ");
			Assert.IsTrue(PlQuery.PlCall("=", new PlTermV(t2, t)), "plquery =");
		}


        [TestMethod]
		public void List_Add_term()
		{
			PlTerm t = new PlTerm("c");
            PlTerm l = new PlTerm("[a,b]");

            Assert.AreEqual("c", t.ToString(), "t != c");
            Assert.AreEqual("[a,b]", l.ToString(), "l != [a,b]");

			Assert.IsTrue(l.Add(t), "Add returns false");
			Assert.AreEqual(3, list_length(l), "list_length l");
			Assert.AreEqual("[a,b,c]", l.ToString(), "string comp");
		}

        [TestMethod]
		public void List_Add_term_2()
		{
			PlTerm t = new PlTerm("[x,y]");
            PlTerm l = new PlTerm("[a,b]");

            Assert.AreEqual("[x,y]", t.ToString(), "t != [x,y]");

			Assert.IsTrue(l.Add(t), "Add returns false");
			Assert.AreEqual(3, list_length(l), "list_length l");
			// NOTE list T is one term so
			Assert.AreEqual("[a,b,[x,y]]", l.ToString(), "string comp");
		}

        [TestMethod]
        public void List_Add_list()
        {
            PlTerm t = new PlTerm("[x,y]");
            PlTerm l = new PlTerm("[a,b]");

            Assert.AreEqual("[x,y]", t.ToString(), "t != [x,y]");
            Assert.AreEqual("[a,b]", l.ToString(), "l != [a,b]");

            Assert.IsTrue(l.AddList(t), "Add returns false");
            Assert.AreEqual("[a,b,x,y]", l.ToString(), "l != [a,b,x,y]");

            Assert.AreEqual(4, list_length(l), "list_length l");
            // NOTE list T is a PlTail so
            Assert.AreEqual("[a,b,x,y]", l.ToString(), "string comp");
        }

        [TestMethod]
        #region List_Add_list_doc
        public void List_Add_list_doc()
        {
            PlTerm t = new PlTerm("[x,y]");
            PlTerm l = new PlTerm("[a,b]");

            Assert.IsTrue(l.IsList);
            Assert.IsTrue(l.AddList(t));
            Assert.IsTrue(l.IsList);
            Assert.AreEqual("[a,b,x,y]", l.ToString());
            Assert.AreEqual("a", l.NextValue().ToString());
            Assert.AreEqual("b", l.NextValue().ToString());
            Assert.AreEqual("[x,y]", l.ToString());
        }
        #endregion List_Add_list_doc

        [TestMethod]
        public void List_construction_var()
        {
            // PlTail l = new PlTail("X");
            PlTerm l = new PlTerm("X");
            Assert.AreEqual(0, list_length(l), "list_length l");
        }

        [TestMethod]
        [ExpectedException(typeof(SbsSW.DesignByContract.PreconditionException), "list")]
        public void List_construction_atom_fail()
        {
            PlTerm l = PlTerm.PlTail(new PlTerm("a"));
        }

        [TestMethod]
        [ExpectedException(typeof(SbsSW.DesignByContract.PreconditionException), "list")]
        public void List_construction_int_fail()
        {
            PlTerm l = PlTerm.PlTail(new PlTerm("1"));
        }


		#endregion


		#region new stuff foreach Enumerable   //list and indexer

        [TestMethod]
		public void List_foreach()
		{
            PlTerm l2 = new PlTerm("[x,y,z]");
			int i = 0;
			foreach (PlTerm t in l2)
			{
				switch (i++)
				{
					case 0: Assert.AreEqual("x", (string)t, "first"); break;
					case 1: Assert.AreEqual("y", (string)t, "second"); break;
					case 2: Assert.AreEqual("z", (string)t, "third"); break;
					default: Assert.IsFalse(true, "GOK"); break;
				}
			}
		}

        [TestMethod]
		public void List_query_foreach()
		{
			string[] sol = new string[]{"[a,b,c]", "[x,y,z]"};
			PlQuery.PlCall("assert(test([a,b,c]))");
			PlQuery.PlCall("assert(test([x,y,z]))");
			PlQuery q = new PlQuery("test", new PlTermV(1));
			int sol_idx = 0;
			foreach (PlTermV s in q.Solutions)
			{
				Console.WriteLine(s[0].ToString());	// testausgabe
				Assert.AreEqual(sol[sol_idx++].ToString(), s[0].ToString());
                PlTerm list = PlTerm.PlTail(s[0]);
				foreach( PlTerm t in list )
				{
					Console.WriteLine(t.ToString());	// testausgabe
				}
			}
		}

		#endregion


		#region sample


		// sample

        public List<PlTerm> BuildList(PlTerm list)
		{
            Assert.IsTrue(list.IsList);
			List<PlTerm> l = new List<PlTerm>();
            foreach (PlTerm t in list) {
                l.Add(t);
            }
            return l;
		}



        [TestMethod]
		public void List_ToList_sample()
		{
            PlTerm l2 = new PlTerm("[x,y,z]");
			List<PlTerm> term_list = BuildList(l2);
			Assert.AreEqual(3, term_list.Count, "List.Count");
			for (int i = 0; i < term_list.Count - 1; i++)
			{
				switch (i)
				{
					case 0: Assert.AreEqual("x", (string)term_list[i], "first"); break;
					case 1: Assert.AreEqual("y", (string)term_list[i], "second"); break;
					case 2: Assert.AreEqual("z", (string)term_list[i], "third"); break;
					default: Assert.IsFalse(true, "GOK"); break;
				}
			}
		}

		#endregion

        [TestMethod]
        public void list_sample()
        {
            
        }

        [TestMethod]
        public void list_sample2()
        {
            PlTerm list = PlQuery.PlCallQuery("sort([2,3,1,5,6], ListSorted)");
            foreach (PlTerm t in list)
            {
                Console.WriteLine(t.ToString());
            }
        }

        [TestMethod]
        public void list_sample3()
        {
            PlQuery q = new PlQuery("sort(L, ListSorted)");
            q.Variables["L"].Unify(new PlTerm("[2,3,1,5,6]"));
            q.NextSolution();
            string s = q.Variables["ListSorted"].ToString();
            Console.WriteLine(s);
        }

	}
}
