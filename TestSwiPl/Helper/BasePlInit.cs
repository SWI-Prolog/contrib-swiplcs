/*********************************************************
 * 
 *  Copyright (c)  SBS-Softwaresysteme.de  2009
 *				written By Uwe Lesta
 * 
*********************************************************/

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SbsSW.SwiPlCs;

namespace TestSwiPl
{
    /// <summary>
    /// Summary description for BasePlInit
    /// </summary>
    [TestClass]
    public class BasePlInit
    {
        public BasePlInit()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        static private String[] empty_param = { "-q" };  // suppressing informational and banner messages
        //        static public String[] empty_param = { "-nosignals" };
        //        static public String[] empty_param = { "--quit"};
        static private void InitializePlEngine()
        {
            if (!PlEngine.IsInitialized)
            {
                PlEngine.Initialize(empty_param);
            }
        }

        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        virtual public void MyTestInitialize()
        {
            InitializePlEngine();
        }
        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        virtual public void MyTestCleanup()
        {
            PlEngine.PlCleanup();
        }
        #endregion


        #region helper
        protected int list_length(PlTerm list)
        {
            // PlTerm list_len = new PlTerm();
            PlTerm list_len = PlTerm.PlVar();
            PlTermV args = new PlTermV(list, list_len);
            Assert.IsTrue(PlQuery.PlCall("length", args));
            return (int)args[1];
        }
        #endregion




    }
}
