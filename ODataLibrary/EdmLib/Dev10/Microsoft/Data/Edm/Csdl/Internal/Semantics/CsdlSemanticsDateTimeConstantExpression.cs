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
using Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;

namespace Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Provides semantics for a Csdl DateTime constant expression.
    /// </summary>
    internal class CsdlSemanticsDateTimeConstantExpression : IEdmDateTimeConstantExpression
    {
        private readonly CsdlConstantExpression expression;

        private readonly Cache<CsdlSemanticsDateTimeConstantExpression, DateTime> valueCache = new Cache<CsdlSemanticsDateTimeConstantExpression, DateTime>();
        private readonly static Func<CsdlSemanticsDateTimeConstantExpression, DateTime> s_computeValue = (me) => me.ComputeValue();

        public CsdlSemanticsDateTimeConstantExpression(CsdlConstantExpression expression)
        {
            this.expression = expression;
        }

        public DateTime Value
        {
            get { return this.valueCache.GetValue(this, s_computeValue, null); }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.DateTimeConstant; }
        }

        private DateTime ComputeValue()
        {
            DateTime value;
            return DateTime.TryParse(this.expression.Value, out value) ? value : DateTime.MinValue;
        }
    }
}
