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
using System.Collections.Generic;		// IEnumerable ( PlQuery )
using SbsSW.DesignByContract;
using SbsSW.SwiPlCs.Exceptions;
using System.Collections.ObjectModel;



// The namespace summary is above class NamespaceDoc
namespace SbsSW.SwiPlCs
{

    #region enum PlQuerySwitch
    /// <summary>
    /// Flags that control  for the foreign predicate parameters 
    /// <para><see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_query()">SWI-Prolog Manual - 9.6.16 Querying Prolog</see>.</para>
    /// </summary>
    /// <seealso cref="PlQuery.Query"/>
    public enum PlQuerySwitch
    {
        /// <summary>The default value.</summary>
        None = 0,
        /// <summary>Return an integer holding the number of arguments given to Prolog from Unix.</summary>
        Argc = LibPl.PL_QUERY_ARGC,
        /// <summary>Return a char ** holding the argument vector given to Prolog from Unix.</summary>
        Argv = LibPl.PL_QUERY_ARGV,
        /// <summary>Read character from terminal.</summary>
        GetChar = LibPl.PL_QUERY_GETC,
        /// <summary>Return a long, representing the maximal integer value represented by a Prolog integer.</summary>
        MaxInteger = LibPl.PL_QUERY_MAX_INTEGER,
        /// <summary>Return a long, representing the minimal integer value.</summary>
        MinInteger = LibPl.PL_QUERY_MIN_INTEGER,
        /// <summary> 	Return a long, representing the version as 10,000 × M + 100 × m + p, where M is the major, m the minor version number and p the patch-level. For example, 20717 means 2.7.17</summary>
        Version = LibPl.PL_QUERY_VERSION,
        /// <summary>Return the maximum number of threads that can be created in this version. Return values of PL_thread_self() are between 0 and this number.</summary>
        MaxThreads = LibPl.PL_QUERY_MAX_THREADS,
        /// <summary>Return the default stream encoding of Prolog (of type IOENC).</summary>
        Encoding = LibPl.PL_QUERY_ENCODING,
        /// <summary>Get amount of user CPU time of the process in milliseconds.</summary>
        UserCpu = LibPl.PL_QUERY_USER_CPU,
    }
    #endregion enum PlQuerySwitch


    /// <summary>
    /// Represents one variable of a Query result.
    /// </summary>
    public class PlQueryVar
    {
        /// <summary>The name of a variable in a Query</summary>
        public string Name { get; internal set; }
        /// <summary>The Value (PlTerm) of a variable in a Query</summary>
        public PlTerm Value { get; internal set; }
        internal PlQueryVar(string name, PlTerm val)
        {
            Name = name;
            Value = val;
        }
    }

    /// <summary>
    /// <para>Represents the set variables of a Query if it was created from a string.</para>
    /// <para>This class is also used to represent the results of a PlQuery after <see cref="PlQuery.ToList()"/> or <see cref="PlQuery.SolutionVariables"/> was called.</para>
    /// </summary>
    /// <example>
    ///     <para>This sample shows both <see cref="PlQuery.Variables"/> is used to unify the variables of two nested queries
    ///     and the result </para>
    ///     <code source="..\..\TestSwiPl\LinqSwiPl.cs" region="compound_nested_query_with_variables_3_doc" />
    /// </example>
    /// <seealso cref="PlQuery.Variables"/>
    public class PlQueryVariables
    {
        private readonly List<PlQueryVar> _vars = new List<PlQueryVar>();

        internal void Add(PlQueryVar var)
        {
            _vars.Add(var);
        }

        /// <summary>
        /// Returns the number of elements in the sequence. (Defined by Enumerable of List&lt;PlQueryVar&gt;.)
        /// </summary>
        public int Count { get { return _vars.Count; } }

        /// <summary>
        /// Gets the <see cref="PlTerm"/> of the given variable name or throw an ArgumentException.
        /// </summary>
        /// <param name="name">The name of the variable</param>
        /// <returns>The PlTerm (value) of the variable </returns>
        /// <exception cref="ArgumentException">Is thrown if the name is not the name of a variable.</exception> 
        public PlTerm this[string name]
        {
            get
            {
                PlQueryVar v = _vars.Find(n1 => n1.Name == name);
                if(v == null)
                    throw new ArgumentException("'" + name + "' is not a variable", "name");
                return v.Value;
            }
        }

