//---------------------------------------------------------------------
// <copyright file="ODataPayloadElementExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common
{
    #region Namespaces
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.OData.Common.Annotations;
    #endregion Namespaces

    /// <summary>
    /// Extension methods for the ODataPayloadElement class
    /// </summary>
    public static class ODataPayloadElementExtensions
    {
        /// <summary>
        /// Returns annotation of a payload element.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to get.</typeparam>
        /// <param name="payloadElement">The payload element to get the annotation from.</param>
        /// <returns>The annotation or null if none was found.</returns>
        public static T GetAnnotation<T>(this ODataPayloadElement payloadElement) where T : ODataPayloadElementAnnotation
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            return (T)payloadElement.GetAnnotation(typeof(T));
        }

        /// <summary>
        /// Helper to remove annotations of a certain type
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work with.</typeparam>
        /// <param name="payloadElement">payload from which to remove annotations</param>
        /// <param name="annotationType">the annotationType to remove</param>
        /// <returns>The <paramref name="payloadElement"/> after the annotations were removed.</returns>
        public static T RemoveAnnotations<T>(this T payloadElement, Type annotationType) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(annotationType, "annotationType");

            var annotationsToRemove = payloadElement.Annotations.Where(annotation => annotation.GetType() == annotationType).ToList();
            foreach (ODataPayloadElementAnnotation annotationToRemove in annotationsToRemove)
            {
                payloadElement.Annotations.Remove(annotationToRemove);
            }

            return payloadElement;
        }

        /// <summary>
        /// Adds annotation to the payload element.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to work with.</typeparam>
        /// <param name="payloadElement">The payload element to add the annotation to.</param>
        /// <param name="annotation">The annotation to add.</param>
        /// <returns>The <paramref name="payloadElement"/> after the annotation was added.</returns>
        public static T AddAnnotation<T>(this T payloadElement, ODataPayloadElementAnnotation annotation) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            payloadElement.Annotations.Add(annotation);
            return payloadElement;
        }

        /// <summary>
        /// Remove all existing annotations of type <typeparamref name="T"/> and set <paramref name="annotation"/>.
        /// </summary>
        /// <typeparam name="T">The type of the annotation to set.</typeparam>
        /// <param name="payloadElement">The payload element to set the annotation on.</param>
        /// <param name="annotation">The annotation to set.</param>
        /// <returns>The <paramref name="payloadElement"/> with the <paramref name="annotation"/> set.</returns>
        public static T SetAnnotation<T>(this T payloadElement, ODataPayloadElementAnnotation annotation) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(annotation, "annotation");

            payloadElement.RemoveAnnotations(annotation.GetType());
            payloadElement.AddAnnotation(annotation);
            return payloadElement;
        }

        /// <summary>
        /// Adds the annotation of type TElement in the <paramref name="annotatable"/> (if it exists) to the <paramref name="payloadElement"/>.
        /// </summary>
        /// <typeparam name="TElement">The type of the payload element to work with.</typeparam>
        /// <param name="payloadElement">The payload element to add the annotation to.</param>
        /// <param name="annotatable">The annotatable that potentially holds the annotation to add.</param>
        /// <returns>The <paramref name="payloadElement"/> after the annotation was added.</returns>
        public static TElement CopyAnnotation<TElement, TAnnotation>(this TElement payloadElement, IAnnotatable<ODataPayloadElementAnnotation> annotatable)
            where TElement : ODataPayloadElement
            where TAnnotation : ODataPayloadElementAnnotation
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(annotatable, "annotatable");

            TAnnotation annotation = (TAnnotation)annotatable.GetAnnotation(typeof(TAnnotation));
            if (annotation != null)
            {
                payloadElement.AddAnnotation(annotation);
            }

            return payloadElement;
        }

        /// <summary>
        /// Sets an annotation on the payload element to indicate that entity set information should be ignored during reading.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to add the annotation to.</typeparam>
        /// <param name="payloadElement">The payload element to add the annotation to.</param>
        /// <returns>The <paramref name="payloadElement"/> for composability reasons.</returns>
        public static T IgnoreEntitySet<T>(this T payloadElement) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            return payloadElement.SetAnnotation(new IgnoreEntitySetAnnotation());
        }

        /// <summary>
        /// Creates a deep copy of the specified payload element.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to copy.</typeparam>
        /// <param name="payloadElement">The payload element to copy.</param>
        /// <returns>A copy of the <paramref name="payloadElement"/> with all its children copied as well.</returns>
        public static T DeepCopy<T>(this T payloadElement) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            ODataPayloadElementDeepCopyingVisitor visitor = new ODataPayloadElementDeepCopyingVisitor();
            return (T)visitor.DeepCopy(payloadElement);
        }
        
        /// <summary>
        /// Sets the expected entity type for the top-level entity.
        /// </summary>
        /// <param name="entityInstance">The entity instance to set the expected type for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="entityType">The entity type to set as the expected entity type.</param>
        /// <returns>The <paramref name="entityInstance"/> after its expected type was set.</returns>
        public static EntityInstance ExpectedEntityType(this EntityInstance entityInstance, EntitySet entitySet, EntityDataType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityInstance, "entityInstance");
            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(entityInstance);
            entityType = entityType ?? (entitySet == null ? null : DataTypes.EntityType.WithDefinition(entitySet.EntityType));
            annotation.ExpectedType = entityType;
            annotation.EntitySet = entitySet;
            return entityInstance;
        }

        /// <summary>
        /// Sets the expected base entity type for the top-level entity set.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance to set the expected set for.</param>
        /// <param name="baseEntityType">The base entity type to set as the expected base entity type.</param>
        /// <returns>The <paramref name="entitySetInstance"/> after its expected type was set.</returns>
        public static EntitySetInstance ExpectedEntityType(this EntitySetInstance entitySetInstance, EntityType baseEntityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetInstance, "entitySetInstance");
            return entitySetInstance.ExpectedEntityType(/*entitySet*/null, baseEntityType);
        }

        /// <summary>
        /// Sets the expected base entity type for the top-level entity set.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance to set the expected set for.</param>
        /// <param name="baseEntityType">The base entity type to set as the expected base entity type.</param>
        /// <returns>The <paramref name="entitySetInstance"/> after its expected type was set.</returns>
        public static EntitySetInstance ExpectedEntityType(this EntitySetInstance entitySetInstance, EntityDataType baseEntityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetInstance, "entitySetInstance");
            return entitySetInstance.ExpectedEntityType(/*entitySet*/null, baseEntityType);
        }

        /// <summary>
        /// Sets the expected base entity type for the top-level entity set.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance to set the expected set for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="baseEntityType">The base entity type to set as the expected base entity type.</param>
        /// <returns>The <paramref name="entitySetInstance"/> after its expected type was set.</returns>
        public static EntitySetInstance ExpectedEntityType(this EntitySetInstance entitySetInstance, EntitySet entitySet, EntityType baseEntityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetInstance, "entitySetInstance");
            return entitySetInstance.ExpectedEntityType(entitySet, DataTypes.EntityType.WithDefinition(baseEntityType));
        }

        /// <summary>
        /// Sets the expected base entity type for the top-level entity set.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance to set the expected set for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="baseEntityType">The base entity type to set as the expected base entity type.</param>
        /// <returns>The <paramref name="entitySetInstance"/> after its expected type was set.</returns>
        public static EntitySetInstance ExpectedEntityType(this EntitySetInstance entitySetInstance, EntitySet entitySet, EntityDataType baseEntityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetInstance, "entitySetInstance");
            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(entitySetInstance);
            EntityDataType entityType = baseEntityType ?? (entitySet == null ? null : DataTypes.EntityType.WithDefinition(entitySet.EntityType));
            annotation.ExpectedType = entityType;
            annotation.EntitySet = entitySet;
            return entitySetInstance;
        }

        /// <summary>
        /// Sets the expected entity type for the top-level entity.
        /// </summary>
        /// <param name="entityInstance">The entity instance to set the expected type for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="entityType">The entity type to set as the expected entity type.</param>
        /// <returns>The <paramref name="entityInstance"/> after its expected type was set.</returns>
        public static EntityInstance ExpectedEntityType(this EntityInstance entityInstance, IEdmType entityType, IEdmEntitySet entitySet = null, bool nullable = false)
        {
            return entityInstance.ExpectedEntityType(entityType.ToTypeReference(nullable), entitySet);
        }

        /// <summary>
        /// Sets the expected entity type for the top-level entity.
        /// </summary>
        /// <param name="entityInstance">The entity instance to set the expected type for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="entityType">The entity type to set as the expected entity type.</param>
        /// <returns>The <paramref name="entityInstance"/> after its expected type was set.</returns>
        public static EntityInstance ExpectedEntityType(this EntityInstance entityInstance, IEdmTypeReference entityType, IEdmEntitySet entitySet = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityInstance, "entityInstance");
            ExpectedTypeODataPayloadElementAnnotation annotation = ODataPayloadElementExtensions.AddExpectedTypeAnnotation(entityInstance);
            entityType = entityType ?? (entitySet == null ? null : entitySet.EntityType().ToTypeReference());
            annotation.EdmExpectedType = entityType;
            annotation.EdmEntitySet = entitySet;
            return entityInstance;
        }

        /// <summary>
        /// Sets the expected base entity type for the top-level entity set.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance to set the expected set for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="baseEntityType">The base entity type to set as the expected base entity type.</param>
        /// <returns>The <paramref name="entitySetInstance"/> after its expected type was set.</returns>
        public static EntitySetInstance ExpectedEntityType(this EntitySetInstance entitySetInstance, IEdmType baseEntityType, IEdmEntitySet entitySet = null, bool nullable = false)
        {
            return entitySetInstance.ExpectedEntityType(baseEntityType.ToTypeReference(nullable), entitySet);
        }

        /// <summary>
        /// Sets the expected base entity type for the top-level entity set.
        /// </summary>
        /// <param name="entitySetInstance">The entity set instance to set the expected set for.</param>
        /// <param name="entitySet">The entity set the entities belong to.</param>
        /// <param name="baseEntityType">The base entity type to set as the expected base entity type.</param>
        /// <returns>The <paramref name="entitySetInstance"/> after its expected type was set.</returns>
        public static EntitySetInstance ExpectedEntityType(this EntitySetInstance entitySetInstance, IEdmTypeReference baseEntityType, IEdmEntitySet entitySet = null)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySetInstance, "entitySetInstance");
            ExpectedTypeODataPayloadElementAnnotation annotation = ODataPayloadElementExtensions.AddExpectedTypeAnnotation(entitySetInstance);
            var entityType = baseEntityType ?? (entitySet == null ? null : entitySet.EntityType().ToTypeReference());
            annotation.EdmExpectedType = entityType;
            annotation.EdmEntitySet = entitySet;
            return entitySetInstance;
        }

        /// <summary>
        /// Sets the expected property type for the top-level property.
        /// </summary>
        /// <param name="property">The property instance to set the expected type for.</param>
        /// <param name="dataType">The type to set as the expected property type.</param>
        /// <returns>The <paramref name="property"/> after its expected type was set.</returns>
        public static PropertyInstance ExpectedPropertyType(this PropertyInstance property, IEdmTypeReference dataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ODataPayloadElementExtensions.AddExpectedTypeAnnotation(property).EdmExpectedType = dataType;
            return property;
        }

        /// <summary>
        /// Sets the expected property type for the top-level property.
        /// </summary>
        /// <param name="property">The property instance to set the expected type for.</param>
        /// <param name="dataType">The type to set as the expected property type.</param>
        /// <returns>The <paramref name="property"/> after its expected type was set.</returns>
        public static PropertyInstance ExpectedPropertyType(this PropertyInstance property, IEdmType dataType, bool nullable = false)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ODataPayloadElementExtensions.AddExpectedTypeAnnotation(property).EdmExpectedType = dataType.ToTypeReference(nullable);
            return property;
        }
        /// <summary>
        /// Sets the expected property for the top-level property instance.
        /// </summary>
        /// <param name="property">The property instance to set the expected property for.</param>
        /// <param name="owningType">The type owning the expected property.</param>
        /// <param name="expectedPropertyName">The name of the property to set as the expected property.</param>
        /// <returns>The <paramref name="property"/> after its expected property was set.</returns>
        public static PropertyInstance ExpectedProperty(
            this PropertyInstance property,
            IEdmStructuredType owningType,
            string expectedPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ExceptionUtilities.CheckArgumentNotNull(owningType, "owningType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(expectedPropertyName, "expectedPropertyName");

            ExpectedTypeODataPayloadElementAnnotation annotation = ODataPayloadElementExtensions.AddExpectedTypeAnnotation(property);
            annotation.EdmOwningType = owningType;
            var memberProperty = owningType.FindProperty(expectedPropertyName);
            if (memberProperty != null)
            {
                annotation.EdmProperty = memberProperty;
                annotation.EdmExpectedType = memberProperty.Type;
            }
            else
            {
                ExceptionUtilities.Assert(owningType.IsOpen, "For non-declared properties the owning type must be open.");
                annotation.OpenMemberPropertyName = expectedPropertyName;
            }

            return property;
        }

        /// <summary>
        /// Sets the expected property type for the top-level property.
        /// </summary>
        /// <param name="property">The property instance to set the expected type for.</param>
        /// <param name="dataType">The type to set as the expected property type.</param>
        /// <returns>The <paramref name="property"/> after its expected type was set.</returns>
        public static PropertyInstance ExpectedPropertyType(this PropertyInstance property, DataType dataType)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            AddExpectedTypeAnnotation(property).ExpectedType = dataType;
            return property;
        }

        /// <summary>
        /// Sets the expected property type for the top-level property.
        /// </summary>
        /// <param name="property">The property instance to set the expected type for.</param>
        /// <param name="type">The type to set as the expected property type.</param>
        /// <returns>The <paramref name="property"/> after its expected type was set.</returns>
        public static PropertyInstance ExpectedPropertyType(this PropertyInstance property, EdmEntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            AddExpectedTypeAnnotation(property).EdmExpectedType = type.ToTypeReference();
            return property;
        }

        /// <summary>
        /// Sets the expected property for the top-level property instance.
        /// </summary>
        /// <param name="property">The property instance to set the expected property for.</param>
        /// <param name="owningType">The type owning the expected property.</param>
        /// <param name="expectedPropertyName">The name of the property to set as the expected property.</param>
        /// <returns>The <paramref name="property"/> after its expected property was set.</returns>
        public static PropertyInstance ExpectedProperty(
            this PropertyInstance property,
            NamedStructuralType owningType,
            string expectedPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            ExceptionUtilities.CheckArgumentNotNull(owningType, "owningType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(expectedPropertyName, "expectedPropertyName");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(property);
            annotation.OwningType = owningType;
            MemberProperty memberProperty = owningType.GetProperty(expectedPropertyName);
            if (memberProperty != null)
            {
                annotation.MemberProperty = memberProperty;
                annotation.ExpectedType = memberProperty.PropertyType;
            }
            else
            {
                ExceptionUtilities.Assert(owningType.IsOpen, "For non-declared properties the owning type must be open.");
                annotation.OpenMemberPropertyName = expectedPropertyName;
            }

            return property;
        }
        /// <summary>
        /// Sets the expected navigation property for a top-level entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link instance to set the expected navigation property for.</param>
        /// <param name="owningType">The type owning the expected property.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to set as the expected property.</param>
        /// <returns>The <paramref name="entityReferenceLink"/> after its expected property was set.</returns>
        public static DeferredLink ExpectedNavigationProperty(
            this DeferredLink entityReferenceLink,
            IEdmEntitySet entitySet,
            IEdmEntityType owningType,
            string navigationPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityReferenceLink, "entityReferenceLink");
            ExceptionUtilities.CheckArgumentNotNull(owningType, "owningType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(entityReferenceLink);
            annotation.EdmEntitySet = entitySet;
            annotation.EdmOwningType = owningType;
            annotation.EdmNavigationProperty = owningType.FindProperty(navigationPropertyName);
            return entityReferenceLink;
        }

        /// <summary>
        /// Sets the expected navigation property for a top-level entity reference link.
        /// </summary>
        /// <param name="entityReferenceLink">The entity reference link instance to set the expected navigation property for.</param>
        /// <param name="owningType">The type owning the expected property.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to set as the expected property.</param>
        /// <returns>The <paramref name="entityReferenceLink"/> after its expected property was set.</returns>
        public static DeferredLink ExpectedNavigationProperty(
            this DeferredLink entityReferenceLink,
            EntitySet entitySet,
            EntityType owningType,
            string navigationPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityReferenceLink, "entityReferenceLink");
            ExceptionUtilities.CheckArgumentNotNull(owningType, "owningType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(entityReferenceLink);
            annotation.EntitySet = entitySet;
            annotation.OwningType = owningType;
            annotation.NavigationProperty = owningType.GetNavigationProperty(navigationPropertyName);
            return entityReferenceLink;
        }

        /// <summary>
        /// Sets the expected navigation property for a top-level entity reference links collection.
        /// </summary>
        /// <param name="entityReferenceLinks">The entity reference links collection to set the expected navigation property for.</param>
        /// <param name="owningType">The type owning the expected property.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to set as the expected property.</param>
        /// <returns>The <paramref name="entityReferenceLinks"/> after its expected property was set.</returns>
        public static LinkCollection ExpectedNavigationProperty(
            this LinkCollection entityReferenceLinks,
            EdmEntitySet entitySet,
            EdmEntityType owningType,
            string navigationPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityReferenceLinks, "entityReferenceLinks");
            ExceptionUtilities.CheckArgumentNotNull(owningType, "owningType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(navigationPropertyName, "navigationProperty");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(entityReferenceLinks);
            annotation.EdmEntitySet = entitySet;
            annotation.EdmOwningType = owningType;
            annotation.EdmNavigationProperty = owningType.FindProperty(navigationPropertyName);
            return entityReferenceLinks;
        }

        /// <summary>
        /// Sets the expected navigation property for a top-level entity reference links collection.
        /// </summary>
        /// <param name="entityReferenceLinks">The entity reference links collection to set the expected navigation property for.</param>
        /// <param name="owningType">The type owning the expected property.</param>
        /// <param name="navigationPropertyName">The name of the navigation property to set as the expected property.</param>
        /// <returns>The <paramref name="entityReferenceLinks"/> after its expected property was set.</returns>
        public static LinkCollection ExpectedNavigationProperty(
            this LinkCollection entityReferenceLinks,
            EntitySet entitySet,
            EntityType owningType,
            string navigationPropertyName)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityReferenceLinks, "entityReferenceLinks");
            ExceptionUtilities.CheckArgumentNotNull(owningType, "owningType");
            ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(navigationPropertyName, "navigationProperty");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(entityReferenceLinks);
            annotation.EntitySet = entitySet;
            annotation.OwningType = owningType;
            annotation.NavigationProperty = owningType.GetNavigationProperty(navigationPropertyName);
            return entityReferenceLinks;
        }

        /// <summary>
        /// Sets the expected function import for a top-level payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to set the expected function import for.</param>
        /// <param name="functionImport">The function import to set.</param>
        /// <returns>The <paramref name="payloadElement"/> after its expected function import was set.</returns>
        public static T ExpectedFunctionImport<T>(
            this T payloadElement,
            FunctionImport functionImport) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(payloadElement);
            annotation.FunctionImport = functionImport;
            return payloadElement;
        }

        /// <summary>
        /// Sets the expected function import for a top-level payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to set the expected function import for.</param>
        /// <param name="functionImport">The function import to set.</param>
        /// <returns>The <paramref name="payloadElement"/> after its expected function import was set.</returns>
        public static T ExpectedFunctionImport<T>(
            this T payloadElement,
            EdmOperationImport functionImport) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");

            ExpectedTypeODataPayloadElementAnnotation annotation = AddExpectedTypeAnnotation(payloadElement);
            annotation.ProductFunctionImport = functionImport;
            return payloadElement;
        }

        /// <summary>
        /// Sets the expected item type for the top-level collection.
        /// </summary>
        /// <typeparam name="T">The acutal collection type, i.e., a primitive or a complex collection type.</typeparam>
        /// <param name="collection">The collection instance to set the expected item type for.</param>
        /// <param name="dataType">The type to set as the expected item type.</param>
        /// <returns>The <paramref name="collection"/> after its expected type was set.</returns>
        public static T ExpectedCollectionItemType<T>(this T collection, IEdmType dataType, bool nullable = true) where T : ODataPayloadElementCollection
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            AddExpectedTypeAnnotation(collection).EdmExpectedType = dataType.ToTypeReference(nullable);
            return collection;
        }

        /// <summary>
        /// Sets the expected item type for the top-level collection.
        /// </summary>
        /// <typeparam name="T">The acutal collection type, i.e., a primitive or a complex collection type.</typeparam>
        /// <param name="collection">The collection instance to set the expected item type for.</param>
        /// <param name="dataType">The type to set as the expected item type.</param>
        /// <returns>The <paramref name="collection"/> after its expected type was set.</returns>
        public static T ExpectedCollectionItemType<T>(this T collection, IEdmTypeReference dataType) where T : ODataPayloadElementCollection
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            AddExpectedTypeAnnotation(collection).EdmExpectedType = dataType;
            return collection;
        }

        /// <summary>
        /// Sets the expected item type for the top-level collection.
        /// </summary>
        /// <typeparam name="T">The acutal collection type, i.e., a primitive or a complex collection type.</typeparam>
        /// <param name="collection">The collection instance to set the expected item type for.</param>
        /// <param name="dataType">The type to set as the expected item type.</param>
        /// <returns>The <paramref name="collection"/> after its expected type was set.</returns>
        public static T ExpectedCollectionItemType<T>(this T collection, DataType dataType) where T : ODataPayloadElementCollection
        {
            ExceptionUtilities.CheckArgumentNotNull(collection, "collection");
            AddExpectedTypeAnnotation(collection).ExpectedType = dataType;
            return collection;
        }

        /// <summary>
        /// Sets the expected type for a primitive value.
        /// </summary>
        /// <typeparam name="T">The acutal primitive type.</typeparam>
        /// <param name="primitiveValue">The primitive value to set the expected type for.</param>
        /// <param name="primitiveDataType">The primitive type to set as the expected type.</param>
        /// <returns>The <paramref name="primitiveValue"/> after its expected type was set.</returns>
        public static T ExpectedPrimitiveValueType<T>(this T primitiveValue, IEdmPrimitiveTypeReference primitiveDataType) where T : PrimitiveValue
        {
            ExceptionUtilities.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            ODataPayloadElementExtensions.AddExpectedTypeAnnotation(primitiveValue).EdmExpectedType = primitiveDataType;
            return primitiveValue;
        }


        /// <summary>
        /// Sets the expected type for a primitive value.
        /// </summary>
        /// <typeparam name="T">The acutal primitive type.</typeparam>
        /// <param name="primitiveValue">The primitive value to set the expected type for.</param>
        /// <param name="primitiveDataType">The primitive type to set as the expected type.</param>
        /// <returns>The <paramref name="primitiveValue"/> after its expected type was set.</returns>
        public static T ExpectedPrimitiveValueType<T>(this T primitiveValue, PrimitiveDataType primitiveDataType) where T : PrimitiveValue
        {
            ExceptionUtilities.CheckArgumentNotNull(primitiveValue, "primitiveValue");
            AddExpectedTypeAnnotation(primitiveValue).ExpectedType = primitiveDataType;
            return primitiveValue;
        }

        /// <summary>
        /// Gets or adds the expected type annotation.
        /// </summary>
        /// <param name="payloadElement">The payload element to get or add the annotation to.</param>
        /// <returns>The expected type annotation to use.</returns>
        public static ExpectedTypeODataPayloadElementAnnotation AddExpectedTypeAnnotation(this ODataPayloadElement payloadElement)
        {
            Debug.Assert(payloadElement != null, "payloadElement != null");

            var expectedTypeAnnotation = payloadElement.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();
            if (expectedTypeAnnotation == null)
            {
                expectedTypeAnnotation = new ExpectedTypeODataPayloadElementAnnotation();
                payloadElement.SetAnnotation(expectedTypeAnnotation);
            }

            return expectedTypeAnnotation;
        }

        /// <summary>
        /// Gets or adds the expected type annotation.
        /// </summary>
        /// <param name="payloadElement">The payload element to get or add the annotation to.</param>
        /// <returns>The expected type annotation to use.</returns>
        public static ExpectedTypeODataPayloadElementAnnotation AddExpectedTypeAnnotation(this ODataPayloadElement payloadElement, IEdmType expectedType)
        {
            Debug.Assert(payloadElement != null, "payloadElement != null");

            var expectedTypeAnnotation = payloadElement.GetAnnotation<ExpectedTypeODataPayloadElementAnnotation>();
            if (expectedTypeAnnotation == null)
            {
                expectedTypeAnnotation = new ExpectedTypeODataPayloadElementAnnotation();
                payloadElement.SetAnnotation(expectedTypeAnnotation);
            }

            expectedTypeAnnotation.EdmExpectedType = expectedType.ToTypeReference();
            return expectedTypeAnnotation;
        }

        /// <summary>
        /// Sets an expected type annotation.
        /// </summary>
        /// <param name="payloadElement">The payload element to set the annotation on.</param>
        /// <param name="annotation">The <see cref="ExpectedTypeODataPayloadElementAnnotation"/> to add to <paramref name="payloadElement"/>.</param>
        /// <returns>The annotated <paramref name="payloadElement"/> for composability.</returns>
        public static ODataPayloadElement AddExpectedTypeAnnotation(this ODataPayloadElement payloadElement, ExpectedTypeODataPayloadElementAnnotation annotation)
        {
            Debug.Assert(payloadElement != null, "payloadElement != null");
            payloadElement.SetAnnotation(annotation);
            return payloadElement;
        }

        /// <summary>
        /// Adds the name of the encoding to be used for serialization to the payload element.
        /// </summary>
        /// <param name="payloadElement">The payload element to add the encoding name to.</param>
        /// <param name="encodingName">The name of the encoding to be used for serialization.</param>
        /// <param name="omitDeclaration">true to omit the Xml declaration during serialization; otherwise false.</param>
        /// <returns>The <paramref name="payloadElement"/> with the encoding name annotation added.</returns>
        public static ODataPayloadElement SerializationEncoding(
            this ODataPayloadElement payloadElement,
            string encodingName,
            bool omitDeclaration)
        {
            return payloadElement.AddAnnotation(
                new SerializationEncodingNameAnnotation
                {
                    EncodingName = encodingName,
                    OmitDeclaration = omitDeclaration,
                });
        }

        /// <summary>
        /// Sets an annotation on the (top-level) payload element to indicate the content type to be used for the message.
        /// </summary>
        /// <typeparam name="T">The type of the payload element to add the annotation to.</typeparam>
        /// <param name="payloadElement">The payload element to add the annotation to.</param>
        /// <returns>The <paramref name="payloadElement"/> for composability reasons.</returns>
        public static T WithContentType<T>(this T payloadElement, string contentType) where T : ODataPayloadElement
        {
            ExceptionUtilities.CheckArgumentNotNull(payloadElement, "payloadElement");
            ExceptionUtilities.CheckArgumentNotNull(contentType, "contentType");
            return payloadElement.SetAnnotation(new CustomContentTypeHeaderAnnotation(contentType));
        }
    }
}
