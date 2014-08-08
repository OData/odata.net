//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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
