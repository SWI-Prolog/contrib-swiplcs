/*********************************************************
* 
*  Author:        Uwe Lesta
*  Copyright (C): 2008-2014, Uwe Lesta SBS-Softwaresysteme GmbH
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
using SbsSW.SwiPlCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestSwiPl
{

    [TestClass]
    public class TestPlTailPrivate : BasePlInit
	{

        // How to test private members 
        // see http://msdn.microsoft.com/en-us/library/bb546207.aspx
        // and  http://codethings.blogspot.de/2012/10/testing-private-members-with.html

        private bool NextAccessor(PlTerm t, out PlTerm term)
        {
            var tO = new PrivateObject(t);
            term = new PlTerm();
            var obArr = new Object[] { term };
            var b = (bool)tO.Invoke("Next", obArr);
            term = (PlTerm)obArr[0];
            return b;
        }


        [TestMethod]
        public void ListNext()
		{
            PlTerm t1;
            var l2 = new PlTerm("[x,y,z]");

            Assert.IsTrue(NextAccessor(l2, out t1), "next 1");
			Assert.AreEqual("x", (string)t1, "elem x");
            Assert.IsTrue(NextAccessor(l2, out t1), "next 2");
			Assert.AreEqual("y", (string)t1, "elem y");
		}

        [TestMethod]
        public void ListNextLast()
        {
            PlTerm t1;
            var l2 = new PlTerm("[x]");
            Assert.IsTrue(NextAccessor(l2, out t1), "next 1");
            Assert.AreEqual("x", (string)t1, "elem x");
            Assert.IsFalse(NextAccessor(l2, out t1), "next 2");
            Assert.IsTrue(t1.IsVar, "elem var");
        }

		
	}
}
