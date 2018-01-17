//---------------------------------------------------------------------
// <copyright file="IWriterValidator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Writer validator interface.
    /// </summary>
    internal interface IWriterValidator
    {
        /// <summary>
        /// Creates a DuplicatePropertyNameChecker instance.
        /// </summary>
        /// <returns>The created instance.</returns>
        IDuplicatePropertyNameChecker CreateDuplicatePropertyNameChecker();

        /// <summary>
        /// Validates a resource in an expanded link to make sure that the types match.
        /// </summary>
        /// <param name="resourceType">The <see cref="IEdmStructuredType"/> of the resource.</param>
        /// <param name="parentNavigationPropertyType">The type of the parent navigation property or
        /// complex property or complex collection property.</param>
        void ValidateResourceInNestedResourceInfo(IEdmStructuredType resourceType,
                                                  IEdmStructuredType parentNavigationPropertyType);

        /// <summary>
        /// Validates that the specified nested resource info has cardinality set, i.e., it has the
        /// IsCollection value set.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to validate.</param>
        void ValidateNestedResourceInfoHasCardinality(ODataNestedResourceInfo nestedResourceInfo);

        /// <summary>
        /// Validates that an open property value is supported.
        /// </summary>
        /// <param name="propertyName">The name of the open property.</param>
        /// <param name="value">The value of the open property.</param>
        void ValidateOpenPropertyValue(string propertyName, object value);

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
        void ValidateIsExpectedPrimitiveType(object value,
                                             IEdmPrimitiveTypeReference valuePrimitiveTypeReference,
                                             IEdmTypeReference expectedTypeReference);

        /// <summary>
        /// Validates the value type reference against metadata.
        /// </summary>
        /// <param name="typeReferenceFromMetadata">The metadata type reference.</param>
        /// <param name="typeReferenceFromValue">The value type reference.</param>
        void ValidateTypeReference(IEdmTypeReference typeReferenceFromMetadata,
                                   IEdmTypeReference typeReferenceFromValue);

        /// <summary>
        /// Validates that the observed type kind is the expected type kind.
        /// </summary>
        /// <param name="actualTypeKind">The actual type kind.</param>
        /// <param name="expectedTypeKind">The expected type kind.</param>
        /// <param name="expectStructuredType">This value indicates if the <paramref name="actualTypeKind"/> is expected to be complex or entity.
        /// True for complex or entity, false for non-structured type kind, null for indetermination.</param>
        /// <param name="edmType">The edm type to use in the error.</param>
        void ValidateTypeKind(EdmTypeKind actualTypeKind,
            EdmTypeKind expectedTypeKind,
            bool? expectStructuredType,
            IEdmType edmType);

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
        void ValidateMetadataResource(ODataResourceBase resource, IEdmEntityType resourceType);

        /// <summary>
        /// Validates that the expected property allows null value.
        /// </summary>
        /// <param name="expectedPropertyTypeReference">The expected property type or null if we
        /// don't have any.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="model">The model used to get the OData version.</param>
        void ValidateNullPropertyValue(IEdmTypeReference expectedPropertyTypeReference,
                                       string propertyName, IEdmModel model);

        /// <summary>
        /// Validates a null collection item against the expected type.
        /// </summary>
        /// <param name="expectedItemType">The expected item type or null if none.</param>
        void ValidateNullCollectionItem(IEdmTypeReference expectedItemType);

        /// <summary>
        /// Validates that a property with the specified name exists on a given structured type.
        /// The structured type can be null if no metadata is available.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="owningStructuredType">Hosting type of the property or null if no metadata is
        /// available.</param>
        /// <returns>An <see cref="IEdmProperty"/> instance representing the specified property or
        /// null if no metadata is available.</returns>
        IEdmProperty ValidatePropertyDefined(string propertyName,
                                             IEdmStructuredType owningStructuredType);

        /// <summary>
        /// Validates an <see cref="ODataNestedResourceInfo"/> to ensure all required information is specified and valid.
        /// </summary>
        /// <param name="nestedResourceInfo">The nested resource info to validate.</param>
        /// <param name="declaringStructuredType">The <see cref="IEdmStructuredType"/> declaring the structural property or navigation property; or null if metadata is not available.</param>
        /// <param name="expandedPayloadKind">The <see cref="ODataPayloadKind"/> of the expanded content of this nested resource info or null for deferred links.</param>
        /// <returns>The type of the navigation property for this nested resource info; or null if no <paramref name="declaringStructuredType"/> was specified.</returns>
        IEdmNavigationProperty ValidateNestedResourceInfo(
            ODataNestedResourceInfo nestedResourceInfo,
            IEdmStructuredType declaringStructuredType,
            ODataPayloadKind? expandedPayloadKind);
    }
}
