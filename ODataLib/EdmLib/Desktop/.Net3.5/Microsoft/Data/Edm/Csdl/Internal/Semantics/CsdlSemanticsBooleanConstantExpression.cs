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
    /// Provides semantics for a Csdl Bool constant expression.
    /// </summary>
    internal class CsdlSemanticsBooleanConstantExpression : CsdlSemanticsExpression, IEdmBooleanConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsBooleanConstantExpression, bool> valueCache = new Cache<CsdlSemanticsBooleanConstantExpression, bool>();
        private static readonly Func<CsdlSemanticsBooleanConstantExpression, bool> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsBooleanConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsBooleanConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public bool Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.BooleanConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private bool ComputeValue()
        {
            bool? local;
            return EdmValueParser.TryParseBool(this.expression.Value, out local) ? local.Value : false;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            bool? value;
            if (!EdmValueParser.TryParseBool(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidBoolean, Edm.Strings.ValueParser_InvalidBoolean(this.expression.Value)) };
            }

            return Enumerable.Empty<EdmError>();
        }
    }
}
