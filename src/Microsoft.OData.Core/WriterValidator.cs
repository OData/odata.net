//---------------------------------------------------------------------
// <copyright file="WriterValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Writer validator that binds to an ODataMessageWriterSettings instance.
    /// </summary>
    internal class WriterValidator : IWriterValidator
    {
        /// <summary>
        /// References the bound ODataMessageWriterSettings instance.
        /// </summary>
        private readonly ODataMessageWriterSettings settings;

        /// <summary>
        /// Creates a WriterValidator instance and binds it to settings.
        /// </summary>
        /// <param name="settings">The ODataMessageWriterSettings instance to bind to.</param>
        internal WriterValidator(ODataMessageWriterSettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Creates a DuplicatePropertyNameChecker instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        public IDuplicatePropertyNameChecker CreateDuplicatePropertyNameChecker()
        {
            return settings.ThrowOnDuplicatePropertyNames
                   ? (IDuplicatePropertyNameChecker)new DuplicatePropertyNameChecker()
                   : (IDuplicatePropertyNameChecker)new NullDuplicatePropertyNameChecker();
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
            if (settings.ThrowIfTypeConflictsWithMetadata)
            {
                WriterValidationUtils.ValidateNestedResource(
                    resourceType, parentNavigationPropertyType);
            }
        }

        /// <summary>
        /// Validates that the specified nested resource info has cardinality set, i.e., it has the
        /// IsCollection value set.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to validate.</param>
        public virtual void ValidateNestedResourceInfoHasCardinality(
            ODataNestedResourceInfo nestedResourceInfo)
        {
            WriterValidationUtils.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);
        }

        /// <summary>
        /// Validates that an open property value is supported.
        /// </summary>
        /// <param name="propertyName">The name of the open property.</param>
        /// <param name="value">The value of the open property.</param>
        public virtual void ValidateOpenPropertyValue(string propertyName, object value)
        {
            ValidationUtils.ValidateOpenPropertyValue(propertyName, value);
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
            if (settings.ThrowIfTypeConflictsWithMetadata)
            {
                ValidationUtils.ValidateIsExpectedPrimitiveType(
                    value, valuePrimitiveTypeReference, expectedTypeReference);
            }
        }

        /// <summary>
        /// Validates the value type reference against metadata.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The metadata type reference.</param>
        /// <param name="typeReferenceFromValue">The value type reference.</param>
        public virtual void ValidateTypeReference(IEdmTypeReference typeReferenceFromMetadata,
                                                  IEdmTypeReference typeReferenceFromValue)
        {
            if (settings.ThrowIfTypeConflictsWithMetadata)
            {
                // Make sure the types are the same
                if (typeReferenceFromValue.IsODataPrimitiveTypeKind())
                {
                    // Primitive types must match exactly except for nullability
                    ValidationUtils.ValidateMetadataPrimitiveType(typeReferenceFromMetadata,
                                                                  typeReferenceFromValue);
                }
                else if (typeReferenceFromMetadata.IsEntity())
                {
                    ValidationUtils.ValidateEntityTypeIsAssignable(
                        (IEdmEntityTypeReference)typeReferenceFromMetadata,
                        (IEdmEntityTypeReference)typeReferenceFromValue);
                }
                else if (typeReferenceFromMetadata.IsComplex())
                {
                    ValidationUtils.ValidateComplexTypeIsAssignable(
                        typeReferenceFromMetadata.Definition as IEdmComplexType,
                        typeReferenceFromValue.Definition as IEdmComplexType);
                }
                else if (typeReferenceFromMetadata.IsCollection())
                {
                    // Collection types must match exactly.
                    if (!typeReferenceFromMetadata.Definition.IsElementTypeEquivalentTo(
                            typeReferenceFromValue.Definition))
                    {
                        throw new ODataException(
                            Strings.ValidationUtils_IncompatibleType(
                                typeReferenceFromValue.FullName(),
                                typeReferenceFromMetadata.FullName()));
                    }
                }
                else
                {
                    // For other types, compare their full type name.
                    if (typeReferenceFromMetadata.FullName() != typeReferenceFromValue.FullName())
                    {
                        throw new ODataException(
                            Strings.ValidationUtils_IncompatibleType(
                                typeReferenceFromValue.FullName(),
                                typeReferenceFromMetadata.FullName()));
                    }
                }
            }
        }

        /// <summary>
        /// Validates that the observed type kind is the expected type kind.
        /// </summary>
        /// <param name="actualTypeKind">The actual type kind.</param>
        /// <param name="expectedTypeKind">The expected type kind.</param>
        /// <param name="expectStructuredType">This value indicates if the <paramref name="actualTypeKind"/> is expected to be complex or entity.
        /// True for complex or entity, false for non-structured type kind, null for indetermination.</param>
        /// <param name="edmType">The edm type to use in the error.</param>
        /// <remarks>If expectedStructuredType is true, then expectedTypeKind could be </remarks>
        public virtual void ValidateTypeKind(EdmTypeKind actualTypeKind,
            EdmTypeKind expectedTypeKind,
            bool? expectStructuredType,
            IEdmType edmType)
        {
            if (settings.ThrowIfTypeConflictsWithMetadata)
            {
                ValidationUtils.ValidateTypeKind(
                    actualTypeKind, expectedTypeKind, expectStructuredType, edmType == null ? null : edmType.FullTypeName());
            }
        }

        /// <summary>
        /// Validates that the specified <paramref name="resource"/> is a valid resource as per the
        /// specified type.
        /// </summary>
        /// <param name="resource">The resource to validate.</param>
        /// <param name="resourceType">Optional entity type to validate the resource against.</param>
        /// <remarks>
        /// If the <paramref name="resourceType"/> is available, only resource-level tests are
        /// performed; properties and such are not validated.
        /// </remarks>
        public virtual void ValidateMetadataResource(ODataResourceBase resource, IEdmEntityType resourceType)
        {
            ValidationUtils.ValidateMediaResource(resource, resourceType);
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
            if (settings.ThrowIfTypeConflictsWithMetadata)
            {
                WriterValidationUtils.ValidateNullPropertyValue(expectedPropertyTypeReference, propertyName, model);
            }
        }

        /// <summary>
        /// Validates a null collection item against the expected type.
        /// </summary>
        /// <param name="expectedItemType">The expected item type or null if none.</param>
        public void ValidateNullCollectionItem(IEdmTypeReference expectedItemType)
        {
            if (settings.ThrowIfTypeConflictsWithMetadata)
            {
                ValidationUtils.ValidateNullCollectionItem(expectedItemType);
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
            return WriterValidationUtils.ValidatePropertyDefined(
                propertyName, owningStructuredType, settings.ThrowOnUndeclaredPropertyForNonOpenType);
        }

        /// <summary>
        /// Validates an <see cref="ODataNestedResourceInfo"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to validate.</param>
        /// <param name="declaringStructuredType">The <see cref="IEdmStructuredType"/> declaring the structural property or navigation property; or null if metadata is not available.</param>
        /// <param name="expandedPayloadKind">The <see cref="ODataPayloadKind"/> of the expanded content of this nested resource info or null for deferred links.</param>
        /// <returns>The type of the navigation property for this nested resource info; or null if no <paramref name="declaringStructuredType"/> was specified.</returns>
        public IEdmNavigationProperty ValidateNestedResourceInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmStructuredType declaringStructuredType,
            ODataPayloadKind? expandedPayloadKind)
        {
            return WriterValidationUtils.ValidateNestedResourceInfo(nestedResourceInfo, declaringStructuredType, expandedPayloadKind, settings.ThrowOnUndeclaredPropertyForNonOpenType);
        }
    }
}
