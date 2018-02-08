//---------------------------------------------------------------------
// <copyright file="ODataObjectModelExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------
namespace Microsoft.OData
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
        public static void SetSerializationInfo(this ODataResourceBase resource, ODataResourceSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(resource, "resource");
            resource.SerializationInfo = serializationInfo;
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
        /// Provide additional serialization information to the <see cref="ODataWriter"/> for <paramref name="nestedResourceInfo"/>.
        /// </summary>
        /// <param name="nestedResourceInfo">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataNestedResourceInfo nestedResourceInfo, ODataNestedResourceInfoSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(nestedResourceInfo, "resource");
            nestedResourceInfo.SerializationInfo = serializationInfo;
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
            ODataResourceSerializationInfo resourceSerializationInfo = new ODataResourceSerializationInfo()
            {
                NavigationSourceName = serializationInfo.EntitySetName,
                NavigationSourceEntityTypeName = serializationInfo.EntityTypeName,
                NavigationSourceKind = EdmNavigationSourceKind.EntitySet,
                ExpectedTypeName = serializationInfo.ExpectedTypeName
            };

            deltaResourceSet.SerializationInfo = resourceSerializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaResourceSet"/>.
        /// </summary>
        /// <param name="deltaResourceSet">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaResourceSet deltaResourceSet, ODataResourceSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaResourceSet, "deltaResourceSet");
            deltaResourceSet.SerializationInfo = serializationInfo;
        }

        /// <summary>
        /// Provide additional serialization information to the <see cref="ODataDeltaWriter"/> for <paramref name="deltaDeletedEntry"/>.
        /// </summary>
        /// <param name="deltaDeletedEntry">The instance to set the serialization info.</param>
        /// <param name="serializationInfo">The serialization info to set.</param>
        public static void SetSerializationInfo(this ODataDeltaDeletedEntry deltaDeletedEntry, ODataResourceSerializationInfo serializationInfo)
        {
            ExceptionUtils.CheckArgumentNotNull(deltaDeletedEntry, "deltaDeletedEntry");
            ODataDeltaSerializationInfo resourceSerializationInfo = new ODataDeltaSerializationInfo()
            {
                NavigationSourceName = serializationInfo.NavigationSourceName,
            };

            deltaDeletedEntry.SerializationInfo = resourceSerializationInfo;
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
    }
}