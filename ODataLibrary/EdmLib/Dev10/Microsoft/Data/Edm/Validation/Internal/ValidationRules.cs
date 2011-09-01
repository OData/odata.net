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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Csdl;
using Microsoft.Data.Edm.Internal;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Validation.Internal
{
    internal static class ValidationRules
    {
        #region IEdmNamedElement

        internal static readonly ValidationRule<IEdmNamedElement> IEdmNamedElement_NameMustNotBeEmptyOrWhiteSpace =
            new ValidationRule<IEdmNamedElement>(
                (context, item) =>
                {
                    if (EdmUtil.IsNullOrWhiteSpaceInternal(item.Name) || item.Name.Length == 0)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidName,
                            Edm.Strings.EdmModel_Validator_Syntactic_MissingName);
                    }
                });

        internal static readonly ValidationRule<IEdmNamedElement> IEdmNamedElement_NameIsTooLong =
            new ValidationRule<IEdmNamedElement>(
                (context, item) =>
                {
                    if (item.Name.Length > CsdlConstants.Max_NameLength)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.NameTooLong,
                            Edm.Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(item.Name));
                    }
                });

        internal static readonly ValidationRule<IEdmNamedElement> IEdmNamedElement_NameIsNotAllowed =
            new ValidationRule<IEdmNamedElement>(
                (context, item) =>
                {
                    if (item.Name.Length <= CsdlConstants.Max_NameLength && item.Name.Length > 0 && !EdmUtil.IsNullOrWhiteSpaceInternal(item.Name))
                    {
                        if (!EdmUtil.IsValidDataModelItemName(item.Name))
                        {
                            context.AddError(
                                item.Location(),
                                EdmErrorCode.InvalidName,
                                Edm.Strings.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(item.Name));
                        }
                    }
                });

        #endregion

        #region IEdmSchemaElement

        internal static readonly ValidationRule<IEdmSchemaElement> IEdmSchemaElement_NamespaceMustNotBeEmptyOrWhiteSpace =
            new ValidationRule<IEdmSchemaElement>(
                (context, item) =>
                {
                    if (EdmUtil.IsNullOrWhiteSpaceInternal(item.Namespace) || item.Namespace.Length == 0)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidNamespaceName,
                            Edm.Strings.EdmModel_Validator_Syntactic_MissingNamespaceName);
                    }
                });

        internal static readonly ValidationRule<IEdmSchemaElement> IEdmSchemaElement_NamespaceIsTooLong =
            new ValidationRule<IEdmSchemaElement>(
                (context, item) =>
                {
                    if (item.Namespace.Length > CsdlConstants.Max_NamespaceLength)
                    {
                        context.AddError(
                            item.Location(),
                            EdmErrorCode.InvalidNamespaceName,
                            Edm.Strings.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong(
                                item.Namespace));
                    }
                });

        internal static readonly ValidationRule<IEdmSchemaElement> IEdmSchemaElement_NamespaceIsNotAllowed =
            new ValidationRule<IEdmSchemaElement>(
                (context, item) =>
                {
                    // max length is hard coded in the xsd
                    if (item.Namespace.Length <= CsdlConstants.Max_NamespaceLength && item.Namespace.Length > 0 && !EdmUtil.IsNullOrWhiteSpaceInternal(item.Namespace))
                    {
                        if (!EdmUtil.IsValidQualifiedItemNamespace(item.Namespace))
                        {
                            context.AddError(
                                item.Location(),
                                EdmErrorCode.InvalidNamespaceName,
                                Edm.Strings.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed(item.Namespace));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmSchemaElement> IEdmSchemaElement_SystemNamespaceEncountered =
            new ValidationRule<IEdmSchemaElement>(
                (context, element) =>
                {
                    if (ValidationHelper.IsEdmSystemNamespace(element.Namespace))
                    {
                        context.AddError(
                            element.Location(),
                            EdmErrorCode.SystemNamespaceEncountered,
                            Edm.Strings.EdmModel_Validator_Semantic_SystemNamespaceEncountered(element.Namespace));
                    }
                });

        #endregion

        #region EdmEntityContainer

        internal static readonly ValidationRule<IEdmEntityContainer> IEdmEntityContainer_SimilarRelationshipEnd =
            new ValidationRule<IEdmEntityContainer>(
                (context, entityContainer) =>
                {
                    List<KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd>> endList = new List<KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd>>();
                    foreach (var set in entityContainer.AssociationSets())
                    {
                        KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd> end1 = new KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd>(set, set.End1);
                        KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd> end2 = new KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd>(set, set.End2);

                        var existEnd1 = endList.FirstOrDefault(e => ValidationHelper.AreRelationshipEndsEqual(e, end1));
                        var existEnd2 = endList.FirstOrDefault(e => ValidationHelper.AreRelationshipEndsEqual(e, end2));

                        if (!existEnd1.Equals(default(KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd>)))
                        {
                            context.AddError(
                                entityContainer.Location(),
                                EdmErrorCode.SimilarRelationshipEnd,
                                Edm.Strings.EdmModel_Validator_Semantic_SimilarRelationshipEnd(
                                    existEnd1.Value.Role.Name,
                                    existEnd1.Key.Name,
                                    set.Name,
                                    existEnd1.Value.EntitySet.Name,
                                    entityContainer.Name)
                                );
                        }
                        else
                        {
                            endList.Add(end1);
                        }

                        if (!existEnd2.Equals(default(KeyValuePair<IEdmAssociationSet, IEdmAssociationSetEnd>)))
                        {
                            context.AddError(
                                entityContainer.Location(),
                                EdmErrorCode.SimilarRelationshipEnd,
                                Edm.Strings.EdmModel_Validator_Semantic_SimilarRelationshipEnd(
                                    existEnd2.Value.Role.Name,
                                    existEnd2.Key.Name,
                                    set.Name,
                                    existEnd2.Value.EntitySet.Name,
                                    entityContainer.Name)
                                );
                        }
                        else
                        {
                            endList.Add(end2);
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmEntityContainer> IEdmEntityContainer_DuplicateEntityContainerMemberName =
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
                                    Edm.Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
                            }

                            List<IEdmFunctionImport> functionList;
                            if (functionDictionary.TryGetValue(function.Name, out functionList))
                            {
                                foreach (IEdmFunctionImport existingFunction in functionList)
                                {
                                    if (function.IsEquivalentTo(existingFunction))
                                    {
                                        context.AddError(
                                            item.Location(),
                                            EdmErrorCode.DuplicateEntityContainerMemberName,
                                            Edm.Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
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
                                        Edm.Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name),
                                /*supressError*/ false))
                            {
                                if (functionDictionary.ContainsKey(item.Name))
                                {
                                    context.AddError(
                                        item.Location(),
                                        EdmErrorCode.DuplicateEntityContainerMemberName,
                                        Edm.Strings.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(item.Name));
                                }
                            }
                        }
                    }
                });

        #endregion

        #region IEdmEntitySet

        internal static readonly ValidationRule<IEdmEntitySet> IEdmEntitySet_EntitySetTypeHasNoKeys =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    if (entitySet.ElementType.Key() == null || entitySet.ElementType.Key().Count() == 0)
                    {
                        context.AddError(
                            entitySet.Location(),
                            EdmErrorCode.EntitySetTypeHasNoKeys,
                            Edm.Strings.EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys(
                                entitySet.Name, entitySet.ElementType.Name)
                            );
                    }
                });

        internal static readonly ValidationRule<IEdmEntitySet> IEdmEntitySet_InaccessibleEntityType =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    if (context.FindTypeInModel(entitySet.ElementType.FullName()) != entitySet.ElementType)
                    {
                        context.AddError(
                            entitySet.Location(),
                            EdmErrorCode.BadUnresolvedType,
                            Edm.Strings.EdmModel_Validator_Semantic_InaccessibleType(entitySet.ElementType.FullName()));
                    }
                });

        internal static readonly ValidationRule<IEdmEntitySet> IEdmEntitySet_SimilarRelationshipEnd =
            new ValidationRule<IEdmEntitySet>(
                (context, entitySet) =>
                {
                    if (entitySet.ElementType.IsAbstract)
                    {
                        context.AddError(
                            entitySet.Location(),
                            EdmErrorCode.AbstractEntitiesCannotBeInstantiated,
                            Edm.Strings.EdmModel_Validator_Semantic_AbstractEntitiesCannotBeInstantiated(
                                entitySet.ElementType.Name, entitySet.Name)
                            );
                    }
                });

        #endregion

        #region IEdmAssociationSet

        // Todo: when associations are not types, we need to use FindAssociation rather than find type
        internal static readonly ValidationRule<IEdmAssociationSet> IEdmAssociationSet_InaccessibleAssociation =
            new ValidationRule<IEdmAssociationSet>(
                (context, associationSet) =>
                {
                    if (context.Model.FindAssociation(associationSet.Association.FullName()) != associationSet.Association)
                    {
                        context.AddError(
                            associationSet.Location(),
                            EdmErrorCode.BadUnresolvedType,
                            Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_InaccessibleAssociation(associationSet.Association.FullName()));
                    }
                });

        internal static readonly ValidationRule<IEdmAssociationSet> IEdmAssociationSet_DuplicateEndName =
            new ValidationRule<IEdmAssociationSet>(
                (context, associationSet) =>
                {
                    if (associationSet.End1.Role.Name == associationSet.End2.Role.Name)
                    {
                        context.AddError(
                            associationSet.Location(),
                            EdmErrorCode.InvalidName,
                            Edm.Strings.EdmModel_Validator_Semantic_DuplicateEndName(associationSet.End1.Role.Name));
                    }
                });

        #endregion

        #region IEdmAssociationSetEnd

        internal static readonly ValidationRule<IEdmAssociationSetEnd> IEdmAssociationSetEnd_EntitySetMustBeOfCorrectEntityType =
            new ValidationRule<IEdmAssociationSetEnd>(
                (context, end) =>
                {
                    if (end.EntitySet.ElementType != end.Role.EntityType && !end.Role.EntityType.InheritsFrom(end.EntitySet.ElementType))
                    {
                        context.AddError(
                            end.Location(),
                            EdmErrorCode.InvalidAssociationSetEndSetWrongType,
                            Edm.Strings.EdmModel_Validator_Semantic_InvalidAssociationSetEndSetWrongType(end.EntitySet.ElementType.FullName(), end.EntitySet.Name, end.Role.Name, end.Role.EntityType.Name));
                    }
                });

        #endregion

        #region IEdmStructuralType

        internal static readonly ValidationRule<IEdmStructuredType> IEdmStructuredType_InvalidMemberNameMatchesTypeName =
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
                                        Edm.Strings.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName(property.Name, schemaType.FullName())
                                        );
                                    }
                                }
                            }
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmStructuredType> IEdmStructuredType_PropertyNameAlreadyDefined =
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
                                Edm.Strings.EdmModel_Validator_Semantic_PropertyNameAlreadyDefined(property.Name),
                                /*supressError*/ !structuredType.DeclaredProperties.Contains(property));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmStructuredType> IEdmStructuredType_BaseTypeMustBeSameKindAsDerivedKind =
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
                                Edm.Strings.EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind);
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmStructuredType> IEdmStructuredType_InaccessibleBaseType =
            new ValidationRule<IEdmStructuredType>(
                (context, structuredType) =>
                {
                    IEdmSchemaType schemaBaseType = structuredType.BaseType as IEdmSchemaType;
                    if (schemaBaseType != null)
                    {
                        if (context.FindTypeInModel(schemaBaseType.FullName()) != schemaBaseType)
                        {
                            context.AddError(
                                structuredType.Location(),
                                EdmErrorCode.BadUnresolvedType,
                                Edm.Strings.EdmModel_Validator_Semantic_InaccessibleAssociation(schemaBaseType.FullName()));
                        }
                    }
                });

        #endregion

        #region IEdmEntityType

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_DuplicatePropertyNameSpecifiedInEntityKey =
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
                                Edm.Strings.EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey(entityType.Name, item.Name),
                                /*supressError*/ false);
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_InvalidKeyNullablePart =
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
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidKeyNullablePart(key.Name, entityType.Name));
                                }
                            }
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_EntityKeyMustBeScalar =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.Key() != null)
                    {
                        foreach (var key in entityType.Key())
                        {
                            if (!key.Type.IsBad())
                            {
                                if (!key.Type.IsPrimitive())
                                {
                                    context.AddError(
                                    key.Location(),
                                    EdmErrorCode.EntityKeyMustBeScalar,
                                    Edm.Strings.EdmModel_Validator_Semantic_EntityKeyMustBeScalar(key.Name, entityType.Name));
                                }
                            }
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_EntityKeyMustNotBeBinaryBeforeV2 =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.Key() != null)
                    {
                        foreach (var key in entityType.Key())
                        {
                            if (!key.Type.IsBad() && key.Type.IsBinary())
                            {
                                context.AddError(
                                key.Location(),
                                EdmErrorCode.EntityKeyMustNotBeBinary,
                                Edm.Strings.EdmModel_Validator_Semantic_EntityKeyMustNotBeBinaryBeforeV2(key.Name, entityType.Name));
                            }
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_InvalidKeyKeyDefinedInBaseClass =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.BaseType != null &&
                        entityType.DeclaredKey != null &&
                         entityType.BaseEntityType().DeclaredKey != null)
                    {
                        context.AddError(
                        entityType.Location(),
                        EdmErrorCode.InvalidKey,
                        Edm.Strings.EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass(entityType.Name, entityType.BaseEntityType().Name));
                    }
                });

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_KeyMissingOnEntityType =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.Key() == null || entityType.Key().Count() == 0)
                    {
                        context.AddError(
                        entityType.Location(),
                        EdmErrorCode.KeyMissingOnEntityType,
                        Edm.Strings.EdmModel_Validator_Semantic_KeyMissingOnEntityType(entityType.Name));
                    }
                });

        #endregion

        #region IEdmEntityReferenceType

        internal static readonly ValidationRule<IEdmEntityReferenceType> IEdmEntityReferenceType_InaccessibleEntityType =
            new ValidationRule<IEdmEntityReferenceType>(
                (context, entityReferenceType) =>
                {
                    if (context.FindTypeInModel(entityReferenceType.EntityType.FullName()) != entityReferenceType.EntityType)
                    {
                        context.AddError(
                            entityReferenceType.Location(),
                            EdmErrorCode.BadUnresolvedType,
                            Edm.Strings.EdmModel_Validator_Semantic_InaccessibleType(entityReferenceType.EntityType.FullName()));
                    }
                });

        #endregion

        #region IEdmComplexType

        internal static readonly ValidationRule<IEdmComplexType> IEdmComplexType_InvalidIsAbstractV1 =
            new ValidationRule<IEdmComplexType>(
                (context, complexType) =>
                {
                    if (complexType.IsAbstract)
                    {
                        context.AddError(complexType.Location(),
                                         EdmErrorCode.InvalidAbstractComplexType,
                                         Edm.Strings.EdmModel_Validator_Semantic_InvalidComplexTypeAbstract(complexType.FullName()));
                    }
                });

        internal static readonly ValidationRule<IEdmComplexType> IEdmComplexType_InvalidIsPolymorphicV1 =
            new ValidationRule<IEdmComplexType>(
                (context, edmComplexType) =>
                {
                    if (edmComplexType.BaseType != null)
                    {
                        context.AddError(edmComplexType.Location(),
                                         EdmErrorCode.InvalidPolymorphicComplexType,
                                         Edm.Strings.EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic(edmComplexType.FullName()));
                    }
                });

        #endregion

        #region IEdmAssociationType

        internal static readonly ValidationRule<IEdmAssociation> IEdmAssociationType_InvalidOperationMultipleEndsInAssociation =
           new ValidationRule<IEdmAssociation>(
               (context, associationType) =>
               {
                   if (associationType.End1.OnDelete != EdmOnDeleteAction.None && associationType.End2.OnDelete != EdmOnDeleteAction.None)
                   {
                       context.AddError(
                           associationType.Location(),
                           EdmErrorCode.InvalidAction,
                           Edm.Strings.EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation);
                   }
               });

        internal static readonly ValidationRule<IEdmAssociation> IEdmAssociationType_EndNameAlreadyDefinedDuplicate =
            new ValidationRule<IEdmAssociation>(
                (context, associationType) =>
                {
                    if (associationType.End1.Name == associationType.End2.Name)
                    {
                        context.AddError(
                            associationType.End1.Location(),
                            EdmErrorCode.AlreadyDefined,
                            Edm.Strings.EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate(associationType.End1.Name));
                    }
                });

        #endregion

        #region IEdmAssociationEnd

        internal static readonly ValidationRule<IEdmAssociationEnd> IEdmAssociationEnd_EndWithManyMultiplicityCannotHaveOperationsSpecified =
           new ValidationRule<IEdmAssociationEnd>(
               (context, end) =>
               {
                   // If an end has a multiplicity of many, it cannot have any operation behaviour
                   if (end.Multiplicity == EdmAssociationMultiplicity.Many &&
                       end.OnDelete != EdmOnDeleteAction.None)
                   {
                       context.AddError(
                           end.Location(),
                           EdmErrorCode.EndWithManyMultiplicityCannotHaveOperationsSpecified,
                           Edm.Strings.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified(
                               end.Name,
                               end.DeclaringAssociation.FullName()));
                   }
               });

        internal static readonly ValidationRule<IEdmAssociationEnd> IEdmAssociationEnd_InaccessibleEntityType =
            new ValidationRule<IEdmAssociationEnd>(
                (context, end) =>
                {
                    if (context.FindTypeInModel(end.EntityType.FullName()) != end.EntityType)
                    {
                        context.AddError(
                            end.Location(),
                            EdmErrorCode.BadUnresolvedType,
                            Edm.Strings.EdmModel_Validator_Semantic_InaccessibleType(end.EntityType.FullName()));
                    }
                });

        #endregion

        #region IEdmRowType

        internal static readonly ValidationRule<IEdmRowType> IEdmRowType_BaseTypeMustBeNull =
            new ValidationRule<IEdmRowType>(
                (context, rowType) =>
                {
                    if (rowType.BaseType != null)
                    {
                        context.AddError(
                            rowType.Location(),
                            EdmErrorCode.RowTypeMustNotHaveBaseType,
                            Edm.Strings.EdmModel_Validator_Semantic_RowTypeMustNotHaveBaseType);
                    }
                });

        internal static readonly ValidationRule<IEdmRowType> IEdmRowType_MustContainProperties =
            new ValidationRule<IEdmRowType>(
                (context, rowType) =>
                {
                    if (rowType.Properties().Count() == 0)
                    {
                        context.AddError(
                            rowType.Location(),
                            EdmErrorCode.RowTypeMustHaveProperties,
                            Edm.Strings.EdmModel_Validator_Semantic_RowTypeMustHaveProperties);
                    }
                });

        #endregion

        #region IEdmReferentialConstriant

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidMultiplicityFromRoleUpperBoundMustBeOne =
           new ValidationRule<IEdmReferentialConstraint>(
               (context, referentialConstraint) =>
               {
                   if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                   {
                       if (referentialConstraint.PrincipalEnd.Multiplicity == EdmAssociationMultiplicity.Many)
                       {
                           context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleUpperBoundMustBeOne(referentialConstraint.PrincipalEnd.Name, referentialConstraint.PrincipalEnd.DeclaringAssociation.Name));
                       }
                   }
               });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNullable =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (ValidationHelper.AllPropertiesAreNullable(referentialConstraint.DependentProperties)
                          && referentialConstraint.PrincipalEnd.Multiplicity == EdmAssociationMultiplicity.One)
                      {
                          context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNullableV1(referentialConstraint.PrincipalEnd.Name, referentialConstraint.PrincipalEnd.DeclaringAssociation.Name));
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNonNullableV1 =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (!ValidationHelper.AllPropertiesAreNullable(referentialConstraint.DependentProperties)
                          && !(referentialConstraint.PrincipalEnd.Multiplicity == EdmAssociationMultiplicity.One))
                      {
                          context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV1(referentialConstraint.PrincipalEnd.Name, referentialConstraint.PrincipalEnd.DeclaringAssociation.Name));
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNonNullableV2 =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (!ValidationHelper.HasNullableProperty(referentialConstraint.DependentProperties)
                          && !(referentialConstraint.PrincipalEnd.Multiplicity == EdmAssociationMultiplicity.One))
                      {
                          context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV2(referentialConstraint.PrincipalEnd.Name, referentialConstraint.PrincipalEnd.DeclaringAssociation.Name));
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidToPropertyInRelationshipConstraintBeforeV2 =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (!ValidationHelper.PropertySetIsSubset(referentialConstraint.DependentEnd().EntityType.Key(), referentialConstraint.DependentProperties))
                      {
                          context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidPropertyInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint(
                                        referentialConstraint.DependentEnd().Name, referentialConstraint.DependentEnd().EntityType.FullName(), referentialConstraint.PrincipalEnd.DeclaringAssociation.FullName()));
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidMultiplicityToRoleUpperBoundMustBeOne =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (ValidationHelper.PropertySetsAreEquivalent(referentialConstraint.DependentEnd().EntityType.Key(), referentialConstraint.DependentProperties)
                          && referentialConstraint.DependentEnd().Multiplicity == EdmAssociationMultiplicity.Many)
                      {
                          context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeOne(referentialConstraint.DependentEnd().Name, referentialConstraint.PrincipalEnd.DeclaringAssociation.Name));
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_InvalidMultiplicityToRoleUpperBoundMustBeMany =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (!ValidationHelper.PropertySetsAreEquivalent(referentialConstraint.DependentEnd().EntityType.Key(), referentialConstraint.DependentProperties)
                          && referentialConstraint.DependentEnd().Multiplicity != EdmAssociationMultiplicity.Many)
                      {
                          context.AddError(
                                    referentialConstraint.Location(),
                                    EdmErrorCode.InvalidMultiplicityInRoleInRelationshipConstraint,
                                    Edm.Strings.EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeMany(referentialConstraint.DependentEnd().Name, referentialConstraint.PrincipalEnd.DeclaringAssociation.Name));
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_TypeMismatchRelationshipConstraint =
          new ValidationRule<IEdmReferentialConstraint>(
              (context, referentialConstraint) =>
              {
                  if (ValidationHelper.ReferentialConstraintReadyForValidation(referentialConstraint))
                  {
                      if (referentialConstraint.DependentProperties.Count() == referentialConstraint.PrincipalEnd.EntityType.Key().Count())
                      {
                          for (int i = 0; i < referentialConstraint.DependentProperties.Count(); i++)
                          {
                              if (!referentialConstraint.DependentProperties.ElementAtOrDefault(i).Type.Definition.IsEquivalentTo(referentialConstraint.PrincipalEnd.EntityType.Key().ElementAtOrDefault(i).Type.Definition))
                              {
                                  context.AddError(
                                         referentialConstraint.Location(),
                                         EdmErrorCode.TypeMismatchRelationshipConstraint,
                                         Edm.Strings.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(
                                                                        referentialConstraint.DependentProperties.ToList()[i].Name,
                                                                        referentialConstraint.DependentEnd().EntityType.FullName(),
                                                                        referentialConstraint.PrincipalEnd.EntityType.DeclaredKey.ToList()[i].Name,
                                                                        referentialConstraint.PrincipalEnd.EntityType.Name,
                                                                        referentialConstraint.PrincipalEnd.DeclaringAssociation.Name
                                                                        ));
                              }
                          }
                      }
                  }
              });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_DuplicateDependentProperty =
            new ValidationRule<IEdmReferentialConstraint>(
                (context, referentialConstraint) =>
                {
                    HashSetInternal<string> propertyNames = new HashSetInternal<string>();
                    foreach (var property in referentialConstraint.DependentProperties)
                    {
                        if (property != null)
                        {
                            ValidationHelper.AddMemberNameToHashSet(
                                property,
                                propertyNames,
                                context,
                                EdmErrorCode.DuplicateDependentProperty,
                                Edm.Strings.EdmModel_Validator_Semantic_DuplicateDependentProperty(referentialConstraint.PrincipalEnd.DeclaringAssociation.Name, property.Name),
                                /*supressError*/ false);
                        }
                    }
                });

        #endregion

        #region IEdmStructuralProperty

        internal static readonly ValidationRule<IEdmStructuralProperty> IEdmStructuralProperty_NullableComplexType =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    if (property.Type.IsComplex() && property.Type.IsNullable)
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.NullableComplexType,
                        Edm.Strings.EdmModel_Validator_Semantic_NullableComplexType(property.Name));
                    }
                });

        internal static readonly ValidationRule<IEdmStructuralProperty> IEdmStructuralProperty_InvalidPropertyType =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    if (property.DeclaringType.TypeKind == EdmTypeKind.Row &&
                        (property.Type.TypeKind() == EdmTypeKind.None))
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.InvalidPropertyType,
                        Edm.Strings.EdmModel_Validator_Semantic_InvalidRowPropertyType((property.Type.IsCollection()
                                                                                            ? EdmConstants.Type_Collection
                                                                                            : property.Type.TypeKind().ToString())));
                    }
                    else
                    {
                        if (property.Type.TypeKind() != EdmTypeKind.Primitive &&
                            property.Type.TypeKind() != EdmTypeKind.Enum &&
                            property.Type.TypeKind() != EdmTypeKind.Complex)
                        {
                            context.AddError(
                            property.Location(),
                            EdmErrorCode.InvalidPropertyType,
                            Edm.Strings.EdmModel_Validator_Semantic_InvalidPropertyType((property.Type.IsCollection() ? EdmConstants.Type_Collection : property.Type.TypeKind().ToString())));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmStructuralProperty> IEdmStructuralProperty_InvalidPropertyTypeConcurrencyMode =
            new ValidationRule<IEdmStructuralProperty>(
                (context, property) =>
                {
                    if (property.ConcurrencyMode == EdmConcurrencyMode.Fixed &&
                        !property.Type.IsPrimitive())
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.InvalidPropertyType,
                        Edm.Strings.EdmModel_Validator_Semantic_InvalidPropertyTypeConcurrencyMode((property.Type.IsCollection() ? EdmConstants.Type_Collection : property.Type.TypeKind().ToString())));
                    }
                });

        #endregion

        #region IEdmNavigationProperty

        internal static readonly ValidationRule<IEdmNavigationProperty> IEdmNavigationProperty_InaccessibleAssociation =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (context.Model.FindAssociation(property.To.DeclaringAssociation.FullName()) != property.To.DeclaringAssociation)
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.BadUnresolvedType,
                        Edm.Strings.EdmModel_Validator_Semantic_InaccessibleAssociation(property.To.DeclaringAssociation.FullName()));
                    }
                });

        internal static readonly ValidationRule<IEdmNavigationProperty> IEdmNavigationProperty_CorrectType =
            new ValidationRule<IEdmNavigationProperty>(
                (context, property) =>
                {
                    if (!property.Type.IsEntity() && !property.Type.IsCollection() && !property.Type.AsCollection().ElementType().IsEntity())
                    {
                        context.AddError(
                        property.Location(),
                        EdmErrorCode.InvalidNavigationPropertyType,
                        Edm.Strings.EdmModel_Validator_Semantic_InvalidNavigationPropertyType(property.Name));
                    }
                });

        #endregion

        #region IEdmFunction

        internal static readonly ValidationRule<IEdmFunction> IEdmFunction_FunctionsNotSupportedBeforeV2 =
           new ValidationRule<IEdmFunction>(
               (context, function) =>
               {
                   context.AddError(
                       function.Location(),
                       EdmErrorCode.FunctionsNotSupportedBeforeV2,
                       Edm.Strings.EdmModel_Validator_Semantic_FunctionsNotSupportedBeforeV2);
               });

        internal static readonly ValidationRule<IEdmFunction> IEdmFunction_OnlyInputParametersAllowedInFunctions =
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
                                Edm.Strings.EdmModel_Validator_Semantic_OnlyInputParametersAllowedInFunctions(
                                    parameter.Name, function.Name
                                ));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmFunction> IEdmFunction_FunctionReturnTypeIncorrectType =
            new ValidationRule<IEdmFunction>(
                (context, function) =>
                {
                    if (ReportTypeErrorForFunction(function.ReturnType))
                    {
                        context.AddError(
                            function.Location(),
                            EdmErrorCode.FunctionReturnTypeIncorrectType,
                            Edm.Strings.EdmModel_Validator_Semantic_FunctionReturnTypeIncorrectType(
                                function.ReturnType.FullName(), function.FullName()));
                    }
                });

        internal static readonly ValidationRule<IEdmFunction> IEdmFunction_ParametersIncorrectType =
            new ValidationRule<IEdmFunction>(
                (context, function) =>
                {
                    foreach (var functionParameter in function.Parameters)
                    {
                        if (ReportTypeErrorForFunction(functionParameter.Type))
                        {
                            context.AddError(
                                functionParameter.Location(),
                                EdmErrorCode.FunctionParameterIncorrectType,
                                Edm.Strings.EdmModel_Validator_Semantic_FunctionParameterIncorrectType(
                                    functionParameter.Type.FullName(), functionParameter.Name));
                        }
                    }
                });

        private static bool ReportTypeErrorForFunction(IEdmTypeReference type)
        {
            // If the type is bad it will be reported elsewhere.
            if (type == null || type.IsBad())
            {
                return false;
            }

            if (type.TypeKind() == EdmTypeKind.Collection)
            {
                IEdmCollectionTypeReference collectionType = type.AsCollection();
                return !collectionType.ElementType().IsBad() && collectionType.ElementType().TypeKind() == EdmTypeKind.None;
            }

            return type.TypeKind() == EdmTypeKind.None;
        }
        #endregion

        #region IEdmFunctionImport

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportUnsupportedReturnType_V1 =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.ReturnType != null)
                   {
                       var type = functionImport.ReturnType;
                       bool reportError = !type.IsBad() && (!type.IsCollection() || (!type.AsCollection().ElementType().IsBad()) &&
                                                  !(type.AsCollection().ElementType().IsPrimitive() || type.AsCollection().ElementType().IsEntity()));

                       if (reportError)
                       {
                           context.AddError(
                               functionImport.Location(),
                               EdmErrorCode.FunctionImportUnsupportedReturnType,
                               Edm.Strings.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1(functionImport.Name));
                       }
                   }
               });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportUnsupportedReturnType_V1_1 =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.ReturnType != null)
                    {
                        var type = functionImport.ReturnType;
                        // In Version 1.1, return type can be a primitive complex or entity type or a collection of one of those.
                        bool reportError = !type.IsBad() && !(type.IsPrimitive() || type.IsComplex() || type.IsEntity());

                        reportError = reportError && (!type.IsCollection() || (!type.AsCollection().ElementType().IsBad() &&
                                                  !(type.AsCollection().ElementType().IsPrimitive() || type.AsCollection().ElementType().IsEntity() || type.AsCollection().ElementType().IsComplex())));

                        if (reportError)
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportUnsupportedReturnType,
                                Edm.Strings.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1_1(functionImport.Name));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportUnsupportedReturnType_V2 =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.ReturnType != null)
                    {
                        var type = functionImport.ReturnType;
                        bool reportError = !type.IsBad() && (!type.IsCollection() || (!type.AsCollection().ElementType().IsBad()) &&
                                                  !(type.AsCollection().ElementType().IsPrimitive() || type.AsCollection().ElementType().IsEntity() || type.AsCollection().ElementType().IsComplex()));

                        if (reportError)
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportUnsupportedReturnType,
                                Edm.Strings.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV2(functionImport.Name));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportUnsupportedReturnType_V3 =
            new ValidationRule<IEdmFunctionImport>(
                (context, functionImport) =>
                {
                    if (functionImport.ReturnType != null)
                    {
                        var type = functionImport.ReturnType.TypeKind() == EdmTypeKind.Collection ? functionImport.ReturnType.AsCollection().ElementType() : functionImport.ReturnType;
                        if (!type.IsBad() &&
                            type.TypeKind() != EdmTypeKind.Primitive && type.TypeKind() != EdmTypeKind.Enum && type.TypeKind() != EdmTypeKind.Entity && type.TypeKind() != EdmTypeKind.Complex)
                        {
                            context.AddError(
                                functionImport.Location(),
                                EdmErrorCode.FunctionImportUnsupportedReturnType,
                                Edm.Strings.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV2(functionImport.Name));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet =
           new ValidationRule<IEdmFunctionImport>(
               (context, edmFunctionImport) =>
               {
                   if (edmFunctionImport.ReturnType != null &&
                       edmFunctionImport.ReturnType.IsCollection() &&
                       edmFunctionImport.ReturnType.AsCollection().ElementType().IsEntity() &&
                       edmFunctionImport.EntitySet == null)
                   {
                       context.AddError(
                           edmFunctionImport.Location(),
                           EdmErrorCode.FunctionImportReturnsEntitiesButDoesNotSpecifyEntitySet,
                           Edm.Strings.EdmModel_Validator_Semantic_FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet(edmFunctionImport.Name)
                           );
                   }
               });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportEntityTypeDoesNotMatchEntitySet =
            new ValidationRule<IEdmFunctionImport>(
                (context, edmFunctionImport) =>
                {
                    if (edmFunctionImport.ReturnType != null &&
                        edmFunctionImport.ReturnType.IsCollection() &&
                        edmFunctionImport.ReturnType.AsCollection().ElementType().IsEntity() &&
                        edmFunctionImport.EntitySet != null)
                    {
                        if (!edmFunctionImport.EntitySet.ElementType.IsEquivalentTo(edmFunctionImport.ReturnType.AsCollection().ElementType().Definition))
                        {
                            context.AddError(
                                edmFunctionImport.Location(),
                                EdmErrorCode.FunctionImportEntityTypeDoesNotMatchEntitySet,
                                Edm.Strings.EdmModel_Validator_Semantic_FunctionImportEntityTypeDoesNotMatchEntitySet(
                                    edmFunctionImport,
                                    edmFunctionImport.EntitySet.ElementType.FullName(),
                                    edmFunctionImport.EntitySet.Name
                                ));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_FunctionImportSpecifiesEntitySetButDoesNotReturnEntityType =
            new ValidationRule<IEdmFunctionImport>(
                (context, edmFunctionImport) =>
                {
                    if (edmFunctionImport.EntitySet != null &&
                        edmFunctionImport.ReturnType != null &&
                        !(edmFunctionImport.ReturnType.TypeKind() == EdmTypeKind.Entity ||
                         edmFunctionImport.ReturnType.TypeKind() == EdmTypeKind.Collection && edmFunctionImport.ReturnType.AsCollection().ElementType().TypeKind() == EdmTypeKind.Entity))
                    {
                        context.AddError(
                            edmFunctionImport.Location(),
                            EdmErrorCode.FunctionImportSpecifiesEntitySetButDoesNotReturnEntityType,
                            Edm.Strings.EdmModel_Validator_Semantic_FunctionImportSpecifiesEntitySetButNotEntityType(
                                edmFunctionImport.Name
                            ));
                    }
                });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_ParametersIncorrectTypeBeforeV3 =
            new ValidationRule<IEdmFunctionImport>(
                (context, function) =>
                {
                    foreach (var functionParameter in function.Parameters)
                    {
                        var type = functionParameter.Type;
                        bool reportError = !(type.IsBad() || type.TypeKind() == EdmTypeKind.Primitive || type.TypeKind() == EdmTypeKind.Complex);
                        if (reportError)
                        {
                            context.AddError(
                                functionParameter.Location(),
                                EdmErrorCode.FunctionParameterIncorrectType,
                                Edm.Strings.EdmModel_Validator_Semantic_FunctionImportParameterIncorrectType(
                                    type.FullName(), functionParameter.Name));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_IsSideEffectingNotSupportedBeforeV3 =
           new ValidationRule<IEdmFunctionImport>(
               (context, edmFunctionImport) =>
               {
                   if (edmFunctionImport.SideEffecting != Csdl.CsdlConstants.Default_IsSideEffecting)
                   {
                       context.AddError(
                           edmFunctionImport.Location(),
                           EdmErrorCode.FunctionImportSideEffectingNotSupportedBeforeV3,
                           Edm.Strings.EdmModel_Validator_Semantic_FunctionImportSideEffectingNotSupportedBeforeV3);
                   }
               });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_IsComposableNotSupportedBeforeV3 =
           new ValidationRule<IEdmFunctionImport>(
               (context, edmFunctionImport) =>
               {
                   if (edmFunctionImport.Composable != Csdl.CsdlConstants.Default_IsComposable)
                   {
                       context.AddError(
                           edmFunctionImport.Location(),
                           EdmErrorCode.FunctionImportComposableNotSupportedBeforeV3,
                           Edm.Strings.EdmModel_Validator_Semantic_FunctionImportComposableNotSupportedBeforeV3);
                   }
               });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_IsBindableNotSupportedBeforeV3 =
           new ValidationRule<IEdmFunctionImport>(
               (context, edmFunctionImport) =>
               {
                   if (edmFunctionImport.Bindable != Csdl.CsdlConstants.Default_IsBindable)
                   {
                       context.AddError(
                           edmFunctionImport.Location(),
                           EdmErrorCode.FunctionImportBindableNotSupportedBeforeV3,
                           Edm.Strings.EdmModel_Validator_Semantic_FunctionImportBindableNotSupportedBeforeV3);
                   }
               });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_ComposableFunctionImportCannotBeSideEffecting =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.Composable && functionImport.SideEffecting)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.ComposableFunctionImportCannotBeSideEffecting,
                           Edm.Strings.EdmModel_Validator_Semantic_ComposableFunctionImportCannotBeSideEffecting(functionImport.Name));
                   }
               });

        internal static readonly ValidationRule<IEdmFunctionImport> IEdmFunctionImport_BindableFunctionImportMustHaveParameters =
           new ValidationRule<IEdmFunctionImport>(
               (context, functionImport) =>
               {
                   if (functionImport.Bindable && functionImport.Parameters.Count() == 0)
                   {
                       context.AddError(
                           functionImport.Location(),
                           EdmErrorCode.BindableFunctionImportMustHaveParameters,
                           Edm.Strings.EdmModel_Validator_Semantic_BindableFunctionImportMustHaveParameters(functionImport.Name));
                   }
               });

        #endregion

        #region IEdmFunctionBase

        internal static readonly ValidationRule<IEdmFunctionBase> IEdmFunctionBase_ParameterNameAlreadyDefinedDuplicate =
           new ValidationRule<IEdmFunctionBase>(
               (context, edmFunction) =>
               {
                   HashSetInternal<string> parameterList = new HashSetInternal<string>();
                   if (edmFunction.Parameters != null)
                   {
                       foreach (var parameter in edmFunction.Parameters)
                       {
                           ValidationHelper.AddMemberNameToHashSet(parameter,
                               parameterList,
                               context,
                               EdmErrorCode.AlreadyDefined,
                               Edm.Strings.EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate(parameter.Name),
                               /*supressError*/ false);
                       }
                   }
               });

        #endregion

        #region IEdmTypeReference
        internal static readonly ValidationRule<IEdmTypeReference> IEdmTypeReference_InaccessibleSchemaType =
            new ValidationRule<IEdmTypeReference>(
                (context, typeReference) =>
                {
                    IEdmSchemaType schemaType = typeReference.Definition as IEdmSchemaType;
                    if (schemaType != null)
                    {
                        if (context.FindTypeInModel(schemaType.FullName()) != schemaType)
                        {
                            context.AddError(
                                typeReference.Location(),
                                EdmErrorCode.BadUnresolvedType,
                                Edm.Strings.EdmModel_Validator_Semantic_InaccessibleType(schemaType.FullName()));
                        }
                    }
                });

        // TODO: Need validation rules for valid type reference facet values
        #endregion

        #region IEdmDecimalTypeReference

        internal static readonly ValidationRule<IEdmDecimalTypeReference> IEdmDecimalTypeReference_ScaleOutOfRange =
            new ValidationRule<IEdmDecimalTypeReference>(
                (context, type) =>
                {
                    if (type.Scale > type.Precision || type.Scale < 0)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.ScaleOutOfRange,
                            Edm.Strings.EdmModel_Validator_Semantic_ScaleOutOfRange);
                    }
                });

        #endregion

        #region IEdmStringTypeReference

        internal static readonly ValidationRule<IEdmStringTypeReference> IEdmStringTypeReference_MaxLengthNegative =
            new ValidationRule<IEdmStringTypeReference>(
                (context, type) =>
                {
                    if (type.MaxLength < 0)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.MaxLengthOutOfRange,
                            Edm.Strings.EdmModel_Validator_Semantic_StringMaxLengthOutOfRange);
                    }
                });

        #endregion

        #region IEdmBinaryTypeReference

        internal static readonly ValidationRule<IEdmBinaryTypeReference> IEdmBinaryTypeReference_MaxLengthNegative =
            new ValidationRule<IEdmBinaryTypeReference>(
                (context, type) =>
                {
                    if (type.MaxLength < 0)
                    {
                        context.AddError(
                            type.Location(),
                            EdmErrorCode.MaxLengthOutOfRange,
                            Edm.Strings.EdmModel_Validator_Semantic_MaxLengthOutOfRange);
                    }
                });

        #endregion

        #region IEdmModel

        internal static readonly ValidationRule<IEdmModel> IEdmModel_TypeNameAlreadyDefined =
           new ValidationRule<IEdmModel>(
               (context, model) =>
               {
                   HashSetInternal<string> nonFunctionNameList = new HashSetInternal<string>();
                   Dictionary<string, List<IEdmFunction>> functionDictionary = new Dictionary<string, List<IEdmFunction>>();
                   foreach (var item in model.SchemaElements)
                   {
                       IEdmFunction function = item as IEdmFunction;
                       if (function != null)
                       {
                           if (nonFunctionNameList.Contains(item.FullName()))
                           {
                               context.AddError(
                                   item.Location(),
                                   EdmErrorCode.AlreadyDefined,
                                   Edm.Strings.EdmModel_Validator_Semantic_TypeNameAlreadyDefined(item.FullName()));
                           }

                           List<IEdmFunction> functionList;
                           if (functionDictionary.TryGetValue(function.FullName(), out functionList))
                           {
                               if (functionList.Any(existingFunction => function.IsEquivalentTo(existingFunction)))
                               {
                                   context.AddError(
                                       item.Location(),
                                       EdmErrorCode.AlreadyDefined,
                                       Edm.Strings.EdmModel_Validator_Semantic_TypeNameAlreadyDefined(item.FullName()));
                               }
                           }
                           else
                           {
                               functionList = new List<IEdmFunction>();
                           }

                           functionList.Add(function);
                       }
                       else
                       {
                           if (ValidationHelper.AddMemberNameToHashSet(
                                item,
                                nonFunctionNameList,
                                context,
                                EdmErrorCode.AlreadyDefined,
                                Edm.Strings.EdmModel_Validator_Semantic_TypeNameAlreadyDefined(item.FullName()),
                               /*supressError*/ false))
                           {
                               if (functionDictionary.ContainsKey(item.FullName()))
                               {
                                   context.AddError(
                                       item.Location(),
                                       EdmErrorCode.AlreadyDefined,
                                       Edm.Strings.EdmModel_Validator_Semantic_TypeNameAlreadyDefined(item.FullName()));
                               }
                           }
                       }
                   }
               });

        internal static readonly ValidationRule<IEdmModel> IEdmEntityContainer_ConcurrencyRedefinedOnSubTypeOfEntitySetType =
            new ValidationRule<IEdmModel>(
                (context, model) =>
                {
                    Dictionary<IEdmEntityType, IEdmEntitySet> baseEntitySetTypes = new Dictionary<IEdmEntityType, IEdmEntitySet>();
                    foreach (var entityContainer in model.EntityContainers)
                    {
                        foreach (var entitySet in entityContainer.EntitySets())
                        {
                            if (!baseEntitySetTypes.ContainsKey(entitySet.ElementType))
                            {
                                baseEntitySetTypes.Add(entitySet.ElementType, entitySet);
                            }
                        }
                    }

                    // look through each type in this schema and see if it is derived from a base
                    // type. If it is see if it has some "new" Concurrency fields
                    foreach (var entityType in model.SchemaElements.OfType<IEdmEntityType>())
                    {
                        IEdmEntitySet set;
                        if (ValidationHelper.TryGetTypeSubtypeAndSubtypeSet(entityType, baseEntitySetTypes, out set) &&
                           ValidationHelper.TypeDefinesNewConcurrencyProperties(entityType))
                        {
                            context.AddError(
                                entityType.Location(),
                                EdmErrorCode.ConcurrencyRedefinedOnSubtypeOfEntitySetType,
                                Edm.Strings.EdmModel_Validator_Semantic_ConcurrencyRedefinedOnSubTypeOfEntitySetType(
                                    entityType.FullName(),
                                    set.ElementType.FullName(),
                                    set.Name));
                        }
                    }
                });

        #endregion

        #region GraphConsistency

        internal static readonly ValidationRule<IEdmAssociationSet> IEdmAssociationSet_AssociationSetEndRoleMustBelongToSetElementType =
            new ValidationRule<IEdmAssociationSet>(
                (context, associationSet) =>
                {
                    if (!associationSet.End1.Role.DeclaringAssociation.Equals(associationSet.Association))
                    {
                        context.AddError(
                            associationSet.End1.Location(),
                            EdmErrorCode.AssociationSetEndRoleMustBelongToSetElementType,
                            Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_AssociationSetEndRoleMustBelongToSetElementType(associationSet.End1.Role.Name, associationSet.Name, associationSet.Association.Name));
                    }

                    if (!associationSet.End2.Role.DeclaringAssociation.Equals(associationSet.Association))
                    {
                        context.AddError(
                            associationSet.End2.Location(),
                            EdmErrorCode.AssociationSetEndRoleMustBelongToSetElementType,
                            Microsoft.Data.Edm.Strings.EdmModel_Validator_Semantic_AssociationSetEndRoleMustBelongToSetElementType(associationSet.End2.Role.Name, associationSet.Name, associationSet.Association.Name));
                    }
                });

        internal static readonly ValidationRule<IEdmEntityType> IEdmEntityType_KeyPropertyMustBelongToEntity =
            new ValidationRule<IEdmEntityType>(
                (context, entityType) =>
                {
                    if (entityType.DeclaredKey != null)
                    {
                        foreach (var key in entityType.DeclaredKey)
                        {
                            var property = entityType.FindProperty(key.Name);
                            // If we can't find the property by name, or we find a good property but it's not our key property
                            if (property == null || (!property.IsBad() && !property.Equals(key)))
                            {
                                context.AddError(
                                entityType.Location(),
                                EdmErrorCode.KeyPropertyMustBelongToEntity,
                                Edm.Strings.EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity(key.Name, entityType.Name));
                            }
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmAssociation> IEdmAssociationType_ReferentialConstraintPrincipleEndMustBelongToAssociation =
            new ValidationRule<IEdmAssociation>(
                (context, associationType) =>
                {
                    if (associationType.ReferentialConstraint != null && ValidationHelper.ReferentialConstraintReadyForValidation(associationType.ReferentialConstraint))
                    {
                        if (!associationType.ReferentialConstraint.PrincipalEnd.Equals(associationType.End1) &&
                            !associationType.ReferentialConstraint.PrincipalEnd.Equals(associationType.End2))
                        {
                            context.AddError(
                                associationType.ReferentialConstraint.Location(),
                                EdmErrorCode.ReferentialConstraintPrincipleEndMustBelongToAssociation,
                                Edm.Strings.EdmModel_Validator_Semantic_ReferentialConstraintPrincipleEndMustBelongToAssociation(associationType.Name));
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmReferentialConstraint> IEdmReferentialConstraint_DependentPropertiesMustBelongToDependentEntity =
           new ValidationRule<IEdmReferentialConstraint>(
               (context, referentialConstraint) =>
               {
                   var dependentEntity = referentialConstraint.DependentEnd().EntityType;
                   foreach (var dependantProperty in referentialConstraint.DependentProperties)
                   {
                       var property = dependentEntity.FindProperty(dependantProperty.Name);
                       // If we can't find the property by name, or we find a good property but it's not our dependent property
                       if (property == null || (!property.IsBad() && !property.Equals(dependantProperty)))
                       {
                           context.AddError(
                            referentialConstraint.Location(),
                            EdmErrorCode.DependentPropertiesMustBelongToDependentEntity,
                            Edm.Strings.EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity(dependantProperty.Name, dependentEntity.Name));
                       }
                   }
               });

        internal static readonly ValidationRule<IEdmStructuredType> IEdmStructuredType_PropertiesDeclaringTypeMustBeCorrect =
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
                                Edm.Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(property.Name, ((IEdmSchemaType)structuredType).FullName())
                                );
                            }
                        }
                    }
                });

        internal static readonly ValidationRule<IEdmAssociation> IEdmAssociationType_EndsMustHaveCorrectDeclaringType =
            new ValidationRule<IEdmAssociation>(
                (context, associationType) =>
                {
                    if (!associationType.End1.DeclaringAssociation.Equals(associationType))
                    {
                        context.AddError(
                        associationType.End1.Location(),
                        EdmErrorCode.DeclaringTypeMustBeCorrect,
                        Edm.Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(associationType.End1.Name, associationType.FullName())
                        );
                    }

                    if (!associationType.End2.DeclaringAssociation.Equals(associationType))
                    {
                        context.AddError(
                        associationType.End2.Location(),
                        EdmErrorCode.DeclaringTypeMustBeCorrect,
                        Edm.Strings.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(associationType.End2.Name, associationType.FullName())
                        );
                    }
                });

        #endregion
    }
}
