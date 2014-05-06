/*********************************************************
* 
*  Author:        Uwe Lesta
*  Copyright (C): 2008-2014, Uwe Lesta SBS-Softwaresysteme GmbH
*
*  Part of SwiPlCs
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



// =============================================================================
//
// The values calculated here are used in swi-pl-cs (LibPl.cs SetStreamFunction)
//
// =============================================================================


#include <stdio.h>
// to activate the define 
//#define __CYGWIN__ 1
#include <include/SWI-Stream.h>
// SWI_HOME_DIR has to be set as an environment variable to the SWI-Prolog installation directory 
// it is set as a sarch path in the project properties - Configuration Properties - VC++ Directories - include Directories
// #include "D:/Lesta/swi-pl/pl/include/SWI-prolog.h"



typedef struct io_stream_test
{ char		       *bufp;		/* `here' */
  char		       *limitp;		/* read/write limit */
  char		       *buffer;		/* the buffer */
  char		       *unbuffer;	/* Sungetc buffer */
  int			lastc;		/* last character written */
  int			magic;		/* magic number SIO_MAGIC */
  int  			bufsize;	/* size of the buffer */
  int			flags;		/* Status flags */
  IOPOS			posbuf;		/* location in file */
  IOPOS *		position;	/* pointer to above */
  void		       *handle;		/* function's handle */
} XXXX;


int main(int argc, char* argv[])
{
	printf("size of struct io_stream %ld \n", sizeof(io_stream));

	// to calculate the size and offset of S__iob 
	// needed in SwiPlCs (LibPl.cs SetStreamFunction)
	int ttt = sizeof(XXXX);
	int x = sizeof(S__iob);
	int s = sizeof(IOSTREAM);
	int off_functions = 4*sizeof(char*) + 4*sizeof(int) + sizeof(IOPOS) + sizeof(IOPOS*) + sizeof(void*);
	int iof = sizeof(IOFUNCTIONS);
	int single_func_ptr = sizeof(Sread_function);
	int look = Sinput->locks;

	printf("size of struct io_stream_test %d \n", ttt);
	printf("size S__iob %d \n", x);
	printf("size off_functions %d \n", off_functions);
	printf("size IOFUNCTIONS %d \n", iof);
	printf("size single_func_ptr %d \n", single_func_ptr);

    //size_t val_of_size_t;
	printf("size val_of_size_t %d \n", sizeof(size_t));
	printf("size short %d \n", sizeof(short));
	printf("size int %d \n", sizeof(int));
	printf("size long %d \n", sizeof(long));
	printf("size uintptr_t %d \n", sizeof(uintptr_t));

	// Asserts
#if _PL_X64
	if(232 != sizeof(io_stream))
		printf("ERROR 232 != io_stream\n");
	if(104 != ttt)
		printf("ERROR 104 != io_stream_test\n");
	if(696 != x)
		printf("ERROR 696 != S__iob\n");
	if(104 != off_functions)
		printf("ERROR 104 != off_functions\n");
	if(48 != iof)
		printf("ERROR 48 != IOFUNCTIONS (iof)\n");
	if(8 != single_func_ptr)
		printf("ERROR 8 != single_func_ptr\n");
#else
	if(144 != sizeof(io_stream))
		printf("ERROR 144 != io_stream\n");
	if(72 != ttt)
		printf("ERROR 72 != io_stream_test\n");
	if(432 != x)
		printf("ERROR 432 != S__iob\n");
	if(72 != off_functions)
		printf("ERROR 72 != off_functions\n");
	if(24 != iof)
		printf("ERROR 24 != IOFUNCTIONS (iof)\n");
	if(4 != single_func_ptr)
		printf("ERROR 4 != single_func_ptr\n");
#endif


   // long l = PL_query(PL_QUERY_VERSION);

	return 0;
}
