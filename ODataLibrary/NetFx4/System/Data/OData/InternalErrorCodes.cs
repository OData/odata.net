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

namespace System.Data.OData
{
    /// <summary>
    /// An enumeration that lists the internal errors.
    /// </summary>
    internal enum InternalErrorCodes
    {
        /// <summary>Unreachable codepath in ODataWriterCore.WriteEnd</summary>
        ODataWriterCore_WriteEnd_UnreachableCodePath = 1,

        /// <summary>Unreachable codepath in ODataWriterCore.ValidateTransition</summary>
        ODataWriterCore_ValidateTransition_UnreachableCodePath = 2,

        /// <summary>Unreachable codepath in ODataWriterCore.Scope.Create</summary>
        ODataWriterCore_Scope_Create_UnreachableCodePath = 3,

        /// <summary>Unreachable codepath in ODataUtils.VersionString</summary>
        ODataUtils_VersionString_UnreachableCodePath = 4,

        /// <summary>Unreachable codepath in ODataUtils.VersionString</summary>
        ODataUtils_GetDefaultEncoding_UnreachableCodePath = 5,

        /// <summary>Unreachable codepath in ODataUtils.MatchMediaTypes</summary>
        ODataUtils_MatchMediaTypes_UnreachableCodePath = 6,

        /// <summary>Unreachable codepath in ODataWriter.CreateWriter</summary>
        ODataWriter_CreateWriter_UnreachableCodePath = 7,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteProperty</summary>
        ODataMessageWriter_WriteProperty = 8,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteAssociatedEntityLink</summary>
        ODataMessageWriter_WriteAssociatedEntityLink = 9,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteAssociatedEntityLinks</summary>
        ODataMessageWriter_WriteAssociatedEntityLinks = 10,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteError</summary>
        ODataMessageWriter_WriteError = 11,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteProperty</summary>
        ODataMessageWriter_WriteServiceDocument = 12,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.WriteEntryEpm when writing content target.</summary>
        EpmSyndicationWriter_WriteEntryEpm_ContentTarget = 13,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.WriteEntryEpm when writing target segment.</summary>
        EpmSyndicationWriter_WriteEntryEpm_TargetSegment = 14,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.CreateAtomTextConstruct when converting text kind from Syndication enumeration.</summary>
        EpmSyndicationWriter_CreateAtomTextConstruct = 15,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.WritePersonEpm.</summary>
        EpmSyndicationWriter_WritePersonEpm = 16,

        /// <summary>Unreachable codepath in ODataAtomConvert.ToString(AtomTextConstructKind)</summary>
        ODataAtomConvert_ToString = 17,

        /// <summary>Unreachable codepath in ODataCollectionWriter.CreateCollectionWriter</summary>
        ODataCollectionWriter_CreateCollectionWriter_UnreachableCodePath = 18,

        /// <summary>Unreachable codepath in ODataCollectionWriterCore.ValidateTransition</summary>
        ODataCollectionWriterCore_ValidateTransition_UnreachableCodePath = 19,

        /// <summary>Unreachable codepath in ODataCollectionWriterCore.WriteEnd</summary>
        ODataCollectionWriterCore_WriteEnd_UnreachableCodePath = 20,

        /// <summary>Unreachable codepath in ODataPathValidator.ValidateSegment root branch</summary>
        QueryPathValidator_ValidateSegment_Root = 21,

        /// <summary>Unreachable codepath in ODataPathValidator.ValidateSegment non-root branch</summary>
        QueryPathValidator_ValidateSegment_NonRoot = 22,

        /// <summary>Unreachable codepath in ODataBatchWriter.ValidateTransition</summary>
        ODataBatchWriter_ValidateTransition_UnreachableCodePath = 23,

        /// <summary>Unreachable codepath in ODataBatchWriter.ToText(this HttpMethod).</summary>
        ODataBatchWriterUtils_HttpMethod_ToText_UnreachableCodePath = 24,

        /// <summary>Unreachable codepath in UriQueryExpressionParser.ParseComparison</summary>
        /// <remarks>Was a new binary operator keyword without adding it to the switch in the ParseComparison?</remarks>
        UriQueryExpressionParser_ParseComparison = 25,

        /// <summary>Unreachable codepath in UriPrimitiveTypeParser.TryUriStringToPrimitive</summary>
        /// <remarks>Unsupported type was asked to be parsed.</remarks>
        UriPrimitiveTypeParser_TryUriStringToPrimitive = 26,

        /// <summary>Unreachable codepath in UriPrimitiveTypeParser.HexCharToNibble</summary>
        UriPrimitiveTypeParser_HexCharToNibble = 27,

        /// <summary>Unreachable codepath in MetadataBinder.BindServiceOperation, unrecognized kind of service opertion.</summary>
        MetadataBinder_BindServiceOperation = 28,

        /// <summary>Unreachable codepath in QueryNodeUtils.BinaryOperatorResultType, unrecognized kind of binary operator.</summary>
        QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath = 29,

        /// <summary>Unreachable codepath in QueryExpressionTranslator.TranslateBinaryOperator, unrecognized kind of binary operator.</summary>
        QueryExpressionTranslator_TranslateBinaryOperator_UnreachableCodepath = 30,

        /// <summary>Unreachable codepath in QueryExpressionTranslator.TranslateUnaryOperator, unrecognized kind of unary operator.</summary>
        QueryExpressionTranslator_TranslateUnaryOperator_UnreachableCodepath = 31,

        /// <summary>Unreachable codepath in TypePromotionUtils.GetFunctionSignatures(BinaryOperatorKind), unrecognized kind of binary operator.</summary>
        TypePromotionUtils_GetFunctionSignatures_Binary_UnreachableCodepath = 32,

        /// <summary>Unreachable codepath in TypePromotionUtils.GetFunctionSignatures(UnaryOperatorKind), unrecognized kind of unary operator.</summary>
        TypePromotionUtils_GetFunctionSignatures_Unary_UnreachableCodepath = 33,
    }
}
