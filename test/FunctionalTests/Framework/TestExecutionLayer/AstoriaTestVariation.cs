//---------------------------------------------------------------------
// <copyright file="AstoriaTestVariation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// AstoriaTestVariation
	//
	////////////////////////////////////////////////////////   
    public class AstoriaTestVariation : TestVariation
    {
		//Constructor
        public	AstoriaTestVariation(TestFunc method)
			: base(method)
		{
		}

        //Overrides
        public override void                Init()
        {
            //Delegate
            base.Init();
        }

        public override void                Terminate()
        {
            //Delegate
            base.Terminate();
        }

        public override void                Execute()
        {
            //Delegate
            base.Execute();
        }

        protected override void             OnEnter(TestMethod method)
        {
        }

        protected override void             OnLeave(TestMethod method)
        {
        }

        protected override TestResult       HandleException(Exception e)
        {
            return AstoriaTestLog.HandleException(e);
        }
    }
}
