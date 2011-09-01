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

        /// <summary>Unreachable codepath in ODataUtils.VersionString</summary>
        ODataUtils_VersionString_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtilsInternal.ToDataServiceVersion</summary>
        ODataUtilsInternal_ToDataServiceVersion_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtils.GetDefaultEncoding</summary>
        ODataUtils_GetDefaultEncoding_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtils.ParseSerializableEpmAnnotations</summary>
        ODataUtils_ParseSerializableEpmAnnotations_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataWriter.CreateWriter</summary>
        ODataWriter_CreateWriter_UnreachableCodePath,

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

        /// <summary>Unhandled SyndicationItemProperty enum value in EpmSyndicationWriter.WriteCategoryEpm.</summary>
        EpmSyndicationWriter_WriteCategoryEpm_TargetSyndicationItem,

        /// <summary>Unhandled SyndicationItemProperty enum value in EpmSyndicationWriter.WriteLinkEpm.</summary>
        EpmSyndicationWriter_WriteLinkEpm_TargetSyndicationItem,

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

        /// <summary>Unreachable codepath in ODataPathValidator.ValidateSegment root branch</summary>
        QueryPathValidator_ValidateSegment_Root,

        /// <summary>Unreachable codepath in ODataPathValidator.ValidateSegment non-root branch</summary>
        QueryPathValidator_ValidateSegment_NonRoot,

        /// <summary>Unreachable codepath in ODataBatchWriter.ValidateTransition</summary>
        ODataBatchWriter_ValidateTransition_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataBatchWriter.ToText(this HttpMethod).</summary>
        ODataBatchWriterUtils_HttpMethod_ToText_UnreachableCodePath,

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

        /// <summary>Unreachable codepath in ODataJsonEntryAndFeedDeserializer.ReadEntryProperty.</summary>
        ODataJsonEntryAndFeedDeserializer_ReadEntryProperty,

        /// <summary>Unreachable codepath in ODataJsonReader.ReadImplementation.</summary>
        ODataJsonReader_ReadImplementation_NavigationLinkEnd,

        /// <summary>Unreachable codepath in ODataJsonReader.ReadAtNavigationLinkStartImplementation.</summary>
        ODataJsonReader_ReadAtNavigationLinkStartImplementation,

        /// <summary>Unreachable codepath in ODataJsonPropertyAndValueDeserializer.ReadPropertyValue.</summary>
        ODataJsonPropertyAndValueDeserializer_ReadPropertyValue,

        /// <summary>Unreachable codepath in ODataCollectionReader.CreateReader.</summary>
        ODataCollectionReader_CreateReader_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataCollectionReader.ReadImplementation.</summary>
        ODataCollectionReader_ReadImplementation,

        /// <summary>Method or property on BufferingXmlReader was called which is not supported. Should never get there.</summary>
        BufferingXmlReader_UnsupportedMethod,

        /// <summary>Method or property on BufferingXmlReader was called which is not supported in the buffering mode, but the reader is in buffer mode. Should never get there.</summary>
        BufferingXmlReader_UnsupportedMethodWhileBuffering,

        /// <summary>Unrecognized format in ODataInputContext.CreateInputContextForStream.</summary>
        ODataInputContext_CreateInputContextForStream_UnrecognizedFormat,

        /// <summary>CreateBatchReader on ODataJsonInputContext should never be called.</summary>
        ODataJsonInputContext_CreateBatchReader,

        /// <summary>ReadValue on ODataJsonInputContext should never be called.</summary>
        ODataJsonInputContext_ReadValue,

        /// <summary>ReadMetadataDocument on ODataJsonInputContext should never be called.</summary>
        ODataJsonInputContext_ReadMetadataDocument,

        /// <summary>Unreachable codepath in ODataAtomReader.StartNavigationLink.</summary>
        ODataAtomReader_StartNavigationLink,

        /// <summary>Unreachable codepath in ODataAtomReader.ReadAtNavigationLinkStartImplementation.</summary>
        ODataAtomReader_ReadAtNavigationLinkStartImplementation,

        /// <summary>CreateBatchReader on ODataAtomInputContext should never be called.</summary>
        ODataAtomInputContext_CreateBatchReader,

        /// <summary>CreateParameterReader on ODataAtomInputContext should never be called.</summary>
        ODataAtomInputContext_CreateParameterReader,

        /// <summary>ReadValue on ODataAtomInputContext should never be called.</summary>
        ODataAtomInputContext_ReadValue,

        /// <summary>ReadMetadataDocument on ODataAtomInputContext should never be called.</summary>
        ODataAtomInputContext_ReadMetadataDocument,

        /// <summary>Unreachable codepath in ODataAtomPropertyAndValueDeserializer.ReadNonEntityValue.</summary>
        ODataAtomPropertyAndValueDeserializer_ReadNonEntityValue,

        /// <summary>CreateFeedReader on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_CreateFeedReader,

        /// <summary>CreateEntryReader on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_CreateEntryReader,

        /// <summary>CreateCollectionReader on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_CreateCollectionReader,

        /// <summary>CreateParameterReader on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_CreateParameterReader,

        /// <summary>ReadServiceDocument on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_ReadServiceDocument,

        /// <summary>ReadMetadataDocument on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_ReadMetadataDocument,

        /// <summary>ReadProperty on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_ReadProperty,

        /// <summary>ReadError on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_ReadError,

        /// <summary>ReadEntityReferenceLinks on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_ReadEntityReferenceLinks,

        /// <summary>ReadEntityReferenceLink on ODataRawInputContext should never be called.</summary>
        ODataRawInputContext_ReadEntityReferenceLink,

        /// <summary>Unreachable codepath in AtomValueUtils.ConvertStringToPrimitive.</summary>
        AtomValueUtils_ConvertStringToPrimitive,

        /// <summary>Unreachable codepath in EdmCoreModel.PrimitiveType (unsupported type).</summary>
        EdmCoreModel_PrimitiveType,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.ToTypeReference (unsupported type kind).</summary>
        EdmLibraryExtensions_ToTypeReference,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.ToClrType (unsupported type kind).</summary>
        EdmLibraryExtensions_ToClrType,

        /// <summary>Unreachable codepath in EdmLibraryExtensions.PrimitiveTypeReference (unsupported Clr type).</summary>
        EdmLibraryExtensions_PrimitiveTypeReference,

        /// <summary>CreateFeedReader on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_CreateFeedReader,

        /// <summary>CreateEntryReader on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_CreateEntryReader,

        /// <summary>CreateCollectionReader on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_CreateCollectionReader,

        /// <summary>CreateBatchReader on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_CreateBatchReader,

        /// <summary>CreateParameterReader on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_CreateParameterReader,

        /// <summary>ReadServiceDocument on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_ReadServiceDocument,

        /// <summary>ReadValue on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_ReadValue,

        /// <summary>ReadProperty on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_ReadProperty,

        /// <summary>ReadError on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_ReadError,

        /// <summary>ReadEntityReferenceLinks on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_ReadEntityReferenceLinks,

        /// <summary>ReadEntityReferenceLink on ODataMetadataInputContext should never be called.</summary>
        ODataMetadataInputContext_ReadEntityReferenceLink,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadEntryEpm when reading content target.</summary>
        EpmSyndicationReader_ReadEntryEpm_ContentTarget,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadEntryEpm when reading multivalue target.</summary>
        EpmSyndicationReader_ReadEntryEpm_MultiValueTarget,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadParentSegment.</summary>
        EpmSyndicationReader_ReadParentSegment_TargetSegmentName,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadPersonEpm.</summary>
        EpmSyndicationReader_ReadPersonEpm,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadLinkEpm.</summary>
        EpmSyndicationReader_ReadLinkEpm,

        /// <summary>Unreachable codepath in EpmSyndicationReader.ReadCategoryEpm.</summary>
        EpmSyndicationReader_ReadCategoryEpm,

        /// <summary>Unreachable codepath in EpmReader.SetEpmValueForSegment when found unexpected type kind.</summary>
        EpmReader_SetEpmValueForSegment_TypeKind,

        /// <summary>Unreachable codepath in EpmReader.SetEpmValueForSegment when found EPM for a primitive stream property.</summary>
        EpmReader_SetEpmValueForSegment_StreamProperty,

        /// <summary>Unreachable codepath in ReaderValidationUtils.ResolveAndValidateTypeName in the strict branch, unexpected type kind.</summary>
        ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind,

        /// <summary>Unreachable codepath in ReaderValidationUtils.ResolveAndValidateTypeName in the lax branch, unexpected type kind.</summary>
        ReaderValidationUtils_ResolveAndValidateTypeName_Lax_TypeKind,
    }
}
