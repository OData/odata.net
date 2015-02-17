//---------------------------------------------------------------------
// <copyright file="TestException.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// TestSkippedException
	//
	// Since this is a very common exception and makes it so you
	// can catch only the skipped exceptions this is quite useful
	//
	////////////////////////////////////////////////////////////////
	public class TestSkippedException : TestException
	{
		//Constructor
		public	TestSkippedException(string message)
			: this(message, false, true, null)
		{
		}

		public	TestSkippedException(string message, object actual, object expected, Exception inner)
			: base(TestResult.Skipped, message, actual, expected, inner)
		{
		}
	}

	////////////////////////////////////////////////////////////////
	// TestFailedException
	//
	////////////////////////////////////////////////////////////////
	public class TestFailedException : TestException
	{
		//Constructor
		public	TestFailedException(string message)
			: this(message, false, true, null)
		{
		}

		public	TestFailedException(string message, object actual, object expected, Exception inner)
			: base(TestResult.Failed, message, actual, expected, inner)
		{
		}
	}

	////////////////////////////////////////////////////////////////
	// TestWarningException
	//
	////////////////////////////////////////////////////////////////
	public class TestWarningException : TestException
	{
		//Constructor
		public	TestWarningException(string message)
			: this(message, false, true, null)
		{
		}

		public	TestWarningException(string message, object actual, object expected, Exception inner)
			: base(TestResult.Warning, message, actual, expected, inner)
		{
		}
	}

    ////////////////////////////////////////////////////////////////
	// TestException
	//
	// For all other cases, you can just setup the status directly
	////////////////////////////////////////////////////////////////
	public class TestException : SystemException
	{ 
		//Data
		public	TestResult  Result;
		public  object      Actual;
		public	object      Expected;

		//Constructor
		public	TestException(TestResult result, string message)
			: this(result, message, false, true, null)
		{
		}

		public	TestException(TestResult result, string message, object actual, object expected, Exception inner)
			: base(message, inner)
		{
			//Note: iResult is the variation result (ie: TEST_PASS, TEST_FAIL, etc...)
			//Setup the exception
			Result		= result;
			Actual		= actual;
			Expected	= expected;
		}
	}
}
