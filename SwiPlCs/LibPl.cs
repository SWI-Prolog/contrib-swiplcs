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
using SbsSW.DesignByContract;
using SbsSW.SwiPlCs.Exceptions;         // in PlHalt
using System.Runtime.InteropServices;

namespace SbsSW.SwiPlCs
{


	/**********************************
	* Wrapper libpl(cs) - DllFileName *
	**********************************/
	#region private Wraper classe libpl - MyLibPl mit constanten
	internal static class LibPl
	{

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Local

        internal const int PL_Q_NORMAL = 0x02;
        internal const int PL_Q_NODEBUG = 0x04;
        internal const int PL_Q_CATCH_EXCEPTION = 0x08;
        internal const int PL_Q_PASS_EXCEPTION = 0x10;
		/*******************************
		 *	    CHAR BUFFERS	        *
         *	    from include/SWI-Prolog.h
		 *******************************/
        private const int CVT_ATOM = 0x0001;
        private const int CVT_STRING = 0x0002;
        private const int CVT_LIST = 0x0004;
        private const int CVT_INTEGER = 0x0008;
        private const int CVT_FLOAT = 0x0010;
        private const int CVT_VARIABLE = 0x0020;
        private const int CVT_NUMBER = (CVT_INTEGER | CVT_FLOAT);
        private const int CVT_ATOMIC = (CVT_NUMBER | CVT_ATOM | CVT_STRING);
        internal const int CVT_WRITE = 0x0040;		// as of version 3.2.10
        internal const int CVT_WRITE_CANONICAL = 0x0080;		// as of version 5.10.1
        internal const int CVT_ALL = (CVT_ATOMIC | CVT_LIST);
        private const int CVT_MASK = 0x00ff;

        private const int BUF_DISCARDABLE = 0x0000;
        internal const int BUF_RING = 0x0100;
        private const int BUF_MALLOC = 0x0200;

        private const int REP_ISO_LATIN_1 = 0x0000;		/* output representation */
        internal const int REP_UTF8 = 0x1000;
        private const int REP_MB = 0x2000;

		//	 ENGINES (MT-ONLY)
        private const int PL_ENGINE_MAIN = 0x1;			//	  ((PL_engine_t)0x1)
        private const int PL_ENGINE_CURRENT = 0x2;		// ((PL_engine_t)0x2)

        internal const int PL_ENGINE_SET = 0;			// engine set successfully 
        internal const int PL_ENGINE_INVAL = 2;			// engine doesn'termRef exist
        internal const int PL_ENGINE_INUSE = 3;			// engine is in use 

// ReSharper restore UnusedMember.Local
// ReSharper restore InconsistentNaming


		/////////////////////////////
		/// libpl
		///

		#region helper for initialize and cleanUp halt

		// Unmanaged resource. CLR will ensure SafeHandles get freed, without requiring a finalizer on this class.
		static SafeLibraryHandle _hLibrary;


		private static bool IsValid
		{
			get { return _hLibrary != null && !_hLibrary.IsInvalid; }
		}
		
		private static void LoadUnmanagedLibrary(string fileName)
        {
			if (_hLibrary == null)
			{
				_hLibrary = NativeMethods.LoadDll(fileName);
				if (_hLibrary.IsInvalid)
				{
					int hr = Marshal.GetHRForLastWin32Error();
					Marshal.ThrowExceptionForHR(hr);
				}
			}
        }

		private static void UnLoadUnmanagedLibrary()
		{
			if (!_hLibrary.IsClosed)
			{
                _hLibrary.Close();
                do
                {
                    // be sure to unload swipl.sll
                } while (_hLibrary.UnLoad()); 
                // m_hLibrary.UnLoad();
                _hLibrary.Dispose();
                _hLibrary = null;
			}
		}

		#endregion helper for initialize and cleanUp halt


        /// <summary>
        /// The standard SWI-Prolog streams ( inout output error )
        /// </summary>
        internal enum StreamsFunction
        {
            /// <summary>0 - Sread_function.</summary>
            Read = 0,
            /// <summary>1 - Swrite_function.</summary>
            Write = 1
        }


