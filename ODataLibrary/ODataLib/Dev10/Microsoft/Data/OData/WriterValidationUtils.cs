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
    using System.Collections.Generic;
    using System.Diagnostics;
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
            if (string.IsNullOrEmpty(property.Name))
            {
                throw new ODataException(Strings.WriterValidationUtils_PropertiesMustHaveNonEmptyName);
            }
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
        /// Resolve a type name against the provided <paramref name="model"/>. If no type name is given we either throw (if a type name on the value is required, e.g., on entries)
        /// or infer the type from the model (if it is a user model).
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeReferenceFromMetadata">The type inferred from the model or null if the model is not a user model.</param>
        /// <param name="typeName">The type name to be resolved.</param>
        /// <param name="typeKind">The expected type kind of the resolved type.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>A type for the <paramref name="typeName"/> or null if no metadata is available.</returns>
        internal static IEdmTypeReference ResolveTypeNameForWriting(IEdmModel model, IEdmTypeReference typeReferenceFromMetadata, ref string typeName, EdmTypeKind typeKind, bool isOpenPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(
                typeKind == EdmTypeKind.Complex || typeKind == EdmTypeKind.Collection,
                "Only complex or multivalue types are supported by this method. This method assumes that the types in question don't support inheritance.");

            IEdmType typeFromValue = WriterValidationUtils.ValidateValueTypeName(model, typeName, typeKind, isOpenPropertyType);
            IEdmTypeReference typeReferenceFromValue = typeFromValue.ToTypeReference();

            if (typeReferenceFromMetadata != null)
            {
                ValidationUtils.ValidateTypeKind(typeReferenceFromMetadata.Definition, typeKind);
            }

            typeReferenceFromValue = ValidateMetadataType(typeReferenceFromMetadata, typeReferenceFromValue);

            if (typeKind == EdmTypeKind.Collection && typeReferenceFromValue != null)
            {
                // validate that the collection type represents a valid MultiValue type (e.g., is unordered).
                typeReferenceFromValue = ValidationUtils.ValidateMultiValueType(typeReferenceFromValue);
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
        internal static void ValidateAssociationLink(ODataAssociationLink associationLink, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(associationLink != null, "associationLink != null");

            ODataVersionChecker.CheckAssociationLinks(version);

            ValidationUtils.ValidateAssociationLink(associationLink);
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
        /// Validates a service document.
        /// </summary>
        /// <param name="workspace">The workspace to validate.</param>
        /// <returns>The set of collections in the specified <paramref name="workspace"/>.</returns>
        internal static IEnumerable<ODataResourceCollectionInfo> ValidateWorkspace(ODataWorkspace workspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(workspace != null, "workspace != null");

            IEnumerable<ODataResourceCollectionInfo> workspaceCollections = workspace.Collections;
            if (workspaceCollections != null)
            {
                foreach (ODataResourceCollectionInfo collection in workspaceCollections)
                {
                    if (collection == null)
                    {
                        throw new ODataException(Strings.WriterValidationUtils_WorkspaceCollectionsMustNotContainNullItem);
                    }

                    // validate that resource collection Url is not null.
                    ValidationUtils.ValidateResourceCollectionInfo(collection);
                }
            }

            return workspaceCollections;
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
        internal static void ValidateStreamReferenceProperty(ODataProperty streamProperty, IEdmProperty edmProperty, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            ODataVersionChecker.CheckStreamReferenceProperty(version);
            ValidationUtils.ValidateStreamReferenceProperty(streamProperty, edmProperty);

            ODataStreamReferenceValue streamValue = (ODataStreamReferenceValue)streamProperty.Value;
            ValidateStreamReferenceValue(streamValue, false /*isDefaultStream*/);
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
                // Complex and Multivalue types must match exactly
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
