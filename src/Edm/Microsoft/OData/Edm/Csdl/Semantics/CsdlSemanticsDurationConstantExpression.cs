//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
    /// Provides semantics for a Csdl duration constant expression.
    /// </summary>
    internal class CsdlSemanticsDurationConstantExpression : CsdlSemanticsExpression, IEdmDurationConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDurationConstantExpression, TimeSpan> valueCache = new Cache<CsdlSemanticsDurationConstantExpression, TimeSpan>();
        private static readonly Func<CsdlSemanticsDurationConstantExpression, TimeSpan> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsDurationConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsDurationConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
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
            get { return EdmExpressionKind.DurationConstant; }
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
            return EdmValueParser.TryParseDuration(this.expression.Value, out value) ? value.Value : TimeSpan.Zero;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            TimeSpan? value;
            if (!EdmValueParser.TryParseDuration(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidDuration, Edm.Strings.ValueParser_InvalidDuration(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
