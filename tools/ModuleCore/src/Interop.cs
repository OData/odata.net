//---------------------------------------------------------------------
// <copyright file="Interop.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;      //IndexerName
using System.Security;


//Note: Currently TLBIMP.EXE has a bug where it doesn't allow the user to specify the 
//AllowsPartialTrustedCaller attribute, so we cannot run our tests in anything 
//but FullTrust, becuase of the interop.  So for now we will write our own interop
//wrapper.  Eventually this code could also just be compiled into the ModuleCore.dll,
//but since all the tests currently link with dll already, we'll just built them here...

namespace Microsoft.Test.ModuleCore
{
//  internal class Interop 
//	{
	    ////////////////////////////////////////////////////////////////
	    // LtmContext
	    //
	    // Managed wrapper around Ltm context object
	    ////////////////////////////////////////////////////////////////
	    [ComImport(), Guid("DAA3A3F6-B08B-11d1-A971-00C04F94A717"), ClassInterface(ClassInterfaceType.None)]
	    public class LtmContext
	    {
	    }

		////////////////////////////////////////////////////////////////////////
		// TestResult
		//
		////////////////////////////////////////////////////////////////////////
		public enum TestResult
		{
			Failed = 0,
			Passed,
			Skipped,
			NonExistent,
			Unknown,
			Timeout,
			Warning,
			Exception,
			Aborted,
			Assert
		};

		////////////////////////////////////////////////////////////////////////
		// TestPropertyFlags
		//
		////////////////////////////////////////////////////////////////////////
		public enum TestPropertyFlags
		{
			Read			= 0x00000000,
			Write			= 0x00000001,
			Required		= 0x00000010,
			Inheritance	    = 0x00000100,
			MultipleValues	= 0x00001000,
			DefaultValue	= 0x00002000,
			Visible		    = 0x00010000,
		};

		////////////////////////////////////////////////////////////////////////
		// TestType
		//
		////////////////////////////////////////////////////////////////////////
		public enum TestType
		{
			TestSuite		= 0,
			TestModule		= 1,
			TestCase		= 2,
			TestVariation	= 3,
		};

        ////////////////////////////////////////////////////////////////////////
		// TestFlags
		//
		////////////////////////////////////////////////////////////////////////
		public enum TestFlags
		{
		};

		////////////////////////////////////////////////////////////////////////
		// TestMethod
		//
		////////////////////////////////////////////////////////////////////////
        public enum TestMethod
        {
	        Init			= 0,
	        Terminate		= 1,
	        Execute			= 2,
        };

		////////////////////////////////////////////////////////////////////////
		// TestLogFlags
		//
		////////////////////////////////////////////////////////////////////////
		public enum TestLogFlags
		{
			Raw		= 0x00000000,	//No fixup - Don't use, unless you know the text contains no CR/LF, no Xml reserverd tokens, or no other non-respresentable characters
			Text	= 0x00000001,	//Default  - Automatically fixup CR/LF correctly for log files, fixup xml tokens, etc
			Xml		= 0x00000002,	//For Xml  - User text is placed into a CDATA section (with no xml fixups)
			Ignore	= 0x00000004,	//Ignore   - User text is placed into ignore tags (can combine this with console_xml as well)
		    Trace   = 0x00000010,	//Trace    - User text is not displayed unless epxlicitly enabled
		};

