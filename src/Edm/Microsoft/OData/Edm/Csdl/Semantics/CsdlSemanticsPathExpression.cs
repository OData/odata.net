//   OData .NET Libraries ver. 6.9
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
using System.Collections.Generic;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Path expression.
    /// </summary>
    internal class CsdlSemanticsPathExpression : CsdlSemanticsExpression, IEdmPathExpression
    {
        protected readonly CsdlPathExpression Expression;

        protected readonly IEdmEntityType BindingContext;

        protected readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> PathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();

        protected static readonly Func<CsdlSemanticsPathExpression, IEnumerable<string>> ComputePathFunc = (me) => me.ComputePath();

        public CsdlSemanticsPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.Expression = expression;
            this.BindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.Expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public IEnumerable<string> Path
        {
            get { return this.PathCache.GetValue(this, ComputePathFunc, null); }
        }

        private IEnumerable<string> ComputePath()
        {
            return this.Expression.Path.Split(new char[] { '/' }, StringSplitOptions.None);
        }
    }
}
