//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Builder class to construct the metadata pointers for the Json Light format for the various payload kinds.
    /// </summary>
    internal abstract class ODataJsonLightContextUriBuilder
    {
        /// <summary>
        /// Gets the base URI of the metadata document uri. May be null to indicate that there is no metadata document uri.
        /// </summary>
        internal abstract Uri BaseUri { get; }

        /// <summary>
        /// Creates a context uri builder after validating user input.
        /// </summary>
        /// <param name="metadataLevel">The JSON Light metadata level being written.</param>
        /// <param name="writingResponse">if set to <c>true</c> indicates that a response is being written.</param>
        /// <param name="writerSettings">The writer settings.</param>
        /// <param name="model">The Edm model instance.</param>
        /// <returns>The context uri builder to use while writing.</returns>
        internal static ODataJsonLightContextUriBuilder CreateFromSettings(JsonLightMetadataLevel metadataLevel, bool writingResponse, ODataMessageWriterSettings writerSettings, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writerSettings != null, "writerSettings != null");
            Debug.Assert(metadataLevel != null, "metadataLevel != null");
            Debug.Assert(model != null, "model != null");

            // if the content type explictly specifies no OData metadata, then the metadata URL should not
            // be written regardless of whether it was provided.
            if (!metadataLevel.ShouldWriteODataContextUri())
            {
                return NullContextUriBuilder.Instance;
            }

            // for other metadata levels, responses require a metadata document URL to be provided.
            ODataMetadataDocumentUri metadataDocumentUri = writerSettings.MetadataDocumentUri;
            if (metadataDocumentUri == null)
            {
                if (writingResponse)
                {
                    throw new ODataException(OData.Core.Strings.ODataJsonLightOutputContext_MetadataDocumentUriMissing);
                }

                return NullContextUriBuilder.Instance;
            }

            return CreateDirectlyFromUri(metadataDocumentUri, model, writingResponse);
        }

        /// <summary>
        /// Creates a context uri builder for the given base metadata document uri.
        /// DEVNOTE: specifically for unit testing.
        /// </summary>
        /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
        /// <param name="model">The Edm model instance.</param>
        /// <param name="writingResponse">if set to <c>true</c> indicates that a response is being written.</param>
        /// <returns>A new context uri builder.</returns>
        internal static ODataJsonLightContextUriBuilder CreateDirectlyFromUri(ODataMetadataDocumentUri metadataDocumentUri, IEdmModel model, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadataDocumentUri != null, "metadataDocumentUri != null");
            return new DefaultContextUriBuilder(metadataDocumentUri, model, writingResponse);
        }

        /// <summary>
        /// Creates the context URI for a feed based on the entity set the entries in the feed belong to.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
        /// <param name="contextUri">Returns the context URI for a feed based on the entity set the entries in the feed belong to.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildFeedContextUri(ODataFeedAndEntryTypeContext typeContext, out Uri contextUri);

        /// <summary>
        /// Creates the context URI for an entry based on the entity set it belongs to.
        /// </summary>
        /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
        /// <param name="contextUri">Returns the context URI for an entry based on the entity set it belongs to.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildEntryContextUri(ODataFeedAndEntryTypeContext typeContext, out Uri contextUri);

        /// <summary>
        /// Creates the context URI for a property based on its value.
        /// </summary>
        /// <param name="property">The property to create the context URI for.</param>
        /// <param name="contextUri">Returns the context URI for a property based on its owning type.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildContextUriForValue(ODataProperty property, out Uri contextUri);

        /// <summary>
        /// Creates the context URI for an entity reference link.
        /// </summary>
        /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
        /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
        /// <param name="navigationProperty">The navigation property to create the context URI for.</param>
        /// <param name="contextUri">Returns the context URI for an entity reference link or a collection of entity reference links.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildEntityReferenceLinkContextUri(ODataEntityReferenceLinkSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri contextUri);

        /// <summary>
        /// Creates the context URI for a collection of entity reference links.
        /// </summary>
        /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
        /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
        /// <param name="navigationProperty">The navigation property to create the context URI for.</param>
        /// <param name="contextUri">Returns the context URI for an entity reference link or a collection of entity reference links.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildEntityReferenceLinksContextUri(ODataEntityReferenceLinksSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri contextUri);

        /// <summary>
        /// Creates the context URI for an operation (function, action, service op) based on its operation import.
        /// </summary>
        /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
        /// <param name="itemTypeReference">The item type of the collection.</param>
        /// <param name="contextUri">Returns the context URI for an operation (function, action, service op) based on its operation import.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildCollectionContextUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference, out Uri contextUri);

        /// <summary>
        /// Creates the context URI for the service document.
        /// </summary>
        /// <param name="contextUri">Returns the context URI for the service document.</param>
        /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
        internal abstract bool TryBuildServiceDocumentContextUri(out Uri contextUri);

        /// <summary>
        /// context URI builder which uses a user-provided uri and $select clause.
        /// </summary>
        private sealed class DefaultContextUriBuilder : ODataJsonLightContextUriBuilder
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
            /// Initializes a new instance of the <see cref="DefaultContextUriBuilder"/> class.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="model">The Edm model instance.</param>
            /// <param name="writingResponse">if set to <c>true</c> indicates that a response is being written.</param>
            internal DefaultContextUriBuilder(ODataMetadataDocumentUri metadataDocumentUri, IEdmModel model, bool writingResponse)
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
            /// Creates the context URI for a feed based on the entity set the entries in the feed belong to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
            /// <param name="contextUri">Returns the context URI for a feed based on the entity set the entries in the feed belong to.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildFeedContextUri(ODataFeedAndEntryTypeContext typeContext, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = CreateFeedOrEntryContextUri(this.metadataDocumentUri, this.model, typeContext, /*isEntry*/ false, this.writingResponse);
                return contextUri != null;
            }

            /// <summary>
            /// Creates the context URI for an entry based on the entity set it belongs to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
            /// <param name="contextUri">Returns the context URI for an entry based on the entity set it belongs to.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildEntryContextUri(ODataFeedAndEntryTypeContext typeContext, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = CreateFeedOrEntryContextUri(this.metadataDocumentUri, this.model, typeContext, /*isEntry*/ true, this.writingResponse);
                return contextUri != null;
            }

            /// <summary>
            /// Creates the context URI for a property based on its value.
            /// </summary>
            /// <param name="property">The property to create the context URI for.</param>
            /// <param name="contextUri">Returns the context URI for a property based on its owning type.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildContextUriForValue(ODataProperty property, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                Debug.Assert(property != null, "property != null");

                string typeName = GetContextUriTypeNameForValue(property);
                if (string.IsNullOrEmpty(typeName))
                {
                    throw new ODataException(OData.Core.Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
                }

                contextUri = CreateTypeContextUri(this.metadataDocumentUri, typeName);
                return contextUri != null;
            }

            /// <summary>
            /// Creates the context URI for an entity reference link.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the context URI for.</param>
            /// <param name="contextUri">Returns the context URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinkContextUri(ODataEntityReferenceLinkSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri contextUri)
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

                contextUri = navigationPropertyName == null ? null : CreateEntityContainerElementContextUri(this.metadataDocumentUri, entitySetName, typecast, navigationPropertyName, /*appendItemSelector*/isCollectionNavigationProperty);
                if (this.writingResponse && contextUri == null)
                {
                    throw new ODataException(OData.Core.Strings.ODataJsonLightContextUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinkResponse);
                }

                return contextUri != null;
            }

            /// <summary>
            /// Creates the context URI for a collection of entity reference links.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the context URI for.</param>
            /// <param name="contextUri">Returns the context URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinksContextUri(ODataEntityReferenceLinksSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri contextUri)
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

                contextUri = navigationPropertyName == null ? null : CreateEntityContainerElementContextUri(this.metadataDocumentUri, entitySetName, typecast, navigationPropertyName, /*appendItemSelector*/ true);
                if (this.writingResponse && contextUri == null)
                {
                    throw new ODataException(OData.Core.Strings.ODataJsonLightContextUriBuilder_EntitySetOrNavigationPropertyMissingForTopLevelEntityReferenceLinksResponse);
                }

                return contextUri != null;
            }

            /// <summary>
            /// Creates the context URI for an operation (function, action, service op) based on its operation import.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
            /// <param name="itemTypeReference">The item type of the collection.</param>
            /// <param name="contextUri">Returns the context URI for an operation (function, action, service op) based on its operation import.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildCollectionContextUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference, out Uri contextUri)
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

                contextUri = CreateTypeContextUri(this.metadataDocumentUri, collectionTypeName);
                if (this.writingResponse && contextUri == null)
                {
                    throw new ODataException(OData.Core.Strings.ODataJsonLightContextUriBuilder_TypeNameMissingForTopLevelCollectionWhenWritingResponsePayload);
                }

                return contextUri != null;
            }

            /// <summary>
            /// Creates the context URI for the service document.
            /// </summary>
            /// <param name="contextUri">Returns the context URI for the service document.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildServiceDocumentContextUri(out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = this.metadataDocumentUri.BaseUri;
                return true;
            }

            /// <summary>
            /// Gets the context URI type name based on the given property.
            /// </summary>
            /// <param name="property">The property.</param>
            /// <returns>The type name for the context URI.</returns>
            private static string GetContextUriTypeNameForValue(ODataProperty property)
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
                    throw new ODataException(OData.Core.Strings.ODataWriter_StreamPropertiesMustBePropertiesOfODataEntry(property.Name));
                }

                return EdmLibraryExtensions.GetPrimitiveTypeReference(primitive.Value.GetType()).ODataFullName();
            }

            /// <summary>
            /// Gets the entity set name for the context Uri.
            /// </summary>
            /// <param name="entitySet">The entity set in question.</param>
            /// <param name="edmModel">The model instance.</param>
            /// <returns>Returns the entity set name for the context Uri.</returns>
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
                    throw new ODataException(OData.Core.Strings.ODataJsonLightContextUriBuilder_ValidateDerivedType(entitySetElementType.FullName(), entityType.FullName()));
                }

                return entityType.ODataFullName();
            }

            /// <summary>
            /// Creates the context URI for a type.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="fullTypeName">The fully qualified type name to create the context URI for.</param>
            /// <returns>Returns the context URI for a value based on its type.</returns>
            private static Uri CreateTypeContextUri(ODataMetadataDocumentUri metadataDocumentUri, string fullTypeName)
            {
                DebugUtils.CheckNoExternalCallers();
                return fullTypeName == null ? null : new Uri(metadataDocumentUri.BaseUri, JsonLightConstants.ContextUriFragmentIndicator + fullTypeName);
            }

            /// <summary>
            /// Creates the context URI for a feed or entry.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="model">The Edm model instance.</param>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry or feed.</param>
            /// <param name="isEntry">true if the context URI is built for an entry, false if the context URI is built for a feed.</param>
            /// <param name="writingResponse">true if the context URI is for a response payload, false if the context URI is for a request payload.</param>
            /// <returns>Returns the context URI for the feed or entry.</returns>
            private static Uri CreateFeedOrEntryContextUri(
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
                Uri contextUri = CreateEntityContainerElementContextUri(metadataDocumentUri, typeContext.EntitySetName, typecast, /*navigationPropertyName*/ null, /*appendItemSelector*/ isEntry);
                Debug.Assert(contextUri != null || !writingResponse, "contextUri != null || !writingResponse -- contextUri cannot be null when writing a response payload.");
                return contextUri;
            }

            /// <summary>
            /// Creates the context URI for an entity set.
            /// </summary>
            /// <param name="metadataDocumentUri">The non-null, absolute metadata document URI.</param>
            /// <param name="entitySetName">The fully qualified entity set name for which to create the context URI.</param>
            /// <param name="typecast">The fully qualified entity type name of the entries in the result. This has to be an entity type derived
            ///   from the result entity set's base type or null to use its base type.</param>
            /// <param name="navigationPropertyName">Navigation Property name to create a $link context URI to, if not null a $link uri will be created</param>
            /// <param name="appendItemSelector">true to append the '@Element" item selector at the end of the context URI; otherwise false.</param>
            /// <returns>The context URI for the <paramref name="entitySetName"/>.</returns>
            private static Uri CreateEntityContainerElementContextUri(
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

                // If a response is a entity references, the context URL does not contain the type of the referenced entities.
                if (navigationPropertyName != null)
                {
                    if (appendItemSelector)
                    {
                        // #Collection($ref)
                        builder.Append(JsonLightConstants.CollectionOfEntityReferencesContextUrlSegment);
                    }
                    else
                    {
                        // #$ref
                        builder.Append(JsonLightConstants.SingleEntityReferencesContextUrlSegment);
                    }
                }
                else
                {
                    // #
                    builder.Append(JsonLightConstants.ContextUriFragmentIndicator);

                    // #ContainerName.EntitySetName
                    builder.Append(entitySetName);

                    if (typecast != null)
                    {
                        // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName/Namespace.DerivedTypeName
                        builder.Append(JsonLightConstants.ContextUriFragmentPartSeparator);
                        builder.Append(typecast);
                    }

                    if (appendItemSelector)
                    {
                        // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName/@Element
                        builder.Append(JsonLightConstants.ContextUriFragmentPartSeparator);
                        builder.Append(JsonLightConstants.ContextUriFragmentItemSelector);
                    }

                    string selectClause = metadataDocumentUri.SelectClause;
                    if (selectClause != null)
                    {
                        // #ContainerName.EntitySetName  ==>  #ContainerName.EntitySetName&$select=selectedPropertyList
                        builder.Append(JsonLightConstants.ContextUriQueryOptionSeparator);
                        builder.Append(JsonLightConstants.ContextUriSelectQueryOptionName);
                        builder.Append(JsonLightConstants.ContextUriQueryOptionValueSeparator);
                        builder.Append(selectClause);
                    }
                }

                return new Uri(metadataDocumentUri.BaseUri, builder.ToString());
            }
        }

        /// <summary>
        /// Context uri builder which never actually builds anything. Used for the case where 'none' is explicitly requested in the media type.
        /// </summary>
        private sealed class NullContextUriBuilder : ODataJsonLightContextUriBuilder
        {
            /// <summary>
            /// Singleton instance of <see cref="NullContextUriBuilder"/>.
            /// </summary>
            internal static readonly NullContextUriBuilder Instance = new NullContextUriBuilder();

            /// <summary>
            /// Prevents a default instance of the <see cref="NullContextUriBuilder"/> class from being created.
            /// </summary>
            private NullContextUriBuilder()
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
            /// Creates the context URI for a feed based on the entity set the entries in the feed belong to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the feed.</param>
            /// <param name="contextUri">Returns the context URI for a feed based on the entity set the entries in the feed belong to.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildFeedContextUri(ODataFeedAndEntryTypeContext typeContext, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }

            /// <summary>
            /// Creates the context URI for an entry based on the entity set it belongs to.
            /// </summary>
            /// <param name="typeContext">The context object to answer basic questions regarding the type of the entry.</param>
            /// <param name="contextUri">Returns the context URI for an entry based on the entity set it belongs to.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildEntryContextUri(ODataFeedAndEntryTypeContext typeContext, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }

            /// <summary>
            /// Creates the context URI for a property based on its value.
            /// </summary>
            /// <param name="property">The property to create the context URI for.</param>
            /// <param name="contextUri">Returns the context URI for a property based on its owning type.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildContextUriForValue(ODataProperty property, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }

            /// <summary>
            /// Creates the context URI for an entity reference link.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the context URI for.</param>
            /// <param name="contextUri">Returns the context URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinkContextUri(ODataEntityReferenceLinkSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }

            /// <summary>
            /// Creates the context URI for a collection of entity reference links.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
            /// <param name="entitySet">The entity set of the declaring type of the navigation property</param>
            /// <param name="navigationProperty">The navigation property to create the context URI for.</param>
            /// <param name="contextUri">Returns the context URI for an entity reference link or a collection of entity reference links.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildEntityReferenceLinksContextUri(ODataEntityReferenceLinksSerializationInfo serializationInfo, IEdmEntitySet entitySet, IEdmNavigationProperty navigationProperty, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }

            /// <summary>
            /// Creates the context URI for an operation (function, action, service op) based on its operation import.
            /// </summary>
            /// <param name="serializationInfo">Serialization information to generate the context URI.</param>
            /// <param name="itemTypeReference">The item type of the collection.</param>
            /// <param name="contextUri">Returns the context URI for an operation (function, action, service op) based on its operation import.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildCollectionContextUri(ODataCollectionStartSerializationInfo serializationInfo, IEdmTypeReference itemTypeReference, out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }

            /// <summary>
            /// Creates the context URI for the service document.
            /// </summary>
            /// <param name="contextUri">Returns the context URI for the service document.</param>
            /// <returns>true if we have successfully built the context URI; false otherwise.</returns>
            internal override bool TryBuildServiceDocumentContextUri(out Uri contextUri)
            {
                DebugUtils.CheckNoExternalCallers();
                contextUri = null;
                return false;
            }
        }
    }
}
