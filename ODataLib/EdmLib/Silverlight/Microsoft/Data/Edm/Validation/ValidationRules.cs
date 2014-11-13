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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.Data.Edm.Annotations;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Expressions;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Library.Internal;
using Microsoft.Data.Edm.Validation.Internal;
using Microsoft.Data.Edm.Values;

namespace Microsoft.Data.Edm.Validation
{
    /// <summary>
    /// Built in Edm validation rules.
    /// </summary>
    public static class ValidationRules
    {
        #region IEdmElement

        /// <summary>
        /// Validates that no direct value annotations share the same name and namespace.
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
                    // Don't run this rule for IEdmDirectValueAnnotation, since these custom annotations may have names which are invalid C# identifiers, but work in XML. See bug 425714.
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
                    HashSetInternal<string> nonFunctionNameList = new HashSetInternal<string>();
                    Dictionary<string, List<IEdmFunctionImport>> functionDictionary = new Dictionary<string, List<IEdmFunctionImport>>();
                    foreach (var item in entityContainer.Elements)
                    {
                        IEdmFunctionImport function = item as IEdmFunctionImport;
                        if (function != null)
                        {
                            if (nonFunctionNameList.Contains(item.Name))
                            {
                                context.AddError(
                                    item.Location(),
                                    EdmErrorCode.DuplicateEntityContainerMemberName,
                                    Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
                            }

                            List<IEdmFunctionImport> functionList;
                            if (functionDictionary.TryGetValue(function.Name, out functionList))
                            {
                                foreach (IEdmFunctionImport existingFunction in functionList)
                                {
                                    if (function.IsFunctionSignatureEquivalentTo(existingFunction))
                                    {
                                        context.AddError(
                                            item.Location(),
                                            EdmErrorCode.DuplicateEntityContainerMemberName,
                                            Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                functionList = new List<IEdmFunctionImport>();
                            }

                            functionList.Add(function);
                        }
                        else
                        {
                            if (ValidationHelper.AddMemberNameToHashSet(
                                item,
                                nonFunctionNameList,
                                context,
                                EdmErrorCode.DuplicateEntityContainerMemberName,
                                Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name),
                                /*supressError*/ false))
                            {
                                if (functionDictionary.ContainsKey(item.Name))
                                {
                                    context.AddError(
                                        item.Location(),
                                        EdmErrorCode.DuplicateEntityContainerMemberName,
                                        Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
                                }
                            }
                        }
                    }
                });

        #endregion

        #region IEdmEntitySet

        /// <summary>
        /// Validates that there is no entity set whose entity type has no key.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetTypeHasNoKeys =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    if ((entitySet.ElementType.Key() == null || entitySet.ElementType.Key().Count() == 0) && !context.IsBad(entitySet.ElementType))
                    {
                        string errorMessage = Strings.EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys(
                            entitySet.Name, 
                            entitySet.ElementType.Name);

                        context.AddError(
                            entitySet.Location(),
                            EdmErrorCode.EntitySetTypeHasNoKeys,
                            errorMessage);
                    }
                });

