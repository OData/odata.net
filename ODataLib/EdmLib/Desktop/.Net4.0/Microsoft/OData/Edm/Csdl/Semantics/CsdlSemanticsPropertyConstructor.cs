//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a CsdlPropertyValue used in a record expression.
    /// </summary>
    internal class CsdlSemanticsPropertyConstructor : CsdlSemanticsElement, IEdmPropertyConstructor
    {
        private readonly CsdlPropertyValue property;
        private readonly CsdlSemanticsRecordExpression context;

        private readonly Cache<CsdlSemanticsPropertyConstructor, IEdmExpression> valueCache = new Cache<CsdlSemanticsPropertyConstructor, IEdmExpression>();
        private static readonly Func<CsdlSemanticsPropertyConstructor, IEdmExpression> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsPropertyConstructor(CsdlPropertyValue property, CsdlSemanticsRecordExpression context)
            : base(property)
        {
            this.property = property;
            this.context = context;
        }

        public string Name
        {
            get { return this.property.Property; }
        }

        public IEdmExpression Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override CsdlElement Element
        {
            get { return this.property; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.context.Model; }
        }

        private IEdmExpression ComputeValue()
        {
            return CsdlSemanticsModel.WrapExpression(this.property.Expression, this.context.BindingContext, this.context.Schema);
        }
    }
}
