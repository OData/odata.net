//---------------------------------------------------------------------
// <copyright file="TestLog.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Diagnostics;				//TraceListener
using System.Reflection;				//TargetInvocationException
using System.IO;                        //TextWriter
using System.Text;                      //Encoding

namespace Microsoft.Test.ModuleCore
{
    ////////////////////////////////////////////////////////////////
    // TraceLevel
    //
    ////////////////////////////////////////////////////////////////
    public enum TraceLevel
    {
        None,
        Info,
        Default,
        Debug,
        All,
    };

	////////////////////////////////////////////////////////////////
	// TestLog
	//
	////////////////////////////////////////////////////////////////
	public class TestLog
	{ 
		//Data
		static ITestLog					_internal;
	    static TraceLevel	            _level          = TraceLevel.Default;
        static TestLogAssertHandler     _asserthandler  = null;

		//Accessors
		static public	ITestLog		Internal
		{
			set 
			{ 
			    _internal = value;
			    DisableAsserts();
			}
			get	
            { 
//				if(_internal == null)
//                  _internal = (new LtmContext() as ITestLog);
                return _internal;				
            }
		}

        static public	TraceLevel      Level
	    {
    		get { return _level;                }
    		set { _level = value;               }
	    }

		static public	TestLogAssertHandler	AssertHandler
		{
		    get { return _asserthandler;        }
        }

		static internal	void			Dispose()
		{
		    //Reset the info.  
		    _internal			= null;
            if(_asserthandler != null)
                Debug.Listeners.Remove(_asserthandler);
		    _asserthandler      = null;
		}

		//Helpers
		static public	string			NewLine
		{
			get { return "\n";	}
		}

        static public bool WillTrace(TraceLevel level)
        {
	        return(_level >= level);
        }

		static public	void			Write(object value)
		{
             Write(TestLogFlags.Text, StringEx.ToString(value));
		}

		static public	void			WriteLine(object value)
		{
			WriteLine(TestLogFlags.Text, StringEx.ToString(value));
		}

		static public	void			WriteLine()
		{
			WriteLine(TestLogFlags.Text, null);
		}

        static public	void			Write(string text)
		{
			Write(TestLogFlags.Text, text);
		}
		
		static public	void			Write(string text, params object[] args)
		{
			//Delegate
			Write(TestLogFlags.Text, String.Format(text, args));
		}

		static public	void			WriteLine(string text)
		{
			WriteLine(TestLogFlags.Text, text);
		}

		static public	void			WriteLine(string text, params object[] args)
		{
			//Delegate
			WriteLine(String.Format(text, args));
		}

		static public	void			Write(char[] value)
		{
			WriteLine(TestLogFlags.Text, new string(value));
		}
		
		static public	void			WriteLine(char[] value)
		{
			WriteLine(TestLogFlags.Text, new string(value));
		}

		static public	void			WriteXml(string text)
		{
			Write(TestLogFlags.Xml, text);
		}
		
		static public	void			WriteRaw(string text)
		{
			Write(TestLogFlags.Raw, text);
		}

		static public	void			WriteIgnore(string text)
		{
			Write(TestLogFlags.Ignore, text);
		}

		static public	void			WriteLineIgnore(string text)
		{
			WriteLine(TestLogFlags.Ignore, text);
		}

		static public	void			Write(TestLogFlags flags, string text)
		{	
            if(Internal != null)
				Internal.Write(flags, text);
    		else
	    		Console.Write(text);
		}

		static public	void			WriteLine(TestLogFlags flags, string text)
		{
			if(Internal != null)
				Internal.WriteLine(flags, text);
			else
				Console.WriteLine(text);
		}

        static public	void			Trace(String value)
	    {	
	        Trace(TraceLevel.Default, value);
	    }

        static public	void			TraceLine(String value)
	    {
            TraceLine(TraceLevel.Default, value);
	    }

        static public void              TraceLine()
        {
            TraceLine(TraceLevel.Default, null);
        }

        static public void              Trace(TraceLevel level, String value)
	    {	
	        if(WillTrace(level))
	            Write(TestLogFlags.Trace | TestLogFlags.Ignore, value);
	    }

	    static public	void			TraceLine(TraceLevel level, String value)
	    {
            if (WillTrace(level))
                Write(TestLogFlags.Trace | TestLogFlags.Ignore, value + TestLog.NewLine);
	    }

        static public void              TraceLine(TraceLevel level)
        {
            TraceLine(level, null);
        }

