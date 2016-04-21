//---------------------------------------------------------------------
// <copyright file="ODataObjectModelExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData.Core
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Extension methods on the OData object model.
    /// </summary>
    public static class ODataObjectModelExtensions
    {
        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="resourceSet"/>.
        /// </summary>
        /// <param name="resourceSet">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataResourceSet resourceSet, ODataResourceSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(resourceSet, "resourceSet");
            resourceSet.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="resource"/>.
        /// </summary>
        /// <param name="resource">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataResource resource, ODataResourceSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(resource, "resource");
            resource.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="property"/>.
        /// </summary>
        /// <param name="property">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataProperty property, ODataPropertySerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(property, "property");
            property.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataCollectionWriter"/> for <paramref name="collectionStart"/>.
        /// </summary>
        /// <param name="collectionStart">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataCollectionStart collectionStart, ODataCollectionStartSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(collectionStart, "collectionStart");
            collectionStart.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaResourceSet"/>.
        /// </summary>
        /// <param name="deltaResourceSet">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaResourceSet deltaResourceSet, ODataDeltaResourceSetSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaResourceSet, "deltaResourceSet");
            deltaResourceSet.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaDeletedEntry"/>.
        /// </summary>
        /// <param name="deltaDeletedEntry">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaDeletedEntry deltaDeletedEntry, ODataDeltaSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaDeletedEntry, "deltaDeletedEntry");
            deltaDeletedEntry.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltalink"/>.
        /// </summary>
        /// <param name="deltalink">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaLinkBase deltalink, ODataDeltaSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltalink, "deltalink");
            deltalink.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Set the payload value converter
        /// </summary>
        /// <param name="model">The instance to set the value payload converter.</param>
        /// <param name="converter">The payload value converter to set.</param>
        public static void SetPayloadValueConverter(this IEdmModel model, ODataPayloadValueConverter converter)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");
            ExceptionUtils.CheckArgumentNotNull(converter, "converter");

            // Set payload value converter for the model.
            model.SetAnnotationValue(
                model,
                Metadata.EdmConstants.InternalUri,
                Metadata.EdmConstants.PayloadValueConverterAnnotation,
                converter);
        }

        /// <summary>
        /// Get the payload value converter
        /// </summary>
        /// <param name="model">The instance to get the value payload converter from.</param>
        /// <returns>The payload value converter</returns>
        public static ODataPayloadValueConverter GetPayloadValueConverter(this IEdmModel model)
        {
            ExceptionUtils.CheckArgumentNotNull(model, "model");

            // Get payload value converter for the model.
            ODataPayloadValueConverter converter = model.GetAnnotationValue(
                model,
                Metadata.EdmConstants.InternalUri,
                Metadata.EdmConstants.PayloadValueConverterAnnotation)
                as ODataPayloadValueConverter;

            return converter ?? ODataPayloadValueConverter.Default;
        }
    }
}