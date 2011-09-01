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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.Data.Edm.Library;
using Microsoft.Data.Edm.Validation.Internal;

namespace Microsoft.Data.Edm.Validation
{
    /// <summary>
    /// A set of rules to run during validation.
    /// </summary>
    public sealed class ValidationRuleSet
    {
        private readonly Dictionary<Type, List<ValidationRule>> rules;

        /// <summary>
        /// Initializes a new instance of the ValidationRuleSet class.
        /// </summary>
        /// <param name="rules">Rules to be contained in this ruleset.</param>
        public ValidationRuleSet(IEnumerable<ValidationRule> rules)
        {
            EdmUtil.CheckArgumentNull(rules, "rules");
            this.rules = new Dictionary<Type, List<ValidationRule>>();
            foreach(ValidationRule rule in rules)
            {
                this.AddRule(rule);
            }
        }

        /// <summary>
        /// Initializes a new instance of the ValidationRuleSet class.
        /// </summary>
        /// <param name="baseSet">Ruleset whose rules should be contained in this set.</param>
        /// <param name="newRules">Additional rules to add to the set.</param>
        public ValidationRuleSet(ValidationRuleSet baseSet, IEnumerable<ValidationRule> newRules)
        {
            EdmUtil.CheckArgumentNull(baseSet, "baseSet");
            EdmUtil.CheckArgumentNull(newRules, "newRules");
            this.rules = new Dictionary<Type, List<ValidationRule>>();
            foreach (List<ValidationRule> rulelist in baseSet.rules.Values)
            {
                foreach (ValidationRule rule in rulelist)
                {
                    this.AddRule(rule);
                }
            }

            foreach (ValidationRule rule in newRules)
            {
                this.AddRule(rule);
            }
        }

        private void AddRule(ValidationRule rule)
        {
            List<ValidationRule> typeRules;
            if (!this.rules.TryGetValue(rule.ValidatedType, out typeRules))
            {
                typeRules = new List<ValidationRule>();
                this.rules[rule.ValidatedType] = typeRules;
            }

            Debug.Assert(!typeRules.Contains(rule), "Ruleset should not already contain the rule being added: " + rule.ToString());
            typeRules.Add(rule);
        }

        internal IEnumerable<ValidationRule> GetRules(Type t)
        {
            List<ValidationRule> foundRules;
            return this.rules.TryGetValue(t, out foundRules) ? foundRules : Enumerable.Empty<ValidationRule>();
        }

        private static readonly ValidationRuleSet GraphConsistencyRules = new ValidationRuleSet(
                new ValidationRule[]{
                    ValidationRules.IEdmAssociationSet_AssociationSetEndRoleMustBelongToSetElementType,
                    ValidationRules.IEdmEntityType_KeyPropertyMustBelongToEntity,
                    ValidationRules.IEdmAssociationType_ReferentialConstraintPrincipleEndMustBelongToAssociation,
                    ValidationRules.IEdmReferentialConstraint_DependentPropertiesMustBelongToDependentEntity,
                    ValidationRules.IEdmStructuredType_PropertiesDeclaringTypeMustBeCorrect,
                    ValidationRules.IEdmAssociationType_EndsMustHaveCorrectDeclaringType
                });

