//---------------------------------------------------------------------
// <copyright file="CsdlConstantExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Vocabularies;

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

        public override EdmExpressionKind ExpressionKind
        {
            get
            {
                switch (this.kind)
                {
                    case EdmValueKind.Binary:
                        return EdmExpressionKind.BinaryConstant;
                    case EdmValueKind.Boolean:
                        return EdmExpressionKind.BooleanConstant;
                    case EdmValueKind.DateTimeOffset:
                        return EdmExpressionKind.DateTimeOffsetConstant;
                    case EdmValueKind.Decimal:
                        return EdmExpressionKind.DecimalConstant;
                    case EdmValueKind.Floating:
                        return EdmExpressionKind.FloatingConstant;
                    case EdmValueKind.Guid:
                        return EdmExpressionKind.GuidConstant;
                    case EdmValueKind.Integer:
                        return EdmExpressionKind.IntegerConstant;
                    case EdmValueKind.String:
                        return EdmExpressionKind.StringConstant;
                    case EdmValueKind.Duration:
                        return EdmExpressionKind.DurationConstant;
                    case EdmValueKind.Date:
                        return EdmExpressionKind.DateConstant;
                    case EdmValueKind.TimeOfDay:
                        return EdmExpressionKind.TimeOfDayConstant;
                    case EdmValueKind.Null:
                        return EdmExpressionKind.Null;
                    default:
                        return EdmExpressionKind.None;
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
