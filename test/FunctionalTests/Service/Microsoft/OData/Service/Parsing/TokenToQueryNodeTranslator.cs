//---------------------------------------------------------------------
// <copyright file="TokenToQueryNodeTranslator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Parsing
{
    using System;
    using System.Diagnostics;
    using Microsoft.OData.UriParser;
    using Microsoft.Spatial;
    using Strings = Microsoft.OData.Service.Strings;

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

                case ExpressionTokenKind.DurationLiteral:
                    node = ParseTypedLiteral(typeof(TimeSpan), XmlConstants.EdmDurationTypeName, token);
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
