//---------------------------------------------------------------------
// <copyright file="fxBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// fxBase
	//
	////////////////////////////////////////////////////////   
    public class fxBase
    {
        //Data
        protected String    _name;
        
        //Constructor
        public fxBase(String name)
        {
            _name = name;
        }

        //Accessors
        public  virtual String      Name
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _name;                 }

            [System.Diagnostics.DebuggerStepThrough]
            set { _name = value;                }
        }
    }        
}
