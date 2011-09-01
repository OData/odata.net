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

namespace Microsoft.Data.Edm {
    using System;
    using System.Resources;

    /// <summary>
    ///    Strongly-typed and parameterized string resources.
    /// </summary>
    internal static class Strings {
        /// <summary>
        /// A string like "Unexpected primitive type kind."
        /// </summary>
        internal static string EdmPrimitive_UnexpectedKind {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// A string like "Dependent navigation property must have a principal partner."
        /// </summary>
        internal static string EdmNavigation_RequiresPartner {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmNavigation_RequiresPartner);
            }
        }

        /// <summary>
        /// A string like "Collection value must have a collection type."
        /// </summary>
        internal static string EdmCollectionValueType {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmCollectionValueType);
            }
        }

        /// <summary>
        /// A string like "Annotation '{0}:{1}' cannot be changed or removed."
        /// </summary>
        internal static string Annotations_ImmutableChange(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Annotations_ImmutableChange,p0,p1);
        }

        /// <summary>
        /// A string like "Annotations in the Documentation namespace must implement 'IEdmDocumentation', but '{0}' does not."
        /// </summary>
        internal static string Annotations_DocumentationPun(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Annotations_DocumentationPun,p0);
        }

        /// <summary>
        /// A string like "Annotation of type '{0}' cannot be interpreted as '{1}'."
        /// </summary>
        internal static string Annotations_TypeMismatch(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Annotations_TypeMismatch,p0,p1);
        }

        /// <summary>
        /// A string like "The entity type '{0}' is invalid because it was removed from this model after being assigned as the entity type of this entity set."
        /// </summary>
        internal static string Constructable_EntitySetTypeInvalidFromEntityTypeRemoval(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Constructable_EntitySetTypeInvalidFromEntityTypeRemoval,p0);
        }

        /// <summary>
        /// A string like "The type '{0}' could not be converted to be a '{1}' type."
        /// </summary>
        internal static string TypeSemantics_CouldNotConvertTypeReference(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.TypeSemantics_CouldNotConvertTypeReference,p0,p1);
        }

        /// <summary>
        /// A string like "The property cannot be added to the type. DeclaringType of the property does not match the type to which the property is being added."
        /// </summary>
        internal static string AddProperty_DeclaringTypeMismatch {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.AddProperty_DeclaringTypeMismatch);
            }
        }

        /// <summary>
        /// A string like "Row types cannot have base types."
        /// </summary>
        internal static string SetBaseType_RowCantHaveBaseType {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.SetBaseType_RowCantHaveBaseType);
            }
        }

        /// <summary>
        /// A string like "The namespace '{0}' is a system namespace and cannot be used by non-system types. Please choose a different namespace."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SystemNamespaceEncountered(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_SystemNamespaceEncountered,p0);
        }

        /// <summary>
        /// A string like "The role '{0}' in '{1}' and '{2}' association set refers to the same entity set '{3}' in entity container '{4}'. Make sure that if two or more association sets refer to the same association type, the ends must not refer to the same entity set."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SimilarRelationshipEnd(object p0, object p1, object p2, object p3, object p4) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_SimilarRelationshipEnd,p0,p1,p2,p3,p4);
        }

        /// <summary>
        /// A string like "Type '{0}' is derived from the type '{1}' that is the type for EntitySet '{2}'. Type '{0}' defines new concurrency requirements that are not allowed for sub types of base EntitySet types."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ConcurrencyRedefinedOnSubTypeOfEntitySetType(object p0, object p1, object p2) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_ConcurrencyRedefinedOnSubTypeOfEntitySetType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The EntitySet {0} is based on type {1} that has no keys defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_EntitySetTypeHasNoKeys,p0,p1);
        }

        /// <summary>
        /// A string like "An end with the name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateEndName(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateEndName,p0);
        }

        /// <summary>
        /// A string like "Key specified in EntityType '{0}' is not valid. Property '{1}' is referenced more than once in the key element."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey,p0,p1);
        }

        /// <summary>
        /// A string like "ComplexType '{0}' is marked as abstract. Abstract complex types are only supported in version 1.1 EDM models."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidComplexTypeAbstract(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidComplexTypeAbstract,p0);
        }

        /// <summary>
        /// A string like "Complex type '{0}' has a base type specified. Complex type inheritance is not supported in version 1.0 EDM models."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic,p0);
        }

        /// <summary>
        /// A string like "Key Part: '{0}' for type {1} is not valid. All parts of the key must be non nullable."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidKeyNullablePart(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidKeyNullablePart,p0,p1);
        }

        /// <summary>
        /// A string like "The property '{0}' in entity type '{1}' is not valid. All properties that are part of the entity key must be of primitive type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntityKeyMustBeScalar(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_EntityKeyMustBeScalar,p0,p1);
        }

        /// <summary>
        /// A string like "Key usage is not valid. {0} cannot define keys because one of its base classes '{1}' defines keys."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass,p0,p1);
        }

        /// <summary>
        /// A string like "EntityType '{0}' has no key defined. Define the key for this entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_KeyMissingOnEntityType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_KeyMissingOnEntityType,p0);
        }

        /// <summary>
        /// A string like "Navigation property is not valid. The role {0} is not defined in relationship {1}."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole,p0,p1);
        }

        /// <summary>
        /// A string like "Navigation property is not valid. The from role and to role are the same."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame);
            }
        }

        /// <summary>
        /// A string like "Navigation property type could not be determined from the role '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType,p0);
        }

        /// <summary>
        /// A string like "OnDelete can be specified on only one end of an association."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation);
            }
        }

        /// <summary>
        /// A string like "End '{0}' on relationship '{1}' cannot have operation specified since its multiplicity is '*'. Operations cannot be specified on ends with multiplicity '*'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified,p0,p1);
        }

        /// <summary>
        /// A string like "Each name and plural name in a relationship must be unique. '{0}' was already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate,p0);
        }

        /// <summary>
        /// A string like "In relationship '{0}', the principal and dependent role of the referential constraint refers to the same role in the relationship type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint,p0);
        }

        /// <summary>
        /// A string like "Multiplicity is not valid in role '{0}' in relationship '{1}'. Valid values for multiplicity for principal role are '0..1' or '1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleUpperBoundMustBeOne(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleUpperBoundMustBeOne,p0,p1);
        }

        /// <summary>
        /// A string like "Multiplicity is not valid in role '{0}' in relationship '{1}'. Because all the properties in the dependent role are nullable, multiplicity of the principal role must be '0..1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNullableV1(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNullableV1,p0,p1);
        }

        /// <summary>
        /// A string like "Multiplicity conflicts with the referential constraint in role '{0}' in relationship '{1}'. Because one/all of the properties in the dependent role is non-nullable, multiplicity of the principal role must be '1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV1(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV1,p0,p1);
        }

        /// <summary>
        /// A string like "Multiplicity conflicts with the referential constraint in role '{0}' in relationship '{1}'. Because all of the properties in the dependent role are non-nullable, multiplicity of the principal role must be '1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV2(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityFromRoleToPropertyNonNullableV2,p0,p1);
        }

        /// <summary>
        /// A string like "Properties referred by the dependent role {0} must be a subset of the key of the entity type {1} referred to by the dependent role in the referential constraint for relationship {2}."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint(object p0, object p1, object p2) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidToPropertyInRelationshipConstraint,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Multiplicity is not valid in role '{0}' in relationship '{1}'. Because the dependent role refers to the key properties, the upper bound of the multiplicity of the Dependent Role must be 1."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeOne(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeOne,p0,p1);
        }

        /// <summary>
        /// A string like "Multiplicity is not valid in role '{0}' in relationship '{1}'. Because the dependent role properties are not the key properties, the upper bound of the multiplicity of the Dependent Role must be *."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeMany(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityToRoleUpperBoundMustBeMany,p0,p1);
        }

        /// <summary>
        /// A string like "Number of properties in the dependent and principal role in a relationship constraint must be exactly identical."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint);
            }
        }

        /// <summary>
        /// A string like "The types of all properties in the dependent role of a referential constraint must be the same as the corresponding property types in the Principal Role. The type of property '{0}' on entity '{1}' does not match the type of property '{2}' on entity '{3}' in the referential constraint '{4}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(object p0, object p1, object p2, object p3, object p4) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint,p0,p1,p2,p3,p4);
        }

        /// <summary>
        /// A string like "There is no property with name '{0}' defined in the type referred to by role '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd,p0,p1);
        }

        /// <summary>
        /// A string like "There is no property with name '{0}' referenced in the key of the type referred to by role '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd,p0,p1);
        }

        /// <summary>
        /// A string like "Nullable complex type is not supported. Property '{0}' must not allow nulls."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NullableComplexType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_NullableComplexType,p0);
        }

        /// <summary>
        /// A string like "A property cannot be of type {0}. The property type must be a complex, a primitive or an enum type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyType,p0);
        }

        /// <summary>
        /// A string like "Function import '{0}' cannot be composable and side-effecting at the same time."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ComposableFunctionImportCannotBeSideEffecting(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_ComposableFunctionImportCannotBeSideEffecting,p0);
        }

        /// <summary>
        /// A string like "The bindable function import '{0}' must have at least one parameter."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BindableFunctionImportMustHaveParameters(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_BindableFunctionImportMustHaveParameters,p0);
        }

        /// <summary>
        /// A string like "Return type is not valid in function import '{0}'. The function import must return a collection of scalar values or a collection of entities."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1,p0);
        }

        /// <summary>
        /// A string like "Return type is not valid in function import '{0}'. The function import must return scalar, entity, or complex type, or a collection of scalar, entity or complex types"
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1_1(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV1_1,p0);
        }

        /// <summary>
        /// A string like "Return type is not valid in function import '{0}'. The function import can have no return type or return a collection of scalar values, a collection of complex types or a collection of entities."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV2(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportWithUnsupportedReturnTypeV2,p0);
        }

        /// <summary>
        /// A string like "Function import '{0}' returns entities but does not specify an entity set."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet,p0);
        }

        /// <summary>
        /// A string like "Function import '{0}' returns entities of type '{1}' that cannot exist in the declared entity set '{2}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportEntityTypeDoesNotMatchEntitySet(object p0, object p1, object p2) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportEntityTypeDoesNotMatchEntitySet,p0,p1,p2);
        }

        /// <summary>
        /// A string like "Function import '{0}' specifies an entity set but does not return entities."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportSpecifiesEntitySetButNotEntityType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportSpecifiesEntitySetButNotEntityType,p0);
        }

        /// <summary>
        /// A string like "Each parameter name in a function must be unique. The parameter name '{0}' was already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate,p0);
        }

        /// <summary>
        /// A string like "Each member name in an EntityContainer must be unique. A member with name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName,p0);
        }

        /// <summary>
        /// A string like "Each type name in a schema must be unique. Type name '{0}' was already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_TypeNameAlreadyDefined(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_TypeNameAlreadyDefined,p0);
        }

        /// <summary>
        /// A string like "Name {0} cannot be used in type {1}. Member names cannot be the same as their enclosing type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName,p0,p1);
        }

        /// <summary>
        /// A string like "Each property name in a type must be unique. Property name '{0}' was already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PropertyNameAlreadyDefined(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_PropertyNameAlreadyDefined,p0);
        }

        /// <summary>
        /// A string like "The base type kind of a structured type must be the same as its derived type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind);
            }
        }

        /// <summary>
        /// A string like "Row types cannot have a base type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_RowTypeMustNotHaveBaseType {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_RowTypeMustNotHaveBaseType);
            }
        }

        /// <summary>
        /// A string like "Functions are not supported prior to version 2.0."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionsNotSupportedBeforeV2 {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionsNotSupportedBeforeV2);
            }
        }

        /// <summary>
        /// A string like "SideEffecting setting of function imports is not supported before version 3.0."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportSideEffectingNotSupportedBeforeV3 {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportSideEffectingNotSupportedBeforeV3);
            }
        }

        /// <summary>
        /// A string like "Composable setting of function imports is not supported before version 3.0."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportComposableNotSupportedBeforeV3 {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportComposableNotSupportedBeforeV3);
            }
        }

        /// <summary>
        /// A string like "Bindable setting of function imports is not supported before version 3.0."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportBindableNotSupportedBeforeV3 {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportBindableNotSupportedBeforeV3);
            }
        }

        /// <summary>
        /// A string like "The entity type '{0}' cannot be used in the entity set '{1}' because it is abstract."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_AbstractEntitiesCannotBeInstantiated(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_AbstractEntitiesCannotBeInstantiated,p0,p1);
        }

        /// <summary>
        /// A string like "The association set end role '{0}' in the association set '{1}' must be one of the ends in the association type '{2}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_AssociationSetEndRoleMustBelongToSetElementType(object p0, object p1, object p2) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_AssociationSetEndRoleMustBelongToSetElementType,p0,p1,p2);
        }

        /// <summary>
        /// A string like "The key property '{0}' must belong to the entity '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity,p0,p1);
        }

        /// <summary>
        /// A string like "The principle end of the referential constraint belonging to '{0}' must be one of the ends of '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ReferentialConstraintPrincipleEndMustBelongToAssociation(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_ReferentialConstraintPrincipleEndMustBelongToAssociation,p0);
        }

        /// <summary>
        /// A string like "The dependent property '{0}' must belong to the dependent entity '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity,p0,p1);
        }

        /// <summary>
        /// A string like "The declaring type of the property '{0}' must be the type '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect,p0,p1);
        }

        /// <summary>
        /// A string like "The named association '{0}' could not be found from the model being validated."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InaccessibleAssociation(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InaccessibleAssociation,p0);
        }

        /// <summary>
        /// A string like "The named type '{0}' could not be found from the model being validated."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InaccessibleType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InaccessibleType,p0);
        }

        /// <summary>
        /// A string like "The type of the navigation property '{0}' is invalid. Only entity type or a collection of entity type is valid for a navigation property."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidNavigationPropertyType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidNavigationPropertyType,p0);
        }

        /// <summary>
        /// A string like "The type '{0}' of entity set '{1}' is invalid for the end '{2}'. The entity set of this end should have a type that can accommodate an instance of type '{3}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidAssociationSetEndSetWrongType(object p0, object p1, object p2, object p3) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidAssociationSetEndSetWrongType,p0,p1,p2,p3);
        }

        /// <summary>
        /// A string like "The mode of the parameter '{0}' in the function '{1}' is invalid. Only input parameters are allowed in functions."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OnlyInputParametersAllowedInFunctions(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_OnlyInputParametersAllowedInFunctions,p0,p1);
        }

        /// <summary>
        /// A string like "The type '{0}' of parameter '{1}' is invalid. A function parameter must be one of the following types: A simple type or collection of simple types. An entity type or collection of entity types. A complex type or collection of complex types. A row type or collection of row types. An entity reference type or collection of entity reference types."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionParameterIncorrectType(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionParameterIncorrectType,p0,p1);
        }

        /// <summary>
        /// A string like "The type '{0}' of parameter '{1}' is invalid. A function import parameter must be one of the following types: A simple type or complex type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportParameterIncorrectType(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportParameterIncorrectType,p0,p1);
        }

        /// <summary>
        /// A string like "The return type '{0}' of function '{1}' is invalid. A function return type must be one of the following types: A simple type or collection of simple types. An entity type or collection of entity types. A complex type or collection of complex types. A row type or collection of row types. An entity reference type or collection of entity reference types."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionReturnTypeIncorrectType(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionReturnTypeIncorrectType,p0,p1);
        }

        /// <summary>
        /// A string like "A row property cannot be of type {0}. The property type must be a primitive, entity, entity reference, row or collection type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidRowPropertyType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidRowPropertyType,p0);
        }

        /// <summary>
        /// A string like "Row type is invalid. A row must contain at least one property."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_RowTypeMustHaveProperties {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_RowTypeMustHaveProperties);
            }
        }

        /// <summary>
        /// A string like "The dependent property '{1}' in the referential constraint of the association '{0}' is a duplicate."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateDependentProperty(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateDependentProperty,p0,p1);
        }

        /// <summary>
        /// A string like "The scale value can range from 0 through the specified precision value."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ScaleOutOfRange {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_ScaleOutOfRange);
            }
        }

        /// <summary>
        /// A string like "The max length facet specifies the maximum length of an instance of the string type. For unicode equal to true, the max length can range from 1 to 2^30, or if false, 1 to 2^31."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_StringMaxLengthOutOfRange {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_StringMaxLengthOutOfRange);
            }
        }

        /// <summary>
        /// A string like "Max length can range from 1 to 2^31."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_MaxLengthOutOfRange {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_MaxLengthOutOfRange);
            }
        }

        /// <summary>
        /// A string like "A property with a fixed concurrency mode cannot be of type {0}. The property type must be a primitive type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyTypeConcurrencyMode(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyTypeConcurrencyMode,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' in EntityType '{1}' is not valid. Binary types are not allowed in entity keys before version 2.0."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntityKeyMustNotBeBinaryBeforeV2(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_EntityKeyMustNotBeBinaryBeforeV2,p0,p1);
        }

        /// <summary>
        /// A string like "Path expression has '{0}' as a type context, which is not a valid entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PathExpressionHasNoEntityContext(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Semantic_PathExpressionHasNoEntityContext,p0);
        }

        /// <summary>
        /// A string like "The name is missing or not valid."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_MissingName {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Syntactic_MissingName);
            }
        }

        /// <summary>
        /// A string like "The specified name must not be longer than 480 characters: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong,p0);
        }

        /// <summary>
        /// A string like "The specified name is not allowed: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed,p0);
        }

        /// <summary>
        /// A string like "The namespace name is missing or not valid."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_MissingNamespaceName {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Syntactic_MissingNamespaceName);
            }
        }

        /// <summary>
        /// A string like "The specified name must not be longer than 480 characters: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong,p0);
        }

        /// <summary>
        /// A string like "The specified namespace name is not allowed: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Single file provided but model cannot be serialized into single file."
        /// </summary>
        internal static string Serializer_SingleFileExpected {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Serializer_SingleFileExpected);
            }
        }

        /// <summary>
        /// A string like "Unknown Edm version."
        /// </summary>
        internal static string Serializer_UnknownEdmVersion {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Serializer_UnknownEdmVersion);
            }
        }

        /// <summary>
        /// A string like "Unknown Edmx version."
        /// </summary>
        internal static string Serializer_UnknownEdmxVersion {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Serializer_UnknownEdmxVersion);
            }
        }

        /// <summary>
        /// A string like "{0} does not contain a schema definition, or the XmlReader provided started at the end of the file."
        /// </summary>
        internal static string XmlParser_EmptyFile(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_EmptyFile,p0);
        }

        /// <summary>
        /// A string like "The source XmlReader does not contain a schema definition or started at the end of the file."
        /// </summary>
        internal static string XmlParser_EmptySchemaTextReader {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_EmptySchemaTextReader);
            }
        }

        /// <summary>
        /// A string like "{0} '{1}' is not valid."
        /// </summary>
        internal static string XmlParser_InvalidName(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_InvalidName,p0,p1);
        }

        /// <summary>
        /// A string like "Required schema attribute '{0}' is not present on element '{1}'."
        /// </summary>
        internal static string XmlParser_MissingAttribute(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_MissingAttribute,p0,p1);
        }

        /// <summary>
        /// A string like "The current schema element does not support text '{0}'."
        /// </summary>
        internal static string XmlParser_TextNotAllowed(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_TextNotAllowed,p0);
        }

        /// <summary>
        /// A string like "Unrecognized schema attribute: {0}."
        /// </summary>
        internal static string XmlParser_UnexpectedAttribute(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_UnexpectedAttribute,p0);
        }

        /// <summary>
        /// A string like "Unrecognized schema element: {0}."
        /// </summary>
        internal static string XmlParser_UnexpectedElement(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_UnexpectedElement,p0);
        }

        /// <summary>
        /// A string like "Unused schema element: {0}."
        /// </summary>
        internal static string XmlParser_UnusedElement(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_UnusedElement,p0);
        }

        /// <summary>
        /// A string like "Unexpected XmlNode type: {0}."
        /// </summary>
        internal static string XmlParser_UnexpectedNodeType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_UnexpectedNodeType,p0);
        }

        /// <summary>
        /// A string like "The element {0} was unexpected for the root element. The root element should be {1}."
        /// </summary>
        internal static string XmlParser_UnexpectedRootElement(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_UnexpectedRootElement,p0,p1);
        }

        /// <summary>
        /// A string like "The root element has no namespace. The root element is expected to belong to one of the following namespaces: {0}."
        /// </summary>
        internal static string XmlParser_UnexpectedRootElementNoNamespace(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.XmlParser_UnexpectedRootElementNoNamespace,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid boolean. The value must be True or False."
        /// </summary>
        internal static string CsdlParser_InvalidBoolean(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidBoolean,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid integer. The value must be a valid 32 bit integer."
        /// </summary>
        internal static string CsdlParser_InvalidInteger(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidInteger,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid integer. The value must be a valid 64 bit integer."
        /// </summary>
        internal static string CsdlParser_InvalidLong(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidLong,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid integer. The value must be a valid 32 bit integer or 'Max'."
        /// </summary>
        internal static string CsdlParser_InvalidMaxLength(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidMaxLength,p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid SRID. The value must either be a 32 bit integer or 'Variable'."
        /// </summary>
        internal static string CsdlParser_InvalidSrid(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidSrid,p0);
        }

        /// <summary>
        /// A string like "Association may have at most one constraint. Multiple constraints were specified for this association."
        /// </summary>
        internal static string CsdlParser_AssociationHasAtMostOneConstraint {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_AssociationHasAtMostOneConstraint);
            }
        }

        /// <summary>
        /// A string like "Delete action '{0}' is not valid. Action must be: 'None', 'Cascade', or 'Restrict'."
        /// </summary>
        internal static string CsdlParser_InvalidDeleteAction(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidDeleteAction,p0);
        }

        /// <summary>
        /// A string like "The association '{0}' is not valid. Associations must contain exactly two End elements."
        /// </summary>
        internal static string CsdlParser_InvalidAssociationIncorrectNumberOfEnds(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidAssociationIncorrectNumberOfEnds,p0);
        }

        /// <summary>
        /// A string like "The association set '{0}' is not valid. Association sets must contain exactly two End elements."
        /// </summary>
        internal static string CsdlParser_InvalidAssociationSetIncorrectNumberOfEnds(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidAssociationSetIncorrectNumberOfEnds,p0);
        }

        /// <summary>
        /// A string like "Concurrency mode '{0}' is not valid. Concurrency mode must be: 'None', or 'Fixed'."
        /// </summary>
        internal static string CsdlParser_InvalidConcurrencyMode(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidConcurrencyMode,p0);
        }

        /// <summary>
        /// A string like "Parameter mode '{0}' is not valid. Parameter mode must be: 'In', 'Out', or 'InOut'."
        /// </summary>
        internal static string CsdlParser_InvalidParameterMode(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidParameterMode,p0);
        }

        /// <summary>
        /// A string like "There is no Role with name '{0}' defined in relationship '{1}'."
        /// </summary>
        internal static string CsdlParser_InvalidEndRoleInRelationshipConstraint(object p0, object p1) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidEndRoleInRelationshipConstraint,p0,p1);
        }

        /// <summary>
        /// A string like "Multiplicity '{0}' is not valid. Multiplicity must be: '*', '0..1', or '1'."
        /// </summary>
        internal static string CsdlParser_InvalidMultiplicity(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_InvalidMultiplicity,p0);
        }

        /// <summary>
        /// A string like "Referential constraint requires one dependent role. Multiple dependent roles were specified for this referential constraint."
        /// </summary>
        internal static string CsdlParser_ReferentialConstraintRequiresOneDependent {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_ReferentialConstraintRequiresOneDependent);
            }
        }

        /// <summary>
        /// A string like "Referential constraint requires one principal role. Multiple principal roles were specified for this referential constraint."
        /// </summary>
        internal static string CsdlParser_ReferentialConstraintRequiresOnePrincipal {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlParser_ReferentialConstraintRequiresOnePrincipal);
            }
        }

        /// <summary>
        /// A string like " There was a mismatch in the principle and dependant ends of the referential constraint."
        /// </summary>
        internal static string CsdlSemantics_ReferentialConstraintMismatch {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlSemantics_ReferentialConstraintMismatch);
            }
        }

        /// <summary>
        /// A string like "The enumeration member value exceeds the range of its data type 'http://www.w3.org/2001/XMLSchema:long'."
        /// </summary>
        internal static string CsdlSemantics_EnumMemberValueOutOfRange {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.CsdlSemantics_EnumMemberValueOutOfRange);
            }
        }

        /// <summary>
        /// A string like "The Edmx version specified in the Version attribute does not match the version corresponding to the namespace of the Edmx element."
        /// </summary>
        internal static string EdmxParser_EdmxVersionMismatch {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmxParser_EdmxVersionMismatch);
            }
        }

        /// <summary>
        /// A string like "Unexpected {0} element while parsing Edmx. Edmx is expected to have at most one of Runtime or DataServices elements."
        /// </summary>
        internal static string EdmxParser_BodyElement(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.EdmxParser_BodyElement,p0);
        }

        /// <summary>
        /// A string like "Invalid multiplicity: {0}"
        /// </summary>
        internal static string UnknownEnumVal_Multiplicity(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_Multiplicity,p0);
        }

        /// <summary>
        /// A string like "Invalid schema element kind: {0}"
        /// </summary>
        internal static string UnknownEnumVal_SchemaElementKind(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_SchemaElementKind,p0);
        }

        /// <summary>
        /// A string like "Invalid type kind: {0}"
        /// </summary>
        internal static string UnknownEnumVal_TypeKind(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_TypeKind,p0);
        }

        /// <summary>
        /// A string like "Invalid primitive kind: {0}"
        /// </summary>
        internal static string UnknownEnumVal_PrimitiveKind(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_PrimitiveKind,p0);
        }

        /// <summary>
        /// A string like " Invalid container element kind: {0}"
        /// </summary>
        internal static string UnknownEnumVal_ContainerElementKind(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_ContainerElementKind,p0);
        }

        /// <summary>
        /// A string like " Invalid edmx target: {0}"
        /// </summary>
        internal static string UnknownEnumVal_EdmxTarget(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_EdmxTarget,p0);
        }

        /// <summary>
        /// A string like " Invalid function parameter mode: {0}"
        /// </summary>
        internal static string UnknownEnumVal_FunctionParameterMode(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_FunctionParameterMode,p0);
        }

        /// <summary>
        /// A string like " Invalid concurrency mode: {0}"
        /// </summary>
        internal static string UnknownEnumVal_ConcurrencyMode(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_ConcurrencyMode,p0);
        }

        /// <summary>
        /// A string like " Invalid property kind: {0}"
        /// </summary>
        internal static string UnknownEnumVal_PropertyKind(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.UnknownEnumVal_PropertyKind,p0);
        }

        /// <summary>
        /// A string like "The name '{0}' is ambiguous."
        /// </summary>
        internal static string Bad_AmbiguousElementBinding(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_AmbiguousElementBinding,p0);
        }

        /// <summary>
        /// A string like "This item is bad because was reached through a bad type."
        /// </summary>
        internal static string Bad_BadByAssociation {
            get {
                return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_BadByAssociation);
            }
        }

        /// <summary>
        /// A string like "The type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedType,p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEntityType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedEntityType,p0);
        }

        /// <summary>
        /// A string like "The primitive type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedPrimitiveType(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedPrimitiveType,p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEntitySet(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedEntitySet,p0);
        }

        /// <summary>
        /// A string like "The entity container '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEntityContainer(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedEntityContainer,p0);
        }

        /// <summary>
        /// A string like "The property '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedProperty(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedProperty,p0);
        }

        /// <summary>
        /// A string like "The value term '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedValueTerm(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UnresolvedValueTerm,p0);
        }

        /// <summary>
        /// A string like "The entity '{0}' is invalid because its base type is cyclic."
        /// </summary>
        internal static string Bad_CyclicEntity(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_CyclicEntity,p0);
        }

        /// <summary>
        /// A string like "The complex type '{0}' is invalid because its base type is cyclic."
        /// </summary>
        internal static string Bad_CyclicComplex(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_CyclicComplex,p0);
        }

        /// <summary>
        /// A string like "The entity container '{0}' is invalid because its extends hierarchy is cyclic."
        /// </summary>
        internal static string Bad_CyclicEntityContainer(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_CyclicEntityContainer,p0);
        }

        /// <summary>
        /// A string like "The association end '{0}' could not be computed."
        /// </summary>
        internal static string Bad_UncomputableAssociationEnd(object p0) {
            return Microsoft.Data.Edm.EntityRes.GetString(Microsoft.Data.Edm.EntityRes.Bad_UncomputableAssociationEnd,p0);
        }

    }

    /// <summary>
    ///    Strongly-typed and parameterized exception factory.
    /// </summary>
    internal static partial class Error {

        /// <summary>
        /// The exception that is thrown when a null reference (Nothing in Visual Basic) is passed to a method that does not accept it as a valid argument.
        /// </summary>
        internal static Exception ArgumentNull(string paramName) {
            return new ArgumentNullException(paramName);
        }
        
        /// <summary>
        /// The exception that is thrown when the value of an argument is outside the allowable range of values as defined by the invoked method.
        /// </summary>
        internal static Exception ArgumentOutOfRange(string paramName) {
            return new ArgumentOutOfRangeException(paramName);
        }

        /// <summary>
        /// The exception that is thrown when the author has yet to implement the logic at this point in the program. This can act as an exception based TODO tag.
        /// </summary>
        internal static Exception NotImplemented() {
            return new NotImplementedException();
        }

        /// <summary>
        /// The exception that is thrown when an invoked method is not supported, or when there is an attempt to read, seek, or write to a stream that does not support the invoked functionality. 
        /// </summary>
        internal static Exception NotSupported() {
            return new NotSupportedException();
        }        
    }
}
