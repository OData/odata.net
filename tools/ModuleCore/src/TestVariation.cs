//---------------------------------------------------------------------
// <copyright file="TestVariation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Reflection;      //BindingFlags

namespace Microsoft.Test.ModuleCore
{
	////////////////////////////////////////////////////////////////
	// Delegate
	//
	////////////////////////////////////////////////////////////////
	public delegate	void	TestFunc();

    ////////////////////////////////////////////////////////////////
	// TestVariation
	//
	////////////////////////////////////////////////////////////////
	public class TestVariation : TestItem
	{ 
		//Data
        protected TestFunc    _func = null;

		//Constructor
		public	TestVariation()
			: this(null, null)
		{
		}

		public	TestVariation(TestFunc func)
			: this(func.Method.Name, func)
		{
		}

		public	TestVariation(string desc, TestFunc func)
			: base(null, desc, TestType.TestVariation)
		{
            _func       = func;
		}

        // MemberwiseClone Shallow Copy
        public  TestVariation Clone()
        {
            return (TestVariation)MemberwiseClone();
        }

        protected override void	            DetermineChildren()
		{
            //No children
        }
        
        protected override TestAttribute	CreateAttribute()
		{ 
			return new VariationAttribute();
		}

        public override void                Execute()
		{ 
			if(Parent != null)
				Parent.CurrentChild = this;
			
            //Delegate function
            //Note: Override, if you have some other approach to executing your code (reflection, xml file, etc)
            _func();
		}

        public override int                 CompareTo(object o)
        {
            //Default comparison, Id based.
            return this.Id.CompareTo(((TestItem)o).Id);
        }
	}
}
