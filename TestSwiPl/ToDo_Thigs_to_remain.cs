using SbsSW.SwiPlCs;
using SbsSW.SwiPlCs.Exceptions;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestSwiPl
{
    /// <summary>
    /// things todo
    /// </summary>
    [TestClass]
    public class ToDo : BasePlInit
    {
        // in this case an exception was thrown but 
        //
        // "'$atomic_list_concat'/2: Type error: `list' expected, found `[a,b,c]'"
        //
        // i like to see it with out a try catch
        [TestMethod]
        [ExpectedException(typeof(PlTypeException), "`list' expected, found `[a,b,c]'")]
        public void TestException_in_a_query()
        {
            PlQuery plq = new PlQuery("atomic_list_concat(L, A)");
            Assert.IsTrue(plq.Variables["L"].Unify("[a,b,c]"));
            //Assert.IsTrue(plq.Variables["L"].Unify(new PlTerm("[a,b,c]")));
            foreach (PlQueryVariables vars in plq.SolutionVariables)
            {
                Assert.AreEqual("abc", vars["A1"].ToString());
            }
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


    } // ToDo 
}
