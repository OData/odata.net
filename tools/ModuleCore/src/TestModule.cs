//---------------------------------------------------------------------
// <copyright file="TestModule.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;        //Assembly
using System.IO;				//Directory
using System.Collections;		//ArrayList
using System.Security;          //AllowPartiallyTrustedCallersAttribute

[assembly: AllowPartiallyTrustedCallersAttribute]
[assembly: SecurityRules(SecurityRuleSet.Level1, SkipVerificationInFullTrust = true)]
namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// TestModule
	//
	////////////////////////////////////////////////////////////////
	public abstract class TestModule : TestItem
	{ 
        //Default Loader
        public class ModuleLoader : TestLoader
        {
        }

        //Data
		protected TestSpec		_testspec;
		protected ArrayList		_includes   = new ArrayList();

        //Constructors
		public TestModule()
			: this(null, null)
		{ 
		}

		public TestModule(string name, string desc)
			: base(name, desc, TestType.TestModule)
		{ 
			this.Guid	= GetType().ToString();
			
			//Population
		    DetermineChildren();
            DetermineIncludes();
		    DetermineFilters();
		}

		//Accessors
		public new virtual TestModuleAttribute	Attribute
		{
			get { return (TestModuleAttribute)base.Attribute;}
			set { base.Attribute = value;			}
		}

		public string			                Created
		{ 
			get { return Attribute.Created;			}
			set { Attribute.Created = value;		}
		}

		public string			                Modified
		{ 
			get { return Attribute.Modified;		}
			set { Attribute.Modified = value;		}
		}

		public TestSpec		                    TestSpec
		{ 
			get 
			{ 
				//Deferred Creation
				if(_testspec == null)
					_testspec = new TestSpec(this);
				return _testspec;
			}
		}

		public override  void	                Init()
		{
			//Delegate
			base.Init();
			
			//Includes (aggregate tests)
			foreach(TestModule include in _includes)
				include.Init();
		}

        public override  void	                Terminate()
		{
			//Includes (aggregate tests)
			foreach(TestModule include in _includes)
				include.Terminate();

			//Delegate
			base.Terminate();
		}

		//Helpers
		protected override TestAttribute	    CreateAttribute()
		{
			//Obtain attributes
			//Note: All the other levels, fill in the attribute (when enumerating), however
			//the module doesn't have anyone who fills it in, so just obtain it directly
			object[] attributes = GetType().GetCustomAttributes(typeof(TestModuleAttribute), false/*inhert*/);
			if(attributes != null && attributes.Length>0) 
				return (TestModuleAttribute)attributes[0];

			//Otherwise
			return new TestModuleAttribute();
		}

        protected override void	                DetermineChildren()
		{
            base.DetermineChildren();
		}

        protected virtual void		            DetermineIncludes()
		{
			try
			{
				//[TestInclude] attributes
				foreach(TestIncludeAttribute attr in this.GetType().GetCustomAttributes(typeof(TestIncludeAttribute), false/*inhert*/)) 
				{
					//Files (either specified or search pattern)
					string[] filenames = new string[]{attr.File};
					if(attr.Files != null)
						filenames = Directory.GetFiles(".", attr.Files);

					//Loop through the files
					foreach(string filename in filenames)
					{
						try
						{
							//Note: Prevent recursion - (ie: ignore this assembly)
							Assembly assembly = Assembly.LoadFrom(filename);
							if(assembly == Assembly.GetAssembly(this.GetType()))
								continue;

							//Find the TestModule(s) (from the specified assembly)
							foreach(Type type in assembly.GetTypes())
							{
								//Were only interested in objects that inherit from TestModule
								if(type.IsSubclassOf(typeof(TestModule)) )
								{
									//Create this module (call the constructor with no arguments)
									TestModule testmodule = (TestModule)Activator.CreateInstance(type);

									//Filter the module (as specified)
									if(attr.Filter != null)
										testmodule.TestSpec.ApplyFilter(testmodule, attr.Filter);
								
									//Add the children
									if(testmodule.Children.Count > 0)
									{
										//Determine what prefix to use (to distinguish testcases)
										string prefix = testmodule.Name;
										if(attr.Name != null)
											prefix = attr.Name;

										//Add all the testcases
										//TODO: What about ModuleInit/Terminate for this group?
										_includes.Add(testmodule);
										foreach(TestItem testcase in testmodule.Children)
										{
											if(prefix != null && prefix.Length > 0)
												testcase.Name = (prefix + "." + testcase.Name);
											Children.Add(testcase);
										}
									}
								}
							}
						}
						catch(Exception e)
						{
							//We might not be able to load all assemblies in the directory (ie: *.dll)
							//so simply move onto the next assembly...
							TestLog.WriteLineIgnore(e.ToString());
						}
					}
				}
			}
			catch(Exception e)
			{
				//Make this easier to debug
				TestLog.WriteLineIgnore(e.ToString());
				throw e;
			}
		}

		protected virtual void		            DetermineFilters()
		{
			//Override if you don't want filtering, or want to provider your own mechanism
			try
			{				
				//	/Filter
				if(TestInput.Filter != null)
					this.TestSpec.ApplyFilter(this, this.FilterScope(TestInput.Filter));

				// /MaxPriority
                string maxprifilter = "//TestVariation[@Priority<='{0}']";
				if(TestInput.MaxPriority != null) 
					this.TestSpec.ApplyFilter(this, String.Format(maxprifilter, TestInput.MaxPriority));

				// Module [TestFilter(...)]
				if(this.Filter != null)
					this.TestSpec.ApplyFilter(this, this.FilterScope(this.Filter));

				// TestCase [TestFilter(...)]
				foreach(TestItem testcase in this.Children)
				{
					if(testcase.Filter != null) 
						this.TestSpec.ApplyFilter(testcase, this.FilterScope(testcase.Filter));
				}
			}
			catch(Exception e)
			{
				//Make this easier to debug
				TestLog.WriteLineIgnore(e.ToString());
				throw e;
			}
		}

		protected virtual string	            FilterScope(string xpath)
		{
			//Basically we want to allow either simply filtering at the variation node (ie: no scope),
			//in which case we'll just add the 'assumed' scope, or allow filtering at any level.  
			//We also want to be consitent with the XmlDriver in which all filters are predicates only.
			string varfilter	= "//TestVariation[{0}]";
			if(xpath != null)
			{
				xpath = xpath.Trim();
				if(xpath.Length > 0)
				{
					//Add the Variation Scope, if no scope was specified
					if(xpath[0] != '/')
						xpath = String.Format(varfilter, xpath);
				}
			}

			return xpath;
		}
	}
}
