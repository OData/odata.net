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
