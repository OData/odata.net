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
using System.Linq;
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Validation;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl DateTime constant expression.
    /// </summary>
    internal class CsdlSemanticsTimeConstantExpression : CsdlSemanticsExpression, IEdmTimeConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsTimeConstantExpression, TimeSpan> valueCache = new Cache<CsdlSemanticsTimeConstantExpression, TimeSpan>();
        private static readonly Func<CsdlSemanticsTimeConstantExpression, TimeSpan> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsTimeConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsTimeConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsTimeConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsTimeConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public TimeSpan Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.TimeConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private TimeSpan ComputeValue()
        {
            TimeSpan? value;
            return EdmValueParser.TryParseTime(this.expression.Value, out value) ? value.Value : TimeSpan.Zero;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            TimeSpan? value;
            if (!EdmValueParser.TryParseTime(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidTime, Edm.Strings.ValueParser_InvalidTime(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