        internal PlQueryVar this[int idx]
        {
            get
            {
                return _vars[idx];
            }
        }

    } // class PlQueryVariables




    /********************************
	*	  PlQuery               	*
	********************************/

    #region public class PlQuery
    /// <summary>
    /// <para>This class allows queries to prolog.</para>
    /// <para>A query can be created by a string or by constructing compound terms see <see href="Overload_SbsSW_SwiPlCs_PlQuery__ctor.htm">Constructors</see> for details.</para>
    ///
    /// <para>All resources an terms created by a query are reclaimed by <see cref="Dispose()"/>. It is recommended to build a query in a <see langword="using"/> scope.</para>
    ///
    /// <para>There are four possible opportunities to query Prolog</para>
    /// <list type="table">  
    /// <listheader><term>Query type</term><description>Description </description></listheader>  
    /// <item><term>A <see href="Overload_SbsSW_SwiPlCs_PlQuery_PlCall.htm">static call</see></term><description>To ask prolog for a proof. Return only true or false.</description></item>  
    /// <item><term>A <see cref="PlCallQuery(string)"/></term><description>To get the first result of a goal</description></item>  
    /// <item><term><see href="Overload_SbsSW_SwiPlCs_PlQuery__ctor.htm">Construct</see> a PlQuery object by a string.</term><description>The most convenient way.</description></item>  
    /// <item><term><see href="Overload_SbsSW_SwiPlCs_PlQuery__ctor.htm">Construct</see> a PlQuery object by compound terms.</term><description>The most flexible and fast (runtime) way.</description></item>  
    /// </list>   
    ///
    /// <para>For examples see <see cref="PlQuery(string)"/> and <see cref="PlQuery(string, PlTermV)"/></para>
    /// </summary>
    /// <remarks>
    /// <para>The query will be opened by <see cref="NextSolution()"/> and will be closed if NextSolution() return false.</para>
    /// </remarks>
    public class PlQuery : IDisposable
    {

        #region public members
        /// <summary>
        /// The List of <see cref="PlQueryVariables"/> of this PlQuery.
        /// </summary>
        /// <example>
        ///     <para>In the following example you see how the query Variables can be used to set a variable.</para>
        ///     <code source="..\..\TestSwiPl\LinqSwiPl.cs" region="compound_query_with_variables_doc" />
        ///     <para>Here is a more complex sample where the variables of two queries are connected.</para>
        ///     <code source="..\..\TestSwiPl\LinqSwiPl.cs" region="compound_nested_query_with_variables_doc" />
        /// </example>
        /// <seealso cref="PlQueryVariables"/>
        public PlQueryVariables Variables { get { return _queryVariables; } }

        /// <summary>
        /// Gets a <see cref="Collection&lt;T&gt;"/> of the variable names if the query was built by a string.
        /// </summary>
        public Collection<string> VariableNames
        {
            get
            {
                var sl = new Collection<string>();
                for(int i = 0; i < _queryVariables.Count; i++)
	            {
                    sl.Add(_queryVariables[i].Name);
	            }
                return sl;
            }
        }
        #endregion public members


        #region private members

        private const string ModuleDefault = "user";
        private readonly string _module = ModuleDefault;
        private readonly string _name;
        private uintptr_t _qid;		// <qid_t/>
        private PlTermV _av;	// Argument vector

        /// <summary>the list of prolog record's (the copies to store the variable bindings over backtracking)</summary>
        private readonly List<uintptr_t> _records = new List<uintptr_t>();
        private readonly PlQueryVariables _queryVariables = new PlQueryVariables();

        #endregion private members


        //private static string _queryString;
        //static private long Sread(IntPtr handle, IntPtr buffer, long buffersize)
        //{
        //    byte[] array = Encoding.Unicode.GetBytes(_queryString);
        //    System.Runtime.InteropServices.Marshal.Copy(array, 0, buffer, array.Length);
        //    return array.Length;
        //}

        private void EraseRecords()
        {
            try
            {
                _records.ForEach(LibPl.PL_erase);
                _records.Clear();
            }
            catch (AccessViolationException ex)
            {
                TraceAccessViolationException(ex);
            }
        }

        private static void TraceAccessViolationException(AccessViolationException ex)
        {
            System.Diagnostics.Trace.WriteLine("*****************************************");
            System.Diagnostics.Trace.WriteLine("**  A AccessViolationException occure  **");
            System.Diagnostics.Trace.Write("**  ");
            System.Diagnostics.Trace.WriteLine(ex.Message);
            System.Diagnostics.Trace.WriteLine(ex.StackTrace);
            System.Diagnostics.Trace.WriteLine("*****************************************");
        }

