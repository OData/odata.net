//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser
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

        /// <summary>Unreachable codepath in QueryExpressionTranslator.TranslateBinaryOperator, unrecognized kind of binary operator.</summary>
        QueryExpressionTranslator_TranslateBinaryOperator_UnreachableCodepath,

        /// <summary>Unreachable codepath in UriPrimitiveTypeParser.HexCharToNibble</summary>
        UriPrimitiveTypeParser_HexCharToNibble,

        /// <summary>Unreachable codepath in UriQueryExpressionParser.ParseComparison</summary>
        /// <remarks>Was a new binary operator keyword without adding it to the switch in the ParseComparison?</remarks>
        UriQueryExpressionParser_ParseComparison,

        /// <summary>Unreachable codepath in UriPrimitiveTypeParser.TryUriStringToPrimitive</summary>
        /// <remarks>Unsupported type was asked to be parsed.</remarks>
        UriPrimitiveTypeParser_TryUriStringToPrimitive,

        /// <summary>Unreachable codepath in QueryNodeUtils.BinaryOperatorResultType, unrecognized kind of binary operator.</summary>
        QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath,

        /// <summary>Unreachable codepath in QueryExpressionTranslator.TranslateUnaryOperator, unrecognized kind of unary operator.</summary>
        QueryExpressionTranslator_TranslateUnaryOperator_UnreachableCodepath,

        /// <summary>Unreachable codepath in BinaryOperator.GetOperator, unrecognized kind of binary operator.</summary>
        BinaryOperator_GetOperator_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUriBuilder.WriteUnary, unrecognized kind of unary operator.</summary>
        ODataUriBuilder_WriteUnary_UnreachableCodePath
    }
}
