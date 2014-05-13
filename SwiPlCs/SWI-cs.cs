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

/*
 * XML comments see:
 * 
 * ++ http://thoughtpad.net/alan-dean/cs-xml-documentation.html
 * http://www.microsoft.com/downloads/details.aspx?familyid=E82EA71D-DA89-42EE-A715-696E3A4873B2&displaylang=en
 * http://www.codeplex.com/Wiki/View.aspx?ProjectName=SHFB
 * 
 * 
 * German : http://www.galileocomputing.de/openbook/visual_csharp/visual_csharp_06_005.htm#mje2e442064f253d6adeaa29bb3ca57032
 * 
 * nice example of the formatting possibilities 
 *      "Documenting C# source code with XML Comments"  http://www.winnershtriangle.com/w/Articles.XMLCommentsInCSharp.asp
 * see also 
 *      "C# and XML Source Code Documentation"  http://www.codeproject.com/KB/XML/csharpcodedocumentation.aspx
 * 
 * http://www.codeplex.com/SHFB
 * http://www.codeplex.com/DocProject
 * 
 * <code>
<!--
 * for a good spellchecker tool see Mikhail Arkhipov's blog at
 * http://blogs.msdn.com/mikhailarkhipov/archive/2006/04/17/577471.aspx
 * the new version is at
 * http://visualstudiogallery.msdn.microsoft.com/0db4814c-255e-4cc6-a2c2-a428de7f8949/
-->
 * </code>
 * IntelliSense: copy XML file (bin/SwiPlCs.XML) to beside SwiPLCs.dll
 */

/*
 * Changes
 * 
 * 08.05.14 CHANGED Framework version 3.5 
 *          FIX Marshalling 
 * 
 * 02.11.29 ADDED static PlCall in Class PlQuery
 *			FIXED NextSolution throw PlException
 *			ADDED PlString, PlCodeList, PlCharList  
 * 02.12.08 ADDED Class PlTail to support LISTS
 * 
 * 03.08.16 ADDED to DllImport - PL_is_initialised  
 *				PL_create_engine, PlSetEngine, PL_destroy_engine
 *          ADDED to PlEngine - IsInitialized  
 *          ADDED Class PlMtEngine
 */

using System;
using System.Collections;		        // IEnumerable PlTail
using System.Collections.Generic;		// IEnumerable ( PlQuery )
using System.Text;						// ToStringAsListFormat
using SbsSW.DesignByContract;
using SbsSW.SwiPlCs.Exceptions;
using SbsSW.SwiPlCs.Streams;
using System.Collections.ObjectModel;


/********************************
*	       TYPES	Comment     *
********************************/
#region used type from SWI-Prolog.h

/*
<!-- 
#ifdef _PL_INCLUDE_H
	typedef module		module_t;	// atom module 
	typedef Procedure	predicate_t;// atom predicate handle
	typedef Record		record_t;	// handle to atom recorded term
#else
typedef	unsigned long	atom_t;		// Prolog atom
typedef void *		    module_t;		// Prolog module
typedef void *		    predicate_t;	// Prolog procedure
typedef void *		    record_t;		// Prolog recorded term
typedef unsigned long	term_t;		// opaque term handle
typedef unsigned long	qid_t;		// opaque query handle
typedef unsigned long	PL_fid_t;	// opaque foreign context handle
#endif
typedef unsigned long	functor_t;	// Name/Arity pair
typedef unsigned long	atomic_t;	// same atom word
typedef unsigned long	control_t;	// non-deterministic control arg
typedef void *		    PL_engine_t;	// opaque engine handle 
typedef unsigned long	foreign_t;	// return type of foreign functions

unsigned long	-  uint
-->
*/

#endregion

// The namespace summary is above class NamespaceDoc
namespace SbsSW.SwiPlCs.Callback
{

    #region namspace documentation
    /// <summary>
    /// <para>The namespace SbsSW.SwiPlCs.Callback provides the delegates to register .NET methods to be 
    /// called from <a href='http://www.swi-prolog.org' >SWI-Prolog</a>
    /// </para>
    /// </summary>
    /// <remarks>
    /// <note>It is only possible to call <see langword="static"/> methods</note>
    /// </remarks>
    /// <seealso cref="M:SbsSW.SwiPlCs.PlEngine.RegisterForeign(string, System.Delegate)"/>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    class NamespaceDoc
    {
    }
    #endregion namspace documentation


    #region enum PlForeignSwitches
    /// <summary>
    /// Flags that are responsible for the foreign predicate parameters 
    /// </summary>
    [FlagsAttribute]
    public enum PlForeignSwitches
    {
        /// <summary>0 - PL_FA_NOTHING: no flags. </summary>
        None = 0,
        /// <summary>1 - PL_FA_NOTRACE: Predicate cannot be seen in the tracer. </summary>
        NoTrace = 1,
        /// <summary>2 - PL_FA_TRANSPARENT: Predicate is module transparent.</summary>
        Transparent = 2,
        /// <summary>4 - PL_FA_NONDETERMINISTIC: Predicate is non-deterministic. See also PL_retry().</summary>
        /// <seealso href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_retry()">PL_retry()</seealso>
        Nondeterministic = 4,
        /// <summary>8 - PL_FA_VARARGS: (Default) Use alternative calling convention.</summary>
        VarArgs = 8,
    }
    #endregion enum PlForeignSwitches


    #region delagates for C# callbacks
    /// <inheritdoc cref="DelegateParameter2" />
    /// 
    //[System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool DelegateParameter0();


    /// <inheritdoc cref="DelegateParameter2" />
    /// <example>
    /// <para>See also the example in <see cref="DelegateParameter2" />.</para>
    ///     <code source="..\..\TestSwiPl\CallbackForeigenPredicate.cs" region="t_creating_a_list_doc" />
    /// </example>
    /// <param name="term"></param>
    //[System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool DelegateParameter1(PlTerm term);


    /// <summary>
    /// <para>Provide a predefined Delegate to register a C# method to be called from SWI-Prolog</para>
    /// </summary>
    /// <example>
    /// <para>This example is for <see cref="DelegateParameter2" /> and shows how o call a C# method with two parameter.</para>
    /// <para>For other samples see the source file CallbackForeigenPredicate.cs in the TestSwiPl VS2008 test project.</para>
    ///     <code source="..\..\TestSwiPl\CallbackForeigenPredicate.cs" region="t_in_out_doc" />
    /// </example>
    /// <param name="term1"></param>
    /// <param name="term2"></param>
    /// <returns>true for succeeding otherwise false for fail</returns>
    /// <seealso cref="M:SbsSW.SwiPlCs.PlEngine.RegisterForeign(System.Delegate)"/>
    /// <seealso cref="T:SbsSW.SwiPlCs.PlEngine"/>
    //[System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool DelegateParameter2(PlTerm term1, PlTerm term2);

    /// <inheritdoc cref="DelegateParameter2" />
    /// <param name="term1"></param>
    /// <param name="term2"></param>
    /// <param name="term3"></param>
    //[System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate bool DelegateParameter3(PlTerm term1, PlTerm term2, PlTerm term3);


    /// <summary>
    /// <para>With this delegate you can build a call-back predicate with a variable amount of parameters.</para>
    /// <para>This delegate is merkes as Obsolete. DelegateParameterVarArgs is deprecated, please use DelegateParameter1 to 3 instead.</para>
    /// </summary>
    /// <example>
    ///     <code source="..\..\TestSwiPl\CallbackForeigenPredicate.cs" region="t_varargs_doc" />
    /// </example>
    /// <param name="termVector">The termVector representing the arguments which can be accessed by the 
    /// indexer of PlTermV see <see cref="PlTermV"/>. The amount of parameters is in <see cref="PlTermV.Size"/>
    /// </param>
    /// <returns>True for succeeding otherwise false for fail</returns>
    /// <remarks>
    /// <para> TODO: This do *NOT* work on 64-Bit systems. Hope to Fix this in the future.</para>
    /// <para>
    /// It seems to be impossible to marshal two parameter which are bigger than 8 byte 
    /// into one struct. Perhaps there is a way in CLI :-(
    /// </para>
    /// The problem are the parameters of the call back method. These are in C
    /// <c>foreign_t (f)(term_t t0, int a, control_t ctx))</c> (see SWI-cpp.h)
    /// If we provide 
    /// <c>DelegateParameterVarArgs(PlTerm term, int arity);</c>
    /// and do in the callback Method something like
    /// <code>
    /// public static bool my_call_back(PlTerm term, int arity)
    /// {
    ///      PlTermV args = new PlTermV(term, arity);   // This constructor do *not* exist
    /// }
    /// </code>
    /// every thing work fine. The drawback is this ugly ctor. It might be better to do
    /// <c>PlTermV args = PlTermV.VarArgs(term, arity);</c>
    /// with a strong recommendation to use it *OINLY* in this call back scenario.
    /// </remarks>
    //[System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    [Obsolete("DelegateParameterVarArgs is deprecated, please use DelegateParameter1 to 3 instead.")]
    public delegate bool DelegateParameterVarArgs(PlTermV termVector);


    /// <summary>
    /// <para><b>NOT IMPLEMENTED YET</b></para>
    /// <para>For details to implement see <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_register_foreign_in_module()">9.6.17 Registering Foreign Predicates</see></para>
    /// see also <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_foreign_control()">PL_foreign_control</see>
    /// </summary>
    /// <param name="term1">TODO</param>
    /// <param name="term2">TODO</param>
    /// <param name="control">TODO</param>
    /// <returns>TODO</returns>
    /// <example>TODO
    /// <para>See "t_backtrack" in TestSwiPl.CallbackForeigenPredicate.cs</para>
    /// </example>
    //[System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate int DelegateParameterBacktrack(PlTerm term1, PlTerm term2, IntPtr control);

    #endregion delagates for C# callbacks

} // namespace SbsSW.SwiPlCs.Callback



