//---------------------------------------------------------------------
// <copyright file="TestLoader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;   //ArrayList
using System.Reflection;    //Assembly

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// TestLoader
	//
	////////////////////////////////////////////////////////////////
	public class TestLoader : ITestLoader
	{
		//Data
        protected   String              _guid       = null;
        protected   String              _name       = null;
        protected   String              _desc       = null;
        protected   TestProperties      _metadata   = null;

        //Constructor
        public TestLoader()
        {
            _name = this.GetType().FullName;
			_guid = _name;
        }

		// Input (get/set)
		public ITestProperties			Properties	
		{	
			get { return TestInput.Properties.Internal;	            }
			set { TestInput.Properties = new TestProps(value);      }
		}

		// Logging (get/set)
		public ITestLog                 Log
		{	
			get { return TestLog.Internal;      }
			set { TestLog.Internal = value;		}		
        }

        //Accessors
		public string				    Guid
        { 
            get { return _guid;     }
            set { _guid = value;    }
        }

        public string				    Name		
        { 
            get { return _name;     }
            set { _name = value;    }
        }
		
        public string				    Desc
        { 
            get { return _desc;     }
            set { _desc = value;    }
        }

        public ITestProperties		    Metadata
        { 
            get { return _metadata; }
        }

        //Enumeration
		public String[]      			Enumerate(String assemblyname)
		{
            ArrayList array = new ArrayList();
            Assembly assembly = Assembly.LoadFrom(assemblyname);
            foreach(Type type in assembly.GetTypes())
            {
			    //Were only looking for classes that inherit from TestModel
			    if(!type.IsSubclassOf(typeof(TestModule)))
				    continue;

			    //Those classes must have the [TestModule] attribute to be considered
			    if(type.GetCustomAttributes(typeof(TestModuleAttribute), false/*inhert*/).Length == 0) 
			        continue;

                //Found valid TestModule class
                array.Add(type.FullName);
            }

            return (String[])array.ToArray(typeof(String));
		}

        // Initialization
		public void                       Init()
		{
			try
			{
                ;
            }
			catch(Exception e)
			{
				TestLog.HandleException(e);
			}
        }
		
		public void                      Terminate()
		{
			try
			{
				//Clear any previous existing info (in the static class).

                //Note: We actually have to clear these "statics" since they are not cleaned up
				//until the process exits.  Which means that if you run again, in an apartment model
				//thread it will try to release these when setting them which is not allowed to 
				//call a method on an object created in another apartment
				TestInput.Dispose();
				TestLog.Dispose();
			}
			catch(Exception)
			{
//				TestLog.HandleException(e);
			}
        }
		
		public ITestItem   		        CreateTest(String assembly, String type)
		{
            try
            {
//              this.Log        = new LtmContext() as ITestLog;
//              this.Properties = new LtmContext() as ITestProperties;

                //Create an instance
                //Note: Assembly.CreateInstance returns null for type not found
                TestModule test = (TestModule)Assembly.LoadFrom(assembly).CreateInstance(type);
                if(test == null)
                    throw new TypeLoadException(
                                String.Format("Unable to find type: '{0}' in assembly: '{1}'", type, assembly)
                                );
                return test;
            }
			catch(Exception e)
			{
				TestLog.HandleException(e);
                throw new TestFailedException(e.Message, null, null, e);
			}
		}
	}
}
