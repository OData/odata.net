//---------------------------------------------------------------------
// <copyright file="ValidationRuleSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm.Validation
{
    /// <summary>
    /// A set of rules to run during validation.
    /// </summary>
    public sealed class ValidationRuleSet : IEnumerable<ValidationRule>
    {
        private readonly Dictionary<Type, List<ValidationRule>> rules;

        private static readonly ValidationRuleSet BaseRuleSet =
            new ValidationRuleSet(new ValidationRule[]
            {
                ValidationRules.EntityTypeKeyPropertyMustBelongToEntity,
                ValidationRules.StructuredTypePropertiesDeclaringTypeMustBeCorrect,
                ValidationRules.NamedElementNameMustNotBeEmptyOrWhiteSpace,
                ValidationRules.NamedElementNameIsTooLong,
                ValidationRules.NamedElementNameIsNotAllowed,
                ValidationRules.SchemaElementNamespaceIsNotAllowed,
                ValidationRules.SchemaElementNamespaceIsTooLong,
                ValidationRules.SchemaElementNamespaceMustNotBeEmptyOrWhiteSpace,
                ValidationRules.SchemaElementSystemNamespaceEncountered,
                ValidationRules.EntityContainerDuplicateEntityContainerMemberName,
                ValidationRules.EntityTypeDuplicatePropertyNameSpecifiedInEntityKey,
                ValidationRules.EntityTypeInvalidKeyNullablePart,
                ValidationRules.EntityTypeEntityKeyMustBeScalar,
                ValidationRules.EntityTypeInvalidKeyKeyDefinedInBaseClass,
                ValidationRules.EntityTypeKeyMissingOnEntityType,
                ValidationRules.StructuredTypeInvalidMemberNameMatchesTypeName,
                ValidationRules.StructuredTypePropertyNameAlreadyDefined,
                ValidationRules.StructuralPropertyInvalidPropertyType,
                ValidationRules.OperationParameterNameAlreadyDefinedDuplicate,
                ValidationRules.OperationImportEntityTypeDoesNotMatchEntitySet,
                ValidationRules.OperationImportCannotImportBoundOperation,
                ValidationRules.StructuredTypeBaseTypeMustBeSameKindAsDerivedKind,
                ValidationRules.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional,
                ValidationRules.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne,
                ValidationRules.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne,
                ValidationRules.EntitySetTypeMustBeCollectionOfEntityType,
                ValidationRules.NavigationSourceInaccessibleEntityType,
                ValidationRules.StructuredTypeInaccessibleBaseType,
                ValidationRules.EntityReferenceTypeInaccessibleEntityType,
                ValidationRules.TypeReferenceInaccessibleSchemaType,
                ValidationRules.NavigationSourceTypeHasNoKeys,
                ValidationRules.DecimalTypeReferenceScaleOutOfRange,
                ValidationRules.BinaryTypeReferenceBinaryMaxLengthNegative,
                ValidationRules.StringTypeReferenceStringMaxLengthNegative,
                ValidationRules.EnumMemberValueMustHaveSameTypeAsUnderlyingType,
                ValidationRules.EnumTypeEnumMemberNameAlreadyDefined,
                ValidationRules.BoundOperationMustHaveParameters,
                ValidationRules.OptionalParametersMustComeAfterRequiredParameters,
                ValidationRules.OperationEntitySetPathMustBeValid,
                ValidationRules.OperationReturnTypeEntityTypeMustBeValid,
                ValidationRules.OperationImportEntitySetExpressionIsInvalid,
                ValidationRules.FunctionImportWithParameterShouldNotBeIncludedInServiceDocument,
                ValidationRules.BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength,
                ValidationRules.StringTypeReferenceStringUnboundedNotValidForMaxLength,
                ValidationRules.ImmediateValueAnnotationElementAnnotationIsValid,
                ValidationRules.VocabularyAnnotationAssertCorrectExpressionType,
                ValidationRules.IfExpressionAssertCorrectTestType,
                ValidationRules.CollectionExpressionAllElementsCorrectType,
                ValidationRules.RecordExpressionPropertiesMatchType,
                ValidationRules.NavigationPropertyDependentPropertiesMustBelongToDependentEntity,
                ValidationRules.NavigationPropertyInvalidOperationMultipleEndsInAssociatedNavigationProperties,
                ValidationRules.NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified,
                ValidationRules.NavigationPropertyPartnerPathShouldBeResolvable,
                ValidationRules.NavigationPropertyTypeMismatchRelationshipConstraint,
                ValidationRules.NavigationPropertyDuplicateDependentProperty,
                ValidationRules.NavigationPropertyPrincipalEndMultiplicity,
                ValidationRules.NavigationPropertyDependentEndMultiplicity,
                ValidationRules.NavigationPropertyCorrectType,
                ValidationRules.NavigationPropertyBindingPathMustBeResolvable,
                ValidationRules.ImmediateValueAnnotationElementAnnotationHasNameAndNamespace,
                ValidationRules.OpenComplexTypeCannotHaveClosedDerivedComplexType,
                ValidationRules.FunctionApplicationExpressionParametersMatchAppliedFunction,
                ValidationRules.VocabularyAnnotatableNoDuplicateAnnotations,
                ValidationRules.TemporalTypeReferencePrecisionOutOfRange,
                ValidationRules.DecimalTypeReferencePrecisionOutOfRange,
                ValidationRules.ModelDuplicateEntityContainerName,
                ValidationRules.ModelBoundFunctionOverloadsMustHaveSameReturnType,
                ValidationRules.UnBoundFunctionOverloadsMustHaveIdenticalReturnTypes,
                ValidationRules.TypeMustNotHaveKindOfNone,
                ValidationRules.PrimitiveTypeMustNotHaveKindOfNone,
                ValidationRules.PropertyMustNotHaveKindOfNone,
                ValidationRules.SchemaElementMustNotHaveKindOfNone,
                ValidationRules.EntityContainerElementMustNotHaveKindOfNone,
                ValidationRules.PrimitiveValueValidForType,
                ValidationRules.EntitySetCanOnlyBeContainedByASingleNavigationProperty,
                ValidationRules.NavigationMappingMustBeBidirectional,
                ValidationRules.SingletonTypeMustBeEntityType,
                ValidationRules.NavigationPropertyMappingsMustBeUnique,
                ValidationRules.PropertyValueBindingValueIsCorrectType,
                ValidationRules.EnumMustHaveIntegerUnderlyingType,
                ValidationRules.AnnotationInaccessibleTerm,
                ValidationRules.ElementDirectValueAnnotationFullNameMustBeUnique,
                ValidationRules.VocabularyAnnotationInaccessibleTarget,
                ValidationRules.NavigationPropertyEntityMustNotIndirectlyContainItself,
                ValidationRules.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet,
                ValidationRules.NavigationPropertyMappingMustPointToValidTargetForProperty,
                ValidationRules.DirectValueAnnotationHasXmlSerializableName,
                ValidationRules.FunctionMustHaveReturnType,
            });

        private static readonly ValidationRuleSet V4RuleSet =
            new ValidationRuleSet(
                BaseRuleSet,
                new ValidationRule[]
                {
                    ValidationRules.OperationUnsupportedReturnType,
                    ValidationRules.ModelDuplicateSchemaElementName,
                });

        /// <summary>
        /// Initializes a new instance of the ValidationRuleSet class.
        /// </summary>
        /// <param name="baseSet">Ruleset whose rules should be contained in this set.</param>
        /// <param name="newRules">Additional rules to add to the set.</param>
        public ValidationRuleSet(IEnumerable<ValidationRule> baseSet, IEnumerable<ValidationRule> newRules)
            : this(EdmUtil.CheckArgumentNull(baseSet, "baseSet").Concat(EdmUtil.CheckArgumentNull(newRules, "newRules")))
        {
        }

        /// <summary>
        /// Initializes a new instance of the ValidationRuleSet class.
        /// </summary>
        /// <param name="rules">Rules to be contained in this ruleset.</param>
        public ValidationRuleSet(IEnumerable<ValidationRule> rules)
        {
            EdmUtil.CheckArgumentNull(rules, "rules");
            this.rules = new Dictionary<Type, List<ValidationRule>>();
            foreach (ValidationRule rule in rules)
            {
                this.AddRule(rule);
            }
        }

        /// <summary>
        /// Gets the default validation ruleset for the given version.
        /// </summary>
        /// <param name="version">The EDM version being validated.</param>
        /// <returns>The set of rules to validate that the model conforms to the given version.</returns>
        public static ValidationRuleSet GetEdmModelRuleSet(Version version)
        {
            if (version == EdmConstants.EdmVersion4)
            {
                return V4RuleSet;
            }
            else
            {
                throw new InvalidOperationException(Edm.Strings.Serializer_UnknownEdmVersion);
            }
        }

        /// <summary>
        /// Gets all of the rules in this ruleset.
        /// </summary>
        /// <returns>All of the rules in this ruleset.</returns>
        public IEnumerator<ValidationRule> GetEnumerator()
        {
            foreach (List<ValidationRule> ruleList in this.rules.Values)
            {
                foreach (ValidationRule rule in ruleList)
                {
                    yield return rule;
                }
            }
        }

        /// <summary>
        /// Gets all of the rules in this ruleset.
        /// </summary>
        /// <returns>All of the rules in this ruleset.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal IEnumerable<ValidationRule> GetRules(Type t)
        {
            List<ValidationRule> foundRules;
            return this.rules.TryGetValue(t, out foundRules) ? foundRules : Enumerable.Empty<ValidationRule>();
        }

        private void AddRule(ValidationRule rule)
        {
            List<ValidationRule> typeRules;
            if (!this.rules.TryGetValue(rule.ValidatedType, out typeRules))
            {
                typeRules = new List<ValidationRule>();
                this.rules[rule.ValidatedType] = typeRules;
            }

            if (typeRules.Contains(rule))
            {
                throw new InvalidOperationException(Edm.Strings.RuleSet_DuplicateRulesExistInRuleSet);
            }

            typeRules.Add(rule);
        }
    }
}
