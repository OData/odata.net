//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
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
