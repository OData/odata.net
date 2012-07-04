//   Copyright 2011 Microsoft Corporation
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
    /// Provides semantics for a Csdl guid constant expression.
    /// </summary>
    internal class CsdlSemanticsGuidConstantExpression : CsdlSemanticsExpression, IEdmGuidConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsGuidConstantExpression, Guid> valueCache = new Cache<CsdlSemanticsGuidConstantExpression, Guid>();
        private static readonly Func<CsdlSemanticsGuidConstantExpression, Guid> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsGuidConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsGuidConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public Guid Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.GuidConstant; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        public IEnumerable<EdmError> Errors
        {
            get { return this.errorsCache.GetValue(this, ComputeErrorsFunc, null); }
        }

        private Guid ComputeValue()
        {
            Guid? value;
            return EdmValueParser.TryParseGuid(this.expression.Value, out value) ? value.Value : Guid.Empty;
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            Guid? value;
            if (!EdmValueParser.TryParseGuid(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidGuid, Edm.Strings.ValueParser_InvalidGuid(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
