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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl String constant expression.
    /// </summary>
    internal class CsdlSemanticsStringConstantExpression : CsdlSemanticsExpression, IEdmStringConstantExpression
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsStringConstantExpression, string> valueCache = new Cache<CsdlSemanticsStringConstantExpression, string>();
        private static readonly Func<CsdlSemanticsStringConstantExpression, string> ComputeValueFunc = (me) => me.ComputeValue();

        public CsdlSemanticsStringConstantExpression(CsdlConstantExpression expression, CsdlSemanticsSchema schema)
            : base(schema, expression)
        {
            this.expression = expression;
        }

        public override CsdlElement Element
        {
            get { return this.expression; }
        }

        public string Value
        {
            get { return this.valueCache.GetValue(this, ComputeValueFunc, null); }
        }

        public override EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.StringConstant; }
        }

        public IEdmTypeReference Type
        {
            get { return null; }
        }

        public EdmValueKind ValueKind
        {
            get { return this.expression.ValueKind; }
        }

        private string ComputeValue()
        {
            return this.expression.Value;
        }
    }
}