        private static readonly ValidationRuleSet BaseRuleSet = new ValidationRuleSet(GraphConsistencyRules, new ValidationRule[]{
                ValidationRules.IEdmNamedElement_NameMustNotBeEmptyOrWhiteSpace,
                ValidationRules.IEdmNamedElement_NameIsTooLong,
                ValidationRules.IEdmNamedElement_NameIsNotAllowed,
                ValidationRules.IEdmSchemaElement_NamespaceIsNotAllowed,
                ValidationRules.IEdmSchemaElement_NamespaceIsTooLong,
                ValidationRules.IEdmSchemaElement_NamespaceMustNotBeEmptyOrWhiteSpace,
                ValidationRules.IEdmSchemaElement_SystemNamespaceEncountered,
                ValidationRules.IEdmEntityContainer_SimilarRelationshipEnd,
                ValidationRules.IEdmEntityContainer_ConcurrencyRedefinedOnSubTypeOfEntitySetType,
                ValidationRules.IEdmEntityContainer_DuplicateEntityContainerMemberName,
                //ValidationRules.IEdmEntitySet_EntitySetTypeHasNoKeys,
                ValidationRules.IEdmAssociationSet_DuplicateEndName,
                ValidationRules.IEdmEntityType_DuplicatePropertyNameSpecifiedInEntityKey,
                ValidationRules.IEdmEntityType_InvalidKeyNullablePart,
                ValidationRules.IEdmEntityType_EntityKeyMustBeScalar,
                ValidationRules.IEdmEntityType_InvalidKeyKeyDefinedInBaseClass,
                ValidationRules.IEdmEntityType_KeyMissingOnEntityType,
                ValidationRules.IEdmStructuredType_InvalidMemberNameMatchesTypeName,
                ValidationRules.IEdmStructuredType_PropertyNameAlreadyDefined,
                ValidationRules.IEdmAssociationType_InvalidOperationMultipleEndsInAssociation,
                ValidationRules.IEdmAssociationEnd_EndWithManyMultiplicityCannotHaveOperationsSpecified,
                ValidationRules.IEdmAssociationType_EndNameAlreadyDefinedDuplicate,
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNullable,
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityFromRoleUpperBoundMustBeOne,
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityToRoleUpperBoundMustBeMany,
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityToRoleUpperBoundMustBeOne,
                ValidationRules.IEdmReferentialConstraint_TypeMismatchRelationshipConstraint,
                ValidationRules.IEdmStructuralProperty_InvalidPropertyType,
                ValidationRules.IEdmStructuralProperty_NullableComplexType,
                ValidationRules.IEdmFunctionBase_ParameterNameAlreadyDefinedDuplicate,
                ValidationRules.IEdmFunctionImport_FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet,
                ValidationRules.IEdmFunctionImport_FunctionImportEntityTypeDoesNotMatchEntitySet,
                ValidationRules.IEdmFunctionImport_FunctionImportSpecifiesEntitySetButDoesNotReturnEntityType,
                ValidationRules.IEdmModel_TypeNameAlreadyDefined,
                ValidationRules.IEdmStructuredType_BaseTypeMustBeSameKindAsDerivedKind,
                ValidationRules.IEdmRowType_BaseTypeMustBeNull,
                ValidationRules.IEdmNavigationProperty_InaccessibleAssociation,
                ValidationRules.IEdmAssociationSet_InaccessibleAssociation,
                ValidationRules.IEdmEntitySet_InaccessibleEntityType,
                ValidationRules.IEdmStructuredType_InaccessibleBaseType,
                ValidationRules.IEdmEntityReferenceType_InaccessibleEntityType,
                ValidationRules.IEdmAssociationEnd_InaccessibleEntityType,
                ValidationRules.IEdmTypeReference_InaccessibleSchemaType,
                ValidationRules.IEdmEntitySet_SimilarRelationshipEnd,
                ValidationRules.IEdmEntitySet_EntitySetTypeHasNoKeys,
                ValidationRules.IEdmAssociationSetEnd_EntitySetMustBeOfCorrectEntityType,
                ValidationRules.IEdmFunction_OnlyInputParametersAllowedInFunctions,
                ValidationRules.IEdmFunction_ParametersIncorrectType,
                ValidationRules.IEdmFunction_FunctionReturnTypeIncorrectType,
                ValidationRules.IEdmRowType_MustContainProperties,
                ValidationRules.IEdmReferentialConstraint_DuplicateDependentProperty,
                ValidationRules.IEdmNavigationProperty_CorrectType,
                ValidationRules.IEdmDecimalTypeReference_ScaleOutOfRange,
                ValidationRules.IEdmBinaryTypeReference_MaxLengthNegative,
                ValidationRules.IEdmStringTypeReference_MaxLengthNegative,
                ValidationRules.IEdmStructuralProperty_InvalidPropertyTypeConcurrencyMode,
            });
        
        private static readonly ValidationRuleSet V1RuleSet = new ValidationRuleSet(BaseRuleSet, new ValidationRule[]{
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNonNullableV1,
                ValidationRules.IEdmReferentialConstraint_InvalidToPropertyInRelationshipConstraintBeforeV2,
                ValidationRules.IEdmComplexType_InvalidIsAbstractV1,
                ValidationRules.IEdmComplexType_InvalidIsPolymorphicV1,
                ValidationRules.IEdmFunction_FunctionsNotSupportedBeforeV2,
                ValidationRules.IEdmFunctionImport_FunctionImportUnsupportedReturnType_V1,
                ValidationRules.IEdmFunctionImport_ParametersIncorrectTypeBeforeV3,
                ValidationRules.IEdmFunctionImport_IsSideEffectingNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsComposableNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsBindableNotSupportedBeforeV3,
                ValidationRules.IEdmEntityType_EntityKeyMustNotBeBinaryBeforeV2,
            });

