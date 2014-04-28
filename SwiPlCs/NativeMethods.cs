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

// The _LINUX part is from Batu Akan from Sweden. Thank you very much. (Linux Support with Mono)
// Uncomment the following line to compile on Linux or MacOS
// #define _LINUX

/*
http://www.codeproject.com/csharp/legacyplugins.asp
 * http://www.cnblogs.com/Dah/archive/2007/01/07/614040.html
 * ggf. 
 * http://www.pcreview.co.uk/forums/thread-2241486.php
 * http://www.msnewsgroups.net/group/microsoft.public.dotnet.languages.csharp/topic12656.aspx
 * 
 * tool to generate the pinvoke lines from Visual Studio 2005
 * http://www.pinvoke.net/
 * */



using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.ConstrainedExecution;



namespace SbsSW.SwiPlCs
{

    [StructLayout(LayoutKind.Explicit)]
    internal struct uintptr_t
    {
        [FieldOffset(0)]
#if _PL_X64
        private System.UInt64 _uintptr;
        internal uintptr_t(ulong l)
        {
            this._uintptr = l;
        }
        public static uintptr_t operator +(uintptr_t term1, ulong l)
        {
            return new uintptr_t(term1._uintptr + l);
        }
#else
        private UInt32 _uintptr;
        internal uintptr_t(uint i)
        {
            _uintptr = i;
        }
        public static uintptr_t operator +(uintptr_t term1, ulong l)
        {
            return new uintptr_t(term1._uintptr + (uint)l);
        }
#endif

        public override int GetHashCode()
        {
            return _uintptr.GetHashCode();
        }
        /// <inheritdoc />
        public override bool Equals(Object obj)
        {
            if (obj is uintptr_t)
                return this == ((uintptr_t)obj);
            if (obj is int)
                return this == ((int)obj);
            return false;
        }

        public static bool operator ==(uintptr_t term1, uintptr_t term2)
        {
            return term1._uintptr == term2._uintptr;
        }
        public static bool operator !=(uintptr_t term1, uintptr_t term2)
        {
            return term1._uintptr != term2._uintptr;
        }
        public static bool operator ==(uintptr_t term1, int term2)
        {
            return term1._uintptr == (ulong)term2;
        }
        public static bool operator ==(int term2, uintptr_t term1)
        {
            return term1 == term2;
        }
        public static bool operator !=(uintptr_t term1, int term2)
        {
            return term1._uintptr != (ulong)term2;
        }
        public static bool operator !=(int term2, uintptr_t term1)
        {
            return term1 != term2;
        }

        //---------------
        public static bool operator >(uintptr_t term1, ulong term2)
        {
            return term1._uintptr > term2;
        }
        public static bool operator <(uintptr_t term1, ulong term2)
        {
            return term1._uintptr < term2;
        }

        //---------------
        // assignment = operator
        public static implicit operator uintptr_t(long term2)
        {
            uintptr_t x;
#if _PL_X64
            x._uintptr = (ulong)term2;
#else
            x._uintptr = (uint)term2;
#endif
            return x;
        }

    }



	#region Safe Handles and Native imports
	// See http://msdn.microsoft.com/msdnmag/issues/05/10/Reliability/ for more about safe handles.
	[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
	sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		private SafeLibraryHandle() : base(true) { }

		protected override bool ReleaseHandle()
		{
			return NativeMethods.UnLoadDll(handle);
		}

		public bool UnLoad()
		{
			return ReleaseHandle();
		}

	}

	internal static class NativeMethods
	{
#if _LINUX
#warning _LINUX is defined (libdl.so must be available). Compiling for Linux
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        //Linux Compatibility		
        public static int RTLD_NOW = 2; // for dlopen's flags

        const string s_kernel_linux = "libdl.so";

