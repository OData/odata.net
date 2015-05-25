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

namespace Microsoft.Data.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Csdl;
    using Microsoft.Data.OData.JsonLight;
    using Microsoft.Data.OData.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Class with utility methods for validating OData content (applicable for readers and writers).
    /// </summary>
    internal static class ValidationUtils
    {
        /// <summary>The set of characters that are invalid in property names.</summary>
        /// <remarks>Keep this array in sync with MetadataProviderUtils.InvalidCharactersInPropertyNames in Astoria.</remarks>
        internal static readonly char[] InvalidCharactersInPropertyNames = new char[] { ':', '.', '@' };

        /// <summary>Maximum batch boundary length supported (not includeding leading CRLF or '-').</summary>
        private const int MaxBoundaryLength = 70;

        /// <summary>
        /// Validates that an open property value is supported.
        /// </summary>
        /// <param name="propertyName">The name of the open property.</param>
        /// <param name="value">The value of the open property.</param>
        internal static void ValidateOpenPropertyValue(string propertyName, object value)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (value is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ValidationUtils_OpenStreamProperty(propertyName));
            }
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
        /// Validates that <paramref name="collectionTypeName"/> is a valid type name for a collection and returns its item type name.
        /// </summary>
        /// <param name="collectionTypeName">The name of the collection type.</param>
        /// <returns>The item type name for the <paramref name="collectionTypeName"/>.</returns>
        internal static string ValidateCollectionTypeName(string collectionTypeName)
        {
            DebugUtils.CheckNoExternalCallers();

            string itemTypeName = EdmLibraryExtensions.GetCollectionItemTypeName(collectionTypeName);

            if (itemTypeName == null)
            {
                throw new ODataException(Strings.ValidationUtils_InvalidCollectionTypeName(collectionTypeName));
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
        /// Validates that the <paramref name="typeReference"/> represents a collection type.
        /// </summary>
        /// <param name="typeReference">The type reference to validate.</param>
        /// <returns>The <see cref="IEdmCollectionTypeReference"/> instance representing the collection passed as <paramref name="typeReference"/>.</returns>
        internal static IEdmCollectionTypeReference ValidateCollectionType(IEdmTypeReference typeReference)
        {
            DebugUtils.CheckNoExternalCallers();

            IEdmCollectionTypeReference collectionTypeReference = typeReference.AsCollectionOrNull();

            if (collectionTypeReference != null && !typeReference.IsNonEntityCollectionType())
            {
                throw new ODataException(Strings.ValidationUtils_InvalidCollectionTypeReference(typeReference.TypeKind()));
            }

            return collectionTypeReference;
        }

        /// <summary>
        /// Validates an item of a collection to ensure it is not of collection and stream reference types.
        /// </summary>
        /// <param name="item">The collection item.</param>
        /// <param name="isStreamable">True if the items in the collection are streamable, false otherwise.</param>
        internal static void ValidateCollectionItem(object item, bool isStreamable)
        {
            DebugUtils.CheckNoExternalCallers();

            // Null values are allowed for streamable collections (such as top-level collections), but not atomic collections.
            // We don't allow null items in atomic collections because it creates ambigious situations with EPM.
            if (!isStreamable && item == null)
            {
                throw new ODataException(Strings.ValidationUtils_NonStreamingCollectionElementsMustNotBeNull);
            }

            if (item is ODataCollectionValue)
            {
                throw new ODataException(Strings.ValidationUtils_NestedCollectionsAreNotSupported);
            }

            if (item is ODataStreamReferenceValue)
            {
                throw new ODataException(Strings.ValidationUtils_StreamReferenceValuesNotSupportedInCollections);
            }
        }

        /// <summary>
        /// Validates a null collection item against the expected type.
        /// </summary>
        /// <param name="expectedItemType">The expected item type or null if no expected item type exists.</param>
        /// <param name="writerBehavior">The <see cref="ODataWriterBehavior"/> instance controlling the behavior of the writer.</param>
        internal static void ValidateNullCollectionItem(IEdmTypeReference expectedItemType, ODataWriterBehavior writerBehavior)
        {
            DebugUtils.CheckNoExternalCallers();

            if (expectedItemType != null)
            {
                if (expectedItemType.IsODataPrimitiveTypeKind())
                {
                    // WCF DS allows null values for non-nullable primitive types, so we need to check for a knob which enables this behavior.
                    // See the description of ODataWriterBehavior.AllowNullValuesForNonNullablePrimitiveTypes for more details.
                    if (!expectedItemType.IsNullable && !writerBehavior.AllowNullValuesForNonNullablePrimitiveTypes)
                    {
                        throw new ODataException(Strings.ValidationUtils_NullCollectionItemForNonNullableType(expectedItemType.ODataFullName()));
                    }
                }
            }
        }

        /// <summary>
        /// Validates a stream reference property to ensure it's not null and its name if correct.
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
                throw new ODataException(Strings.ValidationUtils_MismatchPropertyKindForStreamProperty(streamProperty.Name));
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
        /// Validates the name for an association link.
        /// </summary>
        /// <param name="associationLinkName">The name of the association link to validate.</param>
        internal static void ValidateAssociationLinkName(string associationLinkName)
        {
            DebugUtils.CheckNoExternalCallers();

            // Association link must have a non-empty name
            if (string.IsNullOrEmpty(associationLinkName))
            {
                throw new ODataException(Strings.ValidationUtils_AssociationLinkMustSpecifyName);
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

            ValidateAssociationLinkName(associationLink.Name);

            // Association link must specify the Url
            if (associationLink.Url == null)
            {
                throw new ODataException(Strings.ValidationUtils_AssociationLinkMustSpecifyUrl);
            }
        }

        /// <summary>
        /// Increases the given recursion depth, and then verifies that it doesn't exceed the recursion depth limit.
        /// </summary>
        /// <param name="recursionDepth">The current depth of the payload element hierarchy.</param>
        /// <param name="maxDepth">The maximum allowed recursion depth.</param>
        internal static void IncreaseAndValidateRecursionDepth(ref int recursionDepth, int maxDepth)
        {
            DebugUtils.CheckNoExternalCallers();

            recursionDepth++;
            if (recursionDepth > maxDepth)
            {
                throw new ODataException(Strings.ValidationUtils_RecursionDepthLimitReached(maxDepth));
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataOperation"/> to ensure it's not null.
        /// </summary>
        /// <param name="operation">The operation to ensure it's not null.</param>
        /// <param name="isAction">Whether <paramref name="operation"/> is an <see cref="ODataAction"/>.</param>
        internal static void ValidateOperationNotNull(ODataOperation operation, bool isAction)
        {
            DebugUtils.CheckNoExternalCallers();

            // null action/function can not appear in the enumeration
            if (operation == null)
            {
                string enumerableName = isAction ? "ODataEntry.Actions" : "ODataEntry.Functions";
                throw new ODataException(Strings.ValidationUtils_EnumerableContainsANullItem(enumerableName));
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataOperation"/> to ensure its metadata is specified and valid.
        /// </summary>
        /// <param name="operation">The operation to validate.</param>
        internal static void ValidateOperationMetadataNotNull(ODataOperation operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operation != null, "operation != null");

            // ODataOperation must have a Metadata property.
            if (operation.Metadata == null)
            {
                throw new ODataException(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyMetadata(operation.GetType().Name));
            }
        }

        /// <summary>
        /// Validates an <see cref="ODataOperation"/> to ensure its target is specified and valid.
        /// </summary>
        /// <param name="operation">The operation to validate.</param>
        internal static void ValidateOperationTargetNotNull(ODataOperation operation)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(operation != null, "operation != null");

            // ODataOperation must have a Target property.
            if (operation.Target == null)
            {
                throw new ODataException(Strings.ValidationUtils_ActionsAndFunctionsMustSpecifyTarget(operation.GetType().Name));
            }
        }

        /// <summary>
        /// Validates that the specified <paramref name="entry"/> is a valid entry as per the specified type.
        /// </summary>
        /// <param name="entry">The entry to validate.</param>
        /// <param name="entityType">Optional entity type to validate the entry against.</param>
        /// <param name="model">Model containing the entity type.</param>
        /// <param name="validateMediaResource">true if the validation of the default MediaResource should be done; false otherwise.</param>
        /// <remarks>If the <paramref name="entityType"/> is available only entry-level tests are performed, properties and such are not validated.</remarks>
        internal static void ValidateEntryMetadataResource(ODataEntry entry, IEdmEntityType entityType, IEdmModel model, bool validateMediaResource)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(entry != null, "entry != null");

            if (entityType != null)
            {
                Debug.Assert(model != null, "model != null");
                Debug.Assert(model.IsUserModel(), "model.IsUserModel()");

                if (validateMediaResource)
                {
                    bool hasDefaultStream = model.HasDefaultStream(entityType);
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
            IEdmPrimitiveTypeReference valuePrimitiveTypeReference = EdmLibraryExtensions.GetPrimitiveTypeReference(valueInstanceType);
            ValidateIsExpectedPrimitiveType(value, valuePrimitiveTypeReference, expectedTypeReference);
        }

        /// <summary>
        /// Validates that a given primitive value is of the expected (primitive) type.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="valuePrimitiveTypeReference">The primitive type reference for the value - some callers have this already, so we save the lookup here.</param>
        /// <param name="expectedTypeReference">The expected type for the value.</param>
        /// <remarks>
        /// Some callers have the primitive type reference already resolved (from the value type)
        /// so this method is an optimized version to not lookup the primitive type reference again.
        /// </remarks>
        internal static void ValidateIsExpectedPrimitiveType(object value, IEdmPrimitiveTypeReference valuePrimitiveTypeReference, IEdmTypeReference expectedTypeReference)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(value != null, "value != null");
            Debug.Assert(expectedTypeReference != null, "expectedTypeReference != null");

            if (valuePrimitiveTypeReference == null)
            {
                throw new ODataException(Strings.ValidationUtils_UnsupportedPrimitiveType(value.GetType().FullName));
            }

            Debug.Assert(valuePrimitiveTypeReference.IsEquivalentTo(EdmLibraryExtensions.GetPrimitiveTypeReference(value.GetType())), "The value and valuePrimitiveTypeReference don't match.");
            if (!expectedTypeReference.IsODataPrimitiveTypeKind())
            {
                // non-primitive type found for primitive value.
                throw new ODataException(Strings.ValidationUtils_NonPrimitiveTypeForPrimitiveValue(expectedTypeReference.ODataFullName()));
            }

            ValidateMetadataPrimitiveType(expectedTypeReference, valuePrimitiveTypeReference);
        }

        /// <summary>
        /// Validates that the expected primitive type matches the actual primitive type.
        /// </summary>
        /// <param name="expectedTypeReference">The expected type.</param>
        /// <param name="typeReferenceFromValue">The actual type.</param>
        internal static void ValidateMetadataPrimitiveType(IEdmTypeReference expectedTypeReference, IEdmTypeReference typeReferenceFromValue)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(expectedTypeReference != null && expectedTypeReference.IsODataPrimitiveTypeKind(), "expectedTypeReference must be a primitive type.");
            Debug.Assert(typeReferenceFromValue != null && typeReferenceFromValue.IsODataPrimitiveTypeKind(), "typeReferenceFromValue must be a primitive type.");

            IEdmPrimitiveType expectedType = (IEdmPrimitiveType)expectedTypeReference.Definition;
            IEdmPrimitiveType typeFromValue = (IEdmPrimitiveType)typeReferenceFromValue.Definition;

            // The two primitive types match if they have the same definition and either both or only the 
            // expected type is nullable
            // NOTE: for strings and binary values we must not check nullability here because the type reference 
            //       from the value is always nullable since C# has no way to express non-nullable strings.
            //       However, this codepath is only hit if the value is not 'null' so we can assign a non-null 
            //       value to both nullable and non-nullable string/binary types.
            bool nullableCompatible = expectedTypeReference.IsNullable == typeReferenceFromValue.IsNullable ||
                expectedTypeReference.IsNullable && !typeReferenceFromValue.IsNullable ||
                !MetadataUtilsCommon.IsODataValueType(typeReferenceFromValue);

            bool typeCompatible = expectedType.IsAssignableFrom(typeFromValue);
            if (!nullableCompatible || !typeCompatible)
            {
                // incompatible type name for value!
                throw new ODataException(Strings.ValidationUtils_IncompatiblePrimitiveItemType(
                    typeReferenceFromValue.ODataFullName(),
                    typeReferenceFromValue.IsNullable,
                    expectedTypeReference.ODataFullName(),
                    expectedTypeReference.IsNullable));
            }
        }

        /// <summary>
        /// Validates a resource collection.
        /// </summary>
        /// <param name="collectionInfo">The resource collection to validate.</param>
        internal static void ValidateResourceCollectionInfo(ODataResourceCollectionInfo collectionInfo)
        {
            DebugUtils.CheckNoExternalCallers();

            if (collectionInfo == null)
            {
                throw new ODataException(Strings.ValidationUtils_WorkspaceCollectionsMustNotContainNullItem);
            }

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
                if (typeName == null)
                {
                    throw new ODataException(Strings.ValidationUtils_IncorrectTypeKindNoTypeName(actualTypeKind.ToString(), expectedTypeKind.ToString()));
                }

                throw new ODataException(Strings.ValidationUtils_IncorrectTypeKind(typeName, expectedTypeKind.ToString(), actualTypeKind.ToString()));
            }
        }

        /// <summary>
        /// Validates that a boundary delimiter is valid (non-null, less than 70 chars, only valid chars, etc.)
        /// </summary>
        /// <param name="boundary">The boundary delimiter to test.</param>
        internal static void ValidateBoundaryString(string boundary)
        {
            DebugUtils.CheckNoExternalCallers();

            // Boundary string must have at least 1 and no more than 70 characters.
            if (boundary == null || boundary.Length == 0 || boundary.Length > MaxBoundaryLength)
            {
                throw new ODataException(Strings.ValidationUtils_InvalidBatchBoundaryDelimiterLength(boundary, MaxBoundaryLength));
            }

            //// NOTE: we do not have to check the validity of the characters in the boundary string
            ////       since we check their validity when reading the boundary parameter value of the Content-Type header.
            ////       See HttpUtils.ReadQuotedParameterValue.
        }

        /// <summary>
        /// Null validation of complex properties will be skipped if edm version is less than v3 and data service version exists.
        /// In such cases, the provider decides what should be done if a null value is stored on a non-nullable complex property.
        /// </summary>
        /// <param name="model">The model containing the complex property.</param>
        /// <returns>True if complex property should be validated for null values.</returns>
        internal static bool ShouldValidateComplexPropertyNullValue(IEdmModel model)
        {
            DebugUtils.CheckNoExternalCallers();

            Debug.Assert(model != null, "For null validation model is required.");
            Debug.Assert(model.IsUserModel(), "For complex properties, the model should be user model.");

            Version edmVersion = model.GetEdmVersion();
            Version dataServiceVersion = model.GetDataServiceVersion();

            if (edmVersion != null && dataServiceVersion != null && edmVersion < ODataVersion.V3.ToDataServiceVersion())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates that a property name is valid in OData.
        /// </summary>
        /// <param name="propertyName">The property name to validate.</param>
        /// <returns>true if the property name is valid, otherwise false.</returns>
        internal static bool IsValidPropertyName(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "The ATOM or JSON reader should have verified that the property name is not null or empty.");

            return propertyName.IndexOfAny(ValidationUtils.InvalidCharactersInPropertyNames) < 0;
        }

        /// <summary>
        /// Validates a property name to check whether it contains reserved characters.
        /// </summary>
        /// <param name="propertyName">The property name to check.</param>
        internal static void ValidatePropertyName(string propertyName)
        {
            DebugUtils.CheckNoExternalCallers();
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");

            if (!IsValidPropertyName(propertyName))
            {
                string invalidChars = string.Join(
                    ", ",
                    ValidationUtils.InvalidCharactersInPropertyNames.Select(c => string.Format(CultureInfo.InvariantCulture, "'{0}'", c)).ToArray());
                throw new ODataException(Strings.ValidationUtils_PropertiesMustNotContainReservedChars(propertyName, invalidChars));
            }
        }

        /// <summary>
        /// Validates that the total number of entity property mapping attributes on the base entity type and the current
        /// entity type does not exceed the specified security limit.
        /// </summary>
        /// <param name="baseCache">The EPM cache of the base entity type or null if no base entity type exists.</param>
        /// <param name="mappings">The EPM collection of the current entity type.</param>
        /// <param name="maxMappingCount">The maximum number of mappings allowed for an entity type (on the type itself and all its base types).</param>
        /// <returns>The total number of entity property mappings for the current entity type.</returns>
        internal static int ValidateTotalEntityPropertyMappingCount(ODataEntityPropertyMappingCache baseCache, ODataEntityPropertyMappingCollection mappings, int maxMappingCount)
        {
            DebugUtils.CheckNoExternalCallers();

            int countOfMappingsOnBaseTypes = baseCache == null ? 0 : baseCache.TotalMappingCount;
            int countOfMappingsOnCurrentType = mappings == null ? 0 : mappings.Count;
            int totalMappingCount = countOfMappingsOnBaseTypes + countOfMappingsOnCurrentType;
            if (totalMappingCount > maxMappingCount)
            {
                throw new ODataException(Strings.ValidationUtils_MaxNumberOfEntityPropertyMappingsExceeded(totalMappingCount, maxMappingCount));
            }

            return totalMappingCount;
        }
    }
}
