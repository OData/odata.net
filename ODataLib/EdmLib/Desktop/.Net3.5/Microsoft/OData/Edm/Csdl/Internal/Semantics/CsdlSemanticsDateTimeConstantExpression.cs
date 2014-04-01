//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.OData.Edm.Expressions;
using Microsoft.OData.Edm.Internal;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl DateTime constant expression.
    /// </summary>
    internal class CsdlSemanticsDateTimeConstantExpression : CsdlSemanticsExpression, IEdmDateTimeConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDateTimeConstantExpression, DateTime> valueCache = new Cache<CsdlSemanticsDateTimeConstantExpression, DateTime>();
        private static readonly Func<CsdlSemanticsDateTimeConstantExpression, DateTime> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDateTimeConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDateTimeConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDateTimeConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDateTimeConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public DateTime Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateTimeConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private DateTime ComputeValue()
        {
            DateTime? value;
            return EdmValueParser.TryParseDateTime(this.expression.Value, out value) ? value.Value : DateTime.MinValue;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            DateTime? value;
            if (!EdmValueParser.TryParseDateTime(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDateTime, Edm.Strings.ValueParser_InvalidDateTime(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