		static public	bool			Compare(bool equal, string message)
		{
			if(equal)
				return true;
			return Compare(false, true, message);
		}

		static public	bool			Compare(object actual, object expected, string message)
		{
			if(InternalEquals(actual, expected))
				return true;
			
			//Compare not only compares but throws - so your test stops processing
			//This way processing stops upon the first error, so you don't have to check return
			//values or validate values afterwards.  If you have other items to do, then use the 
			//TestLog.Equals instead of TestLog.Compare
            throw new TestFailedException(message, actual, expected, null);
            
		}

		static public	bool			Equals(object actual, object expected, string message)
		{
            //Equals is identical to Compare, except that Equals doesn't throw.
            //i.e. the test wants to record the failure and continue to do other things
            if (InternalEquals(actual, expected))
                return true;

            StackTrace stackTrace = new StackTrace(true);
            TestLog.Error(TestResult.Failed, actual, expected, null, message, stackTrace.ToString(), null, 0);

            return false;
		}

		static public	bool			Warning(bool equal, string message)
		{
			return Warning(equal, true, message, null);
		}
		
		static public	bool			Warning(object actual, object expected, string message)
		{
			return Warning(actual, expected, message, null);
		}

		static public	bool			Warning(object actual, object expected, string message, Exception inner)
		{
			//See if these are equal
			bool equal = InternalEquals(actual, expected);
			if(equal)
				return true;

			try
			{
				throw new TestWarningException(message, actual, expected, inner);
			}
			catch(Exception e)
			{
				//Warning should continue - not halt test progress
				TestLog.HandleException(e);
 				return false;
			}
		}
		
		static public	bool			Skip(string message)
		{
			//Delegate
			return Skip(true, message);
		}

		static public	bool			Skip(bool skip, string message)
		{
			if(skip)
				throw new TestSkippedException(message);
			return false;
		}

		static internal bool			InternalEquals(object actual, object expected)
		{
			//Handle null comparison
			if(actual == null && expected == null)
				return true;
			else if(actual == null || expected == null)
				return false;
			else if(Convert.IsDBNull(expected))
				return Convert.IsDBNull(actual);
			
			//Otherwise
			return expected.Equals(actual);
		}

		static public	void			Error(TestResult result, object actual, object expected, string source, string message, string stack, String filename, int lineno)
		{
			//Log the error
			if(Internal != null)
			{
				//ITestConsole.Log
				//TODO: We could pass in ints - if the objects are convertable to int, ie: they may
				//actually be comparing two values, not complex objects.
				Internal.Error(	result,
                            TestLogFlags.Text,          	//flags        
                            StringEx.Format(actual),		//actual
							StringEx.Format(expected),		//expected
							source,							//source
							message,						//message
							stack,	    					//stack
							filename,		                //filename
							lineno	                        //line
						);
			}
			else
			{
				//We call IError::Compare, which logs the error AND increments the error count...
				Console.WriteLine("Message:\t"	+ message);
				Console.WriteLine("Source:\t\t"	+ source);
				Console.WriteLine("Expected:\t"	+ expected);
				Console.WriteLine("Received:\t"	+ actual);
				Console.WriteLine("Details:"    + TestLog.NewLine	+ stack);
				Console.WriteLine("File:\t\t"	+ filename);
				Console.WriteLine("Line:\t\t"	+ lineno);
			}
		}

		public static TestResult	    HandleException(Exception e)
		{
			TestResult result = TestResult.Failed;
            Exception inner = e;

            //If reflection was used check for test skipped/passed exception
            while(inner is TargetInvocationException)
            {
                inner = inner.InnerException;
            }

            if (!(inner is TestException) || 
                       ((inner as TestException).Result != TestResult.Skipped &&  
                        (inner as TestException).Result != TestResult.Passed && 
                        (inner as TestException).Result != TestResult.Warning))
                inner = e; //start over so we do not loose the stack trace

		    while(inner != null)
		    {
                //Need to handle Loader exceptions seperatly
                if(inner is ReflectionTypeLoadException)
                {
                    //Recurse
                    foreach(Exception loaderexception in ((ReflectionTypeLoadException)inner).LoaderExceptions)
                        HandleException(loaderexception);
                }

                //Source
			    String source		= inner.Source;
    			    
			    //Message
			    String	message 	= inner.Message;
			    if(inner != e)
			        message = "Inner Exception -> " + message;
    		
			    //Expected / Actual
			    Object actual		= inner.GetType();
			    Object expected		= null;
                String details      = inner.StackTrace;
                String filename     = null;
                int line            = 0;
			    if(inner is TestException) 
			    {
				    TestException testinner = (TestException)inner;
    				
				    //Setup more meaningful info
				    actual  	= testinner.Actual;
				    expected 	= testinner.Expected;
				    result 		= testinner.Result;
				    switch(result)
				    {
					    case TestResult.Passed:
					    case TestResult.Skipped:
						    WriteLine(message);
						    return result;	//were done
				    };
			    }

			    //Log
			    TestLog.Error(result, actual, expected, source, message, details, filename, line);

			    //Next
			    inner = inner.InnerException;
            }

            return result;
		}

