//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Csdl.Internal.Parsing.Ast
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
                    case EdmValueKind.DateTime:
                        return Expressions.EdmExpressionKind.DateTimeConstant;
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
                    case EdmValueKind.Time:
                        return Expressions.EdmExpressionKind.TimeConstant;
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
