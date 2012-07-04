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

namespace Microsoft.Data.OData
{
    /// <summary>
    /// An enumeration that lists the internal errors.
    /// </summary>
    internal enum InternalErrorCodes
    {
        /// <summary>Unreachable codepath in ODataWriterCore.WriteEnd</summary>
        ODataWriterCore_WriteEnd_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataWriterCore.ValidateTransition</summary>
        ODataWriterCore_ValidateTransition_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataWriterCore.Scope.Create</summary>
        ODataWriterCore_Scope_Create_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataWriterCore.DuplicatePropertyNamesChecker.</summary>
        ODataWriterCore_DuplicatePropertyNamesChecker,

        /// <summary>Unreachable codepath in ODataWriterCore.GetParentNavigationLinkScope.</summary>
        ODataWriterCore_ParentNavigationLinkScope,

        /// <summary>Unreachable codepath in ODataUtils.VersionString</summary>
        ODataUtils_VersionString_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtilsInternal.ToDataServiceVersion</summary>
        ODataUtilsInternal_ToDataServiceVersion_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtilsInternal.IsPayloadKindSupported</summary>
        ODataUtilsInternal_IsPayloadKindSupported_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtils.GetDefaultEncoding</summary>
        ODataUtils_GetDefaultEncoding_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtils.ParseSerializableEpmAnnotations</summary>
        ODataUtils_ParseSerializableEpmAnnotations_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteProperty</summary>
        ODataMessageWriter_WriteProperty,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteEntityReferenceLink</summary>
        ODataMessageWriter_WriteEntityReferenceLink,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteEntityReferenceLinks</summary>
        ODataMessageWriter_WriteEntityReferenceLinks,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteError</summary>
        ODataMessageWriter_WriteError,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteServiceDocument</summary>
        ODataMessageWriter_WriteServiceDocument,

        /// <summary>Unreachable codepath in ODataMessageWriter.WriteMetadataDocument</summary>
        ODataMessageWriter_WriteMetadataDocument,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.WriteEntryEpm when writing content target.</summary>
        EpmSyndicationWriter_WriteEntryEpm_ContentTarget,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.CreateAtomTextConstruct when converting text kind from Syndication enumeration.</summary>
        EpmSyndicationWriter_CreateAtomTextConstruct,

        /// <summary>Unreachable codepath in EpmSyndicationWriter.WritePersonEpm.</summary>
        EpmSyndicationWriter_WritePersonEpm,

        /// <summary>Unhandled EpmTargetPathSegment.SegmentName in EpmSyndicationWriter.WriteParentSegment.</summary>
        EpmSyndicationWriter_WriteParentSegment_TargetSegmentName,

        /// <summary>Unreachable codepath in ODataAtomConvert.ToString(AtomTextConstructKind)</summary>
        ODataAtomConvert_ToString,

        /// <summary>Unreachable codepath in ODataCollectionWriter.CreateCollectionWriter</summary>
        ODataCollectionWriter_CreateCollectionWriter_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataCollectionWriterCore.ValidateTransition</summary>
        ODataCollectionWriterCore_ValidateTransition_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataCollectionWriterCore.WriteEnd</summary>
        ODataCollectionWriterCore_WriteEnd_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataParameterWriter.CreateParameterWriter</summary>
        ODataParameterWriter_CannotCreateParameterWriterForFormat,

        /// <summary>Unreachable codepath in ODataParameterWriter.ValidateTransition</summary>
        ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromStart,

        /// <summary>Unreachable codepath in ODataParameterWriter.ValidateTransition</summary>
        ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromCanWriteParameter,

        /// <summary>Unreachable codepath in ODataParameterWriter.ValidateTransition</summary>
        ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromActiveSubWriter,

        /// <summary>Unreachable codepath in ODataParameterWriter.ValidateTransition</summary>
        ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromCompleted,

        /// <summary>Unreachable codepath in ODataParameterWriter.ValidateTransition</summary>
        ODataParameterWriterCore_ValidateTransition_InvalidTransitionFromError,

        /// <summary>Unreachable codepath in ODataParameterWriter.ValidateTransition</summary>
        ODataParameterWriterCore_ValidateTransition_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataParameterWriter.WriteEndImplementation</summary>
        ODataParameterWriterCore_WriteEndImplementation_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataPathValidator.ValidateSegment root branch</summary>
        QueryPathValidator_ValidateSegment_Root,

        /// <summary>Unreachable codepath in ODataPathValidator.ValidateSegment non-root branch</summary>
        QueryPathValidator_ValidateSegment_NonRoot,

        /// <summary>Unreachable codepath in ODataBatchWriter.ValidateTransition</summary>
        ODataBatchWriter_ValidateTransition_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataBatchWriter.ToText(this HttpMethod).</summary>
        ODataBatchWriterUtils_HttpMethod_ToText_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataBatchReader.ReadImplementation.</summary>
        ODataBatchReader_ReadImplementation,

        /// <summary>Unreachable codepath in ODataBatchReader.GetEndBoundary in state Completed.</summary>
        ODataBatchReader_GetEndBoundary_Completed,

        /// <summary>Unreachable codepath in ODataBatchReader.GetEndBoundary in state Exception.</summary>
        ODataBatchReader_GetEndBoundary_Exception,

        /// <summary>Unreachable codepath in ODataBatchReader.GetEndBoundary because of invalid enum value.</summary>
        ODataBatchReader_GetEndBoundary_UnknownValue,