        #region implementing IDisposable
         // <see 'Implementieren der Methoden "Finalize" und "Dispose" zum Bereinigen von nicht verwalteten Ressourcen'/>
         // http://msdn.microsoft.com/de-de/library/b1yfkh5e.aspx
         // <und "Implementieren einer Dispose-Methode"/>
         // http://msdn.microsoft.com/de-de/library/fs2xkftw.aspx

        private bool _disposed;

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Release all resources from the query
        /// </summary>
        /// <param name="disposing">if true all is deleted</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    EraseRecords();
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
                Free(true);
            }
        }
        #endregion


        #region implementing Free
        /// <inheritdoc />
        ~PlQuery()
        {
            Dispose(false);
        }

        /// <summary>
        /// Discards the query, but does not delete any of the data created by the query if discardData is false. 
        /// It just invalidate qid, allowing for a new PlQuery object in this context.
        /// </summary>
        /// <remarks>see <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_cut_query()"/></remarks>
        /// <param name="discardData">if true all bindings of the query are destroyed</param>
        private void Free(bool discardData)
        {
            if (_qid > 0 && PlEngine.IsInitialized)
            {
                try
                {
                    if (discardData)
                        // <"leider werden dann die gebundenen variablen der query wieder frei e.g. in PlCall(goal)"/>
                        // unfortunately this statement detaches the bound variables of the query e.g. in PlCall(goal)
                        LibPl.PL_close_query(_qid);
                    else
                        LibPl.PL_cut_query(_qid);
                }
                catch (AccessViolationException ex)
                {
                    TraceAccessViolationException(ex);
                }
            }
            _qid = 0;
        }
        #endregion Free



        #region constructors

        //private PlQuery()
        //{
        //}

        /// <overloads>
        /// <para>With these constructors a Prolog query can be created but not opened. To get the results see <see cref="NextSolution()"/></para>
        /// <para>A Query can be created from a string or by a name and PlTermV. The later is a native way and available for compatibility.</para>
        /// <para>If a Query is created from a string representing arbitrary prolog text 
        /// the helper classes <see cref="PlQueryVar"/> and <see cref="PlQueryVariables"/> comes into the game.
        /// In this case the most convenient way to get the results is to use <see cref="SolutionVariables"/> or <see cref="ToList()"/>.
        /// </para>
        /// <para>For examples see <see cref="PlQuery(string)"/>.</para>
        /// 
        /// </overloads>
        /// <summary>
        /// <para>With this constructor a query is created from a string.</para>
        /// <para>Uppercase parameters are interpreted a variables but can't be nested in sub terms.
        /// If you need a variable in a nested term use <see cref="PlQuery(string, PlTermV)"/>.
        /// See the examples for details.</para>
        /// </summary>
        /// <remarks>Muddy Waters sang:"I'am build for comfort, I ain't build for speed"</remarks>
        /// <example>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="queryStringForeach_doc" />
        ///     <para>This sample shows a query with two variables.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="queryString2_doc" />
        ///     <para>And the same with named variables.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="queryStringNamed_doc" />
        ///     <para>This sample shows what happens if the argument vector is used with compound terms.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="PlCallQueryCompound_string_doc" />
        ///     <para>And here how to get the results with named variables with compound terms.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="PlCallQueryCompoundNamed_string_doc" />
        ///  </example>
        /// <param name="goal">A string for a prolog query</param>

        public PlQuery(string goal)
            : this(ModuleDefault, goal)
        {
        }

