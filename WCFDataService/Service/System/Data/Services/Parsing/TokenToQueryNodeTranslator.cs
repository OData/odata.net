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

namespace System.Data.Services.Parsing
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Linq;
    using System.Spatial;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData;
    using Microsoft.Data.OData.Metadata;
    using Microsoft.Data.OData.Query;
    using Microsoft.Data.OData.Query.SemanticAst;
    using Enumerable = System.Linq.Enumerable;
    using Strings = System.Data.Services.Strings;

    /// <summary>
    /// Utility class for creating instances <see cref="QueryNode"/>. 
    /// </summary>
    internal static class TokenToQueryNodeTranslator
    {
        /// <summary>
        /// Tries to create a <see cref="ConstantNode"/> for the given token if it represents a literal.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="node">The node, if one was created.</param>
        /// <returns>Whether or not the token represented a literal.</returns>
        internal static bool TryCreateLiteral(ExpressionToken token, out ConstantNode node)
        {
            switch (token.Kind)
            {
                case ExpressionTokenKind.NullLiteral:
                    node = new ConstantNode(null);
                    break;

                case ExpressionTokenKind.BooleanLiteral:
                    node = ParseTypedLiteral(typeof(bool), XmlConstants.EdmBooleanTypeName, token);
                    break;

                case ExpressionTokenKind.DateTimeLiteral:
                    node = ParseTypedLiteral(typeof(DateTime), XmlConstants.EdmDateTimeTypeName, token);
                    break;

                case ExpressionTokenKind.DecimalLiteral:
                    node = ParseTypedLiteral(typeof(decimal), XmlConstants.EdmDecimalTypeName, token);
                    break;

                case ExpressionTokenKind.StringLiteral:
                    node = ParseTypedLiteral(typeof(string), XmlConstants.EdmStringTypeName, token);
                    break;

                case ExpressionTokenKind.Int64Literal:
                    node = ParseTypedLiteral(typeof(Int64), XmlConstants.EdmInt64TypeName, token);
                    break;

                case ExpressionTokenKind.IntegerLiteral:
                    node = ParseTypedLiteral(typeof(Int32), XmlConstants.EdmInt32TypeName, token);
                    break;

                case ExpressionTokenKind.DoubleLiteral:
                    node = ParseTypedLiteral(typeof(double), XmlConstants.EdmDoubleTypeName, token);
                    break;

                case ExpressionTokenKind.SingleLiteral:
                    node = ParseTypedLiteral(typeof(Single), XmlConstants.EdmSingleTypeName, token);
                    break;

                case ExpressionTokenKind.GuidLiteral:
                    node = ParseTypedLiteral(typeof(Guid), XmlConstants.EdmGuidTypeName, token);
                    break;

                case ExpressionTokenKind.BinaryLiteral:
                    node = ParseTypedLiteral(typeof(byte[]), XmlConstants.EdmBinaryTypeName, token);
                    break;

                case ExpressionTokenKind.TimeLiteral:
                    node = ParseTypedLiteral(typeof(TimeSpan), XmlConstants.EdmTimeTypeName, token);
                    break;

                case ExpressionTokenKind.DateTimeOffsetLiteral:
                    node = ParseTypedLiteral(typeof(DateTimeOffset), XmlConstants.EdmDateTimeOffsetTypeName, token);
                    break;

                case ExpressionTokenKind.GeographylLiteral:
                    node = ParseTypedLiteral(typeof(Geography), XmlConstants.EdmGeographyTypeName, token);
                    break;

                case ExpressionTokenKind.GeometryLiteral:
                    node = ParseTypedLiteral(typeof(Geometry), XmlConstants.EdmGeometryTypeName, token);
                    break;

                default:
                    node = null;
                    return false;
            }

            Debug.Assert(token.IsLiteral, "Token must be a value.");
            return true;
        }

        /// <summary>
        /// Parses the given token into a constant node of the given target type.
        /// </summary>
        /// <param name="targetType">The tarket type.</param>
        /// <param name="targetTypeName">The target type name.</param>
        /// <param name="token">The token to parse.</param>
        /// <returns>The parsed constant node.</returns>
        private static ConstantNode ParseTypedLiteral(Type targetType, string targetTypeName, ExpressionToken token)
        {
            object literalValue;
            if (!LiteralParser.ForExpressions.TryParseLiteral(targetType, token.Text, out literalValue))
            {
                string message = Strings.RequestQueryParser_UnrecognizedLiteral(targetTypeName, token.Text);
                throw DataServiceException.CreateSyntaxError(message);
            }

            return new ConstantNode(literalValue, token.Text);
        }
    }
}
