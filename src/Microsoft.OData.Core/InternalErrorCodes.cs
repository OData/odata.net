//---------------------------------------------------------------------
// <copyright file="InternalErrorCodes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
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

        /// <summary>Unreachable codepath in ODataWriterCore.PropertyAndAnnotationCollector.</summary>
        ODataWriterCore_PropertyAndAnnotationCollector,

        /// <summary>Unreachable codepath in ODataWriterCore.ParentNestedResourceInfoScope.</summary>
        ODataWriterCore_ParentNestedResourceInfoScope,

        /// <summary>Unreachable codepath in ODataUtils.VersionString</summary>
        ODataUtils_VersionString_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtilsInternal.IsPayloadKindSupported</summary>
        ODataUtilsInternal_IsPayloadKindSupported_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataUtils.GetDefaultEncoding</summary>
        ODataUtils_GetDefaultEncoding_UnreachableCodePath,

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

        /// <summary>Unreachable codepath in ODataReaderCoreAsync.ReadAsynchronously.</summary>
        ODataReaderCoreAsync_ReadAsynchronously,

        /// <summary>Unreachable codepath in ODataCollectionReader.CreateReader.</summary>
        ODataCollectionReader_CreateReader_UnreachableCodePath,

        /// <summary>Unreachable codepath in ODataCollectionReaderCore.ReadImplementation.</summary>
        ODataCollectionReaderCore_ReadImplementation,

        /// <summary>Unreachable codepath in ODataCollectionReaderCoreAsync.ReadAsynchronously.</summary>
        ODataCollectionReaderCoreAsync_ReadAsynchronously,

        /// <summary>Unreachable codepath in ODataParameterReaderCore.ReadImplementation.</summary>
        ODataParameterReaderCore_ReadImplementation,

        /// <summary>Unreachable codepath in ODataParameterReaderCoreAsync.ReadAsynchronously.</summary>
        ODataParameterReaderCoreAsync_ReadAsynchronously,

        /// <summary>The value from the parameter reader must be a primitive value, or null</summary>
        ODataParameterReaderCore_ValueMustBePrimitiveOrNull,

        /// <summary>Unreachable codepath in ODataRawValueUtils.ConvertStringToPrimitive.</summary>
        ODataRawValueUtils_ConvertStringToPrimitive,

        /// <summary>Unreachable codepath in EdmCoreModel.PrimitiveType (unsupported type).</summary>
        EdmCoreModel_PrimitiveType,

        /// <summary>Unreachable codepath in ReaderValidationUtils.ResolveAndValidateTypeName in the strict branch, unexpected type kind.</summary>
        ReaderValidationUtils_ResolveAndValidateTypeName_Strict_TypeKind,

        /// <summary>Unreachable codepath in ReaderValidationUtils.ResolveAndValidateTypeName in the lax branch, unexpected type kind.</summary>
        ReaderValidationUtils_ResolveAndValidateTypeName_Lax_TypeKind,

        /// <summary>The ODataMetadataFormat.CreateOutputContextAsync was called, but this method is not yet supported.</summary>
        ODataMetadataFormat_CreateOutputContextAsync,

        /// <summary>The ODataMetadataFormat.CreateInputContextAsync was called, but this method is not yet supported.</summary>
        ODataMetadataFormat_CreateInputContextAsync,

        /// <summary>An unsupported method or property has been called on the IDictionary implementation of the ODataModelFunctions.</summary>
        ODataModelFunctions_UnsupportedMethodOrProperty,

        /// <summary>Unreachable codepath in ODataJsonLightPropertyAndValueDeserializer.ReadPropertyValue.</summary>
        ODataJsonLightPropertyAndValueDeserializer_ReadPropertyValue,

        /// <summary>Unreachable codepath in ODataJsonLightPropertyAndValueDeserializer.GetNonEntityValueKind.</summary>
        ODataJsonLightPropertyAndValueDeserializer_GetNonEntityValueKind,

        /// <summary>Unreachable codepath in ODataJsonLightReader.ReadResourceStart.</summary>
        ODataJsonLightReader_ReadResourceStart,

        /// <summary>Unreachable codepath in ODataJsonLightResourceDeserializer.ReadTopLevelResourceSetAnnotations.</summary>
        ODataJsonLightResourceDeserializer_ReadTopLevelResourceSetAnnotations,

        /// <summary>Unreachable codepath in ODataJsonLightCollectionDeserializer.ReadCollectionStart.</summary>
        ODataJsonLightCollectionDeserializer_ReadCollectionStart,

        /// <summary>Unreachable codepath in ODataJsonLightCollectionDeserializer.ReadCollectionStart.TypeKindFromPayloadFunc.</summary>
        ODataJsonLightCollectionDeserializer_ReadCollectionStart_TypeKindFromPayloadFunc,

        /// <summary>Unreachable codepath in ODataJsonLightCollectionDeserializer.ReadCollectionEnd.</summary>
        ODataJsonLightCollectionDeserializer_ReadCollectionEnd,

        /// <summary>Unreachable codepath in ODataJsonLightEntityReferenceLinkDeserializer.ReadSingleEntityReferenceLink.</summary>
        ODataJsonLightEntityReferenceLinkDeserializer_ReadSingleEntityReferenceLink,

        /// <summary>Unreachable codepath in ODataJsonLightEntityReferenceLinkDeserializer.ReadEntityReferenceLinksAnnotations.</summary>
        ODataJsonLightEntityReferenceLinkDeserializer_ReadEntityReferenceLinksAnnotations,

        /// <summary>Unreachable codepath in ODataJsonLightParameterDeserializer.ReadNextParameter.</summary>
        ODataJsonLightParameterDeserializer_ReadNextParameter,

        /// <summary>Unreachable codepath in EdmTypeWriterResolver.GetReturnType for operation import group.</summary>
        EdmTypeWriterResolver_GetReturnTypeForOperationImportGroup,

        /// <summary>Unreachable codepath in the indexer of ODataVersionCache for unknown versions.</summary>
        ODataVersionCache_UnknownVersion,
    }
}
