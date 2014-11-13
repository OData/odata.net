//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