#pragma warning disable 1573
        /// <inheritdoc cref="PlQuery(string)" />
        /// <summary>locating the predicate in the named module.</summary>
        /// <param name="module">locating the predicate in the named module.</param>
        public PlQuery(string module, string goal)
        {
            if(string.IsNullOrEmpty(goal))
                throw new ArgumentNullException("goal");

            _module = module;
            var queryString = goal;
            try
            {
                // call read_term(Term_of_query_string, [variable_names(VN)]).
                // read_term_from_atom('noun(ş,C)', T, [variable_names(Vars)]).
                // befor 2014 with redirected IO-Streams (PlQuery_Old_Kill_unused)
                var atom = new PlTerm("'"+goal.Replace("'", @"\'")+"'");
                PlTerm term = PlTerm.PlVar();
                PlTerm options = PlTerm.PlVar();
                PlTerm variablenames = PlTerm.PlVar();
                PlTerm l = PlTerm.PlTail(options);
                l.Append(PlTerm.PlCompound("variable_names", variablenames));
                l.Close();
                var args = new PlTermV(atom, term, options);
                if (!PlCall("read_term_from_atom", args))
                    throw new PlLibException("PlCall read_term_from_atom/3 fails! goal:" + queryString);

                // set list of variables and variable_names into _queryVariables
                foreach (PlTerm t in variablenames.ToList())
                {
                    // t[0]='=' , t[1]='VN', t[2]=_G123
                    _queryVariables.Add(new PlQueryVar(t[1].ToString(), t[2]));
                }

                // Build the query
                _name = term.Name;

                // is ok e.g. for listing/0.
                // Check.Require(term.Arity > 0, "PlQuery(PlTerm t): t.Arity must be greater than 0."); 
                _av = new PlTermV(term.Arity);
                for (int index = 0; index < term.Arity; index++)
                {
                    if (0 == LibPl.PL_get_arg(index + 1, term.TermRef, _av[index].TermRef))
                        throw new PlException("PL_get_arg in PlQuery " + term.ToString());
                }
            }
#if _DEBUG
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                Console.WriteLine(ex.Message);
            }
#endif
            finally
            {
                // NBT
            }
        }



        //	public PlQuery(const char *Name, const PlTermV &av)
        /// <summary>
        /// Create a query where name defines the name of the predicate and av the argument vector. 
        /// The arity is deduced from av. The predicate is located in the Prolog module user.
        /// </summary>
        /// <example>
        ///     <para>This sample shows a query with a compound term as an argument.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="PlCallQueryCompound_termv_doc" />
        ///  </example>
        /// <param name="name">the name of the predicate</param>
        /// <param name="termV">the argument vector containing the parameters</param>
        public PlQuery(string name, PlTermV termV)
            : this(ModuleDefault, name, termV)
        {
        }

#pragma warning disable 1573
        /// <inheritdoc cref="PlQuery(string,PlTermV)" />
        /// <summary>locating the predicate in the named module.</summary>
        /// <param name="module">locating the predicate in the named module.</param>
        public PlQuery(string module, string name, PlTermV termV)
        {
            //if (null == termV)
            //    throw new ArgumentNullException("termV");

            _av = termV;
            _name = name;
            _module = module;
        }
