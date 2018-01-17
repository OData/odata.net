//---------------------------------------------------------------------
// <copyright file="Parameterized.Microsoft.OData.Edm.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//
//      GENERATED FILE.  DO NOT MODIFY.
//
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm {
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
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmPrimitive_UnexpectedKind);
            }
        }

        /// <summary>
        /// A string like "Annotations in the 'Documentation' namespace must implement 'IEdmDocumentation', but '{0}' does not."
        /// </summary>
        internal static string Annotations_DocumentationPun(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Annotations_DocumentationPun, p0);
        }

        /// <summary>
        /// A string like "Annotation of type '{0}' cannot be interpreted as '{1}'."
        /// </summary>
        internal static string Annotations_TypeMismatch(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Annotations_TypeMismatch, p0, p1);
        }

        /// <summary>
        /// A string like "The annotation must have non-null target."
        /// </summary>
        internal static string Constructable_VocabularyAnnotationMustHaveTarget {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Constructable_VocabularyAnnotationMustHaveTarget);
            }
        }

        /// <summary>
        /// A string like "An entity type or a collection of an entity type is expected."
        /// </summary>
        internal static string Constructable_EntityTypeOrCollectionOfEntityTypeExpected {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Constructable_EntityTypeOrCollectionOfEntityTypeExpected);
            }
        }

        /// <summary>
        /// A string like "Navigation target entity type must be '{0}'."
        /// </summary>
        internal static string Constructable_TargetMustBeStock(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Constructable_TargetMustBeStock, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' could not be converted to be a '{1}' type."
        /// </summary>
        internal static string TypeSemantics_CouldNotConvertTypeReference(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.TypeSemantics_CouldNotConvertTypeReference, p0, p1);
        }

        /// <summary>
        /// A string like "An element with type 'None' cannot be used in a model."
        /// </summary>
        internal static string EdmModel_CannotUseElementWithTypeNone {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_CannotUseElementWithTypeNone);
            }
        }

        /// <summary>
        /// A string like "Cannot add more than one entity container to an edm model."
        /// </summary>
        internal static string EdmModel_CannotAddMoreThanOneEntityContainerToOneEdmModel {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_CannotAddMoreThanOneEntityContainerToOneEdmModel);
            }
        }

        /// <summary>
        /// A string like "An element with type 'None' cannot be used in an entity container."
        /// </summary>
        internal static string EdmEntityContainer_CannotUseElementWithTypeNone {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmEntityContainer_CannotUseElementWithTypeNone);
            }
        }

        /// <summary>
        /// A string like "The value writer cannot write a value of kind '{0}'."
        /// </summary>
        internal static string ValueWriter_NonSerializableValue(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueWriter_NonSerializableValue, p0);
        }

        /// <summary>
        /// A string like "Value has already been set."
        /// </summary>
        internal static string ValueHasAlreadyBeenSet {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueHasAlreadyBeenSet);
            }
        }

        /// <summary>
        /// A string like "Path segments must not contain '/' character."
        /// </summary>
        internal static string PathSegmentMustNotContainSlash {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.PathSegmentMustNotContainSlash);
            }
        }

        /// <summary>
        /// A string like "The number of dependent properties must match the number of key properties on the principal entity type. '{0}' principal properties were provided, but {1} dependent properties were provided."
        /// </summary>
        internal static string Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType, p0, p1);
        }

        /// <summary>
        /// A string like "Unexpected Edm type."
        /// </summary>
        internal static string EdmType_UnexpectedEdmType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmType_UnexpectedEdmType);
            }
        }

        /// <summary>
        /// A string like "The navigation property binding path is not valid."
        /// </summary>
        internal static string NavigationPropertyBinding_PathIsNotValid {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.NavigationPropertyBinding_PathIsNotValid);
            }
        }

        /// <summary>
        /// A string like "Type '{0}' must have a single type annotation with term type '{1}'."
        /// </summary>
        internal static string Edm_Evaluator_NoTermTypeAnnotationOnType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_NoTermTypeAnnotationOnType, p0, p1);
        }

        /// <summary>
        /// A string like "Type '{0}' must have a single annotation with term '{1}'."
        /// </summary>
        internal static string Edm_Evaluator_NoValueAnnotationOnType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_NoValueAnnotationOnType, p0, p1);
        }

        /// <summary>
        /// A string like "Element must have a single annotation with term '{0}'."
        /// </summary>
        internal static string Edm_Evaluator_NoValueAnnotationOnElement(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_NoValueAnnotationOnElement, p0);
        }

        /// <summary>
        /// A string like "Expression with kind '{0}' cannot be evaluated."
        /// </summary>
        internal static string Edm_Evaluator_UnrecognizedExpressionKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_UnrecognizedExpressionKind, p0);
        }

        /// <summary>
        /// A string like "Function '{0}' is not present in the execution environment."
        /// </summary>
        internal static string Edm_Evaluator_UnboundFunction(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_UnboundFunction, p0);
        }

        /// <summary>
        /// A string like "Path segment '{0}' has no binding in the execution environment."
        /// </summary>
        internal static string Edm_Evaluator_UnboundPath(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_UnboundPath, p0);
        }

        /// <summary>
        /// A string like "A containing object cannot be null when getting value of an annotation with Path in the execution environment."
        /// </summary>
        internal static string Edm_Evaluator_NoContextPath {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_NoContextPath);
            }
        }

        /// <summary>
        /// A string like "Value fails to match type '{0}'."
        /// </summary>
        internal static string Edm_Evaluator_FailedTypeAssertion(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_FailedTypeAssertion, p0);
        }

        /// <summary>
        /// A string like "An edm model must be provided for type cast."
        /// </summary>
        internal static string Edm_Evaluator_TypeCastNeedsEdmModel {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Edm_Evaluator_TypeCastNeedsEdmModel);
            }
        }

        /// <summary>
        /// A string like "The namespace '{0}' is a system namespace and cannot be used by non-system types. Please choose a different namespace."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SystemNamespaceEncountered(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_SystemNamespaceEncountered, p0);
        }

        /// <summary>
        /// A string like "The entity set or singleton '{0}' is based on type '{1}' that has no keys defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationSourceTypeHasNoKeys(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationSourceTypeHasNoKeys, p0, p1);
        }

        /// <summary>
        /// A string like "An end with the name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateEndName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateEndName, p0);
        }

        /// <summary>
        /// A string like "The key specified in entity type '{0}' is not valid. Property '{1}' is referenced more than once in the key element."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicatePropertyNameSpecifiedInEntityKey, p0, p1);
        }

        /// <summary>
        /// A string like "The complex type '{0}' is marked as abstract. Abstract complex types are only supported in version 1.1 EDM models."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidComplexTypeAbstract(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidComplexTypeAbstract, p0);
        }

        /// <summary>
        /// A string like "The complex type '{0}' has a base type specified. Complex type inheritance is only supported in version 1.1 EDM models."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidComplexTypePolymorphic, p0);
        }

        /// <summary>
        /// A string like "The key part '{0}' for type '{1}' is not valid. All parts of the key must be non nullable."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidKeyNullablePart(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidKeyNullablePart, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' in entity type '{1}' is not valid. All properties that are part of the entity key must be of primitive type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntityKeyMustBeScalar(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EntityKeyMustBeScalar, p0, p1);
        }

        /// <summary>
        /// A string like "The key usage is not valid. '{0}' cannot define keys because one of its base classes '{1}' defines keys."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidKeyKeyDefinedInBaseClass, p0, p1);
        }

        /// <summary>
        /// A string like "The entity type '{0}' has no key defined. Define the key for this entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_KeyMissingOnEntityType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_KeyMissingOnEntityType, p0);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' is not valid. The role '{1}' is not defined in relationship '{2}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BadNavigationPropertyUndefinedRole, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The navigation property '{0}'is not valid. The from role and to role are the same."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BadNavigationPropertyRolesCannotBeTheSame, p0);
        }

        /// <summary>
        /// A string like "The navigation property type could not be determined from the role '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BadNavigationPropertyCouldNotDetermineType, p0);
        }

        /// <summary>
        /// A string like "An on delete action can only be specified on one end of an association."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidOperationMultipleEndsInAssociation);
            }
        }

        /// <summary>
        /// A string like "The navigation property '{0}' cannot have 'OnDelete' specified since its multiplicity is '*'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EndWithManyMultiplicityCannotHaveOperationsSpecified, p0);
        }

        /// <summary>
        /// A string like "Each name and plural name in a relationship must be unique. '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EndNameAlreadyDefinedDuplicate, p0);
        }

        /// <summary>
        /// A string like "In relationship '{0}', the principal and dependent role of the referential constraint refers to the same role in the relationship type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_SameRoleReferredInReferentialConstraint, p0);
        }

        /// <summary>
        /// A string like "The principal navigation property '{0}' has an invalid multiplicity. Valid values for the multiplicity of a principal end are '0..1' or '1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyPrincipalEndMultiplicityUpperBoundMustBeOne, p0);
        }

        /// <summary>
        /// A string like "Because all dependent properties of the navigation '{0}' are non-nullable, the multiplicity of the principal end must be '1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNonnullable, p0);
        }

        /// <summary>
        /// A string like "Because all dependent properties of the navigation '{0}' are nullable, the multiplicity of the principal end must be '0..1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityOfPrincipalEndDependentPropertiesAllNullable, p0);
        }

        /// <summary>
        /// A string like "The multiplicity of the dependent end '{0}' is not valid. Because the dependent properties represent the dependent end key, the multiplicity of the dependent end must be '0..1' or '1'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeZeroOneOrOne, p0);
        }

        /// <summary>
        /// A string like "The multiplicity of the dependent end '{0}' is not valid. Because the dependent properties don't represent the dependent end key, the the multiplicity of the dependent end must be '*'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMultiplicityOfDependentEndMustBeMany, p0);
        }

        /// <summary>
        /// A string like "The number of properties in the dependent and principal role in a relationship constraint must be exactly identical."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_MismatchNumberOfPropertiesinRelationshipConstraint);
            }
        }

        /// <summary>
        /// A string like "The types of all properties in the dependent role of a referential constraint must be the same as the corresponding property types in the principal role. The type of property '{0}' on entity '{1}' does not match the type of property '{2}' on entity '{3}' in the referential constraint '{4}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint(object p0, object p1, object p2, object p3, object p4) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_TypeMismatchRelationshipConstraint, p0, p1, p2, p3, p4);
        }

        /// <summary>
        /// A string like "There is no property with name '{0}' defined in the type referred to by role '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintDependentEnd, p0, p1);
        }

        /// <summary>
        /// A string like "The principal end properties in the referential constraint of the association '{0}' do not match the key of the type referred to by role '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyInRelationshipConstraintPrimaryEnd, p0, p1);
        }

        /// <summary>
        /// A string like "A property cannot be of type '{0}'. The property type must be a complex, a primitive, an enum or an untyped type, or a collection of complex, primitive, or enum types."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidPropertyType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidPropertyType, p0);
        }

        /// <summary>
        /// A string like "The Bound operation '{0}' must have at least one parameter."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BoundOperationMustHaveParameters(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BoundOperationMustHaveParameters, p0);
        }

        /// <summary>
        /// A string like "Required Parameter '{0}' must not follow an optional parameter."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_RequiredParametersMustPrecedeOptional, p0);
        }

        /// <summary>
        /// A string like "The return type is not valid in operation '{0}'. The operation has an unsupported type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationWithUnsupportedReturnType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationWithUnsupportedReturnType, p0);
        }

        /// <summary>
        /// A string like "The operation import '{0}' returns entities of type '{1}' that cannot exist in the entity set '{2}' specified for the operation import."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The operation import '{0}' returns entities of type '{1}' that cannot be returned by the entity set path specified for the operation import."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet2(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationImportEntityTypeDoesNotMatchEntitySet2, p0, p1);
        }

        /// <summary>
        /// A string like "The operation import '{0}' specifies an entity set of kind '{1}' which is not supported in this context. Operation import entity set expression can be either an entity set reference or a path starting with a operation import parameter and traversing navigation properties."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationImportEntitySetExpressionKindIsInvalid(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionKindIsInvalid, p0, p1);
        }

        /// <summary>
        /// A string like "The operation import '{0}' specifies an entity set expression which is not valid. Operation import entity set expression can be either an entity set reference or a path starting with a operation import parameter and traversing navigation properties."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationImportEntitySetExpressionIsInvalid, p0);
        }

        /// <summary>
        /// A string like "The operation import '{0}' specifies an entity set but does not return entities."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationImportSpecifiesEntitySetButNotEntityType, p0);
        }

        /// <summary>
        /// A string like "The operation import '{0}' imports operation '{1}' that is bound. Only an unbound operation can be imported using an operation import."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationImportCannotImportBoundOperation, p0, p1);
        }

        /// <summary>
        /// A string like "The function import '{0}' should not be included in service document because it has parameter."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionImportWithParameterShouldNotBeIncludedInServiceDocument, p0);
        }

        /// <summary>
        /// A string like "The function '{0}' must specify a return type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_FunctionMustHaveReturnType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_FunctionMustHaveReturnType, p0);
        }

        /// <summary>
        /// A string like "Each parameter name in a operation must be unique. The parameter name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ParameterNameAlreadyDefinedDuplicate, p0);
        }

        /// <summary>
        /// A string like "Each member name in an EntityContainer must be unique. A member with name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateEntityContainerMemberName, p0);
        }

        /// <summary>
        /// A string like "The function '{0}' has a different return type than other function overloads with the same name. Functions with the same name must have the same return type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_UnboundFunctionOverloadHasIncorrectReturnType, p0);
        }

        /// <summary>
        /// A string like "The unbound operation '{0}' has an entity set path defined. Entity set path can only be defined on bound operations."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationCannotHaveEntitySetPathWithUnBoundOperation, p0);
        }

        /// <summary>
        /// A string like "The attribute '{0}' has an invalid value. The path doesn't contain the binding parameter name."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathMissingBindingParameterName, p0);
        }

        /// <summary>
        /// A string like "The attribute '{0}' is invalid. The first item of the path '{2}' is '{3}' which does not match the first parameter name {3}. The first segment of the entity set path is required to be the name of the first parameter."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathWithFirstPathParameterNotMatchingFirstParameterName(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathWithFirstPathParameterNotMatchingFirstParameterName, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The attribute '{0}' has an invalid value. The path '{1}' has a type cast segment '{2}' that is not an entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathTypeCastSegmentMustBeEntityType(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathTypeCastSegmentMustBeEntityType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The attribute '{0}' has an invalid value. The path '{1}' has a navigation property segment '{2}' that is unknown."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownNavigationProperty, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The attribute '{0}' has an invalid value. The path '{1}' has a type cast segment that doesn't derive from the entity type '{2}' for type segment '{3}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathInvalidTypeCastSegment(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathInvalidTypeCastSegment, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The attribute '{0}' has an invalid value. The path '{1}' has a binding parameter that references a type '{2}' that is not an entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathWithNonEntityBindingParameter, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The attribute '{0}' has an invalid value. The path '{1}' has a type cast segment '{2}' that cannot be found in the model."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidEntitySetPathUnknownTypeCastSegment, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The operation '{0}' has an entity set path and with an invalid return type. The return type is required to be an entity type or a collection of entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationWithEntitySetPathReturnTypeInvalid, p0);
        }

        /// <summary>
        /// A string like "The operation '{0}' entity set path determined entity type '{1}' is not assignable to the return type '{2}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationWithEntitySetPathAndReturnTypeTypeNotAssignable, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The operation '{0}' entity set path was determined to be a collection but the return type is not a collection."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToEntityTypeMismatchesCollectionEntityTypeReturnType, p0);
        }

        /// <summary>
        /// A string like "The operation '{0}' entity set path was determined to be a reference property but the return type is a collection."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OperationWithEntitySetPathResolvesToCollectionEntityTypeMismatchesEntityTypeReturnType, p0);
        }

        /// <summary>
        /// A string like "An element with the name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_SchemaElementNameAlreadyDefined, p0);
        }

        /// <summary>
        /// A string like "The member name '{0}' cannot be used in a type with the same name. Member names cannot be the same as their enclosing type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidMemberNameMatchesTypeName, p0);
        }

        /// <summary>
        /// A string like "Each property name in a type must be unique. Property name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PropertyNameAlreadyDefined(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_PropertyNameAlreadyDefined, p0);
        }

        /// <summary>
        /// A string like "The base type kind of a structured type must be the same as its derived type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BaseTypeMustHaveSameTypeKind);
            }
        }

        /// <summary>
        /// A string like "The base type of open type '{0}' is not open type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BaseTypeOfOpenTypeMustBeOpen(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BaseTypeOfOpenTypeMustBeOpen, p0);
        }

        /// <summary>
        /// A string like "The key property '{0}' must belong to the entity '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_KeyPropertyMustBelongToEntity, p0, p1);
        }

        /// <summary>
        /// A string like "The dependent property '{0}' must belong to the dependent entity '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DependentPropertiesMustBelongToDependentEntity, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}' cannot belong to a type other than its declaring type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DeclaringTypeMustBeCorrect, p0);
        }

        /// <summary>
        /// A string like "The named type '{0}' could not be found from the model being validated."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InaccessibleType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InaccessibleType, p0);
        }

        /// <summary>
        /// A string like "The named type '{0}' is ambiguous from the model being validated."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_AmbiguousType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_AmbiguousType, p0);
        }

        /// <summary>
        /// A string like "The type of the navigation property '{0}' is invalid. The navigation target type must be an entity type or a collection of entity type. The navigation target entity type must match the declaring type of the partner property."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidNavigationPropertyType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidNavigationPropertyType, p0);
        }

        /// <summary>
        /// A string like "The target multiplicity of the navigation property '{0}' is invalid. If a navigation property has 'ContainsTarget' set to true and declaring entity type of the property is the same or inherits from the target entity type, then the property represents a recursive containment and it must have an optional target represented by a collection or a nullable entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentTargetMustBeOptional(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentTargetMustBeOptional, p0);
        }

        /// <summary>
        /// A string like "The source multiplicity of the navigation property '{0}' is invalid. If a navigation property has 'ContainsTarget' set to true and declaring entity type of the property is the same or inherits from the target entity type, then the property represents a recursive containment and the multiplicity of the navigation source must be zero or one."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne, p0);
        }

        /// <summary>
        /// A string like "The source multiplicity of the navigation property '{0}' is invalid. If a navigation property has 'ContainsTarget' set to true and declaring entity type of the property is not the same as the target entity type, then the property represents a non-recursive containment and the multiplicity of the navigation source must be exactly one."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne, p0);
        }

        /// <summary>
        /// A string like "The complex type '{0}' is invalid. A complex type must contain at least one property."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ComplexTypeMustHaveProperties(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ComplexTypeMustHaveProperties, p0);
        }

        /// <summary>
        /// A string like "The dependent property '{0}' of navigation property '{1}' is a duplicate."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateDependentProperty(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateDependentProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The scale value can range from 0 through the specified precision value."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ScaleOutOfRange {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ScaleOutOfRange);
            }
        }

        /// <summary>
        /// A string like "Precision cannot be negative."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PrecisionOutOfRange {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_PrecisionOutOfRange);
            }
        }

        /// <summary>
        /// A string like "The max length facet specifies the maximum length of an instance of the string type. For unicode equal to 'true', the max length can range from 1 to 2^30, or if 'false', 1 to 2^31."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_StringMaxLengthOutOfRange {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_StringMaxLengthOutOfRange);
            }
        }

        /// <summary>
        /// A string like "Max length can range from 1 to 2^31."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_MaxLengthOutOfRange {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_MaxLengthOutOfRange);
            }
        }

        /// <summary>
        /// A string like "The value of enum member '{0}' exceeds the range of its underlying type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EnumMemberValueOutOfRange(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EnumMemberValueOutOfRange, p0);
        }

        /// <summary>
        /// A string like "Each member name of an enum type must be unique. Enum member name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EnumMemberNameAlreadyDefined(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EnumMemberNameAlreadyDefined, p0);
        }

        /// <summary>
        /// A string like "Only entity types can be open types."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_OpenTypesSupportedForEntityTypesOnly {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_OpenTypesSupportedForEntityTypesOnly);
            }
        }

        /// <summary>
        /// A string like "The string reference is invalid because if 'IsUnbounded' is true 'MaxLength' must be null."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_IsUnboundedCannotBeTrueWhileMaxLengthIsNotNull);
            }
        }

        /// <summary>
        /// A string like "The declared name and namespace of the annotation must match the name and namespace of its xml value."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidElementAnnotationMismatchedTerm);
            }
        }

        /// <summary>
        /// A string like "The value of an annotation marked to be serialized as an xml element must have a well-formed xml value."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidElementAnnotationValueInvalidXml);
            }
        }

        /// <summary>
        /// A string like "The value of an annotation marked to be serialized as an xml element must be IEdmStringValue."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationNotIEdmStringValue {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidElementAnnotationNotIEdmStringValue);
            }
        }

        /// <summary>
        /// A string like "The value of an annotation marked to be serialized as an xml element must be a string representing an xml element with non-empty name and namespace."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InvalidElementAnnotationNullNamespaceOrName {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InvalidElementAnnotationNullNamespaceOrName);
            }
        }

        /// <summary>
        /// A string like "Cannot assert the nullable type '{0}' as a non-nullable type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_CannotAssertNullableTypeAsNonNullableType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_CannotAssertNullableTypeAsNonNullableType, p0);
        }

        /// <summary>
        /// A string like "Cannot promote the primitive type '{0}' to the specified primitive type '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ExpressionPrimitiveKindCannotPromoteToAssertedType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ExpressionPrimitiveKindCannotPromoteToAssertedType, p0, p1);
        }

        /// <summary>
        /// A string like "Null value cannot have a non-nullable type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NullCannotBeAssertedToBeANonNullableType);
            }
        }

        /// <summary>
        /// A string like "The type of the expression is incompatible with the asserted type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ExpressionNotValidForTheAssertedType);
            }
        }

        /// <summary>
        /// A string like "A collection expression is incompatible with a non-collection type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_CollectionExpressionNotValidForNonCollectionType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_CollectionExpressionNotValidForNonCollectionType);
            }
        }

        /// <summary>
        /// A string like "A primitive expression is incompatible with a non-primitive type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_PrimitiveConstantExpressionNotValidForNonPrimitiveType);
            }
        }

        /// <summary>
        /// A string like "A record expression is incompatible with a non-structured type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_RecordExpressionNotValidForNonStructuredType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_RecordExpressionNotValidForNonStructuredType);
            }
        }

        /// <summary>
        /// A string like "The record expression does not have a constructor for a property named '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_RecordExpressionMissingProperty(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_RecordExpressionMissingProperty, p0);
        }

        /// <summary>
        /// A string like "The type of the record expression is not open and does not contain a property named '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_RecordExpressionHasExtraProperties(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_RecordExpressionHasExtraProperties, p0);
        }

        /// <summary>
        /// A string like "The annotated element '{0}' has multiple annotations with the term '{1}' and the qualifier '{2}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateAnnotation(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateAnnotation, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The function application provides '{0}' arguments, but the function '{1}' expects '{2}' arguments."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_IncorrectNumberOfArguments(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_IncorrectNumberOfArguments, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Each entity container name in a function must be unique. The name '{0}' is already defined."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateEntityContainerName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateEntityContainerName, p0);
        }

        /// <summary>
        /// A string like "The primitive expression is not compatible with the asserted type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ExpressionPrimitiveKindNotValidForAssertedType);
            }
        }

        /// <summary>
        /// A string like "The enum expression is not compatible with the asserted type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ExpressionEnumKindNotValidForAssertedType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ExpressionEnumKindNotValidForAssertedType);
            }
        }

        /// <summary>
        /// A string like "The value of the integer constant is out of range for the asserted type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_IntegerConstantValueOutOfRange);
            }
        }

        /// <summary>
        /// A string like "The value of the string constant is '{0}' characters long, but the max length of its type is '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_StringConstantLengthOutOfRange(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_StringConstantLengthOutOfRange, p0, p1);
        }

        /// <summary>
        /// A string like "The value of the binary constant is '{0}' characters long, but the max length of its type is '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BinaryConstantLengthOutOfRange(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BinaryConstantLengthOutOfRange, p0, p1);
        }

        /// <summary>
        /// A string like "A type without other errors must not have kind of none."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_TypeMustNotHaveKindOfNone {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_TypeMustNotHaveKindOfNone);
            }
        }

        /// <summary>
        /// A string like "A schema element without other errors must not have kind of none. The kind of schema element '{0}' is none."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SchemaElementMustNotHaveKindOfNone(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_SchemaElementMustNotHaveKindOfNone, p0);
        }

        /// <summary>
        /// A string like "A property without other errors must not have kind of none. The kind of property '{0}' is none."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PropertyMustNotHaveKindOfNone(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_PropertyMustNotHaveKindOfNone, p0);
        }

        /// <summary>
        /// A string like "A primitive type without other errors must not have kind of none. The kind of primitive type '{0}' is none."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PrimitiveTypeMustNotHaveKindOfNone(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_PrimitiveTypeMustNotHaveKindOfNone, p0);
        }

        /// <summary>
        /// A string like "An entity container element without other errors must not have kind of none. The kind of entity container element '{0}' is none."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntityContainerElementMustNotHaveKindOfNone(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EntityContainerElementMustNotHaveKindOfNone, p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' should have only a single mapping for the property '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_DuplicateNavigationPropertyMapping, p0, p1);
        }

        /// <summary>
        /// A string like "The binding of the entity set or singleton '{0}' on navigation property '{1}' is invalid, the binding of bidirectional navigation property must be bidirectional if specified."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationMappingMustBeBidirectional(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationMappingMustBeBidirectional, p0, p1);
        }

        /// <summary>
        /// A string like "The entity set '{0}' is invalid because it is contained by more than one navigation property."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntitySetCanOnlyBeContainedByASingleNavigationProperty(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EntitySetCanOnlyBeContainedByASingleNavigationProperty, p0);
        }

        /// <summary>
        /// A string like "The type annotation is missing a binding for the property '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_TypeAnnotationMissingRequiredProperty(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_TypeAnnotationMissingRequiredProperty, p0);
        }

        /// <summary>
        /// A string like "They type of the type annotation is not open, and does not contain a property named '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_TypeAnnotationHasExtraProperties(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_TypeAnnotationHasExtraProperties, p0);
        }

        /// <summary>
        /// A string like "The underlying type of '{0}' is not valid. The underlying type of an enum type must be an integral type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EnumMustHaveIntegralUnderlyingType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EnumMustHaveIntegralUnderlyingType, p0);
        }

        /// <summary>
        /// A string like "The term '{0}' could not be found from the model being validated."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InaccessibleTerm(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InaccessibleTerm, p0);
        }

        /// <summary>
        /// A string like "The target '{0}' could not be found from the model being validated."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_InaccessibleTarget(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_InaccessibleTarget, p0);
        }

        /// <summary>
        /// A string like "An element already has a direct annotation with the namespace '{0}' and name '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ElementDirectValueAnnotationFullNameMustBeUnique(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ElementDirectValueAnnotationFullNameMustBeUnique, p0, p1);
        }

        /// <summary>
        /// A string like "The association set '{0}' cannot assume an entity set for the role '{2}' because there are no entity sets for the role type '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NoEntitySetsFoundForType(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NoEntitySetsFoundForType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The association set '{0}' must specify an entity set for the role '{2}' because there are multiple entity sets for the role type '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_CannotInferEntitySetWithMultipleSetsPerType(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_CannotInferEntitySetWithMultipleSetsPerType, p0, p1, p2);
        }

        /// <summary>
        /// A string like "Because the navigation property '{0}' is recursive, the mapping from the entity set '{1}' must point back to itself."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet, p0, p1);
        }

        /// <summary>
        /// A string like "The navigation property '{0}' is invalid because it indirectly contains itself."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyEntityMustNotIndirectlyContainItself(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyEntityMustNotIndirectlyContainItself, p0);
        }

        /// <summary>
        /// A string like "The path cannot be resolved in the given context. The segment '{0}' failed to resolve."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_PathIsNotValidForTheGivenContext(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_PathIsNotValidForTheGivenContext, p0);
        }

        /// <summary>
        /// A string like "The entity set or singleton '{1}' is not a valid destination for the navigation property '{0}' because it cannot hold an element of the target entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyMappingMustPointToValidTargetForProperty(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyMappingMustPointToValidTargetForProperty, p0, p1);
        }

        /// <summary>
        /// A string like "The bound function '{0}' is a duplicate of other bound functions. For bound functions the combination of the namespace, name, binding parameter type and unordered set of parameter names uniquely identifies a bound function."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterNames, p0);
        }

        /// <summary>
        /// A string like "The bound function '{0}' is a duplicate of other bound functions. For bound functions the combination of the namespace, name, binding parameter type and ordered set of parameter types uniquely identifies a bound function."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ModelDuplicateBoundFunctionParameterTypes, p0);
        }

        /// <summary>
        /// A string like "The unbound function '{0}' is a duplicate of other unbound functions. For unbound functions the combination of the namespace, name and unordered set of parameter names uniquely identifies an unbound function."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterNames, p0);
        }

        /// <summary>
        /// A string like "The unbound function '{0}' is a duplicate of other unbound functions. For unbound functions the combination of the namespace, name and ordered set of parameter types uniquely identifies an unbound function."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ModelDuplicateUnBoundFunctionsParameterTypes, p0);
        }

        /// <summary>
        /// A string like "The bound action '{0}' is a duplicate of other bound actions. For bound actions the combination of the namespace, name, and binding parameter type uniquely identifies an bound action."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ModelDuplicateBoundActions(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ModelDuplicateBoundActions, p0);
        }

        /// <summary>
        /// A string like "The unbound action '{0}' is a duplicate of other unbound actions. For unbound actions the combination of the namespace, and name uniquely identifies an unbound action."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_ModelDuplicateUnBoundActions, p0);
        }

        /// <summary>
        /// A string like "The bound function overload '{0}' does not have the same return type as other function overloads. Expected type '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_BoundFunctionOverloadsMustHaveSameReturnType, p0, p1);
        }

        /// <summary>
        /// A string like "The type '{0}' of the entity set '{1}' is not valid, it must be collection of entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_EntitySetTypeMustBeCollectionOfEntityType, p0, p1);
        }

        /// <summary>
        /// A string like "The type '{0}' of the singleton '{1}' is not valid, it must be entity type."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_SingletonTypeMustBeEntityType, p0, p1);
        }

        /// <summary>
        /// A string like "The navigation property mapping '{0}' is invalid because its type is collection but target to a singleton '{1}'."
        /// </summary>
        internal static string EdmModel_Validator_Semantic_NavigationPropertyOfCollectionTypeMustNotTargetToSingleton(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Semantic_NavigationPropertyOfCollectionTypeMustNotTargetToSingleton, p0, p1);
        }

        /// <summary>
        /// A string like "The name is missing or not valid."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_MissingName {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_MissingName);
            }
        }

        /// <summary>
        /// A string like "The specified name must not be longer than 480 characters: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NameIsTooLong, p0);
        }

        /// <summary>
        /// A string like "The specified name is not allowed: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NameIsNotAllowed, p0);
        }

        /// <summary>
        /// A string like "The namespace name is missing or not valid."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_MissingNamespaceName {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_MissingNamespaceName);
            }
        }

        /// <summary>
        /// A string like "The specified name must not be longer than 480 characters: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsTooLong, p0);
        }

        /// <summary>
        /// A string like "The specified namespace name is not allowed: '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_EdmModel_NamespaceNameIsNotAllowed, p0);
        }

        /// <summary>
        /// A string like "The value of the property '{0}.{1}' must not be null."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_PropertyMustNotBeNull(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_PropertyMustNotBeNull, p0, p1);
        }

        /// <summary>
        /// A string like "The property '{0}.{1}' of type '{2}' has value '{3}' that is not a valid enum member."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EnumPropertyValueOutOfRange(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_EnumPropertyValueOutOfRange, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "An object with the value '{0}' of the '{1}.{2}' property must implement '{3}' interface."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_InterfaceKindValueMismatch(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_InterfaceKindValueMismatch, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "An object implementing '{0}' interface has type definition of kind '{1}'. The type reference interface must match to the kind of the  definition."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_TypeRefInterfaceTypeKindValueMismatch(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_TypeRefInterfaceTypeKindValueMismatch, p0, p1);
        }

        /// <summary>
        /// A string like "The value '{0}' of the property '{1}.{2}' is not semantically valid. A semantically valid model must not contain elements of kind '{0}'."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_InterfaceKindValueUnexpected(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_InterfaceKindValueUnexpected, p0, p1, p2);
        }

        /// <summary>
        /// A string like "The value of the enumeration the property '{0}.{1}' contains a null element. Enumeration properties must not contain null elements."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_EnumerableMustNotHaveNullElements(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_EnumerableMustNotHaveNullElements, p0, p1);
        }

        /// <summary>
        /// A string like "The partner of the navigation property '{0}' must not be the same property, and must point back to the navigation property."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_NavigationPartnerInvalid(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_NavigationPartnerInvalid, p0);
        }

        /// <summary>
        /// A string like "The chain of base types of type '{0}' is cyclic."
        /// </summary>
        internal static string EdmModel_Validator_Syntactic_InterfaceCriticalCycleInTypeHierarchy(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmModel_Validator_Syntactic_InterfaceCriticalCycleInTypeHierarchy, p0);
        }

        /// <summary>
        /// A string like "Single file provided but model cannot be serialized into single file."
        /// </summary>
        internal static string Serializer_SingleFileExpected {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_SingleFileExpected);
            }
        }

        /// <summary>
        /// A string like "Unknown Edm version."
        /// </summary>
        internal static string Serializer_UnknownEdmVersion {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_UnknownEdmVersion);
            }
        }

        /// <summary>
        /// A string like "Unknown Edmx version."
        /// </summary>
        internal static string Serializer_UnknownEdmxVersion {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_UnknownEdmxVersion);
            }
        }

        /// <summary>
        /// A string like "The operation import '{0}' could not be serialized because its return type cannot be represented inline."
        /// </summary>
        internal static string Serializer_NonInlineOperationImportReturnType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_NonInlineOperationImportReturnType, p0);
        }

        /// <summary>
        /// A string like "A referenced type can not be serialized with an invalid name. The name '{0}' is invalid."
        /// </summary>
        internal static string Serializer_ReferencedTypeMustHaveValidName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_ReferencedTypeMustHaveValidName, p0);
        }

        /// <summary>
        /// A string like "The annotation can not be serialized with an invalid target name. The name '{0}' is invalid."
        /// </summary>
        internal static string Serializer_OutOfLineAnnotationTargetMustHaveValidName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_OutOfLineAnnotationTargetMustHaveValidName, p0);
        }

        /// <summary>
        /// A string like "No CSDL is written because no schema elements could be produced. This is likely because the model is empty."
        /// </summary>
        internal static string Serializer_NoSchemasProduced {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Serializer_NoSchemasProduced);
            }
        }

        /// <summary>
        /// A string like "{0} does not contain a schema definition, or the XmlReader provided started at the end of the file."
        /// </summary>
        internal static string XmlParser_EmptyFile(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_EmptyFile, p0);
        }

        /// <summary>
        /// A string like "The source XmlReader does not contain a schema definition or started at the end of the file."
        /// </summary>
        internal static string XmlParser_EmptySchemaTextReader {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_EmptySchemaTextReader);
            }
        }

        /// <summary>
        /// A string like "Required schema attribute '{0}' is not present on element '{1}'."
        /// </summary>
        internal static string XmlParser_MissingAttribute(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_MissingAttribute, p0, p1);
        }

        /// <summary>
        /// A string like "The current schema element does not support text '{0}'."
        /// </summary>
        internal static string XmlParser_TextNotAllowed(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_TextNotAllowed, p0);
        }

        /// <summary>
        /// A string like "The attribute '{0}' was not expected in the given context."
        /// </summary>
        internal static string XmlParser_UnexpectedAttribute(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnexpectedAttribute, p0);
        }

        /// <summary>
        /// A string like "The schema element '{0}' was not expected in the given context."
        /// </summary>
        internal static string XmlParser_UnexpectedElement(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnexpectedElement, p0);
        }

        /// <summary>
        /// A string like "Unused schema element: '{0}'."
        /// </summary>
        internal static string XmlParser_UnusedElement(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnusedElement, p0);
        }

        /// <summary>
        /// A string like "Unexpected XML node type: {0}."
        /// </summary>
        internal static string XmlParser_UnexpectedNodeType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnexpectedNodeType, p0);
        }

        /// <summary>
        /// A string like "The element '{0}' was unexpected for the root element. The root element should be {1}."
        /// </summary>
        internal static string XmlParser_UnexpectedRootElement(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnexpectedRootElement, p0, p1);
        }

        /// <summary>
        /// A string like "The namespace '{0}' is invalid. The root element is expected to belong to one of the following namespaces: '{1}'."
        /// </summary>
        internal static string XmlParser_UnexpectedRootElementWrongNamespace(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnexpectedRootElementWrongNamespace, p0, p1);
        }

        /// <summary>
        /// A string like "The root element has no namespace. The root element is expected to belong to one of the following namespaces: '{0}'."
        /// </summary>
        internal static string XmlParser_UnexpectedRootElementNoNamespace(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.XmlParser_UnexpectedRootElementNoNamespace, p0);
        }

        /// <summary>
        /// A string like "The {0} '{1}' is invalid. The entitySetPath value is not allowed when IsBound attribute is false."
        /// </summary>
        internal static string CsdlParser_InvalidEntitySetPathWithUnboundAction(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidEntitySetPathWithUnboundAction, p0, p1);
        }

        /// <summary>
        /// A string like "The alias '{0}' is not a valid simple name."
        /// </summary>
        internal static string CsdlParser_InvalidAlias(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidAlias, p0);
        }

        /// <summary>
        /// A string like "The delete action '{0}' is not valid. Action must be: 'None', 'Cascade', or 'Restrict'."
        /// </summary>
        internal static string CsdlParser_InvalidDeleteAction(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidDeleteAction, p0);
        }

        /// <summary>
        /// A string like "An XML attribute or sub-element representing an EDM type is missing."
        /// </summary>
        internal static string CsdlParser_MissingTypeAttributeOrElement {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_MissingTypeAttributeOrElement);
            }
        }

        /// <summary>
        /// A string like "There is no Role with name '{0}' defined in relationship '{1}'."
        /// </summary>
        internal static string CsdlParser_InvalidEndRoleInRelationshipConstraint(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidEndRoleInRelationshipConstraint, p0, p1);
        }

        /// <summary>
        /// A string like "The multiplicity '{0}' is not valid. Multiplicity must be: '*', '0..1', or '1'."
        /// </summary>
        internal static string CsdlParser_InvalidMultiplicity(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidMultiplicity, p0);
        }

        /// <summary>
        /// A string like "Referential constraints requires one dependent role. Multiple dependent roles were specified for this referential constraint."
        /// </summary>
        internal static string CsdlParser_ReferentialConstraintRequiresOneDependent {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_ReferentialConstraintRequiresOneDependent);
            }
        }

        /// <summary>
        /// A string like "Referential constraints requires one principal role. Multiple principal roles were specified for this referential constraint."
        /// </summary>
        internal static string CsdlParser_ReferentialConstraintRequiresOnePrincipal {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_ReferentialConstraintRequiresOnePrincipal);
            }
        }

        /// <summary>
        /// A string like "If expression must contain 3 operands, the first being a boolean test, the second being being evaluated if the first is true, and the third being evaluated if the first is false."
        /// </summary>
        internal static string CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidIfExpressionIncorrectNumberOfOperands);
            }
        }

        /// <summary>
        /// A string like "The IsType expression must contain 1 operand."
        /// </summary>
        internal static string CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidIsTypeExpressionIncorrectNumberOfOperands);
            }
        }

        /// <summary>
        /// A string like "The Cast expression must contain 1 operand."
        /// </summary>
        internal static string CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidCastExpressionIncorrectNumberOfOperands);
            }
        }

        /// <summary>
        /// A string like "The LabeledElement expression must contain 1 operand."
        /// </summary>
        internal static string CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidLabeledElementExpressionIncorrectNumberOfOperands);
            }
        }

        /// <summary>
        /// A string like "The type name '{0}' is invalid. The type name must be that of a primitive type, a fully qualified name or an inline 'Collection' or 'Ref' type."
        /// </summary>
        internal static string CsdlParser_InvalidTypeName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidTypeName, p0);
        }

        /// <summary>
        /// A string like "The qualified name '{0}' is invalid. A qualified name must have a valid namespace or alias, and a valid name."
        /// </summary>
        internal static string CsdlParser_InvalidQualifiedName(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidQualifiedName, p0);
        }

        /// <summary>
        /// A string like "A model could not be produced because no XML readers were provided."
        /// </summary>
        internal static string CsdlParser_NoReadersProvided {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_NoReadersProvided);
            }
        }

        /// <summary>
        /// A string like "A model could not be produced because one of the XML readers was null."
        /// </summary>
        internal static string CsdlParser_NullXmlReader {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_NullXmlReader);
            }
        }

        /// <summary>
        /// A string like "'{0}' is not a valid entity set path."
        /// </summary>
        internal static string CsdlParser_InvalidEntitySetPath(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidEntitySetPath, p0);
        }

        /// <summary>
        /// A string like "'{0}' is not a valid enum member path."
        /// </summary>
        internal static string CsdlParser_InvalidEnumMemberPath(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_InvalidEnumMemberPath, p0);
        }

        /// <summary>
        /// A string like "The 'Nullable' attribute cannot be specified for a navigation property with collection type."
        /// </summary>
        internal static string CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_CannotSpecifyNullableAttributeForNavigationPropertyWithCollectionType);
            }
        }

        /// <summary>
        /// A string like "Metadata document cannot have more than one entity container."
        /// </summary>
        internal static string CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlParser_MetadataDocumentCannotHaveMoreThanOneEntityContainer);
            }
        }

        /// <summary>
        /// A string like " There was a mismatch in the principal and dependent ends of the referential constraint."
        /// </summary>
        internal static string CsdlSemantics_ReferentialConstraintMismatch {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlSemantics_ReferentialConstraintMismatch);
            }
        }

        /// <summary>
        /// A string like "The enumeration member must have a value."
        /// </summary>
        internal static string CsdlSemantics_EnumMemberMustHaveValue {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlSemantics_EnumMemberMustHaveValue);
            }
        }

        /// <summary>
        /// A string like "The annotation target '{0}' could not be resolved because it cannot refer to an annotatable element."
        /// </summary>
        internal static string CsdlSemantics_ImpossibleAnnotationsTarget(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlSemantics_ImpossibleAnnotationsTarget, p0);
        }

        /// <summary>
        /// A string like "The schema '{0}' contains the alias '{1}' more than once."
        /// </summary>
        internal static string CsdlSemantics_DuplicateAlias(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.CsdlSemantics_DuplicateAlias, p0, p1);
        }

        /// <summary>
        /// A string like "The EDMX version specified in the 'Version' attribute does not match the version corresponding to the namespace of the 'Edmx' element."
        /// </summary>
        internal static string EdmxParser_EdmxVersionMismatch {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmxParser_EdmxVersionMismatch);
            }
        }

        /// <summary>
        /// A string like "Unexpected {0} element while parsing Edmx. Edmx is expected to have at most one of 'Runtime' or 'DataServices' elements."
        /// </summary>
        internal static string EdmxParser_BodyElement(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmxParser_BodyElement, p0);
        }

        /// <summary>
        /// A string like "edmx:Reference must contain at least one edmx:Includes or edmx:IncludeAnnotations."
        /// </summary>
        internal static string EdmxParser_InvalidReferenceIncorrectNumberOfIncludes {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmxParser_InvalidReferenceIncorrectNumberOfIncludes);
            }
        }

        /// <summary>
        /// A string like "Unresolved Uri found in edmx:Reference, getReferencedModelReaderFunc should not return null when the URI is not a well-known schema."
        /// </summary>
        internal static string EdmxParser_UnresolvedReferenceUriInEdmxReference {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmxParser_UnresolvedReferenceUriInEdmxReference);
            }
        }

        /// <summary>
        /// A string like "Encountered the following errors when parsing the EDMX document: \r\n{0}"
        /// </summary>
        internal static string EdmParseException_ErrorsEncounteredInEdmx(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmParseException_ErrorsEncounteredInEdmx, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid boolean. The value must be 'true' or 'false'."
        /// </summary>
        internal static string ValueParser_InvalidBoolean(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidBoolean, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid integer. The value must be a valid 32 bit integer."
        /// </summary>
        internal static string ValueParser_InvalidInteger(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidInteger, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid integer. The value must be a valid 64 bit integer."
        /// </summary>
        internal static string ValueParser_InvalidLong(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidLong, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid floating point value."
        /// </summary>
        internal static string ValueParser_InvalidFloatingPoint(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidFloatingPoint, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid integer. The value must be a valid 32 bit integer or 'Max'."
        /// </summary>
        internal static string ValueParser_InvalidMaxLength(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidMaxLength, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid SRID. The value must either be a 32 bit integer or 'Variable'."
        /// </summary>
        internal static string ValueParser_InvalidSrid(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidSrid, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid scale. The value must either be a 32 bit integer or 'Variable'."
        /// </summary>
        internal static string ValueParser_InvalidScale(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidScale, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid Guid."
        /// </summary>
        internal static string ValueParser_InvalidGuid(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidGuid, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid decimal."
        /// </summary>
        internal static string ValueParser_InvalidDecimal(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidDecimal, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid date time offset value."
        /// </summary>
        internal static string ValueParser_InvalidDateTimeOffset(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidDateTimeOffset, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid date time value."
        /// </summary>
        internal static string ValueParser_InvalidDateTime(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidDateTime, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid date value."
        /// </summary>
        internal static string ValueParser_InvalidDate(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidDate, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid duration value."
        /// </summary>
        internal static string ValueParser_InvalidDuration(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidDuration, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid binary value. The value must be a hexadecimal string and must not be prefixed by '0x'."
        /// </summary>
        internal static string ValueParser_InvalidBinary(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidBinary, p0);
        }

        /// <summary>
        /// A string like "The value '{0}' is not a valid TimeOfDay value."
        /// </summary>
        internal static string ValueParser_InvalidTimeOfDay(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.ValueParser_InvalidTimeOfDay, p0);
        }

        /// <summary>
        /// A string like "Invalid multiplicity: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_Multiplicity(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_Multiplicity, p0);
        }

        /// <summary>
        /// A string like "Invalid schema element kind: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_SchemaElementKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_SchemaElementKind, p0);
        }

        /// <summary>
        /// A string like "Invalid type kind: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_TypeKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_TypeKind, p0);
        }

        /// <summary>
        /// A string like "Invalid primitive kind: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_PrimitiveKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_PrimitiveKind, p0);
        }

        /// <summary>
        /// A string like "Invalid container element kind: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_ContainerElementKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_ContainerElementKind, p0);
        }

        /// <summary>
        /// A string like "Invalid CSDL target: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_CsdlTarget(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_CsdlTarget, p0);
        }

        /// <summary>
        /// A string like "Invalid property kind: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_PropertyKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_PropertyKind, p0);
        }

        /// <summary>
        /// A string like "Invalid expression kind: '{0}'"
        /// </summary>
        internal static string UnknownEnumVal_ExpressionKind(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.UnknownEnumVal_ExpressionKind, p0);
        }

        /// <summary>
        /// A string like "The name '{0}' is ambiguous."
        /// </summary>
        internal static string Bad_AmbiguousElementBinding(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_AmbiguousElementBinding, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedType, p0);
        }

        /// <summary>
        /// A string like "The complex type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedComplexType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedComplexType, p0);
        }

        /// <summary>
        /// A string like "The entity type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEntityType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedEntityType, p0);
        }

        /// <summary>
        /// A string like "The primitive type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedPrimitiveType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedPrimitiveType, p0);
        }

        /// <summary>
        /// A string like "The operation '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedOperation(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedOperation, p0);
        }

        /// <summary>
        /// A string like "The operation '{0}' could not be resolved because more than one operation could be used for this application."
        /// </summary>
        internal static string Bad_AmbiguousOperation(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_AmbiguousOperation, p0);
        }

        /// <summary>
        /// A string like "The operation '{0}' could not be resolved because none of the operations with that name take the correct set of parameters."
        /// </summary>
        internal static string Bad_OperationParametersDontMatch(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_OperationParametersDontMatch, p0);
        }

        /// <summary>
        /// A string like "The entity set '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEntitySet(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedEntitySet, p0);
        }

        /// <summary>
        /// A string like "The entity container '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEntityContainer(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedEntityContainer, p0);
        }

        /// <summary>
        /// A string like "The enum type '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEnumType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedEnumType, p0);
        }

        /// <summary>
        /// A string like "The enum member '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedEnumMember(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedEnumMember, p0);
        }

        /// <summary>
        /// A string like "The property '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedProperty(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedProperty, p0);
        }

        /// <summary>
        /// A string like "The parameter '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedParameter(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedParameter, p0);
        }

        /// <summary>
        /// A string like "The labeled element '{0}' could not be found."
        /// </summary>
        internal static string Bad_UnresolvedLabeledElement(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedLabeledElement, p0);
        }

        /// <summary>
        /// A string like "The entity '{0}' is invalid because its base type is cyclic."
        /// </summary>
        internal static string Bad_CyclicEntity(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_CyclicEntity, p0);
        }

        /// <summary>
        /// A string like "The complex type '{0}' is invalid because its base type is cyclic."
        /// </summary>
        internal static string Bad_CyclicComplex(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_CyclicComplex, p0);
        }

        /// <summary>
        /// A string like "The entity container '{0}' is invalid because its extends hierarchy is cyclic."
        /// </summary>
        internal static string Bad_CyclicEntityContainer(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_CyclicEntityContainer, p0);
        }

        /// <summary>
        /// A string like "A navigation property could not be found for the path '{0}' starting from the type '{1}'."
        /// </summary>
        internal static string Bad_UnresolvedNavigationPropertyPath(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Bad_UnresolvedNavigationPropertyPath, p0, p1);
        }

        /// <summary>
        /// A string like "The same rule cannot be in the same rule set twice."
        /// </summary>
        internal static string RuleSet_DuplicateRulesExistInRuleSet {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.RuleSet_DuplicateRulesExistInRuleSet);
            }
        }

        /// <summary>
        /// A string like "Conversion of EDM values to a CLR type with type {0} is not supported."
        /// </summary>
        internal static string EdmToClr_UnsupportedType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_UnsupportedType, p0);
        }

        /// <summary>
        /// A string like "Conversion of an EDM structured value is supported only to a CLR class."
        /// </summary>
        internal static string EdmToClr_StructuredValueMappedToNonClass {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_StructuredValueMappedToNonClass);
            }
        }

        /// <summary>
        /// A string like "Cannot initialize a property '{0}' on an object of type '{1}'. The property already has a value."
        /// </summary>
        internal static string EdmToClr_IEnumerableOfTPropertyAlreadyHasValue(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_IEnumerableOfTPropertyAlreadyHasValue, p0, p1);
        }

        /// <summary>
        /// A string like "An EDM structured value contains multiple values for the property '{0}'. Conversion of an EDM structured value with duplicate property values is not supported."
        /// </summary>
        internal static string EdmToClr_StructuredPropertyDuplicateValue(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_StructuredPropertyDuplicateValue, p0);
        }

        /// <summary>
        /// A string like "Conversion of an EDM value of the type '{0}' to the CLR type '{1}' is not supported."
        /// </summary>
        internal static string EdmToClr_CannotConvertEdmValueToClrType(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_CannotConvertEdmValueToClrType, p0, p1);
        }

        /// <summary>
        /// A string like "Conversion of an edm collection value to the CLR type '{0}' is not supported. EDM collection values can be converted to System.Collections.Generic.IEnumerable&lt;T&gt;, System.Collections.Generic.IList&lt;T&gt; or System.Collections.Generic.ICollection&lt;T&gt;."
        /// </summary>
        internal static string EdmToClr_CannotConvertEdmCollectionValueToClrType(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_CannotConvertEdmCollectionValueToClrType, p0);
        }

        /// <summary>
        /// A string like "The type '{0}' of the object returned by the TryCreateObjectInstance delegate is not assignable to the expected type '{1}'."
        /// </summary>
        internal static string EdmToClr_TryCreateObjectInstanceReturnedWrongObject(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmToClr_TryCreateObjectInstanceReturnedWrongObject, p0, p1);
        }

        /// <summary>
        /// A string like "The MIME type annotation must not have a null value."
        /// </summary>
        internal static string EdmUtil_NullValueForMimeTypeAnnotation {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmUtil_NullValueForMimeTypeAnnotation);
            }
        }

        /// <summary>
        /// A string like "An annotation of type string was expected for the '{{http://docs.oasis-open.org/odata/ns/metadata}}:{0}' annotation, but an annotation of type '{1}' was found."
        /// </summary>
        internal static string EdmUtil_InvalidAnnotationValue(object p0, object p1) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.EdmUtil_InvalidAnnotationValue, p0, p1);
        }

        /// <summary>
        /// A string like "The time zone information is missing on the DateTimeOffset value '{0}'. A DateTimeOffset value must contain the time zone information."
        /// </summary>
        internal static string PlatformHelper_DateTimeOffsetMustContainTimeZone(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.PlatformHelper_DateTimeOffsetMustContainTimeZone, p0);
        }

        /// <summary>
        /// A string like "The added or subtracted value results in an un-representable Date."
        /// </summary>
        internal static string Date_InvalidAddedOrSubtractedResults {
            get {
                return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Date_InvalidAddedOrSubtractedResults);
            }
        }

        /// <summary>
        /// A string like "The Year '{0}', Month '{1}' and Day '{2}' parameters describe an un-representable Date."
        /// </summary>
        internal static string Date_InvalidDateParameters(object p0, object p1, object p2) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Date_InvalidDateParameters, p0, p1, p2);
        }

        /// <summary>
        /// A string like "String '{0}' was not recognized as a valid Date."
        /// </summary>
        internal static string Date_InvalidParsingString(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Date_InvalidParsingString, p0);
        }

        /// <summary>
        /// A string like "Target object '{0}' is not an instance with type of Date."
        /// </summary>
        internal static string Date_InvalidCompareToTarget(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.Date_InvalidCompareToTarget, p0);
        }

        /// <summary>
        /// A string like "The Hour '{0}', Minute '{1}', Second '{2}' and Millisecond '{3}' parameters describe an un-representable TimeOfDay."
        /// </summary>
        internal static string TimeOfDay_InvalidTimeOfDayParameters(object p0, object p1, object p2, object p3) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.TimeOfDay_InvalidTimeOfDayParameters, p0, p1, p2, p3);
        }

        /// <summary>
        /// A string like "The ticks value '{0}' is out of representable TimeOfDay range."
        /// </summary>
        internal static string TimeOfDay_TicksOutOfRange(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.TimeOfDay_TicksOutOfRange, p0);
        }

        /// <summary>
        /// A string like "The TimeSpan value '{0}' is out of representable TimeOfDay range."
        /// </summary>
        internal static string TimeOfDay_ConvertErrorFromTimeSpan(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.TimeOfDay_ConvertErrorFromTimeSpan, p0);
        }

        /// <summary>
        /// A string like "String '{0}' was not recognized as a valid TimeOfDay."
        /// </summary>
        internal static string TimeOfDay_InvalidParsingString(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.TimeOfDay_InvalidParsingString, p0);
        }

        /// <summary>
        /// A string like "Target object '{0}' is not an instance with type of TimeOfDay."
        /// </summary>
        internal static string TimeOfDay_InvalidCompareToTarget(object p0) {
            return Microsoft.OData.Edm.EntityRes.GetString(Microsoft.OData.Edm.EntityRes.TimeOfDay_InvalidCompareToTarget, p0);
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
        /// The exception that is thrown when the author has not yet implemented the logic at this point in the program. This can act as an exception based TODO tag.
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
