//---------------------------------------------------------------------
// <copyright file="InternalErrorCodes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// An enumeration that lists the internal errors.
    /// </summary>
    internal enum InternalErrorCodes
    {
        /// <summary>Unreachable codepath in TypePromotionUtils.GetFunctionSignatures(BinaryOperatorKind), unrecognized kind of binary operator.</summary>
        TypePromotionUtils_GetFunctionSignatures_Binary_UnreachableCodepath,

        /// <summary>Unreachable codepath in TypePromotionUtils.GetFunctionSignatures(UnaryOperatorKind), unrecognized kind of unary operator.</summary>
        TypePromotionUtils_GetFunctionSignatures_Unary_UnreachableCodepath,

        /// <summary>Unreachable codepath in UriQueryExpressionParser.ParseComparison</summary>
        /// <remarks>Was a new binary operator keyword without adding it to the switch in the ParseComparison?</remarks>
        UriQueryExpressionParser_ParseComparison,

        /// <summary>Unreachable codepath in UriPrimitiveTypeParser.TryUriStringToPrimitive</summary>
        /// <remarks>Unsupported type was asked to be parsed.</remarks>
        UriPrimitiveTypeParser_TryUriStringToPrimitive,

        /// <summary>Unreachable codepath in QueryNodeUtils.BinaryOperatorResultType, unrecognized kind of binary operator.</summary>
        QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath,
    }
}