#pragma warning restore 1573


        #endregion





        /// <summary>Provide access to the Argument vector for the query</summary>
        public PlTermV Args { get { return _av; } }

        /// <summary>
        /// Provide the next solution to the query. Prolog exceptions are mapped to C# exceptions.
        /// </summary>
        /// <returns>return true if successful and false if there are no (more) solutions.</returns>
        /// <remarks>
        /// <para>If the query is closed it will be opened. If the last solution was generated the query will be closed.</para>
        /// <para>If an exception is thrown while parsing (open) the query the _qid is set to zero.</para>
        /// </remarks>
        /// <exception cref="PlException">Is thrown if <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_next_solution()">SWI-Prolog Manual PL_next_solution()</see> returns false </exception>
        public bool NextSolution()
        {
            if (0 == _qid)
            {
                Check.Require(!string.IsNullOrEmpty(_name), "PlQuery.NextSolution() _name is required");

                IntPtr p = LibPl.PL_predicate(_name, _av.Size, _module);
                _qid = LibPl.PL_open_query((IntPtr)0, LibPl.PL_Q_CATCH_EXCEPTION, p, _av.A0);
            }
            int rval = LibPl.PL_next_solution(_qid);
            if (0 == rval)
            {	// error
                uintptr_t ex; // term_t
                if ((ex = LibPl.PL_exception(_qid)) > 0)
                {
                    _qid = 0;   // to avoid an AccessViolationException on Dispose. E.g. if the query is miss spelled.
                    var etmp = new PlException(new PlTerm(ex));
                    etmp.Throw();
                }
            }
            if(rval <= 0)
                Free(false);

            return rval > 0;
        }

        /// <summary>
        /// <para>Enumerate the solutions.</para>
        /// <para>For examples see <see cref="PlQuery(string)"/></para>
        /// </summary>
        /// <seealso cref="NextSolution"/>
        /// <seealso href="Overload_SbsSW_SwiPlCs_PlQuery__ctor.htm">Constructors</seealso>
        public IEnumerable<PlTermV> Solutions
        {
            get
            {
                while (NextSolution())
                {
                    yield return _av;
                }
            }
        }

        /// <summary>
        /// <para>Enumerate the <see cref="PlQueryVariables"/> of one solution.</para>
        /// </summary>
        /// <example>
        ///     <code source="..\..\TestSwiPl\LinqSwiPl.cs" region="compound_query_SolutionVariables_doc" />
        /// </example>
        /// <seealso cref="NextSolution"/>
        public IEnumerable<PlQueryVariables> SolutionVariables
        {
            get
            {
                while (NextSolution())
                {
                    var qv = new PlQueryVariables();
                    for (int i = 0; i < _queryVariables.Count; i++)
                    {
                        qv.Add(new PlQueryVar(_queryVariables[i].Name, _queryVariables[i].Value));
                    }
                    yield return qv;
                }
            }
        }


        /// <summary>
        /// <para>Create a <see cref="ReadOnlyCollection&lt;T&gt;"/> of <see cref="PlQueryVariables"/>.</para>
        /// <para>If calling ToList() all solutions of the query are generated and stored in the Collection.</para>
        /// </summary>
        /// <returns>A ReadOnlyCollection of PlQueryVariables containing all solutions of the query.</returns>
        /// <example>
        ///     <code source="..\..\TestSwiPl\LinqSwiPl.cs" region="Test_multi_goal_ToList_doc" />
        /// </example>
        public ReadOnlyCollection<PlQueryVariables> ToList()
        {
            var list = new List<PlQueryVariables>();
            EraseRecords();
            while (NextSolution())
            {
                for (int i = 0; i < _queryVariables.Count; i++ )
                {
                    _records.Add(LibPl.PL_record(_queryVariables[i].Value.TermRef));   // to keep the PlTerms
                }
            }

            var qv = new PlQueryVariables(); // dummy to make the compiler happy
            int avIdx = _queryVariables.Count;
            foreach (uintptr_t recordTerm in _records)
            {
                var ptrTerm = LibPl.PL_new_term_ref(); 
                LibPl.PL_recorded(recordTerm, ptrTerm);
                if (avIdx == _queryVariables.Count)
                {
                    qv = new PlQueryVariables();
                    list.Add(qv);
                    avIdx = 0;
                }
                //qv.Add(new PlQueryVar(GetVariableName(avIdx), new PlTerm(term)));   // If this line is deleted -> update comment in PlTern(term_ref)
                qv.Add(new PlQueryVar(_queryVariables[avIdx].Name, new PlTerm(ptrTerm)));   // If this line is deleted -> update comment in PlTern(term_ref)
                avIdx++;
                //av[avIdx++].TermRef = term_t;
            }
            return new ReadOnlyCollection<PlQueryVariables>(list);
        }



        #region static PlCall




    
        /// <summary>
        /// <para>Obtain status information on the Prolog system. The actual argument type depends on the information required. 
        /// The parameter queryType describes what information is wanted.</para>
        /// <para>Returning pointers and integers as a long is bad style. The signature of this function should be changed.</para>
        /// <see>PlQuerySwitch</see>
        /// </summary>
        /// <example>
        ///     <para>This sample shows how to get SWI-Prologs version number</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="get_prolog_version_number_doc" />
        /// </example>
        /// <param name="queryType">A <see>PlQuerySwitch</see>.</param>
        /// <returns>A int depending on the given queryType</returns>
        public static long Query(PlQuerySwitch queryType)
        {
            Check.Require(queryType != PlQuerySwitch.None, "PlQuerySwitch (None) is not valid");
            return LibPl.PL_query((uint)queryType);
        }


        /// <overloads>
        /// The main purpose of the static PlCall methods is to call a prolog prove or to do some site effects.
        /// <example>
        /// <code>
        ///      Assert.IsTrue(PlQuery.PlCall("is_list", new PlTerm("[a,b,c,d]")));
        /// </code>
        /// <code>
        ///      Assert.IsTrue(PlQuery.PlCall("consult", new PlTerm("some_file_name")));
        ///      // or
        ///      Assert.IsTrue(PlQuery.PlCall("consult('some_file_name')"));
        /// </code>
        /// </example>
        /// </overloads>
        /// <inheritdoc cref="PlQuery(string,PlTermV)" />
        /// <remarks>
        /// <para>Create a PlQuery from the arguments, generates the first solution by NextSolution() and destroys the query.</para>
        /// </remarks>
        /// <param name="predicate">defines the name of the predicate</param>
        /// <param name="args">Is a <see cref="PlTermV"/> of arguments for the predicate</param>
        /// <returns>Return true or false as the result of NextSolution() or throw an exception.</returns>
        public static bool PlCall(string predicate, PlTermV args)
        {
            return PlCall(ModuleDefault, predicate, args);
        }
