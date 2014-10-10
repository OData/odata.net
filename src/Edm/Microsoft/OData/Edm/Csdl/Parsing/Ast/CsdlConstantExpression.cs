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

using Microsoft.OData.Edm.Values;

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
    /// <summary>
    /// Represents a CSDL constant expression.
    /// </summary>
    internal class CsdlConstantExpression : CsdlExpressionBase
    {
        private readonly EdmValueKind kind;
        private readonly string value;

        public CsdlConstantExpression(EdmValueKind kind, string value, CsdlLocation location)
            : base(location)
        {
            this.kind = kind;
            this.value = value;
        }

        public override Expressions.EdmExpressionKind ExpressionKind
        {
            get
            {
                switch (this.kind)
                {
                    case EdmValueKind.Binary:
                        return Expressions.EdmExpressionKind.BinaryConstant;
                    case EdmValueKind.Boolean:
                        return Expressions.EdmExpressionKind.BooleanConstant;
                    case EdmValueKind.DateTimeOffset:
                        return Expressions.EdmExpressionKind.DateTimeOffsetConstant;
                    case EdmValueKind.Decimal:
                        return Expressions.EdmExpressionKind.DecimalConstant;
                    case EdmValueKind.Floating:
                        return Expressions.EdmExpressionKind.FloatingConstant;
                    case EdmValueKind.Guid:
                        return Expressions.EdmExpressionKind.GuidConstant;
                    case EdmValueKind.Integer:
                        return Expressions.EdmExpressionKind.IntegerConstant;
                    case EdmValueKind.String:
                        return Expressions.EdmExpressionKind.StringConstant;
                    case EdmValueKind.Duration:
                        return Expressions.EdmExpressionKind.DurationConstant;
                    case EdmValueKind.Date:
                        return Expressions.EdmExpressionKind.DateConstant;
                    case EdmValueKind.TimeOfDay:
                        return Expressions.EdmExpressionKind.TimeOfDayConstant;
                    case EdmValueKind.Null:
                        return Expressions.EdmExpressionKind.Null;
                    default:
                        return Expressions.EdmExpressionKind.None;
                }
            }
        }

        public EdmValueKind ValueKind
        {
            get
            {
                return this.kind;
            }
        }

        public string Value
        {
            get { return this.value; }
        }
    }
}