        internal static void SetStreamFunction(Streams.PlStreamType streamType, StreamsFunction functionType, Delegate function)
        {
            // to calculate to following values use the C++ Project CalcStreamSize
#if _PL_X64
#warning _PL_X64 is defined
            const int sizeOfIostream = 256;
            const int offsetToPoninterOfIofunctions = 104;
            const int sizeOfPointer = 8;
#else
#warning _PL_X64 is NOT defined
            const int sizeOfIostream = 160;
            const int offsetToPoninterOfIofunctions = 72;
            const int sizeOfPointer = 4;
#endif
            IntPtr callbackFunctionPtr = Marshal.GetFunctionPointerForDelegate(function);
            IntPtr addressStdStreamArray = NativeMethods.GetPoninterOfIoFunctions(_hLibrary);
            IntPtr functionArrayOut = Marshal.ReadIntPtr(addressStdStreamArray, (sizeOfIostream * (int)streamType) + offsetToPoninterOfIofunctions);

#if _PL_X64
            Marshal.WriteIntPtr(new IntPtr(functionArrayOut.ToInt64() + (sizeOfPointer * (int)functionType)), callbackFunctionPtr);
#else
            Marshal.WriteIntPtr(new IntPtr(functionArrayOut.ToInt32() + (sizeOfPointer * (int)functionType)), callbackFunctionPtr);
#endif
        }

        internal static void LoadLibPl()
        {
            LoadUnmanagedLibrary(SafeNativeMethods.DllFileName1);
        }

		internal static int PL_initialise(int argc, String[] argv)
		{
            LoadLibPl();
            return SafeNativeMethods.PL_initialise(argc, argv);
		}


        //internal static int PL_is_initialised(ref int argc, ref String[] argv)
        //{
        //    int iRet = 0;
        //    if (IsValid)
        //    {
        //        iRet = SafeNativeMethods.PL_is_initialised(ref argc, ref argv);
        //    }
        //    return iRet;
        //}

        /// <summary>
        /// Does NOT work correct if engine is_initialised
        /// int PL_is_initialised(int *argc, char ***argv) 
        /// PL_is_initialised is the *only* function which may called befor PL_initialise
        /// </summary>
        /// <param name="argc"></param>
        /// <param name="argv"></param>
        /// <returns></returns>
        internal static int PL_is_initialised(IntPtr argc, IntPtr argv)
		{
			int iRet = 0;
			if (IsValid)
			{
				iRet = SafeNativeMethods.PL_is_initialised(argc, argv);
			}
			return iRet;
		}

		internal static int PL_halt(int i)
		{
			int iRet = 0;
			if (IsValid)
			{
				iRet = SafeNativeMethods.PL_halt(i);
				if (0 == iRet)
                    throw new PlLibException("PL_halt returned false");
				UnLoadUnmanagedLibrary();
			}
			return iRet;
		}

        internal static int PL_cleanup(int status)
        {
            int iRet = 0;
            if (IsValid)
            {
                //    System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
                //    System.Diagnostics.Trace.WriteLine(System.Threading.Thread.CurrentThread.IsThreadPoolThread);
                //    System.Diagnostics.Trace.WriteLine(System.Diagnostics.Process.GetCurrentProcess().Id);
                iRet = SafeNativeMethods.PL_cleanup(status);

                UnLoadUnmanagedLibrary();
                //GC.Collect();
                //GC.WaitForPendingFinalizers();
            }
            return iRet;
        }

        // see http://www.codeproject.com/KB/dotnet/Cdecl_CSharp_VB.aspx
        internal static int PL_register_foreign_in_module(string module, string name, int arity, Delegate function, int flags)
		{ return SafeNativeMethods.PL_register_foreign_in_module(module, name, arity, function, flags); }


		internal static IntPtr PL_create_engine(IntPtr attr)
		{ return SafeNativeMethods.PL_create_engine(attr); }

		internal static int PL_set_engine(IntPtr engine, ref IntPtr old)
		{ return SafeNativeMethods.PL_set_engine(engine, ref old); }

		internal static int PL_destroy_engine(IntPtr engine)
		{ return SafeNativeMethods.PL_destroy_engine(engine); }


        internal static uintptr_t PL_new_atom_wchars(string text)
	    {
	        int len = text.Length;
	        return SafeNativeMethods.PL_new_atom_wchars(len, text);
	    }

        internal static String PL_atom_wchars(uintptr_t tAtom)
        {
            // see http://www.mycsharp.de/wbb2/thread.php?threadid=51100
            int dummyLen = 0;
            return Marshal.PtrToStringUni(SafeNativeMethods.PL_atom_wchars(tAtom, ref dummyLen));
        }



		/********************************
		*         QUERY PROLOG          *
		*********************************/

