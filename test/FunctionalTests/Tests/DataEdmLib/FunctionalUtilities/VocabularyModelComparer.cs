//---------------------------------------------------------------------
// <copyright file="VocabularyModelComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    public class VocabularyModelComparer
    {
        private IList<string> errorMessages;
        private ScopeContext scopeContext;
        private IEdmModel expectedModel;
        private IEdmModel actualModel;

        public IList<string> CompareModels(IEdmModel expected, IEdmModel actual)
        {
            this.errorMessages = new List<string>();
            this.scopeContext = new ScopeContext();

            // ?? caused by bad product design: xxx.VocabularyAnnotations(IEdmModel)
            this.expectedModel = expected;
            this.actualModel = actual;

            // TODO: compare other regular stuffs: 
            // 0: complex type, property 
            // 1: association, association set, function, function import, enum
            this.CompareEntityContainers(expected.SchemaElements.OfType<IEdmEntityContainer>(), actual.SchemaElements.OfType<IEdmEntityContainer>());

            this.CompareEntityTypes(expected.SchemaElements.OfType<IEdmEntityType>(), actual.SchemaElements.OfType<IEdmEntityType>());

            this.CompareTerms(expected.SchemaElements.OfType<IEdmTerm>(), actual.SchemaElements.OfType<IEdmTerm>());

            this.CheckIsEquivalentTo(expected, actual);

            return this.errorMessages;
        }

        private void CheckIsEquivalentTo(IEdmModel expected, IEdmModel actual)
        {
            this.CheckComplexTypeIsEquivalentTo(expected.SchemaElements.OfType<IEdmComplexType>(), expected.SchemaElements.OfType<IEdmComplexType>());
            this.CheckEnumTypeIsEquivalentTo(expected.SchemaElements.OfType<IEdmEnumType>(), expected.SchemaElements.OfType<IEdmEnumType>());
            this.CheckFunctionIsEquivalentTo(expected.SchemaElements.OfType<IEdmOperation>(), expected.SchemaElements.OfType<IEdmOperation>());
        }

        private void CheckComplexTypeIsEquivalentTo(IEnumerable<IEdmComplexType> expectedComplexTypes, IEnumerable<IEdmComplexType> actualComplexTypes)
        {
            if (this.SatisfiesEquals(expectedComplexTypes.Count(), actualComplexTypes.Count(), "Invalid ComplexType count."))
            {
                var actualList = actualComplexTypes.ToList();
                foreach (var expectedComplexType in expectedComplexTypes)
                {
                    var actualComplexType = actualList.FirstOrDefault(a => a.Name.Equals(expectedComplexType.Name));
                    this.SatisfiesCondition(expectedComplexType.IsEquivalentTo(actualComplexType), "Expected ComplexType {0} is not equivalent to actual ComplexType {1}", expectedComplexType.Name, actualComplexType.Name);

                    this.CheckPropertyIsEquivalentTo(expectedComplexType.Properties(), actualComplexType.Properties());
                }
            }
        }

        private void CheckPropertyIsEquivalentTo(IEnumerable<IEdmProperty> expectedProperties, IEnumerable<IEdmProperty> actualProperties)
        {
            if (this.SatisfiesEquals(expectedProperties.Count(), actualProperties.Count(), "Invalid ComplexType count."))
            {
                var actualList = actualProperties.ToList();
                foreach (var expectedProperty in expectedProperties)
                {
                    var actualProperty = actualList.FirstOrDefault(a => a.Name.Equals(expectedProperty.Name));
                    this.SatisfiesCondition(expectedProperty.Type.IsEquivalentTo(actualProperty.Type), "Expected Property.Type {0} is not equivalent to actual Property.Type {1}", expectedProperty.Name, actualProperty.Name);
                }
            }
        }

        private void CheckEnumTypeIsEquivalentTo(IEnumerable<IEdmEnumType> expectedEnumTypes, IEnumerable<IEdmEnumType> actualEnumTypes)
        {
            if (this.SatisfiesEquals(expectedEnumTypes.Count(), actualEnumTypes.Count(), "Invalid EnumType count."))
            {
                var actualList = actualEnumTypes.ToList();
                foreach (var expectedEnumType in expectedEnumTypes)
                {
                    var actualEnumType = actualList.FirstOrDefault(a => a.Name.Equals(expectedEnumType.Name));
                    this.SatisfiesCondition(expectedEnumType.IsEquivalentTo(actualEnumType), "Expected EnumType {0} is not equivalent to actual EnumType {1}", expectedEnumType.Name, actualEnumType.Name);
                }
            }
        }
        
        private void CheckFunctionIsEquivalentTo(IEnumerable<IEdmOperation> expectedFunctions, IEnumerable<IEdmOperation> actualFunctions)
        {
            if (this.SatisfiesEquals(expectedFunctions.Count(), actualFunctions.Count(), "Invalid Function count."))
            {
                var actualList = actualFunctions.ToList();
                foreach (var expectedFunction in expectedFunctions)
                {
                    var actualFunction = actualList.FirstOrDefault(a => a.Name.Equals(expectedFunction.Name));
                    this.CheckFunctionParameterIsEquivalentTo(expectedFunction.Parameters, actualFunction.Parameters);
                }
            }
        }

        private void CheckFunctionImportIsEquivalentTo(IEnumerable<IEdmOperationImport> expectedFunctionImports, IEnumerable<IEdmOperationImport> actualFunctionImports)
        {
            if (this.SatisfiesEquals(expectedFunctionImports.Count(), actualFunctionImports.Count(), "Invalid FunctionImport count."))
            {
                var actualList = actualFunctionImports.ToList();
                foreach (var expectedFunctionImport in expectedFunctionImports)
                {
                    var actualFunctionImport = actualList.FirstOrDefault(a => a.Name.Equals(expectedFunctionImport.Name));
                    this.CheckFunctionParameterIsEquivalentTo(expectedFunctionImport.Operation.Parameters, actualFunctionImport.Operation.Parameters);
                }
            }
        }
        
        private void CheckFunctionParameterIsEquivalentTo(IEnumerable<IEdmOperationParameter> expectedParameters, IEnumerable<IEdmOperationParameter> actualParameters)
        {
            if (this.SatisfiesEquals(expectedParameters.Count(), actualParameters.Count(), "Invalid Parameter count."))
            {
                var actualList = actualParameters.ToList();
                foreach (var expectedParameter in expectedParameters)
                {
                    var actualFunctionParameter = actualList.FirstOrDefault(a => a.Name.Equals(expectedParameter.Name));
                    this.SatisfiesCondition(expectedParameter.Type.IsEquivalentTo(actualFunctionParameter.Type), "Expected Parameter.Type {0} is not equivalent to actual Parameter.Type {1}", expectedParameter.Name, actualFunctionParameter.Name);
                }
            }
        }

        private void CompareEntityContainers(IEnumerable<IEdmEntityContainer> expectedContainers, IEnumerable<IEdmEntityContainer> actualContainers)
        {
            if (this.SatisfiesEquals(expectedContainers.Count(), actualContainers.Count(), "Wrong Container count."))
            {
                var actualList = actualContainers.ToList();
                foreach (var expectedContainer in expectedContainers)
                {
                    var actualContainer = actualList.FirstOrDefault(a => a.Name == expectedContainer.Name);
                    if (this.SatisfiesCondition(actualContainer != null, "Cannot find EntityContainer {0}", expectedContainer.Name))
                    {
                        this.scopeContext.Enter(expectedContainer.Name);

                        this.CompareEntityContainer(expectedContainer, actualContainer);
                        actualList.Remove(actualContainer);

                        this.scopeContext.ExitCurrentScope();
                    }
                }

                foreach (var nonMatchedTerm in actualList)
                {
                    this.WriteErrorMessage("Cannot find EntityContainer {0}", nonMatchedTerm.Name);
                }
            }
        }

        private void CompareEntityContainer(IEdmEntityContainer expectedContainer, IEdmEntityContainer actualContainer)
        {
            this.CompareEntitySets(expectedContainer.EntitySets(), actualContainer.EntitySets());

            this.CompareTermAnnotations(expectedContainer, actualContainer);

            this.CheckFunctionImportIsEquivalentTo(expectedContainer.OperationImports(), actualContainer.OperationImports());
        }

        private void CompareEntitySets(IEnumerable<IEdmEntitySet> exptectedEntitySets, IEnumerable<IEdmEntitySet> actualEntitySets)
        {
            if (this.SatisfiesEquals(exptectedEntitySets.Count(), actualEntitySets.Count(), "Wrong EntitySet count."))
            {
                var actualList = actualEntitySets.ToList();
                foreach (var expectedEntitySet in exptectedEntitySets)
                {
                    var actualEntitySet = actualList.FirstOrDefault(a => a.Name == expectedEntitySet.Name);
                    if (this.SatisfiesCondition(actualEntitySet != null, "Cannot find EntitySet {0}", expectedEntitySet.Name))
                    {
                        this.scopeContext.Enter(expectedEntitySet.Name);

                        this.CompareEntitySet(expectedEntitySet, actualEntitySet);

                        actualList.Remove(actualEntitySet);

                        this.scopeContext.ExitCurrentScope();
                    }
                }

                foreach (var nonMatchedTerm in actualList)
                {
                    this.WriteErrorMessage("Cannot find EntitySet {0}", nonMatchedTerm.Name);
                }
            }
        }

        private void CompareEntitySet(IEdmEntitySet expectedEntitySet, IEdmEntitySet actualEntitySet)
        {
            this.CompareTermAnnotations(expectedEntitySet, actualEntitySet);
        }

        private void CompareEntityTypes(IEnumerable<IEdmEntityType> expectedEntities, IEnumerable<IEdmEntityType> actualEntities)
        {
            if (this.SatisfiesEquals(expectedEntities.Count(), actualEntities.Count(), "Wrong EntityType count."))
            {
                var actualList = actualEntities.ToList();
                foreach (var expectedEntity in expectedEntities)
                {
                    var actualEntity = actualList.FirstOrDefault(a => a.FullName() == expectedEntity.FullName());
                    if (this.SatisfiesCondition(actualEntity != null, "Cannot find EntityType {0}", expectedEntity.FullName()))
                    {
                        this.scopeContext.Enter(expectedEntity.Name);
                        this.CompareEntityType(expectedEntity, actualEntity);
                        actualList.Remove(actualEntity);

                        this.scopeContext.ExitCurrentScope();
                    }
                }

                foreach (var nonMatchedTerm in actualList)
                {
                    this.WriteErrorMessage("Cannot find EntityType {0}", nonMatchedTerm.FullName());
                }
            }
        }

        private void CompareTerms(IEnumerable<IEdmTerm> expectedValueTerms, IEnumerable<IEdmTerm> actualValueTerms)
        {
            if (this.SatisfiesEquals(expectedValueTerms.Count(), actualValueTerms.Count(), "Wrong ValueTerm count."))
            {
                var actualList = actualValueTerms.ToList();
                foreach (var expectedValueTerm in expectedValueTerms)
                {
                    var actualValueTerm = actualList.FirstOrDefault(a => a.Name == expectedValueTerm.Name && a.Namespace == expectedValueTerm.Namespace);
                    if (this.SatisfiesCondition(actualValueTerm != null, "Cannot find ValueTerm {0} in {1}", expectedValueTerm.Name, expectedValueTerm.Namespace))
                    {
                        this.scopeContext.Enter(expectedValueTerm.Name);

                        this.CompareTerm(expectedValueTerm, actualValueTerm);
                        actualList.Remove(actualValueTerm);

                        this.scopeContext.ExitCurrentScope();
                    }
                }

                foreach (var nonMatchedTerm in actualList)
                {
                    this.WriteErrorMessage("Cannot find ValueTerm {0} in {1}", nonMatchedTerm.Name, nonMatchedTerm.Namespace);
                }
            }
        }

        private void CompareTerm(IEdmTerm expectedValueTerm, IEdmTerm actualValueTerm)
        {
            // TODO: can we push type reference equals into product? 
            this.SatisfiesCondition(
                this.EdmTypeReferenceEquals(expectedValueTerm.Type, actualValueTerm.Type),
                "Type of ValueTerm {0} in {1} not Equal.",
                expectedValueTerm.Name,
                expectedValueTerm.Namespace);

            this.CompareTermAnnotations(expectedValueTerm, actualValueTerm);
        }

        private bool EdmTypeReferenceEquals(IEdmTypeReference expectedTypeReference, IEdmTypeReference actualTypeReference)
        {
            return this.SatisfiesEquals(expectedTypeReference.IsNullable, actualTypeReference.IsNullable, "TypeReference IsNullable.") &&
                   this.SatisfiesEquals(expectedTypeReference.FullName(), actualTypeReference.FullName(), "TypeReference definition full name.") &&
                   expectedTypeReference.TypeKind() != EdmTypeKind.Entity ?
                        this.SatisfiesEquals(expectedTypeReference.ToString(), actualTypeReference.ToString(), "TypeReference ToString().") : true;
        }

        private void CompareTermAnnotations(IEdmVocabularyAnnotatable expectedTarget, IEdmVocabularyAnnotatable actualTarget)
        {
            IEnumerable<IEdmVocabularyAnnotation> expectedAnnotations = this.expectedModel.FindVocabularyAnnotations(expectedTarget);
            IEnumerable<IEdmVocabularyAnnotation> actualAnnotations = this.actualModel.FindVocabularyAnnotations(actualTarget);

            if (this.SatisfiesEquals(expectedAnnotations.Count(), actualAnnotations.Count(), "Wrong TermAnnotations count at {0}.", this.scopeContext))
            {
                var actualList = actualAnnotations.ToList();
                foreach (var expectedTermAnnotation in expectedAnnotations)
                {
                    // TODO: push correct behavior for Term.Equals() and Term.ToString()?
                    var actualTermAnnotation = actualList.FirstOrDefault(a => a.Term.Name.Equals(expectedTermAnnotation.Term.Name) &&
                                                                              a.Term.Namespace == expectedTermAnnotation.Term.Namespace &&
                                                                              a.Qualifier == expectedTermAnnotation.Qualifier);
                    if (this.SatisfiesCondition(actualTermAnnotation != null, "Cannot find TermAnnotation {0} with Qualifier {1}.", expectedTermAnnotation.Term, expectedTermAnnotation.Qualifier))
                    {
                        this.CompareTermAnnotationContent(expectedTermAnnotation, actualTermAnnotation);
                    }

                    actualList.Remove(actualTermAnnotation);
                }

                foreach (var nonMatchedTerm in actualList)
                {
                    this.WriteErrorMessage("Cannot find TermAnnotation {0} with Qualifier {1}.", nonMatchedTerm.Term, nonMatchedTerm.Qualifier);
                }
            }
        }

        private void CompareTermAnnotationContent(IEdmVocabularyAnnotation expectedTermAnnotation, IEdmVocabularyAnnotation actualTermAnnotation)
        {
            // TODO: push Expression Equals() and ToString() to product?
            this.SatisfiesCondition(this.CompareIEdmExpression(expectedTermAnnotation.Value, actualTermAnnotation.Value), "Value expression mismatch at {0}.", this.scopeContext);
        }

        private bool CompareIEdmExpression(IEdmExpression expected, IEdmExpression actual)
        {
            if (expected.ExpressionKind != actual.ExpressionKind)
            {
                return false;
            }
            switch (expected.ExpressionKind)
            {
                case EdmExpressionKind.Null:
                    return expected.ExpressionKind == actual.ExpressionKind;
                case EdmExpressionKind.IntegerConstant:
                    return ((IEdmIntegerConstantExpression)expected).Value == ((IEdmIntegerConstantExpression)actual).Value;
                case EdmExpressionKind.StringConstant:
                    return ((IEdmStringConstantExpression)expected).Value == ((IEdmStringConstantExpression)actual).Value;
                case EdmExpressionKind.Record:
                    var expectedRecordExpression = expected as IEdmRecordExpression;
                    var actualRecordExpression = actual as IEdmRecordExpression;
                    if (expectedRecordExpression == null || actualRecordExpression == null)
                    {
                        return false;
                    }
                    if (expectedRecordExpression.Properties.Count() != actualRecordExpression.Properties.Count())
                    {
                        return false;
                    }
                    foreach (var property in expectedRecordExpression.Properties)
                    {
                        if (!actualRecordExpression.Properties.Distinct().Any(n => this.CompareIEdmExpression(property.Value, n.Value)))
                        {
                            return false;
                        }
                    }
                    return true;
                case EdmExpressionKind.Collection:
                    var expectedCollectionExpression = expected as IEdmCollectionExpression;
                    var actualCollectionExpression = actual as IEdmCollectionExpression;
                    if (expectedCollectionExpression == null || actualCollectionExpression == null)
                    {
                        return false;
                    }
                    if (expectedCollectionExpression.Elements.Count() != actualCollectionExpression.Elements.Count())
                    {
                        return false;
                    }
                    var elementCount = expectedCollectionExpression.Elements.Count();
                    for (int i = 0; i < elementCount; i++)
                    {
                        if(!this.CompareIEdmExpression(expectedCollectionExpression.Elements.ElementAt(i), actualCollectionExpression.Elements.ElementAt(i)))
                        {
                            return false;
                        }
                    }
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }

        private void CompareEntityType(IEdmEntityType expectedEntityType, IEdmEntityType actualEntityType)
        {
            this.SatisfiesEquals(expectedEntityType.FullName(), actualEntityType.FullName(), "EntityType name does not match.");
            this.SatisfiesEquals(expectedEntityType.IsAbstract, actualEntityType.IsAbstract, "IsAbstract does not match for EntityType '{0}'.", expectedEntityType.FullName());

            string expectedBaseTypeName = expectedEntityType.BaseType != null ? ((IEdmSchemaElement)expectedEntityType.BaseType).FullName() : null;
            string actualBaseTypeName = actualEntityType.BaseType != null ? ((IEdmSchemaElement)actualEntityType.BaseType).FullName() : null;

            this.SatisfiesEquals(expectedBaseTypeName, actualBaseTypeName, "BaseType does not match for EntityType '{0}'.", expectedEntityType.FullName());
            this.CompareProperties(expectedEntityType.StructuralProperties().Cast<IEdmProperty>(), actualEntityType.StructuralProperties().Cast<IEdmProperty>());
            this.CompareProperties(expectedEntityType.Key().OfType<IEdmProperty>(), actualEntityType.Key().OfType<IEdmProperty>());
            this.CompareNavigationProperty(expectedEntityType.Properties().OfType<IEdmNavigationProperty>(), actualEntityType.Properties().OfType<IEdmNavigationProperty>());

            this.CompareTermAnnotations(expectedEntityType, actualEntityType);
        }

        private void CompareProperties(IEnumerable<IEdmProperty> expectedProperties, IEnumerable<IEdmProperty> actualProperties)
        {
            // TODO: for open type, actual can have more properties. 
            // TODO: Need to compare annotations
            this.SatisfiesEquals(expectedProperties.Count(), actualProperties.Count(), "Count of Properties does not match.");

            foreach (IEdmProperty memberProperty in expectedProperties)
            {
                var actualMemberProperties = actualProperties.Where(p => p.Name == memberProperty.Name);
                if (this.SatisfiesEquals(1, actualMemberProperties.Count(), "Should find exactly one member '{0}'.", memberProperty.Name))
                {
                    var actualMemberProperty = actualMemberProperties.Single();
                    this.CompareMemberProperty(memberProperty, actualMemberProperty);
                }
            }
        }

        private void CompareNavigationProperty(IEnumerable<IEdmNavigationProperty> expectedNavigationProperties, IEnumerable<IEdmNavigationProperty> actualNavigationProperties)
        {
            // TODO: Need to compare annotations
            this.SatisfiesEquals(expectedNavigationProperties.Count(), actualNavigationProperties.Count(), "Count of NavigationProperties does not match.");

            foreach (IEdmNavigationProperty expectedNavigationProperty in expectedNavigationProperties)
            {
                var actualNavigationProperty = actualNavigationProperties.Single(p => p.Name == expectedNavigationProperty.Name);
                this.SatisfiesEquals(expectedNavigationProperty.Name, actualNavigationProperty.Name, "Navigation property name does not match.");

                // Minor bug? IsPrincipal is arbitrarily true for no FK?
                ////this.SatisfiesEquals(expectedNavigationProperty.IsPrincipal, actualNavigationProperty.IsPrincipal, "Navigation property IsPrincipal does not match.");

                // Minor bug? Parsed collection type reference is not nullable?
                ////this.SatisfiesEquals(expectedNavigationProperty.Type.ToTraceString(), actualNavigationProperty.Type.ToTraceString(), "Navigation property IsPrincipal does not match.");
            }
        }

        private void CompareMemberProperty(IEdmProperty expectedProperty, IEdmProperty actualProperty)
        {
            this.SatisfiesEquals(expectedProperty.Name, actualProperty.Name, "Property name does not match.");
            this.SatisfiesEquals(expectedProperty.Type.ToTraceString(), actualProperty.Type.ToTraceString(), "Property type does not match.");
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

            this.WriteErrorMessage(errorMessage, messageParameters);
            return false;
        }

        private void WriteErrorMessage(string errorMessage, params object[] messageParameters)
        {
            this.errorMessages.Add(string.Format(CultureInfo.InvariantCulture, errorMessage, messageParameters));
        }

        private class ScopeContext
        {
            private List<string> scopeList = new List<string>();

            public void Enter(string scope)
            {
                scopeList.Add(scope);
            }

            public void ExitCurrentScope()
            {
                scopeList.RemoveAt(scopeList.Count - 1);
            }

            public override string ToString()
            {
                return string.Join(".", scopeList.ToArray());
            }
        }
    }
}
