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
    /// Provides semantics for a Csdl binary constant expression.
    /// </summary>
    internal class CsdlSemanticsBinaryConstantExpression : CsdlSemanticsExpression, IEdmBinaryConstantExpression, IEdmCheckable
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsBinaryConstantExpression, byte[]> valueCache = new Cache<CsdlSemanticsBinaryConstantExpression, byte[]>();
        private static readonly Func<CsdlSemanticsBinaryConstantExpression, byte[]> ComputeValueFunc = (me) => me.ComputeValue();

        private readonly Cache<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>> errorsCache = new Cache<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>>();
        private static readonly Func<CsdlSemanticsBinaryConstantExpression, IEnumerable<EdmError>> ComputeErrorsFunc = (me) => me.ComputeErrors();

        public CsdlSemanticsBinaryConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public byte[] Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.BinaryConstant; }
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

        private byte[] ComputeValue()
        {
            byte[] binary;
            return EdmValueParser.TryParseBinary(this.expression.Value, out binary) ? binary : new byte[0];
        }

        private IEnumerable<EdmError> ComputeErrors()
        {
            byte[] value;
            if (!EdmValueParser.TryParseBinary(this.expression.Value, out value))
            {
                return new EdmError[] { new EdmError(this.Location, EdmErrorCode.InvalidBinary, Edm.Strings.ValueParser_InvalidBinary(this.expression.Value)) };
            }
            else
            {
                return Enumerable.Empty<EdmError>();
            }
        }
    }
}