        // see http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_query()
// ReSharper disable InconsistentNaming
        internal const int PL_QUERY_ARGC = 1;	            /* return main() argc */
        internal const int PL_QUERY_ARGV = 2;	            /* return main() argv */
					                        /* 3: Obsolete PL_QUERY_SYMBOLFILE */
					                        /* 4: Obsolete PL_QUERY_ORGSYMBOLFILE*/
        internal const int PL_QUERY_GETC = 5;	            /* Read character from terminal */
        internal const int PL_QUERY_MAX_INTEGER = 6;	    /* largest integer */
        internal const int PL_QUERY_MIN_INTEGER = 7;	    /* smallest integer */
        internal const int PL_QUERY_MAX_TAGGED_INT = 8;	    /* largest tagged integer */
        internal const int PL_QUERY_MIN_TAGGED_INT = 9;	    /* smallest tagged integer */
        internal const int PL_QUERY_VERSION = 10;	        /* 207006 = 2.7.6 */
        internal const int PL_QUERY_MAX_THREADS = 11;	    /* maximum thread count */
        internal const int PL_QUERY_ENCODING = 12;	        /* I/O encoding */
        internal const int PL_QUERY_USER_CPU = 13;	        /* User CPU in milliseconds */
// ReSharper restore InconsistentNaming


        // get information from Prolog
        internal static int PL_query(uint queryType)
		{ return SafeNativeMethods.PL_query(queryType); }


		// PlFrame
        internal static uintptr_t PL_open_foreign_frame()
		{ return SafeNativeMethods.PL_open_foreign_frame(); }

        internal static void PL_close_foreign_frame(uintptr_t fid_t)
		{ 
			if(IsValid)
				SafeNativeMethods.PL_close_foreign_frame(fid_t); 
		}

        internal static void PL_rewind_foreign_frame(uintptr_t fid_t)
		{ SafeNativeMethods.PL_rewind_foreign_frame(fid_t); }

        // record 

        internal static uintptr_t PL_record(uintptr_t term_t)
        { return SafeNativeMethods.PL_record(term_t); }

        internal static void PL_recorded(uintptr_t record_t, [Out]uintptr_t term_t)    // term_t - ( ausgabe )
        { SafeNativeMethods.PL_recorded(record_t, term_t); }

        internal static void PL_erase(uintptr_t record_t)
        { SafeNativeMethods.PL_erase(record_t); }

		// PlQuery

        internal static int PL_next_solution(uintptr_t qid_t)
		{ return SafeNativeMethods.PL_next_solution(qid_t); }

		// TODO wchar
        internal static IntPtr PL_predicate(string name, int arity, string module)
		{ return SafeNativeMethods.PL_predicate(name, arity, module); }

        internal static uintptr_t PL_open_query(IntPtr module, int flags, IntPtr pred, uintptr_t term)
		{ return SafeNativeMethods.PL_open_query(module, flags, pred, term); }

        internal static void PL_cut_query(uintptr_t qid)
        {
            if (IsValid)
                SafeNativeMethods.PL_cut_query(qid);
        }
        internal static void PL_close_query(uintptr_t qid)
        {
            if (IsValid)
                SafeNativeMethods.PL_close_query(qid);
        }

		// PlTerm

        internal static uintptr_t PL_new_term_ref()
		{ return SafeNativeMethods.PL_new_term_ref(); }

        internal static void PL_put_integer(uintptr_t term, int i)
		{ SafeNativeMethods.PL_put_integer(term, i); }

        internal static void PL_put_float(uintptr_t term, double i)
		{ SafeNativeMethods.PL_put_float(term, i); }

        internal static int PL_get_wchars(uintptr_t term, out string s, uint flags)
        {
            var dummyLen = 0;
            var ps = IntPtr.Zero;
            var iRet = SafeNativeMethods.PL_get_wchars(term, ref dummyLen, ref ps, flags);
            s = Marshal.PtrToStringUni(ps);
            return iRet;
        }

        internal static int PL_get_long(uintptr_t term, ref int i)
		{ return SafeNativeMethods.PL_get_long(term, ref i); }

        internal static int PL_get_float(uintptr_t term, ref double i)
		{ return SafeNativeMethods.PL_get_float(term, ref i); }

        internal static int PL_term_type(uintptr_t t)
		{ return SafeNativeMethods.PL_term_type(t); }

		// COMPARE
        internal static int PL_compare(uintptr_t term1, uintptr_t term2)
		{ return SafeNativeMethods.PL_compare(term1, term2); }



		// PlTermV
        internal static uintptr_t PL_new_term_refs(int n)
		{ return SafeNativeMethods.PL_new_term_refs(n); }

        internal static void PL_put_term(uintptr_t t1, uintptr_t t2)
		{ SafeNativeMethods.PL_put_term(t1, t2); }

	    internal static int PL_wchars_to_term(string chars, uintptr_t term)
	    {
            return SafeNativeMethods.PL_wchars_to_term(chars, term);
	    }

        internal static void PL_cons_functor_v(uintptr_t term, uintptr_t functor_t, uintptr_t termA0)
		{ SafeNativeMethods.PL_cons_functor_v(term, functor_t, termA0); }

        internal static uintptr_t PL_new_functor(uintptr_t atomA, int a)
		{ return SafeNativeMethods.PL_new_functor(atomA, a); }


