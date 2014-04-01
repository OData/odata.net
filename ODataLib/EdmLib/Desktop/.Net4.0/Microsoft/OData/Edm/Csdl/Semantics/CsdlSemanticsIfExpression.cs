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
    internal class CsdlSemanticsIfExpression : CsdlSemanticsExpression, IEdmIfExpression
    {
        private readonly CsdlIfExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> testCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeTestFunc = (me) => me.ComputeTest();

        private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> ifTrueCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeIfTrueFunc = (me) => me.ComputeIfTrue();

        private readonly Cache<CsdlSemanticsIfExpression, IEdmExpression> ifFalseCache = new Cache<CsdlSemanticsIfExpression, IEdmExpression>();
        private static readonly Func<CsdlSemanticsIfExpression, IEdmExpression> ComputeIfFalseFunc = (me) => me.ComputeIfFalse();

        public CsdlSemanticsIfExpression(CsdlIfExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public IEdmEntityType BindingContext
        {
            get { return this.bindingContext; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.If; }
        }

        public IEdmExpression TestExpression
        {
            get { return this.testCache.GetValue(this, ComputeTestFunc, null); }
        }

        public IEdmExpression TrueExpression
        {
            get { return this.ifTrueCache.GetValue(this, ComputeIfTrueFunc, null); }
        }

        public IEdmExpression FalseExpression
        {
            get { return this.ifFalseCache.GetValue(this, ComputeIfFalseFunc, null); }
        }

        private IEdmExpression ComputeTest()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.Test, this.BindingContext, this.Schema);
        }

        private IEdmExpression ComputeIfTrue()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.IfTrue, this.BindingContext, this.Schema);
        }

        private IEdmExpression ComputeIfFalse()
        {
            return CsdlSemanticsModel.WrapExpression(this.expression.IfFalse, this.BindingContext, this.Schema);
        }
    }
}
