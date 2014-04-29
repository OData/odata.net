//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl Decimal constant expression.
    /// </summary>
    internal class CsdlSemanticsDecimalConstantExpression : CsdlSemanticsExpression, IEdmDecimalConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDecimalConstantExpression, decimal> valueCache = new Cache<CsdlSemanticsDecimalConstantExpression, decimal>();
        private static readonly Func<CsdlSemanticsDecimalConstantExpression, decimal> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDecimalConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDecimalConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public decimal Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DecimalConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private decimal ComputeValue()
        {
            decimal? value;
            return EdmValueParser.TryParseDecimal(this.expression.Value, out value) ? value.Value : 0;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            decimal? value;
            if (!EdmValueParser.TryParseDecimal(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDecimal, Edm.Strings.ValueParser_InvalidDecimal(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