        // Testing the type of a term
		// all return non zero if condition succeed
        internal static int PL_is_variable(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_variable(term_t); }

        internal static int PL_is_ground(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_ground(term_t); }

        internal static int PL_is_atom(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_atom(term_t); }

        internal static int PL_is_string(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_string(term_t); }

        internal static int PL_is_integer(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_integer(term_t); }

        internal static int PL_is_float(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_float(term_t); }

        internal static int PL_is_compound(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_compound(term_t); }

        internal static int PL_is_list(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_list(term_t); }

        internal static int PL_is_atomic(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_atomic(term_t); }

        internal static int PL_is_number(uintptr_t term_t)
		{ return SafeNativeMethods.PL_is_number(term_t); }

		// LISTS (PlTail)

        internal static uintptr_t PL_copy_term_ref(uintptr_t term_t)
		{ return SafeNativeMethods.PL_copy_term_ref(term_t); }

        internal static int PL_unify_list(uintptr_t termL, uintptr_t termH, uintptr_t termT)
		{ return SafeNativeMethods.PL_unify_list(termL, termH, termT); }

        internal static int PL_unify_nil(uintptr_t term_t)
		{ return SafeNativeMethods.PL_unify_nil(term_t); }

        internal static int PL_get_list(uintptr_t termL, uintptr_t termH, uintptr_t termT)
		{ return SafeNativeMethods.PL_get_list(termL, termH, termT); }

        internal static int PL_get_nil(uintptr_t term_t)
		{ return SafeNativeMethods.PL_get_nil(term_t); }

        internal static int PL_unify(uintptr_t t1, uintptr_t t2)
        { return SafeNativeMethods.PL_unify(t1, t2); }

	    /// <summary>
	    /// Unify t with a textual representation of the C wide-character array s. 
	    /// The type argument defines the Prolog representation and is one of PL_ATOM, PL_STRING, PL_CODE_LIST or PL_CHAR_LIST.
	    /// </summary>
        /// <param name="t1"></param>
        /// <param name="type"></param>
        /// <param name="wchars"></param>
	    /// <returns></returns>
        internal static int PL_unify_wchars(uintptr_t t1, PlType type, string wchars)
        {
            return PL_unify_wchars(t1, type, wchars.Length, wchars);
        }

        internal static int PL_unify_wchars(uintptr_t t1, PlType type, int len, string wchars)
        {
            Check.Require(type == PlType.PlAtom || type == PlType.PlString || type == PlType.PlCharList || type == PlType.PlCodeList);
            return SafeNativeMethods.PL_unify_wchars(t1, (int)type, len, wchars);
        }


		// Exceptions
		// Handling exceptions
        internal static uintptr_t PL_exception(uintptr_t qid)
		{ return SafeNativeMethods.PL_exception(qid); }

        internal static int PL_raise_exception(uintptr_t exceptionTerm)
		{ return SafeNativeMethods.PL_raise_exception(exceptionTerm); }

        internal static int PL_get_arg(int index, uintptr_t t, uintptr_t a)
		{ return SafeNativeMethods.PL_get_arg(index, t, a); }

        internal static int PL_get_name_arity(uintptr_t t, ref uintptr_t name, ref int arity)
		{ return SafeNativeMethods.PL_get_name_arity(t, ref name, ref arity); }

		// ******************************
		// *	  PROLOG THREADS		*
		// ******************************
		internal static int PL_thread_self()
		{ return SafeNativeMethods.PL_thread_self(); }

		internal static int PL_thread_attach_engine(IntPtr attr)
		{ return SafeNativeMethods.PL_thread_attach_engine(attr); }

		internal static int PL_thread_destroy_engine()
		{ return SafeNativeMethods.PL_thread_destroy_engine(); }


        // ******************************
        // *	  PROLOG STREAM's		*
        // ******************************

        //internal static int Slinesize()
        //{ return SafeNativeMethods.SlineSize(); }

        //internal static IntPtr S__getiob()
        //{ return SafeNativeMethods.S__getiob(); }


        // <returns> a SWI-PROLOG IOSTREAM defined in spl-stream.h</returns>
        //internal static IntPtr Snew()
        //{ return SafeNativeMethods.Snew(IntPtr.Zero, 0, IntPtr.Zero); }


        // from pl-itf.h
        // PL_EXPORT(int)  	PL_unify_stream(term_t t, IOSTREAM *s);
        //internal static int PL_unify_stream(uintptr_t term_t, IntPtr iostream)
        //{ return SafeNativeMethods.PL_unify_stream(term_t, iostream); }


	} // LibPl
	#endregion


} // namespace SbsSW.SwiPlCs
