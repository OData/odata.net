//---------------------------------------------------------------------
// <copyright file="ModelExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Text;
using System.Reflection;

namespace Microsoft.Test.KoKoMo
{
    public delegate object ModelFunction();
    ////////////////////////////////////////////////////////////////
    // ModelExpresion
    //
    ////////////////////////////////////////////////////////////////
    public class ModelExpression : ModelRequirement
    {
        //Data
        ModelFunction _func = null;

        //Constructor
        public ModelExpression(ModelFunction func, ModelValue value)
            : base(null, value)
        {
            _func = func;
        }

        //Overrides
        public override bool Evaluate(Object expected)
        {
            return base.Evaluate(_func());
        }
    }
}