        private static readonly ValidationRuleSet V1_1RuleSet = new ValidationRuleSet(BaseRuleSet, new ValidationRule[]{
                ValidationRules.IEdmFunction_FunctionsNotSupportedBeforeV2,
                ValidationRules.IEdmReferentialConstraint_InvalidToPropertyInRelationshipConstraintBeforeV2,
                ValidationRules.IEdmFunctionImport_FunctionImportUnsupportedReturnType_V1_1,
                ValidationRules.IEdmFunctionImport_IsSideEffectingNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsComposableNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsBindableNotSupportedBeforeV3,
                ValidationRules.IEdmEntityType_EntityKeyMustNotBeBinaryBeforeV2,
                ValidationRules.IEdmFunctionImport_ParametersIncorrectTypeBeforeV3,
            });

        private static readonly ValidationRuleSet V1_2RuleSet = new ValidationRuleSet(BaseRuleSet, new ValidationRule[]{
                ValidationRules.IEdmFunction_FunctionsNotSupportedBeforeV2,
                ValidationRules.IEdmReferentialConstraint_InvalidToPropertyInRelationshipConstraintBeforeV2,
                ValidationRules.IEdmFunctionImport_FunctionImportUnsupportedReturnType_V1_1,
                ValidationRules.IEdmFunctionImport_ParametersIncorrectTypeBeforeV3,
                ValidationRules.IEdmFunctionImport_IsSideEffectingNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsComposableNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsBindableNotSupportedBeforeV3,
                ValidationRules.IEdmEntityType_EntityKeyMustNotBeBinaryBeforeV2,
            });

        private static readonly ValidationRuleSet V2RuleSet = new ValidationRuleSet(BaseRuleSet, new ValidationRule[]{
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNonNullableV2,
                ValidationRules.IEdmFunctionImport_ParametersIncorrectTypeBeforeV3,
                ValidationRules.IEdmFunctionImport_FunctionImportUnsupportedReturnType_V2,
                ValidationRules.IEdmFunctionImport_IsSideEffectingNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsComposableNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsBindableNotSupportedBeforeV3,
            });

        private static readonly ValidationRuleSet V2_2RuleSet = new ValidationRuleSet(BaseRuleSet, new ValidationRule[]{
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNonNullableV2,
                ValidationRules.IEdmFunctionImport_FunctionImportUnsupportedReturnType_V2,
                ValidationRules.IEdmFunctionImport_ParametersIncorrectTypeBeforeV3,
                ValidationRules.IEdmFunctionImport_IsSideEffectingNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsComposableNotSupportedBeforeV3,
                ValidationRules.IEdmFunctionImport_IsBindableNotSupportedBeforeV3,
            });

        private static readonly ValidationRuleSet V3RuleSet = new ValidationRuleSet(BaseRuleSet, new ValidationRule[]{
                ValidationRules.IEdmReferentialConstraint_InvalidMultiplicityFromRoleToPropertyNonNullableV2,
                ValidationRules.IEdmFunctionImport_FunctionImportUnsupportedReturnType_V3,
                ValidationRules.IEdmFunctionImport_BindableFunctionImportMustHaveParameters,
                ValidationRules.IEdmFunctionImport_ComposableFunctionImportCannotBeSideEffecting,
            });

        internal static ValidationRuleSet GetEdmModelRuleSet(Version version)
        {
            if (version == EdmConstants.EdmVersion1)
            {
                return V1RuleSet;
            }

            if (version == EdmConstants.EdmVersion1_1)
            {
                return V1_1RuleSet;
            }

            if (version == EdmConstants.EdmVersion1_2)
            {
                return V1_2RuleSet;
            }

            if (version == EdmConstants.EdmVersion2)
            {
                return V2RuleSet;
            }

            if (version == EdmConstants.EdmVersion2_2)
            {
                return V2_2RuleSet;
            }

            if (version == EdmConstants.EdmVersion3)
            {
                return V3RuleSet;
            }

            throw new InvalidOperationException(Edm.Strings.Serializer_UnknownEdmVersion);
        }
    }
}