        [DllImport(s_kernel_linux, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern SafeLibraryHandle dlopen([In] string filename, [In] int flags);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(s_kernel_linux, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool dlclose(IntPtr hModule);

        // see: http://blogs.msdn.com/jmstall/archive/2007/01/06/Typesafe-GetProcAddress.aspx
        [DllImport(s_kernel_linux, CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
        internal static extern IntPtr dlsym(SafeLibraryHandle hModule, String procname);
        
#else
#warning _LINUX is *not* defined (kernel32.dll must be available). Compiling for Windows
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
		//Windows
        const string WinKernel32 = "kernel32";
        [DllImport(WinKernel32, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
		private static extern SafeLibraryHandle LoadLibrary(string fileName);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(WinKernel32, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool FreeLibrary(IntPtr hModule);

        // see: http://blogs.msdn.com/jmstall/archive/2007/01/06/Typesafe-GetProcAddress.aspx
        [DllImport(WinKernel32, CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, String procname);
#endif		
		
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
		//Platform independant function calls
		internal static SafeLibraryHandle LoadDll(string filename) {
#if _LINUX
            //if (Environment.OSVersion.Platform == PlatformID.Unix ||
            //    Environment.OSVersion.Platform == PlatformID.MacOSX) {
                return dlopen(filename, RTLD_NOW);
            //}
#else
            return LoadLibrary(filename); 		
#endif
        }

		public static bool UnLoadDll(IntPtr hModule) {
#if _LINUX
            return dlclose(hModule);
#else
            return FreeLibrary(hModule);		
#endif
        }

//        internal static IntPtr GetProcHandle(SafeLibraryHandle hModule, String procname)
//        {
//#if _LINUX
//            return dlsym(hModule, procname);
//#else
//            return GetProcAddress(hModule, procname);		
//#endif
//        }


        internal static IntPtr GetPoninterOfIoFunctions(SafeLibraryHandle hModule)
        {
#if _LINUX
            return dlsym(hModule, "S__iob");
#else
            return GetProcAddress(hModule, "S__iob");
#endif
        }
		
		
	}
	#endregion // Safe Handles and Native imports



	// for details see http://msdn2.microsoft.com/en-us/library/06686c8c-6ad3-42f7-a355-cbaefa347cfc(vs.80).aspx
	// and http://blogs.msdn.com/fxcop/archive/2007/01/14/faq-how-do-i-fix-a-violation-of-movepinvokestonativemethodsclass.aspx

	//NativeMethods - This class does not suppress stack walks for unmanaged code permission. 
	//    (System.Security.SuppressUnmanagedCodeSecurityAttribute must not be applied to this class.) 
	//    This class is for methods that can be used anywhere because a stack walk will be performed.

	//SafeNativeMethods - This class suppresses stack walks for unmanaged code permission. 
	//    (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
	//    This class is for methods that are safe for anyone to call. Callers of these methods are not 
	//    required to do a full security review to ensure that the usage is secure because the methods are harmless for any caller.

	//UnsafeNativeMethods - This class suppresses stack walks for unmanaged code permission. 
	//    (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
	//    This class is for methods that are potentially dangerous. Any caller of these methods must do a 
	//    full security review to ensure that the usage is secure because no stack walk will be performed.


	[ System.Security.SuppressUnmanagedCodeSecurityAttribute ]
	internal static class SafeNativeMethods
	{
        private const string DllFileName = "libswipl.dll";
		
		public static string DllFileName1
		{
			get 
			{ 
				if (Environment.OSVersion.Platform == PlatformID.Unix ||
                	Environment.OSVersion.Platform == PlatformID.MacOSX) 
				{
                	return "libpl.so";
            	}
				return DllFileName; 
			}
		} 


		
		/////////////////////////////
		/// libpl
		///
        // das funktioniert NICHT wenn umlaute e.g. ü im pfad sind.
        // TODO wchar
        [DllImport(DllFileName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_initialise(int argc, String[] argv);
			// PL_EXPORT(int)		PL_is_initialised(int *argc, char ***argv);
        //[DllImport(DllFileName)]
        //internal static extern int PL_is_initialised([In, Out] ref int argc, [In, Out] ref String[] argv);
		[DllImport(DllFileName)]
		internal static extern int PL_is_initialised(IntPtr argc, IntPtr argv);
		[DllImport(DllFileName)]
		internal static extern int PL_halt(int i);

        [DllImport(DllFileName)]
        // The function returns TRUE if successful and FALSE otherwise. Currently, FALSE is returned when an attempt is 
        // made to call PL_cleanup() recursively or if PL_cleanup() is not called from the main-thread. 
        // int PL_cleanup(int status)
        internal static extern int PL_cleanup(int status);


            // PL_EXPORT(int)		PL_register_foreign_in_module(const char *module, const char *name, int arity, pl_function_t func, int flags);
            // typedef unsigned long	foreign_t
        // int PL_register_foreign_in_module(const char *module, const char *name, int arity, foreign_t (*function)(), int flags)
        // TODO wchar
        [DllImport(DllFileName, CallingConvention=CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_register_foreign_in_module(string module, string name, int arity, Delegate function, int flags);



		//	 ENGINES (MT-ONLY)
		// TYPES :  PL_engine_t			-> void *
		//			PL_thread_attr_t	-> struct
		[DllImport(DllFileName)]
			// PL_EXPORT(PL_engine_t)	PL_create_engine(PL_thread_attr_t *attributes);
		internal static extern IntPtr PL_create_engine(IntPtr attr);
		[DllImport(DllFileName)]	// PL_EXPORT(int)		PlSetEngine(PL_engine_t engine, PL_engine_t *old);
		internal static extern int PL_set_engine(IntPtr engine, [In, Out] ref IntPtr old);
		[DllImport(DllFileName)]	// PL_EXPORT(int)		PL_destroy_engine(PL_engine_t engine);
		internal static extern int PL_destroy_engine(IntPtr engine);

        // atom_t PL_new_atom_nchars(size_t len, const char *s)
        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern uintptr_t PL_new_atom_wchars(Int32 len, string text);

        // pl_wchar_t* PL_atom_wchars(atom_t atom, int *len)
        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)] // return const char *
        internal static extern IntPtr PL_atom_wchars(uintptr_t atom, [In, Out] ref int len);
        
        // Pl_Query
        [DllImport(DllFileName)]
        internal static extern int PL_query(uint plQuerySwitch);
        
        // PlFrame
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_open_foreign_frame();
        [DllImport(DllFileName)]
        internal static extern void PL_close_foreign_frame(uintptr_t fid_t);
        [DllImport(DllFileName)]
        internal static extern void PL_rewind_foreign_frame(uintptr_t fid_t);
        // record recorded erase
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_record(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern void PL_recorded(uintptr_t record_t, uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern void PL_erase(uintptr_t record_t);
        // PlQuery
		[DllImport(DllFileName)]
        internal static extern int PL_next_solution(uintptr_t qid_t);
        // TODO wchar
        [DllImport(DllFileName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		internal static extern IntPtr PL_predicate(string name, int arity, string module);
		[DllImport(DllFileName)]
			//qid_t PL_open_query(module_t m, int flags, predicate_t pred, term_t t0);
        internal static extern uintptr_t PL_open_query(IntPtr module, int flags, IntPtr pred, uintptr_t term);
        [DllImport(DllFileName)]
        internal static extern void PL_cut_query(uintptr_t qid);
        [DllImport(DllFileName)]
        internal static extern void PL_close_query(uintptr_t qid);
	
		// PlTerm
		//__pl_export term_t	PL_new_term_ref(void);
		[DllImport(DllFileName)] // return term_t
        internal static extern uintptr_t PL_new_term_ref();
		//__pl_export void	PL_put_integer(term_t term, long i);
		[DllImport(DllFileName)]
        internal static extern void PL_put_integer(uintptr_t term, int i);
		[DllImport(DllFileName)]
        internal static extern void PL_put_float(uintptr_t term, double i);

        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_get_wchars(uintptr_t term, [In, Out]ref int len, [In, Out]ref IntPtr s, uint flags);



		// __pl_export int		PL_get_long(term_t term, long *i);
		[DllImport(DllFileName)]
        internal static extern int PL_get_long(uintptr_t term, [In, Out] ref int i);
		// __pl_export int		PL_get_float(term_t term, double *f);
		[DllImport(DllFileName)]
        internal static extern int PL_get_float(uintptr_t term, [In, Out] ref double i);
		//__pl_export int		PL_term_type(term_t term);
		[DllImport(DllFileName)]
        internal static extern int PL_term_type(uintptr_t t);

		// COMPARE
		//__pl_export int		PL_compare(term_t t1, term_t t2);
		[DllImport(DllFileName)]
        internal static extern int PL_compare(uintptr_t term1, uintptr_t term2);

 

		// PlTermV
		[DllImport(DllFileName)] // return term_t
        internal static extern uintptr_t PL_new_term_refs(int n);
		//__pl_export void	PL_put_term(term_t t1, term_t t2);
		[DllImport(DllFileName)]
        internal static extern void PL_put_term(uintptr_t t1, uintptr_t t2);

		// PlCompound
        // __pl_export int PL_wchars_to_term(const pl_wchar_t *chars, term_t term);
        // __pl_export int PL_chars_to_term(const char *chars, term_t term);
        //__pl_export void	PL_cons_functor_v(term_t h, functor_t fd, term_t A0);
		//__pl_export functor_t	PL_new_functor(atom_t f, int atom);

        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_wchars_to_term([In]string chars, uintptr_t term);
        [DllImport(DllFileName)]
        internal static extern void PL_cons_functor_v(uintptr_t term, uintptr_t functor_t, uintptr_t termA0);
		[DllImport(DllFileName)]
        internal static extern uintptr_t PL_new_functor(uintptr_t atom, int a);

		// Testing the type of a term
		//__pl_export int		PL_is_variable(term_t term);
		//__pl_export int		PL_is_list(term_t term);
		// ...
		[DllImport(DllFileName)]
        internal static extern int PL_is_variable(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_ground(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_atom(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_string(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_integer(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_float(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_compound(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_list(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_atomic(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_is_number(uintptr_t term_t);

		// LISTS (PlTail)
		//__pl_export term_t	PL_copy_term_ref(term_t from);
		//__pl_export int		PL_unify_list(term_t l, term_t h, term_t term);
		//__pl_export int		PL_unify_nil(term_t l);
		//__pl_export int		PL_get_list(term_t l, term_t h, term_t term);
		//__pl_export int		PL_get_nil(term_t l);
		// __pl_export int		PL_unify(term_t t1, term_t t2);
		[DllImport(DllFileName)]
        internal static extern uintptr_t PL_copy_term_ref(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_unify_list(uintptr_t termL, uintptr_t termH, uintptr_t termT);
		[DllImport(DllFileName)]
        internal static extern int PL_unify_nil(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_get_list(uintptr_t termL, uintptr_t termH, uintptr_t termT);
		[DllImport(DllFileName)]
        internal static extern int PL_get_nil(uintptr_t term_t);
		[DllImport(DllFileName)]
        internal static extern int PL_unify(uintptr_t t1, uintptr_t t2);
        //__pl_export int PL_unify_wchars(term_t t, int type, size_t len, const pl_wchar_t *s)
        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_unify_wchars(uintptr_t t1, int type, int len, string atom);


		// Exceptions
		// Handling exceptions
		//__pl_export term_t	PL_exception(qid_t _qid);
		//__pl_export int		PL_raise_exception(term_t exception);
		//__pl_export int		PL_throw(term_t exception);
		[DllImport(DllFileName)]
        internal static extern uintptr_t PL_exception(uintptr_t qid);
		[DllImport(DllFileName)]
        internal static extern int PL_raise_exception(uintptr_t exceptionTerm);
		//__pl_export int		PL_get_arg(int index, term_t term, term_t atom);
		[DllImport(DllFileName)]
        internal static extern int PL_get_arg(int index, uintptr_t t, uintptr_t a);
		//__pl_export int		PL_get_name_arity(term_t term, atom_t *Name, int *Arity);
		[DllImport(DllFileName)]
        internal static extern int PL_get_name_arity(uintptr_t t, ref uintptr_t name, ref int arity);

		// ******************************
		// *	  PROLOG THREADS		*
		// ******************************

		// from file pl-itf.h
		/*
		typedef struct
				{
					unsigned long	    local_size;		// Stack sizes
					unsigned long	    global_size;
					unsigned long	    trail_size;
					unsigned long	    argument_size;
					char *	    alias;					// alias Name
				} PL_thread_attr_t;
		*/
		//PL_EXPORT(int)	PL_thread_self(void);	/* Prolog thread id (-1 if none) */
		//PL_EXPORT(int)	PL_thread_attach_engine(PL_thread_attr_t *attr);
		//PL_EXPORT(int)	PL_thread_destroy_engine(void);
		//PL_EXPORT(int)	PL_thread_at_exit(void (*function)(void *), void *closure, int global);
		[DllImport(DllFileName)]
		internal static extern int PL_thread_self();
		[DllImport(DllFileName)]
		internal static extern int PL_thread_attach_engine(IntPtr attr);
		//internal static extern int PL_thread_attach_engine(ref PL_thread_attr_t attr);
		[DllImport(DllFileName)]
		internal static extern int PL_thread_destroy_engine();



        // ******************************
        // *	  PROLOG STREAM's		*
        // ******************************


        #region structurs

        // int Slinesize

        // IOFUNCTIONS  Sfilefunctions



        /*
         * long ssize_t
         * 
        typedef ssize_t (*Sread_function)(void *handle, char *buf, size_t bufsize);
        typedef ssize_t (*Swrite_function)(void *handle, char*buf, size_t bufsize);
        typedef long  (*Sseek_function)(void *handle, long pos, int whence);
        typedef int64_t (*Sseek64_function)(void *handle, int64_t pos, int whence);
        typedef int   (*Sclose_function)(void *handle);
        typedef int   (*Scontrol_function)(void *handle, int action, void *arg);


        typedef struct io_functions
        { Sread_function	read;		//* fill the buffer
          Swrite_function	write;		//* empty the buffer 
          Sseek_function	seek;		//* seek to position 
          Sclose_function	close;		//* close stream 
          Scontrol_function	control;	//* Info/control 
          Sseek64_function	seek64;		//* seek to position (intptr_t files) 
        } IOFUNCTIONS;
        */

        
        // IOSTREAM    S__iob[3]
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct MIOSTREAM
        {
            /*
            char		    *bufp;		    // `here'
            char		    *limitp;		    // read/write limit 
            char		    *buffer;		    // the buffer 
            char		    *unbuffer;	    // Sungetc buffer 
            int			    lastc;		    // last character written 
            int			    magic;		    // magic number SIO_MAGIC 
            int  			bufsize;	    // size of the buffer 
            int			    flags;		    // Status flags 
            IOPOS			posbuf;		    // location in file 
            IOPOS *		    position;	    // pointer to above 
            IntPtr	        *handle;		    // function's handle 
            MIOFUNCTIONS	*functions;	    // open/close/read/write/seek 
            int		        locks;		    // lock/unlock count 
            */
            //IOLOCK *		    mutex;		    // stream mutex 
            readonly IntPtr mutex;

            readonly long[] place_holder_1;
					            // SWI-Prolog 4.0.7 
              //void			    (*close_hook)(void* closure);
              //void *		    closure;
              //                  // SWI-Prolog 5.1.3 
              //int			    timeout;	    // timeout (milliseconds) 
              //                  // SWI-Prolog 5.4.4 
              //char *		    message;	    // error/warning message 
              //IOENC			    encoding;	    // character encoding used 
              //struct io_stream *	tee;		// copy data to this stream 
              //mbstate_t *		mbstate;	    // ENC_ANSI decoding 
              //struct io_stream *	upstream;	// stream providing our input 
              //struct io_stream *	downstream;	// stream providing our output 
              //unsigned		    newline : 2;	// Newline mode 
              //void *		    exception;	    // pending exception (record_t) 
              //intptr_t		    reserved[2];	// reserved for extension 
        };

        /*

         * 
typedef struct io_position
{ int64_t		byteno;		// byte-position in file 
  int64_t		charno;		// character position in file 
  int			lineno;		// lineno in file 
  int			linepos;	// position in line 
  intptr_t		reserved[2];	// future extensions 
} IOPOS;

         * 
typedef struct io_stream{ 
  char		       *bufp;		    // `here'
  char		       *limitp;		    // read/write limit 
  char		       *buffer;		    // the buffer 
  char		       *unbuffer;	    // Sungetc buffer 
  int			    lastc;		    // last character written 
  int			    magic;		    // magic number SIO_MAGIC 
  int  			    bufsize;	    // size of the buffer 
  int			    flags;		    // Status flags 
  IOPOS			    posbuf;		    // location in file 
  IOPOS *		    position;	    // pointer to above 
  void		       *handle;		    // function's handle 
  IOFUNCTIONS	   *functions;	    // open/close/read/write/seek 
  int		        locks;		    // lock/unlock count 
  IOLOCK *		    mutex;		    // stream mutex 
					// SWI-Prolog 4.0.7 
  void			    (*close_hook)(void* closure);
  void *		    closure;
					// SWI-Prolog 5.1.3 
  int			    timeout;	    // timeout (milliseconds) 
					// SWI-Prolog 5.4.4 
  char *		    message;	    // error/warning message 
  IOENC			    encoding;	    // character encoding used 
  struct io_stream *	tee;		// copy data to this stream 
  mbstate_t *		mbstate;	    // ENC_ANSI decoding 
  struct io_stream *	upstream;	// stream providing our input 
  struct io_stream *	downstream;	// stream providing our output 
  unsigned		    newline : 2;	// Newline mode 
  void *		    exception;	    // pending exception (record_t) 
  intptr_t		    reserved[2];	// reserved for extension 
} IOSTREAM;

         */

        #endregion structurs

      

	} // class SafeNativeMethods

} // namespace SbsSW.SwiPlCs
