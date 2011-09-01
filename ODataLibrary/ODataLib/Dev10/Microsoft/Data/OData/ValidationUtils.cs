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
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for validating OData content (applicable for readers and writers).
    /// </summary>
    internal static class ValidationUtils
    {
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
        /// Validates that an open property value is supported.
        /// </summary>
        /// <param name="propertyName">The name of the open property.</param>
        /// <param name="value">The value of the open property.</param>
        internal static void ValidateOpenPropertyValue(string propertyName, object value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (value is ODataMultiValue)
            {
                throw new ODataException(Strings.ValidationUtils_OpenMultiValueProperty(propertyName));
            }

            if (value is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ValidationUtils_OpenStreamProperty(propertyName));
            }
        }

        /// <summary>
        /// Validates a type name to ensure that it's not an empty string and resolves it against the provided <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <returns>The type with the given name and kind if a user model was available, otherwise null.</returns>
        internal static IEdmType ValidateTypeName(IEdmModel model, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            Debug.Assert(typeName != null, "typeName != null");

            // we do not allow empty type names
            if (typeName.Length == 0)
            {
                throw new ODataException(Strings.ValidationUtils_TypeNameMustNotBeEmpty);
            }

            // If we do have metadata, lookup the type and translate it to a type.
            IEdmType type = MetadataUtils.ResolveTypeName(model, typeName);
            if (model.IsUserModel() && type == null)
            {
                throw new ODataException(Strings.ValidationUtils_UnrecognizedTypeName(typeName));
            }

            return type;
        }

        /// <summary>
        /// Validates an entity type name to ensure that it's not an empty string and resolves it if metadata is available.
        /// </summary>
        /// <param name="model">The model to use or null if no metadata is available.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <returns>The type with the given name and kind if the metadata was available, otherwise null.</returns>
        internal static IEdmEntityType ValidateEntityTypeName(IEdmModel model, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            IEdmType edmType = ValidateTypeName(model, typeName);

            if (edmType != null)
            {
                ValidateTypeKind(edmType, EdmTypeKind.Entity);
            }

            return (IEdmEntityType)edmType;
        }

        /// <summary>
        /// Validates a type kind for a value type.
        /// </summary>
        /// <param name="typeKind">The type kind.</param>
        /// <param name="typeName">The name of the type (used for error reporting only).</param>
        internal static void ValidateValueTypeKind(EdmTypeKind typeKind, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(typeName != null, "typeName != null");

            if (typeKind != EdmTypeKind.Primitive && typeKind != EdmTypeKind.Complex && typeKind != EdmTypeKind.Collection)
            {
                throw new ODataException(Strings.ValidationUtils_IncorrectValueTypeKind(typeName, typeKind.ToString()));
            }
        }

        /// <summary>
        /// Validates a type name to ensure that it's not an empty string and resolves it if metadata is available
        /// and checks that it is a value type (non-Entity).
        /// </summary>
        /// <param name="model">The model to use.</param>
        /// <param name="typeName">The type name to validate.</param>
        /// <param name="typeKind">The expected type kind for the given type name.</param>
        /// <returns>The  type with the given name if the metadata was available, otherwise null.</returns>
        internal static IEdmType ValidateValueTypeName(IEdmModel model, string typeName, EdmTypeKind typeKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");
            
            IEdmType type = ValidateTypeName(model, typeName);

            if (type != null)
            {
                ValidateTypeKind(type, typeKind);
            }

            return type;
        }

        /// <summary>
        /// Validates that <paramref name="multiValueTypeName"/> is a valid type name for a MultiValue and returns its item type name.
        /// </summary>
        /// <param name="multiValueTypeName">The name of the MultiValue type.</param>
        /// <returns>The item type name for the <paramref name="multiValueTypeName"/>.</returns>
        internal static string ValidateMultiValueTypeName(string multiValueTypeName)
        {
            DebugUtils.CheckNoExternalCallers();

            string itemTypeName = MetadataUtils.GetMultiValueItemTypeName(multiValueTypeName);

            if (itemTypeName == null)
            {
                throw new ODataException(Strings.ValidationUtils_InvalidMultiValueTypeName(multiValueTypeName));
            }

            return itemTypeName;
        }

        /// <summary>
        /// Validates that the <paramref name="payloadEntityTypeReference"/> is assignable to the <paramref name="expectedEntityTypeReference"/>
        /// and fails if it's not.
        /// </summary>
        /// <param name="expectedEntityTypeReference">The expected entity type reference, the base type of the entities expected.</param>
        /// <param name="payloadEntityTypeReference">The payload entity type reference to validate.</param>
        internal static void ValidateEntityTypeIsAssignable(IEdmEntityTypeReference expectedEntityTypeReference, IEdmEntityTypeReference payloadEntityTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(expectedEntityTypeReference != null, "expectedEntityTypeReference != null");
            Debug.Assert(payloadEntityTypeReference != null, "payloadEntityTypeReference != null");

            // Entity types must be assignable
            if (!EdmLibraryExtensions.IsAssignableFrom(expectedEntityTypeReference.EntityDefinition(), payloadEntityTypeReference.EntityDefinition()))
            {
                throw new ODataException(Strings.ValidationUtils_EntryTypeNotAssignableToExpectedType(payloadEntityTypeReference.ODataFullName(), expectedEntityTypeReference.ODataFullName()));
            }
        }

        /// <summary>
        /// Validates that the <paramref name="typeReference"/> represents a MultiValue type.
        /// </summary>
        /// <param name="typeReference">The type reference to validate.</param>
        /// <returns>The <see cref="IEdmCollectionTypeReference"/> instance representing the MultiValue passed as <paramref name="typeReference"/>.</returns>
        internal static IEdmCollectionTypeReference ValidateMultiValueType(this IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsMultiValueOrNull();

            if (collectionTypeReference != null && !typeReference.IsODataMultiValueTypeKind())
            {
                throw new ODataException(Strings.ValidationUtils_InvalidMultiValueTypeReference(typeReference.TypeKind()));
            }

            return collectionTypeReference;
        }

        /// <summary>
        /// Validates an item of a MultiValue to ensure it's not null and it's not of multi-value and stream reference types.
        /// </summary>
        /// <param name="item">The MultiValue item.</param>
        internal static void ValidateMultiValueItem(object item)
        {
            DebugUtils.CheckNoExternalCallers();

            if (item == null)
            {
                throw new ODataException(Strings.ValidationUtils_MultiValueElementsMustNotBeNull);
            }

            if (item is ODataMultiValue)
            {
                throw new ODataException(Strings.ValidationUtils_NestedMultiValuesAreNotSupported);
            }

            if (item is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ValidationUtils_StreamReferenceValuesNotSupportedInMultiValues);
            }
        }

        /// <summary>
        /// Validates an item of a collection to ensure it is not of multi-value and stream reference types.
        /// </summary>
        /// <param name="item">The collection item.</param>
        internal static void ValidateCollectionItem(object item)
        {
            DebugUtils.CheckNoExternalCallers();

            if (item is ODataMultiValue)
            {
                throw new ODataException(Strings.ValidationUtils_MultiValuesNotSupportedInCollections);
            }

            if (item is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ValidationUtils_StreamReferenceValueNotSupportedInCollections);
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataNavigationLink"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="navigationLink">The navigation link to validate.</param>
        internal static void ValidateNavigationLink(ODataNavigationLink navigationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(navigationLink != null, "navigationLink != null");

            // Navigation link must have a non-empty name
            if (string.IsNullOrEmpty(navigationLink.Name))
            {
                throw new ODataException(Strings.ValidationUtils_LinkMustSpecifyName);
            }
        }

        /// <summary>
        /// Validates a stream reference property to ensure it's not null and it's name if correct.
        /// </summary>
        /// <param name="streamProperty">The stream reference property to validate.</param>
        /// <param name="edmProperty">Property metadata to validate against.</param>
        internal static void ValidateStreamReferenceProperty(ODataProperty streamProperty, IEdmProperty edmProperty)
        {
            DebugUtils.CheckNoExternalCallers();

            Debug.Assert(streamProperty != null, "streamProperty != null");
            Debug.Assert(!string.IsNullOrEmpty(streamProperty.Name), "!string.IsNullOrEmpty(streamProperty.Name)");
            Debug.Assert(streamProperty.Value is ODataStreamReferenceValue, "This method should only be called for stream reference properties.");
            Debug.Assert(edmProperty == null || edmProperty.Name == streamProperty.Name, "edmProperty == null || edmProperty.Name == streamProperty.Name");

            if (edmProperty != null && !edmProperty.Type.IsStream())
            {
                throw new ODataException(Strings.ValidationUtils_MismatchPropertyKindForNamedStreamProperty(streamProperty.Name));
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataAssociationLink"/> to ensure it's not null.
        /// </summary>
        /// <param name="associationLink">The association link to ensure it's not null.</param>
        internal static void ValidateAssociationLinkNotNull(ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();

            // null link can not appear in the enumeration
            if (associationLink == null)
            {
                throw new ODataException(Strings.ValidationUtils_EnumerableContainsANullItem("ODataEntry.AssociationLinks"));
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataAssociationLink"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="associationLink">The association link to validate.</param>
        internal static void ValidateAssociationLink(ODataAssociationLink associationLink)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(associationLink != null, "associationLink != null");

            // Association link must have a non-empty name
            if (string.IsNullOrEmpty(associationLink.Name))
            {
                throw new ODataException(Strings.ValidationUtils_AssociationLinkMustSpecifyName);
            }

            // Association link must specify the Url
            if (associationLink.Url == null)
            {
                throw new ODataException(Strings.ValidationUtils_AssociationLinkMustSpecifyUrl);
            }
        }


        /// <summary>
        /// Validates that the specified <paramref name="entry"/> is a valid entry as per the specified type.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        /// <param name="entityType">Optional entity type to validate the entry against.</param>
        /// <param name="validateMediaResource">true if the validation of the default MediaResource should be done; false otherwise.</param>
        /// <remarks>If the <paramref name="entityType"/> is available only entry-level tests are performed, properties and such are not validated.</remarks>
        internal static void ValidateEntryMetadata(ODataEntry entry, IEdmEntityType entityType, bool validateMediaResource)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            if (entityType != null)
            {
                if (validateMediaResource)
                {
                    bool hasDefaultStream = entityType.HasDefaultStream();
                    if (entry.MediaResource == null)
                    {
                        if (hasDefaultStream)
                        {
                            throw new ODataException(Strings.ValidationUtils_EntryWithoutMediaResourceAndMLEType(entityType.ODataFullName()));
                        }
                    }
                    else
                    {
                        if (!hasDefaultStream)
                        {
                            throw new ODataException(Strings.ValidationUtils_EntryWithMediaResourceAndNonMLEType(entityType.ODataFullName()));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates that a given primitive value is of the expected (primitive) type.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="expectedTypeReference">The expected type for the value.</param>
        internal static void ValidateIsExpectedPrimitiveType(object value, IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");

            // Note that valueInstanceType is never a nullable type because GetType() on Nullable variables at runtime will always yield
            // a Type object that represents the underlying type, not the Nullable type itself. 
            Type valueInstanceType = value.GetType();
            IEdmTypeReference valueTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(valueInstanceType);
            if (valueTypeReference == null)
            {
                throw new ODataException(Strings.ValidationUtils_UnsupportedPrimitiveType(valueInstanceType.FullName));
            }

            if (!expectedTypeReference.IsODataPrimitiveTypeKind())
            {
                // non-primitive type found for primitive value.
                throw new ODataException(Strings.ValidationUtils_NonPrimitiveTypeForPrimitiveValue(expectedTypeReference.ODataFullName()));
            }

            ValidateMetadataPrimitiveType(expectedTypeReference, valueTypeReference);
        }

        /// <summary>
        /// Validates that the expected primitive type matches the actual primitive type.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type.</param>
        /// <param name="actualTypeReference">The actual type.</param>
        internal static void ValidateMetadataPrimitiveType(IEdmTypeReference expectedTypeReference, IEdmTypeReference actualTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(expectedTypeReference != null && expectedTypeReference.IsODataPrimitiveTypeKind(), "expectedTypeReference must be a primitive type.");
            Debug.Assert(actualTypeReference != null && actualTypeReference.IsODataPrimitiveTypeKind(), "actualTypeReference must be a primitive type.");

            // The two primitive types match if they have the same definition and either both or only the expected type is nullable
            bool nullableCompatible = expectedTypeReference.IsNullable == actualTypeReference.IsNullable || expectedTypeReference.IsNullable && !actualTypeReference.IsNullable;
            IEdmType expectedType = expectedTypeReference.Definition;
            IEdmType actualType = actualTypeReference.Definition;
            bool typeCompatible = expectedTypeReference.Definition.IsEquivalentTo(actualTypeReference.Definition) || IsValidGeographyType(expectedType, actualType);
            if (!nullableCompatible || !typeCompatible)
            {
                // incompatible type name for value!
                throw new ODataException(Strings.ValidationUtils_IncompatiblePrimitiveItemType(actualTypeReference.ODataFullName(), expectedTypeReference.ODataFullName()));
            }
    }

        /// <summary>
        /// Validates that the expected property allows null value.
        /// </summary>
        /// <param name="expectedProperty">The metadata for the expected property.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        internal static void ValidateNullPropertyValue(IEdmProperty expectedProperty, ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(writerBehavior != null, "writerBehavior != null");

            if (expectedProperty != null)
            {
                IEdmTypeReference propertyTypeReference = expectedProperty.Type;
                if (propertyTypeReference.IsODataMultiValueTypeKind())
                {
                    throw new ODataException(Strings.ValidationUtils_MultiValuePropertiesMustNotHaveNullValue(expectedProperty.Name));
                }

                if (propertyTypeReference.IsODataPrimitiveTypeKind())
                {
                    // WCF DS allows null values for non-nullable primitive types, so we need to check for a knob which enables this behavior.
                    // See the description of ODataWriterBehavior.AllowNullValuesForNonNullablePrimitiveTypes for more details.
                    if (!propertyTypeReference.IsNullable && !writerBehavior.AllowNullValuesForNonNullablePrimitiveTypes)
                    {
                        throw new ODataException(Strings.ValidationUtils_NonNullablePrimitivePropertiesMustNotHaveNullValue(expectedProperty.Name, expectedProperty.Type.ODataFullName()));
                    }
                }
                else if (propertyTypeReference.IsStream())
                {
                    throw new ODataException(Strings.ValidationUtils_NamedStreamPropertiesMustNotHaveNullValue(expectedProperty.Name));
                }
            }
        }

        /// <summary>
        /// Validates a resource collection.
        /// </summary>
        /// <param name="collectionInfo">The resource collection to validate.</param>
        internal static void ValidateResourceCollectionInfo(ODataResourceCollectionInfo collectionInfo)
        {
            DebugUtils.CheckNoExternalCallers();

            // The resource collection URL must not be null; 
            if (collectionInfo.Url == null)
            {
                throw new ODataException(Strings.ValidationUtils_ResourceCollectionMustSpecifyUrl);
            }
        }

        /// <summary>
        /// Validates a resource collection Url.
        /// </summary>
        /// <param name="collectionInfoUrl">The resource collection url to validate.</param>
        internal static void ValidateResourceCollectionInfoUrl(string collectionInfoUrl)
        {
            DebugUtils.CheckNoExternalCallers();

            // The resource collection URL must not be null or empty; 
            if (collectionInfoUrl == null)
            {
                throw new ODataException(Strings.ValidationUtils_ResourceCollectionUrlMustNotBeNull);
            }
        }

        /// <summary>
        /// Validates that the specified type is of the requested kind.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <param name="typeKind">The expected type kind for the given type name.</param>
        /// <remarks>This override of the method will not create the type name unless it fails.</remarks>
        internal static void ValidateTypeKind(IEdmType type, EdmTypeKind typeKind)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(type != null, "typeReference != null");

            if (type.TypeKind != typeKind)
            {
                throw new ODataException(Strings.ValidationUtils_IncorrectTypeKind(type.ODataFullName(), typeKind.ToString(), type.TypeKind.ToString()));
            }
        }

        /// <summary>
        /// Validates that the observed type kind is the expected type kind.
        /// </summary>
        /// <param name="actualTypeKind">The actual type kind to compare.</param>
        /// <param name="expectedTypeKind">The expected type kind to compare against.</param>
        /// <param name="typeName">The name of the type to use in the error.</param>
        internal static void ValidateTypeKind(EdmTypeKind actualTypeKind, EdmTypeKind expectedTypeKind, string typeName)
        {
            DebugUtils.CheckNoExternalCallers();

            if (actualTypeKind != expectedTypeKind)
            {
                throw new ODataException(Strings.ValidationUtils_IncorrectTypeKind(typeName, expectedTypeKind.ToString(), actualTypeKind.ToString()));
            }
        }

        /// <summary>
        /// Validates the given <paramref name="model"/>. In particular it checks for non-null models that they have  
        /// at most one entity container.
        /// </summary>
        /// <param name="model">The <see cref="IEdmModel"/> to check.</param>
        internal static void ValidateModel(IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(model != null, "model != null");

            // Make sure that the model has a single entity container
            if (model.EntityContainers != null)
            {
                IEdmEntityContainer container = model.EntityContainers.Skip(1).FirstOrDefault();
                if (container != null)
                {
                    throw new ODataException(Strings.ValidationUtils_ModelWithMultipleContainers);
                }
            }
        }

        /// <summary>
        /// If the expected type is Geography, validates that the actual type is a valid derived type.
        /// </summary>
        /// <param name="expectedType">Expected type to validate against.</param>
        /// <param name="actualType">Actual type to validate.</param>
        /// <returns>True if Geography is expected and the actual type is a valid derived type, otherwise false.</returns>
        private static bool IsValidGeographyType(IEdmType expectedType, IEdmType actualType)
        {
            IEdmPrimitiveType expectedPrimitiveType = expectedType as IEdmPrimitiveType;
            IEdmPrimitiveType actualPrimitiveType = actualType as IEdmPrimitiveType;

            return expectedPrimitiveType.PrimitiveKind == EdmPrimitiveTypeKind.Geography && actualPrimitiveType.IsGeographyType();
        }
    }
}