namespace SbsSW.SwiPlCs.Streams
{

    #region namspace documentation
    /// <summary>
    /// <para>The namespace SbsSW.SwiPlCs.Streams provides the delegates to redirect the read 
    /// and write functions of the <a href='http://www.swi-prolog.org' >SWI-Prolog</a> IO Streams.</para>
    /// <para>When <see cref="PlEngine.Initialize"/> is called the *Sinput->functions.read is 
    /// replaced by the .NET method 'Sread_function' and *Sinput->functions.write by 'Swrite_funktion'.</para>
    /// <para>For further examples see the methods <see cref="PlEngine.SetStreamFunctionWrite"/> and <see cref="PlEngine.SetStreamFunctionRead"/></para>
    /// </summary>
    /// <remarks>
    /// <note>The reason for this is debugging.</note>
    /// </remarks>
    /// <example>
    /// <code source="..\swi-cs.cs" region="default_io_doc" />
    /// </example>
    /// <seealso cref="PlEngine.SetStreamFunctionRead"/>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    class NamespaceDoc
    {
    }
    #endregion namspace documentation



    /// <summary>
    /// The standard SWI-Prolog streams ( input output error )
    /// </summary>
    public enum PlStreamType
    {
        /// <summary>0 - The standard input stream.</summary>
        Input = 0,
        /// <summary>1 - The standard input stream.</summary>
        Output = 1,
        /// <summary>1 - The standard error stream.</summary>
        Error = 2
    }


    /// <summary>
    /// See <see cref="PlEngine.SetStreamFunctionRead"/>
    /// </summary>
    /// <param name="handle">A C stream handle. simple ignore it.</param>
    /// <param name="buffer">A pointer to a string buffer</param>
    /// <param name="bufferSize">The size of the string buffer</param>
    /// <returns>A <see cref="System.Delegate"/></returns>
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate long DelegateStreamReadFunction(IntPtr handle, IntPtr buffer, long bufferSize);

    /// <summary>
    /// See <see cref="PlEngine.SetStreamFunctionWrite"/>
    /// </summary>
    /// <param name="handle">A C stream handle. simple ignore it.</param>
    /// <param name="buffer">A pointer to a string buffer</param>
    /// <param name="bufferSize">The size of the string buffer</param>
    /// <returns>A <see cref="System.Delegate"/></returns>
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate long DelegateStreamWriteFunction(IntPtr handle, string buffer, long bufferSize);

    /*
    <!--
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate long Sseek_function(IntPtr handle, long pos, int whence);
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate Int64 Sseek64_function(IntPtr handle, Int64 pos, int whence);
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate int Sclose_function(IntPtr handle);
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate int Scontrol_function(IntPtr handle, int action, IntPtr arg);


    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MIOFUNCTIONS
    {
        Sread_function read;		//* fill the buffer
        DelegateStreamWriteFunction write;		//* empty the buffer 
        Sseek_function seek;		//* seek to position 
        Sclose_function close;		//* close stream 
        Scontrol_function control;	//* Info/control 
        Sseek64_function seek64;		//* seek to position (intptr_t files) 
    };
    
     -->
     */

}


// The namespace summary is above class NamespaceDoc
namespace SbsSW.SwiPlCs
{

    #region namspace documentation
    /// <summary>
    /// <para>The online documentation home is <a href='http://www.lesta.de/prolog/swiplcs/Generated/Index.aspx'>here</a>.</para>
    /// <para>This namespace SbsSW.SwiPlCs provides an .NET interface to <a href='http://www.swi-prolog.org' >SWI-Prolog</a></para>
    /// 
    /// 
    /// <para><h4>Overview</h4></para>
    /// <para>Prolog variables are dynamically typed and all information is passed around using the 
    /// C-interface type term_t witch is an int. In C#, term_t is embedded in the lightweight struct <see cref="PlTerm"/>. 
    /// Constructors and operator definitions provide flexible operations and integration with important C#-types (<c>string, int and double</c>).
    /// </para>
    /// 
    /// <para>The list below summarises the important classes / struct defined in the C# interface.</para>
    /// <list type="table">  
    /// <listheader><term>class / struct</term><description>Short description </description></listheader>  
    /// <item><term><see cref="T:SbsSW.SwiPlCs.PlEngine"/></term><description>A static class represents the prolog engine.</description></item>  
    /// <item><term><see cref="T:SbsSW.SwiPlCs.PlTerm"/></term><description>A struct representing prolog data.</description></item>  
    /// <item><term><see cref="T:SbsSW.SwiPlCs.PlTermV"/></term><description>A vector of <see cref="PlTerm"/>.</description></item>  
    /// <item><term><see cref="T:SbsSW.SwiPlCs.PlQuery"/></term><description>A class to query Prolog.</description></item>  
    /// </list>   
    /// </summary>
    /// 
    /// <example>
    /// <para>Before going into a detailed description of the CSharp classes let me present a few examples 
    /// illustrating the `feel' of the interface. The Assert class in the sample is from the test framework 
    /// and has nothing to do with the interface. It shows only which return values are expected.
    /// </para>
    /// <h4>Creating terms</h4>
    /// <para>This very simple example shows the basic creation of a Prolog term and how a Prolog term is converted to C#-data:</para>
    /// <code>
    ///     PlTerm t1 = new PlTerm("x(A)"); 
    ///     PlTerm t2 = new PlTerm("x(1)"); 
    ///     Assert.IsTrue(t1.Unify(t2));
    ///     Assert.AreEqual("x(1)", t1.ToString());
    /// </code>
    /// 
    /// <h4>Calling Prolog</h4>
    /// <para>This example shows how to make a simple call to prolog.</para>
    /// <code>
    ///     PlTerm l1 = new PlTerm("[a,b,c,d]");
    ///     Assert.IsTrue(PlQuery.PlCall("is_list", l1));
    /// </code>
    /// 
    /// <h4>Getting the solutions of a query</h4>
    /// <para>This example shows how to obtain all solutions of a prolog query.</para>
    /// <para><see cref="T:SbsSW.SwiPlCs.PlQuery"/> takes the name of a predicate and the goal-argument vector as arguments.
    /// From this information it deduces the arity and locates the predicate. the member-function 
    /// NextSolution() yields true if there was a solution and false otherwise. 
    /// If the goal yielded a Prolog exception it is mapped into a C# exception.</para>
    /// <code>
    ///     PlQuery q = new PlQuery("member", new PlTermV(new PlTerm("A"), new PlTerm("[a,b,c]")));
    ///     while (q.NextSolution())
    ///         Console.WriteLine(s[0].ToString());
    /// </code>
    /// <para>There is an other constructor of <see cref="T:SbsSW.SwiPlCs.PlQuery"/> which simplify the sample above.</para>
    /// <code>
    ///     PlQuery q = new PlQuery("member(A, [a,b,c])");
    ///     foreach (PlTermV s in q.Solutions)
    ///         Console.WriteLine(s[0].ToString());
    /// </code>
    /// <para>An other way to get the results is to use <see cref="PlQuery.SolutionVariables"/> to iterate over <see cref="PlQueryVariables"/>.</para>
    /// <code>
    ///     PlQuery q = new PlQuery("member(A, [a,b,c])");
    ///     foreach (PlQueryVariables vars in q.SolutionVariables)
    ///         Console.WriteLine(vars["A"].ToString());
    /// </code>
    /// <para>It is also possible to get all solutions in a list by <see cref="PlQuery.ToList()"/>. 
    /// This could be used to work with LinQ to objects which is really nice. <see cref="T:SbsSW.SwiPlCs.PlQuery"/> and <see cref="PlQuery.ToList()"/> for further samples.</para>
    /// <code>
    ///     var results = from n in new PlQuery("member(A, [a,b,c])").ToList() select new {A = n["A"].ToString()};
    ///     foreach (var s in results)
    ///         Console.WriteLine(s.A);
    /// </code>
    /// </example>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute]
    class NamespaceDoc
    {
    }
    #endregion namspace documentation

    
    /// <summary>
    /// Obtain the type of a term, which should be a term returned by one of the other 
    /// interface predicates or passed as an argument. The function returns the type of 
    /// the Prolog term. The type identifiers are listed below. 
    /// </summary>
    /// <remarks>see <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_term_type()">PL_term_type(term_t)</see> in the SWI-Prolog Manual.</remarks>
    /// <seealso cref="P:SbsSW.SwiPlCs.PlTerm.PlType"/>
    /// <example>
    /// In this sample a Prolog variable is created in <see cref="PlTerm">PlTerm t</see> and the <see cref="P:SbsSW.SwiPlCs.PlTerm.PlType"/> 
    /// is checked by his integer representation and his name.
    /// <code>
    ///     PlTerm t = PlTerm.PlVar();
    ///     Assert.AreEqual(1, (int)t.PlType);
    ///     Assert.AreEqual(PlType.PlVariable, t.PlType);
    /// </code>
    /// </example>
    public enum PlType
    {
        /// <summary>0 - PL_UNKNOWN: Undefined </summary>
        PlUnknown = 0,
        /// <summary>1 - PL_VARIABLE: An unbound variable. The value of term as such is a unique identifier for the variable.</summary>
        PlVariable = 1,
        /// <summary>2 - PL_ATOM: A Prolog atom.</summary>
        PlAtom = 2,
        /// <summary>3 - PL_INTEGER: A Prolog integer.</summary>
        PlInteger = 3,
        /// <summary>4 - PL_FLOAT: A Prolog floating point number.</summary>
        PlFloat = 4,
        /// <summary>5 - PL_STRING: A Prolog string.</summary>
        PlString = 5,
        /// <summary>6 - PL_TERM: A compound term. Note that a list is a compound term ./2.</summary>
        PlTerm = 6,

