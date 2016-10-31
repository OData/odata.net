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
using System.Collections.Generic;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Path expression.
    /// </summary>
    internal class CsdlSemanticsPathExpression : CsdlSemanticsExpression, IEdmPathExpression
    {
        private readonly CsdlPathExpression expression;
        private readonly IEdmEntityType bindingContext;

        private readonly Cache<CsdlSemanticsPathExpression, IEnumerable<string>> pathCache = new Cache<CsdlSemanticsPathExpression, IEnumerable<string>>();
        private static readonly Func<CsdlSemanticsPathExpression, IEnumerable<string>> ComputePathFunc = (me) => me.ComputePath();

        public CsdlSemanticsPathExpression(CsdlPathExpression expression, IEdmEntityType bindingContext, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
            this.bindingContext = bindingContext;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Path; }
        }

        public IEnumerable<string> Path
        {
            get { return this.pathCache.GetValue(this, ComputePathFunc, null); }
        }

        private IEnumerable<string> ComputePath()
        {
            return this.expression.Path.Split(new char[] { '/' }, StringSplitOptions.None);
        }
    }
}
