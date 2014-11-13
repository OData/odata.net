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
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Validation
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
                ValidationRules.ComplexTypeInvalidAbstractComplexType,
                ValidationRules.ComplexTypeInvalidPolymorphicComplexType,
                ValidationRules.FunctionBaseParameterNameAlreadyDefinedDuplicate,
                ValidationRules.FunctionImportReturnEntitiesButDoesNotSpecifyEntitySet,
                ValidationRules.FunctionImportEntityTypeDoesNotMatchEntitySet,
                ValidationRules.ComposableFunctionImportMustHaveReturnType,
                ValidationRules.StructuredTypeBaseTypeMustBeSameKindAsDerivedKind,
                ValidationRules.RowTypeBaseTypeMustBeNull,
                ValidationRules.NavigationPropertyWithRecursiveContainmentTargetMustBeOptional,
                ValidationRules.NavigationPropertyWithRecursiveContainmentSourceMustBeFromZeroOrOne,
                ValidationRules.NavigationPropertyWithNonRecursiveContainmentSourceMustBeFromOne,
                ValidationRules.EntitySetInaccessibleEntityType,
                ValidationRules.StructuredTypeInaccessibleBaseType,
                ValidationRules.EntityReferenceTypeInaccessibleEntityType,
                ValidationRules.TypeReferenceInaccessibleSchemaType,
                ValidationRules.EntitySetTypeHasNoKeys,
                ValidationRules.FunctionOnlyInputParametersAllowedInFunctions,
                ValidationRules.RowTypeMustContainProperties,
                ValidationRules.DecimalTypeReferenceScaleOutOfRange,
                ValidationRules.BinaryTypeReferenceBinaryMaxLengthNegative,
                ValidationRules.StringTypeReferenceStringMaxLengthNegative,
                ValidationRules.StructuralPropertyInvalidPropertyTypeConcurrencyMode,
                ValidationRules.EnumMemberValueMustHaveSameTypeAsUnderlyingType,
                ValidationRules.EnumTypeEnumMemberNameAlreadyDefined,
                ValidationRules.FunctionImportBindableFunctionImportMustHaveParameters,
                ValidationRules.FunctionImportComposableFunctionImportCannotBeSideEffecting,
                ValidationRules.FunctionImportEntitySetExpressionIsInvalid,
                ValidationRules.BinaryTypeReferenceBinaryUnboundedNotValidForMaxLength,
                ValidationRules.StringTypeReferenceStringUnboundedNotValidForMaxLength,
                ValidationRules.ImmediateValueAnnotationElementAnnotationIsValid,
                ValidationRules.ValueAnnotationAssertCorrectExpressionType,
                ValidationRules.IfExpressionAssertCorrectTestType,
                ValidationRules.CollectionExpressionAllElementsCorrectType,
                ValidationRules.RecordExpressionPropertiesMatchType,
                ValidationRules.NavigationPropertyDependentPropertiesMustBelongToDependentEntity,
                ValidationRules.NavigationPropertyInvalidOperationMultipleEndsInAssociation,
                ValidationRules.NavigationPropertyEndWithManyMultiplicityCannotHaveOperationsSpecified,
                ValidationRules.NavigationPropertyTypeMismatchRelationshipConstraint,
                ValidationRules.NavigationPropertyDuplicateDependentProperty,
                ValidationRules.NavigationPropertyPrincipalEndMultiplicity,
                ValidationRules.NavigationPropertyDependentEndMultiplicity,
                ValidationRules.NavigationPropertyCorrectType,
                ValidationRules.ImmediateValueAnnotationElementAnnotationHasNameAndNamespace,
                ValidationRules.FunctionApplicationExpressionParametersMatchAppliedFunction,
                ValidationRules.VocabularyAnnotatableNoDuplicateAnnotations,
                ValidationRules.TemporalTypeReferencePrecisionOutOfRange,
                ValidationRules.DecimalTypeReferencePrecisionOutOfRange,
                ValidationRules.ModelDuplicateEntityContainerName,
                ValidationRules.FunctionImportParametersCannotHaveModeOfNone,
                ValidationRules.TypeMustNotHaveKindOfNone,
                ValidationRules.PrimitiveTypeMustNotHaveKindOfNone,
                ValidationRules.PropertyMustNotHaveKindOfNone,
                ValidationRules.TermMustNotHaveKindOfNone,
                ValidationRules.SchemaElementMustNotHaveKindOfNone,
                ValidationRules.EntityContainerElementMustNotHaveKindOfNone,
                ValidationRules.PrimitiveValueValidForType,
                ValidationRules.EntitySetCanOnlyBeContainedByASingleNavigationProperty,
                ValidationRules.EntitySetNavigationMappingMustBeBidirectional,
                ValidationRules.EntitySetNavigationPropertyMappingsMustBeUnique,
                ValidationRules.TypeAnnotationAssertMatchesTermType,
                ValidationRules.TypeAnnotationInaccessibleTerm,
                ValidationRules.PropertyValueBindingValueIsCorrectType,
                ValidationRules.EnumMustHaveIntegerUnderlyingType,
                ValidationRules.ValueAnnotationInaccessibleTerm,
                ValidationRules.ElementDirectValueAnnotationFullNameMustBeUnique,
                ValidationRules.VocabularyAnnotationInaccessibleTarget,
                ValidationRules.ComplexTypeMustContainProperties,
                ValidationRules.EntitySetAssociationSetNameMustBeValid,
                ValidationRules.NavigationPropertyAssociationEndNameIsValid,
                ValidationRules.NavigationPropertyAssociationNameIsValid,
                ValidationRules.OnlyEntityTypesCanBeOpen,
                ValidationRules.NavigationPropertyEntityMustNotIndirectlyContainItself,
                ValidationRules.EntitySetRecursiveNavigationPropertyMappingsMustPointBackToSourceEntitySet,
                ValidationRules.EntitySetNavigationPropertyMappingMustPointToValidTargetForProperty,
                ValidationRules.DirectValueAnnotationHasXmlSerializableName
            });
        
        private static readonly ValidationRuleSet V1RuleSet = 
            new ValidationRuleSet(
                BaseRuleSet,
                new ValidationRule[] 
                {
                    ValidationRules.NavigationPropertyInvalidToPropertyInRelationshipConstraintBeforeV2,
                    ValidationRules.FunctionsNotSupportedBeforeV2,
                    ValidationRules.FunctionImportUnsupportedReturnTypeV1,
                    ValidationRules.FunctionImportParametersIncorrectTypeBeforeV3,
                    ValidationRules.FunctionImportIsSideEffectingNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsComposableNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsBindableNotSupportedBeforeV3,
                    ValidationRules.EntityTypeEntityKeyMustNotBeBinaryBeforeV2,
                    ValidationRules.EnumTypeEnumsNotSupportedBeforeV3,
                    ValidationRules.NavigationPropertyContainsTargetNotSupportedBeforeV3,
                    ValidationRules.StructuralPropertyNullableComplexType,
                    ValidationRules.ValueTermsNotSupportedBeforeV3,
                    ValidationRules.VocabularyAnnotationsNotSupportedBeforeV3,
                    ValidationRules.OpenTypesNotSupported,
                    ValidationRules.StreamTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.SpatialTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.ModelDuplicateSchemaElementNameBeforeV3,
            });

        private static readonly ValidationRuleSet V1_1RuleSet = 
            new ValidationRuleSet(
                BaseRuleSet.Where(r => r != ValidationRules.ComplexTypeInvalidAbstractComplexType &&
                                       r != ValidationRules.ComplexTypeInvalidPolymorphicComplexType), 
                new ValidationRule[]
                {
                    ValidationRules.NavigationPropertyInvalidToPropertyInRelationshipConstraintBeforeV2,
                    ValidationRules.FunctionsNotSupportedBeforeV2,
                    ValidationRules.FunctionImportUnsupportedReturnTypeAfterV1,
                    ValidationRules.FunctionImportIsSideEffectingNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsComposableNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsBindableNotSupportedBeforeV3,
                    ValidationRules.EntityTypeEntityKeyMustNotBeBinaryBeforeV2,
                    ValidationRules.FunctionImportParametersIncorrectTypeBeforeV3,
                    ValidationRules.EnumTypeEnumsNotSupportedBeforeV3,
                    ValidationRules.NavigationPropertyContainsTargetNotSupportedBeforeV3,
                    ValidationRules.ValueTermsNotSupportedBeforeV3,
                    ValidationRules.VocabularyAnnotationsNotSupportedBeforeV3,
                    ValidationRules.OpenTypesNotSupported,
                    ValidationRules.StreamTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.SpatialTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.ModelDuplicateSchemaElementNameBeforeV3,
                });

        private static readonly ValidationRuleSet V1_2RuleSet = 
            new ValidationRuleSet(
                BaseRuleSet.Where(r => r != ValidationRules.ComplexTypeInvalidAbstractComplexType &&
                                       r != ValidationRules.ComplexTypeInvalidPolymorphicComplexType),
                new ValidationRule[] 
                {
                    ValidationRules.NavigationPropertyInvalidToPropertyInRelationshipConstraintBeforeV2,
                    ValidationRules.FunctionsNotSupportedBeforeV2,
                    ValidationRules.FunctionImportUnsupportedReturnTypeAfterV1,
                    ValidationRules.FunctionImportParametersIncorrectTypeBeforeV3,
                    ValidationRules.FunctionImportIsSideEffectingNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsComposableNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsBindableNotSupportedBeforeV3,
                    ValidationRules.EntityTypeEntityKeyMustNotBeBinaryBeforeV2,
                    ValidationRules.EnumTypeEnumsNotSupportedBeforeV3,
                    ValidationRules.NavigationPropertyContainsTargetNotSupportedBeforeV3,
                    ValidationRules.ValueTermsNotSupportedBeforeV3,
                    ValidationRules.VocabularyAnnotationsNotSupportedBeforeV3,
                    ValidationRules.StreamTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.SpatialTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.ModelDuplicateSchemaElementNameBeforeV3,
                });

        private static readonly ValidationRuleSet V2RuleSet = 
            new ValidationRuleSet(
                BaseRuleSet, 
                new ValidationRule[] 
                {
                    ValidationRules.FunctionImportParametersIncorrectTypeBeforeV3,
                    ValidationRules.FunctionImportUnsupportedReturnTypeAfterV1,
                    ValidationRules.FunctionImportIsSideEffectingNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsComposableNotSupportedBeforeV3,
                    ValidationRules.FunctionImportIsBindableNotSupportedBeforeV3,
                    ValidationRules.EnumTypeEnumsNotSupportedBeforeV3,
                    ValidationRules.NavigationPropertyContainsTargetNotSupportedBeforeV3,
                    ValidationRules.StructuralPropertyNullableComplexType,
                    ValidationRules.ValueTermsNotSupportedBeforeV3,
                    ValidationRules.VocabularyAnnotationsNotSupportedBeforeV3,
                    ValidationRules.OpenTypesNotSupported,
                    ValidationRules.StreamTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.SpatialTypeReferencesNotSupportedBeforeV3,
                    ValidationRules.ModelDuplicateSchemaElementNameBeforeV3,
                });

        private static readonly ValidationRuleSet V3RuleSet = 
            new ValidationRuleSet(
                BaseRuleSet, 
                new ValidationRule[]
                {
                    ValidationRules.FunctionImportUnsupportedReturnTypeAfterV1,
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

            if (version == EdmConstants.EdmVersion3)
            {
                return V3RuleSet;
            }

            throw new InvalidOperationException(Edm.Strings.Serializer_UnknownEdmVersion);
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