        /// <summary>14 - PL_CODE_LIST: [ascii...].</summary>
        PlCodeList = 14,
        /// <summary>15 - PL_CHAR_LIST: [h,e,l,l,o].</summary>
        PlCharList = 15,
    }


    /********************************
    *     GENERIC PROLOG TERM		*
    ********************************/
    #region public struct PlTerm
#pragma warning disable 1574
    // warning CS1574: XML comment on 'SbsSW.SwiPlCs.PlTerm' has cref attribute 'System.Linq' that 
    // SwiPlCs need no assambly reference to Linq

    /// <summary>
    ///  <para>The PlTerm <see langword="struct"/> plays a central role in conversion and operating on Prolog data.</para>
    ///  <para>PlTerm implements <see cref="System.IComparable"/> to support ordering in <see cref="T:System.Linq"/> queries if PlTerm is a List.
    /// see <see cref="PlTerm.Append"/> for examples.
    /// </para>
    ///  <para>Creating a PlTerm can be done by the <see href="Overload_SbsSW_SwiPlCs_PlTerm__ctor.htm">Constructors</see> or by the following static methods:</para>
    ///  <para>PlVar(), PlTail(), PlCompound, PlString(), PlCodeList(), PlCharList() (see remarks)</para>
    /// </summary>
    /// <remarks>
    /// <list type="table">  
    /// <listheader><term>static method</term><description>Description </description></listheader>  
    /// <item><term><see cref="PlVar()"/></term><description>Creates a new initialised term (holding a Prolog variable).</description></item>  
    /// <item><term><see cref="PlTail(PlTerm)"/></term><description>PlTail is for analysing and constructing lists.</description></item>  
    /// <item><term><see href="Overload_SbsSW_SwiPlCs_PlTerm_PlCompound.htm">PlCompound(string)</see></term><description>Create compound terms. E.g. by parsing (as read/1) the given text.</description></item>  
    /// <item><term><see cref="PlString(string)"/></term><description>Create a SWI-Prolog string.</description></item>  
    /// <item><term><see cref="PlCodeList(string)"/></term><description>Create a Prolog list of ASCII codes from a 0-terminated C-string.</description></item>  
    /// <item><term><see cref="PlCharList(string)"/></term><description>Create a Prolog list of one-character atoms from a 0-terminated C-string.</description></item>  
    /// </list>   
    /// </remarks>
#pragma warning restore 1574
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public struct PlTerm : IComparable, IEnumerable<PlTerm>// TODO, IList<PlTerm> // LISTS
    {
        private uintptr_t _termRef; 

        // Properties
        internal uintptr_t TermRef
        {
            get
            {
                Check.Require(_termRef != 0, "use of an uninitialized PlTerm. If you need a variable use PlTerm.PlVar() instead");
                return _termRef;
            }
        }


        #region implementing IComparable CompareTo

        ///<inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj is PlTerm)
                return LibPl.PL_compare(TermRef, ((PlTerm)obj).TermRef);
            throw new ArgumentException("object is not a PlTerm");
        }

        #endregion

        /// <summary>
        /// <para>If PlTerm is compound and index is between 0 and Arity (including), the nth PlTerm is returned.</para>
        /// <para>If pos is 0 the functor of the term is returned (For a list '.').</para>
        /// <para>See: <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual//foreigninclude.html#PL_get_arg()">PL_get_arg/3</see></para>
        /// </summary>
        /// <param name="position">To Get the nth PlTerm</param>
        /// <returns>a PlTerm</returns>
        /// <example>
        ///     <code source="..\..\TestSwiPl\PlTerm.cs" region="PlTerm_indexer_doc" />
        /// </example>
        /// <exception cref="NotSupportedException">Is thrown if PlTerm is not of Type PlCompound see <see cref="IsCompound"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown if (pos &lt;  0 || pos >= Arity)</exception>
        /// <exception cref="InvalidOperationException">Is thrown if PL_get_arg returns 0.</exception>
        public PlTerm this[int position]
        {
            get
            {
                if(!IsCompound)
                    throw new NotSupportedException("Work only for compound terms!");
                
                if(position < 0 || position > Arity )
                    throw new ArgumentOutOfRangeException("position", "Must be greater than 0 and lesser then the arity of the term");

                if (0 == position)
                {
                    return IsList ? new PlTerm("'.'") : new PlTerm(Name);
                }
                uintptr_t a = LibPl.PL_new_term_ref();
                if (0 != LibPl.PL_get_arg(position, TermRef, a))
                    return new PlTerm(a);
                throw new InvalidOperationException("PlTerm indexer: PL_get_arg return 0");
            }
            //set
            //{
            //    myData[pos] = value;
            //}
        }


        #region constructors


        // NOTE : Be Careful you *can* delete this constructor or make it private 
        //        it compiles but the tests will fail
        /// <summary>
        /// Create a PlTerm but *no* new term_ref it only copies the term_ref into the new object
        /// Used Intern by
        /// - PlException constructor
        /// - PlQueryQ.GetSolutions()
        /// - PlTermV this[int index] indexer
        /// </summary>
        /// <param name="termRef"></param>
        internal PlTerm(uintptr_t termRef)
        {
            _termRef = termRef;
        }


        /// <overloads>
        /// <summary>
        /// A new PlTerm can be also created by the static methods:
        /// <list type="table">  
        /// <listheader><term>static method</term><description>Description </description></listheader>  
        /// <item><term><see cref="PlVar()"/></term><description>Creates a new initialised term (holding a Prolog variable).</description></item>  
        /// <item><term><see cref="PlTail(PlTerm)"/></term><description>PlTail is for analysing and constructing lists.</description></item>  
        /// <item><term><see href="Overload_SbsSW_SwiPlCs_PlTerm_PlCompound.htm">PlCompound(string)</see></term><description>Create compound terms. E.g. by parsing (as read/1) the given text.</description></item>  
        /// <item><term><see cref="PlString(string)"/></term><description>Create a SWI-Prolog string.</description></item>  
        /// <item><term><see cref="PlCodeList(string)"/></term><description>Create a Prolog list of ASCII codes from a 0-terminated C-string.</description></item>  
        /// <item><term><see cref="PlCharList(string)"/></term><description>Create a Prolog list of one-character atoms from a 0-terminated C-string.</description></item>  
        /// </list>   
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Creates a term-references holding a Prolog term representing text.
        /// </summary>
        /// <param name="text">the text</param>
        public PlTerm(string text)
        {
            if(string.IsNullOrEmpty(text))
                text = "''";

            uintptr_t t = LibPl.PL_new_term_ref();

            if (0 == LibPl.PL_wchars_to_term(text, t))
                throw new PlException(new PlTerm(t));

            _termRef = LibPl.PL_new_term_ref();
            LibPl.PL_put_term(TermRef, t);
        }
        /// <summary>
        /// Creates a term-references holding a Prolog integer representing value.
        /// </summary>
        /// <param name="value">a integer value</param>
        public PlTerm(int value)
        {
            _termRef = LibPl.PL_new_term_ref();
            LibPl.PL_put_integer(TermRef, value);
        }
        /// <summary>
        /// Creates a term-references holding a Prolog float representing value.
        /// </summary>
        /// <param name="value">a double value</param>
        public PlTerm(double value)
        {
            _termRef = LibPl.PL_new_term_ref();
            LibPl.PL_put_float(TermRef, value);
        }

        #endregion


     /***************************************
	 *	    SPECIALISED TERM CREATION		*
     *	    as static methods               *
	 ***************************************/

        #region PlVar Creation

        /// <summary>
        /// Creates a new initialised term (holding a Prolog variable).
        /// </summary>
        /// <returns>a PlTerm</returns>
        static public PlTerm PlVar()
        {
            return new PlTerm { _termRef = LibPl.PL_new_term_ref() }; 
        }
        #endregion

        #region PlList Creation
        /// <summary>
        /// <para>
        /// PlTail is for analysing and constructing lists. 
        /// It is called PlTail as enumeration-steps make the term-reference follow the `tail' of the list.
        /// </para>
        /// <para>
        /// A PlTail is created by making a new term-reference pointing to the same object. 
        /// As PlTail is used to enumerate or build a Prolog list, the initial list 
        /// term-reference keeps pointing to the head of the list.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="Append(PlTerm)" select="example"/>
        /// <param name="list">The initial PlTerm</param>
        /// <returns>A PlTerm for which is_list/1 succeed.</returns>
        /// <seealso cref="Append(PlTerm)"/>
        /// <seealso cref="Add(PlTerm)"/>
        /// <seealso cref="AddList(PlTerm)"/>
        /// <seealso cref="Close()"/>
        /// <seealso cref="NextValue()"/>
        static public PlTerm PlTail(PlTerm list)
        {
            Check.Require(list != null);
            Check.Require(list.IsList || list.IsVar);

            var term = new PlTerm();
            if (0 != LibPl.PL_is_variable(list.TermRef) || 0 != LibPl.PL_is_list(list.TermRef))
                term._termRef = LibPl.PL_copy_term_ref(list.TermRef);
            else
                throw new PlTypeException("list", list);

            return term;
        }
        #endregion


        #region PlCompound Creation
        /// <overloads>
        /// <summary>
        /// <para>These static methods creates a new compound <see cref="PlTerm"/>.</para>
        /// <para>For an example <see cref="PlCompound(string, PlTermV)"/></para>
        /// </summary>
        /// </overloads>
        /// <summary>
        /// It is the same as new PlTerm(text).
        /// </summary>
        /// <param name="text">The string representing the compound term.</param>
        /// <returns>a new <see cref="PlTerm"/></returns>
        [Obsolete("PlTerm.PlCompound(test) is deprecated, please use new PlTerm(text) instead.")]
        static internal PlTerm PlCompound(string text)
        {
            return new PlTerm(text);
        }

