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
    /// Provides semantics for a Csdl datetime with offset constant expression.
    /// </summary>
    internal class CsdlSemanticsDateTimeOffsetConstantExpression : CsdlSemanticsExpression, IEdmDateTimeOffsetConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset> valueCache = new Cache<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset>();
        private static readonly Func<CsdlSemanticsDateTimeOffsetConstantExpression, DateTimeOffset> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDateTimeOffsetConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDateTimeOffsetConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public DateTimeOffset Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateTimeOffsetConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private DateTimeOffset ComputeValue()
        {
            DateTimeOffset? value;
            return EdmValueParser.TryParseDateTimeOffset(this.expression.Value, out value) ? value.Value : DateTimeOffset.MinValue;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            DateTimeOffset? value;
            if (!EdmValueParser.TryParseDateTimeOffset(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDateTimeOffset, Edm.Strings.ValueParser_InvalidDateTimeOffset(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
