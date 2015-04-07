//---------------------------------------------------------------------
// <copyright file="ODataObjectModelExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Core.PrimitivePayloadValueConverters;
using Microsoft.OData.Edm;
using System.Diagnostics;
namespace Microsoft.OData.Core
{
    /// <summary>
    /// Extension methods on the OData object model.
    /// </summary>
    public static class ODataObjectModelExtensions
    {
        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="feed"/>.
        /// </summary>
        /// <param name="feed">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataFeed feed, ODataFeedAndEntrySerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(feed, "feed");
            feed.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="entry"/>.
        /// </summary>
        /// <param name="entry">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataEntry entry, ODataFeedAndEntrySerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(entry, "entry");
            entry.SerializationInfo = serializationInfo;
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
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaFeed"/>.
        /// </summary>
        /// <param name="deltaFeed">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaFeed deltaFeed, ODataDeltaFeedSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaFeed, "deltaFeed");
            deltaFeed.SerializationInfo = serializationInfo;
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
        /// Set the default of primitive payload value converter
        /// </summary>
        /// <param name="model">The instance to set the primitive value payload converter.</param>
        /// <param name="converter">The primitive payload value converter to set.</param>
        public static void SetPrimitivePayloadValueConverter(this IEdmModel model, IPrimitivePayloadValueConverter converter)
        {
            Debug.Assert(model != null, "model != null");
            Debug.Assert(converter != null, "converter != null");

            // Set primitive payload value converter for the model.
            model.SetAnnotationValue(model, 
                Microsoft.OData.Core.Metadata.EdmConstants.InternalUri, 
                Microsoft.OData.Core.Metadata.EdmConstants.PrimitivePayloadValueConverterAnnotation,
                converter);
        }

        /// <summary>
        /// Get the default of primitive payload value converter
        /// </summary>
        /// <param name="model">The instance to get the primitive value payload converter from.</param>
        /// <returns>The primitive payload value converter</returns>
        public static IPrimitivePayloadValueConverter GetPrimitivePayloadValueConverter(this IEdmModel model)
        {
            Debug.Assert(model != null, "model != null");

            // Get primitive payload value converter for the model.
            object annotationValue = model.GetAnnotationValue(model,
                Microsoft.OData.Core.Metadata.EdmConstants.InternalUri,
                Microsoft.OData.Core.Metadata.EdmConstants.PrimitivePayloadValueConverterAnnotation);

            IPrimitivePayloadValueConverter converter = annotationValue as IPrimitivePayloadValueConverter;
            if (converter != null)
                return converter;

            return DefaultPrimitivePayloadValueConverter.Instance;
        }
    }
}