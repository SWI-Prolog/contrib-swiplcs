
using System;
using SbsSW.SwiPlCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;		// für List_ToList sample
using System.Linq;

namespace TestSwiPl
{

	/// <summary>
	/// These Testcases are mor samples how to use Linq to 
    /// get the values of a Plrolog list ( PlTail )
	/// </summary>

    [TestClass()]
    public class t_LinqPLTail : BasePlInit
	{


		#region query a prologlist ( PlTail ) with Linq


        [TestMethod]
        public void T_LinQ_2_Object_from_a_list()
        {
            PlTerm list = new PlTerm("[w,x,y,z]");
            var result = from n in list
                         where n != "x"
                         orderby n descending
                         select n;

            // check
            string mm = "zyw";
            int i = 0;
            foreach (PlTerm t in result)
                Assert.AreEqual(mm[i++].ToString(), t.ToString());
        }

        [TestMethod]
        public void T_LinQ_2_Object_from_a_list_numbers()
        {
            PlTerm list = new PlTerm("[4,5,a,f,6,7,8]");
            var result = from n in list
                         where n != "6" && n.IsNumber && (int)n >= 5
                         orderby n descending
                         select n;

            // check
            string mm = "875";
            int i = 0;
            foreach (PlTerm t in result)
                Assert.AreEqual(mm[i++].ToString(), t.ToString());
        }

        #endregion

    }
}
