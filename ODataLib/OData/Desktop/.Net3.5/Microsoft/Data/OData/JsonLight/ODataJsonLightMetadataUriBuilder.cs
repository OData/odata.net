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

namespace Microsoft.Data.OData.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Builder class to construct the metadata pointers for the Json Light format for the various payload kinds.
    /// </summary>
    internal abstract class ODataJsonLightMetadataUriBuilder
    {
        /// <summary>
        /// Gets the base URI of the metadata document uri. May be null to indicate that there is no metadata document uri.
        /// </summary>
        internal abstract Uri BaseUri { get; }

        /// <summary>
        /// Creates a metadata uri builder after validating user input.
        /// </summary>
        /// <param name="metadataLevel">The JSON Light metadata level being written.</param>
        /// <param name="writingResponse">if set to <c>true</c> indicates that a response is being written.</param>
        /// <param name="writerSettings">The writer settings.</param>
        /// <param name="model">The Edm model instance.</param>
        /// <returns>The metadata uri builder to use while writing.</returns>
        internal static ODataJsonLightMetadataUriBuilder CreateFromSettings(JsonLightMetadataLevel metadataLevel, bool writingResponse, ODataMessageWriterSettings writerSettings, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writerSettings != null, "writerSettings != null");
            Debug.Assert(metadataLevel != null, "metadataLevel != null");
            Debug.Assert(model != null, "model != null");

            // if the content type explictly specifies no OData metadata, then the metadata URL should not
            // be written regardless of whether it was provided.
            if (!metadataLevel.ShouldWriteODataMetadataUri())
            {
                return NullMetadataUriBuilder.Instance;
            }

            // for other metadata levels, responses require a metadata URL to be provided.
            ODataMetadataDocumentUri metadataDocumentUri = writerSettings.MetadataDocumentUri;
            if (metadataDocumentUri == null)
            {
                if (writingResponse)
                {
                    throw new ODataException(OData.Strings.ODataJsonLightOutputContext_MetadataDocumentUriMissing);
                }

                return NullMetadataUriBuilder.Instance;
            }

            return CreateDirectlyFromUri(metadataDocumentUri, model, writingResponse);
        }

        /// <summary>
        /// Creates a metadata uri builder for the given base metadata document uri.
        /// DEVNOTE: specifically for unit testing.
        /// </summary>
        /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
        /// <param name="model">The Edm model instance.</param>
        /// <param name="writingResponse">if set to <c>true</c> indicates that a response is being written.</param>
        /// <returns>A new metadata uri builder.</returns>
        internal static ODataJsonLightMetadataUriBuilder CreateDirectlyFromUri(ODataMetadataDocumentUri metadataDocumentUri, IEdmModel model, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            return new DefaultMetadataUriBuilder(metadataDocumentUri, model, writingResponse);
        }

        /// <summary>
        /// Creates the metadata URI for a feed based on the entity set the entries in the feed belong to.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
        /// <param name="metadataUri">Returns the metadata URI for a feed based on the entity set the entries in the feed belong to.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildFeedMetadataUri(ODataFeedAndEntryTypeContext typeContext, out Uri metadataUri);

        /// <summary>
        /// Creates the metadata URI for an entry based on the entity set it belongs to.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
        /// <param name="metadataUri">Returns the metadata URI for an entry based on the entity set it belongs to.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildEntryMetadataUri(ODataFeedAndEntryTypeContext typeContext, out Uri metadataUri);

        /// <summary>
        /// Creates the metadata URI for a property based on its value.
        /// </summary>
        /// <param name="property">The property to create the metadata URI for.</param>
        /// <param name="metadataUri">Returns the metadata URI for a property based on its owning type.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildMetadataUriForValue(ODataProperty property, out Uri metadataUri);

        /// <summary>
        /// Creates the metadata URI for an entity reference link.
        /// </summary>
        /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
        /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
        /// <param name="navigationProperty">The navigation property to create the metadata URI for.</param>
        /// <param name="metadataUri">Returns the metadata URI for an entity reference link or a collection of entity reference links.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildEntityReferenceLinkMetadataUri(ODataEntityReferenceLinkSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri metadataUri);

        /// <summary>
        /// Creates the metadata URI for a collection of entity reference links.
        /// </summary>
        /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
        /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
        /// <param name="navigationProperty">The navigation property to create the metadata URI for.</param>
        /// <param name="metadataUri">Returns the metadata URI for an entity reference link or a collection of entity reference links.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildEntityReferenceLinksMetadataUri(ODataEntityReferenceLinksSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri metadataUri);

        /// <summary>
        /// Creates the metadata URI for an operation (function, action, service op) based on its function import.
        /// </summary>
        /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
        /// <param name="itemTypeReference">The item type of the collection.</param>
        /// <param name="metadataUri">Returns the metadata URI for an operation (function, action, service op) based on its function import.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildCollectionMetadataUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference, out Uri metadataUri);

        /// <summary>
        /// Creates the metadata URI for the service document.
        /// </summary>
        /// <param name="metadataUri">Returns the metadata URI for the service document.</param>
        /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
        internal abstract bool TryBuildServiceDocumentMetadataUri(out Uri metadataUri);

        /// <summary>
        /// Metadata uri builder which uses a user-provided uri and $select clause.
        /// </summary>
        private sealed class DefaultMetadataUriBuilder : ODataJsonLightMetadataUriBuilder
        {
            /// <summary>
            /// The base metadata document uri and $select clause provided by the user.
            /// </summary>
            private readonly ODataMetadataDocumentUri metadataDocumentUri;

            /// <summary>
            /// The Edm model instance.
            /// </summary>
            private readonly IEdmModel model;

            /// <summary>
            /// if set to <c>true</c> indicates that a response is being written.
            /// </summary>
            private readonly bool writingResponse;

            /// <summary>
            /// Initializes a new instance of the <see cref="DefaultMetadataUriBuilder"/> class.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="model">The Edm model instance.</param>
            /// <param name="writingResponse">if set to <c>true</c> indicates that a response is being written.</param>
            internal DefaultMetadataUriBuilder(ODataMetadataDocumentUri metadataDocumentUri, IEdmModel model, bool writingResponse)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
                Debug.Assert(model != null, "model != null");

                this.metadataDocumentUri = metadataDocumentUri;
                this.model = model;
                this.writingResponse = writingResponse;
            }

            /// <summary>
            /// Gets the base URI of the metadata document uri. May be null to indicate that there is no metadata document uri.
            /// </summary>
            internal override Uri BaseUri
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return this.metadataDocumentUri.BaseUri;
                }
            }

            /// <summary>
            /// Creates the metadata URI for a feed based on the entity set the entries in the feed belong to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
            /// <param name="metadataUri">Returns the metadata URI for a feed based on the entity set the entries in the feed belong to.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildFeedMetadataUri(ODataFeedAndEntryTypeContext typeContext, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = CreateFeedOrEntryMetadataUri(this.metadataDocumentUri, this.model, typeContext, /*isEntry*/ false, this.writingResponse);
                return metadataUri != null;
            }

            /// <summary>
            /// Creates the metadata URI for an entry based on the entity set it belongs to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
            /// <param name="metadataUri">Returns the metadata URI for an entry based on the entity set it belongs to.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildEntryMetadataUri(ODataFeedAndEntryTypeContext typeContext, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = CreateFeedOrEntryMetadataUri(this.metadataDocumentUri, this.model, typeContext, /*isEntry*/ true, this.writingResponse);
                return metadataUri != null;
            }

            /// <summary>
            /// Creates the metadata URI for a property based on its value.
            /// </summary>
            /// <param name="property">The property to create the metadata URI for.</param>
            /// <param name="metadataUri">Returns the metadata URI for a property based on its owning type.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildMetadataUriForValue(ODataProperty property, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(property != null, "property != null");

                string typeName = GetMetadataUriTypeNameForValue(property);
                if (string.IsNullOrEmpty(typeName))
                {
                    throw new ODataException(OData.Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
                }

                metadataUri = CreateTypeMetadataUri(this.metadataDocumentUri, typeName);
                return metadataUri != null;
            }

            /// <summary>
            /// Creates the metadata URI for an entity reference link.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the metadata URI for.</param>
            /// <param name="metadataUri">Returns the metadata URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinkMetadataUri(ODataEntityReferenceLinkSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                string entitySetName = null;
                string typecast = null;
                string navigationPropertyName = null;
                bool isCollectionNavigationProperty = false;
                if (serializationInfo != null)
                {
                    entitySetName = serializationInfo.SourceEntitySetName;
                    typecast = serializationInfo.Typecast;
                    navigationPropertyName = serializationInfo.NavigationPropertyName;
                    isCollectionNavigationProperty = serializationInfo.IsCollectionNavigationProperty;
                }
                else if (navigationProperty != null)
                {
                    entitySetName = GetEntitySetName(entitySet, this.model);
                    typecast = GetTypecast(entitySet, navigationProperty.DeclaringEntityType());
                    navigationPropertyName = navigationProperty.Name;
                    isCollectionNavigationProperty = navigationProperty.Type.IsCollection();
                }

                metadataUri = navigationPropertyName == null ? null : CreateEntityContainerElementMetadataUri(this.metadataDocumentUri, entitySetName, typecast, navigationPropertyName, /*appendItemSelector*/isCollectionNavigationProperty);
                if (this.writingResponse && metadataUri == null)
                {
                    throw new ODataException(OData.Strings.ODataJsonLightMetadataUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinkResponse);
                }

                return metadataUri != null;
            }

            /// <summary>
            /// Creates the metadata URI for a collection of entity reference links.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the metadata URI for.</param>
            /// <param name="metadataUri">Returns the metadata URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinksMetadataUri(ODataEntityReferenceLinksSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                string entitySetName = null;
                string typecast = null;
                string navigationPropertyName = null;
                if (serializationInfo != null)
                {
                    entitySetName = serializationInfo.SourceEntitySetName;
                    typecast = serializationInfo.Typecast;
                    navigationPropertyName = serializationInfo.NavigationPropertyName;
                }
                else if (navigationProperty != null)
                {
                    entitySetName = GetEntitySetName(entitySet, this.model);
                    typecast = GetTypecast(entitySet, navigationProperty.DeclaringEntityType());
                    navigationPropertyName = navigationProperty.Name;
                }

                metadataUri = navigationPropertyName == null ? null : CreateEntityContainerElementMetadataUri(this.metadataDocumentUri, entitySetName, typecast, navigationPropertyName, /*appendItemSelector*/ false);
                if (this.writingResponse && metadataUri == null)
                {
                    throw new ODataException(OData.Strings.ODataJsonLightMetadataUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinksResponse);
                }

                return metadataUri != null;
            }

            /// <summary>
            /// Creates the metadata URI for an operation (function, action, service op) based on its function import.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
            /// <param name="itemTypeReference">The item type of the collection.</param>
            /// <param name="metadataUri">Returns the metadata URI for an operation (function, action, service op) based on its function import.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildCollectionMetadataUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                string collectionTypeName = null;
                if (serializationInfo != null)
                {
                    collectionTypeName = serializationInfo.CollectionTypeName;
                }
                else if (itemTypeReference != null)
                {
                    collectionTypeName = EdmLibraryExtensions.GetCollectionTypeName(itemTypeReference.ODataFullName());
                }

                metadataUri = CreateTypeMetadataUri(this.metadataDocumentUri, collectionTypeName);
                if (this.writingResponse && metadataUri == null)
                {
                    throw new ODataException(OData.Strings.ODataJsonLightMetadataUriBuilder_TypeNameMissingForTopLevelCollectionWhenWritingResponsePayload);
                }

                return metadataUri != null;
            }

            /// <summary>
            /// Creates the metadata URI for the service document.
            /// </summary>
            /// <param name="metadataUri">Returns the metadata URI for the service document.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildServiceDocumentMetadataUri(out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = this.metadataDocumentUri.BaseUri;
                return true;
            }

            /// <summary>
            /// Gets the metadata URI type name based on the given property.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <returns>The type name for the metadata URI.</returns>
            private static string GetMetadataUriTypeNameForValue(ODataProperty property)
            {
                Debug.Assert(property != null, "property != null");

                ODataValue value = property.ODataValue;
                Debug.Assert(value != null, "value != null");

                // special identifier for null values.
                if (value.IsNullValue)
                {
                    return JsonLightConstants.MetadataUriFragmentNull;
                }

                var typeAnnotation = value.GetAnnotation<SerializationTypeNameAnnotation>();
                if (typeAnnotation != null && !string.IsNullOrEmpty(typeAnnotation.TypeName))
                {
                    return typeAnnotation.TypeName;
                }

                var complexValue = value as ODataComplexValue;
                if (complexValue != null)
                {
                    return complexValue.TypeName;
                }

                var collectionValue = value as ODataCollectionValue;
                if (collectionValue != null)
                {
                    return collectionValue.TypeName;
                }

                ODataPrimitiveValue primitive = value as ODataPrimitiveValue;
                if (primitive == null)
                {
                    Debug.Assert(value is ODataStreamReferenceValue, "value is ODataStreamReferenceValue");
                    throw new ODataException(OData.Strings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(property.Name));
                }

                return EdmLibraryExtensions.GetPrimitiveTypeReference(primitive.Value.GetType()).ODataFullName();
            }

            /// <summary>
            /// Gets the entity set name for the metadata Uri.
            /// </summary>
            /// <param name="entitySet">The entity set in question.</param>
            /// <param name="edmModel">The model instance.</param>
            /// <returns>Returns the entity set name for the metadata Uri.</returns>
            private static string GetEntitySetName(IEdmEntitySet entitySet, IEdmModel edmModel)
            {
                Debug.Assert(edmModel != null, "edmModel != null");
                if (entitySet == null)
                {
                    return null;
                }

                string entitySetName;
                IEdmEntityContainer container = entitySet.Container;
                if (edmModel.IsDefaultEntityContainer(container))
                {
                    entitySetName = entitySet.Name;
                }
                else
                {
                    entitySetName = container.Namespace + "." + container.Name + "." + entitySet.Name;
                }

                return entitySetName;
            }

            /// <summary>
            /// Returns the fully qualified name of <paramref name="entityType"/> if it is a derived type of the <paramref name="entitySet"/>;
            /// returns null if <paramref name="entityType"/> is the root type of <paramref name="entitySet"/>.
            /// </summary>
            /// <param name="entitySet">The entity set in question.</param>
            /// <param name="entityType">The eneity type in question.</param>
            /// <returns>
            /// Returns the fully qualified name of <paramref name="entityType"/> if it is a derived type of the <paramref name="entitySet"/>;
            /// returns null if <paramref name="entityType"/> is the root type of <paramref name="entitySet"/>.
            /// </returns>
            private static string GetTypecast(IEdmEntitySet entitySet, IEdmEntityType entityType)
            {
                DebugUtils.CheckNoExternalCallers();
                if (entitySet == null || entityType == null)
                {
                    return null;
                }

                // The client type resolver is not necessary for writes, ResolveEntitySetElementType() will return entitySet.ElementType when model is null
                // and we don't need to look up the type in the model.
                IEdmEntityType entitySetElementType = EdmTypeWriterResolver.Instance.GetElementType(entitySet);

                if (entitySetElementType.IsEquivalentTo(entityType))
                {
                    return null;
                }

                if (!entitySetElementType.IsAssignableFrom(entityType))
                {
                    throw new ODataException(OData.Strings.ODataJsonLightMetadataUriBuilder_ValidateDerivedType(entitySetElementType.FullName(), entityType.FullName()));
                }

                return entityType.ODataFullName();
            }

            /// <summary>
            /// Creates the metadata URI for a type.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="fullTypeName">The fully qualified type name to create the metadata URI for.</param>
            /// <returns>Returns the metadata URI for a value based on its type.</returns>
            private static Uri CreateTypeMetadataUri(ODataMetadataDocumentUri metadataDocumentUri, string fullTypeName)
            {
                DebugUtils.CheckNoExternalCallers();
                return fullTypeName == null ? null : new Uri(metadataDocumentUri.BaseUri, JsonLightConstants.MetadataUriFragmentIndicator + fullTypeName);
            }

            /// <summary>
            /// Creates the metadata URI for a feed or entry.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="model">The Edm model instance.</param>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry or feed.</param>
            /// <param name="isEntry">true if the metadata URI is built for an entry, false if the metadata URI is built for a feed.</param>
            /// <param name="writingResponse">true if the metadata URI is for a response payload, false if the metadata URI is for a request payload.</param>
            /// <returns>Returns the metadata URI for the feed or entry.</returns>
            private static Uri CreateFeedOrEntryMetadataUri(
                ODataMetadataDocumentUri metadataDocumentUri,
                IEdmModel model,
                ODataFeedAndEntryTypeContext typeContext,
                bool isEntry,
                bool writingResponse)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
                Debug.Assert(model != null, "model != null");
                Debug.Assert(typeContext != null, "typeContext != null");
                
                string typecast = typeContext.EntitySetElementTypeName == typeContext.ExpectedEntityTypeName ? null : typeContext.ExpectedEntityTypeName;
                Uri metadataUri = CreateEntityContainerElementMetadataUri(metadataDocumentUri, typeContext.EntitySetName, typecast, /*navigationPropertyName*/ null, /*appendItemSelector*/ isEntry);
                Debug.Assert(metadataUri != null || !writingResponse, "metadataUri != null || !writingResponse -- metadataUri cannot be null when writing a response payload.");
                return metadataUri;
            }

            /// <summary>
            /// Creates the metadata URI for an entity set.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="entitySetName">The fully qualified entity set name for which to create the metadata URI.</param>
            /// <param name="typecast">The fully qualified entity type name of the entries in the result. This has to be an entity type derived
            ///   from the result entity set's base type or null to use its base type.</param>
            /// <param name="navigationPropertyName">Navigation Property name to create a $link metadata uri to, if not null a $link uri will be created</param>
            /// <param name="appendItemSelector">true to append the '@Element" item selector at the end of the metadata URI; otherwise false.</param>
            /// <returns>The metadata URI for the <paramref name="entitySetName"/>.</returns>
            private static Uri CreateEntityContainerElementMetadataUri(
                ODataMetadataDocumentUri metadataDocumentUri,
                string entitySetName,
                string typecast,
                string navigationPropertyName,
                bool appendItemSelector)
            {
                Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
                if (entitySetName == null)
                {
                    return null;
                }

                StringBuilder builder = new StringBuilder();

                // #
                builder.Append(JsonLightConstants.MetadataUriFragmentIndicator);

                // #ContainerName.EntitySetName
                builder.Append(entitySetName);

                if (typecast != null)
                {
                    // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName/Namespace.DerivedTypeName
                    builder.Append(JsonLightConstants.MetadataUriFragmentPartSeparator);
                    builder.Append(typecast);
                }

                if (navigationPropertyName != null)
                {
                    // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName/$link/NavigationPropertyName
                    builder.Append(JsonLightConstants.MetadataUriFragmentPartSeparator);
                    builder.Append(ODataConstants.AssociationLinkSegmentName);
                    builder.Append(JsonLightConstants.MetadataUriFragmentPartSeparator);
                    builder.Append(navigationPropertyName);
                }

                if (appendItemSelector)
                {
                    // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName/@Element
                    builder.Append(JsonLightConstants.MetadataUriFragmentPartSeparator);
                    builder.Append(JsonLightConstants.MetadataUriFragmentItemSelector);
                }

                string selectClause = metadataDocumentUri.SelectClause;
                if (selectClause != null)
                {
                    // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName&$select=selectedPropertyList
                    builder.Append(JsonLightConstants.MetadataUriQueryOptionSeparator);
                    builder.Append(JsonLightConstants.MetadataUriSelectQueryOptionName);
                    builder.Append(JsonLightConstants.MetadataUriQueryOptionValueSeparator);
                    builder.Append(selectClause);
                }

                return new Uri(metadataDocumentUri.BaseUri, builder.ToString());
            }
        }

        /// <summary>
        /// Metadata uri builder which never actually builds anything. Used for the case where 'nometadata' is explicitly requested in the media type.
        /// </summary>
        private sealed class NullMetadataUriBuilder : ODataJsonLightMetadataUriBuilder
        {
            /// <summary>
            /// Singleton instance of <see cref="NullMetadataUriBuilder"/>.
            /// </summary>
            internal static readonly NullMetadataUriBuilder Instance = new NullMetadataUriBuilder();

            /// <summary>
            /// Prevents a default instance of the <see cref="NullMetadataUriBuilder"/> class from being created.
            /// </summary>
            private NullMetadataUriBuilder()
            {
                DebugUtils.CheckNoExternalCallers();
            }

            /// <summary>
            /// Gets the base URI of the metadata document uri. May be null to indicate that there is no metadata document uri.
            /// </summary>
            internal override Uri BaseUri
            {
                get
                {
                    DebugUtils.CheckNoExternalCallers();
                    return null;
                }
            }

            /// <summary>
            /// Creates the metadata URI for a feed based on the entity set the entries in the feed belong to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
            /// <param name="metadataUri">Returns the metadata URI for a feed based on the entity set the entries in the feed belong to.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildFeedMetadataUri(ODataFeedAndEntryTypeContext typeContext, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }

            /// <summary>
            /// Creates the metadata URI for an entry based on the entity set it belongs to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
            /// <param name="metadataUri">Returns the metadata URI for an entry based on the entity set it belongs to.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildEntryMetadataUri(ODataFeedAndEntryTypeContext typeContext, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }

            /// <summary>
            /// Creates the metadata URI for a property based on its value.
            /// </summary>
            /// <param name="property">The property to create the metadata URI for.</param>
            /// <param name="metadataUri">Returns the metadata URI for a property based on its owning type.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildMetadataUriForValue(ODataProperty property, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }

            /// <summary>
            /// Creates the metadata URI for an entity reference link.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the metadata URI for.</param>
            /// <param name="metadataUri">Returns the metadata URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinkMetadataUri(ODataEntityReferenceLinkSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }

            /// <summary>
            /// Creates the metadata URI for a collection of entity reference links.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the metadata URI for.</param>
            /// <param name="metadataUri">Returns the metadata URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinksMetadataUri(ODataEntityReferenceLinksSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }

            /// <summary>
            /// Creates the metadata URI for an operation (function, action, service op) based on its function import.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the metadata uri.</param>
            /// <param name="itemTypeReference">The item type of the collection.</param>
            /// <param name="metadataUri">Returns the metadata URI for an operation (function, action, service op) based on its function import.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildCollectionMetadataUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference, out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }

            /// <summary>
            /// Creates the metadata URI for the service document.
            /// </summary>
            /// <param name="metadataUri">Returns the metadata URI for the service document.</param>
            /// <returns>true if we have successfully built the metadata URI; false otherwise.</returns>
            internal override bool TryBuildServiceDocumentMetadataUri(out Uri metadataUri)
            {
                DebugUtils.CheckNoExternalCallers();
                metadataUri = null;
                return false;
            }
        }
    }
}