        /// <summary>Unreachable codepath in ODataBatchReaderStream.SkipToBoundary.</summary>
        ODataBatchReaderStream_SkipToBoundary,

        /// <summary>Unreachable codepath in ODataBatchReaderStream.ReadLine.</summary>
        ODataBatchReaderStream_ReadLine,

        /// <summary>Unreachable codepath in ODataBatchReaderStream.ReadWithDelimiter.</summary>
        ODataBatchReaderStream_ReadWithDelimiter,

        /// <summary>Unreachable codepath in ODataBatchReaderStreamBuffer.ScanForBoundary.</summary>
        ODataBatchReaderStreamBuffer_ScanForBoundary,

        /// <summary>Unreachable codepath in ODataBatchReaderStreamBuffer.ReadWithLength.</summary>
        ODataBatchReaderStreamBuffer_ReadWithLength,

        /// <summary>Unreachable codepath in JsonReader.Read.</summary>
        JsonReader_Read,

        /// <summary>Unreachable codepath in ODataReader.CreateReader.</summary>
        ODataReader_CreateReader_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataReaderCore.ReadImplementation.</summary>
        ODataReaderCore_ReadImplementation,

        /// <summary>Unreachable codepath in ODataJsonEntryAndFeedDeserializer.ReadFeedProperty.</summary>
        ODataJsonEntryAndFeedDeserializer_ReadFeedProperty,

        /// <summary>Unreachable codepath in ODataJsonReader.ReadEntryStart.</summary>
        ODataJsonReader_ReadEntryStart,

        /// <summary>Unreachable codepath in ODataJsonReaderUtils.GetPayloadTypeName.</summary>
        ODataJsonReaderUtils_GetPayloadTypeName,

        /// <summary>Unreachable codepath in ODataJsonEntryAndFeedDeserializer.ReadEntryProperty.</summary>
        ODataJsonEntryAndFeedDeserializer_ReadEntryProperty,

        /// <summary>Unreachable codepath in ODataJsonReader.ReadAtNavigationLinkStartImplementation.</summary>
        ODataJsonReader_ReadAtNavigationLinkStartImplementation,

        /// <summary>Unreachable codepath in ODataJsonPropertyAndValueDeserializer.ReadPropertyValue.</summary>
        ODataJsonPropertyAndValueDeserializer_ReadPropertyValue,

        /// <summary>Unreachable codepath in ODataCollectionReader.CreateReader.</summary>
        ODataCollectionReader_CreateReader_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataCollectionReader.ReadImplementation.</summary>
        ODataCollectionReader_ReadImplementation,

        /// <summary>Unreachable codepath in ODataParameterReader.ReadImplementation.</summary>
        ODataParameterReader_ReadImplementation,

        /// <summary>The value from the parameter reader must be a primitive value, an ODataComplexValue or null</summary>
        ODataParameterReaderCore_ValueMustBePrimitiveOrComplexOrNull,

        /// <summary>Unrecognized format in ODataInputContext.CreateInputContext.</summary>
        ODataInputContext_CreateInputContext_UnrecognizedFormat,

        /// <summary>Unrecognized format in ODataOutputContext.CreateInputContext.</summary>
        ODataOutputContext_CreateOutputContext_UnrecognizedFormat,

        /// <summary>Unreachable codepath in ODataAtomReader.ReadAtNavigationLinkStartImplementation.</summary>
        ODataAtomReader_ReadAtNavigationLinkStartImplementation,

        /// <summary>Unreachable codepath in ODataAtomPropertyAndValueDeserializer.ReadNonEntityValue.</summary>
        ODataAtomPropertyAndValueDeserializer_ReadNonEntityValue,

        /// <summary>Unreachable codepath in AtomValueUtils.ConvertStringToPrimitive.</summary>
        AtomValueUtils_ConvertStringToPrimitive,

        /// <summary>Unreachable codepath in EdmCoreModel.PrimitiveType (unsupported type).</summary>
        EdmCoreModel_PrimitiveType,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadEntryEpm when reading content target.</summary>
        EpmSyndicationReader_ReadEntryEpm_ContentTarget,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadParentSegment.</summary>
        EpmSyndicationReader_ReadParentSegment_TargetSegmentName,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadPersonEpm.</summary>
        EpmSyndicationReader_ReadPersonEpm,

        /// <summary>Unreachable codepath in EpmReader.SetEpmValueForSegment when found unexpected type kind.</summary>
        EpmReader_SetEpmValueForSegment_TypeKind,

        /// <summary>Unreachable codepath in EpmReader.SetEpmValueForSegment when found EPM for a primitive stream property.</summary>
        EpmReader_SetEpmValueForSegment_StreamProperty,

        /// <summary>Unreachable codepath in ReaderValidationUtils.ResolveAndValidateTypeName in the strict branch, unexpected type kind.</summary>
        ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind,

        /// <summary>Unreachable codepath in ReaderValidationUtils.ResolveAndValidateTypeName in the lax branch, unexpected type kind.</summary>
        ReaderValidationUtils_ResolveAndValidateTypeName_Lax_TypeKind,

        /// <summary>Unreachable codepath in EpmExtensionMethods.ToAttributeValue(ODataSyndicationItemProperty) when found unexpected type syndication item property kind.</summary>
        EpmExtensionMethods_ToAttributeValue_SyndicationItemProperty,
    }
}
