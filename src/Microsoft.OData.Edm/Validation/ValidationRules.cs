//---------------------------------------------------------------------
// <copyright file="ValidationRules.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// Built in Edm validation rules.
    /// </summary>
    public static class ValidationRules
    {
        #region IEdmElement

        /// <summary>
        /// Validates that no direct annotations share the same name and namespace.
        /// </summary>
        public static readonly ValidationRule<IEdmElement> ElementDirectValueAnnotationFullNameMustBeUnique =
            new ValidationRule<IEdmElement>(
                (context, item) =>
                {
                    HashSetInternal<string> annotationNameSet = new HashSetInternal<string>();
                    foreach (IEdmDirectValueAnnotation annotation in context.Model.DirectValueAnnotationsManager.GetDirectValueAnnotations(item))
                    {
                        if (!annotationNameSet.Add(annotation.NamespaceUri + ":" + annotation.Name))
                        {
                            context.AddError(
                            annotation.Location(),
                            EdmErrorCode.DuplicateDirectValueAnnotationFullName,
                            Strings.EdmModel_Validator_Semantic_ElementDirectValueAnnotationFullNameMustBeUnique(annotation.NamespaceUri, annotation.Name));
                        }
                    }
                });

        #endregion

        #region IEdmNamedElement

        /// <summary>
        /// Validates that a name is not empty or whitespace.
        /// </summary>
        public static readonly ValidationRule<IEdmNamedElement> NamedElementNameMustNotBeEmptyOrWhiteSpace =
            new ValidationRule<IEdmNamedElement>(
                (context, item) =>
                {
                    if (EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) || item.Name.Length == 0)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidName,
                            Strings.EdmModel_Validator_Syntactic_MissingName);
                    }
                });

        /// <summary>
        /// Validates that an element name is not too long according to the CSDL spec.
        /// </summary>
        public static readonly ValidationRule<IEdmNamedElement> NamedElementNameIsTooLong =
            new ValidationRule<IEdmNamedElement>(
                (context, item) =>
                {
                    if (!EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) && item.Name.Length > CsdlConstants.Max_NameLength)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.NameTooLong,
                            Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(item.Name));
                    }
                });

        /// <summary>
        /// Validates that an element name matches the allowed pattern of names according to the CSDL spec.
        /// </summary>
        public static readonly ValidationRule<IEdmNamedElement> NamedElementNameIsNotAllowed =
            new ValidationRule<IEdmNamedElement>(
                (context, item) =>
                {
                    // Don't run this rule for IEdmDirectValueAnnotation, since these custom annotations may have names which are invalid C# identifiers, but work in XML.
                    // We validate the name of IEdmDirectionValueAnnotation in a separate validation rule, DirectValueAnnotationHasXmlSerializableName.
                    if (item is IEdmDirectValueAnnotation)
                    {
                        return;
                    }

                    if (!EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) && item.Name.Length <= CsdlConstants.Max_NameLength && item.Name.Length > 0)
                    {
                        if (!EdmUtil.IsValidUndottedName(item.Name))
                        {
                            context.AddError(
                                item.Location(),
                                EdmErrorCode.InvalidName,
                                Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(item.Name));
                        }
                    }
                });

        #endregion

        #region IEdmSchemaElement

        /// <summary>
        /// Validates that an element namespace is not empty or whitespace.
        /// </summary>
        public static readonly ValidationRule<IEdmSchemaElement> SchemaElementNamespaceMustNotBeEmptyOrWhiteSpace =
            new ValidationRule<IEdmSchemaElement>(
                (context, item) =>
                {
                    if (EdmUtil.IsNullOrWhiteSpaceInternal(item.Namespace) || item.Namespace.Length == 0)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidNamespaceName,
                            Strings.EdmModel_Validator_Syntactic_MissingNamespaceName);
                    }
                });

        /// <summary>
        /// Validates that an element namespace is not too long according to the CSDL spec.
        /// </summary>
        public static readonly ValidationRule<IEdmSchemaElement> SchemaElementNamespaceIsTooLong =
            new ValidationRule<IEdmSchemaElement>(
                (context, item) =>
                {
                    if (item.Namespace.Length > CsdlConstants.Max_NamespaceLength)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidNamespaceName,
                            Strings.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong(item.Namespace));
                    }
                });

        /// <summary>
        /// Validates that an element namespace matches the allowed pattern of namespaces according to the CSDL spec.
        /// </summary>
        public static readonly ValidationRule<IEdmSchemaElement> SchemaElementNamespaceIsNotAllowed =
            new ValidationRule<IEdmSchemaElement>(
                (context, item) =>
                {
                    // max length is hard coded in the xsd
                    if (item.Namespace.Length <= CsdlConstants.Max_NamespaceLength && item.Namespace.Length > 0 && !EdmUtil.IsNullOrWhiteSpaceInternal(item.Namespace))
                    {
                        if (!EdmUtil.IsValidDottedName(item.Namespace))
                        {
                            context.AddError(
                                item.Location(),
                                EdmErrorCode.InvalidNamespaceName,
                                Strings.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed(item.Namespace));
                        }
                    }
                });

        /// <summary>
        /// Validates that an element namespace is not a reserved system namespace.
        /// </summary>
        public static readonly ValidationRule<IEdmSchemaElement> SchemaElementSystemNamespaceEncountered =
            new ValidationRule<IEdmSchemaElement>(
                (context, element) =>
                {
                    if (ValidationHelper.IsEdmSystemNamespace(element.Namespace))
                    {
                        context.AddError(
                            element.Location(),
                            EdmErrorCode.SystemNamespaceEncountered,
                            Strings.EdmModel_Validator_Semantic_SystemNamespaceEncountered(element.Namespace));
                    }
                });

        /// <summary>
        /// A schema element without other errors must not have kind of none.
        /// </summary>
        public static readonly ValidationRule<IEdmSchemaElement> SchemaElementMustNotHaveKindOfNone =
            new ValidationRule<IEdmSchemaElement>(
                (context, element) =>
                {
                    if (element.SchemaElementKind == EdmSchemaElementKind.None && !context.IsBad(element))
                    {
                        context.AddError(
                        element.Location(),
                        EdmErrorCode.SchemaElementMustNotHaveKindOfNone,
                        Strings.EdmModel_Validator_Semantic_SchemaElementMustNotHaveKindOfNone(element.FullName()));
                    }
                });

        #endregion

        #region IEdmEntityContainerElement

        /// <summary>
        /// An entity container element without other errors must not have kind of none.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityContainerElement> EntityContainerElementMustNotHaveKindOfNone =
            new ValidationRule<IEdmEntityContainerElement>(
                (context, element) =>
                {
                    if (element.ContainerElementKind == EdmContainerElementKind.None && !context.IsBad(element))
                    {
                        context.AddError(
                        element.Location(),
                        EdmErrorCode.EntityContainerElementMustNotHaveKindOfNone,
                        Strings.EdmModel_Validator_Semantic_EntityContainerElementMustNotHaveKindOfNone(element.Container.FullName() + '/' + element.Name));
                    }
                });

        #endregion

        #region IEdmEntityContainer

        /// <summary>
        /// Validates that there are no duplicate names in an entity container.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityContainer> EntityContainerDuplicateEntityContainerMemberName =
            new ValidationRule<IEdmEntityContainer>(
                (context, entityContainer) =>
                {
                    HashSetInternal<string> nonOperationList = new HashSetInternal<string>();
                    HashSetInternal<string> operationImportOperationList = new HashSetInternal<string>();
                    HashSetInternal<string> operationImportList = new HashSetInternal<string>();
                    foreach (var item in entityContainer.Elements)
                    {
                        bool duplicate = false;

                        var operationImport = item as IEdmOperationImport;
                        if (operationImport != null)
                        {
                            if (!operationImportList.Contains(operationImport.Name))
                            {
                                operationImportList.Add(operationImport.Name);
                            }

                            // OperationImports of the same name can exist as long as they reference different operations.
                            string operationImportUniqueString = operationImport.Name + "_" + operationImport.Operation.GetHashCode();
                            if (operationImportOperationList.Contains(operationImportUniqueString))
                            {
                                duplicate = true;
                            }
                            else
                            {
                                operationImportOperationList.Add(operationImportUniqueString);
                            }

                            if (nonOperationList.Contains(operationImport.Name))
                            {
                                duplicate = true;
                            }
                        }
                        else
                        {
                            if (nonOperationList.Contains(item.Name))
                            {
                                duplicate = true;
                            }
                            else
                            {
                                nonOperationList.Add(item.Name);
                            }

                            if (operationImportList.Contains(item.Name))
                            {
                                duplicate = true;
                            }
                        }

                        if (duplicate)
                        {
                            context.AddError(
                                item.Location(),
                                EdmErrorCode.DuplicateEntityContainerMemberName,
                                Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
                        }
                    }
                });

        #endregion

        #region IEdmNavigationSource

        /// <summary>
        /// Validates that there is no entity set or singleton whose entity type has no key.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationSourceTypeHasNoKeys =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    IEdmEntityType entityType = navigationSource.EntityType();

                    if (entityType == null)
                    {
                        return;
                    }

                    if ((navigationSource.EntityType().Key() == null || !navigationSource.EntityType().Key().Any()) && !context.IsBad(navigationSource.EntityType()))
                    {
                        string errorMessage = Strings.EdmModel_Validator_Semantic_NavigationSourceTypeHasNoKeys(
                            navigationSource.Name,
                            navigationSource.EntityType().Name);

                        context.AddError(
                            navigationSource.Location(),
                            EdmErrorCode.NavigationSourceTypeHasNoKeys,
                            errorMessage);
                    }
                });

        /// <summary>
        /// Validates that there is no entity set or singleton whose entity type has not property defined with Path type.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationSourceDeclaringTypeCannotHavePathTypeProperty =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    IEdmEntityType entityType = navigationSource.EntityType();

                    if (entityType == null)
                    {
                        return;
                    }

                    IList<IEdmStructuredType> visited = new List<IEdmStructuredType>();
                    if (HasPathTypeProperty(entityType, visited))
                    {
                        string name = navigationSource is IEdmSingleton ? "singleton" : "entity set";

                        string errorMessage = Strings
                            .EdmModel_Validator_Semantic_DeclaringTypeOfNavigationSourceCannotHavePathProperty(entityType.FullName(), name, navigationSource.Name);

                        context.AddError(
                            navigationSource.Location(),
                            EdmErrorCode.DeclaringTypeOfNavigationSourceCannotHavePathProperty,
                            errorMessage);
                    }
                });

        /// <summary>
        /// Validates that the entity type of an entity set or singleton can be found from the model being validated.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationSourceInaccessibleEntityType =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    IEdmEntityType entityType = navigationSource.EntityType();
                    if (entityType != null && !context.IsBad(entityType))
                    {
                        CheckForUnreacheableTypeError(context, entityType, navigationSource.Location());
                    }
                });

        /// <summary>
        /// Validates that no navigation property is mapped multiple times for a single path.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationPropertyMappingsMustBeUnique =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    var set = new HashSetInternal<KeyValuePair<IEdmNavigationProperty, string>>();
                    foreach (var mapping in navigationSource.NavigationPropertyBindings)
                    {
                        if (!set.Add(new KeyValuePair<IEdmNavigationProperty, string>(mapping.NavigationProperty, mapping.Path.Path)))
                        {
                            // TODO: Update error message in V7.1 #644
                            context.AddError(
                                navigationSource.Location(),
                                EdmErrorCode.DuplicateNavigationPropertyMapping,
                                Strings.EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping(navigationSource.Name, mapping.NavigationProperty.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that the target of a navigation property mapping is valid for the target type of the property.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationPropertyMappingMustPointToValidTargetForProperty =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    foreach (IEdmNavigationPropertyBinding mapping in navigationSource.NavigationPropertyBindings)
                    {
                        if (mapping.NavigationProperty.IsBad() || mapping.Target.IsBad())
                        {
                            continue;
                        }

                        if (!(mapping.Target.EntityType().IsOrInheritsFrom(mapping.NavigationProperty.ToEntityType()) || mapping.NavigationProperty.ToEntityType().IsOrInheritsFrom(mapping.Target.EntityType())) && !context.IsBad(mapping.Target))
                        {
                            context.AddError(
                                navigationSource.Location(),
                                EdmErrorCode.NavigationPropertyMappingMustPointToValidTargetForProperty,
                                Strings.EdmModel_Validator_Semantic_NavigationPropertyMappingMustPointToValidTargetForProperty(mapping.NavigationProperty.Name, mapping.Target.Name));
                        }

                        if (mapping.Target is IEdmSingleton && mapping.NavigationProperty.Type.Definition.TypeKind == EdmTypeKind.Collection)
                        {
                            context.AddError(
                                navigationSource.Location(),
                                EdmErrorCode.NavigationPropertyOfCollectionTypeMustNotTargetToSingleton,
                                Strings.EdmModel_Validator_Semantic_NavigationPropertyOfCollectionTypeMustNotTargetToSingleton(mapping.NavigationProperty.Name, mapping.Target.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that the binding path of navigation property must be resolved to a valid path, that is:
        /// Each segments in path must be defined, and inner path segments can only be complex or containment, and last path segment must be the navigation property name.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationPropertyBindingPathMustBeResolvable =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    foreach (IEdmNavigationPropertyBinding mapping in navigationSource.NavigationPropertyBindings)
                    {
                        if (mapping.NavigationProperty.IsBad() || mapping.Target.IsBad())
                        {
                            continue;
                        }

                        if (!TryResolveNavigationPropertyBindingPath(context.Model, navigationSource, mapping))
                        {
                            // TODO: Update error message in V7.1 #644
                            context.AddError(
                                navigationSource.Location(),
                                EdmErrorCode.UnresolvedNavigationPropertyBindingPath,
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    "The binding path {0} for navigation property {1} under navigation source {2} is not valid.",
                                    mapping.Path.Path,
                                    mapping.NavigationProperty.Name,
                                    navigationSource.Name));
                        }
                    }
                });

        #endregion

        #region IEdmEntitySet

        /// <summary>
        /// Validates that an entity set can only have a single navigation property targeting it that has Contains set to true.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetCanOnlyBeContainedByASingleNavigationProperty =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    bool containmentFound = false;
                    foreach (IEdmNavigationSource navigationSource in set.Container.Elements.OfType<IEdmNavigationSource>())
                    {
                        foreach (IEdmNavigationPropertyBinding binding in navigationSource.NavigationPropertyBindings)
                        {
                            IEdmNavigationProperty property = binding.NavigationProperty;

                            if (binding.Target == set && property.ContainsTarget)
                            {
                                if (containmentFound)
                                {
                                    context.AddError(
                                        set.Location(),
                                        EdmErrorCode.EntitySetCanOnlyBeContainedByASingleNavigationProperty,
                                        Strings.EdmModel_Validator_Semantic_EntitySetCanOnlyBeContainedByASingleNavigationProperty(set.Container.FullName() + "." + set.Name));
                                }

                                containmentFound = true;
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that if a navigation property is traversed to another entity set/singleton, and then the navigation properties partner is traversed, the destination will be the source entity set/singleton.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationSource> NavigationMappingMustBeBidirectional =
            new ValidationRule<IEdmNavigationSource>(
                (context, navigationSource) =>
                {
                    foreach (IEdmNavigationPropertyBinding binding in navigationSource.NavigationPropertyBindings)
                    {
                        IEdmNavigationProperty property = binding.NavigationProperty;
                        if (property.Partner == null || property.IsBad())
                        {
                            continue;
                        }

                        IEdmNavigationSource opposingNavigationSource = binding.Target.FindNavigationTarget(property.Partner, new EdmPathExpression(property.Partner.Name));

                        if (opposingNavigationSource == null || opposingNavigationSource is IEdmUnknownEntitySet || opposingNavigationSource is IEdmContainedEntitySet)
                        {
                            continue;
                        }

                        if (opposingNavigationSource != navigationSource && property.Partner.DeclaringEntityType().FindProperty(property.Partner.Name) == property.Partner)
                        {
                            context.AddError(
                                navigationSource.Location(),
                                EdmErrorCode.NavigationMappingMustBeBidirectional,
                                Strings.EdmModel_Validator_Semantic_NavigationMappingMustBeBidirectional(navigationSource.Name, property.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that if a navigation property mapping is of recursive containment, the mapping points back to the source entity set.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    foreach (IEdmNavigationPropertyBinding mapping in set.NavigationPropertyBindings)
                    {
                        if (mapping.NavigationProperty.ContainsTarget &&
                            mapping.NavigationProperty.DeclaringType.IsOrInheritsFrom(mapping.NavigationProperty.ToEntityType()) &&
                            mapping.Target != set)
                        {
                            context.AddError(
                                set.Location(),
                                EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet,
                                Strings.EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet(mapping.NavigationProperty, set.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that the type of entity set is collection of entity type.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetTypeMustBeCollectionOfEntityType =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    bool isCollectionOfEntityType = false;
                    IEdmCollectionType collectionType = entitySet.Type as IEdmCollectionType;

                    if (collectionType != null)
                    {
                        isCollectionOfEntityType = collectionType.ElementType != null && collectionType.ElementType.Definition is IEdmEntityType;
                    }

                    if (!isCollectionOfEntityType)
                    {
                        string errorMessage = Strings.EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType(entitySet.Type.FullTypeName(), entitySet.Name);

                        context.AddError(
                            entitySet.Location(),
                            EdmErrorCode.EntitySetTypeMustBeCollectionOfEntityType,
                            errorMessage);
                    }
                });

        /// <summary>
        /// Validates that the type of an entity set cannot be Edm.EntityType.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetTypeCannotBeEdmEntityType =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    if (entitySet.Type.AsElementType() == EdmCoreModelEntityType.Instance)
                    {
                        context.AddError(
                           entitySet.Location(),
                           EdmErrorCode.EntityTypeOfEntitySetCannotBeEdmEntityType,
                           Strings.EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfEntitySet(entitySet.Name));
                    }
                });
        #endregion

        #region IEdmSingelton

        /// <summary>
        /// Validates that the type of singleton is entity type.
        /// </summary>
        public static readonly ValidationRule<IEdmSingleton> SingletonTypeMustBeEntityType =
            new ValidationRule<IEdmSingleton>(
                (context, singleton) =>
                {
                    if (!(singleton.Type is IEdmEntityType))
                    {
                        string errorMessage =
                            Strings.EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType(
                                singleton.Type.FullTypeName(), singleton.Name);

                        context.AddError(
                            singleton.Location(),
                            EdmErrorCode.SingletonTypeMustBeEntityType,
                            errorMessage);
                    }
                });

        /// <summary>
        /// Validates that the type of singleton cannot be Edm.EntityType.
        /// </summary>
        public static readonly ValidationRule<IEdmSingleton> SingletonTypeCannotBeEdmEntityType =
            new ValidationRule<IEdmSingleton>(
                (context, singleton) =>
                {
                    if (singleton.Type == EdmCoreModelEntityType.Instance)
                    {
                        context.AddError(
                           singleton.Location(),
                           EdmErrorCode.EntityTypeOfSingletonCannotBeEdmEntityType,
                           Strings.EdmModel_Validator_Semantic_EdmEntityTypeCannotBeTypeOfSingleton(singleton.Name));
                    }
                });
        #endregion

        #region IEdmStructuredType

        /// <summary>
        /// Validates that a type does not have a property with the same name as that type.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> StructuredTypeInvalidMemberNameMatchesTypeName =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    IEdmSchemaType schemaType = structuredType as IEdmSchemaType;
                    if (schemaType != null)
                    {
                        var properties = structuredType.Properties().ToList();
                        if (properties.Count > 0)
                        {
                            foreach (var property in properties)
                            {
                                if (property != null)
                                {
                                    if (property.Name.EqualsOrdinal(schemaType.Name))
                                    {
                                        context.AddError(
                                        property.Location(),
                                        EdmErrorCode.BadProperty,
                                        Strings.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName(property.Name));
                                    }
                                }
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that there are not duplicate properties in a type.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> StructuredTypePropertyNameAlreadyDefined =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    HashSetInternal<string> propertyNames = new HashSetInternal<string>();
                    foreach (var property in structuredType.Properties())
                    {
                        // We only want to report the properties that are declared in this type. Otherwise properties will get reported multiple times due to inheritance.
                        if (property != null)
                        {
                            ValidationHelper.AddMemberNameToHashSet(
                                property,
                                propertyNames,
                                context,
                                EdmErrorCode.AlreadyDefined,
                                Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefined(property.Name),
                                /*supressError*/ !structuredType.DeclaredProperties.Contains(property));
                        }
                    }
                });

        /// <summary>
        /// Validates that the base type of a complex type is complex, and the base type of an entity type is an entity.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> StructuredTypeBaseTypeMustBeSameKindAsDerivedKind =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    // We can either have 2 rules (entity and complex) or have one rule. I'm choosing the latter.
                    if (structuredType is IEdmSchemaType)
                    {
                        if (structuredType.BaseType != null && structuredType.BaseType.TypeKind != structuredType.TypeKind)
                        {
                            context.AddError(
                                structuredType.Location(),
                                (structuredType.TypeKind == EdmTypeKind.Entity) ? EdmErrorCode.EntityMustHaveEntityBaseType : EdmErrorCode.ComplexTypeMustHaveComplexBaseType,
                                Strings.EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind);
                        }
                    }
                });

        /// <summary>
        /// Validates that the base type of a structured type cannot be Edm.EntityType or Edm.ComplexType.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> StructuredTypeBaseTypeCannotBeAbstractType =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    if (structuredType.BaseType != null &&
                        (structuredType.BaseType == EdmCoreModelComplexType.Instance || structuredType.BaseType == EdmCoreModelEntityType.Instance) &&
                        !context.IsBad(structuredType.BaseType))
                    {
                        string typeKind = structuredType.TypeKind == EdmTypeKind.Entity ? "entity" : "complex";
                        context.AddError(
                            structuredType.Location(),
                            (structuredType.TypeKind == EdmTypeKind.Entity)
                                ? EdmErrorCode.EntityTypeBaseTypeCannotBeEdmEntityType
                                : EdmErrorCode.ComplexTypeBaseTypeCannotBeEdmComplexType,
                            Strings.EdmModel_Validator_Semantic_StructuredTypeBaseTypeCannotBeAbstractType(
                                structuredType.BaseType.FullTypeName(), typeKind, structuredType.FullTypeName()));
                    }
                });

        /// <summary>
        /// Validates that the base type of a structured type can be found from the model being validated.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> StructuredTypeInaccessibleBaseType =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    IEdmSchemaType schemaBaseType = structuredType.BaseType as IEdmSchemaType;
                    if (schemaBaseType != null && !context.IsBad(schemaBaseType))
                    {
                        CheckForUnreacheableTypeError(context, schemaBaseType, structuredType.Location());
                    }
                });

        /// <summary>
        /// Validates that the declaring type of a property contains that property.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> StructuredTypePropertiesDeclaringTypeMustBeCorrect =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    foreach (var property in structuredType.DeclaredProperties)
                    {
                        if (property != null)
                        {
                            if (!property.DeclaringType.Equals(structuredType))
                            {
                                context.AddError(
                                property.Location(),
                                EdmErrorCode.DeclaringTypeMustBeCorrect,
                                Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(property.Name));
                            }
                        }
                    }
                });

        #endregion

        #region IEdmEnumType

        /// <summary>
        /// Validates that there are not duplicate enum members in an enum.
        /// </summary>
        public static readonly ValidationRule<IEdmEnumType> EnumTypeEnumMemberNameAlreadyDefined =
            new ValidationRule<IEdmEnumType>(
                (context, enumType) =>
                {
                    HashSetInternal<string> memberNames = new HashSetInternal<string>();
                    foreach (var member in enumType.Members)
                    {
                        // We only want to report the properties that are declared in this type. Otherwise properties will get reported multiple times due to inheritance.
                        if (member != null)
                        {
                            ValidationHelper.AddMemberNameToHashSet(
                                member,
                                memberNames,
                                context,
                                EdmErrorCode.AlreadyDefined,
                                Strings.EdmModel_Validator_Semantic_EnumMemberNameAlreadyDefined(member.Name),
                                /*supressError*/ false);
                        }
                    }
                });

        /// <summary>
        /// Raises an error if the underlying type of an enum type is not an integer type.
        /// </summary>
        public static readonly ValidationRule<IEdmEnumType> EnumMustHaveIntegerUnderlyingType =
           new ValidationRule<IEdmEnumType>(
               (context, enumType) =>
               {
                   if (!enumType.UnderlyingType.PrimitiveKind.IsIntegral() && !context.IsBad(enumType.UnderlyingType))
                   {
                       context.AddError(
                           enumType.Location(),
                           EdmErrorCode.EnumMustHaveIntegerUnderlyingType,
                           Strings.EdmModel_Validator_Semantic_EnumMustHaveIntegralUnderlyingType(enumType.FullName()));
                   }
               });

        /// <summary>
        /// Validates that the underlying type of a type definition cannot be Edm.PrimitiveType.
        /// </summary>
        public static readonly ValidationRule<IEdmEnumType> EnumUnderlyingTypeCannotBeEdmPrimitiveType =
            new ValidationRule<IEdmEnumType>(
                (context, enumType) =>
                {
                    if (enumType.UnderlyingType.PrimitiveKind == EdmPrimitiveTypeKind.PrimitiveType && !context.IsBad(enumType.UnderlyingType))
                    {
                        context.AddError(
                            enumType.Location(),
                            EdmErrorCode.TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType,
                            Strings.EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsUnderlyingType("enumeration", enumType.FullName()));
                    }
                });
        #endregion

        #region IEdmEnumMember

        /// <summary>
        /// Raises an error if the type of an enum member doesn't match the underlying type of the enum it belongs to.
        /// </summary>
        public static readonly ValidationRule<IEdmEnumMember> EnumMemberValueMustHaveSameTypeAsUnderlyingType =
           new ValidationRule<IEdmEnumMember>(
               (context, enumMember) =>
               {
                   IEnumerable<EdmError> discoveredErrors;
                   if (!context.IsBad(enumMember.DeclaringType) &&
                       !context.IsBad(enumMember.DeclaringType.UnderlyingType))
                   {
                       IEdmPrimitiveValue enumValue = new EdmIntegerConstant(enumMember.Value.Value);

                       if (!enumValue.TryCastPrimitiveAsType(enumMember.DeclaringType.UnderlyingType.GetPrimitiveTypeReference(false), out discoveredErrors))
                       {
                           context.AddError(
                           enumMember.Location(),
                           EdmErrorCode.EnumMemberValueOutOfRange,
                           Strings.EdmModel_Validator_Semantic_EnumMemberValueOutOfRange(enumMember.Name));
                       }
                   }
               });

        #endregion

        #region IEdmTypeDefintion

        /// <summary>
        /// Validates that the underlying type of a type definition cannot be Edm.PrimitiveType.
        /// </summary>
        public static readonly ValidationRule<IEdmTypeDefinition> TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType =
            new ValidationRule<IEdmTypeDefinition>(
                (context, typeDefinition) =>
                {
                    if (typeDefinition.UnderlyingType == EdmCoreModel.Instance.GetPrimitiveType() && !context.IsBad(typeDefinition.UnderlyingType))
                    {
                        context.AddError(
                            typeDefinition.Location(),
                            EdmErrorCode.TypeDefinitionUnderlyingTypeCannotBeEdmPrimitiveType,
                            Strings.EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsUnderlyingType("type definition", typeDefinition.FullName()));
                    }
                });
        #endregion

        #region IEdmEntityType

        /// <summary>
        /// Validates that there are not duplicate properties in an entity key.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeDuplicatePropertyNameSpecifiedInEntityKey =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.DeclaredKey != null)
                    {
                        HashSetInternal<string> keyPropertyNameList = new HashSetInternal<string>();
                        foreach (var item in entityType.DeclaredKey)
                        {
                            ValidationHelper.AddMemberNameToHashSet(
                                item,
                                keyPropertyNameList,
                                context,
                                EdmErrorCode.DuplicatePropertySpecifiedInEntityKey,
                                Strings.EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey(entityType.Name, item.Name),
                                /*supressError*/ false);
                        }
                    }
                });

        /// <summary>
        /// Validates that no part of an entity key is nullable.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeInvalidKeyNullablePart =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.Key() != null)
                    {
                        foreach (var key in entityType.Key())
                        {
                            if (key.Type.IsPrimitive())
                            {
                                if (key.Type.IsNullable)
                                {
                                    context.AddError(
                                    key.Location(),
                                    EdmErrorCode.InvalidKey,
                                    Strings.EdmModel_Validator_Semantic_InvalidKeyNullablePart(key.Name, entityType.Name));
                                }
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that all parts of an entity key are scalar.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeEntityKeyMustBeScalar =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.Key() != null)
                    {
                        foreach (IEdmStructuralProperty key in entityType.Key())
                        {
                            if (!key.Type.IsPrimitive() && !key.Type.IsEnum() && !context.IsBad(key))
                            {
                                context.AddError(
                                key.Location(),
                                EdmErrorCode.EntityKeyMustBeScalar,
                                Strings.EdmModel_Validator_Semantic_EntityKeyMustBeScalar(key.Name, entityType.Name));
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that a key is not defined if there is already a key in the base type.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeInvalidKeyKeyDefinedInBaseClass =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.BaseType != null &&
                        entityType.DeclaredKey != null &&
                        entityType.BaseType.TypeKind == EdmTypeKind.Entity &&
                        entityType.BaseEntityType().DeclaredKey != null)
                    {
                        context.AddError(
                        entityType.Location(),
                        EdmErrorCode.InvalidKey,
                        Strings.EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass(entityType.Name, entityType.BaseEntityType().Name));
                    }
                });

        /// <summary>
        /// Validates that the entity type has a key.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeKeyMissingOnEntityType =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    // Abstract entity type can have no key.
                    var keys = entityType.Key();
                    if ((keys == null || !keys.Any()) && entityType.BaseType == null && !entityType.IsAbstract)
                    {
                        context.AddError(
                        entityType.Location(),
                        EdmErrorCode.KeyMissingOnEntityType,
                        Strings.EdmModel_Validator_Semantic_KeyMissingOnEntityType(entityType.Name));
                    }
                });

        /// <summary>
        /// Validates that all properties in the key of an entity blong to that entity.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeKeyPropertyMustBelongToEntity =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.DeclaredKey != null)
                    {
                        foreach (IEdmStructuralProperty key in entityType.DeclaredKey)
                        {
                            // Key must be one of the declared properties.
                            if (key.DeclaringType != entityType && !context.IsBad(key))
                            {
                                context.AddError(
                                entityType.Location(),
                                EdmErrorCode.KeyPropertyMustBelongToEntity,
                                Strings.EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity(key.Name, entityType.Name));
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that Edm.PrimitiveType cannot be used as the type of a key property of an entity type.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeKeyTypeCannotBeEdmPrimitiveType =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.DeclaredKey != null)
                    {
                        foreach (IEdmStructuralProperty key in entityType.DeclaredKey)
                        {
                            if (key.Type.Definition == EdmCoreModel.Instance.GetPrimitiveType())
                            {
                                context.AddError(
                                    entityType.Location(),
                                    EdmErrorCode.KeyPropertyTypeCannotBeEdmPrimitiveType,
                                    Strings.EdmModel_Validator_Semantic_EdmPrimitiveTypeCannotBeUsedAsTypeOfKey(
                                        key.Name, entityType.FullName()));
                            }
                        }
                    }
                });

        #endregion

        #region IEdmEntityReferenceType

        /// <summary>
        /// Validates that the entity type wrapped in this entity reference can be found through the model being validated.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityReferenceType> EntityReferenceTypeInaccessibleEntityType =
            new ValidationRule<IEdmEntityReferenceType>(
                (context, entityReferenceType) =>
                {
                    if (!context.IsBad(entityReferenceType.EntityType))
                    {
                        CheckForUnreacheableTypeError(context, entityReferenceType.EntityType, entityReferenceType.Location());
                    }
                });

        #endregion

        #region IEdmType

        /// <summary>
        /// A type without other errors must not have kind of none.
        /// </summary>
        public static readonly ValidationRule<IEdmType> TypeMustNotHaveKindOfNone =
            new ValidationRule<IEdmType>(
                (context, type) =>
                {
                    if (type.TypeKind == EdmTypeKind.None && !context.IsBad(type))
                    {
                        context.AddError(
                        type.Location(),
                        EdmErrorCode.TypeMustNotHaveKindOfNone,
                        Strings.EdmModel_Validator_Semantic_TypeMustNotHaveKindOfNone);
                    }
                });
        #endregion

        #region IEdmPrimitiveType

        /// <summary>
        /// A primtive type without other errors must not have kind of none.
        /// </summary>
        public static readonly ValidationRule<IEdmPrimitiveType> PrimitiveTypeMustNotHaveKindOfNone =
            new ValidationRule<IEdmPrimitiveType>(
                (context, type) =>
                {
                    if (type.PrimitiveKind == EdmPrimitiveTypeKind.None && !context.IsBad(type))
                    {
                        context.AddError(
                        type.Location(),
                        EdmErrorCode.PrimitiveTypeMustNotHaveKindOfNone,
                        Strings.EdmModel_Validator_Semantic_PrimitiveTypeMustNotHaveKindOfNone(type.FullName()));
                    }
                });

        #endregion

        #region IEdmComplexType

        /// <summary>
        /// Validates that a open complex type can not have closed derived complex type.
        /// </summary>
        public static readonly ValidationRule<IEdmComplexType> OpenComplexTypeCannotHaveClosedDerivedComplexType =
            new ValidationRule<IEdmComplexType>(
                (context, complexType) =>
                {
                    if (complexType.BaseType != null && complexType.BaseType.IsOpen && !complexType.IsOpen)
                    {
                        context.AddError(
                            complexType.Location(),
                            EdmErrorCode.InvalidAbstractComplexType,
                            Strings.EdmModel_Validator_Semantic_BaseTypeOfOpenTypeMustBeOpen(complexType.FullName()));
                    }
                });

        #endregion

        #region IEdmStructuralProperty

        /// <summary>
        /// Validates that the property is of an allowed type.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuralProperty> StructuralPropertyInvalidPropertyType =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    IEdmType validatedType;
                    if (property.Type.IsCollection())
                    {
                        validatedType = property.Type.AsCollection().ElementType().Definition;
                    }
                    else
                    {
                        validatedType = property.Type.Definition;
                    }

                    if (validatedType.TypeKind != EdmTypeKind.Primitive && validatedType.TypeKind != EdmTypeKind.Enum
                        && validatedType.TypeKind != EdmTypeKind.Untyped && validatedType.TypeKind != EdmTypeKind.Complex
                        && validatedType.TypeKind != EdmTypeKind.Path
                        && !context.IsBad(validatedType))
                    {
                        context.AddError(property.Location(), EdmErrorCode.InvalidPropertyType, Strings.EdmModel_Validator_Semantic_InvalidPropertyType(property.Type.TypeKind().ToString()));
                    }
                });

        #endregion

        #region IEdmNavigationProperty

        /// <summary>
        /// Validates that only one end of an association has an OnDelete operation.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyInvalidOperationMultipleEndsInAssociatedNavigationProperties =
           new ValidationRule<IEdmNavigationProperty>(
               (context, navigationProperty) =>
               {
                   if (navigationProperty.OnDelete != EdmOnDeleteAction.None && navigationProperty.Partner != null && navigationProperty.Partner.OnDelete != EdmOnDeleteAction.None)
                   {
                       context.AddError(
                           navigationProperty.Location(),
                           EdmErrorCode.InvalidAction,
                           Strings.EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation);
                   }
               });

        /// <summary>
        /// Validates that the type of a navigation property corresponds to the other end of the association and the multiplicity of the other end.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyCorrectType =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    // Validates that target must be an entity type.
                    if (property.ToEntityType() == null)
                    {
                        context.AddError(
                            property.Location(),
                            EdmErrorCode.InvalidNavigationPropertyType,
                            Strings.EdmModel_Validator_Semantic_InvalidNavigationPropertyType(property.Name));
                        return;
                    }

                    if (property.Partner == null
                        || property.Partner is BadNavigationProperty
                        || property.Partner.DeclaringType is IEdmComplexType)
                    {
                        return;
                    }

                    if (property.ToEntityType() != property.Partner.DeclaringEntityType())
                    {
                        context.AddError(
                            property.Location(),
                            EdmErrorCode.InvalidNavigationPropertyType,
                            Strings.EdmModel_Validator_Semantic_InvalidNavigationPropertyType(property.Name));
                    }
                });

        /// <summary>
        /// Validates that the dependent properties of a navigation property contain no duplicates.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDuplicateDependentProperty =
            new ValidationRule<IEdmNavigationProperty>(
                (context, navigationProperty) =>
                {
                    IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties();
                    if (dependentProperties != null)
                    {
                        HashSetInternal<string> propertyNames = new HashSetInternal<string>();
                        foreach (var property in navigationProperty.DependentProperties())
                        {
                            if (property != null)
                            {
                                ValidationHelper.AddMemberNameToHashSet(
                                    property,
                                    propertyNames,
                                    context,
                                    EdmErrorCode.DuplicateDependentProperty,
                                    Strings.EdmModel_Validator_Semantic_DuplicateDependentProperty(property.Name, navigationProperty.Name),
                                    /*supressError*/ false);
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates multiplicity of the principal end:
        /// 0..1 - if some dependent properties are nullable,
        ///    1 - if some dependent properties are not nullable.
        ///    * - not allowed.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyPrincipalEndMultiplicity =
          new ValidationRule<IEdmNavigationProperty>(
              (context, navigationProperty) =>
              {
                  /*
                    Dependent properties |       |         |        |
                    nullable?            |  All  |  Mixed  |  None  |
                    -------------------------------------------------
                                    0..1 |   +   |    +    |  error |
                    -------------------------------------------------
                                       1 | error |    +    |    +   |
                    -------------------------------------------------
                                       * | error |  error  |  error |
                    -------------------------------------------------
                  */

                  IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties();
                  if (dependentProperties != null)
                  {
                      if (ValidationHelper.AllPropertiesAreNullable(dependentProperties))
                      {
                          if (navigationProperty.TargetMultiplicity() != EdmMultiplicity.ZeroOrOne)
                          {
                              context.AddError(
                                  navigationProperty.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfPrincipalEnd,
                                  Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable(navigationProperty.Name));
                          }
                      }
                      else if (!ValidationHelper.HasNullableProperty(dependentProperties))
                      {
                          if (navigationProperty.TargetMultiplicity() != EdmMultiplicity.One)
                          {
                              context.AddError(
                                  navigationProperty.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfPrincipalEnd,
                                  Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable(navigationProperty.Name));
                          }
                      }
                      else
                      {
                          if (navigationProperty.TargetMultiplicity() != EdmMultiplicity.One &&
                              navigationProperty.TargetMultiplicity() != EdmMultiplicity.ZeroOrOne)
                          {
                              context.AddError(
                                  navigationProperty.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfPrincipalEnd,
                                  Strings.EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne(navigationProperty.Name));
                          }
                      }
                  }
              });

        /// <summary>
        /// Validates that if the dependent properties are equivalent to the key of the dependent entity, the multiplicity of the dependent entity cannot be 1
        /// Validates multiplicity of the dependent entity according to the following rules:
        /// 0..1, 1 - if dependent properties represent the dependent entity key.
        ///       * - if dependent properties don't represent the dependent entity key.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDependentEndMultiplicity =
          new ValidationRule<IEdmNavigationProperty>(
              (context, navigationProperty) =>
              {
                  // without a partner, the multiplicity of the source end cannot be determined.
                  if (navigationProperty.Partner == null)
                  {
                      return;
                  }

                  IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties();
                  if (dependentProperties != null)
                  {
                      if (ValidationHelper.PropertySetsAreEquivalent(navigationProperty.DeclaringEntityType().Key(), dependentProperties))
                      {
                          if (navigationProperty.Type.IsCollection())
                          {
                              context.AddError(
                                  navigationProperty.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfDependentEnd,
                                  Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne(navigationProperty.Name));
                          }
                      }
                      else if (!navigationProperty.Partner.Type.IsCollection())
                      {
                          context.AddError(
                              navigationProperty.Location(),
                              EdmErrorCode.InvalidMultiplicityOfDependentEnd,
                              Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany(navigationProperty.Name));
                      }
                  }
              });

        /// <summary>
        /// Validates that all dependent properties of a navigation property belong to the dependent entity type.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDependentPropertiesMustBelongToDependentEntity =
           new ValidationRule<IEdmNavigationProperty>(
               (context, navigationProperty) =>
               {
                   IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties();
                   if (dependentProperties != null)
                   {
                       IEdmEntityType dependentEntity = navigationProperty.DeclaringEntityType();
                       foreach (IEdmStructuralProperty dependantProperty in dependentProperties)
                       {
                           if (!context.IsBad(dependantProperty) && !dependantProperty.IsBad())
                           {
                               var property = dependentEntity.FindProperty(dependantProperty.Name);

                               // If we can't find the property by name, or we find a good property but it's not our dependent property
                               if (property != dependantProperty)
                               {
                                   context.AddError(
                                       navigationProperty.Location(),
                                       EdmErrorCode.DependentPropertiesMustBelongToDependentEntity,
                                       Strings.EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity(dependantProperty.Name, dependentEntity.Name));
                               }
                           }
                       }
                   }
               });

        /// <summary>
        /// Validates that the navigation property does not have both a multiplicity of many and an OnDelete operation.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified =
           new ValidationRule<IEdmNavigationProperty>(
               (context, end) =>
               {
                   // If an end has a multiplicity of many, it cannot have any operation behaviour
                   if (
                       end.Partner != null &&
                       end.Partner.Type.IsCollection() &&
                       end.OnDelete != EdmOnDeleteAction.None)
                   {
                       string errorMessage = Strings.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified(end.Name);

                       context.AddError(
                           end.Location(),
                           EdmErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified,
                           errorMessage);
                   }
               });

        /// <summary>
        /// Validates that the navigation property partner path, if exists, should be resolvable to a navigation property.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyPartnerPathShouldBeResolvable =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    var path = property.GetPartnerPath();
                    if (path != null
                        && property.Type.Definition.AsElementType() is IEdmEntityType
                        && CsdlSemanticsNavigationProperty.ResolvePartnerPath(
                               (IEdmEntityType)property.Type.Definition.AsElementType(), path, context.Model)
                           == null)
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.UnresolvedNavigationPropertyPartnerPath,
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "Cannot resolve partner path for navigation property '{0}'.",
                            property.Name));
                    }
                });

        /// <summary>
        /// Validates that if a navigation property has <see cref="IEdmNavigationProperty.ContainsTarget"/> = true and the target entity type is the same as
        /// the declaring type of the property, then the multiplicity of the target of navigation is 0..1 or Many.
        /// This depends on there being a targetting cycle. Because of the rule <see cref="NavigationMappingMustBeBidirectional" />, we know that either this is always true, or there will be an error
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithRecursiveContainmentTargetMustBeOptional =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (property.ContainsTarget &&
                        property.DeclaringType.IsOrInheritsFrom(property.ToEntityType()) &&
                        !(property.Type.IsCollection() || property.Type.IsNullable))
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional,
                        Strings.EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentTargetMustBeOptional(property.Name));
                    }
                });

        /// <summary>
        /// Validates that if a navigation property has <see cref="IEdmNavigationProperty.ContainsTarget"/> = true and the target entity type is the same as
        /// the declaring type of the property, then the multiplicity of the source of navigation is Zero-Or-One.
        /// This depends on there being a targetting cycle. Because of the rule <see cref="NavigationMappingMustBeBidirectional" />, we know that either this is always true, or there will be an error
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (property.Partner != null &&
                        property.ContainsTarget &&
                        property.DeclaringType.IsOrInheritsFrom(property.ToEntityType()) &&
                        (property.Partner.Type.IsCollection() || !property.Partner.Type.IsNullable))
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne,
                        Strings.EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne(property.Name));
                    }
                });

        /// <summary>
        /// Validates that if a navigation property has <see cref="IEdmNavigationProperty.ContainsTarget"/> = true and the target entity type is defferent than
        /// the declaring type of the property, then the multiplicity of the source of navigation is One.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (property.Partner != null &&
                        property.ContainsTarget &&
                        !property.DeclaringType.IsOrInheritsFrom(property.ToEntityType()) &&
                        (property.Partner.Type.IsCollection() || property.Partner.Type.IsNullable))
                    {
                        context.AddError(
                            property.Location(),
                            EdmErrorCode.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne,
                            Strings.EdmModel_Validator_Semantic_NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne(property.Name));
                    }
                });

        /// <summary>
        /// Validates that if an entity does not directly contain itself, it cannot contain itself through a containment loop.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyEntityMustNotIndirectlyContainItself =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (property.ContainsTarget &&
                        !property.DeclaringType.IsOrInheritsFrom(property.ToEntityType()))
                    {
                        if (ValidationHelper.TypeIndirectlyContainsTarget(property.ToEntityType(), property.DeclaringEntityType(), new HashSetInternal<IEdmEntityType>(), context.Model))
                        {
                            context.AddError(
                                property.Location(),
                                EdmErrorCode.NavigationPropertyEntityMustNotIndirectlyContainItself,
                                Strings.EdmModel_Validator_Semantic_NavigationPropertyEntityMustNotIndirectlyContainItself(property.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that the type of the navigation property cannot have path type property defined.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyTypeCannotHavePathTypeProperty =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    IEdmTypeReference propertyType = property.Type;
                    if (propertyType.IsCollection())
                    {
                        propertyType = propertyType.AsCollection().ElementType();
                    }

                    IEdmStructuredType structuredType = propertyType.ToStructuredType();
                    if (structuredType == null)
                    {
                        return;
                    }

                    IList<IEdmStructuredType> visited = new List<IEdmStructuredType>();
                    if (HasPathTypeProperty(structuredType, visited))
                    {
                        string errorMessage = Strings
                            .EdmModel_Validator_Semantic_TypeOfNavigationPropertyCannotHavePathProperty(property.Type.FullName(), property.Name, property.DeclaringType.FullTypeName());

                        context.AddError(
                            property.Location(),
                            EdmErrorCode.TypeOfNavigationPropertyCannotHavePathProperty,
                            errorMessage);
                    }
                });

        /// <summary>
        /// Validates that each pair of properties between the dependent properties and the principal properties are of the same type.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyTypeMismatchRelationshipConstraint =
          new ValidationRule<IEdmNavigationProperty>(
              (context, navigationProperty) =>
              {
                  IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties();
                  if (dependentProperties != null)
                  {
                      int dependentPropertiesCount = dependentProperties.Count();
                      IEdmEntityType principalEntityType = navigationProperty.ToEntityType();
                      IEnumerable<IEdmStructuralProperty> principalProperties = navigationProperty.PrincipalProperties();
                      if (dependentPropertiesCount == principalProperties.Count())
                      {
                          for (int i = 0; i < dependentPropertiesCount; i++)
                          {
                              var dependentType = dependentProperties.ElementAtOrDefault(i).Type.Definition;
                              var principalType = principalProperties.ElementAtOrDefault(i).Type.Definition;
                              if (!(dependentType is BadType) && !(principalType is BadType) && !dependentType.IsEquivalentTo(principalType))
                              {
                                  string errorMessage = Strings.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(navigationProperty.DependentProperties().ToList()[i].Name, navigationProperty.DeclaringEntityType().FullName(), principalProperties.ToList()[i].Name, principalEntityType.Name, "Fred");

                                  context.AddError(navigationProperty.Location(), EdmErrorCode.TypeMismatchRelationshipConstraint, errorMessage);
                              }
                          }
                      }
                  }
              });

        #endregion

        #region IEdmProperty

        /// <summary>
        /// A property without other errors must not have kind of none.
        /// </summary>
        public static readonly ValidationRule<IEdmProperty> PropertyMustNotHaveKindOfNone =
            new ValidationRule<IEdmProperty>(
                (context, property) =>
                {
                    if (property.PropertyKind == EdmPropertyKind.None && !context.IsBad(property))
                    {
                        context.AddError(
                            property.Location(),
                            EdmErrorCode.PropertyMustNotHaveKindOfNone,
                            Strings.EdmModel_Validator_Semantic_PropertyMustNotHaveKindOfNone(property.Name));
                    }
                });

        /// <summary>
        /// Collection(Edm.PrimitiveType) and Collection(Edm.ComplexType) cannot be used as the type of a property.
        /// </summary>
        public static readonly ValidationRule<IEdmProperty> PropertyTypeCannotBeCollectionOfAbstractType =
            new ValidationRule<IEdmProperty>(
                (context, property) =>
                {
                    if (property.Type.IsCollection())
                    {
                        IEdmTypeReference elementType = property.Type.AsCollection().ElementType();
                        if (elementType.Definition == EdmCoreModelComplexType.Instance ||
                            elementType.Definition == EdmCoreModel.Instance.GetPrimitiveType())
                        {
                            context.AddError(
                                property.Location(),
                                EdmErrorCode.PropertyTypeCannotBeCollectionOfAbstractType,
                                Strings.EdmModel_Validator_Semantic_PropertyTypeCannotBeCollectionOfAbstractType(
                                    property.Type.FullName(), property.Name));
                        }
                    }
                });

        #endregion

        #region IEdmOperationImport

        /// <summary>
        /// Validates that if an operation import cannot import an operation that is bound.
        /// </summary>
        public static readonly ValidationRule<IEdmOperationImport> OperationImportCannotImportBoundOperation =
           new ValidationRule<IEdmOperationImport>(
               (context, operationImport) =>
               {
                   if (operationImport.Operation.IsBound)
                   {
                       context.AddError(
                           operationImport.Location(),
                           EdmErrorCode.OperationImportCannotImportBoundOperation,
                           Strings.EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation(operationImport.Name, operationImport.Operation.Name));
                   }
               });

        /// <summary>
        /// Validates that the entity set of a operation import is defined using a path or an entity set reference expression.
        /// </summary>
        public static readonly ValidationRule<IEdmOperationImport> OperationImportEntitySetExpressionIsInvalid =
            new ValidationRule<IEdmOperationImport>(
                (context, operationImport) =>
                {
                    if (operationImport.EntitySet != null)
                    {
                        if (operationImport.EntitySet.ExpressionKind != EdmExpressionKind.Path)
                        {
                            context.AddError(
                                operationImport.Location(),
                                EdmErrorCode.OperationImportEntitySetExpressionIsInvalid,
                                Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionKindIsInvalid(operationImport.Name, operationImport.EntitySet.ExpressionKind));
                        }
                        else
                        {
                            IEdmEntitySetBase entitySet;

                            if (!operationImport.TryGetStaticEntitySet(context.Model, out entitySet))
                            {
                                context.AddError(
                                    operationImport.Location(),
                                    EdmErrorCode.OperationImportEntitySetExpressionIsInvalid,
                                    Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                            }
                            else
                            {
                                // Checking isbad so that two errors are not returned when a csdl document is parsed, one of it being unresolved another
                                // for the error below.
                                if (!context.IsBad(entitySet))
                                {
                                    IEdmEntitySet foundEntitySet = operationImport.Container.FindEntitySetExtended(entitySet.Name);
                                    if (foundEntitySet == null)
                                    {
                                        context.AddError(
                                            operationImport.Location(),
                                            EdmErrorCode.OperationImportEntitySetExpressionIsInvalid,
                                            Strings.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(operationImport.Name));
                                    }
                                }
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that the return type of a operation import must match the type of the entity set of the function.
        /// </summary>
        public static readonly ValidationRule<IEdmOperationImport> OperationImportEntityTypeDoesNotMatchEntitySet =
            new ValidationRule<IEdmOperationImport>(
                (context, operationImport) =>
                {
                    if (operationImport.EntitySet != null && operationImport.Operation.ReturnType != null)
                    {
                        IEdmTypeReference elementType = operationImport.Operation.ReturnType.IsCollection() ? operationImport.Operation.ReturnType.AsCollection().ElementType() : operationImport.Operation.ReturnType;
                        if (elementType.IsEntity())
                        {
                            IEdmEntityType returnedEntityType = elementType.AsEntity().EntityDefinition();

                            IEdmEntitySetBase entitySet;
                            IEdmOperationParameter parameter;
                            Dictionary<IEdmNavigationProperty, IEdmPathExpression> path;
                            IEnumerable<EdmError> errors;
                            if (operationImport.TryGetStaticEntitySet(context.Model, out entitySet))
                            {
                                IEdmEntityType entitySetElementType = entitySet.EntityType();
                                if (!returnedEntityType.IsOrInheritsFrom(entitySetElementType) && !context.IsBad(returnedEntityType) && !context.IsBad(entitySet) && !context.IsBad(entitySetElementType))
                                {
                                    string errorMessage = Strings.EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet(
                                        operationImport.Name,
                                        returnedEntityType.FullName(),
                                        entitySet.Name);

                                    context.AddError(
                                        operationImport.Location(),
                                        EdmErrorCode.OperationImportEntityTypeDoesNotMatchEntitySet,
                                        errorMessage);
                                }
                            }
                            else if (operationImport.TryGetRelativeEntitySetPath(context.Model, out parameter, out path, out errors))
                            {
                                List<IEdmNavigationProperty> pathList = path.Select(s => s.Key).ToList();
                                IEdmTypeReference relativePathType = pathList.Count == 0 ? parameter.Type : path.Last().Key.Type;
                                IEdmTypeReference relativePathElementType = relativePathType.IsCollection() ? relativePathType.AsCollection().ElementType() : relativePathType;
                                if (!returnedEntityType.IsOrInheritsFrom(relativePathElementType.Definition) && !context.IsBad(returnedEntityType) && !context.IsBad(relativePathElementType.Definition))
                                {
                                    context.AddError(
                                        operationImport.Location(),
                                        EdmErrorCode.OperationImportEntityTypeDoesNotMatchEntitySet,
                                        Strings.EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet2(operationImport.Name, elementType.FullName()));
                                }
                            }

                            // The case when all try gets fail is caught by the FunctionImportEntitySetExpressionIsInvalid rule.
                        }
                        else if (!context.IsBad(elementType.Definition))
                        {
                            context.AddError(
                                operationImport.Location(),
                                EdmErrorCode.OperationImportSpecifiesEntitySetButDoesNotReturnEntityType,
                                Strings.EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType(operationImport.Name));
                        }
                    }
                });

        #endregion

        #region IEdmFunctionImport

        /// <summary>
        /// Validates that the function import included in service document must not have parameters.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportWithParameterShouldNotBeIncludedInServiceDocument =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.IncludeInServiceDocument && functionImport.Function.Parameters.Any())
                    {
                        context.AddError(
                            functionImport.Location(),
                            EdmErrorCode.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument,
                            Strings.EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument(functionImport.Name));
                    }
                });

        #endregion

        #region IEdmFunction

        /// <summary>
        /// Validates that if a function, it must have a return type.
        /// </summary>
        public static readonly ValidationRule<IEdmFunction> FunctionMustHaveReturnType =
            new ValidationRule<IEdmFunction>(
                (context, function) =>
                {
                    if (function.ReturnType == null)
                    {
                        context.AddError(
                            function.Location(),
                            EdmErrorCode.FunctionMustHaveReturnType,
                            Strings.EdmModel_Validator_Semantic_FunctionMustHaveReturnType(function.Name));
                    }
                });

        #endregion

        #region IEdmOperation

        /// <summary>
        /// Validates that a operation import has an allowed return type.
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> OperationUnsupportedReturnType =
            new ValidationRule<IEdmOperation>(
                (context, operation) =>
                {
                    if (operation.ReturnType != null)
                    {
                        IEdmTypeReference elementType = operation.ReturnType.IsCollection() ? operation.ReturnType.AsCollection().ElementType() : operation.ReturnType;
                        var isUnresolvedElement = elementType.Definition is IUnresolvedElement;
                        if (!isUnresolvedElement && context.IsBad(elementType.Definition))
                        {
                            context.AddError(
                                operation.Location(),
                                EdmErrorCode.OperationImportUnsupportedReturnType,
                                Strings.EdmModel_Validator_Semantic_OperationWithUnsupportedReturnType(operation.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that a operation does not have multiple parameters with the same name.
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> OperationParameterNameAlreadyDefinedDuplicate =
           new ValidationRule<IEdmOperation>(
               (context, operation) =>
               {
                   HashSetInternal<string> parameterList = new HashSetInternal<string>();
                   if (operation.Parameters != null)
                   {
                       foreach (var parameter in operation.Parameters)
                       {
                           ValidationHelper.AddMemberNameToHashSet(
                               parameter,
                               parameterList,
                               context,
                               EdmErrorCode.AlreadyDefined,
                               Strings.EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate(parameter.Name),
                               /*supressError*/ false);
                       }
                   }
               });

        /// <summary>
        /// Validates that if an operation is bindable, it must have non-optional parameters.
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> BoundOperationMustHaveParameters =
           new ValidationRule<IEdmOperation>(
               (context, operation) =>
               {
                   if (operation.IsBound && !operation.Parameters.Any(p => !(p is IEdmOptionalParameter)))
                   {
                       context.AddError(
                           operation.Location(),
                           EdmErrorCode.BoundOperationMustHaveParameters,
                           Strings.EdmModel_Validator_Semantic_BoundOperationMustHaveParameters(operation.Name));
                   }
               });

        /// <summary>
        /// Validates optional parameters must come before required parameters.
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> OptionalParametersMustComeAfterRequiredParameters =
           new ValidationRule<IEdmOperation>(
               (context, operation) =>
               {
                   bool foundOptional = false;
                   foreach (IEdmOperationParameter parameter in operation.Parameters)
                   {
                       if (parameter is IEdmOptionalParameter)
                       {
                           foundOptional = true;
                       }
                       else if (foundOptional)
                       {
                           context.AddError(
                               operation.Location(),
                               EdmErrorCode.RequiredParametersMustPrecedeOptional,
                               Strings.EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional(parameter.Name));
                       }
                   }
               });

        /// <summary>
        /// Validates that if a operationImport is bindable, it must have parameters.
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> OperationEntitySetPathMustBeValid =
            new ValidationRule<IEdmOperation>((context, operation) =>
            {
                IEdmOperationParameter bindingParameter = null;
                Dictionary<IEdmNavigationProperty, IEdmPathExpression> navProps = null;
                IEdmEntityType lastEntityType = null;
                IEnumerable<EdmError> errors = null;

                operation.TryGetRelativeEntitySetPath(context.Model, out bindingParameter, out navProps, out lastEntityType, out errors);

                foreach (var error in errors)
                {
                    context.AddError(error);
                }
            });

        /// <summary>
        /// Validates that the return type is consistent with the entityset path if it exists.
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> OperationReturnTypeEntityTypeMustBeValid =
           new ValidationRule<IEdmOperation>((context, operation) =>
           {
               IEdmOperationParameter bindingParameter = null;
               Dictionary<IEdmNavigationProperty, IEdmPathExpression> navProps = null;
               IEdmEntityType lastEntityType = null;
               IEnumerable<EdmError> errors = null;

               // If there is no relative entity set path to reolve then there is nothing to compare against.
               if (!operation.TryGetRelativeEntitySetPath(context.Model, out bindingParameter, out navProps, out lastEntityType, out errors))
               {
                   return;
               }

               if (operation.ReturnType != null)
               {
                   IEdmEntityType elementType = operation.ReturnType.Definition as IEdmEntityType;
                   IEdmCollectionType returnCollectionType = operation.ReturnType.Definition as IEdmCollectionType;
                   if (elementType == null && returnCollectionType != null)
                   {
                       elementType = returnCollectionType.ElementType.Definition as IEdmEntityType;
                   }

                   bool isEntity = operation.ReturnType.IsEntity();
                   if (returnCollectionType != null)
                   {
                       isEntity = returnCollectionType.ElementType.IsEntity();
                   }

                   if (!isEntity || context.IsBad(elementType))
                   {
                       context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathReturnTypeInvalid, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid(operation.Name));
                   }

                   IEdmNavigationProperty navProp = null;
                   if (navProps != null)
                   {
                       navProp = navProps.LastOrDefault().Key;
                   }

                   if (navProp != null && navProp.TargetMultiplicity() == EdmMultiplicity.Many)
                   {
                       if (returnCollectionType == null)
                       {
                           context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType(operation.Name));
                       }
                   }

                   if (navProp != null && navProp.TargetMultiplicity() != EdmMultiplicity.Many)
                   {
                       if (returnCollectionType != null)
                       {
                           context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType(operation.Name));
                       }
                   }

                   if (lastEntityType != null && elementType != null && !elementType.IsOrInheritsFrom(lastEntityType))
                   {
                       context.AddError(operation.Location(), EdmErrorCode.OperationWithEntitySetPathAndReturnTypeTypeNotAssignable, Strings.EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable(operation.Name, elementType.FullName(), lastEntityType.FullName()));
                   }
               }
           });


        /// <summary>
        /// Validates that the return type cannot be Collection(Edm.PrimitiveType) or Collection(Edm.ComplexType).
        /// </summary>
        public static readonly ValidationRule<IEdmOperation> OperationReturnTypeCannotBeCollectionOfAbstractType =
            new ValidationRule<IEdmOperation>((context, operation) =>
            {
                if (operation.ReturnType != null && operation.ReturnType.IsCollection())
                {
                    IEdmTypeReference elementType = operation.ReturnType.AsCollection().ElementType();
                    if (elementType.Definition == EdmCoreModelComplexType.Instance ||
                        elementType.Definition == EdmCoreModel.Instance.GetPrimitiveType())
                    {
                        context.AddError(
                            operation.Location(),
                            EdmErrorCode.OperationWithCollectionOfAbstractReturnTypeInvalid,
                            Strings.EdmModel_Validator_Semantic_OperationReturnTypeCannotBeCollectionOfAbstractType(operation.ReturnType.FullName(), operation.FullName()));
                    }
                }
            });

        #endregion

        #region IEdmTypeReference

        /// <summary>
        /// Validates that a type reference refers to a type that can be found through the model being validated.
        /// </summary>
        public static readonly ValidationRule<IEdmTypeReference> TypeReferenceInaccessibleSchemaType =
            new ValidationRule<IEdmTypeReference>(
                (context, typeReference) =>
                {
                    IEdmSchemaType schemaType = typeReference.Definition as IEdmSchemaType;
                    if (schemaType != null && !context.IsBad(schemaType))
                    {
                        CheckForUnreacheableTypeError(context, schemaType, typeReference.Location());
                    }
                });

        #endregion

        #region IEdmDecimalTypeReference

        /// <summary>
        /// Validates that the scale is between 0 and the precision of the decimal type.
        /// </summary>
        public static readonly ValidationRule<IEdmDecimalTypeReference> DecimalTypeReferenceScaleOutOfRange =
            new ValidationRule<IEdmDecimalTypeReference>(
                (context, type) =>
                {
                    if (type.Scale > type.Precision || type.Scale < 0)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.ScaleOutOfRange,
                            Strings.EdmModel_Validator_Semantic_ScaleOutOfRange);
                    }
                });

        /// <summary>
        /// Validates that the precision is between 0 and the max precision of the decimal type.
        /// </summary>
        public static readonly ValidationRule<IEdmDecimalTypeReference> DecimalTypeReferencePrecisionOutOfRange =
            new ValidationRule<IEdmDecimalTypeReference>(
                (context, type) =>
                {
                    if (type.Precision > EdmConstants.Max_Precision || type.Precision < EdmConstants.Min_Precision)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.PrecisionOutOfRange,
                            Strings.EdmModel_Validator_Semantic_PrecisionOutOfRange);
                    }
                });

        #endregion

        #region IEdmStringTypeReference

        /// <summary>
        /// Validates that the max length of a string is not negative.
        /// </summary>
        public static readonly ValidationRule<IEdmStringTypeReference> StringTypeReferenceStringMaxLengthNegative =
            new ValidationRule<IEdmStringTypeReference>(
                (context, type) =>
                {
                    if (type.MaxLength < 0)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.MaxLengthOutOfRange,
                            Strings.EdmModel_Validator_Semantic_StringMaxLengthOutOfRange);
                    }
                });

        /// <summary>
        /// Validates that IsUnbounded cannot be true if MaxLength is non-null.
        /// </summary>
        public static readonly ValidationRule<IEdmStringTypeReference> StringTypeReferenceStringUnboundedNotValidForMaxLength =
            new ValidationRule<IEdmStringTypeReference>(
                (context, type) =>
                {
                    if (type.MaxLength != null && type.IsUnbounded)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull,
                            Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
                    }
                });

        #endregion

        #region IEdmBinaryTypeReference

        /// <summary>
        /// Validates that the max length of a binary type is not negative.
        /// </summary>
        public static readonly ValidationRule<IEdmBinaryTypeReference> BinaryTypeReferenceBinaryMaxLengthNegative =
            new ValidationRule<IEdmBinaryTypeReference>(
                (context, type) =>
                {
                    if (type.MaxLength < 0)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.MaxLengthOutOfRange,
                            Strings.EdmModel_Validator_Semantic_MaxLengthOutOfRange);
                    }
                });

        /// <summary>
        /// Validates that isUnbounded cannot be true if MaxLength is non-null.
        /// </summary>
        public static readonly ValidationRule<IEdmBinaryTypeReference> BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength =
            new ValidationRule<IEdmBinaryTypeReference>(
                (context, type) =>
                {
                    if (type.MaxLength != null && type.IsUnbounded)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull,
                            Strings.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
                    }
                });

        #endregion

        #region IEdmTemporalTypeReference

        /// <summary>
        /// Validates that the precision is between 0 and the max precision of the temporal type.
        /// </summary>
        public static readonly ValidationRule<IEdmTemporalTypeReference> TemporalTypeReferencePrecisionOutOfRange =
            new ValidationRule<IEdmTemporalTypeReference>(
                (context, type) =>
                {
                    if (type.Precision > EdmConstants.Max_Precision || type.Precision < EdmConstants.Min_Precision)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.PrecisionOutOfRange,
                            Strings.EdmModel_Validator_Semantic_PrecisionOutOfRange);
                    }
                });

        #endregion

        #region IEdmModel
        /// <summary>
        /// Validates every schema element in the current model is unique across all referenced models.
        /// </summary>
        public static readonly ValidationRule<IEdmModel> ModelDuplicateSchemaElementName =
           new ValidationRule<IEdmModel>(
               (context, model) =>
               {
                   HashSetInternal<string> nonFunctionNameList = new HashSetInternal<string>();
                   DuplicateOperationValidator duplicateOperationValidator = new DuplicateOperationValidator(context);
                   HashSetInternal<string> operationList = new HashSetInternal<string>();

                   foreach (var item in model.SchemaElements)
                   {
                       bool duplicate = false;
                       string fullName = item.FullName();
                       IEdmOperation operation = item as IEdmOperation;
                       if (operation != null)
                       {
                           if (!operationList.Contains(operation.FullName()))
                           {
                               operationList.Add(operation.FullName());
                           }

                           // If a non-function already exists with the same name, stop processing as a function, as it is irrelevant it will always be an error.
                           if (nonFunctionNameList.Contains(fullName))
                           {
                               duplicate = true;
                           }

                           duplicateOperationValidator.ValidateNotDuplicate(operation, false /*skipError*/);

                           if (!duplicate)
                           {
                               duplicate = model.OperationOrNameExistsInReferencedModel(operation, fullName);
                           }
                       }
                       else
                       {
                           if (!nonFunctionNameList.Add(fullName))
                           {
                               duplicate = true;
                           }
                           else
                           {
                               if (operationList.Contains(fullName))
                               {
                                   duplicate = true;
                               }
                               else
                               {
                                   duplicate = model.ItemExistsInReferencedModel(fullName, true);
                               }
                           }
                       }

                       if (duplicate)
                       {
                           context.AddError(
                                       item.Location(),
                                       EdmErrorCode.AlreadyDefined,
                                       Strings.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined(fullName));
                       }
                   }
               });

        /// <summary>
        /// Validates that there are not duplicate properties in an entity key.
        /// </summary>
        public static readonly ValidationRule<IEdmModel> ModelDuplicateEntityContainerName =
            new ValidationRule<IEdmModel>(
                (context, model) =>
                {
                    HashSetInternal<string> entityContainerNameList = new HashSetInternal<string>();
                    var container = model.EntityContainer;
                    if (container != null)
                    {
                        ValidationHelper.AddMemberNameToHashSet(
                            container,
                            entityContainerNameList,
                            context,
                            EdmErrorCode.DuplicateEntityContainerName,
                            Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerName(container.Name),
                            /*supressError*/ false);
                    }
                });

        /// <summary>
        /// Validates all function overloads with the same name have the same returntype.
        /// </summary>
        public static readonly ValidationRule<IEdmModel> ModelBoundFunctionOverloadsMustHaveSameReturnType =
            new ValidationRule<IEdmModel>(
                (context, model) =>
                {
                    foreach (var functionNameGroup in model.SchemaElements.OfType<IEdmFunction>().Where(f => f.IsBound).GroupBy(f2 => f2.FullName()))
                    {
                        Dictionary<IEdmTypeReference, IEdmTypeReference> bindingTypeReturnTypeLookup = new Dictionary<IEdmTypeReference, IEdmTypeReference>(new EdmTypeReferenceComparer());
                        foreach (var function in functionNameGroup)
                        {
                            // Skip invalid functions that have no parameters or null ReturnType. This is validated elsewhere.
                            if (!function.Parameters.Any() || function.ReturnType == null)
                            {
                                continue;
                            }

                            var bindingParameter = function.Parameters.First();
                            if (!bindingTypeReturnTypeLookup.ContainsKey(bindingParameter.Type))
                            {
                                bindingTypeReturnTypeLookup.Add(bindingParameter.Type, function.ReturnType);
                            }
                            else
                            {
                                IEdmTypeReference expectedReturnType = bindingTypeReturnTypeLookup[bindingParameter.Type];
                                if (!function.ReturnType.IsEquivalentTo(expectedReturnType))
                                {
                                    context.AddError(
                                        function.Location(),
                                        EdmErrorCode.BoundFunctionOverloadsMustHaveSameReturnType,
                                        Strings.EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType(function.Name, expectedReturnType.FullName()));
                                }
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that all function overloads have the same return types.
        /// </summary>
        public static readonly ValidationRule<IEdmModel> UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes =
            new ValidationRule<IEdmModel>((context, model) =>
            {
                Dictionary<string, IEdmTypeReference> functionNameToReturnTypeLookup = new Dictionary<string, IEdmTypeReference>();
                foreach (var function in model.SchemaElements.OfType<IEdmFunction>().Where(f => !f.IsBound))
                {
                    if (!functionNameToReturnTypeLookup.ContainsKey(function.Name))
                    {
                        functionNameToReturnTypeLookup.Add(function.Name, function.ReturnType);
                    }
                    else
                    {
                        if (!function.ReturnType.IsEquivalentTo(functionNameToReturnTypeLookup[function.Name]))
                        {
                            context.AddError(
                                       function.Location(),
                                       EdmErrorCode.UnboundFunctionOverloadHasIncorrectReturnType,
                                       Strings.EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType(function.Name));
                        }
                    }
                }
            });

        #endregion

        #region IEdmImmediateValueAnnotation

        /// <summary>
        /// Validates that an immediate annotation has a name and a namespace.
        /// </summary>
        public static readonly ValidationRule<IEdmDirectValueAnnotation> ImmediateValueAnnotationElementAnnotationIsValid =
            new ValidationRule<IEdmDirectValueAnnotation>(
                (context, annotation) =>
                {
                    IEdmStringValue stringValue = annotation.Value as IEdmStringValue;
                    if (stringValue != null)
                    {
                        if (stringValue.IsSerializedAsElement(context.Model))
                        {
                            if (EdmUtil.IsNullOrWhiteSpaceInternal(annotation.NamespaceUri) || EdmUtil.IsNullOrWhiteSpaceInternal(annotation.Name))
                            {
                                context.AddError(
                                   annotation.Location(),
                                   EdmErrorCode.InvalidElementAnnotation,
                                   Strings.EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm);
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that an immediate annotation that is flagged to be serialized as an element can be serialized safely.
        /// </summary>
        public static readonly ValidationRule<IEdmDirectValueAnnotation> ImmediateValueAnnotationElementAnnotationHasNameAndNamespace =
            new ValidationRule<IEdmDirectValueAnnotation>(
                (context, annotation) =>
                {
                    IEdmStringValue stringValue = annotation.Value as IEdmStringValue;
                    if (stringValue != null)
                    {
                        if (stringValue.IsSerializedAsElement(context.Model))
                        {
                            EdmError error;
                            if (!ValidationHelper.ValidateValueCanBeWrittenAsXmlElementAnnotation(stringValue, annotation.NamespaceUri, annotation.Name, out error))
                            {
                                context.AddError(error);
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that the name of a direct annotation can safely be serialized as XML.
        /// </summary>
        public static readonly ValidationRule<IEdmDirectValueAnnotation> DirectValueAnnotationHasXmlSerializableName =
            new ValidationRule<IEdmDirectValueAnnotation>(
                (context, annotation) =>
                {
                    string name = annotation.Name;

                    // We check for null, whitespace, and length in separate IEdmNamedElement validation rules.
                    if (!EdmUtil.IsNullOrWhiteSpaceInternal(name) && name.Length <= CsdlConstants.Max_NameLength && name.Length > 0)
                    {
                        try
                        {
                            // Note: this check can be done without the try/catch block, but we need XmlConvert.IsStartNCNameChar and XmlConvert.IsNCNameChar, which are not available in 3.5.
                            XmlConvert.VerifyNCName(annotation.Name);
                        }
                        catch (XmlException)
                        {
                            IEdmValue value = annotation.Value as IEdmValue;
                            EdmLocation errorLocation = value == null ? null : value.Location();
                            context.AddError(new EdmError(errorLocation, EdmErrorCode.InvalidName, Edm.Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(annotation.Name)));
                        }
                    }
                });

        #endregion

        #region IEdmVocabularyAnnotation

        /// <summary>
        /// Validates that a vocabulary annotations target can be found through the model containing the annotation.
        /// </summary>
        public static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationInaccessibleTarget =
            new ValidationRule<IEdmVocabularyAnnotation>(
                (context, annotation) =>
                {
                    var target = annotation.Target;
                    bool foundTarget = false;

                    IEdmEntityContainer entityContainer = target as IEdmEntityContainer;
                    if (entityContainer != null)
                    {
                        foundTarget = (context.Model.FindEntityContainer(entityContainer.FullName()) as IEdmEntityContainer != null);
                    }
                    else
                    {
                        IEdmEntitySet entitySet = target as IEdmEntitySet;
                        if (entitySet != null)
                        {
                            IEdmEntityContainer container = entitySet.Container as IEdmEntityContainer;
                            if (container != null)
                            {
                                foundTarget = (container.FindEntitySetExtended(entitySet.Name) != null);
                            }
                        }
                        else
                        {
                            IEdmSchemaType schemaType = target as IEdmSchemaType;
                            if (schemaType != null)
                            {
                                foundTarget = (context.Model.FindType(schemaType.FullName()) as IEdmSchemaType != null);
                            }
                            else
                            {
                                IEdmTerm term = target as IEdmTerm;
                                if (term != null)
                                {
                                    foundTarget = (context.Model.FindTerm(term.FullName()) != null);
                                }
                                else
                                {
                                    IEdmOperation operation = target as IEdmOperation;
                                    if (operation != null)
                                    {
                                        foundTarget = context.Model.FindOperations(operation.FullName()).Any();
                                    }
                                    else
                                    {
                                        IEdmOperationImport operationImport = target as IEdmOperationImport;
                                        if (operationImport != null)
                                        {
                                            foundTarget = operationImport.Container.FindOperationImportsExtended(operationImport.Name).Any();
                                        }
                                        else
                                        {
                                            IEdmProperty typeProperty = target as IEdmProperty;
                                            if (typeProperty != null)
                                            {
                                                string declaringTypeFullName = EdmUtil.FullyQualifiedName(typeProperty.DeclaringType as IEdmSchemaType);
                                                IEdmStructuredType modelType = context.Model.FindType(declaringTypeFullName) as IEdmStructuredType;
                                                if (modelType != null)
                                                {
                                                    // If we can find a structured type with this name in the model check if it has a property with this name
                                                    foundTarget = (modelType.FindProperty(typeProperty.Name) != null);
                                                }
                                            }
                                            else
                                            {
                                                IEdmOperationParameter operationParameter = target as IEdmOperationParameter;
                                                if (operationParameter != null)
                                                {
                                                    IEdmOperation declaringOperation = operationParameter.DeclaringOperation as IEdmOperation;
                                                    if (declaringOperation != null)
                                                    {
                                                        // For all functions with this name declared in the model check if it has a parameter with this name
                                                        foreach (var func in context.Model.FindOperations(declaringOperation.FullName()))
                                                        {
                                                            if (func.FindParameter(operationParameter.Name) != null)
                                                            {
                                                                foundTarget = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        IEdmOperationImport declaringFunctionImport = operationParameter.DeclaringOperation as IEdmOperationImport;
                                                        if (declaringFunctionImport != null)
                                                        {
                                                            var container = declaringFunctionImport.Container as IEdmEntityContainer;
                                                            foreach (var currentFunction in container.FindOperationImportsExtended(declaringFunctionImport.Name))
                                                            {
                                                                if (currentFunction.Operation.FindParameter(operationParameter.Name) != null)
                                                                {
                                                                    foundTarget = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    // Only validate annotations targeting elements that can be found via the model API.
                                                    // E.g. annotations targeting annotations will not be valid without this branch.
                                                    foundTarget = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!foundTarget)
                    {
                        context.AddError(
                            annotation.Location(),
                            EdmErrorCode.BadUnresolvedTarget,
                            Strings.EdmModel_Validator_Semantic_InaccessibleTarget(EdmUtil.FullyQualifiedName(target)));
                    }
                });

        /// <summary>
        /// Validates that if a vocabulary annotation declares a type, the expression for that annotation has the correct type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationAssertCorrectExpressionType =
            new ValidationRule<IEdmVocabularyAnnotation>(
                (context, annotation) =>
                {
                    IEnumerable<EdmError> errors;
                    if (!annotation.Value.TryCast(annotation.Term.Type, out errors))
                    {
                        foreach (EdmError error in errors)
                        {
                            if (error.ErrorCode != EdmErrorCode.RecordExpressionMissingRequiredProperty)
                            {
                                context.AddError(error);
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that a vocabulary annotations term can be found through the model containing the annotation.
        /// </summary>
        public static readonly ValidationRule<IEdmVocabularyAnnotation> AnnotationInaccessibleTerm =
            new ValidationRule<IEdmVocabularyAnnotation>(
                (context, annotation) =>
                {
                    // An unbound term is not treated as a semantic error, and looking up its name would fail.
                    IEdmTerm term = annotation.Term;
                    if (!(term is Microsoft.OData.Edm.Csdl.CsdlSemantics.IUnresolvedElement) && context.Model.FindTerm(term.FullName()) == null)
                    {
                        context.AddError(
                            annotation.Location(),
                            EdmErrorCode.BadUnresolvedTerm,
                            Strings.EdmModel_Validator_Semantic_InaccessibleTerm(annotation.Term.FullName()));
                    }
                });

        #endregion

        #region IEdmPropertyValueBinding

        /// <summary>
        /// Validates that the value of a property value binding is the correct type.
        /// </summary>
        public static readonly ValidationRule<IEdmPropertyValueBinding> PropertyValueBindingValueIsCorrectType =
            new ValidationRule<IEdmPropertyValueBinding>(
                (context, binding) =>
                {
                    IEnumerable<EdmError> errors;
                    if (!binding.Value.TryCast(binding.BoundProperty.Type, out errors) &&
                        !context.IsBad(binding) &&
                        !context.IsBad(binding.BoundProperty))
                    {
                        foreach (EdmError error in errors)
                        {
                            context.AddError(error);
                        }
                    }
                });

        #endregion

        #region IEdmIfExpression

        /// <summary>
        /// Validates that an if expression has a boolean condition.
        /// </summary>
        public static readonly ValidationRule<IEdmIfExpression> IfExpressionAssertCorrectTestType =
            new ValidationRule<IEdmIfExpression>(
                (context, expression) =>
                {
                    IEnumerable<EdmError> errors;
                    if (!expression.TestExpression.TryCast(EdmCoreModel.Instance.GetBoolean(false), out errors))
                    {
                        foreach (EdmError error in errors)
                        {
                            context.AddError(error);
                        }
                    }
                });

        #endregion

        #region IEdmCollectionExpression

        /// <summary>
        /// Validates that all properties of a collection expression are of the correct type.
        /// </summary>
        public static readonly ValidationRule<IEdmCollectionExpression> CollectionExpressionAllElementsCorrectType =
            new ValidationRule<IEdmCollectionExpression>(
                (context, expression) =>
                {
                    if (expression.DeclaredType != null && !context.IsBad(expression) && !context.IsBad(expression.DeclaredType))
                    {
                        IEnumerable<EdmError> discoveredErrors;
                        ExpressionTypeChecker.TryCastCollectionAsType(expression, expression.DeclaredType, null, false, out discoveredErrors);
                        foreach (EdmError error in discoveredErrors)
                        {
                            context.AddError(error);
                        }
                    }
                });

        #endregion

        #region IEdmRecordExpression

        /// <summary>
        /// Validates that if a value record expression declares a type, the property types are correct.
        /// </summary>
        public static readonly ValidationRule<IEdmRecordExpression> RecordExpressionPropertiesMatchType =
            new ValidationRule<IEdmRecordExpression>(
                (context, expression) =>
                {
                    if (expression.DeclaredType != null && !context.IsBad(expression) && !context.IsBad(expression.DeclaredType))
                    {
                        IEnumerable<EdmError> discoveredErrors;
                        ExpressionTypeChecker.TryCastRecordAsType(expression, expression.DeclaredType, null, false, out discoveredErrors);
                        foreach (EdmError error in discoveredErrors)
                        {
                            context.AddError(error);
                        }
                    }
                });

        #endregion

        #region IEdmApplyExpression

        /// <summary>
        /// Validates the types of a function application are correct.
        /// </summary>
        public static readonly ValidationRule<IEdmApplyExpression> FunctionApplicationExpressionParametersMatchAppliedFunction =
            new ValidationRule<IEdmApplyExpression>(
                (context, expression) =>
                {
                    IEdmFunction appliedFunction = expression.AppliedFunction;
                    if (appliedFunction != null && !context.IsBad(appliedFunction))
                    {
                        if (appliedFunction.Parameters.Count() != expression.Arguments.Count())
                        {
                            context.AddError(new EdmError(
                                    expression.Location(),
                                    EdmErrorCode.IncorrectNumberOfArguments,
                                    Edm.Strings.EdmModel_Validator_Semantic_IncorrectNumberOfArguments(expression.Arguments.Count(), appliedFunction.FullName(), appliedFunction.Parameters.Count())));
                        }

                        IEnumerator<IEdmExpression> parameterExpressionEnumerator = expression.Arguments.GetEnumerator();
                        foreach (IEdmOperationParameter parameter in appliedFunction.Parameters)
                        {
                            parameterExpressionEnumerator.MoveNext();
                            IEnumerable<EdmError> recursiveErrors;
                            if (!parameterExpressionEnumerator.Current.TryCast(parameter.Type, out recursiveErrors))
                            {
                                foreach (EdmError error in recursiveErrors)
                                {
                                    context.AddError(error);
                                }
                            }
                        }
                    }
                });

        #endregion

        #region IEdmVocabularyAnnotatable

        /// <summary>
        /// Validates that there are no annotations that share the same term and qualifier.
        /// </summary>
        public static readonly ValidationRule<IEdmVocabularyAnnotatable> VocabularyAnnotatableNoDuplicateAnnotations =
            new ValidationRule<IEdmVocabularyAnnotatable>(
                (context, annotatable) =>
                {
                    HashSetInternal<string> annotationSet = new HashSetInternal<string>();
                    foreach (IEdmVocabularyAnnotation annotation in context.Model.FindDeclaredVocabularyAnnotations(annotatable))
                    {
                        if (!annotationSet.Add(annotation.Term.FullName() + ":" + annotation.Qualifier))
                        {
                            context.AddError(new EdmError(
                                    annotation.Location(),
                                    EdmErrorCode.DuplicateAnnotation,
                                    Edm.Strings.EdmModel_Validator_Semantic_DuplicateAnnotation(EdmUtil.FullyQualifiedName(annotatable), annotation.Term.FullName(), annotation.Qualifier)));
                        }
                    }
                });

        #endregion

        #region IEdmPrimitiveValue

        /// <summary>
        /// Validates that if a primitive value declares a type, the value is acceptable for the type.
        /// </summary>
        public static readonly ValidationRule<IEdmPrimitiveValue> PrimitiveValueValidForType =
            new ValidationRule<IEdmPrimitiveValue>(
                (context, value) =>
                {
                    if (value.Type != null && !context.IsBad(value) && !context.IsBad(value.Type))
                    {
                        IEnumerable<EdmError> discoveredErrors;
                        ExpressionTypeChecker.TryCastPrimitiveAsType(value, value.Type, out discoveredErrors);
                        foreach (EdmError error in discoveredErrors)
                        {
                            context.AddError(error);
                        }
                    }
                });

        #endregion

        private static void CheckForUnreacheableTypeError(ValidationContext context, IEdmSchemaType type, EdmLocation location)
        {
            IEdmType foundType = context.Model.FindType(type.FullName());
            if (foundType is AmbiguousTypeBinding)
            {
                context.AddError(
                    location,
                    EdmErrorCode.BadAmbiguousElementBinding,
                    Strings.EdmModel_Validator_Semantic_AmbiguousType(type.FullName()));
            }
            else if (!foundType.IsEquivalentTo(type))
            {
                context.AddError(
                    location,
                    EdmErrorCode.BadUnresolvedType,
                    Strings.EdmModel_Validator_Semantic_InaccessibleType(type.FullName()));
            }
        }

        private static bool TryResolveNavigationPropertyBindingPath(IEdmModel model, IEdmNavigationSource navigationSource, IEdmNavigationPropertyBinding binding)
        {
            var pathSegments = binding.Path.PathSegments.ToArray();
            var definingType = navigationSource.EntityType() as IEdmStructuredType;
            for (int index = 0; index < pathSegments.Length - 1; index++)
            {
                string segment = pathSegments[index];
                if (segment.IndexOf('.') < 0)
                {
                    var property = definingType.FindProperty(segment);
                    if (property == null)
                    {
                        return false;
                    }

                    var navProperty = property as IEdmNavigationProperty;
                    if (navProperty != null && !navProperty.ContainsTarget)
                    {
                        return false;
                    }

                    definingType = property.Type.Definition.AsElementType() as IEdmStructuredType;
                    if (definingType == null)
                    {
                        return false;
                    }
                }
                else
                {
                    var derivedType = model.FindType(segment) as IEdmStructuredType;
                    if (derivedType == null || !derivedType.IsOrInheritsFrom(definingType))
                    {
                        return false;
                    }

                    definingType = derivedType;
                }
            }

            var navigationProperty = definingType.FindProperty(pathSegments.Last()) as IEdmNavigationProperty;
            return navigationProperty != null;
        }

        private static bool HasPathTypeProperty(IEdmStructuredType structuredType, IList<IEdmStructuredType> visited)
        {
            if (structuredType == null || visited == null || visited.Any(c => c == structuredType))
            {
                return false;
            }

            visited.Add(structuredType);

            IEdmStructuredType baseType = structuredType.BaseType;
            if (baseType != null)
            {
                if (HasPathTypeProperty(baseType, visited))
                {
                    return true;
                }
            }

            foreach (var property in structuredType.DeclaredProperties)
            {
                IEdmTypeReference propertyType = property.Type;
                if (propertyType.IsCollection())
                {
                    propertyType = propertyType.AsCollection().ElementType();
                }

                if (propertyType.IsStructured())
                {
                    if (HasPathTypeProperty(propertyType.AsStructured().StructuredDefinition(), visited))
                    {
                        return true;
                    }
                }
                else if (propertyType.IsPath())
                {
                    return true;
                }
            }

            return false;
        }


        internal class EdmTypeReferenceComparer : IEqualityComparer<IEdmTypeReference>
        {
            public bool Equals(IEdmTypeReference x, IEdmTypeReference y)
            {
                return x.IsEquivalentTo(y);
            }

            public int GetHashCode(IEdmTypeReference obj)
            {
                string fullName = obj.FullName();

                // Currently when operations have Row types this occurs, simply returning 0 instead to force equals comparision.
                // Code may be able to be removed when RowType is removed.
                if (fullName == null)
                {
                    return 0;
                }

                return fullName.GetHashCode();
            }
        }
    }
}
