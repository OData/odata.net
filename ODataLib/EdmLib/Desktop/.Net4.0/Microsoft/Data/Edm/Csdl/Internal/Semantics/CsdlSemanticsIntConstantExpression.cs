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
    /// Provides semantics for a Csdl Int constant expression.
    /// </summary>
    internal class CsdlSemanticsIntConstantExpression : CsdlSemanticsExpression, IEdmIntegerConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsIntConstantExpression, Int64> valueCache = new Cache<CsdlSemanticsIntConstantExpression, Int64>();
        private static readonly Func<CsdlSemanticsIntConstantExpression, Int64> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsIntConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsIntConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public Int64 Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.IntegerConstant; }
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

        private Int64 ComputeValue()
        {
            Int64? value;
            return EdmValueParser.TryParseLong(this.expression.Value, out value) ? value.Value : 0;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            Int64? value;
            if (!EdmValueParser.TryParseLong(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidInteger, Edm.Strings.ValueParser_InvalidInteger(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