#pragma warning disable 1573
// Parameter 'predicate' has no matching param tag in the XML comment for 'SbsSW.SwiPlCs.PlQuery.PlCall(string, string, SbsSW.SwiPlCs.PlTermV)' (but other parameters do)
        /// <inheritdoc cref="PlCall(string, PlTermV)" />
        /// <summary>As <see cref="PlCall(string, PlTermV)"/> but locating the predicate in the named module.</summary>
        /// <param name="module">locating the predicate in the named module.</param>
        public static bool PlCall(string module, string predicate, PlTermV args)
        {
            bool bRet;
            using (var q = new PlQuery(module, predicate, args))
            {
                bRet = q.NextSolution();
                q.Free(false);
            }
            return bRet;
        }
#pragma warning restore 1573
        /// <inheritdoc cref="PlCall(string, PlTermV)" />
        /// <summary>Call a goal once.</summary>
        /// <example>
        /// <code>
        ///      Assert.IsTrue(PlQuery.PlCall("is_list([a,b,c,d])"));
        /// </code>
        /// <code>
        ///      Assert.IsTrue(PlQuery.PlCall("consult('some_file_name')"));
        /// </code>
        /// </example>
        /// <param name="goal">The complete goal as a string</param>
        public static bool PlCall(string goal)
        {
            bool bRet;
            using (var q = new PlQuery("call", new PlTermV(new PlTerm(goal))))
            {
                bRet = q.NextSolution();
                q.Free(true);
            }
            return bRet;
        }

        #endregion


        #region PlCallQuery

        /// <summary>
        /// return the solution of a query which is called once by call PlQuery(goal)
        /// </summary>
        /// <param name="goal">a goal with *one* variable</param>
        /// <returns>the bound variable of the first solution</returns>
        /// <exception cref="ArgumentException">Throw an ArgumentException if there is no or more than one variable the goal.</exception>
        /// <example>
        ///     <para>This sample shows simple unifikation.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="PlCallQuery_direct_1_doc" />
        /// 
        ///     <para>This sample shows how a simple calculation can be done by a predicate.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="PlCallQuery_direct_1_doc" />
        /// 
        ///     <para>This sample shows both how to get the working_directory from SWI-Prolog.</para>
        ///     <code source="..\..\TestSwiPl\PlQuery.cs" region="PlCallQuery_direct_3_doc" />
        /// </example>
        public static PlTerm PlCallQuery(string goal)
        {
            return PlCallQuery(ModuleDefault, goal);
        }

#pragma warning disable 1573
        /// <inheritdoc cref="PlCallQuery(System.String)" />
        /// <summary>As <see cref="PlCallQuery(string)"/> but executed in the named module.</summary>
        /// <param name="module">The modulename in which the query is executed</param>
        public static PlTerm PlCallQuery(string module, string goal)
        {
            PlTerm retVal;
            using (var q = new PlQuery(module, goal))
            {
                // find the variable or throw an exception
                PlTerm? t = null;
                if (q.Variables.Count == 1)
                {
                    t = new PlTerm(q.Variables[0].Value.TermRef);
                }
                else
                {
                    for (int i = 0; i < q._av.Size; i++)
                    {
                        if (!q._av[i].IsVar) continue;
                        if (t == null)
                        {
                            t = new PlTerm(q._av[i].TermRef);
                        }
                        else
                            throw new ArgumentException("More than one Variable in " + goal);
                    }
                }
                if (t == null)
                    throw new ArgumentException("No Variable found in " + goal);

                if (q.NextSolution())
                {
                    retVal = (PlTerm)t;
                }
                else
                    retVal = new PlTerm();    // null
                q.Free(false);
            }
            return retVal;
        }
#pragma warning restore 1573

        #endregion



    } // class PlQuery
    #endregion



} // namespace SbsSW.SwiPlCs