        private static void             DisableAsserts()
        {
            try
            {
                //Turn on assert dialog boxes.
                if(_asserthandler == null)
                {
                    //Disable the assert pop-up dialogs
                    foreach(TraceListener listener in Debug.Listeners)
                    {
                        if(listener is DefaultTraceListener)
                        {
                            //Disable assert dialogs popups
                            ((DefaultTraceListener)listener).AssertUiEnabled = false;
                        }
                    }

                    //Add our own handler
                    //  Note: We can't simply set DefaultTeaceListener.AssertUiEnabled = false, since that 
                    //  actually disables the assertions, and we no longer see them.  We need to count them 
                    //  as errors and continue in Ltm
                    _asserthandler = new TestLogAssertHandler();
                    Debug.Listeners.Add(_asserthandler);
                }
            }
            catch (Exception e)
            {
                //If disabling the asserts fails, simply log it and move on.
                //No reason to hold up the test or block error logging for that, and don't count it as an error
                TestLog.WriteLine("Unable to disable asserts:" + TestLog.NewLine + e);
            }
        }
    }

    ////////////////////////////////////////////////////////////////
    // TestLogWriter
    //
    ////////////////////////////////////////////////////////////////
    public class TestLogWriter : TextWriter
    {
        //Data
        protected TestLogFlags _flags = TestLogFlags.Text;

        //Constructor
        public TestLogWriter(TestLogFlags flags)
        {
            _flags = flags;
        }

        //Overrides    
        public override void            Write(char ch)
        {
            //A subclass must minimally implement the Write(Char) method. 
            Write(ch.ToString());
        }

        public override void            Write(string text)
        {
            //Delegate
            TestLog.Write(_flags, text);
        }

        public override void            Write(char[] ch)
        {
            //Note: This is a workaround the TextWriter::Write(char[]) that incorrectly 
            //writes 1 char at a time, which means \r\n is written sperately and then gets fixed
            //up to be two carriage returns!
            if (ch != null)
            {
                StringBuilder builder = new StringBuilder(ch.Length);
                builder.Append(ch);
                Write(builder.ToString());
            }
        }

        public override void            WriteLine(string strText)
        {
            Write(strText + this.NewLine);
        }

        public override void            WriteLine()
        {
            //Writes a line terminator to the text stream. 
            //The default line terminator is a carriage return followed by a line feed ("\r\n"), 
            //but this value can be changed using the NewLine property.
            Write(this.NewLine);
        }

        public override Encoding        Encoding
        {
            get { return Encoding.Unicode; }
        }
    }

	////////////////////////////////////////////////////////////////
	// TestLogAssertHandler
	//
	////////////////////////////////////////////////////////////////
    public class TestLogAssertHandler : DefaultTraceListener
	{ 
		//Data
        protected bool  _shouldthrow  = false;
		
		//Constructor
        public TestLogAssertHandler()
		{
		}

        //Accessors
        public virtual bool         ShouldThrow     
        {
            get { return _shouldthrow;  }
            set { _shouldthrow = value; }
        }

        //Overloads
        public override void        Fail(string message, string details)
		{
            //Handle the assert, treat it as an error.
            Exception e = new TestException(TestResult.Assert, message, details, null, null);
            e.Source = "Debug.Assert";

            //Note: We don't throw the exception (by default), since we want to continue on Asserts
            //(as many of them are benign or simply a valid runtime error).
            if(this.ShouldThrow)
                throw e;
            
            StackTrace stack = new StackTrace();
            TestLog.Error(TestResult.Assert, details, null, "Debug.Assert", message, stack.ToString(), null, 0);
        }

		public	override void		Write(string strText)
		{
		    //TODO: Product tracing
		}

		public	override void		WriteLine(string strText)
		{
            //TODO: Product tracing
        }
	}
}
