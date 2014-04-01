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
