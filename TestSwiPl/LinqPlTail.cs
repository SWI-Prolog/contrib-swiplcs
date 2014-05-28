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
using System.Linq;

namespace TestSwiPl
{

	/// <summary>
	/// These Testcases are more samples how to use Linq to 
    /// get the values of a Plrolog list ( PlTail )
	/// </summary>




    [TestClass]
    public class TestLinqPlTail : BasePlInit
	{
        [TestMethod]
        public void xxxxxx()
        {
            string[] mm = { "aa", "bb", "cc" };
            var q = new PlQuery("member(A, [aa, bb, cc])");
            Assert.AreEqual(3, q.SolutionVariables.Count());
            int i = 0;
            foreach (PlQueryVariables s in q.SolutionVariables)
            {
                Assert.AreEqual(mm[i++], s["A"].ToString());
            }

            i = 0;
            foreach (PlQueryVariables s in q.SolutionVariables)
            {
                Assert.AreEqual(mm[i++], s["A"].ToString());
            }

            var result = from svs in q.ToList() select svs["A"].ToString();
            if(result.Any() )
            CollectionAssert.AreEqual(mm, result.ToList());
        }


	    #region query_prologlist_PlTail_with_Linq_doc
        [TestMethod]
        public void LinQ_2_Object_from_a_list()
        {
            var list = new PlTerm("[w,x,y,z]");
            var result = from n in list
                         where n != "x"
                         orderby n descending
                         select n;
            // check
            string[] mm = {"z", "y", "w"};
            int i = 0;
            foreach (PlTerm t in result)
                Assert.AreEqual(mm[i++], t.ToString());
        }

        [TestMethod]
        public void LinQ_2_Object_from_a_list_numbers()
        {
            var list = new PlTerm("[4,5,a,f,6,7,8]");
            var result = from n in list
                         where n != "6" && n.IsNumber && (int)n >= 5
                         orderby n descending
                         select n;
            // check
            string[] mm = { "8", "7", "5" };
            int i = 0;
            foreach (PlTerm t in result)
                Assert.AreEqual(mm[i++], t.ToString());
        }

        [TestMethod]
        public void LinQ_2_Object_from_a_list_numbers_as_int()
        {
            var list = new PlTerm("[4,5,a,f,6,7,8]");
            var result = from n in list
                where n != "6" && n.IsNumber && (int) n >= 5
                orderby n descending
                select (int) n;
            // check
            CollectionAssert.AreEqual(new[] { 8, 7, 5 }, result.ToList());
        }
        #endregion

    }
}
