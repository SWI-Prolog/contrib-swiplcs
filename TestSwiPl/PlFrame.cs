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

namespace TestSwiPl
{

    [TestClass]
    public class TestPlFrame : BasePlInit
	{

	    private uint TermRefAccessor(PlTerm t)
	    {
            var tO = new PrivateObject(t);
            var termTef = tO.GetProperty("TermRef");
            var tRef = new PrivateObject(termTef);
            var tRefInt = tRef.GetField("_uintptr");
#if _PL_X64
            return (uint)(ulong)tRefInt;
#else
            return (uint)tRefInt;
#endif
	    }

	    /// <summary>
        /// Sample from the documentation PlFrame
        /// </summary>
        [TestMethod]
        public void TestPlFrameSample()
        {
            var t1 = new PlTerm("dummy");
            AssertWord("test1");
            AssertWord("test2");
            var t2 = new PlTerm("dummy2");

            Assert.AreEqual(TermRefAccessor(t1), TermRefAccessor(t2) - 3);  // one per call of AssertWord

            PlQuery.PlCall("assert( (test(N) :- findall(X, word(X), L), length(L, N) ))");

            PlTerm term = PlQuery.PlCallQuery("test(N)");
            Assert.AreEqual(2, (int)term);
        }


        #region AssertWord_doc
        void AssertWord(string word)
        {
            using (PlFrame fr = new PlFrame())
            {
                PlTermV av = new PlTermV(1);
                av[0] = PlTerm.PlCompound("word", new PlTermV(new PlTerm(word)));
                using (PlQuery q = new PlQuery("assert", av))
                {
                    q.NextSolution();
                }
            }
        }
        #endregion AssertWord_doc

        #region AssertWord2_doc
        void AssertWord2(string word)
        {
            PlFrame fr = new PlFrame();
            PlTermV av = new PlTermV(1);
            av[0] = PlTerm.PlCompound("word", new PlTermV(new PlTerm(word)));
            PlQuery q = new PlQuery("assert", av);
            q.NextSolution();
            q.Dispose();   // IMPORTANT ! never forget to free the query before the PlFrame is closed
            fr.Dispose();
        }
        #endregion AssertWord2_doc


    } // test class PlFrame
}
