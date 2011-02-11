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

namespace System.Data.OData
{
    #region Namespaces.
    using System.Collections.Generic;
    using System.Data.Services.Providers;
    using System.Diagnostics;
    #endregion Namespaces.

    /// <summary>
    /// Class with utility methods for validating OData content.
    /// </summary>
    internal static class ValidationUtils
    {
        /// <summary>
        /// Validates an <see cref="ODataProperty"/> to ensure all required information is specified.
        /// </summary>
        /// <param name="property">The property to validate.</param>
        internal static void ValidateProperty(ODataProperty property)
        {
            DebugUtils.CheckNoExternalCallers();

            if (property == null)
            {
                throw new ODataException(Strings.ODataWriter_PropertyMustNotBeNull);
            }

            // Properties must have a non-empty name
            if (string.IsNullOrEmpty(property.Name))
            {
                throw new ODataException(Strings.ODataWriter_PropertiesMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// Validates that a property with the specified name exists on a given resource type.
        /// The resource type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">The name of the property to validate.</param>
        /// <param name="owningType">The owning resource type of the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</param>
        /// <returns>The <see cref="ResourceProperty"/> instance representing the property with name <paramref name="propertyName"/> 
        /// or null if no metadata is available.</returns>
        internal static ResourceProperty ValidatePropertyDefined(string propertyName, ResourceType owningType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (owningType == null)
            {
                return null;
            }

            ResourceProperty resourceProperty = owningType.TryResolvePropertyName(propertyName);

            // verify that the property is declared if the type is not an open type.
            if (!owningType.IsOpenType && resourceProperty == null)
            {
                throw new ODataException(Strings.ODataWriter_PropertyDoesNotExistOnType(propertyName, owningType.FullName));
            }

            return resourceProperty;
        }

        /// <summary>
        /// Validates a type name to ensure that it's not an empty string.
        /// </summary>
        /// <param name="metadata">The metadata provider to use or null if no metadata is available.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <param name="typeKind">The expected type kind for the given type name.</param>
        /// <param name="isOpenPropertyType">True if the type name belongs to an open property.</param>
        /// <returns>The resource type with the given name and kind if the metadata was available, otherwise null.</returns>
        internal static ResourceType ValidateTypeName(DataServiceMetadataProviderWrapper metadata, string typeName, ResourceTypeKind typeKind, bool isOpenPropertyType)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeName == null)
            {
                // if we have metadata the type name of an entry or a complex value of an open property must not be null
                if (metadata != null && (typeKind == ResourceTypeKind.EntityType || isOpenPropertyType))
                {
                    throw new ODataException(Strings.ODataWriterCore_MissingTypeNameWithMetadata);
                }

                return null;
            }

            // we do not allow empty type names
            if (typeName.Length == 0)
            {
                throw new ODataException(Strings.ODataWriter_TypeNameMustNotBeEmpty);
            }

            // If we do have metadata, lookup the type and translate it to ResourceType.
            ResourceType resourceType = null;
            if (metadata != null)
            {
                resourceType = typeKind == ResourceTypeKind.MultiValue
                    ? ValidateMultiValueTypeName(metadata, typeName)
                    : ValidateNonMultiValueTypeName(metadata, typeName, typeKind);
            }

            return resourceType;
        }

        /// <summary>
        /// Validates that the (optional) <paramref name="typeFromMetadata"/> is compatible with the (optional) <paramref name="typeFromValue"/>.
        /// </summary>
        /// <param name="typeFromMetadata">The (optional) type from the metadata definition.</param>
        /// <param name="typeFromValue">The (optional) type derived from the value.</param>
        /// <param name="typeKind">The expected type kind.</param>
        /// <returns>The type of the property as derived from the <paramref name="typeFromMetadata"/> and/or <paramref name="typeFromValue"/>.</returns>
        internal static ResourceType ValidateMetadataType(ResourceType typeFromMetadata, ResourceType typeFromValue, ResourceTypeKind typeKind)
        {
            DebugUtils.CheckNoExternalCallers();

            if (typeFromMetadata == null)
            {
                // if we have no metadata information there is nothing to validate
                return typeFromValue;
            }

            if (typeFromValue == null)
            {
                // derive the property type from the metadata
                if (typeFromMetadata.ResourceTypeKind != typeKind)
                {
                    throw new ODataException(Strings.ODataWriter_IncompatibleTypeKind(typeKind.ToString(), typeFromMetadata.Name, typeFromMetadata.ResourceTypeKind.ToString()));
                }

                return typeFromMetadata;
            }
            else
            {
                // Make sure the types are consistent
                if (typeFromMetadata.FullName != typeFromValue.FullName)
                {
                    throw new ODataException(Strings.ODataWriter_IncompatibleType(typeFromValue.FullName, typeFromMetadata.FullName));
                }

                return typeFromValue;
            }
        }

        /// <summary>
        /// Validates an item of a MultiValue to ensure it's not null.
        /// </summary>
        /// <param name="item">The MultiValue item.</param>
        internal static void ValidateMultiValueItem(object item)
        {
            DebugUtils.CheckNoExternalCallers();

            if (item == null)
            {
                throw new ODataException(Strings.ODataWriter_MultiValueElementsMustNotBeNull);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataLink"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="link">The link to validate.</param>
        internal static void ValidateLink(ODataLink link)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(link != null, "link != null");

            // Link must have a non-empty name
            if (string.IsNullOrEmpty(link.Name))
            {
                throw new ODataException(Strings.ODataWriter_LinkMustSpecifyName);
            }
        }

        /// <summary>
        /// Validates a named stream to ensure it's not null and it's name if correct.
        /// </summary>
        /// <param name="namedStream">The named stream to validate.</param>
        /// <param name="version">The version of the OData protocol used for checking.</param>
        internal static void ValidateNamedStream(ODataMediaResource namedStream, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            ODataVersionChecker.CheckNamedStreams(version);

            if (namedStream == null)
            {
                throw new ODataException(Strings.ODataWriter_NamedStreamMustNotBeNull);
            }

            if (string.IsNullOrEmpty(namedStream.Name))
            {
                throw new ODataException(Strings.ODataWriter_NamedStreamMustHaveNonEmptyName);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataAssociationLink"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="associationLink">The association link to validate.</param>
        /// <param name="version">The version of the OData protocol used for checking.</param>
        internal static void ValidateAssociationLink(ODataAssociationLink associationLink, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();

            ODataVersionChecker.CheckAssociationLinks(version);

            // null link can not appear in the enumeration
            if (associationLink == null)
            {
                throw new ODataException(Strings.ODataWriter_AssociationLinkMustNotBeNull);
            }

            // Association link must have a non-empty name
            if (string.IsNullOrEmpty(associationLink.Name))
            {
                throw new ODataException(Strings.ODataWriter_AssociationLinkMustSpecifyName);
            }

            // Association link must specify the Url
            if (associationLink.Url == null)
            {
                throw new ODataException(Strings.ODataWriter_AssociationLinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataEntry"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        internal static void ValidateEntry(ODataEntry entry)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            // Verify non-empty ID (entries can have no (null) ID for insert scenarios; empty IDs are not allowed)
            if (entry.Id != null && entry.Id.Length == 0)
            {
                throw new ODataException(Strings.ODataWriter_EntriesMustHaveNonEmptyId);
            }

            // Type name is verified in the format writers since it's shared with other non-entity types
            // and verifying it here would mean doing it twice.

            // Verify the default stream if it's present
            ODataMediaResource defaultStream = entry.MediaResource;
            if (defaultStream != null)
            {
                if (defaultStream.Name != null)
                {
                    throw new ODataException(Strings.ODataWriter_DefaultStreamMustNotHaveName);
                }

                if (string.IsNullOrEmpty(defaultStream.ContentType))
                {
                    throw new ODataException(Strings.ODataWriter_DefaultStreamMustHaveNonEmptyContentType);
                }

                if (defaultStream.ReadLink == null)
                {
                    throw new ODataException(Strings.ODataWriter_DefaultStreamMustHaveReadLink);
                }

                if (defaultStream.EditLink == null && defaultStream.ETag != null)
                {
                    throw new ODataException(Strings.ODataWriter_DefaultStreamMustHaveEditLinkToHaveETag);
                }
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataFeed"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="feed">The feed to validate.</param>
        /// <param name="writingRequest">Flag indicating whether the feed is written as part of a request or a response.</param>
        /// <param name="version">The version of the OData protocol used for checking.</param>
        internal static void ValidateFeed(ODataFeed feed, bool writingRequest, ODataVersion version)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(feed != null, "feed != null");

            // Verify non-empty ID
            if (string.IsNullOrEmpty(feed.Id))
            {
                throw new ODataException(Strings.ODataWriter_FeedsMustHaveNonEmptyId);
            }

            // Verify next link
            if (feed.NextPageLink != null)
            {
                // Check that NextPageLink is not set for requests
                if (writingRequest)
                {
                    throw new ODataException(Strings.ODataWriterCore_NextPageLinkInRequest);
                }

                // Verify version requirements
                ODataVersionChecker.CheckServerPaging(version);
            }
        }

        /// <summary>
        /// Validates that <paramref name="multiValueTypeName"/> is a valid type name for a MultiValue.
        /// </summary>
        /// <param name="metadata">The metadata against which to validate the type name.</param>
        /// <param name="multiValueTypeName">The name of the MultiValue.</param>
        /// <returns>A <see cref="MultiValueResourceType"/> for the <paramref name="multiValueTypeName"/>.</returns>
        internal static ResourceType ValidateMultiValueTypeName(DataServiceMetadataProviderWrapper metadata, string multiValueTypeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadata != null, "metadata != null");

            string itemTypeName = MetadataUtils.GetMultiValueItemTypeName(multiValueTypeName);

            if (itemTypeName == null)
            {
                throw new ODataException(Strings.ODataWriterCore_InvalidMultiValueTypeName(multiValueTypeName));
            }

            ResourceType resourceType = metadata.TryResolveResourceType(itemTypeName);

            if (resourceType == null)
            {
                // if we are resolving a MultiValue the item type might be primitive
                ResourceType[] primitiveTypes = PrimitiveTypeUtils.PrimitiveTypes;
                for (int i = 0; i < primitiveTypes.Length; ++i)
                {
                    ResourceType primitiveType = primitiveTypes[i];
                    if (string.CompareOrdinal(itemTypeName, primitiveType.FullName) == 0)
                    {
                        resourceType = primitiveType;
                        break;
                    }
                }
            }

            if (resourceType == null)
            {
                throw new ODataException(Strings.ODataWriterCore_UnrecognizedTypeName(multiValueTypeName, ResourceTypeKind.MultiValue));
            }

            // create a MultiValue resource type from the item type
            resourceType = ResourceType.GetMultiValueResourceType(resourceType);

            return resourceType;
        }

        /// <summary>
        /// Validates that <paramref name="typeName"/> is a valid type name of the specified kind (<paramref name="typeKind"/>).
        /// </summary>
        /// <param name="metadata">The metadata against which to validate the type name.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <param name="typeKind">The expected <see cref="ResourceTypeKind"/> of the type.</param>
        /// <returns>A <see cref="ResourceType"/> for the <paramref name="typeName"/>.</returns>
        internal static ResourceType ValidateNonMultiValueTypeName(DataServiceMetadataProviderWrapper metadata, string typeName, ResourceTypeKind typeKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(metadata != null, "metadata != null");

            ResourceType resourceType = metadata.TryResolveResourceType(typeName);

            if (resourceType == null)
            {
                throw new ODataException(Strings.ODataWriterCore_UnrecognizedTypeName(typeName, typeKind));
            }

            if (resourceType.ResourceTypeKind != typeKind)
            {
                throw new ODataException(Strings.ODataWriterCore_IncorrectTypeKind(typeName, typeKind.ToString(), resourceType.ResourceTypeKind.ToString()));
            }

            return resourceType;
        }

        /// <summary>
        /// Validates that a given primitive value is of the expected (primitive) type.
        /// </summary>
        /// <param name="value">The MultiValue item to check.</param>
        /// <param name="expectedType">The expected type for the item.</param>
        internal static void ValidateIsExpectedPrimitiveType(object value, ResourceType expectedType)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");
            Debug.Assert(expectedType != null, "expectedItemType != null");

            if (expectedType.ResourceTypeKind != ResourceTypeKind.Primitive)
            {
                // non-primitive type found for primitive value.
                throw new ODataException(Strings.ODataUtils_NonPrimitiveTypeForPrimitiveValue(expectedType.FullName));
            }

            ResourceType valueType = ResourceType.GetPrimitiveResourceType(value.GetType());
            Debug.Assert(valueType != null && valueType.ResourceTypeKind == ResourceTypeKind.Primitive, "Could not resolve type of primitve value.");

            if (!valueType.IsAssignableFrom(expectedType))
            {
                // incompatible type name for value!
                throw new ODataException(Strings.ODataUtils_IncompatiblePrimitiveItemType(valueType.FullName, expectedType.FullName));
            }
        }

        /// <summary>
        /// Validates a service document.
        /// </summary>
        /// <param name="entitySets">The entity sets used to validate the <paramref name="workspace"/> against.</param>
        /// <param name="workspace">The workspace to validate.</param>
        /// <returns>The set of collections in the specified <paramref name="workspace"/>.</returns>
        internal static IEnumerable<ODataResourceCollectionInfo> ValidateWorkspace(IEnumerable<ResourceSetWrapper> entitySets, ODataWorkspace workspace)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(workspace != null, "workspace != null");

            IEnumerable<ODataResourceCollectionInfo> workspaceCollections = workspace.Collections;
            Dictionary<string, ODataResourceCollectionInfo> collectionDictionary = new Dictionary<string, ODataResourceCollectionInfo>(EqualityComparer<string>.Default);
            if (workspaceCollections != null)
            {
                foreach (ODataResourceCollectionInfo collection in workspaceCollections)
                {
                    if (collection == null)
                    {
                        throw new ODataException(Strings.ODataUtils_WorkspaceCollectionsMustNotContainNullItem);
                    }

                    // validate that resource collection names are not null or empty.
                    ValidationUtils.ValidateResourceCollectionInfo(collection);

                    string collectionName = collection.Name;
                    if (collectionDictionary.ContainsKey(collectionName))
                    {
                        throw new ODataException(Strings.ODataUtils_ResourceCollectionMustHaveUniqueName(collectionName));
                    }

                    collectionDictionary.Add(collectionName, collection);
                }
            }

            // We validate that all entity sets in metadata are reflected as resource collections in the default workspace.
            if (entitySets != null)
            {
                foreach (ResourceSetWrapper entitySet in entitySets)
                {
                    ODataResourceCollectionInfo resourceCollection;
                    if (!collectionDictionary.TryGetValue(entitySet.Name, out resourceCollection))
                    {
                        // missing a resource collection for a resource set that exists in metadata
                        throw new ODataException(Strings.ODataUtils_MissingResourceCollectionForEntitySet(entitySet.Name));
                    }
                }
            }

            return collectionDictionary.Values;
        }

        /// <summary>
        /// Validates a resource collection.
        /// </summary>
        /// <param name="collectionInfo">The resource collection to validate.</param>
        internal static void ValidateResourceCollectionInfo(ODataResourceCollectionInfo collectionInfo)
        {
            DebugUtils.CheckNoExternalCallers();

            // The resource collection name must not be null or empty; it represents the name of the entity set.
            if (string.IsNullOrEmpty(collectionInfo.Name))
            {
                throw new ODataException(Strings.ODataUtils_ResourceCollectionMustHaveName);
            }
        }
    }
}