        /// <summary>
        /// <para>Create a compound term with the given name from the given vector of arguments. See <see cref="PlTermV"/> for details.</para>
        /// </summary>
        /// <example>
        /// <para>The example below creates the Prolog term hello(world).</para>
        /// <code>
        ///  PlTerm t = PlTerm.PlCompound("hello", new PlTermv("world"));
        /// </code>
        /// </example>
        /// <param name="functor">The functor (name) of the compound term</param>
        /// <param name="args">the arguments as a <see cref="PlTermV"/></param>
        /// <returns>a new <see cref="PlTerm"/></returns>
        static public PlTerm PlCompound(string functor, PlTermV args)
        {
            Check.Require(args.A0 != 0);
            var term = new PlTerm {_termRef = LibPl.PL_new_term_ref()};
            LibPl.PL_cons_functor_v(term.TermRef, LibPl.PL_new_functor(LibPl.PL_new_atom_wchars(functor), args.Size), args.A0);
            return term;
        }

        /// <summary>
        /// <para>Create a compound term with the given name ant the arguments</para>
        /// </summary>
        /// <param name="functor">The functor (name) of the compound term</param>
        /// <param name="arg1">The first Argument as a <see cref="PlTerm"/></param>
        /// <returns>a new <see cref="PlTerm"/></returns>
        static public PlTerm PlCompound(string functor, PlTerm arg1)
        {
            var args = new PlTermV(arg1);
            return PlCompound(functor, args);
        }
#pragma warning disable 1573
        ///<inheritdoc cref="PlCompound(string, PlTerm)" />
        /// <param name="arg2">The second Argument as a <see cref="PlTerm"/></param>
        static public PlTerm PlCompound(string functor, PlTerm arg1, PlTerm arg2)
        {
            var args = new PlTermV(arg1, arg2);
            return PlCompound(functor, args);
        }
        ///<inheritdoc cref="PlCompound(string, PlTerm, PlTerm)" />
        /// <param name="arg3">The third Argument as a <see cref="PlTerm"/></param>
        static public PlTerm PlCompound(string functor, PlTerm arg1, PlTerm arg2, PlTerm arg3)
        {
            var args = new PlTermV(arg1, arg2, arg3);
            return PlCompound(functor, args);
        }
#pragma warning restore 1573
        #endregion PlCompound Creation
        
        #region PlString Creation

        /// <summary>
        /// A SWI-Prolog string represents a byte-string on the global stack.
        /// It's lifetime is the same as for compound terms and other data living on the global stack.
        /// Strings are not only a compound representation of text that is garbage-collected,
        /// but as they can contain 0-bytes, they can be used to contain arbitrary C-data structures.
        /// </summary>
        /// <param name="text">the string</param>
        /// <returns>a new PlTerm</returns>
        /// <remarks>NOTE: this Method do *not* work with unicode characters. Concider to use new PlTerm(test) instead.</remarks>
        static public PlTerm PlString(string text)
        {
            var t = new PlTerm {_termRef = LibPl.PL_new_term_ref()};
            LibPl.PL_unify_wchars(t.TermRef, PlType.PlString, text);
            return t;
        }
#pragma warning disable 1573
        /// <inheritdoc cref="PlString(string)" />
        /// <param name="len">the length of the string</param>
        static public PlTerm PlString(string text, int len)
        {
            var t = new PlTerm {_termRef = LibPl.PL_new_term_ref()};
            LibPl.PL_unify_wchars(t.TermRef, PlType.PlString, len, text);
            return t;
        }
#pragma warning restore 1573
        #endregion PlString Creation

        #region PlCodeList Creation
        /// <summary>
        /// Create a Prolog list of ASCII codes from a 0-terminated C-string.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>a new <see cref="PlTerm"/></returns>
        static public PlTerm PlCodeList(string text)
        {
            var term = new PlTerm {_termRef = LibPl.PL_new_term_ref()};
            LibPl.PL_unify_wchars(term.TermRef, PlType.PlCodeList, text);
            return term;
        }
        #endregion

        #region PlCharList Creation
        /// <overloads>
        /// <summary>
        /// <para>These static methods creates a new PlCharList.</para>
        /// </summary>
        /// </overloads>
        /// <summary>Create a Prolog list of one-character atoms from a C#-string.</summary>
        /// <remarks>Character lists are compliant to Prolog's <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/manipatom.html#atom_chars/2">atom_chars/2</see> predicate.</remarks>
        /// <param name="text">a string</param>
        /// <returns>A new PlTerm containing a prolog list of character</returns>
        static public PlTerm PlCharList(string text)
        {
            var term = new PlTerm {_termRef = LibPl.PL_new_term_ref()};
            LibPl.PL_unify_wchars(term.TermRef, PlType.PlCharList, text);
            return term;
        }
        #endregion



     /***************************************
	 *	                            		*
	 ***************************************/


        #region Testing the type of a term ( IsVar, IsList, .... )


        /// <summary>
        /// return false for a PlTerm variable wihich is only declareted 
        /// and tru if it is also Initialized
        /// </summary>
        public bool IsInitialized { get { return 0 != _termRef; } }


        /// <summary>Get the <see cref="T:SbsSW.SwiPlCs.PlType"/> of a <see cref="PlTerm"/>.</summary>
        public PlType PlType
        {
            get { return (PlType)LibPl.PL_term_type(TermRef); }
        }

        // all return non zero if condition succeed

        /// <summary>Return true if <see cref="PlTerm"/> is a variable</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsVar { get { return 0 != LibPl.PL_is_variable(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is a ground term. See also ground/1. This function is cycle-safe.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsGround { get { return 0 != LibPl.PL_is_ground(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is an atom.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsAtom { get { return 0 != LibPl.PL_is_atom(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is a string.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsString { get { return 0 != LibPl.PL_is_string(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is an integer.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsInteger { get { return 0 != LibPl.PL_is_integer(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is a float.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsFloat { get { return 0 != LibPl.PL_is_float(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is a compound term. Note that a list is a compound term ./2</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsCompound { get { return 0 != LibPl.PL_is_compound(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is a compound term with functor ./2 or the atom [].</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsList { get { return 0 != LibPl.PL_is_list(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is atomic (not variable or compound).</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsAtomic { get { return 0 != LibPl.PL_is_atomic(TermRef); } }

        /// <summary>Return true if <see cref="PlTerm"/> is an integer or float.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        public bool IsNumber { get { return 0 != LibPl.PL_is_number(TermRef); } }

        #endregion



     /***************************************
	 *	LIST ( PlTerm ) implementation      *
	 ***************************************/

        #region list ( PlTail ) Methods

        // building
        /// <summary>
        /// Appends element to the list and make the PlTail reference point to the new variable tail. 
        /// If A is a variable, and this method is called on it using the argument "gnat", 
        /// a list of the form [gnat|B] is created and the PlTail object now points to the new variable B.
        /// 
        /// This method returns TRUE if the unification succeeded and FALSE otherwise. No exceptions are generated.
        /// </summary>
        /// <example>
        ///     <code source="..\..\TestSwiPl\PlTail.cs" region="List_Append_from_doc" />
        /// </example>
        /// <param name="term">The PlTerm to append on the list.</param>
        /// <returns>true if successful otherwise false</returns>
        /// <example>
        /// <para>The following samples shows how <see cref="T:System.Linq"/> can be used to filter a Prolog list.</para>
        /// <code source="..\..\TestSwiPl\LinqPlTail.cs" region="query_prologlist_PlTail_with_Linq_doc" />
        /// </example>
        public bool Append(PlTerm term)
        {
            Check.Require(IsList || IsVar);
            Check.Require(term._termRef != 0);

            uintptr_t tmp = LibPl.PL_new_term_ref();
            if (0 != LibPl.PL_unify_list(TermRef, tmp, TermRef) && 0 != LibPl.PL_unify(tmp, term.TermRef))
                return true;

            return false;
        }

        /// <summary>
        /// Appends an element to a list by creating a new one and copy all elements
        /// Note This is a slow version
        /// see my mail from Jan from 2007.11.06 14:44
        /// </summary>
        /// <param name="term">a closed list</param>
        /// <returns>True if Succeed</returns>
        public bool Add(PlTerm term)
        {
            Check.Require(IsList);
            Check.Require(term != null);

            uintptr_t list, head, tail;
            BuildOpenList(out list, out head, out tail);

            if (0 == LibPl.PL_unify_list(tail, head, tail))	// extend the list with a variable
                return false;
            if (0 == LibPl.PL_unify(term.TermRef, head))	// Unify this variable with the new list
                return false;
            LibPl.PL_unify_nil(tail);

            _termRef = list;
            return true;
        }


        /// <summary>
        /// Appends a list ( PlTail ) to a list by creating a new one and copy all elements
        /// </summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\PlTail.cs" region="List_Add_list_doc" />
        /// </example>
        /// <param name="listToAppend">a closed list</param>
        /// <returns>True if Succeed</returns>
        public bool AddList(PlTerm listToAppend)
        {
            Check.Require(IsList);
            Check.Require(listToAppend != null);
            Check.Require(listToAppend.IsList);

            uintptr_t list, head, tail;
            BuildOpenList(out list, out head, out tail);

            uintptr_t list2 = LibPl.PL_copy_term_ref(listToAppend.TermRef);
            uintptr_t elem = LibPl.PL_new_term_ref(); 			// 'elem' for iterating the old list
            while (0 != LibPl.PL_get_list(list2, elem, list2))
            {
                LibPl.PL_unify_list(tail, head, tail);	// extend the list with a variable
                LibPl.PL_unify(elem, head);				// Unify this variable with the new list
            }

            LibPl.PL_unify_nil(tail);

            _termRef = list;
            return true;
        }

        /// <summary>
        /// Unifies the term with [] and returns the result of the unification.
        /// </summary>
        /// <returns>The <c>int</c> value of <c>PL_unify_nil(TermRef)</c></returns>
        public int Close()
        {
            Check.Require(IsList || IsVar);
            return LibPl.PL_unify_nil(TermRef);
        }

