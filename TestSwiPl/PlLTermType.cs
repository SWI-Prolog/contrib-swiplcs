using SbsSW.SwiPlCs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestSwiPl
{


	/// <summary>
	/// TestFälle zu 'SWI-cs' dem SWI prolog interface in CSharp
	/// </summary>
	
	[TestClass]
	public class PLTermType : BasePlInit
	{

		[TestMethod]
		public void var()
		{
            // PlTerm t = new PlTerm();
            PlTerm t = PlTerm.PlVar();
            Assert.AreEqual(1, (int)t.PlType, "type");
			Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
			Assert.IsTrue(t.IsVar);
			Assert.IsFalse(t.IsNumber);
			Assert.IsFalse(t.IsCompound);
		}

		[TestMethod]
		public void var2()
		{
			PlTerm t = new PlTerm("A");
			Assert.AreEqual(1, (int)t.PlType, "type");
			Assert.AreEqual(PlType.PlVariable, t.PlType, "type enum");
			Assert.IsTrue(t.IsVar);
			Assert.IsFalse(t.IsNumber);
			Assert.IsFalse(t.IsCompound);
		}

        [TestMethod]
        public void atom()
        {
            PlTerm t = new PlTerm("atomic");
            Assert.AreEqual(2, (int)t.PlType, "type");
            Assert.AreEqual(PlType.PlAtom, t.PlType, "type enum");
            Assert.IsTrue(t.IsAtom);
            Assert.IsTrue(t.IsAtomic);
            Assert.IsFalse(t.IsVar);
            Assert.IsFalse(t.IsNumber);
            Assert.IsFalse(t.IsCompound);
        }

        [TestMethod]
		public void pl_string()
		{
			PlTerm t = PlTerm.PlString("string");
			Assert.AreEqual(5, (int)t.PlType, "type");
			Assert.AreEqual(PlType.PlString, t.PlType, "type enum");
			Assert.IsTrue(t.IsString, "plstring");
			Assert.IsTrue(t.IsAtomic, "atomic");
			Assert.IsFalse(t.IsList, "list");
			Assert.IsFalse(t.IsAtom, "atom");
			Assert.IsFalse(t.IsVar, "var");
			Assert.IsFalse(t.IsNumber, "number");
			Assert.IsFalse(t.IsCompound, "compound");
		}

        [TestMethod]
        public void Compound()
        {
            PlTerm t = new PlTerm("a(i)");
            Assert.AreEqual(6, (int)t.PlType, "type");
            Assert.AreEqual(PlType.PlTerm, t.PlType, "type enum");
            Assert.IsTrue(t.IsCompound, "compound");
            Assert.IsFalse(t.IsVar, "var");
            Assert.IsFalse(t.IsNumber, "number");
        }


        [TestMethod]
        [ExpectedException(typeof(SbsSW.SwiPlCs.Exceptions.PlException), "Syntax error: Illegal start of term\na(i,\n** here **\n)")]
        public void Compound_error_2()
        {
            PlTerm t = new PlTerm("a(i,)");
        }

        [TestMethod]
        public void Compound_term1()
        {
            PlTerm t = new PlTerm("a(i, b(A,L))");
            Assert.AreEqual(2, t.Arity);
            Assert.AreEqual("a", t.Name);
            Assert.IsTrue(t.IsCompound, "compound");
            Assert.IsFalse(t.IsVar, "var");
            Assert.IsFalse(t.IsNumber, "number");
        }
        [TestMethod]
        public void Compound_term2()
        {
            PlTerm t = new PlTerm("X=a(i, b(A,L));true");
            Assert.AreEqual(2, t.Arity);
            Assert.AreEqual(";", t.Name);
            Assert.IsTrue(t.IsCompound, "compound");
            Assert.IsFalse(t.IsVar, "var");
            Assert.IsFalse(t.IsNumber, "number");
        }
        [TestMethod]
        public void Compound_compound1()
        {
            PlTerm t = new PlTerm("a(i, b(A,L))");
            Assert.AreEqual(2, t.Arity);
            Assert.AreEqual("a", t.Name);
            Assert.IsTrue(t.IsCompound, "compound");
            Assert.IsFalse(t.IsVar, "var");
            Assert.IsFalse(t.IsNumber, "number");
        }

        [TestMethod]
		public void is_float()
		{
			PlTerm t = new PlTerm(1.2);
			Assert.AreEqual(4, (int)t.PlType, "type");
			Assert.AreEqual(PlType.PlFloat, t.PlType, "type enum");
			Assert.IsTrue(t.IsFloat, "float");
			Assert.IsTrue(t.IsNumber, "number");
			Assert.IsFalse(t.IsInteger, "int");
			Assert.IsFalse(t.IsVar, "var");
			Assert.IsFalse(t.IsCompound, "compound");
		}

		[TestMethod]
		public void is_int()
		{
			PlTerm t = new PlTerm(12);
			Assert.AreEqual(3, (int)t.PlType, "type");
			Assert.AreEqual(PlType.PlInteger, t.PlType, "type ");
			Assert.IsTrue(t.IsNumber, "number");
			Assert.IsTrue(t.IsInteger, "int");
			Assert.IsFalse(t.IsFloat, "float");
			Assert.IsFalse(t.IsVar, "var");
			Assert.IsFalse(t.IsCompound, "compound");
		}

		[TestMethod]
		public void is_list()
		{
			PlTerm t = new PlTerm("[1,2,3]");
			Assert.AreEqual(6, (int)t.PlType, "type");
			Assert.AreEqual(PlType.PlTerm, t.PlType, "type ");
			Assert.IsTrue(t.IsCompound, "compound");
			Assert.IsTrue(t.IsList, "list");
			Assert.IsFalse(t.IsNumber, "number");
			Assert.IsFalse(t.IsInteger, "int");
			Assert.IsFalse(t.IsFloat, "float");
			Assert.IsFalse(t.IsVar, "var");
		}


	}
}
