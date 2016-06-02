//---------------------------------------------------------------------
// <copyright file="WriterValidatorWithBasicValidation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Metadata;

    /// <summary>
    /// Writer validator with BasicValidation functionality.
    /// </summary>
    internal class WriterValidatorWithBasicValidation : WriterValidatorWithoutBasicValidation
    {
        /// <summary>
        /// Creates a WriterValidatorWithBasicValidation instance.
        /// </summary>
        /// <param name="settings">An ODataMessageWriterSettings instance that contains the
        /// validation settings to use for the created WriterValidatorWithBasicValidation
        /// instance.</param>
        internal WriterValidatorWithBasicValidation(ODataMessageWriterSettings settings)
            : base(settings)
        {
            // nop
        }

        /// <summary>
        /// Validates a resource in an expanded link to make sure that the types match.
        /// </summary>
        /// <param name="resourceType">The <see cref="IEdmStructuredType"/> of the resource.</param>
        /// <param name="parentNavigationPropertyType">The type of the parent navigation property or
        /// complex property or complex collection property.</param>
        public override void ValidateResourceInNestedResourceInfo(
            IEdmStructuredType resourceType,
            IEdmStructuredType parentNavigationPropertyType)
        {
            WriterValidationUtils.ValidateResourceInExpandedLink(resourceType,
                                                                 parentNavigationPropertyType);
        }

        /// <summary>
        /// Validates that the specified nested resource info has cardinality set, i.e., it has the
        /// IsCollection value set.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to validate.</param>
        public override void ValidateNestedResourceInfoHasCardinality(
            ODataNestedResourceInfo nestedResourceInfo)
        {
            WriterValidationUtils.ValidateNestedResourceInfoHasCardinality(nestedResourceInfo);
        }

        /// <summary>
        /// Validates that an open property value is supported.
        /// </summary>
        /// <param name="propertyName">The name of the open property.</param>
        /// <param name="value">The value of the open property.</param>
        public override void ValidateOpenPropertyValue(string propertyName, object value)
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
        public override void ValidateIsExpectedPrimitiveType(
            object value, IEdmPrimitiveTypeReference valuePrimitiveTypeReference,
            IEdmTypeReference expectedTypeReference)
        {
            ValidationUtils.ValidateIsExpectedPrimitiveType(value, valuePrimitiveTypeReference,
                                                            expectedTypeReference);
        }

        /// <summary>
        /// Validates the value type reference against metadata.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The metadata type reference.</param>
        /// <param name="typeReferenceFromValue">The value type reference.</param>
        public override void ValidateTypeReference(IEdmTypeReference typeReferenceFromMetadata,
                                                   IEdmTypeReference typeReferenceFromValue)
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
                if (!(typeReferenceFromMetadata.Definition.IsElementTypeEquivalentTo(
                        typeReferenceFromValue.Definition)))
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

        /// <summary>
        /// Validates that the observed type kind is the expected type kind.
        /// </summary>
        /// <param name="actualTypeKind">The actual type kind.</param>
        /// <param name="expectedTypeKind">The expected type kind.</param>
        /// <param name="edmType">The edm type to use in the error.</param>
        public override void ValidateTypeKind(EdmTypeKind actualTypeKind,
                                              EdmTypeKind expectedTypeKind, IEdmType edmType)
        {
            ValidationUtils.ValidateTypeKind(
                actualTypeKind, expectedTypeKind, edmType == null ? null : edmType.FullTypeName());
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
        public override void ValidateMetadataResource(ODataResource resource,
                                                      IEdmEntityType resourceType, IEdmModel model)
        {
            ValidationUtils.ValidateMediaResource(resource, resourceType, model, true);
        }
    }
}
