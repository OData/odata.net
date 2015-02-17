//---------------------------------------------------------------------
// <copyright file="EntityModelSchemaComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.EntityModel
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.EntityModel;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Way to compare two EntityModelSchemas together
    /// </summary>
    [ImplementationName(typeof(IEntityModelSchemaComparer), "Default")]
    public class EntityModelSchemaComparer : IEntityModelSchemaComparer
    {
        private List<string> errors;

        /// <summary>
        /// Compares the two models to each other and throws an exception when there is an erro
        /// </summary>
        /// <param name="expectedTestEntityModel">EntityModelSchema to compare y to</param>
        /// <param name="actualEntityModelSchema">EntityModelSchema to compare x to</param>
        /// <returns>List of errors</returns>
        public ICollection<string> Compare(EntityModelSchema expectedTestEntityModel, EntityModelSchema actualEntityModelSchema)
        {
            this.errors = new List<string>();
            foreach (ComplexType complexType in expectedTestEntityModel.ComplexTypes)
            {
                List<ComplexType> complexTypes = actualEntityModelSchema.ComplexTypes.Where(ct => ct.FullName == complexType.FullName).ToList();
                if (!this.WriteErrorIfFalse(complexTypes.Count == 1, "Cannot find complexType '{0}'", complexType.Name))
                {
                    ComplexType ycomplexType = complexTypes.Single();
                    this.CompareComplexType(complexType, ycomplexType);
                }
            }

            foreach (EntityType expectedEntityType in expectedTestEntityModel.EntityTypes)
            {
                List<EntityType> entityTypes = actualEntityModelSchema.EntityTypes.Where(et => et.FullName == expectedEntityType.FullName).ToList();
                if (!this.WriteErrorIfFalse(entityTypes.Count == 1, "Cannot find entityType '{0}'", expectedEntityType.Name))
                {
                    EntityType actualEntityType = entityTypes.Single();
                    this.CompareEntityTypes(expectedEntityType, actualEntityType);
                }
            }

            foreach (EntityContainer expectedEntityContainer in expectedTestEntityModel.EntityContainers)
            {
                List<EntityContainer> entityContainers = actualEntityModelSchema.EntityContainers.Where(ec => ec.Name == expectedEntityContainer.Name).ToList();
                if (!this.WriteErrorIfFalse(entityContainers.Count == 1, "Cannot find entityContainer '{0}'", expectedEntityContainer.Name))
                {
                    EntityContainer actualEntityContainer = entityContainers.Single();
                    this.CompareEntityContainer(expectedEntityContainer, actualEntityContainer);
                }
            }

            return this.errors;
        }

        /// <summary>
        /// Compares the actual function import against the expected.
        /// </summary>
        /// <param name="expectedFunctionImport">expected function import</param>
        /// <param name="actualFunctionImport">actual import</param>
        protected virtual void CompareFunctionImport(FunctionImport expectedFunctionImport, FunctionImport actualFunctionImport)
        {
            this.WriteErrorIfFalse(expectedFunctionImport.Name == actualFunctionImport.Name, "Wrong FunctionImport to compare.");
            
            // verify return type
            this.WriteErrorIfFalse(expectedFunctionImport.ReturnTypes.Count == actualFunctionImport.ReturnTypes.Count, "Wrong number of return type.");
            
            FunctionImportReturnType expectedReturnType = null;
            FunctionImportReturnType actualReturnType = null;
            if (expectedFunctionImport.ReturnTypes.Any())
            {
                expectedReturnType = expectedFunctionImport.ReturnTypes.Single();
                actualReturnType = actualFunctionImport.ReturnTypes.Single();
                this.WriteErrorIfFalse(expectedReturnType.DataType.GetType().Equals(actualReturnType.DataType.GetType()), "Expected Return Type {0} is not equal to actual Return Type {1}", expectedReturnType.DataType.GetType(), actualReturnType.DataType.GetType());
                if (expectedReturnType.EntitySet == null)
                {
                    this.WriteErrorIfFalse(actualReturnType.EntitySet == null, "Expected EntitySet {0} is not equal to actual EntitySet {1} {2}", expectedReturnType.EntitySet, actualReturnType.EntitySet, actualFunctionImport.Name);
                }
                else
                {
                    this.WriteErrorIfFalse(expectedReturnType.EntitySet.Name == actualReturnType.EntitySet.Name, "Expected EntitySet {0} is not equal to actual EntitySet {1} {2}", expectedReturnType.EntitySet, actualReturnType.EntitySet, actualFunctionImport.Name);
                }
            }

            // verify parameters
            ExceptionUtilities.Assert(expectedFunctionImport.Parameters.Count == actualFunctionImport.Parameters.Count, "Wrong number of parameters.");
            foreach (FunctionParameter expectedParameter in expectedFunctionImport.Parameters)
            {
                FunctionParameter actualParameter = actualFunctionImport.Parameters.Single(p => p.Name == expectedParameter.Name);
                this.WriteErrorIfFalse(expectedParameter.DataType.GetType().Equals(actualParameter.DataType.GetType()), "Wrong parameter type, Expected {0}, Actual {1}", expectedParameter.DataType.GetType(), actualParameter.DataType.GetType());
                this.WriteErrorIfFalse(expectedParameter.Mode == actualParameter.Mode, "Wrong parameter mode, Expected {0}, Actual {1}.", expectedParameter.Mode, actualParameter.Mode);
            }

            this.WriteErrorIfFalse(expectedFunctionImport.IsBindable == actualFunctionImport.IsBindable, "FunctionImport.IsBindable is not equal, Expected {0}, Actual: {1}", expectedFunctionImport.IsBindable, actualFunctionImport.IsBindable);
            this.WriteErrorIfFalse(expectedFunctionImport.IsComposable == actualFunctionImport.IsComposable, "FunctionImport.IsComposable is not equal, Expected {0}, Actual: {1}", expectedFunctionImport.IsComposable, actualFunctionImport.IsComposable);
            this.WriteErrorIfFalse(expectedFunctionImport.IsSideEffecting == actualFunctionImport.IsSideEffecting, "FunctionImport.IsSideEffecting is not equal, Expected {0}, Actual: {1}", expectedFunctionImport.IsSideEffecting, actualFunctionImport.IsSideEffecting);
        }

        /// <summary>
        /// Writes an error to a delay logged source
        /// </summary>
        /// <param name="assert">Assert condition</param>
        /// <param name="errorMessage">Error condition to record if assertion is true</param>
        /// <param name="messageParameters">Parameters to the message</param>
        /// <returns>True or false on if it failed or not</returns>
        protected bool WriteErrorIfFalse(bool assert, string errorMessage, params object[] messageParameters)
        {
            if (!assert)
            {
                this.WriteError(string.Format(CultureInfo.InvariantCulture, errorMessage, messageParameters));
                return true;
            }

            return false;
        }

        /// <summary>
        /// Compares the member properties.
        /// </summary>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="expectedProperties">The expected properties.</param>
        /// <param name="actualProperties">The actual properties.</param>
        protected virtual void CompareMemberProperties(NamedStructuralType expectedType, IEnumerable<MemberProperty> expectedProperties, IEnumerable<MemberProperty> actualProperties)
        {
            foreach (MemberProperty expectedProperty in expectedProperties)
            {
                List<MemberProperty> members = actualProperties.Where(p => p.Name == expectedProperty.Name).ToList();
                if (!this.WriteErrorIfFalse(members.Count == 1, "Cannot find member '{0}' on type '{1}'", expectedProperty.Name, expectedType.FullName))
                {
                    MemberProperty actualProperty = members.Single();
                    this.CompareMemberProperty(expectedProperty, actualProperty);
                }
            }
        }

        /// <summary>
        /// Writes an error to a delay logged source
        /// </summary>
        /// <param name="errorMessage">Error condition to record</param>
        /// <param name="messageParameters">Parameters to the message</param>
        private void WriteError(string errorMessage, params object[] messageParameters)
        {
            this.errors.Add(string.Format(CultureInfo.InvariantCulture, errorMessage, messageParameters));
        }

        private void CompareEntityContainer(EntityContainer expectedEntityContainer, EntityContainer actualEntityContainer)
        {
            foreach (EntitySet expectedEntitySet in expectedEntityContainer.EntitySets)
            {
                List<EntitySet> entitySets = actualEntityContainer.EntitySets.Where(es => es.Name == expectedEntitySet.Name).ToList();
                if (!this.WriteErrorIfFalse(entitySets.Count == 1, "Cannot find entitySet '{0}'", expectedEntitySet.Name))
                {
                    EntitySet yentitySet = entitySets.Single();
                    this.CompareEntitySet(expectedEntitySet, yentitySet);
                }
            }

            foreach (AssociationSet expectedAssociationSet in expectedEntityContainer.AssociationSets)
            {
                List<AssociationSet> associationSets = actualEntityContainer.AssociationSets.Where(asSet => asSet.Name == expectedAssociationSet.Name).ToList();
                if (!this.WriteErrorIfFalse(associationSets.Count == 1, "Cannot find associationSet '{0}'", expectedAssociationSet.Name))
                {
                    AssociationSet actualassociationSet = associationSets.Single();
                    this.CompareAssociationSet(expectedAssociationSet, actualassociationSet);
                }
            }

            foreach (FunctionImport expectedFunctionImport in expectedEntityContainer.FunctionImports)
            {
                List<FunctionImport> functionImports = actualEntityContainer.FunctionImports.Where(fi => fi.Name == expectedFunctionImport.Name && fi.Parameters.Count() == expectedFunctionImport.Parameters.Count()).ToList();
                if (!this.WriteErrorIfFalse(functionImports.Count == 1, "Cannot find FunctionImport '{0}'", expectedFunctionImport.Name))
                {
                    FunctionImport actualFunctionImport = functionImports.Single();
                    this.CompareFunctionImport(expectedFunctionImport, actualFunctionImport);
                }
            }
        }

        private void CompareAssociationSet(AssociationSet expectedAssociationSet, AssociationSet actualAssociationSet)
        {
            foreach (AssociationSetEnd expectedEnd in expectedAssociationSet.Ends)
            {
                List<AssociationSetEnd> associationSetEnds = actualAssociationSet.Ends.Where(asEnd => asEnd.AssociationEnd.RoleName == expectedEnd.AssociationEnd.RoleName).ToList();
                if (!this.WriteErrorIfFalse(associationSetEnds.Count == 1, "Cannot find associationSetEnd '{0}' ", expectedEnd.AssociationEnd.RoleName))
                {
                    this.CompareAssociationSetEnd(expectedEnd, associationSetEnds.Single());
                }
            }

            this.CompareAssociationType(expectedAssociationSet.AssociationType, actualAssociationSet.AssociationType);
        }

        private void CompareAssociationSetEnd(AssociationSetEnd expectedEnd, AssociationSetEnd actualEnd)
        {
            this.WriteErrorIfFalse(expectedEnd.EntitySet.Name == actualEnd.EntitySet.Name, "Expected EntitySet Name to be '{0}' actual '{1}' on an associationSetEnd", expectedEnd.EntitySet.Name, actualEnd.EntitySet.Name);
            this.CompareAssociationEnd(expectedEnd.AssociationEnd, actualEnd.AssociationEnd);          
        }

        private void CompareEntitySet(EntitySet expectedEntitySet, EntitySet actualEntitySet)
        {
            this.WriteErrorIfFalse(expectedEntitySet.EntityType.Name == actualEntitySet.EntityType.Name, "EntitySet EntityType property does not match Expected '{0}' Actual '{1}'", expectedEntitySet.EntityType, actualEntitySet.EntityType);
        }

        private void CompareEntityTypes(EntityType expectedEntityType, EntityType actualEntityType)
        {
            string actualBaseTypeName = null;
            string expectedBaseTypeName = null;

            if (expectedEntityType.BaseType != null)
            {
                expectedBaseTypeName = expectedEntityType.BaseType.Name;
            }

            if (actualEntityType.BaseType != null)
            {
                actualBaseTypeName = actualEntityType.BaseType.Name;
            }

            this.WriteErrorIfFalse(expectedBaseTypeName == actualBaseTypeName, "EntityType BaseType property does not match Expected '{0}' Actual '{1}'", expectedBaseTypeName, actualBaseTypeName);

            this.CompareMemberProperties(expectedEntityType, expectedEntityType.Properties, actualEntityType.Properties);

            foreach (NavigationProperty navigationProperty in expectedEntityType.NavigationProperties)
            {
                List<NavigationProperty> actualNavProps = actualEntityType.NavigationProperties.Where(p => p.Name == navigationProperty.Name).ToList();
                if (!this.WriteErrorIfFalse(actualNavProps.Count == 1, "Cannot find NavProp '{0}'", navigationProperty.Name))
                {
                    NavigationProperty actualNavigationProperty = actualNavProps.Single();
                    this.CompareNavigationProperty(navigationProperty, actualNavigationProperty);
                }
            }
        }

        private void CompareNavigationProperty(NavigationProperty expectedNavigationProperty, NavigationProperty actualNavigationProperty)
        {
            this.CompareAssociationType(expectedNavigationProperty.Association, actualNavigationProperty.Association);
            this.CompareAssociationEnd(expectedNavigationProperty.FromAssociationEnd, actualNavigationProperty.FromAssociationEnd);
            this.CompareAssociationEnd(expectedNavigationProperty.ToAssociationEnd, actualNavigationProperty.ToAssociationEnd);
        }

        private void CompareAssociationEnd(AssociationEnd expectedEnd, AssociationEnd actualEnd)
        {
            this.WriteErrorIfFalse(expectedEnd.Multiplicity == actualEnd.Multiplicity, "Expected Multiplicity to be '{0}' actual '{1}' on AssociationEnd {2}", expectedEnd.Multiplicity, actualEnd.Multiplicity, expectedEnd.RoleName);

            string expectedEntityTypeName = null;
            if (expectedEnd.EntityType != null)
            {
                expectedEntityTypeName = expectedEnd.EntityType.Name;
            }

            string actualEntityTypeName = null;
            if (actualEnd.EntityType != null)
            {
                actualEntityTypeName = actualEnd.EntityType.Name;
            }

            this.WriteErrorIfFalse(expectedEntityTypeName == actualEntityTypeName, "Expected EntityType Name to be '{0}' actual '{1}' on associationEnd {2}", expectedEntityTypeName, actualEntityTypeName, actualEnd.RoleName);
        }

        private void CompareAssociationType(AssociationType expectedAssociationType, AssociationType actualAssociationType)
        {
            this.WriteErrorIfFalse(expectedAssociationType.Name == actualAssociationType.Name, "Expected AssociationType.Name to be '{0}' actual '{1}'", expectedAssociationType.Name, actualAssociationType.Name);
            this.WriteErrorIfFalse(expectedAssociationType.NamespaceName == actualAssociationType.NamespaceName, "Expected AssociationType.NamespaceName to be '{0}' actual '{1}'", expectedAssociationType.NamespaceName, actualAssociationType.NamespaceName);
        }

        private void CompareMemberProperty(MemberProperty expectedProperty, MemberProperty actualProperty)
        {
            this.CompareMemberPropertyDatatype(expectedProperty.Name, expectedProperty.PropertyType, actualProperty.PropertyType);

            this.WriteErrorIfFalse(expectedProperty.IsPrimaryKey == actualProperty.IsPrimaryKey, "Expected '{0}' for IsPrimaryKey of property '{1}' but got '{2}'", expectedProperty.IsPrimaryKey, expectedProperty.Name, actualProperty.IsPrimaryKey);
            
            ConcurrencyTokenAnnotation concurrencyToken = expectedProperty.Annotations.OfType<ConcurrencyTokenAnnotation>().FirstOrDefault();
            if (concurrencyToken != null)
            {
                this.WriteErrorIfFalse(expectedProperty.Annotations.Any(), "Expected property '{0}' to have a concurrencyTokenAnnotation", expectedProperty.Name);
            }
        }

        private void CompareMemberPropertyDatatype(string memberName, DataType expectedDataType, DataType actualDataType)
        {
            PrimitiveDataType expectedPrimitiveDataType = expectedDataType as PrimitiveDataType;
            ComplexDataType expectedComplexDataType = expectedDataType as ComplexDataType;
            CollectionDataType expectedCollectionDataType = expectedDataType as CollectionDataType;
            SpatialDataType expectedSpatialDataType = expectedDataType as SpatialDataType;

            if (expectedPrimitiveDataType != null)
            {
                PrimitiveDataType actualPrimitiveDataType = actualDataType as PrimitiveDataType;
                this.WriteErrorIfFalse(actualPrimitiveDataType != null, "Expected member '{0}' with primitiveDataType '{1}' instead of '{2}'", memberName, expectedPrimitiveDataType, actualDataType);

                if (expectedSpatialDataType != null)
                {
                    SpatialDataType actualSpatialDataType = actualDataType as SpatialDataType;
                    this.CompareSpatialDataType(memberName, expectedSpatialDataType, actualSpatialDataType);
                }
            }
            else if (expectedComplexDataType != null)
            {
                ComplexDataType actualComplexDataType = actualDataType as ComplexDataType;
                if (!this.WriteErrorIfFalse(expectedComplexDataType != null, "Expected member '{0}' with complexDataType '{1}' instead of '{2}'", memberName, expectedComplexDataType, actualDataType))
                {
                    this.WriteErrorIfFalse(expectedComplexDataType.Definition.Name == actualComplexDataType.Definition.Name, "Expected member '{0}' with complexType Name '{1}' not '{2}'", memberName, expectedComplexDataType.Definition.Name, actualComplexDataType.Definition.Name);
                }
            }
            else
            {
                CollectionDataType actualCollectionDataType = actualDataType as CollectionDataType;
                if (!this.WriteErrorIfFalse(expectedCollectionDataType != null, "Expected member '{0}' with collectionType '{1}' instead of '{2}'", memberName, expectedCollectionDataType, actualDataType))
                {
                    this.CompareMemberPropertyDatatype(memberName, expectedCollectionDataType.ElementDataType, actualCollectionDataType.ElementDataType);
                }
            }

            // Complex properties need not be handled specially as they can also have IsNullable = true. The scenario for complex property is given below:
            // For reflection and custom: If the DSV <= 2 then it should always contain Nullable=’false’. For DSV >= 3, it should always contain Nullable=’true’
            // For EF: For all DSV values it should always contain Nullable=’false’ since complex types are always non-nullable in EF.
            this.WriteErrorIfFalse(expectedDataType.IsNullable == actualDataType.IsNullable, "Expected member '{0}' to have an IsNullable of '{1}' instead of '{2}'", memberName, expectedDataType.IsNullable, actualDataType.IsNullable);
        }

        private void CompareComplexType(ComplexType expectedComplexType, ComplexType actualComplexType)
        {
            foreach (MemberProperty memberProperty in expectedComplexType.Properties)
            {
                List<MemberProperty> members = actualComplexType.Properties.Where(p => p.Name == memberProperty.Name).ToList();
                if (!this.WriteErrorIfFalse(members.Count == 1, "Cannot find member '{0}'", memberProperty.Name))
                {
                    MemberProperty ymemberProperty = members.Single();
                    this.CompareMemberProperty(memberProperty, ymemberProperty);
                }
            }
        }

        private void CompareSpatialDataType(string memberName, SpatialDataType expectedSpatialDataType, SpatialDataType actualSpatialDataType)
        {
            SridFacet expectedSridFacet = expectedSpatialDataType.Facets.OfType<SridFacet>().SingleOrDefault();
            SridFacet actualSridFacet = actualSpatialDataType.Facets.OfType<SridFacet>().SingleOrDefault();

            if (expectedSridFacet == null)
            {
                this.WriteErrorIfFalse(actualSridFacet == null, "Expected no SRID facet on member '{0}' but got '{1}'", memberName, actualSridFacet);
            }
            else if (actualSridFacet == null)
            {
                this.WriteError("Expected SRID '{0}' on member '{1}' but got none", expectedSridFacet, memberName);
            }
            else
            {
                this.WriteErrorIfFalse(expectedSridFacet.Value == actualSridFacet.Value, "Expected SRID '{0}' on member '{1}' but got '{2}'", expectedSridFacet, memberName, actualSridFacet);
            }
        }
    }
}