        ////////////////////////////////////////////////////////////////////////
		// ITestItem
		//
		////////////////////////////////////////////////////////////////////////
        [ComImport, Guid("A40464E3-320C-4a0d-BFE2-112BF04F6202"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurityAttribute()]
        public interface ITestItem
		{
			// Simple Meta-data about the item
			int							Id  		{ get;	}
			String						Guid		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Name		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Desc		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Owner		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Version		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			int							Priority	{ get;	}
			TestType         			Type		{ get;	}
			TestFlags         			Flags		{ get;	}

			// Extensible Meta-data about the item
			ITestProperties				Metadata	{ [return: MarshalAs(UnmanagedType.Interface)] get;	}

			// Children (testcases, variations, etc)
			ITestItems					Children	{ [return: MarshalAs(UnmanagedType.Interface)] get;	}

			// Execution
			//		Control Flow:	Init->Execute->(recurse into children)->Terminate
			TestResult				    Init		();
			TestResult  				Execute		();
			TestResult	    			Terminate	();
		}

		////////////////////////////////////////////////////////////////////////
		// ITestItems
		//
		////////////////////////////////////////////////////////////////////////
        [ComImport, Guid("F2D83417-A9FF-4503-B11F-8776FF812AEA"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurityAttribute()]
        public interface ITestItems
		{
			// Enumeration
			int							Count		{ get;	}
			[return: MarshalAs(UnmanagedType.Interface)]
            ITestItem      			    GetItem     ( int index);
		}

        ////////////////////////////////////////////////////////////////////////
		// ITestProperty
		//
		////////////////////////////////////////////////////////////////////////
        [ComImport, Guid("2FD231B5-0333-44f1-909E-483CDE87B196"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurityAttribute()]
        public interface ITestProperty
		{
			// Simple Meta-data about the property
//			int							Id  		{ get;	}
//			String						Guid  		{ get;	}
			String						Name		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Desc		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
//			string						Owner		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
//			int							Priority	{ get;	}
			TestPropertyFlags   		Flags		{ get; set;	} 
			Object 				        Value       { get; }
            void                        set_Value(ref object value);

			// Extensible Meta-data about the property
			ITestProperties				Metadata	{ [return: MarshalAs(UnmanagedType.Interface)] get;	}

			// Children - Heiarchical properties
			ITestProperties				Children	{ [return: MarshalAs(UnmanagedType.Interface)] get;	}
		}

		////////////////////////////////////////////////////////////////////////
		// ITestProperties
		//
		////////////////////////////////////////////////////////////////////////
        [ComImport, Guid("EECA332B-072A-4018-A7B5-DF83AD9BAB28"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurityAttribute()]
        public interface ITestProperties
		{
			// Enumeration
			int							Count		{ get;	}
			[return: MarshalAs(UnmanagedType.Interface)]
            ITestProperty  			    GetItem     ( int index);

			//Access methods
			[return: MarshalAs(UnmanagedType.Interface )]	
			ITestProperty				Get		    ([MarshalAs(UnmanagedType.BStr)] string name);
			[return: MarshalAs(UnmanagedType.Interface )]	
			ITestProperty				Add	    	([MarshalAs(UnmanagedType.BStr)] string name);
			void                        Remove  	([MarshalAs(UnmanagedType.BStr)] string name);
			void                        Clear  	    ();
		}

		////////////////////////////////////////////////////////////////////////
		// ITestLoader
		//
		////////////////////////////////////////////////////////////////////////
        [ComImport, Guid("54FE009F-5D58-4f90-B238-8961350FD632"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurityAttribute()]
        public interface ITestLoader
		{
            //Simple Metadata
//			int							Id  		{ get;	}
			String						Guid  		{ get;	}
			String						Name		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Desc		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}

			// Extensible Meta-data about the item
			ITestProperties				Metadata	{ [return: MarshalAs(UnmanagedType.Interface)] get;	}

            //Execution 
			void          				Init		();
			[return: MarshalAs(UnmanagedType.Interface )]	
			ITestItem					CreateTest 	( [MarshalAs(UnmanagedType.BStr)] string assembly, [MarshalAs(UnmanagedType.BStr)] string test);
			void                        Terminate	();

			//Enumeration
            [return: MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VarEnum.VT_BSTR)] 
            String[]                    Enumerate	( [MarshalAs(UnmanagedType.BStr)] string assembly);

            //Input (get/set)
			ITestProperties				Properties	{ [param: MarshalAs(UnmanagedType.Interface)] set; [return: MarshalAs(UnmanagedType.Interface)] get;	}

			//Logging (get/set)
			ITestLog				    Log		    { [param: MarshalAs(UnmanagedType.Interface)] set; [return: MarshalAs(UnmanagedType.Interface)] get;	 }
		}

        ////////////////////////////////////////////////////////////////////////
		// ITestLog
		//
		////////////////////////////////////////////////////////////////////////
        [ComImport, Guid("11B3FA9D-B07A-44ad-9C9B-81558AA663B8"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurityAttribute()]
        public interface ITestLog 
		{
            //Simple Metadata
//			int							Id  		{ get;	}
//			String						Guid  		{ get;	}
			String						Name		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}
			String						Desc		{ [return: MarshalAs(UnmanagedType.BStr)] get;	}

			// Extensible Meta-data about the item
			ITestProperties				Metadata	{ [return: MarshalAs(UnmanagedType.Interface)] get;	}

			//Construction
            void                        Init		();
			void                        Terminate	();

            //Console
            void		                Write(      TestLogFlags flags, 
							                        [MarshalAs(UnmanagedType.BStr)] string text);
			void		                WriteLine(  TestLogFlags flags, 
							                        [MarshalAs(UnmanagedType.BStr)] string text);

            //Scoping
	        void                        Enter(      [MarshalAs(UnmanagedType.Interface)] ITestItem item, TestMethod method );
	        void		                Leave(      [MarshalAs(UnmanagedType.Interface)] ITestItem item, TestMethod method, TestResult result );
            
            //(Error) Logging routines
			void		                Error     ( TestResult result,
                                                    TestLogFlags flags,
                                                    [MarshalAs(UnmanagedType.BStr)] string actual, 
							                        [MarshalAs(UnmanagedType.BStr)] string expected, 
							                        [MarshalAs(UnmanagedType.BStr)] string source,
							                        [MarshalAs(UnmanagedType.BStr)] string message,
							                        [MarshalAs(UnmanagedType.BStr)] string stack,
							                        [MarshalAs(UnmanagedType.BStr)] string filename, 
							                        int lineno);
        }
//	}
}