        /// <summary>
        /// Validates that the entity type of an entity set can be found from the model being validated.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetInaccessibleEntityType =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    if (!context.IsBad(entitySet.ElementType))
                    {
                        CheckForUnreacheableTypeError(context, entitySet.ElementType, entitySet.Location());
                    }
                });

        /// <summary>
        /// Validates that an entity set can only have a single navigation property targetting it that has Contains set to true.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetCanOnlyBeContainedByASingleNavigationProperty =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    bool containmentFound = false;
                    foreach (IEdmEntitySet otherSet in set.Container.EntitySets())
                    {
                        foreach (IEdmNavigationTargetMapping mapping in otherSet.NavigationTargets)
                        {
                            IEdmNavigationProperty property = mapping.NavigationProperty;

                            if (mapping.TargetEntitySet == set && property.ContainsTarget)
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
        /// Validates that if a navigation property is traversed to another entity set, and then the navigation properties partner is traversed, the destination will be the source entity set.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetNavigationMappingMustBeBidirectional =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    foreach (IEdmNavigationTargetMapping mapping in set.NavigationTargets)
                    {
                        IEdmNavigationProperty property = mapping.NavigationProperty;

                        IEdmEntitySet opposingTarget = mapping.TargetEntitySet.FindNavigationTarget(property.Partner);

                        // If the navigation property is not silent, or if it is silent but is still mapped, it must be mapped correctly.
                        if ((opposingTarget != null || property.Partner.DeclaringEntityType().FindProperty(property.Partner.Name) == property.Partner) && opposingTarget != set)
                        {
                            context.AddError(
                                set.Location(),
                                EdmErrorCode.EntitySetNavigationMappingMustBeBidirectional,
                                Strings.EdmModel_Validator_Semantic_EntitySetNavigationMappingMustBeBidirectional(set.Container.FullName() + "." + set.Name, property.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates an association set's name is correct
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetAssociationSetNameMustBeValid =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    foreach (IEdmNavigationTargetMapping mapping in set.NavigationTargets)
                    {
                        if (mapping.NavigationProperty.GetPrimary() == mapping.NavigationProperty)
                        {
                            CheckForNameError(context, context.Model.GetAssociationSetName(set, mapping.NavigationProperty), set.Location());
                        }
                    }
                });

        /// <summary>
        /// Validates that no navigation property is mapped to two different entity sets.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetNavigationPropertyMappingsMustBeUnique =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    foreach (IEdmNavigationTargetMapping mapping in set.NavigationTargets)
                    {
                        HashSetInternal<IEdmNavigationProperty> mappedPropertySet = new HashSetInternal<IEdmNavigationProperty>();
                        if (!mappedPropertySet.Add(mapping.NavigationProperty))
                        {
                            context.AddError(
                                set.Location(),
                                EdmErrorCode.DuplicateNavigationPropertyMapping,
                                Strings.EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping(set.Container.FullName() + "." + set.Name, mapping.NavigationProperty.Name));
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
                    foreach (IEdmNavigationTargetMapping mapping in set.NavigationTargets)
                    {
                        if (mapping.NavigationProperty.ContainsTarget &&
                            mapping.NavigationProperty.DeclaringType.IsOrInheritsFrom(mapping.NavigationProperty.ToEntityType()) &&
                            mapping.TargetEntitySet != set)
                        {
                            context.AddError(
                                set.Location(),
                                EdmErrorCode.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet,
                                Strings.EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet(mapping.NavigationProperty, set.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that the target of a navigation property mapping is valid for the target type of the property.
        /// </summary>
        public static readonly ValidationRule<IEdmEntitySet> EntitySetNavigationPropertyMappingMustPointToValidTargetForProperty =
            new ValidationRule<IEdmEntitySet>(
                (context, set) =>
                {
                    foreach (IEdmNavigationTargetMapping mapping in set.NavigationTargets)
                    {
                        if (!(mapping.TargetEntitySet.ElementType.IsOrInheritsFrom(mapping.NavigationProperty.ToEntityType()) || mapping.NavigationProperty.ToEntityType().IsOrInheritsFrom(mapping.TargetEntitySet.ElementType)) && !context.IsBad(mapping.TargetEntitySet))
                        {
                            context.AddError(
                                set.Location(),
                                EdmErrorCode.EntitySetNavigationPropertyMappingMustPointToValidTargetForProperty,
                                Strings.EdmModel_Validator_Semantic_EntitySetNavigationPropertyMappingMustPointToValidTargetForProperty(mapping.NavigationProperty.Name, mapping.TargetEntitySet.Name));
                        }
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
                    // We can either have 2 rules (entity and complex) or have one rule and exclude row type. I'm choosing the latter.
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

        /// <summary>
        /// Open types are supported only in version 1.2 and after version 2.0.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> OpenTypesNotSupported =
           new ValidationRule<IEdmStructuredType>(
               (context, structuredType) =>
               {
                   if (structuredType.IsOpen)
                   {
                       context.AddError(
                           structuredType.Location(),
                           EdmErrorCode.OpenTypeNotSupported,
                           Strings.EdmModel_Validator_Semantic_OpenTypesSupportedOnlyInV12AndAfterV3);
                   }
               });

        /// <summary>
        /// Open types are supported only on entity types.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuredType> OnlyEntityTypesCanBeOpen =
           new ValidationRule<IEdmStructuredType>(
               (context, structuredType) =>
               {
                   if (structuredType.IsOpen && structuredType.TypeKind != EdmTypeKind.Entity)
                   {
                       context.AddError(
                           structuredType.Location(),
                           EdmErrorCode.OpenTypeNotSupported,
                           Strings.EdmModel_Validator_Semantic_OpenTypesSupportedForEntityTypesOnly);
                   }
               });

        #endregion

        #region IEdmEnumType

        /// <summary>
        /// Raises an error if an enum type is found.
        /// </summary>
        public static readonly ValidationRule<IEdmEnumType> EnumTypeEnumsNotSupportedBeforeV3 =
           new ValidationRule<IEdmEnumType>(
               (context, enumType) =>
               {
                   context.AddError(
                       enumType.Location(),
                       EdmErrorCode.EnumsNotSupportedBeforeV3,
                       Strings.EdmModel_Validator_Semantic_EnumsNotSupportedBeforeV3);
               });

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
                       !context.IsBad(enumMember.DeclaringType.UnderlyingType) &&
                       !enumMember.Value.TryAssertPrimitiveAsType(enumMember.DeclaringType.UnderlyingType.GetPrimitiveTypeReference(false), out discoveredErrors))
                   {
                       context.AddError(
                           enumMember.Location(),
                           EdmErrorCode.EnumMemberTypeMustMatchEnumUnderlyingType,
                           Strings.EdmModel_Validator_Semantic_EnumMemberTypeMustMatchEnumUnderlyingType(enumMember.Name));
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
                            if (!key.Type.IsPrimitive() && !context.IsBad(key))
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
        /// Validates that no part of an entity key is a binary primitive type.
        /// </summary>
        public static readonly ValidationRule<IEdmEntityType> EntityTypeEntityKeyMustNotBeBinaryBeforeV2 =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.Key() != null)
                    {
                        foreach (IEdmStructuralProperty key in entityType.Key())
                        {
                            if (key.Type.IsBinary() && !context.IsBad(key.Type.Definition))
                            {
                                context.AddError(
                                key.Location(),
                                EdmErrorCode.EntityKeyMustNotBeBinary,
                                Strings.EdmModel_Validator_Semantic_EntityKeyMustNotBeBinaryBeforeV2(key.Name, entityType.Name));
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
                    if ((entityType.Key() == null || entityType.Key().Count() == 0) && entityType.BaseType == null)
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
        /// Validates that a complex type is not abstract.
        /// </summary>
        public static readonly ValidationRule<IEdmComplexType> ComplexTypeInvalidAbstractComplexType =
            new ValidationRule<IEdmComplexType>(
                (context, complexType) =>
                {
                    if (complexType.IsAbstract)
                    {
                        context.AddError(
                            complexType.Location(),
                            EdmErrorCode.InvalidAbstractComplexType,
                            Strings.EdmModel_Validator_Semantic_InvalidComplexTypeAbstract(complexType.FullName()));
                    }
                });

        /// <summary>
        /// Validates that a complex type does not inherit.
        /// </summary>
        public static readonly ValidationRule<IEdmComplexType> ComplexTypeInvalidPolymorphicComplexType =
            new ValidationRule<IEdmComplexType>(
                (context, edmComplexType) =>
                {
                    if (edmComplexType.BaseType != null)
                    {
                        context.AddError(
                            edmComplexType.Location(),
                            EdmErrorCode.InvalidPolymorphicComplexType,
                            Strings.EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic(edmComplexType.FullName()));
                    }
                });

        /// <summary>
        /// Validates that a complex type contains at least one property.
        /// </summary>
        public static readonly ValidationRule<IEdmComplexType> ComplexTypeMustContainProperties =
            new ValidationRule<IEdmComplexType>(
                (context, complexType) =>
                {
                    if (!complexType.Properties().Any())
                    {
                        context.AddError(
                            complexType.Location(),
                            EdmErrorCode.ComplexTypeMustHaveProperties,
                            Strings.EdmModel_Validator_Semantic_ComplexTypeMustHaveProperties(complexType.FullName()));
                    }
                });

        #endregion

        #region IEdmRowType

        /// <summary>
        /// Validates that a row type does not have a base type.
        /// </summary>
        public static readonly ValidationRule<IEdmRowType> RowTypeBaseTypeMustBeNull =
            new ValidationRule<IEdmRowType>(
                (context, rowType) =>
                {
                    if (rowType.BaseType != null)
                    {
                        context.AddError(
                            rowType.Location(),
                            EdmErrorCode.RowTypeMustNotHaveBaseType,
                            Strings.EdmModel_Validator_Semantic_RowTypeMustNotHaveBaseType);
                    }
                });

        /// <summary>
        /// Validates that a row type contains at least one property.
        /// </summary>
        public static readonly ValidationRule<IEdmRowType> RowTypeMustContainProperties =
            new ValidationRule<IEdmRowType>(
                (context, rowType) =>
                {
                    if (!rowType.Properties().Any())
                    {
                        context.AddError(
                            rowType.Location(),
                            EdmErrorCode.RowTypeMustHaveProperties,
                            Strings.EdmModel_Validator_Semantic_RowTypeMustHaveProperties);
                    }
                });

        #endregion

        #region IEdmStructuralProperty

        /// <summary>
        /// Validates that any property with a complex type is not nullable.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuralProperty> StructuralPropertyNullableComplexType =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    if (property.Type.IsComplex() && property.Type.IsNullable)
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.NullableComplexTypeProperty,
                        Strings.EdmModel_Validator_Semantic_NullableComplexTypeProperty(property.Name));
                    }
                });

        /// <summary>
        /// Validates that the property is of an allowed type.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuralProperty> StructuralPropertyInvalidPropertyType =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    if (property.DeclaringType.TypeKind != EdmTypeKind.Row)
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

                        if (validatedType.TypeKind != EdmTypeKind.Primitive &&
                            validatedType.TypeKind != EdmTypeKind.Enum &&
                            validatedType.TypeKind != EdmTypeKind.Complex &&
                            !context.IsBad(validatedType))
                        {
                            context.AddError(
                            property.Location(),
                            EdmErrorCode.InvalidPropertyType,
                            Strings.EdmModel_Validator_Semantic_InvalidPropertyType(property.Type.TypeKind().ToString()));
                        }
                    }
                });

        /// <summary>
        /// Validates that if the concurrency mode of a property is fixed, the type is primitive.
        /// </summary>
        public static readonly ValidationRule<IEdmStructuralProperty> StructuralPropertyInvalidPropertyTypeConcurrencyMode =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    if (property.ConcurrencyMode == EdmConcurrencyMode.Fixed &&
                        !property.Type.IsPrimitive() &&
                        !context.IsBad(property.Type.Definition))
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.InvalidPropertyType,
                        Strings.EdmModel_Validator_Semantic_InvalidPropertyTypeConcurrencyMode((property.Type.IsCollection() ? EdmConstants.Type_Collection : property.Type.TypeKind().ToString())));
                    }
                });

        #endregion

        #region IEdmNavigationProperty

        /// <summary>
        /// Validates that only one end of an association has an OnDelete operation.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyInvalidOperationMultipleEndsInAssociation =
           new ValidationRule<IEdmNavigationProperty>(
               (context, navigationProperty) =>
               {
                   if (navigationProperty.OnDelete != EdmOnDeleteAction.None && navigationProperty.Partner.OnDelete != EdmOnDeleteAction.None)
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
                    bool isBad = false;

                    if (property.ToEntityType() != property.Partner.DeclaringEntityType())
                    {
                        isBad = true;
                    }
                    else
                    {
                        switch (property.Partner.Multiplicity())
                        {
                            case EdmMultiplicity.Many:
                                if (!property.Type.IsCollection())
                                {
                                    isBad = true;
                                }

                                break;
                            case EdmMultiplicity.ZeroOrOne:
                                if (property.Type.IsCollection() || !property.Type.IsNullable)
                                {
                                    isBad = true;
                                }

                                break;
                            case EdmMultiplicity.One:
                                if (property.Type.IsCollection() || property.Type.IsNullable)
                                {
                                    isBad = true;
                                }

                                break;
                        }
                    }

                    if (isBad)
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
                    IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties;
                    if (dependentProperties != null)
                    {
                        HashSetInternal<string> propertyNames = new HashSetInternal<string>();
                        foreach (var property in navigationProperty.DependentProperties)
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

                  IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties;
                  if (dependentProperties != null)
                  {
                      if (ValidationHelper.AllPropertiesAreNullable(dependentProperties))
                      {
                          if (navigationProperty.Partner.Multiplicity() != EdmMultiplicity.ZeroOrOne)
                          {
                              context.AddError(
                                  navigationProperty.Partner.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfPrincipalEnd,
                                  Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable(navigationProperty.Partner.Name, navigationProperty.Name));
                          }
                      }
                      else if (!ValidationHelper.HasNullableProperty(dependentProperties))
                      {
                          if (navigationProperty.Partner.Multiplicity() != EdmMultiplicity.One)
                          {
                              context.AddError(
                                  navigationProperty.Partner.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfPrincipalEnd,
                                  Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable(navigationProperty.Partner.Name, navigationProperty.Name));
                          }
                      }
                      else
                      {
                          if (navigationProperty.Partner.Multiplicity() != EdmMultiplicity.One &&
                              navigationProperty.Partner.Multiplicity() != EdmMultiplicity.ZeroOrOne)
                          {
                              context.AddError(
                                  navigationProperty.Partner.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfPrincipalEnd,
                                  Strings.EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne(navigationProperty.Name));
                          }
                      }
                  }
              });

        /// <summary>
        /// Validates that if the dependent properties are equivalent to the key of the dependent end, the multiplicity of the dependent end cannot be 1
        /// Validates multiplicity of the dependent end according to the following rules:
        /// 0..1, 1 - if dependent properties represent the dependent end key.
        ///       * - if dependent properties don't represent the dependent end key.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyDependentEndMultiplicity =
          new ValidationRule<IEdmNavigationProperty>(
              (context, navigationProperty) =>
              {
                  IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties;
                  if (dependentProperties != null)
                  {
                      if (ValidationHelper.PropertySetsAreEquivalent(navigationProperty.DeclaringEntityType().Key(), dependentProperties))
                      {
                          if (navigationProperty.Multiplicity() != EdmMultiplicity.ZeroOrOne &&
                              navigationProperty.Multiplicity() != EdmMultiplicity.One)
                          {
                              context.AddError(
                                  navigationProperty.Location(),
                                  EdmErrorCode.InvalidMultiplicityOfDependentEnd,
                                  Strings.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne(navigationProperty.Name));
                          }
                      }
                      else if (navigationProperty.Multiplicity() != EdmMultiplicity.Many)
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
                   IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties;
                   if (dependentProperties != null)
                   {
                       IEdmEntityType dependentEntity = navigationProperty.DeclaringEntityType();
                       foreach (IEdmStructuralProperty dependantProperty in dependentProperties)
                       {
                           if (!context.IsBad(dependantProperty))
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
        /// Validates that all dependent properties are a subset of the dependent entity types key.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyInvalidToPropertyInRelationshipConstraintBeforeV2 =
            new ValidationRule<IEdmNavigationProperty>(
            (context, navigationProperty) =>
            {
                IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties;
                if (dependentProperties != null && !ValidationHelper.PropertySetIsSubset(navigationProperty.DeclaringEntityType().Key(), dependentProperties))
                {
                    string errorMessage = Strings.EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint(
                        navigationProperty.Name,
                        navigationProperty.DeclaringEntityType().FullName());

                    context.AddError(
                        navigationProperty.Location(),
                        EdmErrorCode.InvalidPropertyInRelationshipConstraint,
                        errorMessage);
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
                   if (end.Multiplicity() == EdmMultiplicity.Many &&
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
        /// Validates that <see cref="IEdmNavigationProperty.ContainsTarget"/> is not set prior to V3.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyContainsTargetNotSupportedBeforeV3 =
           new ValidationRule<IEdmNavigationProperty>(
               (context, property) =>
               {
                   if (property.ContainsTarget != CsdlConstants.Default_ContainsTarget)
                   {
                       context.AddError(
                           property.Location(),
                           EdmErrorCode.NavigationPropertyContainsTargetNotSupportedBeforeV3,
                           Strings.EdmModel_Validator_Semantic_NavigationPropertyContainsTargetNotSupportedBeforeV3);
                   }
               });

        /// <summary>
        /// Validates that if a navigation property has <see cref="IEdmNavigationProperty.ContainsTarget"/> = true and the target entity type is the same as 
        /// the declaring type of the property, then the multiplicity of the target of navigation is 0..1 or Many.
        /// This depends on there being a targetting cycle. Because of the rule <see cref="EntitySetNavigationMappingMustBeBidirectional" />, we know that either this is always true, or there will be an error
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
        /// This depends on there being a targetting cycle. Because of the rule <see cref="EntitySetNavigationMappingMustBeBidirectional" />, we know that either this is always true, or there will be an error
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (property.ContainsTarget &&
                        property.DeclaringType.IsOrInheritsFrom(property.ToEntityType()) &&
                        property.Multiplicity() != EdmMultiplicity.ZeroOrOne)
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
                    if (property.ContainsTarget &&
                        !property.DeclaringType.IsOrInheritsFrom(property.ToEntityType()) &&
                        property.Multiplicity() != EdmMultiplicity.One)
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
        /// Validates that each pair of properties between the dependent properties and the principal ends key are of the same type.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyTypeMismatchRelationshipConstraint =
          new ValidationRule<IEdmNavigationProperty>(
              (context, navigationProperty) =>
              {
                  IEnumerable<IEdmStructuralProperty> dependentProperties = navigationProperty.DependentProperties;
                  if (dependentProperties != null)
                  {
                      int dependentPropertiesCount = dependentProperties.Count();
                      IEdmEntityType principalEntityType = navigationProperty.Partner.DeclaringEntityType();
                      IEnumerable<IEdmStructuralProperty> principalKey = principalEntityType.Key();
                      if (dependentPropertiesCount == principalKey.Count())
                      {
                          for (int i = 0; i < dependentPropertiesCount; i++)
                          {
                              if (!navigationProperty.DependentProperties.ElementAtOrDefault(i).Type.Definition.IsEquivalentTo(principalKey.ElementAtOrDefault(i).Type.Definition))
                              {
                                  string errorMessage = Strings.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(
                                      navigationProperty.DependentProperties.ToList()[i].Name,
                                      navigationProperty.DeclaringEntityType().FullName(),
                                      principalKey.ToList()[i].Name,
                                      principalEntityType.Name,
                                      "Fred");

                                  context.AddError(
                                      navigationProperty.Location(),
                                      EdmErrorCode.TypeMismatchRelationshipConstraint,
                                      errorMessage);
                              }
                          }
                      }
                  }
              });

        /// <summary>
        /// Validates that an association name is valid.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyAssociationNameIsValid =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (property.IsPrincipal)
                    {
                        CheckForNameError(context, context.Model.GetAssociationName(property), property.Location());
                    }
                });

        /// <summary>
        /// Validates that an association end name is valid.
        /// </summary>
        public static readonly ValidationRule<IEdmNavigationProperty> NavigationPropertyAssociationEndNameIsValid =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    CheckForNameError(context, context.Model.GetAssociationEndName(property), property.Location());
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

        #endregion

        #region IEdmFunction

        /// <summary>
        /// Raises an error if a function is found.
        /// </summary>
        public static readonly ValidationRule<IEdmFunction> FunctionsNotSupportedBeforeV2 =
           new ValidationRule<IEdmFunction>(
               (context, function) =>
               {
                   context.AddError(
                       function.Location(),
                       EdmErrorCode.FunctionsNotSupportedBeforeV2,
                       Strings.EdmModel_Validator_Semantic_FunctionsNotSupportedBeforeV2);
               });

        /// <summary>
        /// Validates that no function parameters are output parameters.
        /// </summary>
        public static readonly ValidationRule<IEdmFunction> FunctionOnlyInputParametersAllowedInFunctions =
            new ValidationRule<IEdmFunction>(
                (context, function) =>
                {
                    foreach (IEdmFunctionParameter parameter in function.Parameters)
                    {
                        if (parameter.Mode != EdmFunctionParameterMode.In)
                        {
                            context.AddError(
                                parameter.Location(),
                                EdmErrorCode.OnlyInputParametersAllowedInFunctions,
                                Strings.EdmModel_Validator_Semantic_OnlyInputParametersAllowedInFunctions(parameter.Name, function.Name));
                        }
                    }
                });

        #endregion

        #region IEdmFunctionImport

        /// <summary>
        /// Validates that a function import has an allowed return type.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportUnsupportedReturnTypeV1 =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.ReturnType != null)
                   {
                       bool reportError = false;
                       if (functionImport.ReturnType.IsCollection())
                       {
                           IEdmTypeReference elementType = functionImport.ReturnType.AsCollection().ElementType();
                           reportError = !elementType.IsPrimitive() && !elementType.IsEntity() && !context.IsBad(elementType.Definition);
                       }
                       else
                       {
                           reportError = true;
                       }

                       if (reportError && !context.IsBad(functionImport.ReturnType.Definition))
                       {
                           context.AddError(
                               functionImport.Location(),
                               EdmErrorCode.FunctionImportUnsupportedReturnType,
                               Strings.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1(functionImport.Name));
                       }
                   }
               });

        /// <summary>
        /// Validates that a function import has an allowed return type.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportUnsupportedReturnTypeAfterV1 =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.ReturnType != null)
                    {
                        IEdmTypeReference elementType = functionImport.ReturnType.IsCollection() ? functionImport.ReturnType.AsCollection().ElementType() : functionImport.ReturnType;
                        if (!elementType.IsPrimitive() && !elementType.IsEntity() && !elementType.IsComplex() && !elementType.IsEnum() && !context.IsBad(elementType.Definition))
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportUnsupportedReturnType,
                                Strings.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeAfterV1(functionImport.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that if a function import specifies an entity or collection of entities as its return type, it must also specify an entity set.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.ReturnType != null && functionImport.EntitySet == null)
                   {
                        IEdmTypeReference elementType = functionImport.ReturnType.IsCollection() ? functionImport.ReturnType.AsCollection().ElementType() : functionImport.ReturnType;
                        if (elementType.IsEntity() && !context.IsBad(elementType.Definition))
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportReturnsEntitiesButDoesNotSpecifyEntitySet,
                                Strings.EdmModel_Validator_Semantic_FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet(functionImport.Name));
                        }
                   }
               });

        /// <summary>
        /// Validates that the entity set of a function import is defined using a path or an entity set reference expression.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportEntitySetExpressionIsInvalid =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.EntitySet != null)
                    {
                        if (functionImport.EntitySet.ExpressionKind != Expressions.EdmExpressionKind.EntitySetReference &&
                            functionImport.EntitySet.ExpressionKind != Expressions.EdmExpressionKind.Path)
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportEntitySetExpressionIsInvalid,
                                Strings.EdmModel_Validator_Semantic_FunctionImportEntitySetExpressionKindIsInvalid(functionImport.Name, functionImport.EntitySet.ExpressionKind));
                        }
                        else
                        {
                            IEdmEntitySet entitySet;
                            IEdmFunctionParameter parameter;
                            IEnumerable<IEdmNavigationProperty> path;
                            if (!functionImport.TryGetStaticEntitySet(out entitySet) &&
                                !functionImport.TryGetRelativeEntitySetPath(context.Model, out parameter, out path))
                            {
                                context.AddError(
                                    functionImport.Location(),
                                    EdmErrorCode.FunctionImportEntitySetExpressionIsInvalid,
                                    Strings.EdmModel_Validator_Semantic_FunctionImportEntitySetExpressionIsInvalid(functionImport.Name));
                            }
                        }
                    }
                });

        /// <summary>
        /// Validates that the return type of a function import must match the type of the entity set of the function.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportEntityTypeDoesNotMatchEntitySet =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.EntitySet != null && functionImport.ReturnType != null)
                    {
                        IEdmTypeReference elementType = functionImport.ReturnType.IsCollection() ? functionImport.ReturnType.AsCollection().ElementType() : functionImport.ReturnType;
                        if (elementType.IsEntity())
                        {
                            IEdmEntityType returnedEntityType = elementType.AsEntity().EntityDefinition();

                            IEdmEntitySet entitySet;
                            IEdmFunctionParameter parameter;
                            IEnumerable<IEdmNavigationProperty> path;
                            if (functionImport.TryGetStaticEntitySet(out entitySet))
                            {
                                string errorMessage = Strings.EdmModel_Validator_Semantic_FunctionImportEntityTypeDoesNotMatchEntitySet(
                                    functionImport.Name,
                                    returnedEntityType.FullName(),
                                    entitySet.Name);

                                IEdmEntityType entitySetElementType = entitySet.ElementType;
                                if (!returnedEntityType.IsOrInheritsFrom(entitySetElementType) && !context.IsBad(returnedEntityType) && !context.IsBad(entitySet) && !context.IsBad(entitySetElementType))
                                {
                                    context.AddError(
                                        functionImport.Location(),
                                        EdmErrorCode.FunctionImportEntityTypeDoesNotMatchEntitySet,
                                        errorMessage);
                                }
                            }
                            else if (functionImport.TryGetRelativeEntitySetPath(context.Model, out parameter, out path))
                            {
                                List<IEdmNavigationProperty> pathList = path.ToList();
                                IEdmTypeReference relativePathType = pathList.Count == 0 ? parameter.Type : path.Last().Type;
                                IEdmTypeReference relativePathElementType = relativePathType.IsCollection() ? relativePathType.AsCollection().ElementType() : relativePathType;
                                if (!returnedEntityType.IsOrInheritsFrom(relativePathElementType.Definition) && !context.IsBad(returnedEntityType) && !context.IsBad(relativePathElementType.Definition))
                                {
                                    context.AddError(
                                        functionImport.Location(),
                                        EdmErrorCode.FunctionImportEntityTypeDoesNotMatchEntitySet,
                                        Strings.EdmModel_Validator_Semantic_FunctionImportEntityTypeDoesNotMatchEntitySet2(functionImport.Name, elementType.FullName()));
                                }
                            }

                            // The case when all try gets fail is caught by the FunctionImportEntitySetExpressionIsInvalid rule.
                        }
                        else if (!context.IsBad(elementType.Definition))
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportSpecifiesEntitySetButDoesNotReturnEntityType,
                                Strings.EdmModel_Validator_Semantic_FunctionImportSpecifiesEntitySetButNotEntityType(functionImport.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that if a function import is composable, it must have a return type.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> ComposableFunctionImportMustHaveReturnType =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.IsComposable && functionImport.ReturnType == null)
                    {
                        context.AddError(
                            functionImport.Location(),
                            EdmErrorCode.ComposableFunctionImportMustHaveReturnType,
                            Strings.EdmModel_Validator_Semantic_ComposableFunctionImportMustHaveReturnType(functionImport.Name));
                    }
                });

        /// <summary>
        /// Validates that the type of a function imports parameter is correct.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportParametersIncorrectTypeBeforeV3 =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    foreach (var functionParameter in functionImport.Parameters)
                    {
                        var type = functionParameter.Type;
                        if (!type.IsPrimitive() && !type.IsComplex() && !context.IsBad(type.Definition))
                        {
                            context.AddError(
                                functionParameter.Location(),
                                EdmErrorCode.FunctionImportParameterIncorrectType,
                                Strings.EdmModel_Validator_Semantic_FunctionImportParameterIncorrectType(type.FullName(), functionParameter.Name));
                        }
                    }
                });

        /// <summary>
        /// Validates that a function import is not sideeffecting.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportIsSideEffectingNotSupportedBeforeV3 =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.IsSideEffecting != CsdlConstants.Default_IsSideEffecting)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.FunctionImportSideEffectingNotSupportedBeforeV3,
                           Strings.EdmModel_Validator_Semantic_FunctionImportSideEffectingNotSupportedBeforeV3);
                   }
               });

        /// <summary>
        /// Validates that a function import is not composable.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportIsComposableNotSupportedBeforeV3 =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.IsComposable != CsdlConstants.Default_IsComposable)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.FunctionImportComposableNotSupportedBeforeV3,
                           Strings.EdmModel_Validator_Semantic_FunctionImportComposableNotSupportedBeforeV3);
                   }
               });

        /// <summary>
        /// Validates that a function is not bindable.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportIsBindableNotSupportedBeforeV3 =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.IsBindable != CsdlConstants.Default_IsBindable)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.FunctionImportBindableNotSupportedBeforeV3,
                           Strings.EdmModel_Validator_Semantic_FunctionImportBindableNotSupportedBeforeV3);
                   }
               });

        /// <summary>
        /// Validates that if a function is composable, it is not also sideeffecting.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportComposableFunctionImportCannotBeSideEffecting =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.IsComposable && functionImport.IsSideEffecting)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.ComposableFunctionImportCannotBeSideEffecting,
                           Strings.EdmModel_Validator_Semantic_ComposableFunctionImportCannotBeSideEffecting(functionImport.Name));
                   }
               });

        /// <summary>
        /// Validates that if a function is bindable, it must have parameters.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportBindableFunctionImportMustHaveParameters =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.IsBindable && functionImport.Parameters.Count() == 0)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.BindableFunctionImportMustHaveParameters,
                           Strings.EdmModel_Validator_Semantic_BindableFunctionImportMustHaveParameters(functionImport.Name));
                   }
               });

        /// <summary>
        /// Validates that no function import parameters have mode of none.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionImport> FunctionImportParametersCannotHaveModeOfNone =
            new ValidationRule<IEdmFunctionImport>(
                (context, function) =>
                {
                    foreach (IEdmFunctionParameter parameter in function.Parameters)
                    {
                        if (parameter.Mode == EdmFunctionParameterMode.None && !context.IsBad(function))
                        {
                            context.AddError(
                                parameter.Location(),
                                EdmErrorCode.InvalidFunctionImportParameterMode,
                                Strings.EdmModel_Validator_Semantic_InvalidFunctionImportParameterMode(parameter.Name, function.Name));
                        }
                    }
                });

        #endregion

        #region IEdmFunctionBase

        /// <summary>
        /// Validates that a function does not have multiple parameters with the same name.
        /// </summary>
        public static readonly ValidationRule<IEdmFunctionBase> FunctionBaseParameterNameAlreadyDefinedDuplicate =
           new ValidationRule<IEdmFunctionBase>(
               (context, edmFunction) =>
               {
                   HashSetInternal<string> parameterList = new HashSetInternal<string>();
                   if (edmFunction.Parameters != null)
                   {
                       foreach (var parameter in edmFunction.Parameters)
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

        #region IEdmPrimitiveTypeReference

        /// <summary>
        /// References to EDM stream type are not supported before version 3.0.
        /// </summary>
        public static readonly ValidationRule<IEdmPrimitiveTypeReference> StreamTypeReferencesNotSupportedBeforeV3 =
            new ValidationRule<IEdmPrimitiveTypeReference>(
                (context, type) =>
                {
                    if (type.IsStream())
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.StreamTypeReferencesNotSupportedBeforeV3,
                            Strings.EdmModel_Validator_Semantic_StreamTypeReferencesNotSupportedBeforeV3);
                    }
                });

        /// <summary>
        /// References to EDM spatial types are not supported before version 3.0.
        /// </summary>
        public static readonly ValidationRule<IEdmPrimitiveTypeReference> SpatialTypeReferencesNotSupportedBeforeV3 =
            new ValidationRule<IEdmPrimitiveTypeReference>(
                (context, type) =>
                {
                    if (type.IsSpatial())
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.SpatialTypeReferencesNotSupportedBeforeV3,
                            Strings.EdmModel_Validator_Semantic_SpatialTypeReferencesNotSupportedBeforeV3);
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
        /// Validates every schema element in the current model (except for entity containers) is unique across all referenced models.
        /// </summary>
        public static readonly ValidationRule<IEdmModel> ModelDuplicateSchemaElementNameBeforeV3 =
           new ValidationRule<IEdmModel>(
               (context, model) =>
               {
                   HashSetInternal<string> nonFunctionNameList = new HashSetInternal<string>();
                   Dictionary<string, List<IEdmFunction>> functionDictionary = new Dictionary<string, List<IEdmFunction>>();
                   foreach (IEdmSchemaElement item in model.SchemaElements)
                   {
                       bool duplicate = false;
                       string fullName = item.FullName();
                       if (item.SchemaElementKind != EdmSchemaElementKind.EntityContainer)
                       {
                           IEdmFunction function = item as IEdmFunction;
                           if (function != null)
                           {
                               // If a non-function already exists with the same name, stop processing as a function, as it is irrelevant it will always be an error.
                               if (nonFunctionNameList.Contains(fullName))
                               {
                                   duplicate = true;
                               }
                               else
                               {
                                   List<IEdmFunction> functionList;
                                   if (functionDictionary.TryGetValue(fullName, out functionList))
                                   {
                                       if (functionList.Any(existingFunction => function.IsFunctionSignatureEquivalentTo(existingFunction)))
                                       {
                                           duplicate = true;
                                       }
                                   }
                                   else
                                   {
                                       functionList = new List<IEdmFunction>();
                                       functionDictionary[fullName] = functionList;
                                   }

                                   functionList.Add(function);
                               }

                               if (!duplicate)
                               {
                                   duplicate = model.FunctionOrNameExistsInReferencedModel(function, fullName, false);
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
                                   if (functionDictionary.ContainsKey(fullName))
                                   {
                                       duplicate = true;
                                   }
                                   else
                                   {
                                       duplicate = model.ItemExistsInReferencedModel(fullName, false);
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
                   }
               });

        /// <summary>
        /// Validates every schema element in the current model is unique across all referenced models.
        /// </summary>
        public static readonly ValidationRule<IEdmModel> ModelDuplicateSchemaElementName =
           new ValidationRule<IEdmModel>(
               (context, model) =>
               {
                   HashSetInternal<string> nonFunctionNameList = new HashSetInternal<string>();
                   Dictionary<string, List<IEdmFunction>> functionDictionary = new Dictionary<string, List<IEdmFunction>>();
                   foreach (var item in model.SchemaElements)
                   {
                       bool duplicate = false;
                       string fullName = item.FullName();
                       IEdmFunction function = item as IEdmFunction;
                       if (function != null)
                       {
                           // If a non-function already exists with the same name, stop processing as a function, as it is irrelevant it will always be an error.
                           if (nonFunctionNameList.Contains(fullName))
                           {
                               duplicate = true;
                           }
                           else
                           {
                               List<IEdmFunction> functionList;
                               if (functionDictionary.TryGetValue(fullName, out functionList))
                               {
                                   if (functionList.Any(existingFunction => function.IsFunctionSignatureEquivalentTo(existingFunction)))
                                   {
                                       duplicate = true;
                                   }
                               }
                               else
                               {
                                   functionList = new List<IEdmFunction>();
                                   functionDictionary[fullName] = functionList;
                               }

                               functionList.Add(function);
                           }

                           if (!duplicate)
                           {
                               duplicate = model.FunctionOrNameExistsInReferencedModel(function, fullName, true);
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
                               if (functionDictionary.ContainsKey(fullName))
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
                    foreach (var item in model.EntityContainers())
                    {
                        ValidationHelper.AddMemberNameToHashSet(
                            item,
                            entityContainerNameList,
                            context,
                            EdmErrorCode.DuplicateEntityContainerName,
                            Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerName(item.Name),
                            /*supressError*/ false);
                    }
                });

        #endregion

        #region IEdmImmediateValueAnnotation

        /// <summary>
        /// Validates that an immediate value annotation has a name and a namespace.
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
        /// Validates that an immediate value annotation that is flagged to be serialized as an element can be serialized safely.
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
        /// Validates that the name of a direct value annotation can safely be serialized as XML.
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
                            catch (Exception)
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
        /// Vocabulary annotations are not supported before EDM 3.0.
        /// </summary>
        public static readonly ValidationRule<IEdmVocabularyAnnotation> VocabularyAnnotationsNotSupportedBeforeV3 =
           new ValidationRule<IEdmVocabularyAnnotation>(
               (context, vocabularyAnnotation) =>
               {
                   context.AddError(
                       vocabularyAnnotation.Location(),
                       EdmErrorCode.VocabularyAnnotationsNotSupportedBeforeV3,
                       Strings.EdmModel_Validator_Semantic_VocabularyAnnotationsNotSupportedBeforeV3);
               });

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
                                foundTarget = (container.FindEntitySet(entitySet.Name) != null);
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
                                    foundTarget = (context.Model.FindValueTerm(term.FullName()) as IEdmTerm != null);
                                }
                                else
                                {
                                    IEdmFunction function = target as IEdmFunction;
                                    if (function != null)
                                    {
                                        foundTarget = context.Model.FindFunctions(function.FullName()).Any();
                                    }
                                    else
                                    {
                                        IEdmFunctionImport functionImport = target as IEdmFunctionImport;
                                        if (functionImport != null)
                                        {
                                            foundTarget = functionImport.Container.FindFunctionImports(functionImport.Name).Any();
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
                                                IEdmFunctionParameter functionParameter = target as IEdmFunctionParameter;
                                                if (functionParameter != null)
                                                {
                                                    IEdmFunction declaringFunction = functionParameter.DeclaringFunction as IEdmFunction;
                                                    if (declaringFunction != null)
                                                    {
                                                        // For all functions with this name declared in the model check if it has a parameter with this name
                                                        foreach (var func in context.Model.FindFunctions(declaringFunction.FullName()))
                                                        {
                                                            if (func.FindParameter(functionParameter.Name) != null)
                                                            {
                                                                foundTarget = true;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        IEdmFunctionImport declaringFunctionImport = functionParameter.DeclaringFunction as IEdmFunctionImport;
                                                        if (declaringFunctionImport != null)
                                                        {
                                                            var container = declaringFunctionImport.Container as IEdmEntityContainer;
                                                            foreach (var currentFunction in container.FindFunctionImports(declaringFunctionImport.Name))
                                                            {
                                                                if (currentFunction.FindParameter(functionParameter.Name) != null)
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

        #endregion

        #region IEdmValueAnnotation

        /// <summary>
        /// Validates that if a value annotation declares a type, the expression for that annotation has the correct type.
        /// </summary>
        public static readonly ValidationRule<IEdmValueAnnotation> ValueAnnotationAssertCorrectExpressionType =
            new ValidationRule<IEdmValueAnnotation>(
                (context, annotation) =>
                {
                    IEnumerable<EdmError> errors;
                    if (!annotation.Value.TryAssertType(((IEdmValueTerm)annotation.Term).Type, out errors))
                    {
                        foreach (EdmError error in errors)
                        {
                            context.AddError(error);
                        }
                    }
                });

        /// <summary>
        /// Validates that a vocabulary annotations term can be found through the model containing the annotation.
        /// </summary>
        public static readonly ValidationRule<IEdmValueAnnotation> ValueAnnotationInaccessibleTerm =
            new ValidationRule<IEdmValueAnnotation>(
                (context, annotation) =>
                {
                    // An unbound term is not treated as a semantic error, and looking up its name would fail.
                    IEdmTerm term = annotation.Term;
                    if (!(term is Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics.IUnresolvedElement) && context.Model.FindValueTerm(term.FullName()) == null)
                    {
                        context.AddError(
                            annotation.Location(),
                            EdmErrorCode.BadUnresolvedTerm,
                            Strings.EdmModel_Validator_Semantic_InaccessibleTerm(annotation.Term.FullName()));
                    }
                });

        #endregion

        #region IEdmTypeAnnotation

        /// <summary>
        /// Validates that a vocabulary annotations term can be found through the model containing the annotation.
        /// </summary>
        public static readonly ValidationRule<IEdmTypeAnnotation> TypeAnnotationInaccessibleTerm =
            new ValidationRule<IEdmTypeAnnotation>(
                (context, annotation) =>
                {
                    // An unbound term is not treated as a semantic error, and looking up its name would fail.
                    IEdmTerm term = annotation.Term;
                    if (!(term is Microsoft.Data.Edm.Csdl.Internal.CsdlSemantics.IUnresolvedElement) && context.Model.FindType(term.FullName()) as IEdmStructuredType == null)
                    {
                        context.AddError(
                            annotation.Location(),
                            EdmErrorCode.BadUnresolvedTerm,
                            Strings.EdmModel_Validator_Semantic_InaccessibleTerm(annotation.Term.FullName()));
                    }
                });

        /// <summary>
        /// Validates that a type annotation implements its term type properly.
        /// </summary>
        public static readonly ValidationRule<IEdmTypeAnnotation> TypeAnnotationAssertMatchesTermType =
            new ValidationRule<IEdmTypeAnnotation>(
                (context, annotation) =>
                {
                    IEdmStructuredType type = (IEdmStructuredType)annotation.Term;

                    HashSetInternal<IEdmProperty> foundProperties = new HashSetInternal<IEdmProperty>();

                    foreach (IEdmProperty typeProperty in type.Properties())
                    {
                        IEdmPropertyValueBinding annotationProperty = annotation.FindPropertyBinding(typeProperty);
                        if (annotationProperty == null)
                        {
                            context.AddError(new EdmError(annotation.Location(), EdmErrorCode.TypeAnnotationMissingRequiredProperty, Edm.Strings.EdmModel_Validator_Semantic_TypeAnnotationMissingRequiredProperty(typeProperty.Name)));
                        }
                        else
                        {
                            foundProperties.Add(typeProperty);
                        }
                    }

                    if (!type.IsOpen)
                    {
                        foreach (IEdmPropertyValueBinding property in annotation.PropertyValueBindings)
                        {
                            if (!foundProperties.Contains(property.BoundProperty) && !context.IsBad(property))
                            {
                                context.AddError(new EdmError(property.Location(), EdmErrorCode.TypeAnnotationHasExtraProperties, Edm.Strings.EdmModel_Validator_Semantic_TypeAnnotationHasExtraProperties(property.BoundProperty.Name)));
                            }
                        }
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
                    if (!binding.Value.TryAssertType(binding.BoundProperty.Type, out errors) &&
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

        #region IEdmValueTerm

        /// <summary>
        /// Value terms are not supported before EDM 3.0.
        /// </summary>
        public static readonly ValidationRule<IEdmValueTerm> ValueTermsNotSupportedBeforeV3 =
           new ValidationRule<IEdmValueTerm>(
               (context, valueTerm) =>
               {
                   context.AddError(
                       valueTerm.Location(),
                       EdmErrorCode.ValueTermsNotSupportedBeforeV3,
                       Strings.EdmModel_Validator_Semantic_ValueTermsNotSupportedBeforeV3);
               });

        #endregion

        #region IEdmTerm

        /// <summary>
        /// A term without other errors must not have kind of none.
        /// </summary>
        public static readonly ValidationRule<IEdmTerm> TermMustNotHaveKindOfNone =
            new ValidationRule<IEdmTerm>(
                (context, term) =>
                {
                    if (term.TermKind == EdmTermKind.None && !context.IsBad(term))
                    {
                        context.AddError(
                        term.Location(),
                        EdmErrorCode.TermMustNotHaveKindOfNone,
                        Strings.EdmModel_Validator_Semantic_TermMustNotHaveKindOfNone(term.FullName()));
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
                    if (!expression.TestExpression.TryAssertType(EdmCoreModel.Instance.GetBoolean(false), out errors))
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
                        ExpressionTypeChecker.TryAssertCollectionAsType(expression, expression.DeclaredType, null, false, out discoveredErrors);
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
                        ExpressionTypeChecker.TryAssertRecordAsType(expression, expression.DeclaredType,  null, false, out discoveredErrors);
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
                    IEdmFunctionReferenceExpression functionReference = expression.AppliedFunction as IEdmFunctionReferenceExpression;
                    if (functionReference.ReferencedFunction != null && !context.IsBad(functionReference.ReferencedFunction))
                    {
                        if (functionReference.ReferencedFunction.Parameters.Count() != expression.Arguments.Count())
                        {
                            context.AddError(new EdmError(
                                    expression.Location(),
                                    EdmErrorCode.IncorrectNumberOfArguments,
                                    Edm.Strings.EdmModel_Validator_Semantic_IncorrectNumberOfArguments(expression.Arguments.Count(), functionReference.ReferencedFunction.FullName(), functionReference.ReferencedFunction.Parameters.Count())));
                        }

                        IEnumerator<IEdmExpression> parameterExpressionEnumerator = expression.Arguments.GetEnumerator();
                        foreach (IEdmFunctionParameter parameter in functionReference.ReferencedFunction.Parameters)
                        {
                            parameterExpressionEnumerator.MoveNext();
                            IEnumerable<EdmError> recursiveErrors;
                            if (!parameterExpressionEnumerator.Current.TryAssertType(parameter.Type, out recursiveErrors))
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
                    foreach (IEdmVocabularyAnnotation annotation in annotatable.VocabularyAnnotations(context.Model))
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
                        ExpressionTypeChecker.TryAssertPrimitiveAsType(value, value.Type, out discoveredErrors);
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

        private static void CheckForNameError(ValidationContext context, string name, EdmLocation location)
        {
            if (EdmUtil.IsNullOrWhiteSpaceInternal(name) || name.Length == 0)
            {
                context.AddError(
                    location,
                    EdmErrorCode.InvalidName,
                    Strings.EdmModel_Validator_Syntactic_MissingName);
            }
            else if (name.Length > CsdlConstants.Max_NameLength)
            {
                context.AddError(
                    location,
                    EdmErrorCode.NameTooLong,
                    Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(name));
            }
            else if (!EdmUtil.IsValidUndottedName(name))
            {
                context.AddError(
                    location,
                    EdmErrorCode.InvalidName,
                    Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(name));
            }
        }
    }
}
