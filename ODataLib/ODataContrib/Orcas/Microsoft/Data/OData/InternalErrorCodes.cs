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

namespace Microsoft.Data.OData.Query
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

        /// <summary>Unreachable codepath in MetadataBinder.BindServiceOperation, unrecognized kind of service opertion.</summary>
        MetadataBinder_BindServiceOperation,

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
        ODataUriBuilder_WriteUnary_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUriBuilderUtils.ToText(InlineCountKind), unrecognized kind of inline count.</summary>
        ODataUriBuilderUtils_ToText_InlineCountKind_UnreachableCodePath,
    }
}
