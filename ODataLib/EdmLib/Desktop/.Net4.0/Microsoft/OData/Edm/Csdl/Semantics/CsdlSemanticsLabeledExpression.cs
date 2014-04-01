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
    internal class CsdlSemanticsLabeledExpression : CsdlSemanticsElement, IEdmLabeledExpression
    {
        private readonly string name;
        private readonly CsdlExpressionBase sourceElement;
        private readonly CsdlSemanticsSchema schema;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsLabeledExpression, IEdmExpression> expressionCache = new Cache<CsdlSemanticsLabeledExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsLabeledExpression, IEdmExpression> ComputeExpressionFunc = (me) => me.ComputeExpression();

        public CsdlSemanticsLabeledExpression(string name, CsdlExpressionBase element, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(element)
        {
            this.name = name;
            this.sourceElement = element;
            this.bindingContext = bindingContext;
            this.schema = schema;
        }

        public override CsdlElement Element
        {
            get { return this.sourceElement; }
        }

        public override CsdlSemanticsModel Model
        {
            get { return this.schema.Model; }
        }

        public IEdmEntityType BindingContext
        {
            get { return this.bindingContext; }
        }

        public IEdmExpression Expression
        {
            get { return this.expressionCache.GetValue(this, ComputeExpressionFunc, null); }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Labeled; }
        }

        public string Name
        {
            get { return this.name; }
        }

        private IEdmExpression ComputeExpression()
        {
            return CsdlSemanticsModel.WrapExpression(this.sourceElement, this.BindingContext, this.schema);
        }
    }
}