        /// <summary>
        /// return a PlTerm bound to the next element of the list PlTail and advance PlTail. 
        /// Returns the element on success or a free PlTerm (Variable) if PlTail represents the empty list. 
        /// If PlTail is neither a list nor the empty list, a PlTypeException (type_error) is thrown. 
        /// </summary>
        /// <inheritdoc cref="AddList(PlTerm)" select="example"/>
        /// <returns>The Next element in the list as a PlTerm which is a variable for the last element or an empty list</returns>
        public PlTerm NextValue()
        {
            Check.Require(IsList);
            PlTerm term = PlVar();
            if (0 != LibPl.PL_get_list(TermRef, term.TermRef, TermRef))
            {
                return term;
            }
            if (0 != LibPl.PL_get_nil(TermRef))
            {
                return term;
            }
            throw new PlTypeException("list", this);
        }

        /// <summary>
        /// Converts to a strongly typed ReadOnlyCollection of PlTerm objects that can be accessed by index
        /// </summary>
        /// <returns>A strongly typed ReadOnlyCollection of PlTerm objects</returns>
        public ReadOnlyCollection<PlTerm> ToList()
        {
            Check.Require(IsList);
            // make a copy to keep the list
            var tmp = new PlTerm(LibPl.PL_copy_term_ref(TermRef));
            var l = new List<PlTerm>();
            foreach (PlTerm t in tmp)
            {
                l.Add(t);
            }
            return new ReadOnlyCollection<PlTerm>(l);
        }

        /// <summary>
        /// Converts to a strongly typed Collection of strings that can be accessed by index
        /// </summary>
        /// <returns>A strongly typed string Collection</returns>
        public Collection<string> ToListString() 
        {
            Check.Require(IsList);
            // make a copy to keep the list
            var tmp = new PlTerm(LibPl.PL_copy_term_ref(TermRef));
            var l = new List<string>();
            foreach (PlTerm t in tmp)
            {
                l.Add(t.ToString());
            }
            return new Collection<string>(l);
        }


        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A System.Collections.Generic.IEnumerator&lt;T that can be used to iterate through the collection.</returns>
        public IEnumerator<PlTerm> GetEnumerator()
        {
            Check.Require(IsList);
            PlTerm t; //null;
            while (Next(out t))
            {
                yield return t;
            }
        }
        #endregion IEnumerable<T> Members


        // private list helper methods

