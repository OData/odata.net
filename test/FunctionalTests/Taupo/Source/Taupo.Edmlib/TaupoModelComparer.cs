//---------------------------------------------------------------------
// <copyright file="TaupoModelComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Edmlib
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.EntityModel.Edm;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// A visitor that visits all the elements of an EntityModelSchema
    /// </summary>
    public class TaupoModelComparer : IEntityModelSchemaComparer
    {
        private List<string> errors = new List<string>();

        /// <summary>
        /// Compares the two models to each other, returns a list of error messages (if any).
        /// </summary>
        /// <param name="expectedTestEntityModel">Expected EntityModelSchema.</param>
        /// <param name="actualEntityModelSchema">Actual EntityModelSchema.</param>
        /// <returns>List of errors encountered.</returns>
        /// <remarks>
        /// This is a replacement of the default EntityModelSchemaComparer (in Taupo). It does more verification than the default one.
        /// Ideally this should be promoted into Taupo itself (as the default implementation).
        /// But since Astoria functional tests depend on the default implementation already, it's wise to do it locally first, 
        /// otherwise, it could break a lot of their tests when branch RIs
        /// </remarks>
        public ICollection<string> Compare(EntityModelSchema expectedTestEntityModel, EntityModelSchema actualEntityModelSchema)
        {
            this.errors.Clear();

            this.CompareStructuralTypes(expectedTestEntityModel.ComplexTypes.Cast<NamedStructuralType>(), actualEntityModelSchema.ComplexTypes.Cast<NamedStructuralType>(), "complex");
            this.CompareStructuralTypes(expectedTestEntityModel.EntityTypes.Cast<NamedStructuralType>(), actualEntityModelSchema.EntityTypes.Cast<NamedStructuralType>(), "entity");

            this.SatisfiesEquals(expectedTestEntityModel.Associations.Count(), actualEntityModelSchema.Associations.Count(), "Count of Associations does not match.");
            foreach (AssociationType expectedAssociation in expectedTestEntityModel.Associations)
            {
                var actualAssociations = actualEntityModelSchema.Associations.Where(a => a.FullName == expectedAssociation.FullName);
                if (this.SatisfiesEquals(1, actualAssociations.Count(), "Should find exactly 1 association '{0}'.", expectedAssociation.FullName))
                {
                    var actualAssociation = actualAssociations.Single();
                    this.CompareAssociationType(expectedAssociation, actualAssociation);
                    this.CompareAnnotations(expectedAssociation.Annotations, actualAssociation.Annotations);
                }
            }

            this.SatisfiesEquals(expectedTestEntityModel.EntityContainers.Count(), actualEntityModelSchema.EntityContainers.Count(), "Count of EntityContainers does not match.");
            foreach (EntityContainer expectedEntityContainer in expectedTestEntityModel.EntityContainers)
            {
                var actualEntityContainers = actualEntityModelSchema.EntityContainers.Where(ec => ec.Name == expectedEntityContainer.Name);
                if (this.SatisfiesEquals(1, actualEntityContainers.Count(), "Should find exactly 1 entityContainer '{0}'.", expectedEntityContainer.Name))
                {
                    var actualEntityContainer = actualEntityContainers.Single();
                    this.CompareEntityContainer(expectedEntityContainer, actualEntityContainer);
                    this.CompareAnnotations(expectedEntityContainer.Annotations, actualEntityContainer.Annotations);
                }
            }

            this.SatisfiesEquals(expectedTestEntityModel.Functions.Count(), actualEntityModelSchema.Functions.Count(), "Count of Functions does not match.");
            foreach (Function expectedFunction in expectedTestEntityModel.Functions)
            {
                var actualFunctions = actualEntityModelSchema.Functions.Where(f => this.FunctionSignaturesAreSame(f, expectedFunction));
                if (this.SatisfiesEquals(1, actualFunctions.Count(), "Should find exactly 1 function '{0}' (with parameters).", expectedFunction.FullName))
                {
                    var actualFunction = actualFunctions.Single();
                    this.CompareFunction(expectedFunction, actualFunction);
                    this.CompareAnnotations(expectedFunction.Annotations, actualFunction.Annotations);
                }
            }

            this.SatisfiesEquals(expectedTestEntityModel.EnumTypes.Count(), actualEntityModelSchema.EnumTypes.Count(), "Count of EnumTypes does not match.");
            foreach (EnumType expectedEnumType in expectedTestEntityModel.EnumTypes)
            {
                var actualEnumTypes = actualEntityModelSchema.EnumTypes.Where(e => e.FullName == expectedEnumType.FullName);
                if (this.SatisfiesEquals(1, actualEnumTypes.Count(), "Should find exactly 1 enum type '{0}'.", expectedEnumType.FullName))
                {
                    var actualEnumType = actualEnumTypes.Single();
                    this.CompareEnumType(expectedEnumType, actualEnumType);
                    this.CompareAnnotations(expectedEnumType.Annotations, actualEnumType.Annotations);
                }
            }

            this.CompareAnnotations(expectedTestEntityModel.Annotations, actualEntityModelSchema.Annotations);

            return this.errors;
        }

        private void CompareStructuralTypes(IEnumerable<NamedStructuralType> expected, IEnumerable<NamedStructuralType> actual, string description)
        {
            this.SatisfiesEquals(expected.Count(), actual.Count(), "Count of {0} types does not match.", description);
            foreach (var expectedType in expected)
            {
                var actualTypes = actual.Where(t => t.FullName == expectedType.FullName);
                if (this.SatisfiesEquals(1, actualTypes.Count(), "Should find exactly 1 {0} type named '{1}'.", description, expectedType.FullName))
                {
                    var actualType = actualTypes.Single();
                    this.CompareStructuralType(expectedType, actualType);
                    this.CompareAnnotations(expectedType.Annotations, actualType.Annotations);
                }
            }
        }

        private void CompareStructuralType(NamedStructuralType expected, NamedStructuralType actual)
        {
            var expectedEntityType = expected as EntityType;
            if (expectedEntityType != null)
            {
                this.CompareEntityType(expectedEntityType, (EntityType)actual);
            }
            else
            {
                this.CompareComplexType((ComplexType)expected, (ComplexType)actual);
            }
        }

        private bool SatisfiesEquals(object expected, object actual, string errorMessage, params object[] messageParameters)
        {
            string message = string.Format(CultureInfo.InvariantCulture, errorMessage, messageParameters);
            string additionalMessage = string.Format(CultureInfo.InvariantCulture, " - Expected: {0}, Actual: {1}.", expected, actual);

            return this.SatisfiesCondition(object.Equals(expected, actual), message + additionalMessage);
        }

        private bool SatisfiesCondition(bool assertCondition, string errorMessage, params object[] messageParameters)
        {
            if (assertCondition)
            {
                return true;
            }

            this.errors.Add(string.Format(CultureInfo.InvariantCulture, errorMessage, messageParameters));
            return false;
        }

        private void CompareComplexType(ComplexType expectedComplexType, ComplexType actualComplexType)
        {
            this.SatisfiesEquals(expectedComplexType.FullName, actualComplexType.FullName, "ComplexType name does not match.");

            this.CompareProperties(expectedComplexType, actualComplexType);
        }

        private void CompareProperties(NamedStructuralType expectedStructuralType, NamedStructuralType actualStructuralType)
        {
            // TODO: for open type, actual can have more properties
            this.SatisfiesEquals(
                expectedStructuralType.Properties.Count(),
                actualStructuralType.Properties.Count(),
                "Count of Properties of '{0}' does not match.",
                expectedStructuralType.FullName);

            foreach (MemberProperty memberProperty in expectedStructuralType.Properties)
            {
                var actualMemberProperties = actualStructuralType.Properties.Where(p => p.Name == memberProperty.Name);
                if (this.SatisfiesEquals(1, actualMemberProperties.Count(), "Should find exactly one member '{0}'.", memberProperty.Name))
                {
                    var actualMemberProperty = actualMemberProperties.Single();
                    this.CompareMemberProperty(memberProperty, actualMemberProperty);
                    this.CompareAnnotations(memberProperty.Annotations, actualMemberProperty.Annotations);
                }
            }
        }

        private void CompareMemberProperty(MemberProperty expectedProperty, MemberProperty actualProperty)
        {
            this.SatisfiesEquals(expectedProperty.Name, actualProperty.Name, "Property name does not match.");
            this.SatisfiesEquals(expectedProperty.IsPrimaryKey, actualProperty.IsPrimaryKey, "IsPrimaryKey of property '{0}' does not match.", expectedProperty.Name);

            this.CompareDataType(expectedProperty.PropertyType, actualProperty.PropertyType, "Property " + expectedProperty.Name);

            ConcurrencyTokenAnnotation expectedConcurrencyToken = expectedProperty.Annotations.OfType<ConcurrencyTokenAnnotation>().SingleOrDefault();
            ConcurrencyTokenAnnotation actualConcurrencyToken = actualProperty.Annotations.OfType<ConcurrencyTokenAnnotation>().SingleOrDefault();
            if (expectedConcurrencyToken != null)
            {
                this.SatisfiesCondition(actualConcurrencyToken != null, "Expected property '{0}' to be a concurrencyToken", expectedProperty.Name);
            }
            else
            {
                this.SatisfiesCondition(actualConcurrencyToken == null, "Expected property '{0}' to be NOT a concurrencyToken", expectedProperty.Name);
            }
        }

        private void CompareDataType(DataType expectedDataType, DataType actualDataType, string context)
        {
            this.SatisfiesCondition(this.DataTypesAreEqual(expectedDataType, actualDataType), "DataType does not match for {0}.", context);
        }

        private bool DataTypesAreEqual(DataType dataType1, DataType dataType2)
        {
            var primitive1 = dataType1 as PrimitiveDataType;
            var collection1 = dataType1 as CollectionDataType;
            var rowDataType1 = dataType1 as RowDataType;
            if (primitive1 != null)
            {
                var primitive2 = dataType2 as PrimitiveDataType;
                if (primitive2 == null)
                {
                    return false;
                }

                primitive1 = this.CompensatePrimitiveDefaultFacets(primitive1);
                primitive2 = this.CompensatePrimitiveDefaultFacets(primitive2);
                return primitive1.Equals(primitive2);
            }
            else if (collection1 != null)
            {
                var collection2 = dataType2 as CollectionDataType;
                if (collection2 == null)
                {
                    return false;
                }

                return this.DataTypesAreEqual(collection1.ElementDataType, collection2.ElementDataType);
            }
            else if (rowDataType1 != null)
            {
                var rowDataType2 = dataType2 as RowDataType;
                if (rowDataType2 == null || rowDataType1.Definition.Properties.Count() != rowDataType2.Definition.Properties.Count())
                {
                    return false;
                }

                foreach (var propertyType1 in rowDataType1.Definition.Properties)
                {
                    var propertyType2 = rowDataType2.Definition.Properties.SingleOrDefault(n => n.Name == propertyType1.Name && this.DataTypesAreEqual(propertyType1.PropertyType, n.PropertyType));
                    if (propertyType2 == null)
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return dataType1.Equals(dataType2);
            }
        }

        private PrimitiveDataType CompensatePrimitiveDefaultFacets(PrimitiveDataType inputDataType)
        {
            var inputBinary = inputDataType as BinaryDataType;
            var inputString = inputDataType as StringDataType;

            if (inputBinary == null && inputString == null)
            {
                return inputDataType;
            }

            int? maxLength = null;
            if (inputDataType.HasFacet<MaxLengthFacet>())
            {
                maxLength = inputDataType.GetFacet<MaxLengthFacet>().Value;
            }

            bool isUnicode = inputDataType.GetFacetValue<IsUnicodeFacet, bool>(true);

            if (inputBinary != null)
            {
                return EdmDataTypes.Binary(maxLength)
                                   .Nullable(inputDataType.IsNullable);
            }
            else
            {
                return EdmDataTypes.String(maxLength, isUnicode)
                                   .Nullable(inputDataType.IsNullable);
            }
        }

        private void CompareEntityType(EntityType expectedEntityType, EntityType actualEntityType)
        {
            this.SatisfiesEquals(expectedEntityType.FullName, actualEntityType.FullName, "EntityType name does not match.");
            this.SatisfiesEquals(expectedEntityType.IsAbstract, actualEntityType.IsAbstract, "IsAbstract does not match for EntityType '{0}'.", expectedEntityType.FullName);

            string expectedBaseTypeName = expectedEntityType.BaseType != null ? expectedEntityType.BaseType.FullName : null;
            string actualBaseTypeName = actualEntityType.BaseType != null ? actualEntityType.BaseType.FullName : null;
            this.SatisfiesEquals(expectedBaseTypeName, actualBaseTypeName, "BaseType does not match for EntityType '{0}'.", expectedEntityType.FullName);

            this.CompareProperties(expectedEntityType, actualEntityType);

            // TODO: for open type, actual can have more navigation properties
            this.SatisfiesEquals(
                expectedEntityType.NavigationProperties.Count(),
                actualEntityType.NavigationProperties.Count(),
                "Count of NavigationProperties of '{0}' does not match.",
                expectedEntityType.FullName);

            foreach (NavigationProperty navigationProperty in expectedEntityType.NavigationProperties)
            {
                var actualNavigationProperties = actualEntityType.NavigationProperties.Where(p => p.Name == navigationProperty.Name);
                if (this.SatisfiesEquals(1, actualNavigationProperties.Count(), "Should find exactly 1 NavProp '{0}'", navigationProperty.Name))
                {
                    var actualNavigationProperty = actualNavigationProperties.Single();
                    this.CompareNavigationProperty(navigationProperty, actualNavigationProperty);
                    this.CompareAnnotations(navigationProperty.Annotations, actualNavigationProperty.Annotations);
                }
            }
        }

        private void CompareNavigationProperty(NavigationProperty expectedNavigationProperty, NavigationProperty actualNavigationProperty)
        {
            this.SatisfiesEquals(expectedNavigationProperty.Name, actualNavigationProperty.Name, "Navigation property name does not match.");

            this.SatisfiesEquals(
                expectedNavigationProperty.Association.FullName,
                actualNavigationProperty.Association.FullName,
                "Association does not match for navigation property '{0}'.",
                expectedNavigationProperty.Name);

            this.SatisfiesEquals(
                expectedNavigationProperty.FromAssociationEnd.RoleName,
                actualNavigationProperty.FromAssociationEnd.RoleName,
                "FromAssociationEnd does not match for navigation property '{0}'.",
                expectedNavigationProperty.Name);

            this.SatisfiesEquals(
                expectedNavigationProperty.ToAssociationEnd.RoleName,
                actualNavigationProperty.ToAssociationEnd.RoleName,
                "ToAssociationEnd does not match for navigation property '{0}'.",
                expectedNavigationProperty.Name);
        }

        private void CompareAssociationType(AssociationType expectedAssociationType, AssociationType actualAssociationType)
        {
            this.SatisfiesEquals(expectedAssociationType.FullName, actualAssociationType.FullName, "AssociationType name does not match.");

            foreach (var expectedAssociationEnd in expectedAssociationType.Ends)
            {
                var actualAssociationEnds = actualAssociationType.Ends.Where(e => e.RoleName == expectedAssociationEnd.RoleName);
                if (this.SatisfiesEquals(1, actualAssociationEnds.Count(), "Should find exactly 1 AssociationEnd '{0}' in '{1}'", expectedAssociationEnd.RoleName, expectedAssociationType.FullName))
                {
                    this.CompareAssociationEnd(expectedAssociationEnd, actualAssociationEnds.Single());
                }
            }

            if (expectedAssociationType.ReferentialConstraint == null)
            {
                this.SatisfiesCondition(actualAssociationType.ReferentialConstraint == null, "Expected no referential constraint on '{0}'", expectedAssociationType.FullName);
            }
            else
            {
                if (this.SatisfiesCondition(actualAssociationType.ReferentialConstraint != null, "Expected to have referential constraint on '{0}'", expectedAssociationType.FullName))
                {
                    this.CompareReferentialConstraint(expectedAssociationType.ReferentialConstraint, actualAssociationType.ReferentialConstraint);
                }
            }
        }

        private void CompareAssociationEnd(AssociationEnd expectedEnd, AssociationEnd actualEnd)
        {
            this.SatisfiesEquals(expectedEnd.RoleName, actualEnd.RoleName, "RoleName not as expeted.");
            this.SatisfiesEquals(expectedEnd.Multiplicity, actualEnd.Multiplicity, "Multiplicity does not match on AssociationEnd '{0}'.", expectedEnd.RoleName);
            this.SatisfiesEquals(expectedEnd.EntityType.FullName, actualEnd.EntityType.FullName, "EntityType name does not match on AssociationEnd '{0}'.", expectedEnd.RoleName);
            this.SatisfiesEquals(expectedEnd.DeleteBehavior, actualEnd.DeleteBehavior, "DeleteBehavior does not match on AssociationEnd '{0}'.", expectedEnd.RoleName);
        }

        private void CompareReferentialConstraint(ReferentialConstraint expectedReferentialConstraint, ReferentialConstraint actualReferentialConstraint)
        {
            this.SatisfiesEquals(
                expectedReferentialConstraint.PrincipalAssociationEnd.RoleName,
                actualReferentialConstraint.PrincipalAssociationEnd.RoleName,
                "Principal end does not match.");

            this.SatisfiesEquals(
                expectedReferentialConstraint.DependentAssociationEnd.RoleName,
                actualReferentialConstraint.DependentAssociationEnd.RoleName,
                "Dependent end does not match.");

            bool principalCountMatches = this.SatisfiesEquals(
                    expectedReferentialConstraint.PrincipalProperties.Count,
                    actualReferentialConstraint.PrincipalProperties.Count,
                    "Princpal end property count does not match.");

            bool dependentCountMatches = this.SatisfiesEquals(
                    expectedReferentialConstraint.DependentProperties.Count,
                    actualReferentialConstraint.DependentProperties.Count,
                    "Dependent end property count does not match.");

            if (principalCountMatches && dependentCountMatches)
            {
                // Ordering doesn't need to be exact match
                // As long as principal properties and dependent properties are consistent, it's fine.
                for (int originalIndex = 0; originalIndex < expectedReferentialConstraint.PrincipalProperties.Count; originalIndex++)
                {
                    string propertyName = expectedReferentialConstraint.PrincipalProperties[originalIndex].Name;
                    MemberProperty actualProperty = actualReferentialConstraint.PrincipalProperties.SingleOrDefault(p => p.Name == propertyName);
                    int actualIndex = actualReferentialConstraint.PrincipalProperties.IndexOf(actualProperty);

                    if (this.SatisfiesCondition(actualIndex >= 0, "Cannot find property '{0}' in ReferentialConstraint PrincipalEnd.", propertyName))
                    {
                        this.SatisfiesEquals(
                            expectedReferentialConstraint.DependentProperties[originalIndex].Name,
                            actualReferentialConstraint.DependentProperties[actualIndex].Name,
                            "Dependent end property #{0} does not match.",
                            originalIndex);
                    }
                }
            }
        }

        private void CompareEntityContainer(EntityContainer expectedEntityContainer, EntityContainer actualEntityContainer)
        {
            this.SatisfiesEquals(
                expectedEntityContainer.EntitySets.Count(),
                actualEntityContainer.EntitySets.Count(),
                "Count of EntitySets does not match for '{0}'.",
                expectedEntityContainer.Name);
            foreach (EntitySet expectedEntitySet in expectedEntityContainer.EntitySets)
            {
                var actualEntitySets = actualEntityContainer.EntitySets.Where(es => es.Name == expectedEntitySet.Name);
                if (this.SatisfiesEquals(1, actualEntitySets.Count(), "Should find exactly 1 entitySet '{0}'", expectedEntitySet.Name))
                {
                    var actualEntitySet = actualEntitySets.Single();
                    this.CompareEntitySet(expectedEntitySet, actualEntitySet);
                    this.CompareAnnotations(expectedEntitySet.Annotations, actualEntitySet.Annotations);
                }
            }

            this.SatisfiesEquals(
                expectedEntityContainer.AssociationSets.Count(),
                actualEntityContainer.AssociationSets.Count(),
                "Count of AssociationSets does not match for '{0}'.",
                expectedEntityContainer.Name);
            foreach (AssociationSet expectedAssociationSet in expectedEntityContainer.AssociationSets)
            {
                var actualAssociationSets = actualEntityContainer.AssociationSets.Where(asSet => asSet.Name == expectedAssociationSet.Name);
                if (this.SatisfiesEquals(1, actualAssociationSets.Count(), "Should find exactly 1 associationSet '{0}'", expectedAssociationSet.Name))
                {
                    var actualAssociationSet = actualAssociationSets.Single();
                    this.CompareAssociationSet(expectedAssociationSet, actualAssociationSet);
                    this.CompareAnnotations(expectedAssociationSet.Annotations, actualAssociationSet.Annotations);
                }
            }

            this.SatisfiesEquals(
                expectedEntityContainer.FunctionImports.Count(),
                actualEntityContainer.FunctionImports.Count(),
                "Count of FunctionImports does not match for '{0}'.",
                expectedEntityContainer.Name);
            foreach (FunctionImport expectedFunctionImport in expectedEntityContainer.FunctionImports)
            {
                var actualFunctionImports = actualEntityContainer.FunctionImports.Where(fi => this.FunctionImportSignaturesAreSame(fi, expectedFunctionImport));
                if (this.SatisfiesEquals(1, actualFunctionImports.Count(), "Should find exactly 1 FunctionImport '{0}'", expectedFunctionImport.Name))
                {
                    var actualFunctionImport = actualFunctionImports.Single();
                    this.CompareFunctionImport(expectedFunctionImport, actualFunctionImport);
                    this.CompareAnnotations(expectedFunctionImport.Annotations, actualFunctionImport.Annotations);
                }
            }
        }

        private void CompareEntitySet(EntitySet expectedEntitySet, EntitySet actualEntitySet)
        {
            this.SatisfiesEquals(expectedEntitySet.EntityType.FullName, actualEntitySet.EntityType.FullName, "EntitySet EntityType does not match.");
        }

        private void CompareAssociationSet(AssociationSet expectedAssociationSet, AssociationSet actualAssociationSet)
        {
            this.SatisfiesEquals(
                expectedAssociationSet.AssociationType.FullName,
                actualAssociationSet.AssociationType.FullName,
                "Association type name not match for AssociationSet '{0}'.",
                expectedAssociationSet.Name);

            foreach (AssociationSetEnd expectedEnd in expectedAssociationSet.Ends)
            {
                var actualAssociationSetEnds = actualAssociationSet.Ends.Where(asEnd => asEnd.AssociationEnd.RoleName == expectedEnd.AssociationEnd.RoleName);
                if (this.SatisfiesEquals(1, actualAssociationSetEnds.Count(), "Should find exactly 1 associationSetEnd '{0}' ", expectedEnd.AssociationEnd.RoleName))
                {
                    var actualAssociationSetEnd = actualAssociationSetEnds.Single();
                    this.CompareAssociationSetEnd(expectedEnd, actualAssociationSetEnd);
                    this.CompareAnnotations(expectedEnd.Annotations, actualAssociationSetEnd.Annotations);
                }
            }
        }

        private void CompareAssociationSetEnd(AssociationSetEnd expectedEnd, AssociationSetEnd actualEnd)
        {
            this.SatisfiesEquals(expectedEnd.EntitySet.Name, actualEnd.EntitySet.Name, "AssociationEnd '{0}' EntitySet name not match.", expectedEnd.AssociationEnd.RoleName);
        }

        private void CompareFunctionImport(FunctionImport expectedFunctionImport, FunctionImport actualFunctionImport)
        {
            this.SatisfiesCondition(expectedFunctionImport.ReturnTypes.Count == actualFunctionImport.ReturnTypes.Count, "FunctionImport '{0}' has the wrong number of return types. Expected:{1} Actual:{2}.", expectedFunctionImport.Name, expectedFunctionImport.ReturnTypes.Count, actualFunctionImport.ReturnTypes.Count);
            for (int i = 0; i < expectedFunctionImport.ReturnTypes.Count; i++)
            {
                var expectedReturnType = expectedFunctionImport.ReturnTypes[i];
                var actualReturnType = actualFunctionImport.ReturnTypes[i];
                if (expectedReturnType.EntitySet == null)
                {
                    this.SatisfiesCondition(actualReturnType.EntitySet == null, "FunctionImport '{0}' should not have EntitySet.", expectedFunctionImport.Name);
                }
                else
                {
                    this.SatisfiesEquals(
                        expectedReturnType.EntitySet.Name,
                        actualReturnType.EntitySet.Name,
                        "EntitySet on FunctionImport '{0}' does not match.",
                        expectedFunctionImport.Name);
                }

                this.CompareReturnType(
                    expectedReturnType.DataType,
                    actualReturnType.DataType,
                    string.Format(CultureInfo.InvariantCulture, "FunctionImport '{0}'", expectedFunctionImport.Name));
            }

            this.CompareParameters(
                expectedFunctionImport.Parameters,
                actualFunctionImport.Parameters,
                string.Format(CultureInfo.InvariantCulture, "FunctionImport '{0}'", expectedFunctionImport.Name));
        }

        private void CompareReturnType(DataType expectedReturnType, DataType actualReturnType, string context)
        {
            if (expectedReturnType == null)
            {
                this.SatisfiesCondition(actualReturnType == null, context + " should not have ReturnType.");
            }
            else
            {
                this.CompareDataType(expectedReturnType, actualReturnType, "ReturnType of " + context);
            }
        }

        private void CompareParameters(IList<FunctionParameter> expectedParameters, IList<FunctionParameter> actualParameters, string context)
        {
            this.SatisfiesEquals(
                expectedParameters.Count,
                actualParameters.Count,
                "Count of parameters does not match for " + context);
            for (int i = 0; i < expectedParameters.Count; i++)
            {
                this.CompareFunctionParameter(
                    expectedParameters[i],
                    actualParameters[i],
                    string.Format(CultureInfo.InvariantCulture, "Parameter #{0} of {1}", i, context));
            }
        }

        private void CompareFunctionParameter(FunctionParameter expectedParameter, FunctionParameter actualPrameter, string context)
        {
            this.SatisfiesEquals(expectedParameter.Name, actualPrameter.Name, "Parameter name does not match for " + context);
            this.SatisfiesEquals(expectedParameter.Mode, actualPrameter.Mode, "Mode does not match for " + context);

            this.CompareDataType(expectedParameter.DataType, actualPrameter.DataType, context);
        }

        private void CompareFunction(Function expectedFunction, Function actualFunction)
        {
            this.CompareReturnType(
                expectedFunction.ReturnType,
                actualFunction.ReturnType,
                string.Format(CultureInfo.InvariantCulture, "Function '{0}'", expectedFunction.Name));

            this.CompareParameters(
                expectedFunction.Parameters,
                actualFunction.Parameters,
                string.Format(CultureInfo.InvariantCulture, "Function '{0}'", expectedFunction.Name));
        }

        private bool FunctionSignaturesAreSame(Function f1, Function f2)
        {
            if (f1.FullName != f2.FullName)
            {
                return false;
            }

            return this.ParametersMatch(f1.Parameters, f2.Parameters);
        }

        private bool FunctionImportSignaturesAreSame(FunctionImport f1, FunctionImport f2)
        {
            if (f1.Name != f2.Name)
            {
                return false;
            }

            return this.ParametersMatch(f1.Parameters, f2.Parameters);
        }

        private bool ParametersMatch(IList<FunctionParameter> parameters1, IList<FunctionParameter> parameters2)
        {
            if (parameters1.Count != parameters2.Count)
            {
                return false;
            }

            for (int i = 0; i < parameters1.Count; i++)
            {
                if (!this.DataTypesAreEqual(parameters1[i].DataType, parameters2[i].DataType))
                {
                    return false;
                }
            }

            return true;
        }

        private void CompareEnumType(EnumType expectedEnumType, EnumType actualEnumType)
        {
            this.SatisfiesEquals(expectedEnumType.FullName, actualEnumType.FullName, "Enum type name does not match.");

            if (expectedEnumType.IsFlags == null)
            {
                this.SatisfiesCondition(actualEnumType.IsFlags == null, "IsFlags should be null.");
            }
            else
            {
                this.SatisfiesEquals(expectedEnumType.IsFlags, actualEnumType.IsFlags, "IsFlags does not match.");
            }

            if (expectedEnumType.UnderlyingType == null)
            {
                this.SatisfiesCondition(actualEnumType.UnderlyingType == null, "Underlying type should be null.");
            }
            else
            {
                this.SatisfiesEquals(expectedEnumType.UnderlyingType, actualEnumType.UnderlyingType, "Underlying type does not match.");
            }

            this.CompareEnumMembers(expectedEnumType, actualEnumType);
        }

        private object GetIntegralEnumMemberValue(EnumType enumType, EnumMember enumMember)
        {
            var targetEnumMember = enumType.Members.Single(n => n == enumMember);

            if (targetEnumMember.Value != null)
            {
                long integralValue = 0;
                if (long.TryParse(targetEnumMember.Value.ToString(), out integralValue))
                {
                    return integralValue;
                }

                return targetEnumMember.Value;
            }

            if (enumType.Members.ElementAt(0) == targetEnumMember)
            {
                return 0L;
            }

            int memberIndex = 0;
            for (int i = 1; i < enumType.Members.Count; ++i)
            {
                if (enumType.Members.ElementAt(i) == targetEnumMember)
                {
                    memberIndex = i;
                    break;
                }
            }

            var previousMember = enumType.Members.ElementAt(memberIndex - 1);
            var previousValue = previousMember.Value;
            if (previousValue == null)
            {
                previousValue = this.GetIntegralEnumMemberValue(enumType, previousMember);
            }

            long previousIntegralValue = 0;
            if (long.TryParse(previousValue.ToString(), out previousIntegralValue))
            {
                return previousIntegralValue + 1;
            }
            else
            {
                return targetEnumMember.Value;
            }
        }

        private void CompareEnumMembers(EnumType expectedEnumType, EnumType actualEnumType)
        {
            this.SatisfiesEquals(expectedEnumType.Members.Count, actualEnumType.Members.Count, "Count of Members of '{0}' does not match.", expectedEnumType.FullName);

            foreach (var expectedMemeber in expectedEnumType.Members)
            {
                var actualMembers = actualEnumType.Members.Where(m => m.Name == expectedMemeber.Name);
                if (this.SatisfiesEquals(1, actualMembers.Count(), "Should find exactly one member '{0}'.", expectedMemeber.Name))
                {
                    var actualMember = actualMembers.Single();
                    this.CompareEnumMember(expectedEnumType, expectedMemeber, actualEnumType, actualMember);
                }
            }
        }

        private void CompareEnumMember(EnumType expectedEnumType, EnumMember expectedMemeber, EnumType actualEnumType, EnumMember actualMember)
        {
            this.SatisfiesEquals(expectedMemeber.Name, actualMember.Name, "Enum member name not match.");
            this.SatisfiesEquals(this.GetIntegralEnumMemberValue(expectedEnumType, expectedMemeber), this.GetIntegralEnumMemberValue(actualEnumType, actualMember), "Enum member value does not match.");
            this.CompareAnnotations(expectedMemeber.Annotations, actualMember.Annotations);
        }

        /// <summary>
        /// The comparison is going to be loose because there was no comparison of annotations before. 
        /// So to not break existing tests we have to compare annotations that implement the IComparableAnnotation interface being introduced with this change.
        /// Also the comparison is orderless 
        /// </summary>
        /// <param name="expectedAnnotations">expected Annotations</param>
        /// <param name="actualAnnotations">actual Annotations</param>
        private void CompareAnnotations(IEnumerable<Annotation> expectedAnnotations, IEnumerable<Annotation> actualAnnotations)
        {
            var expectedComparableAnnotations = expectedAnnotations.OfType<IComparableAnnotation>();
            var actualComparableAnnotations = actualAnnotations.OfType<IComparableAnnotation>();

            if (this.SatisfiesEquals(expectedComparableAnnotations.Count(), actualComparableAnnotations.Count(), "ComparableAnnotation Count"))
            {
                var copyOfActualComparableAnnotations = new List<IComparableAnnotation>(actualComparableAnnotations);
                foreach (var expectedAnnotation in expectedComparableAnnotations)
                {
                    var equalAnnotation = copyOfActualComparableAnnotations.FirstOrDefault(
                        actualAnn => expectedAnnotation.CompareAnnotation(actualAnn as Annotation));

                    if (this.SatisfiesCondition(equalAnnotation != null, "None of the actual annotations matched the expectedAnnotation"))
                    {
                        copyOfActualComparableAnnotations.Remove(equalAnnotation);
                    }
                }
            }
        }
    }
}
