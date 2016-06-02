//---------------------------------------------------------------------
// <copyright file="WriterValidatorWithoutBasicValidation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Writer validator with no BasicValidation functionality.
    /// </summary>
    internal class WriterValidatorWithoutBasicValidation : IWriterValidator
    {
        /// <summary>
        /// Caches whether ThrowOnNullValuesForNonNullablePrimitiveTypes validation setting is
        /// enabled.
        /// </summary>
        private readonly bool throwOnNullValuesForNonNullablePrimitiveTypes;

        /// <summary>
        /// Caches whether ThrowOnNullValuesForNonNullablePrimitiveTypes validation setting is
        /// enabled.
        /// </summary>
        private readonly bool throwOnUndeclaredProperty;

        /// <summary>
        /// Caches whether ThrowOnDuplicatePropertyNames validation setting is enabled.
        /// </summary>
        private readonly bool throwOnDuplicatePropertyNames;

        /// <summary>
        /// Creates a WriterValidatorWithoutBasicValidation instance.
        /// </summary>
        /// <param name="settings">An ODataMessageWriterSettings instance that contains the
        /// validation settings to use for the created WriterValidatorWithoutBasicValidation
        /// instance.</param>
        internal WriterValidatorWithoutBasicValidation(ODataMessageWriterSettings settings)
        {
            throwOnNullValuesForNonNullablePrimitiveTypes =
                settings.ThrowOnNullValuesForNonNullablePrimitiveTypes;
            throwOnUndeclaredProperty = settings.ThrowOnUndeclaredProperty;
            throwOnDuplicatePropertyNames = settings.ThrowOnDuplicatePropertyNames;
        }

        /// <summary>
        /// Creates a DuplicatePropertyNamesChecker instance.
        /// </summary>
        /// <param name="writeResponse">true if writing a response; false otherwise.</param>
        /// <returns>The created instance.</returns>
        public DuplicatePropertyNamesChecker CreateDuplicatePropertyNamesChecker(bool writeResponse)
        {
            return new DuplicatePropertyNamesChecker(!throwOnDuplicatePropertyNames, writeResponse);
        }

        /// <summary>
        /// Validates a resource in an expanded link to make sure that the types match.
        /// </summary>
        /// <param name="resourceType">The <see cref="IEdmStructuredType"/> of the resource.</param>
        /// <param name="parentNavigationPropertyType">The type of the parent navigation property or
        /// complex property or complex collection property.</param>
        public virtual void ValidateResourceInNestedResourceInfo(
            IEdmStructuredType resourceType,
            IEdmStructuredType parentNavigationPropertyType)
        {
            // nop
        }

        /// <summary>
        /// Validates that the specified nested resource info has cardinality set, i.e., it has the
        /// IsCollection value set.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to validate.</param>
        public virtual void ValidateNestedResourceInfoHasCardinality(
            ODataNestedResourceInfo nestedResourceInfo)
        {
            // nop
        }

        /// <summary>
        /// Validates that an open property value is supported.
        /// </summary>
        /// <param name="propertyName">The name of the open property.</param>
        /// <param name="value">The value of the open property.</param>
        public virtual void ValidateOpenPropertyValue(string propertyName, object value)
        {
            // nop
        }

        /// <summary>
        /// Validates that a given primitive value is of the expected (primitive) type.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="valuePrimitiveTypeReference">The primitive type reference for the value.
        /// So some callers have this already, and we save the lookup here.</param>
        /// <param name="expectedTypeReference">The expected type for the value.</param>
        /// <remarks>
        /// Some callers have the primitive type reference already resolved (from the value type),
        /// and the method will not lookup the primitive type reference again.
        /// </remarks>
        public virtual void ValidateIsExpectedPrimitiveType(
            object value, IEdmPrimitiveTypeReference valuePrimitiveTypeReference,
            IEdmTypeReference expectedTypeReference)
        {
            // nop
        }

        /// <summary>
        /// Validates the value type reference against metadata.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The metadata type reference.</param>
        /// <param name="typeReferenceFromValue">The value type reference.</param>
        public virtual void ValidateTypeReference(IEdmTypeReference typeReferenceFromMetadata,
                                                  IEdmTypeReference typeReferenceFromValue)
        {
            // nop
        }

        /// <summary>
        /// Validates that the observed type kind is the expected type kind.
        /// </summary>
        /// <param name="actualTypeKind">The actual type kind.</param>
        /// <param name="expectedTypeKind">The expected type kind.</param>
        /// <param name="edmType">The edm type to use in the error.</param>
        public virtual void ValidateTypeKind(EdmTypeKind actualTypeKind,
                                             EdmTypeKind expectedTypeKind, IEdmType edmType)
        {
            // nop
        }

        /// <summary>
        /// Validates that the specified <paramref name="resource"/> is a valid resource as per the
        /// specified type.
        /// </summary>
        /// <param name="resource">The resource to validate.</param>
        /// <param name="resourceType">Optional entity type to validate the resource against.</param>
        /// <param name="model">Model containing the entity type.</param>
        /// <remarks>
        /// If the <paramref name="resourceType"/> is available, only resource-level tests are
        /// performed; properties and such are not validated.
        /// </remarks>
        public virtual void ValidateMetadataResource(ODataResource resource,
                                                     IEdmEntityType resourceType, IEdmModel model)
        {
            // nop
        }

        /// <summary>
        /// Validates that the expected property allows null value.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected property type or null if we
        /// don't have any.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="model">The model used to get the OData version.</param>
        public void ValidateNullPropertyValue(IEdmTypeReference expectedPropertyTypeReference,
                                              string propertyName, IEdmModel model)
        {
            Debug.Assert(model != null, "For null validation, model is required.");
            if (expectedPropertyTypeReference != null)
            {
                if (expectedPropertyTypeReference.IsNonEntityCollectionType())
                {
                    throw new ODataException(
                        Strings.WriterValidationUtils_CollectionPropertiesMustNotHaveNullValue(
                            propertyName));
                }

                if (expectedPropertyTypeReference.IsODataPrimitiveTypeKind())
                {
                    if (!expectedPropertyTypeReference.IsNullable
                        && throwOnNullValuesForNonNullablePrimitiveTypes)
                    {
                        throw new ODataException(
                            Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(
                                propertyName, expectedPropertyTypeReference.FullName()));
                    }
                }
                else if (expectedPropertyTypeReference.IsODataEnumTypeKind()
                         && !expectedPropertyTypeReference.IsNullable)
                {
                    throw new ODataException(
                        Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(
                            propertyName, expectedPropertyTypeReference.FullName()));
                }
                else if (expectedPropertyTypeReference.IsStream())
                {
                    throw new ODataException(
                        Strings.WriterValidationUtils_StreamPropertiesMustNotHaveNullValue(
                            propertyName));
                }
                else if (expectedPropertyTypeReference.IsODataComplexTypeKind())
                {
                    IEdmComplexTypeReference complexTypeReference =
                        expectedPropertyTypeReference.AsComplex();
                    if (!complexTypeReference.IsNullable)
                    {
                        throw new ODataException(
                            Strings.WriterValidationUtils_NonNullablePropertiesMustNotHaveNullValue(
                                propertyName, expectedPropertyTypeReference.FullName()));
                    }
                }
            }
        }

        /// <summary>
        /// Validates a null collection item against the expected type.
        /// </summary>
        /// <param name="expectedItemType">The expected item type or null if none.</param>
        public void ValidateNullCollectionItem(IEdmTypeReference expectedItemType)
        {
            if (expectedItemType != null)
            {
                if (expectedItemType.IsODataPrimitiveTypeKind())
                {
                    if (!expectedItemType.IsNullable
                        && throwOnNullValuesForNonNullablePrimitiveTypes)
                    {
                        throw new ODataException(
                            Strings.ValidationUtils_NullCollectionItemForNonNullableType(
                                expectedItemType.FullName()));
                    }
                }
            }
        }

        /// <summary>
        /// Validates that a property with the specified name exists on a given structured type.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="owningStructuredType">Hosting type of the property or null if no metadata is
        /// available.</param>
        /// <returns>An <see cref="IEdmProperty"/> instance representing the specified property or
        /// null if no metadata is available.</returns>
        public IEdmProperty ValidatePropertyDefined(string propertyName,
                                                    IEdmStructuredType owningStructuredType)
        {
            return WriterValidationUtils.ValidatePropertyDefined(propertyName, owningStructuredType,
                                                                 throwOnUndeclaredProperty);
        }

        /// <summary>
        /// Validates that the navigation property with the specified name exists on a given entity
        /// type. The entity type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="owningEntityType">Hosting entity type or null if no metadata is available.
        /// </param>
        /// <returns><see cref="IEdmProperty"/> representing the navigation property with name
        /// <paramref name="propertyName"/> or null if no metadata is available.</returns>
        public IEdmNavigationProperty ValidateNavigationPropertyDefined(
            string propertyName, IEdmEntityType owningEntityType)
        {
            Debug.Assert(!string.IsNullOrEmpty(propertyName), "!string.IsNullOrEmpty(propertyName)");
            if (owningEntityType == null)
            {
                return null;
            }

            IEdmProperty property = ValidatePropertyDefined(propertyName, owningEntityType);
            if (property == null)
            {
                if (!throwOnUndeclaredProperty)
                {
                    return null;
                }

                // We don't support open navigation properties
                Debug.Assert(
                    owningEntityType.IsOpen,
                    "We should have already failed on non-existing property on a closed type.");
                throw new ODataException(
                    Strings.ValidationUtils_OpenNavigationProperty(
                        propertyName, owningEntityType.FullTypeName()));
            }

            if (property.PropertyKind != EdmPropertyKind.Navigation)
            {
                // The property must be a navigation property
                throw new ODataException(
                    Strings.ValidationUtils_NavigationPropertyExpected(
                        propertyName, owningEntityType.FullTypeName(),
                        property.PropertyKind.ToString()));
            }

            return (IEdmNavigationProperty)property;
        }
    }
}
