//---------------------------------------------------------------------
// <copyright file="TestCase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;    //MethodInfo

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// TestCases
	//
	////////////////////////////////////////////////////////////////
	public abstract class TestCase : TestItem
	{ 
		//Data

		//Constructor
		public	TestCase()
			: this(null, null)
		{
		}

		public	TestCase(string name, string desc)
			: base(name, desc, TestType.TestCase)
		{
		}

		//Accessors
		protected override TestAttribute	    CreateAttribute()
		{ 
			return new TestCaseAttribute();
		}

		protected virtual TestVariation	        CreateVariation(TestFunc func)
		{ 
            //Override if you have a specific variation class
            return new TestVariation(func);
		}

        public new virtual TestCaseAttribute	Attribute
		{
			get { return (TestCaseAttribute)base.Attribute;	}
			set { base.Attribute = value;			}
		}

		//Helpers
		protected override void		            DetermineChildren()
		{
            //Delegate (add any nested testcases)
            base.DetermineChildren();

			//Dynamically figure out what functions are variations...
			foreach(MethodInfo method in this.GetType().GetMethods())
			{
				//Loop through the Attributes for method
				foreach(VariationAttribute attr in method.GetCustomAttributes(typeof(VariationAttribute), false/*inhert*/)) 
				{
					//Every method that has a [Variation attribute] = a variation
					//Add this variation to our array
                    TestFunc func = null;
					try
                    {
                        func = (TestFunc)Delegate.CreateDelegate(typeof(TestFunc), this, method);
                    }
                    catch(Exception e)
                    {
                        e = new TestFailedException("Variation: '" + method + "' doesn't match expected signature of 'void func()', unable to add that variation.", null, null, e);
                        HandleException(e);
                        continue;
                    }

                    TestVariation var   = this.CreateVariation(func);
					attr.Name = func.Method.Name;	//Name is always the function name (invoke)
					if(attr.Desc == null)
						attr.Desc = attr.Name;
					var.Attribute = attr;
                    AddChild(var);
				}
			}	

            //Sort		
            //Default sort is based upon IComparable of each item	
		    Children.Sort();
		}
		
		public TestVariation	                    Variation
		{
			//Currently executing child
			get	{	return base.CurrentChild as TestVariation;				}
		}
	}
}
