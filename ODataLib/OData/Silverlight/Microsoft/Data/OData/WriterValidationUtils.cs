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
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for validating OData content when writing.
    /// </summary>
    internal static class WriterValidationUtils
    {
        /// <summary>
        /// Validates that message writer settings are correct.
        /// </summary>
        /// <param name="messageWriterSettings">The message writer settings to validate.</param>
        internal static void ValidateMessageWriterSettings(ODataMessageWriterSettings messageWriterSettings)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(messageWriterSettings != null, "messageWriterSettings != null");

            if (messageWriterSettings.BaseUri != null && !messageWriterSettings.BaseUri.IsAbsoluteUri)
            {
                throw new ODataException(Strings.WriterValidationUtils_MessageWriterSettingsBaseUriMustBeNullOrAbsolute(UriUtilsCommon.UriToString(messageWriterSettings.BaseUri)));
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataProperty"/> for not being null.
        /// </summary>
        /// <param name="property">The property to validate for not being null.</param>
        internal static void ValidatePropertyNotNull(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();

            if (property == null)
            {
                throw new ODataException(Strings.WriterValidationUtils_PropertyMustNotBeNull);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataProperty"/> to ensure all required information is specified.
        /// </summary>
        /// <param name="property">The property to validate (must not be null, call ValidatePropertyNotNull before).</param>
        internal static void ValidateProperty(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(property != null, "property != null");

            // Properties must have a non-empty name
            string propertyName = property.Name;
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ODataException(Strings.WriterValidationUtils_PropertiesMustHaveNonEmptyName);
            }

            ValidationUtils.ValidatePropertyName(propertyName);
        }

        /// <summary>
        /// Validates a type name to ensure that it's not an empty string.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <param name="typeKind">The expected type kind for the given type name.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>The type with the given name and kind if the model is a user model, otherwise null.</returns>
        internal static IEdmType ValidateValueTypeName(IEdmModel model, string typeName, EdmTypeKind typeKind, bool isOpenPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(typeKind != EdmTypeKind.Entity, "This method is only for value types.");

            if (typeName == null)
            {
                // if we have metadata the type name of an entry or a complex value of an open property must not be null
                if (model.IsUserModel() && isOpenPropertyType)
                {
                    throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
                }

                return null;
            }

            return ValidationUtils.ValidateValueTypeName(model, typeName, typeKind);
        }

        /// <summary>
        /// Validates an entity type name to ensure that it's not an empty string.
        /// </summary>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <returns>The entity type with the given name and kind if the metadata was available, otherwise null.</returns>
        internal static IEdmEntityType ValidateEntityTypeName(IEdmModel model, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeName == null)
            {
                // if we have metadata the type name of an entry or a complex value of an open property must not be null
                if (model.IsUserModel())
                {
                    throw new ODataException(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
                }

                return null;
            }

            return ValidationUtils.ValidateEntityTypeName(model, typeName);
        }

        /// <summary>
        /// Validates that a property with the specified name exists on a given structured type.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningStructuredType">The owning type of the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</param>
        /// <returns>The <see cref="IEdmProperty"/> instance representing the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</returns>
        internal static IEdmProperty ValidatePropertyDefined(string propertyName, IEdmStructuredType owningStructuredType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (owningStructuredType == null)
            {
                return null;
            }

            IEdmProperty property = owningStructuredType.FindProperty(propertyName);

            // verify that the property is declared if the type is not an open type.
            if (!owningStructuredType.IsOpen && property == null)
            {
                throw new ODataException(Strings.ValidationUtils_PropertyDoesNotExistOnType(propertyName, owningStructuredType.ODataFullName()));
            }

            return property;
        }

        /// <summary>
        /// Validates that a navigation property with the specified name exists on a given entity type.
        /// The entity type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningEntityType">The owning entity type or null if no metadata is available.</param>
        /// <returns>The <see cref="IEdmProperty"/> instance representing the navigation property with name <paramref name="propertyName"/>
        /// or null if no metadata is available.</returns>
        internal static IEdmProperty ValidateNavigationPropertyDefined(string propertyName, IEdmEntityType owningEntityType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (owningEntityType == null)
            {
                return null;
            }

            IEdmProperty property = ValidatePropertyDefined(propertyName, owningEntityType);
            if (property == null)
            {
                // We don't support open navigation properties
                Debug.Assert(owningEntityType.IsOpen, "We should have already failed on non-existing property on a closed type.");
                throw new ODataException(Strings.ValidationUtils_OpenNavigationProperty(propertyName, owningEntityType.ODataFullName()));
            }

            if (property.PropertyKind != EdmPropertyKind.Navigation)
            {
                // The property must be a navigation property
                throw new ODataException(Strings.ValidationUtils_NavigationPropertyExpected(propertyName, owningEntityType.ODataFullName(), property.PropertyKind.ToString()));
            }

            return property;
        }

        /// <summary>
        /// Validates an entry in an expanded link to make sure the entity types match.
        /// </summary>
        /// <param name="entryEntityType">The <see cref="IEdmEntityType"/> of the entry.</param>
        /// <param name="parentNavigationPropertyType">The type of the parent navigation property.</param>
        internal static void ValidateEntryInExpandedLink(IEdmEntityType entryEntityType, IEdmType parentNavigationPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();

            if (parentNavigationPropertyType == null)
            {
                return;
            }

            Debug.Assert(entryEntityType != null, "If we have a parent navigation property type we should also have an entry type.");

            bool navPropIsCollection = parentNavigationPropertyType.TypeKind == EdmTypeKind.Collection;
            IEdmEntityType parentNavigationPropertyEntityType = (IEdmEntityType)(navPropIsCollection
                ? ((IEdmCollectionType)parentNavigationPropertyType).ElementType.Definition
                : parentNavigationPropertyType);

            // Make sure the entity types are compatible
            if (!parentNavigationPropertyEntityType.IsAssignableFrom(entryEntityType))
            {
                throw new ODataException(Strings.WriterValidationUtils_EntryTypeInExpandedLinkNotCompatibleWithNavigationPropertyType(entryEntityType.ODataFullName(), parentNavigationPropertyEntityType.ODataFullName()));
            }
        }

        /// <summary>
        /// Resolve a type name against the provided <paramref name="model"/>. If no type name is given we either throw (if a type name on the value is required, e.g., on entries)
        /// or infer the type from the model (if it is a user model).
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeReferenceFromMetadata">The type inferred from the model or null if the model is not a user model.</param>
        /// <param name="typeName">The type name to be resolved.</param>
        /// <param name="typeKindFromValue">The expected type kind of the resolved type.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>A type for the <paramref name="typeName"/> or null if no metadata is available.</returns>
        internal static IEdmTypeReference ResolveTypeNameForWriting(IEdmModel model, IEdmTypeReference typeReferenceFromMetadata, ref string typeName, EdmTypeKind typeKindFromValue, bool isOpenPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(
                typeKindFromValue == EdmTypeKind.Complex || typeKindFromValue == EdmTypeKind.Collection,
                "Only complex or collection types are supported by this method. This method assumes that the types in question don't support inheritance.");

            IEdmType typeFromValue = WriterValidationUtils.ValidateValueTypeName(model, typeName, typeKindFromValue, isOpenPropertyType);
            IEdmTypeReference typeReferenceFromValue = typeFromValue.ToTypeReference();

            if (typeReferenceFromMetadata != null)
            {
                ValidationUtils.ValidateTypeKind(
                    typeKindFromValue, 
                    typeReferenceFromMetadata.TypeKind(), 
                    typeFromValue == null ? null : typeFromValue.ODataFullName());
            }

            typeReferenceFromValue = ValidateMetadataType(typeReferenceFromMetadata, typeReferenceFromValue);

            if (typeKindFromValue == EdmTypeKind.Collection && typeReferenceFromValue != null)
            {
                // validate that the collection type represents a valid Collection type (e.g., is unordered).
                typeReferenceFromValue = ValidationUtils.ValidateCollectionType(typeReferenceFromValue);
            }

            // derive the type name from the metadata if available
            if (typeName == null && typeReferenceFromValue != null)
            {
                typeName = typeReferenceFromValue.ODataFullName();
            }

            return typeReferenceFromValue;
        }

        /// <summary>
        /// Validates an <see cref="ODataAssociationLink"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="associationLink">The association link to validate.</param>
        /// <param name="version">The version of the OData protocol used for checking.</param>
        /// <param name="writingResponse">true if we are writing a response; otherwise false.</param>
        internal static void ValidateAssociationLink(ODataAssociationLink associationLink, ODataVersion version, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(associationLink != null, "associationLink != null");

            ODataVersionChecker.CheckAssociationLinks(version);

            if (!writingResponse)
            {
                throw new ODataException(Strings.WriterValidationUtils_AssociationLinkInRequest(associationLink.Name));
            }

            ValidationUtils.ValidateAssociationLink(associationLink);
        }

        /// <summary>
        /// Validates an <see cref="ODataOperation"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="operation">The operation (an action or a function) to validate.</param>
        /// <param name="writingResponse">true if writing a response; otherwise false.</param>
        internal static void ValidateOperation(ODataOperation operation, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operation != null, "operation != null");

            // Operations are only valid in responses; we fail on them in requests
            if (!writingResponse)
            {
                throw new ODataException(Strings.WriterValidationUtils_OperationInRequest(operation.Metadata));
            }

            ValidationUtils.ValidateOperation(operation);
        }

        /// <summary>
        /// Validates an <see cref="ODataFeed"/> to ensure all required information is specified and valid on the WriteStart call.
        /// </summary>
        /// <param name="feed">The feed to validate.</param>
        internal static void ValidateFeedAtStart(ODataFeed feed)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(feed != null, "feed != null");

            // Verify non-empty ID
            if (string.IsNullOrEmpty(feed.Id))
            {
                throw new ODataException(Strings.WriterValidationUtils_FeedsMustHaveNonEmptyId);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataFeed"/> to ensure all required information is specified and valid on the WriteEnd call.
        /// </summary>
        /// <param name="feed">The feed to validate.</param>
        /// <param name="writingRequest">Flag indicating whether the feed is written as part of a request or a response.</param>
        /// <param name="version">The version of the OData protocol used for checking.</param>
        internal static void ValidateFeedAtEnd(ODataFeed feed, bool writingRequest, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(feed != null, "feed != null");

            // Verify next link
            if (feed.NextPageLink != null)
            {
                // Check that NextPageLink is not set for requests
                if (writingRequest)
                {
                    throw new ODataException(Strings.WriterValidationUtils_NextPageLinkInRequest);
                }

                // Verify version requirements
                ODataVersionChecker.CheckNextLink(version);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataEntry"/> to ensure all required information is specified and valid on WriteStart call.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        internal static void ValidateEntryAtStart(ODataEntry entry)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            // Id can be specified at the beginning (and might be written here), so we need to validate it here.
            ValidateEntryId(entry.Id);

            // Type name is verified in the format readers/writers since it's shared with other non-entity types
            // and verifying it here would mean doing it twice.
        }

        /// <summary>
        /// Validates an <see cref="ODataEntry"/> to ensure all required information is specified and valid on WriteEnd call.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        internal static void ValidateEntryAtEnd(ODataEntry entry)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            // If the Id was not specified in the beginning it might have been specified at the end, so validate it here as well.
            ValidateEntryId(entry.Id);
        }

        /// <summary>
        /// Validates an <see cref="ODataStreamReferenceValue"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="streamReference">The stream reference to validate.</param>
        /// <param name="isDefaultStream">true if <paramref name="streamReference"/> is the default stream for an entity; false if it is a named stream property value.</param>
        internal static void ValidateStreamReferenceValue(ODataStreamReferenceValue streamReference, bool isDefaultStream)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(streamReference != null, "streamReference != null");

            if (streamReference.ContentType != null && streamReference.ContentType.Length == 0)
            {
                throw new ODataException(Strings.WriterValidationUtils_StreamReferenceValueEmptyContentType);
            }

            if (isDefaultStream && streamReference.ReadLink == null && streamReference.ContentType != null)
            {
                throw new ODataException(Strings.WriterValidationUtils_DefaultStreamWithContentTypeWithoutReadLink);
            }

            if (isDefaultStream && streamReference.ReadLink != null && streamReference.ContentType == null)
            {
                throw new ODataException(Strings.WriterValidationUtils_DefaultStreamWithReadLinkWithoutContentType);
            }

            // Default stream can be completely empty (no links or anything)
            // that is used to effectively mark the entry as MLE without providing any MR information.
            // OData clients when creating new MLE/MR might not have the MR information (yet) when sending the first PUT, but they still
            // need to mark the entry as MLE so that properties are written out-of-content. In such scenario the client can just set an empty
            // default stream to mark the entry as MLE.
            // That will cause the ATOM writer to write the properties outside the content without producing any content element.
            if (streamReference.EditLink == null && streamReference.ReadLink == null && !isDefaultStream)
            {
                throw new ODataException(Strings.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkOrReadLink);
            }

            if (streamReference.EditLink == null && streamReference.ETag != null)
            {
                throw new ODataException(Strings.WriterValidationUtils_StreamReferenceValueMustHaveEditLinkToHaveETag);
            }
        }

        /// <summary>
        /// Validates a named stream property to ensure it's not null and it's name if correct.
        /// </summary>
        /// <param name="streamProperty">The stream reference property to validate.</param>
        /// <param name="edmProperty">Property metadata to validate against.</param>
        /// <param name="version">The version of the OData protocol used for checking.</param>
        /// <param name="writingResponse">true when writing a response; otherwise false.</param>
        internal static void ValidateStreamReferenceProperty(ODataProperty streamProperty, IEdmProperty edmProperty, ODataVersion version, bool writingResponse)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(streamProperty != null, "streamProperty != null");

            ODataVersionChecker.CheckStreamReferenceProperty(version);
            ValidationUtils.ValidateStreamReferenceProperty(streamProperty, edmProperty);

            if (!writingResponse)
            {
                throw new ODataException(Strings.WriterValidationUtils_StreamPropertyInRequest(streamProperty.Name));
            }

            ODataStreamReferenceValue streamValue = (ODataStreamReferenceValue)streamProperty.Value;
            ValidateStreamReferenceValue(streamValue, false /*isDefaultStream*/);
        }

        /// <summary>
        /// Validates that the specified <paramref name="entityReferenceLink"/> is not null.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to validate.</param>
        /// <remarks>This should be called only for entity reference links inside the ODataEntityReferenceLinks.Links collection.</remarks>
        internal static void ValidateEntityReferenceLinkNotNull(ODataEntityReferenceLink entityReferenceLink)
        {
            DebugUtils.CheckNoExternalCallers();

            if (entityReferenceLink == null)
            {
                throw new ODataException(Strings.WriterValidationUtils_EntityReferenceLinksLinkMustNotBeNull);
            }
        }

        /// <summary>
        /// Validates an entity reference link instance.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link to validate.</param>
        internal static void ValidateEntityReferenceLink(ODataEntityReferenceLink entityReferenceLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entityReferenceLink != null, "entityReferenceLink != null");

            if (entityReferenceLink.Url == null)
            {
                throw new ODataException(Strings.WriterValidationUtils_EntityReferenceLinkUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataNavigationLink"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="navigationLink">The navigation link to validate.</param>
        /// <param name="declaringEntityType">The <see cref="IEdmEntityType"/> declaring the navigation property; or null if metadata is not available.</param>
        /// <param name="expandedPayloadKind">The <see cref="ODataPayloadKind"/> of the expanded content of this navigation link or null for deferred links.</param>
        /// <returns>The type of the navigation property for this navigation link; or null if no <paramref name="declaringEntityType"/> was specified.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Keeping the validation code for navigation link multiplicity in one place.")]
        internal static IEdmType ValidateNavigationLink(ODataNavigationLink navigationLink, IEdmEntityType declaringEntityType, ODataPayloadKind? expandedPayloadKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");
            Debug.Assert(
                !expandedPayloadKind.HasValue ||
                expandedPayloadKind.Value == ODataPayloadKind.EntityReferenceLink ||
                expandedPayloadKind.Value == ODataPayloadKind.Entry ||
                expandedPayloadKind.Value == ODataPayloadKind.Feed,
                "If an expanded payload kind is specified it must be entry, feed or entity reference link.");

            // Navigation link must have a non-empty name
            if (string.IsNullOrEmpty(navigationLink.Name))
            {
                throw new ODataException(Strings.ValidationUtils_LinkMustSpecifyName);
            }

            // If we write an entity reference link, don't validate the multiplicity of the IsCollection
            // property if it is 'false' (since we allow writing a singleton navigation link for
            // a colleciton navigation property in requests) nor the consistency of payload kind and metadata
            // (which is done separately in ODataWriterCore.CheckForNavigationLinkWithContent).
            bool isEntityReferenceLinkPayload = expandedPayloadKind == ODataPayloadKind.EntityReferenceLink;

            // true only if the expandedPayloadKind has a value and the value is 'Feed'
            bool isFeedPayload = expandedPayloadKind == ODataPayloadKind.Feed;

            // Make sure the IsCollection property agrees with the payload kind for entry and feed payloads
            Func<object, string> errorTemplate = null;
            if (!isEntityReferenceLinkPayload && navigationLink.IsCollection.HasValue && expandedPayloadKind.HasValue)
            {
                // For feed/entry make sure the IsCollection property is set correctly.
                if (isFeedPayload != navigationLink.IsCollection.Value)
                {
                    errorTemplate = expandedPayloadKind.Value == ODataPayloadKind.Feed
                        ? (Func<object, string>)Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithFeedContent
                        : Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithEntryContent;
                }
            }

            IEdmType navigationPropertyType = null;
            if (declaringEntityType != null)
            {
                IEdmProperty navigationProperty = WriterValidationUtils.ValidateNavigationPropertyDefined(navigationLink.Name, declaringEntityType);
                Debug.Assert(navigationProperty != null, "If we have a declaring type we expect a non-null navigation property since open nav props are not allowed.");

                navigationPropertyType = navigationProperty.Type.Definition;
                bool isCollectionType = navigationPropertyType.TypeKind == EdmTypeKind.Collection;

                // Make sure the IsCollection property agrees with the metadata type for entry and feed payloads
                if (navigationLink.IsCollection.HasValue && isCollectionType != navigationLink.IsCollection)
                {
                    // Ignore the case where IsCollection is 'false' and we are writing an entity reference link
                    // (see comment above)
                    if (!(navigationLink.IsCollection == false && isEntityReferenceLinkPayload))
                    {
                        errorTemplate = isCollectionType
                            ? (Func<object, string>)Strings.WriterValidationUtils_ExpandedLinkIsCollectionFalseWithFeedMetadata
                            : Strings.WriterValidationUtils_ExpandedLinkIsCollectionTrueWithEntryMetadata;
                    }
                }

                // Make sure that the payload kind agrees with the metadata.
                // For entity reference links we check separately in ODataWriterCore.CheckForNavigationLinkWithContent.
                if (!isEntityReferenceLinkPayload && expandedPayloadKind.HasValue && isCollectionType != isFeedPayload)
                {
                    errorTemplate = isCollectionType
                        ? (Func<object, string>)Strings.WriterValidationUtils_ExpandedLinkWithEntryPayloadAndFeedMetadata
                        : Strings.WriterValidationUtils_ExpandedLinkWithFeedPayloadAndEntryMetadata;
                }
            }

            if (errorTemplate != null)
            {
                string uri = navigationLink.Url == null ? "null" : UriUtilsCommon.UriToString(navigationLink.Url);
                throw new ODataException(errorTemplate(uri));
            }

            return navigationPropertyType;
        }

        /// <summary>
        /// Validates that the expected property allows null value.
        /// </summary>
        /// <param name="expectedProperty">The metadata for the expected property.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        /// <param name="model">The model to use to get the data service version.</param>
        internal static void ValidateNullPropertyValue(IEdmProperty expectedProperty, ODataWriterBehavior writerBehavior, IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writerBehavior != null, "writerBehavior != null");
            Debug.Assert(model != null, "For null validation, model is required.");

            if (expectedProperty != null)
            {
                IEdmTypeReference propertyTypeReference = expectedProperty.Type;
                if (propertyTypeReference.IsNonEntityODataCollectionTypeKind())
                {
                    throw new ODataException(Strings.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue(expectedProperty.Name));
                }

                if (propertyTypeReference.IsODataPrimitiveTypeKind())
                {
                    // WCF DS allows null values for non-nullable primitive types, so we need to check for a knob which enables this behavior.
                    // See the description of ODataWriterBehavior.AllowNullValuesForNonNullablePrimitiveTypes for more details.
                    if (!propertyTypeReference.IsNullable && !writerBehavior.AllowNullValuesForNonNullablePrimitiveTypes)
                    {
                        throw new ODataException(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(expectedProperty.Name, expectedProperty.Type.ODataFullName()));
                    }
                }
                else if (propertyTypeReference.IsStream())
                {
                    throw new ODataException(Strings.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue(expectedProperty.Name));
                }
                else if (propertyTypeReference.IsODataComplexTypeKind())
                {
                    if (ValidationUtils.ShouldValidateComplexPropertyNullValue(model))
                    {
                        IEdmComplexTypeReference complexTypeReference = propertyTypeReference.AsComplex();
                        if (!complexTypeReference.IsNullable)
                        {
                            throw new ODataException(Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(expectedProperty.Name, expectedProperty.Type.ODataFullName()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates that the (optional) <paramref name="typeReferenceFromMetadata"/> is the same as the (optional) <paramref name="typeReferenceFromValue"/>.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The (optional) type from the metadata definition (the expected type).</param>
        /// <param name="typeReferenceFromValue">The (optional) type from the value (the actual type).</param>
        /// <returns>The type as derived from the <paramref name="typeReferenceFromMetadata"/> and/or <paramref name="typeReferenceFromValue"/>.</returns>
        private static IEdmTypeReference ValidateMetadataType(IEdmTypeReference typeReferenceFromMetadata, IEdmTypeReference typeReferenceFromValue)
        {
            if (typeReferenceFromMetadata == null)
            {
                // if we have no metadata information there is nothing to validate
                return typeReferenceFromValue;
            }

            if (typeReferenceFromValue == null)
            {
                // derive the property type from the metadata
                return typeReferenceFromMetadata;
            }

            // Make sure the kinds are the same
            EdmTypeKind typeKind = typeReferenceFromMetadata.TypeKind();
            ValidationUtils.ValidateTypeKind(typeReferenceFromValue.Definition, typeKind);

            // Make sure the types are the same
            if (typeReferenceFromValue.IsODataPrimitiveTypeKind())
            {
                // Primitive types must match exactly except for nullability
                ValidationUtils.ValidateMetadataPrimitiveType(typeReferenceFromMetadata, typeReferenceFromValue);
            }
            else if (typeKind == EdmTypeKind.Entity)
            {
                ValidationUtils.ValidateEntityTypeIsAssignable((IEdmEntityTypeReference)typeReferenceFromMetadata, (IEdmEntityTypeReference)typeReferenceFromValue);
            }
            else
            {
                // Complex and collection types must match exactly
                if (typeReferenceFromMetadata.ODataFullName() != typeReferenceFromValue.ODataFullName())
                {
                    throw new ODataException(Strings.ValidationUtils_IncompatibleType(typeReferenceFromValue.ODataFullName(), typeReferenceFromMetadata.ODataFullName()));
                }
            }

            return typeReferenceFromValue;
        }

        /// <summary>
        /// Validates the value of the Id property on an entry.
        /// </summary>
        /// <param name="id">The id value for an entry to validate.</param>
        private static void ValidateEntryId(string id)
        {
            // Verify non-empty ID (entries can have no (null) ID for insert scenarios; empty IDs are not allowed)
            if (id != null && id.Length == 0)
            {
                throw new ODataException(Strings.WriterValidationUtils_EntriesMustHaveNonEmptyId);
            }
        }
    }
}
