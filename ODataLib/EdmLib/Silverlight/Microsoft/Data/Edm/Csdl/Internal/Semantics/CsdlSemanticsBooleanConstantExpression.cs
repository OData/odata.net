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
