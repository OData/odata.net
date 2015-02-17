//---------------------------------------------------------------------
// <copyright file="AstoriaMetadataExtensions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Execution;

    /// <summary>
    /// Named annotation that contains a specific scalar value
    /// </summary>
    public static class AstoriaMetadataExtensions
    {
        /// <summary>
        /// Determines whether an action is bound to an action or not
        /// </summary>
        /// <param name="function">Action to be looked at</param>
        /// <returns>true if its bound to an open type</returns>
        public static bool IsActionBoundOnOpenType(this Function function)
        {
            bool isOpenType = false;
            ExceptionUtilities.Assert(function.IsAction(), "Expected function to be an action");
            var serviceOperationAnnotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            if (serviceOperationAnnotation != null)
            {
                if (serviceOperationAnnotation.BindingKind.IsBound())
                {
                    var initialBindingDataType = function.Parameters.First().DataType;
                    EntityDataType entityDataType = initialBindingDataType as EntityDataType;
                    var collectionDataType = initialBindingDataType as CollectionDataType;
                    if (collectionDataType != null)
                    {
                        entityDataType = collectionDataType.ElementDataType as EntityDataType;
                    }

                    if (entityDataType != null)
                    {
                        isOpenType = entityDataType.Definition.IsOpen;
                    }
                }
            }

            return isOpenType;
        }

        /// <summary>
        /// Gets the edm type name for the data type
        /// </summary>
        /// <param name="dataType">data type to build name of</param>
        /// <returns>Edm type name</returns>
        public static string GetEdmTypeName(this DataType dataType)
        {
            return EdmDataTypes.GetEdmFullName(dataType);
        }

        /// <summary>
        /// Gets whether the annotation is serializable or not
        /// </summary>
        /// <param name="annotation">Annotation to review</param>
        /// <returns>True if its not serializable</returns>
        public static bool IsNotSerializable(this Annotation annotation)
        {
            ExceptionUtilities.CheckArgumentNotNull(annotation, "annotation");
            if (annotation is CompositeAnnotation)
            {
                return false;
            }
            else if (annotation is ICustomAnnotationSerializer)
            {
                return false;
            }
            else if (annotation is TagAnnotation)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if a function is action or not
        /// </summary>
        /// <param name="function">Function to review</param>
        /// <returns>True if its a action</returns>
        public static bool IsAction(this Function function)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            var annotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            if (annotation != null)
            {
                return annotation.IsAction;
            }

            return false;
        }

        /// <summary>
        /// Determines if a function is a function or not
        /// </summary>
        /// <param name="function">Function to review</param>
        /// <returns>True if its a function</returns>
        public static bool IsFunction(this Function function)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            var annotation = function.Annotations.OfType<ServiceOperationAnnotation>().SingleOrDefault();
            if (annotation != null)
            {
                return !annotation.IsAction;
            }

            return false;
        }

        /// <summary>
        /// Determines if a function is a service operation or not
        /// </summary>
        /// <param name="function">Function to review</param>
        /// <returns>True if its a function</returns>
        public static bool IsServiceOperation(this Function function)
        {
            ExceptionUtilities.CheckArgumentNotNull(function, "function");
            return function.Annotations.OfType<LegacyServiceOperationAnnotation>().Any();
        }

        /// <summary>
        /// Gets the DataServiceBehavior
        /// </summary>
        /// <param name="entityContainer">EntityContainer to find DataServiceBehaviorAnnotation on</param>
        /// <returns>DataServiceBehaviorAnnotation information</returns>
        public static DataServiceBehaviorAnnotation GetDataServiceBehavior(this EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");
            List<DataServiceBehaviorAnnotation> dataServiceBehaviors = entityContainer.Annotations.OfType<DataServiceBehaviorAnnotation>().ToList();
            ExceptionUtilities.Assert(dataServiceBehaviors.Count < 2, "There should never be more than one DataServiceBehaviorAnnotation per EntityContainer");
            return dataServiceBehaviors.FirstOrDefault();
        }

        /// <summary>
        /// Gets the DataServiceConfiguration
        /// </summary>
        /// <param name="entityContainer">EntityContainer to find DataServiceConfigurationAnnotation on</param>
        /// <returns>DataServiceConfiguration information</returns>
        public static DataServiceConfigurationAnnotation GetDataServiceConfiguration(this EntityContainer entityContainer)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityContainer, "entityContainer");
            List<DataServiceConfigurationAnnotation> dataServiceConfigurations = entityContainer.Annotations.OfType<DataServiceConfigurationAnnotation>().ToList();
            ExceptionUtilities.Assert(dataServiceConfigurations.Count < 2, "There should never be more than one dataServiceConfigurationAnnotations per EntityContainer");
            return dataServiceConfigurations.FirstOrDefault();
        }

        /// <summary>
        /// Gets and return the single EntityContainer         
        /// If cannot figure it out throw
        /// </summary>
        /// <param name="schema">Schema to look for the EntityContainer</param>
        /// <returns>Default EntityContainer</returns>
        public static EntityContainer GetDefaultEntityContainer(this EntityModelSchema schema)
        {
            ExceptionUtilities.CheckArgumentNotNull(schema, "schema");
            EntityContainer container = schema.EntityContainers.SingleOrDefault();
            ExceptionUtilities.Assert(container != null, "There is no EntityContainer or more than one Containers in this schema");
            return container;
        }

        /// <summary>
        /// Determines whether the EntityType is a lazily loaded
        /// </summary>
        /// <param name="entityType">Entity type</param>
        /// <returns>true or false</returns>
        public static bool IsLazyLoadType(this StructuralType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            return DoesAnnotationExist<LazyLoadedTypeAnnotation>(entityType);
        }

        /// <summary>
        /// Determines whether the navigationProperty type is declared in the metadata
        /// </summary>
        /// <param name="navigationProperty">Structural type</param>
        /// <returns>true or false</returns>
        public static bool IsMetadataDeclaredProperty(this NavigationProperty navigationProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(navigationProperty, "navigationProperty");
            return DoesAnnotationExist<MetadataDeclaredPropertyAnnotation>(navigationProperty);
        }

        /// <summary>
        /// Determines whether the property is specified in the metadata or not backed 
        /// </summary>
        /// <param name="memberProperty">Member Property</param>
        /// <returns>true or false</returns>
        public static bool IsMetadataDeclaredProperty(this MemberProperty memberProperty)
        {
            ExceptionUtilities.CheckArgumentNotNull(memberProperty, "memberProperty");
            return DoesAnnotationExist<MetadataDeclaredPropertyAnnotation>(memberProperty);
        }

        /// <summary>
        /// Marks the named structural type as being declared in metadata
        /// </summary>
        /// <param name="namedStructuralType">Named Structural type</param>
        public static void MakeLazyLoadedType(this NamedStructuralType namedStructuralType)
        {
            if (!namedStructuralType.Annotations.OfType<LazyLoadedTypeAnnotation>().Any())
            {
                namedStructuralType.Annotations.Add(new LazyLoadedTypeAnnotation());
            }
        }

        /// <summary>
        /// Marks the named structural type as being declared in metadata
        /// </summary>
        /// <param name="namedStructuralType">Named Structural type</param>
        public static void MakeMetadataDeclared(this NamedStructuralType namedStructuralType)
        {
            if (!namedStructuralType.Annotations.OfType<MetadataDeclaredPropertyAnnotation>().Any())
            {
                namedStructuralType.Annotations.Add(new MetadataDeclaredPropertyAnnotation());
            }
        }

        /// <summary>
        /// Marks the member property as being declared in metadata
        /// </summary>
        /// <param name="memberProperty">The member property.</param>
        public static void MakeMetadataDeclared(this MemberProperty memberProperty)
        {
            if (!memberProperty.IsMetadataDeclaredProperty())
            {
                memberProperty.Annotations.Add(new MetadataDeclaredPropertyAnnotation());
            }
        }

        /// <summary>
        /// Marks the member property as being declared in metadata
        /// </summary>
        /// <param name="navigationProperty">The navigation property.</param>
        public static void MakeMetadataDeclared(this NavigationProperty navigationProperty)
        {
            if (!navigationProperty.IsMetadataDeclaredProperty())
            {
                navigationProperty.Annotations.Add(new MetadataDeclaredPropertyAnnotation());
            }
        }

        /// <summary>
        /// Makes all required properties metadata declared.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        public static void MakeAllRequiredPropertiesMetadataDeclared(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            entityType.NavigationProperties.ForEach(p => p.MakeMetadataDeclared());

            if (entityType.IsOpen)
            {
                // mark keys, etags, multivalues, and streams to be declared in metadata
                // note, because of derived types, don't use AllProperties etc
                entityType.Properties.Where(p => p.MustBeDeclaredInMetadata()).ForEach(p => p.MakeMetadataDeclared());
            }
            else
            {
                // mark all properties to be declared
                entityType.Properties.ForEach(p => p.MakeMetadataDeclared());
            }
        }

        /// <summary>
        /// Determines whether a EntityType, ComplexType, NavigationProperty, or MemberProperty are backed by a CLR type on the server
        /// </summary>
        /// <param name="annotatedItem">Annotated Item</param>
        /// <returns>true or false</returns>
        public static bool IsTypeBacked(this AnnotatedItem annotatedItem)
        {
            ExceptionUtilities.CheckArgumentNotNull(annotatedItem, "annotatedItem");
            return DoesAnnotationExist<TypeBackedAnnotation>(annotatedItem);
        }

        /// <summary>
        /// Marks the given item as being type-backed.
        /// </summary>
        /// <param name="annotatedItem">The annotated item</param>
        public static void MakeTypeBacked(this AnnotatedItem annotatedItem)
        {
            if (!annotatedItem.IsTypeBacked())
            {
                annotatedItem.Annotations.Add(new TypeBackedAnnotation());
            }
        }

        /// <summary>
        /// Determines whether a EntityType, has a stream on it
        /// </summary>
        /// <param name="entityType">Entity Type</param>
        /// <returns>true or false</returns>
        public static bool HasStream(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            return DoesAnnotationExist<HasStreamAnnotation>(entityType);
        }

        /// <summary>
        /// Determines whether a EntityType, has a stream on it
        /// </summary>
        /// <param name="entityType">Entity Type</param>
        /// <returns>true or false</returns>
        public static bool HasNamedStreams(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");
            return entityType.Properties.Any(p => p.IsStream());
        }

        /// <summary>
        /// Returns a Guid determining if there are repeats
        /// </summary>
        /// <param name="property">Member Property</param>
        /// <returns>Guid used to find variable name</returns>
        public static Guid? GetDictionaryProviderInstanceLinkGuid(this MemberProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            DictionaryResourcePropertyInstanceLink annotation = property.Annotations.OfType<DictionaryResourcePropertyInstanceLink>().FirstOrDefault();
            if (annotation != null)
            {
                return annotation.UniqueId;
            }

            return null;
        }

        /// <summary>
        /// Reviews a specific association set to see if it needs to be declared in the metadata
        /// </summary>
        /// <param name="associationSet">Association set to review</param>
        /// <returns>True or false on if this is declared or not</returns>
        public static bool IsMetadataDeclaredAssociationSet(this AssociationSet associationSet)
        {
            ExceptionUtilities.CheckArgumentNotNull(associationSet, "associationSet");

            // if the navigation is declared in either direction, the set must be declared
            NavigationProperty property0 = associationSet.AssociationType.Ends[0].FromNavigationProperty();
            NavigationProperty property1 = associationSet.AssociationType.Ends[1].FromNavigationProperty();

            if (property0 != null && property0.IsMetadataDeclaredProperty())
            {
                return true;
            }

            if (property1 != null && property1.IsMetadataDeclaredProperty())
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an annotation to the given collection property to indicate that it should use the given information for the collection contract type
        /// </summary>
        /// <param name="property">The property to annotate</param>
        /// <param name="typeName">The full type name to use for the collection type</param>
        /// <param name="isGeneric">Whether or not the collection type is generic</param>
        /// <returns>The given property</returns>
        public static MemberProperty WithCollectionContractType(this MemberProperty property, string typeName, bool isGeneric)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            property.Annotations.Add(new CollectionContractTypeAnnotation() { FullTypeName = typeName, IsGeneric = isGeneric });
            return property;
        }

        /// <summary>
        /// Adds an annotation to the given collection property to indicate that it should use the given information for the collection instance type
        /// </summary>
        /// <param name="property">The property to annotate</param>
        /// <param name="typeName">The full type name to use for the collection type</param>
        /// <param name="isGeneric">Whether or not the collection type is generic</param>
        /// <returns>The given property</returns>
        public static MemberProperty WithCollectionInstanceType(this MemberProperty property, string typeName, bool isGeneric)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            property.Annotations.Add(new CollectionInstanceTypeAnnotation() { FullTypeName = typeName, IsGeneric = isGeneric });
            return property;
        }

        /// <summary>
        /// Returns the entity set related to the given set via the given navigation property
        /// </summary>
        /// <param name="entitySet">The given set</param>
        /// <param name="property">The given navigation property</param>
        /// <returns>The entity set exposed by the given navigation property</returns>
        public static EntitySet GetRelatedEntitySet(this EntitySet entitySet, NavigationProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckArgumentNotNull(property, "property");

            var container = entitySet.Container;
            ExceptionUtilities.CheckObjectNotNull(container, "Entity set did not belong to a container");
            var model = container.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Entity container did not belong to a model");

            var associationType = property.Association;
            var associationSet = model.EntityContainers.SelectMany(c => c.AssociationSets).Single(a => a.Ends.Any(e => e.EntitySet == entitySet) && a.AssociationType == associationType);
            return associationSet.Ends.Single(e => e.AssociationEnd == property.ToAssociationEnd).EntitySet;
        }

        /// <summary>
        /// Returns whether or not the given entity type will have an etag, based on its properties' annotations
        /// </summary>
        /// <param name="type">The type to check</param>
        /// <returns>True if any of the type's properties are marked with the concurreny token annotation, otherwise false</returns>
        public static bool HasETag(this EntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            return type.AllProperties.Any(p => p.DoesAnnotationExist<ConcurrencyTokenAnnotation>());
        }

        /// <summary>
        /// Returns an enumeration of the type's ancestor types and itself
        /// </summary>
        /// <param name="type">The type to get base types of</param>
        /// <returns>An enumeration containing the base types and the given type</returns>
        public static IEnumerable<EntityType> GetBaseTypesAndSelf(this EntityType type)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");

            var list = new List<EntityType> { type };

            while (type.BaseType != null)
            {
                list.Insert(0, type.BaseType);
                type = type.BaseType;
            }

            return list.AsEnumerable();
        }

        /// <summary>
        /// Adds a page size annotation with the given value to the set and returns it
        /// </summary>
        /// <param name="entitySet">The entity set to annotate and return</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The annotated set</returns>
        public static EntitySet WithPageSize(this EntitySet entitySet, int pageSize)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            entitySet.Annotations.Add(new PageSizeAnnotation() { PageSize = pageSize });
            return entitySet;
        }

        /// <summary>
        /// Adds a page size annotation with the given value to the container and returns it. This will be applied universally.
        /// </summary>
        /// <param name="container">The entity container to annotate and return</param>
        /// <param name="pageSize">The page size</param>
        /// <returns>The annotated container</returns>
        public static EntityContainer WithUniversalPageSize(this EntityContainer container, int pageSize)
        {
            ExceptionUtilities.CheckArgumentNotNull(container, "container");
            container.Annotations.Add(new PageSizeAnnotation() { PageSize = pageSize });
            return container;
        }

        /// <summary>
        /// Returns the set's page size limit or null if it is unlimited
        /// </summary>
        /// <param name="entitySet">The entity set</param>
        /// <returns>The page size for the entity set or null if the set has no limit</returns>
        public static int? GetEffectivePageSize(this EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container, "Entity set did not have its container set");
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container.Model, "Entity set's container did not have its model set");

            var defaultContainer = entitySet.Container.Model.GetDefaultEntityContainer();
            var universalPageSize = defaultContainer.Annotations.OfType<PageSizeAnnotation>().SingleOrDefault();
            var setSpecificPageSize = entitySet.Annotations.OfType<PageSizeAnnotation>().SingleOrDefault();

            // set-specific page size ALWAYS supercedes univeral page size
            int? pageSize = null;
            if (setSpecificPageSize != null)
            {
                pageSize = setSpecificPageSize.PageSize;
            }
            else if (universalPageSize != null)
            {
                pageSize = universalPageSize.PageSize;
            }

            // 0 and int.MaxValue are treated as being un-specified page sizes.
            // because this method is intended to return the 'effective' page size, set the value back to null
            if (pageSize.HasValue && (pageSize.Value == 0 || pageSize.Value == int.MaxValue))
            {
                pageSize = null;
            }

            return pageSize;
        }

        /// <summary>
        /// Returns the set's entity set rights
        /// </summary>
        /// <param name="entitySet">The entity set</param>
        /// <returns>The entity set rights for the entity set</returns>
        public static EntitySetRights? GetEffectiveEntitySetRights(this EntitySet entitySet)
        {
            ExceptionUtilities.CheckArgumentNotNull(entitySet, "entitySet");
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container, "Entity set did not have its container set");
            ExceptionUtilities.CheckObjectNotNull(entitySet.Container.Model, "Entity set's container did not have its model set");

            var defaultContainer = entitySet.Container.Model.GetDefaultEntityContainer();
            var universalEntitySetRights = defaultContainer.Annotations.OfType<EntitySetRightsAnnotation>().SingleOrDefault();
            var setSpecificEntitySetRights = entitySet.Annotations.OfType<EntitySetRightsAnnotation>().SingleOrDefault();

            // set-specific entity set rights ALWAYS supercedes univeral entity set rights
            EntitySetRights? entitySetRights = null;
            if (setSpecificEntitySetRights != null)
            {
                entitySetRights = setSpecificEntitySetRights.Value;
            }
            else if (universalEntitySetRights != null)
            {
                entitySetRights = universalEntitySetRights.Value;
            }

            return entitySetRights;
        }

        /// <summary>
        /// Returns a value indicating whether this item can be queried
        /// </summary>
        /// <param name="annotatedItem">The metadata item to check querying support for </param>
        /// <returns>A value indicating whether this item can be queried</returns>
        public static bool SupportsQuery(this IAnnotatedItem annotatedItem)
        {
            return annotatedItem.SupportsOperation(annotation => annotation.SupportsQuery);
        }

        /// <summary>
        /// Returns a value indicating whether this item can be inserted
        /// </summary>
        /// <param name="annotatedItem">The metadata item to check insert support for </param>
        /// <returns>A value indicating whether this item can be inserted</returns>
        public static bool SupportsInsert(this IAnnotatedItem annotatedItem)
        {
            return annotatedItem.SupportsOperation(annotation => annotation.SupportsInsert);
        }

        /// <summary>
        /// Returns a value indicating whether this item can be updated
        /// </summary>
        /// <param name="annotatedItem">The metadata item to check update support for </param>
        /// <returns>A value indicating whether this item can be updated</returns>
        public static bool SupportsUpdate(this IAnnotatedItem annotatedItem)
        {
            return annotatedItem.SupportsOperation(annotation => annotation.SupportsUpdate);
        }

        /// <summary>
        /// Returns a value indicating whether this item can be deleted
        /// </summary>
        /// <param name="annotatedItem">The metadata item to check delete support for </param>
        /// <returns>A value indicating whether this item can be deleted</returns>
        public static bool SupportsDelete(this IAnnotatedItem annotatedItem)
        {
            return annotatedItem.SupportsOperation(annotation => annotation.SupportsDelete);
        }

        /// <summary>
        /// Get all associations where a given entity type has principle roles
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>associations matching the criteria</returns>
        public static IEnumerable<AssociationType> GetPrincipleAssociations(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var model = entityType.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Cannot get entity model from entity type.");

            var matchingAssociations = new List<AssociationType>();
            foreach (var association in model.Associations.Where(a => a.Ends.Any(e => e.EntityType == entityType)))
            {
                if (association.ReferentialConstraint == null)
                {
                    continue;
                }

                if (association.ReferentialConstraint.PrincipalAssociationEnd.EntityType == entityType)
                {
                    matchingAssociations.Add(association);
                }
            }

            if (entityType.BaseType != null)
            {
                matchingAssociations.AddRange(entityType.BaseType.GetPrincipleAssociations());
            }

            return matchingAssociations.Distinct();
        }

        /// <summary>
        /// get all association ends where entity type are on from-end side.
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>IEnumerable of matched association ends</returns>
        public static IEnumerable<AssociationEnd> FromEndAssociations(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var model = entityType.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Cannot get entity model from entity type.");

            var matchingAssociationEnds = new List<AssociationEnd>();

            // need to check all the associations instead of types' navigational properties because navigation property may be one-way.
            foreach (var association in model.Associations)
            {
                if (association.Ends[0].EntityType == entityType)
                {
                    matchingAssociationEnds.Add(association.Ends[0]);
                }

                if (association.Ends[1].EntityType == entityType)
                {
                    matchingAssociationEnds.Add(association.Ends[1]);
                }
            }

            return matchingAssociationEnds;
        }

        /// <summary>
        /// get all association ends where entity type are on to-end side.
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>IEnumerable of matched association ends</returns>
        public static IEnumerable<AssociationEnd> ToEndAssociations(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var model = entityType.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Cannot get entity model from entity type.");

            var matchingAssociationEnds = new List<AssociationEnd>();
            if (entityType.BaseType != null)
            {
                matchingAssociationEnds.AddRange(entityType.BaseType.ToEndAssociations());
            }

            // need to check all the associations instead of types' navigational properties because navigation property may be one-way.
            foreach (var association in model.Associations)
            {
                if (association.Ends[0].EntityType == entityType)
                {
                    matchingAssociationEnds.Add(association.Ends[1]);
                }

                if (association.Ends[1].EntityType == entityType)
                {
                    matchingAssociationEnds.Add(association.Ends[0]);
                }
            }

            return matchingAssociationEnds;
        }

        /// <summary>
        /// Get all the dependent properties given an association
        /// </summary>
        /// <param name="association">given association</param>
        /// <returns>dependent properties if the association has referential constraint</returns>
        public static IEnumerable<MemberProperty> GetDependentProperties(this AssociationType association)
        {
            ExceptionUtilities.CheckArgumentNotNull(association, "association");

            if (association.ReferentialConstraint == null)
            {
                return new List<MemberProperty>();
            }

            return association.ReferentialConstraint.DependentProperties;
        }

        /// <summary>
        /// Get all the associations in the model which have the same dependent properties as the given one
        /// </summary>
        /// <param name="association">given associations</param>
        /// <returns>all the matching associations. If count is large than 1, it indicates there are foreign keys participating in more than one relationship</returns>
        public static IEnumerable<AssociationType> GetAssociationsWithSameDependentProperty(this AssociationType association)
        {
            ExceptionUtilities.CheckArgumentNotNull(association, "association");

            var model = association.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Cannot get entity model from association.");

            var matchedAssociations = new List<AssociationType>();
            if (association.ReferentialConstraint == null)
            {
                return matchedAssociations;
            }

            foreach (var property in association.ReferentialConstraint.DependentProperties)
            {
                matchedAssociations.AddRange(model.Associations.Where(a => a.ReferentialConstraint != null && a.Ends.Any(e => e.EntityType == association.ReferentialConstraint.DependentAssociationEnd.EntityType)
                                                    && a.ReferentialConstraint.DependentProperties.Contains(property)));
            }

            return matchedAssociations.Distinct();
        }

        /// <summary>
        /// check if an entity type has dependent properties
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>true if it has dependent foreign keys</returns>
        public static IEnumerable<MemberProperty> GetDependentProperties(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var model = entityType.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Cannot get entity model from entity type.");

            var dependentProperties = new List<MemberProperty>();

            foreach (var association in model.Associations.Where(a => a.Ends.Any(e => e.EntityType == entityType)))
            {
                if (association.ReferentialConstraint == null)
                {
                    continue;
                }

                if (association.ReferentialConstraint.DependentAssociationEnd.EntityType != entityType)
                {
                    continue;
                }

                // if entity type contains any property that is dependent property (foreign key), we cannot PUT any instance of entity type.
                // We can do PATCH but PUT since PUT will set default values for all the properties we don't specify.
                dependentProperties.AddRange(association.ReferentialConstraint.DependentProperties);
            }

            if (entityType.BaseType != null)
            {
                dependentProperties.AddRange(entityType.BaseType.GetDependentProperties());
            }

            return dependentProperties.Distinct();
        }

        /// <summary>
        /// Get all properties which participate in referential constraints of given entity type
        /// </summary>
        /// <param name="entityType">entity type</param>
        /// <returns>list of associated keys of given entity type</returns>
        public static IEnumerable<MemberProperty> GetAllAssociatedKeys(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            var model = entityType.Model;
            ExceptionUtilities.CheckObjectNotNull(model, "Cannot get entity model from entity type.");

            var foreignKeys = new List<MemberProperty>();

            foreach (var association in model.Associations.Where(a => a.ReferentialConstraint != null))
            {
                if (association.ReferentialConstraint.DependentAssociationEnd.EntityType == entityType)
                {
                    foreignKeys.AddRange(association.ReferentialConstraint.DependentProperties);
                }

                if (association.ReferentialConstraint.PrincipalAssociationEnd.EntityType == entityType)
                {
                    foreignKeys.AddRange(association.ReferentialConstraint.PrincipalProperties);
                }
            }

            if (entityType.BaseType != null)
            {
                foreignKeys = foreignKeys.Union(GetAllAssociatedKeys(entityType.BaseType)).ToList();
            }

            return foreignKeys;
        }

        /// <summary>
        /// Determines whether a EntityType, has a MultiValue on it
        /// </summary>
        /// <param name="entityType">Entity Type</param>
        /// <param name="searchComplexTypes"> Determines whether to search recursively or not for a MultiValue property</param>
        /// <returns>true or false</returns>        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool HasMultiValue(this EntityType entityType, bool searchComplexTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            return HasMultiValue(entityType.AllProperties, searchComplexTypes);
        }

        /// <summary>
        /// Determines whether a EntityType, has a MultiValue on it
        /// </summary>
        /// <param name="entityType">Entity Type</param>
        /// <returns>true or false</returns>
        public static bool HasSpatialProperties(this EntityType entityType)
        {
            ExceptionUtilities.CheckArgumentNotNull(entityType, "entityType");

            return entityType.AllProperties.Any(p => p.PropertyType.IsSpatial());
        }

        /// <summary>
        /// Determines whether a ComplexType, has a MultiValue on it
        /// </summary>
        /// <param name="complexType">Complex Type</param>
        /// <param name="searchComplexTypes"> Determines whether to search recursively or not for a MultiValue property</param>
        /// <returns>true or false</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        public static bool HasMultiValue(this ComplexType complexType, bool searchComplexTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(complexType, "complexType");

            return HasMultiValue(complexType.Properties, searchComplexTypes);
        }

        /// <summary>
        /// Returns whether or not the given model has any spatial properties or allows spatial literals
        /// </summary>
        /// <param name="model">The model to check</param>
        /// <returns>True if any spatial features are present in the model</returns>
        public static bool HasSpatialFeatures(this EntityModelSchema model)
        {
            var behavior = model.GetDefaultEntityContainer().GetDataServiceBehavior();
            if (behavior.AcceptSpatialLiteralsInQuery != false)
            {
                return true;
            }

            // because of the spatial-specific open type method for distance, the test code requires a
            // reference to the spatial types whenever an open type is present in the model
            if (model.EntityTypes.Any(t => t.IsOpen))
            {
                return true;
            }

            var allDataTypes = model.EntityTypes.Cast<NamedStructuralType>()
                .Concat(model.ComplexTypes.Cast<NamedStructuralType>())
                .SelectMany(nst => nst.Properties).Select(p => p.PropertyType)
                .Concat(model.Functions.Select(f => f.ReturnType))
                .Concat(model.Functions.SelectMany(f => f.Parameters).Select(p => p.DataType))
                .ToList();

            return allDataTypes.Any(t => t is SpatialDataType);
        }

        /// <summary>
        /// Returns whether or not the property must be declared in metadata.
        /// </summary>
        /// <param name="property">The member property.</param>
        /// <returns>Whether or not the property must be declared in metadata</returns>
        public static bool MustBeDeclaredInMetadata(this MemberProperty property)
        {
            ExceptionUtilities.CheckArgumentNotNull(property, "property");
            return property.IsPrimaryKey || property.Annotations.OfType<ConcurrencyTokenAnnotation>().Any() || property.PropertyType is CollectionDataType || property.IsStream();
        }

        /// <summary>
        /// Determines whether a list of properties, has a MultiValue on it
        /// </summary>
        /// <param name="properties">Properties to review for MultiValues</param>
        /// <param name="searchComplexTypes"> Determines whether to search recursively or not for a MultiValue property</param>
        /// <returns>true or false</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi",
            Justification = "Matches product API, but is explicitly 'unrecognized' by code-analysis")]
        internal static bool HasMultiValue(IEnumerable<MemberProperty> properties, bool searchComplexTypes)
        {
            ExceptionUtilities.CheckArgumentNotNull(properties, "properties");

            // If there are bag properties then its at least v3
            if (properties.MultiValue().Any())
            {
                return true;
            }

            if (searchComplexTypes)
            {
                List<ComplexType> complexTypesInProperties = properties.Select(p => p.PropertyType).OfType<ComplexDataType>().Select(cdt => cdt.Definition).ToList();
                List<ComplexType> allComplexTypes = complexTypesInProperties.Union(complexTypesInProperties.ToList().SelectMany(ct => ct.AllComplexTypes()).ToList()).ToList();

                if (allComplexTypes.SelectMany(ct => ct.Properties).MultiValue().Any())
                {
                    return true;
                }
            }

            return false;
        }

        private static bool SupportsOperation(this IAnnotatedItem annotatedItem, Func<SupportedOperationsAnnotation, bool> operationCheck)
        {
            var supportedOperationsAnnotation = annotatedItem.Annotations.OfType<SupportedOperationsAnnotation>().SingleOrDefault();
            if (supportedOperationsAnnotation != null)
            {
                return operationCheck(supportedOperationsAnnotation);
            }

            return true;
        }

        private static bool DoesAnnotationExist<TAnnotation>(this AnnotatedItem metadataItem)
        {
            return metadataItem.Annotations.OfType<TAnnotation>().Any();
        }
    }
}