        // enumerating
        // see: http://www.mycsharp.de/wbb2/thread.php?threadid=53241

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Looks recursive but it is *not*.
        }
        #endregion


        /// <summary>
        /// Bind termRef to the next element of the list PlTail and advance PlTail. 
        /// Returns TRUE on success and FALSE if PlTail represents the empty list. 
        /// If PlTail is neither a list nor the empty list, a type_error is thrown. 
        /// </summary>
        /// <param name="termRef"></param>
        /// <returns></returns>
        private bool Next(out PlTerm termRef)
        {
            Check.Require(IsList);
            termRef = PlVar();  // new PlTerm();
            if (0 != LibPl.PL_get_list(TermRef, termRef._termRef, TermRef))
            {
                return true;
            }
            if (0 != LibPl.PL_get_nil(TermRef))
            {
                return false;
            }
            throw new PlTypeException("list", this);
        }

        private void BuildOpenList(out uintptr_t list, out uintptr_t head, out uintptr_t tail)
        {
            list = LibPl.PL_new_term_ref();			    // our list (starts unbound)
            tail = LibPl.PL_copy_term_ref(list);	    // the tail of it (starts as the whole)
            head = LibPl.PL_new_term_ref(); 			// placeholder for the element
            uintptr_t elem = LibPl.PL_new_term_ref(); 		// for iterating the old list
            while (0 != LibPl.PL_get_list(TermRef, elem, TermRef))
            {
                LibPl.PL_unify_list(tail, head, tail);	// extend the list with a variable
                LibPl.PL_unify(elem, head);				// Unify this variable with the new list
            }
        }

        #endregion list ( PlTail ) Methods


        #region ToString and helpers

        private string ToStringAsListFormat()
        {
            var sb = new StringBuilder(""); 
            var list = PlTail(this);
            foreach (PlTerm t in list)
            {
                if (0 < sb.Length)
                    sb.Append(',');
                sb.Append(t.ToString());
            }
            sb.Insert(0, '[');
            sb.Append(']');
            return sb.ToString();
        }

        /// <inheritdoc />
        /// <summary>
        /// <para>If PlTerm is a list the string is build by calling ToString() for each element in the list 
        /// separated by ',' and put the brackets around '[' ']'.</para>
        /// <para></para>
        /// </summary>
        /// <seealso cref="O:string"/>
        /// <returns>A string representing the PlTerm.</returns>
        override public string ToString()
        {
            string s;
            if (IsList)    //switch (this.PlType)
                s = ToStringAsListFormat();
            else
                s = (string)this;
            return s;
        }


        /// <summary>
        /// Convert a PlTerm to a string by <see href="http://www.swi-prolog.org/pldoc/doc_for?object=c(%27PL_get_chars%27)">PL_get_chars/1</see>
        /// with the CVT_WRITE_CANONICAL flag. If it fails PL_get_chars/3 is called again with REP_MB flag.
        /// </summary>
        /// <returns>return the string of a PlTerm</returns>
        /// <exception cref="PlTypeException">Throws a PlTypeException if PL_get_chars/3 didn't succeeds.</exception>
        public string ToStringCanonical()
        {
            string s;
            if (0 != LibPl.PL_get_wchars(TermRef, out s, LibPl.CVT_WRITE_CANONICAL | LibPl.BUF_RING | LibPl.REP_UTF8))
                return s;
            throw new PlTypeException("text", this);
        }

        #endregion


        #region unification
        /// <overloads>
        /// This methods performs Prolog unification and returns true if successful and false otherwise.
        /// It is equal to the prolog =/2 operator.
        /// <para>See <see cref="Unify(PlTerm)"/> for an example.</para>
        /// <remarks>
        /// This methods are introduced for clear separation between the destructive assignment in C# using =
        /// and prolog unification.
        /// </remarks>
        /// </overloads>
        /// <summary>Unify a PlTerm with a PlTerm</summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\PlTerm.cs" region="UnifyTermVar_doc" />
        /// </example>
        /// <param name="term">the second term for unification</param>
        /// <returns>true or false</returns>
        public bool Unify(PlTerm term)
        {
            return 0 != LibPl.PL_unify(TermRef, term.TermRef);
        }

        /// <inheritdoc cref="Unify(PlTerm)"/>
        /// <param name="atom">A string to unify with</param>
        public bool Unify(string atom)
        {
            return 0 != LibPl.PL_unify_wchars(TermRef, PlType.PlAtom, atom);
        }

        // <summary>
        // Useful e.g. for lists list.Copy().ToList(); list.ToString();
        // </summary>
        // <returns>Return a unifies PlTerm.PlVar of this term</returns>
        //internal PlTerm Copy()
        //{
        //    PlTerm tc = PlVar();
        //    if (!Unify(tc))
        //        throw new PlLibException("Copy term fails (Unification return false)");
        //    return tc;
        //}
        /*
        <!--
        int operator =(const PlTerm &t2)	// term 
        {
            return PL_unify(_termRef, t2._termRef);
        }
        int operator =(const PlAtom &atom)	// atom
        {
            return PL_unify_atom(TermRef, atom._handle);
        }
        int operator =(const char *v)		// atom (from char *)
        {
            return PL_unify_atom_chars(_termRef, v);
        }
        int operator =(long v)		// integer
        {
            return PL_unify_integer(_termRef, v);
        }
        int operator =(int v)			// integer
        {
            return PL_unify_integer(_termRef, v);
        }
        int operator =(double v)		// float
        {
            return PL_unify_float(_termRef, v);
        }
        int operator =(const PlFunctor &f)	// functor
        {
            return PL_unify_functor(_termRef, f.functor);
        }
        -->
        */
        #endregion unification


        #region Arity and Name
        /// <summary><para>Get the arity of the functor if <see cref="PlTerm"/> is a compound term.</para></summary>
        /// <remarks><para><see cref="Arity"/> and <see cref="Name"/> are for compound terms only</para></remarks>
        /// <exception cref="NotSupportedException">Is thrown if the term isn't compound</exception>
        public int Arity
        {
            get
            {
                uintptr_t name = 0; // atom_t 
                int arity = 0;
                if (0 != LibPl.PL_get_name_arity(TermRef, ref name, ref arity))
                    return arity;

                throw new NotSupportedException("Only possible for compound or atoms");
                //throw new PlTypeException("compound", this);   // FxCop Don't like this type of exception
            }
        }

        /// <summary>
        /// <para>Get a holding the name of the functor if <see cref="PlTerm"/> is a compound term.</para>
        /// </summary>
        /// <inheritdoc cref="Arity" />
        public string Name
        {
            get
            {
                uintptr_t name = 0; // atom_t 
                int arity = 0;

                if (0 != LibPl.PL_get_name_arity(TermRef, ref name, ref arity))
                    return LibPl.PL_atom_wchars(name);

                throw new NotSupportedException("Only possible for compound or atoms");
                //throw new PlTypeException("compound", this);   // FyCop Don't like this type of exception
            }
        }
        #endregion Arity and Name

        
        #region cast oprators
        /// <summary>
        /// Converts the Prolog argument into a string which implies Prolog atoms and strings
        /// are converted to the represented text or throw a PlTypeException. 
        /// </summary>
        /// <remarks>
        /// <para>Converts the Prolog argument using PL_get_chars() using the 
        /// flags CVT_ALL|CVT_WRITE|BUF_RING, which implies Prolog atoms and strings
        /// are converted to the represented text or throw a PlTypeException. 
        /// </para>
        /// <para>If the above call return 0 <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_get_chars()">PL_get_chars</see> 
        /// is called a second time with the flags CVT_ALL|CVT_WRITE|BUF_RING|REP_UTF8.</para>
        /// <para>All other data is handed to write/1.</para>
        /// </remarks>
        /// <param name="term">A PlTerm that can be converted to a string</param>
        /// <returns>A C# string</returns>
        /// <exception cref="PlTypeException">Throws a PlTypeException exception</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator string(PlTerm term)
        {
            string s;
            if (0 != LibPl.PL_get_wchars(term.TermRef, out s, LibPl.CVT_ALL | LibPl.CVT_WRITE | LibPl.BUF_RING | LibPl.REP_UTF8))
                return s;
            throw new PlTypeException("text", term);
        }

        /// <summary>
        /// Yields a int if the PlTerm is a Prolog integer or float that can be converted 
        /// without loss to a int. Throws a PlTypeException exception otherwise
        /// </summary>
        /// <param name="term">A PlTerm is a Prolog integer or float that can be converted without loss to a int.</param>
        /// <returns>A C# int</returns>
        /// <exception cref="PlTypeException">Throws a PlTypeException exception if <see cref="PlType"/> 
        /// is not a <see langword="PlType.PlInteger"/> or a <see langword="PlType.PlFloat"/>.</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator int(PlTerm term)
        {
            int v = 0;
            if (0 != LibPl.PL_get_long(term.TermRef, ref v))
                return v;
            throw new PlTypeException("long", term);
        }

        /// <summary>
        /// Yields the value as a C# double if PlTerm represents a Prolog integer or float. 
        /// Throws a PlTypeException exception otherwise. 
        /// </summary>
        /// <param name="term">A PlTerm represents a Prolog integer or float</param>
        /// <returns>A C# double</returns>
        /// <exception cref="PlTypeException">Throws a PlTypeException exception if <see cref="PlType"/> 
        /// is not a <see langword="PlType.PlInteger"/> or a <see langword="PlType.PlFloat"/>.</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator double(PlTerm term)
        {
            double v = 0;
            if (0 != LibPl.PL_get_float(term.TermRef, ref v))
                return v;
            throw new PlTypeException("float", term);
        }

        #endregion cast oprators


        #region compare operators
        // Comparison standard order terms
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return TermRef.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(Object obj)
        {
            if (obj is PlTerm)
                return this == ((PlTerm)obj);
            if (obj is int)
                return this == ((int)obj);
            return false;
        }
        /// <overload>Compare the instance term1 with term2 and return the result according to the Prolog defined standard order of terms.</overload>
        /// <summary>
        /// Yields TRUE if the PlTerm is an atom or string representing the same text as the argument, 
        /// FALSE if the conversion was successful, but the strings are not equal and an 
        /// type_error exception if the conversion failed.
        /// </summary>
        /// <param name="term1">a PlTerm</param>
        /// <param name="term2">a PlTerm</param>
        /// <returns>true or false</returns>
        public static bool operator ==(PlTerm term1, PlTerm term2)
        {
            return LibPl.PL_compare(term1.TermRef, term2.TermRef) == 0;
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator !=(PlTerm term1, PlTerm term2)
        {
            return LibPl.PL_compare(term1.TermRef, term2.TermRef) != 0;
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator <(PlTerm term1, PlTerm term2)
        {
            return LibPl.PL_compare(term1.TermRef, term2.TermRef) < 0;
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator >(PlTerm term1, PlTerm term2)
        {
            return LibPl.PL_compare(term1.TermRef, term2.TermRef) > 0;
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator <=(PlTerm term1, PlTerm term2)
        {
            return LibPl.PL_compare(term1.TermRef, term2.TermRef) <= 0;
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator >=(PlTerm term1, PlTerm term2)
        {
            return LibPl.PL_compare(term1.TermRef, term2.TermRef) >= 0;
        }


        /*
    int operator <=(const PlTerm &t2)
    {
        return PL_compare(_termRef, t2.TermRef) <= 0;
    }
    int operator >=(const PlTerm &t2)
    {
        return PL_compare(_termRef, t2._termRef) >= 0;
    }
    */
        // comparison (long)
#endregion

        #region Equality Method
        /// <overload>Compare the instance term1 with term2 and return the result according to the Prolog defined standard order of terms.</overload>
        /// <summary>
        /// Yields TRUE if the PlTerm is an atom or string representing the same integer as the argument, 
        /// FALSE if the conversion was not successful.
        /// conversion of the term is done by  PL_get_long
        /// </summary>
        /// <param name="term">a PlTerm</param>
        /// <param name="value">a int</param>
        /// <returns>A bool</returns>
        public static bool operator ==(PlTerm term, int value)
        {
            int v0 = 0;
            if (0 != LibPl.PL_get_long(term.TermRef, ref v0))
                return v0 == value;
            return false; // throw new PlTypeException("integer", term);
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator ==(int value, PlTerm term)
        {
            return term == value;
        }
        // comparison (string)
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator ==(PlTerm term, string value)
        {
            return ((string)term).Equals(value);
        }
        /// <inheritdoc cref="op_Equality(PlTerm, PlTerm)" />
        public static bool operator ==(string value, PlTerm term)
        {
            return term == value;
        }

        #endregion


        #region Inequality Method
        /// <overloads>
        /// <summary>
        /// <para>Inequality Method overload</para>
        /// <see cref="op_Equality(PlTerm, PlTerm)"/>
        /// a
        /// <see cref="M:SbsSW.SwiPlCs.PlTerm.op_Equality(SbsSW.SwiPlCs.PlTerm,System.Int32)"/>
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="term"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator !=(PlTerm term, int value)
        {
            int v0 = 0;
            if (0 != LibPl.PL_get_long(term.TermRef, ref v0))
                return v0 != value;
            return true; // throw new PlTypeException("integer", term);
        }
        /// <inheritdoc cref="op_Inequality(PlTerm, int)" />
        public static bool operator !=(int value, PlTerm term)
        {
            return term != value;
        }
        /// <summary>
        /// test
        /// </summary>
        /// <param name="term"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator !=(PlTerm term, string value)
        {
            return !(term == value);
        }
        /// <inheritdoc cref="op_Inequality(PlTerm, string)" />
        public static bool operator !=(string value, PlTerm term)
        {
            return term != value;
        }

        #endregion compare operators


    } // class PlTerm
    #endregion


    /********************************
	*	      PlTermV				*
	********************************/
    #region public class PlTermV
    /// <summary>
    /// <preliminary>The struct PlTermv represents an array of term-references.</preliminary>
    /// <para>This type is used to pass the arguments to a foreign defined predicate (see <see cref="Callback.DelegateParameterVarArgs"/>), 
    /// construct compound terms (see <see cref="PlTerm.PlCompound(string, PlTermV)"/> 
    /// and to create queries (see <see cref="T:SbsSW.SwiPlCs.PlQuery"/>).
    /// </para>
    /// <para>The only useful member function is the overloading of [], providing (0-based) access to the elements. <see cref="PlTermV.this[Int32]"/> 
    /// Range checking is performed and raises a ArgumentOutOfRangeException exception.</para> 
    /// </summary>
    public struct PlTermV : IEquatable<PlTermV>   
    {

        private readonly uintptr_t _a0; // term_t
        private readonly int _size;


        #region constructors

        /// <overloads>Create a PlTermV vector from the given PlTerm parameters
        /// <summary>
        /// <para>Create a new vector with PlTerm as elements</para>
        /// <para>It can be created with <paramref name="size"/> elements</para>
        /// <para>or</para>
        /// <para>automatically for 1, 2 or 3 plTerms</para>
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Create a vector of PlTerms with <paramref name="size"/> elements
        /// </summary>
        /// <param name="size">The amount of PlTerms in the vector</param>
        public PlTermV(int size)
        {
            _a0 = LibPl.PL_new_term_refs(size);
            _size = size;
        }

        /// <summary>Create a PlTermV from the given <see cref="PlTerm"/>s.</summary>
        /// <param name="term0">The first <see cref="PlTerm"/> in the vector.</param>
        public PlTermV(PlTerm term0)
        {
            _size = 1;
            _a0 = term0.TermRef;
        }

// warning CS1573: Parameter 'term0' has no matching param tag in the XML comment for 'SbsSW.SwiPlCs.PlTermV.PlTermV(SbsSW.SwiPlCs.PlTerm, SbsSW.SwiPlCs.PlTerm)' (but other parameters do)
#pragma warning disable 1573
        /// <inheritdoc cref="PlTermV(PlTerm)" />
        /// <param name="term1">The second <see cref="PlTerm"/> in the vector.</param>
        public PlTermV(PlTerm term0, PlTerm term1)
        {
            _size = 2;
            _a0 = LibPl.PL_new_term_refs(2);
            LibPl.PL_put_term(_a0 + 0, term0.TermRef);
            LibPl.PL_put_term(_a0 + 1, term1.TermRef);
        }

        /// <inheritdoc cref="PlTermV(PlTerm, PlTerm)" />
        /// <param name="term2">The third <see cref="PlTerm"/> in the vector.</param>
        public PlTermV(PlTerm term0, PlTerm term1, PlTerm term2)
        {
            _size = 3;
            _a0 = LibPl.PL_new_term_refs(3);
            LibPl.PL_put_term(_a0 + 0, term0.TermRef);
            LibPl.PL_put_term(_a0 + 1, term1.TermRef);
            LibPl.PL_put_term(_a0 + 2, term2.TermRef);
        }

        /// <summary>Create a PlTermV from the given <see cref="PlTerm"/>[] array.</summary>
        /// <param name="terms">An array of <see cref="PlTerm"/>s to build the vector.</param>
        /// <example>
        /// Use of Initializing an Array in CSharp
        /// <code>
        ///    PlTermV v = new PlTermV(new PlTerm[] {t1, t2, t3, t4});
        /// </code>
        /// </example>
        public PlTermV(PlTerm[] terms)
        {
            if (null == terms)
                throw new ArgumentNullException("terms");
            _size = terms.Length;
            _a0 = LibPl.PL_new_term_refs(terms.Length);
            ulong count = 0;
            foreach (PlTerm t in terms)
            {
                LibPl.PL_put_term(_a0 + count, t.TermRef);
                count++;
            }
        }
#pragma warning restore 1573
        #endregion



//        internal PlTermV(PlTermV toCopy)
//            : this(toCopy._size)
//        {
//            for (uint i = 0; i < toCopy._size; i++)
//            {
////                libpl.PL_put_term(_a0 + i, new PlTerm(toCopy[(int)i].TermRef).TermRef);
//                this[i].TermRef = libpl.PL_copy_term_ref(toCopy[i].TermRef);
//            }
//        }

        // Properties

        /// <summary>
        /// the first term_t reference of the array
        /// </summary>
        internal uintptr_t A0
        {
            get { return _a0; }
        }

        /// <summary>Get the size of a PlTermV</summary>
        public int Size
        {
            get { return _size; }
        }


        /// <summary>
        /// A zero based list
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The PlTerm for the given index</returns>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown if (index &lt;  0 || index >= Size)</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public PlTerm this[int index]
        {
            get
            {
                if (index < 0 || index >= Size)
                    throw new ArgumentOutOfRangeException("index");
                return new PlTerm(A0 + (uint)index);  // If this line is deleted -> update comment in PlTern(term_ref)
            }
            set
            {
                if (index < 0 || index >= Size)
                    throw new ArgumentOutOfRangeException("index");
                LibPl.PL_put_term(_a0 + (uint)index, value.TermRef);  // TermRef == 0, "use of an uninitialized PlTerm. If you need a variable use PlTerm.PlVar() instead
            }
        }


        #region IEquatable<PlTermV> Members
        // see http://msdn.microsoft.com/de-de/ms182276.aspx


        ///<inheritdoc />
        public override int GetHashCode()
        {
            return A0.GetHashCode();
        }

        ///<inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is PlTermV))
                return false;
            return Equals((PlTermV)obj);
        }

        ///<inheritdoc />
        /// <summary>
        /// Compare the size and A0 of the PltermV
        /// </summary>
        /// <param name="other">The PlTermV to compare</param>
        /// <returns>Return <c>false</c> if size or A0 are not equal otherwise <c>true</c>.</returns>
        ///<remarks>// TODO compare each PlTerm in PlTermV not only the refereces in A0</remarks>
        public bool Equals(PlTermV other)
        {
            if (_size != other._size)
                return false;

            return A0 == other.A0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="termVector1"></param>
        /// <param name="termVector2"></param>
        /// <returns></returns>
        public static bool operator ==(PlTermV termVector1, PlTermV termVector2)
        {
            return termVector1.Equals(termVector2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="termVector1"></param>
        /// <param name="termVector2"></param>
        /// <returns></returns>
        public static bool operator !=(PlTermV termVector1, PlTermV termVector2)
        {
            return !termVector1.Equals(termVector2);
        }    


        #endregion


    } // class PlTermV
    #endregion




    /****************************************************
	*	  PlFrame		                                *
    *	  PlQuery is in the file SWI-cs-PlQuery.cs      *
	****************************************************/
    #region public class PlFrame

    /// <summary>
    /// <para>The class PlFrame provides an interface to discard unused term-references as well as rewinding unifications (data-backtracking). 
    /// Reclaiming unused term-references is automatically performed after a call to a C#-defined predicate has finished and 
    /// returns control to Prolog. In this scenario PlFrame is rarely of any use.</para>
    /// <para>This class comes into play if the top level program is defined in C# and calls Prolog multiple times. 
    /// Setting up arguments to a query requires term-references and using PlFrame is the only way to reclaim them.</para>
    /// </summary>
    /// <remarks>see <see href="http://www.swi-prolog.org/pldoc/package/pl2cpp.html#sec:8.1"/></remarks>
    /// <example>
    /// A typical use for PlFrame is the definition of C# methods that call Prolog and may be called repeatedly from C#.
    /// Consider the definition of assertWord(), adding a fact to word/1:
    ///     <code source="..\..\TestSwiPl\PlFrame.cs" region="AssertWord2_doc" />
    /// alternatively you can use
    ///     <code source="..\..\TestSwiPl\PlFrame.cs" region="AssertWord_doc" />
    /// <b><note type="caution"> NOTE: in any case you have to destroy any query object used inside a PlFrame</note></b>
    /// </example>
    public class PlFrame : IDisposable
    {
        #region IDisposable
        // see : "Implementing a Dispose Method  [C#]" in  ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconimplementingdisposemethod.htm
        // and IDisposable in class PlQuery

        private bool _disposed;

        /// <summary>Implement IDisposable.</summary>
        /// <remarks>
        /// <para>Do not make this method virtual.</para>
        /// <para>A derived class should not be able to override this method.</para>
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off of the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
                Free();
            }
        }


        #endregion

        private uintptr_t _fid; // fid_t

        /// <summary>
        /// Creating an instance of this class marks all term-references created afterwards to be valid only in the scope of this instance.
        /// </summary>
        public PlFrame()
        {
            _fid = LibPl.PL_open_foreign_frame();
        }

        /// <summary>
        /// Reclaims all term-references created after constructing the instance.
        /// </summary>
        ~PlFrame()
        {
            Dispose(false);
        }

        /// <summary>
        /// Discards all term-references and global-stack data created as well as undoing all unifications after the instance was created.
        /// </summary>
        public void Rewind()
        {
            LibPl.PL_rewind_foreign_frame(_fid);
        }


        /// <summary>called by Dispose</summary>
        private void Free()
        {
            if (_fid > 0 && PlEngine.IsInitialized)
            {
                LibPl.PL_close_foreign_frame(_fid);
            }
            _fid = 0;
        }


    } // PlFrame
    #endregion




    /********************************
	*	      ENGINE				*
	********************************/


    #region public class PlEngine


    /// <summary>
    /// This static class represents the prolog engine.
    /// </summary>
    /// <example>
    /// A sample
    /// <code>
    ///    if (!PlEngine.IsInitialized)
    ///    {
    ///        String[] empty_param = { "" };
    ///        PlEngine.Initialize(empty_param);
    ///        // do some funny things ...
    ///        PlEngine.PlCleanup();
    ///    } 
    ///    // program ends here
    /// </code>
    /// The following sample show how a file is consult via comand-line options.
    /// <code source="..\..\TestSwiPl\PlEngine.cs" region="demo_consult_pl_file_by_param" />
    /// </example>
    public static class PlEngine
    {

        #region RegisterForeign
        
        /// <overloads>
        /// <summary>
        /// <para>Register a C#-function to implement a Prolog predicate.</para>
        /// <para>After this call returns successfully, a predicate with name (a string) and arity arity (a C# int) is created in module module.</para>
        /// <para>If module is NULL, the predicate is created in the module of the calling context or if no context is present in the module user.</para>
        /// </summary>
        /// <remarks>
        /// <para>Add a additional namespace by:</para>
        /// <para><inlinecode>using SbsSW.SwiPlCs.Callback;</inlinecode></para>
        /// </remarks>
        /// <example>For an example see <see cref="Callback.DelegateParameter2"/> and <see cref="Callback.DelegateParameter1"/>.</example>
        /// <seealso cref="Callback"/>
        /// </overloads>
        /// 
        /// <summary>
        /// <para>Register a C# callback method</para>
        /// </summary>
        /// <example>For an example see <see cref="Callback.DelegateParameter2"/> and <see cref="Callback.DelegateParameter1"/>.</example>
        /// <param name="method">a delegate to a c# method <see cref="Callback"/></param>
        /// <returns>true if registration succeed otherwise false</returns>
        public static bool RegisterForeign(Delegate method)
        {
            return RegisterForeign(null, method);
        }


// warning CS1573: Parameter 'term0' has no matching param tag in the XML comment for 'SbsSW.SwiPlCs.PlTermV.PlTermV(SbsSW.SwiPlCs.PlTerm, SbsSW.SwiPlCs.PlTerm)' (but other parameters do)
#pragma warning disable 1573

        ///<inheritdoc cref="RegisterForeign(Delegate)" />
        /// <param name="module">the name of a prolog module <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/modules.html">Using Modules</see></param>
        public static bool RegisterForeign(string module, Delegate method)
        {
            if (method is Callback.DelegateParameterBacktrack)
                return RegisterForeign(module, method, Callback.PlForeignSwitches.Nondeterministic);
            return RegisterForeign(module, method, Callback.PlForeignSwitches.None);
        }

        // <example>
        //     <code source="..\..\TestSwiPl\CallbackForeigenPredicate.cs" region="t_varargs_doc" />
        // </example>


        /// <inheritdoc cref="RegisterForeign(Delegate)" />
        /// <example>For an example see <see cref="SbsSW.SwiPlCs.Callback.DelegateParameterVarArgs"/> </example>
        /// <param name="name">The name of a static C# method</param>
        /// <param name="arity">The amount of parameters</param>
        public static bool RegisterForeign(string name, int arity, Delegate method)
        {
            return RegisterForeign(null, name, arity, method);
        }

        /// <inheritdoc cref="RegisterForeign(string, int, Delegate)" />
        /// <param name="module">The name of the module (Prolog module system)</param>
        public static bool RegisterForeign(string module, string name, int arity, Delegate method)
        {
            return RegisterForeign(module, name, arity, method, Callback.PlForeignSwitches.VarArgs);
        }

#pragma warning restore 1573

        // make public to activate the foreign predicated in named modules
        #region  privates
        private static bool RegisterForeign(string module, Delegate method, Callback.PlForeignSwitches plForeign)
        {
            string name = method.Method.Name;
            int arity = method.Method.GetParameters().Length;
            return RegisterForeign(module, name, arity, method, plForeign);
        }


        /// <summary>
        /// <see href="http://www.swi-prolog.org/pldoc/doc_for?object=c(%27PL_register_foreign_in_module%27)"/>
        /// </summary>
        /// <returns></returns>
        private static bool RegisterForeign(string module, string name, int arity, Delegate method, Callback.PlForeignSwitches plForeign)
        {
            return Convert.ToBoolean(LibPl.PL_register_foreign_in_module(module, name, arity, method, (int)plForeign));
        }

        #endregion  privates


        #endregion RegisterForeign


        /// <summary>To test if the prolog engine is up.</summary>
        public static bool IsInitialized
        {
            get
            {
                var i = LibPl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero);
                return 0 != i;
            }
        }

        /// <summary>
        /// <para>Initialise SWI-Prolog</para>
        /// <para>The write method of the output stream is redirected to <see cref="SbsSW.SwiPlCs.Streams"/> 
        /// before Initialize. The read method of the input stream just after Initialize.</para>
        /// </summary>
        /// <remarks>
        /// <para>A known bug: Initialize work *not* as expected if there are e.g. German umlauts in the parameters
        /// See marshalling in the sorce NativeMethods.cs</para>
        /// </remarks>
        /// <param name="argv">
        /// <para>For a complete parameter description see the <a href="http://gollem.science.uva.nl/SWI-Prolog/Manual/cmdline.html" target="_new">SWI-Prolog reference manual section 2.4 Command-line options</a>.</para>
        /// <para>sample parameter: <code>String[] param = { "-q", "-f", @"some\filename" };</code>
        /// At the first position a parameter "" is added in this method. <see href="http://www.swi-prolog.org/pldoc/doc_for?object=section(3%2C%20%279.6.20%27%2C%20swi(%27%2Fdoc%2FManual%2Fforeigninclude.html%27))">PL_initialise</see>
        /// </para>
        /// </param>
        /// <example>For an example see <see cref="T:SbsSW.SwiPlCs.PlEngine"/> </example>
        public static void Initialize(String[] argv)
        {
            if (argv == null)
                throw new ArgumentNullException("argv", "Minimum is one empty string");
            if (IsInitialized)
                throw new PlLibException("PlEngine is already initialized");

            LibPl.LoadLibPl();
            // redirect input and output stream to receive messages from prolog
            var wf = new DelegateStreamWriteFunction(Swrite_function);
            if (!_isStreamFunctionWriteModified)
            {
                SetStreamFunctionWrite(PlStreamType.Output, wf);
                _isStreamFunctionWriteModified = false;
            }
            var localArgv = new String[argv.Length+1];
            int idx = 0;
            localArgv[idx++] = "";
            foreach (var s in argv)
                localArgv[idx++] = s;

            if (0 == LibPl.PL_initialise(localArgv.Length, localArgv))
            {
                throw new PlLibException("failed to initialize");
            }
            if (!_isStreamFunctionReadModified)
            {
                var rf = new DelegateStreamReadFunction(Sread_function);
                SetStreamFunctionRead(PlStreamType.Input, rf);
                _isStreamFunctionReadModified = false;
            }
        }


        /// <summary>
        /// Try a clean up but it is buggy
        /// search the web for "possible regression from pl-5.4.7 to pl-5.6.27" to see reasons
        /// </summary>
        /// <remarks>Use this method only at the last call before run program ends</remarks>
        static public void PlCleanup()
        {
            LibPl.PL_cleanup(0);
        }

        /// <summary>Stops the PlEngine and <b>the program</b></summary>
        /// <remarks>SWI-Prolog calls internally pl_cleanup and than exit(0)</remarks>
        static public void PlHalt()
        {
            LibPl.PL_halt(0);
        }

        // *****************************
        // STATICs for STREAMS
        // *****************************
        #region stream IO


        #region default_io_doc
        static internal long Swrite_function(IntPtr handle, string buf, long bufsize)
        {
            string s = buf.Substring(0, (int)bufsize);
            Console.Write(s);
            System.Diagnostics.Trace.WriteLine(s);
            return bufsize;
        }

        static internal long Sread_function(IntPtr handle, IntPtr buf, long bufsize)
        {
            throw new PlLibException("SwiPlCs: Prolog try to read from stdin");
        }
        #endregion default_io_doc



        static bool _isStreamFunctionWriteModified;  // default = false;
        static bool _isStreamFunctionReadModified;   // default = false;

        /// <summary>
        /// This is a primitive approach to enter the output from a stream.
        /// </summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\StreamIO.cs" region="StreamWrite_doc" />
        /// </example>
        /// <param name="streamType">Determine which stream to use <see cref="Streams.PlStreamType"/></param>
        /// <param name="function">A <see cref="Streams.DelegateStreamWriteFunction"/></param>
        static public void SetStreamFunctionWrite(PlStreamType streamType, DelegateStreamWriteFunction function)
        {
            LibPl.LoadLibPl();
            LibPl.SetStreamFunction(streamType, LibPl.StreamsFunction.Write, function);
            _isStreamFunctionWriteModified = true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\StreamIO.cs" region="StreamRead_doc" />
        /// </example>
        /// <param name="streamType">Determine which stream to use <see cref="Streams.PlStreamType"/></param>
        /// <param name="function">A <see cref="Streams.DelegateStreamReadFunction"/></param>
        static public void SetStreamFunctionRead(PlStreamType streamType, DelegateStreamReadFunction function)
        {
            LibPl.LoadLibPl();
            LibPl.SetStreamFunction(streamType, LibPl.StreamsFunction.Read, function);
            _isStreamFunctionReadModified = true;
        }


        #endregion stream IO




        // *****************************
        // STATICs for MULTI THreading
        // *****************************
        #region STATICs for MULTI THreading

        /// <summary>
        /// <para>return : reference count of the engine</para>
        ///	<para>		If an error occurs, -1 is returned.</para>
        ///	<para>		If this Prolog is not compiled for multi-threading, -2 is returned.</para>
        /// </summary>
        /// <returns>A reference count of the engine</returns>
        public static int PlThreadAttachEngine()
        {
            return LibPl.PL_thread_attach_engine(IntPtr.Zero);
        }


        /// <summary>
        /// This method is also provided in the single-threaded version of SWI-Prolog, where it returns -2. 
        /// </summary>
        /// <returns>Returns the integer Prolog identifier of the engine or -1 if the calling thread has no Prolog engine. </returns>
        public static int PlThreadSelf()
        {
            return LibPl.PL_thread_self();
        }


        /// <summary>
        /// Destroy the Prolog engine in the calling thread. 
        /// Only takes effect if <c>PL_thread_destroy_engine()</c> is called as many times as <c>PL_thread_attach_engine()</c> in this thread.
        /// <para>Please note that construction and destruction of engines are relatively expensive operations. Only destroy an engine if performance is not critical and memory is a critical resource.</para>
        /// </summary>
        /// <returns>Returns <c>true</c> on success and <c>false</c> if the calling thread has no engine or this Prolog does not support threads.</returns>
        public static bool PlThreadDestroyEngine()
        {
            return 0 != LibPl.PL_thread_destroy_engine();
        }

        #endregion

    } // class PlEngine
    #endregion



    #region public class PlMtEngine

    /// <summary>
    /// This class is experimental
    /// </summary>
    public class PlMtEngine : IDisposable
    {
        private IntPtr _iEngineNumber = IntPtr.Zero;
        // private IntPtr _iEngineNumberStore = IntPtr.Zero;


        #region IDisposable
        // see : "Implementing a Dispose Method  [C#]" in  ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconimplementingdisposemethod.htm
        // and IDisposable in class PlQuery

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off of the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
            Free();
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Free()
        {
            if (IntPtr.Zero != _iEngineNumber && PlEngine.IsInitialized)
            {
                if (0 == LibPl.PL_destroy_engine(_iEngineNumber))
                    throw (new PlLibException("failed to destroy engine"));
                _iEngineNumber = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~PlMtEngine()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public PlMtEngine()
        {
            if (0 != LibPl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero))
            {
                try
                {
                    _iEngineNumber = LibPl.PL_create_engine(IntPtr.Zero);
                }
                catch (Exception ex)
                {
                    throw (new PlLibException("PL_create_engine : " + ex.Message));
                }
            }
            else
            {
                throw new PlLibException("There is no PlEngine initialized");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlSetEngine()
        {
            IntPtr pNullPointer = IntPtr.Zero;
            int iRet = LibPl.PL_set_engine(_iEngineNumber, ref pNullPointer);
            switch (iRet)
            {
                case LibPl.PL_ENGINE_SET: break; // all is fine
                case LibPl.PL_ENGINE_INVAL: throw (new PlLibException("PlSetEngine returns Invalid")); //break;
                case LibPl.PL_ENGINE_INUSE: throw (new PlLibException("PlSetEngine returns it is used by an other thread")); //break;
                default: throw (new PlLibException("Unknown return from PlSetEngine"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void PlDetachEngine()
        {
            IntPtr pNullPointer = IntPtr.Zero;
            int iRet = LibPl.PL_set_engine(IntPtr.Zero, ref pNullPointer);
            switch (iRet)
            {
                case LibPl.PL_ENGINE_SET: break; // all is fine
                case LibPl.PL_ENGINE_INVAL: throw (new PlLibException("PlSetEngine(detach) returns Invalid")); //break;
                case LibPl.PL_ENGINE_INUSE: throw (new PlLibException("PlSetEngine(detach) returns it is used by an other thread")); //break;
                default: throw (new PlLibException("Unknown return from PlSetEngine(detach)"));
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            return _iEngineNumber.ToString();
        }

    } // class PlMtEngine
    #endregion


} // namespace SbsSW.SwiPlCs
