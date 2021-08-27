//---------------------------------------------------------------------
// <copyright file="RoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using System.Linq;
    using ApprovalTests.Reporters;
    using System.Runtime.CompilerServices;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [UseReporter(typeof(LoggingReporter))]
    [DeploymentItem("FunctionalTests")]
    public class RoundTripTests : EdmLibTestCaseBase
    {
        [TestMethod]
		[MethodImpl(MethodImplOptions.NoOptimization)]
        public void RoundTripOneEntityWithAllPrimitiveProperty()
        {
            this.BasicRoundtripTest(ModelBuilder.OneEntityWithAllPrimitivePropertyEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripOneEntityWithAllSpatialProperty()
        {
            this.BasicRoundtripTest(ModelBuilder.OneEntityWithAllSpatialPropertyEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripOneComplexWithOneProperty()
        {
            this.BasicRoundtripTest(ModelBuilder.OneComplexWithOnePropertyEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripOneComplexWithNestedComplex()
        {
            this.BasicRoundtripTest(ModelBuilder.OneComplexWithNestedComplexEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripMultipleNamespaces()
        {
            this.BasicRoundtripTest(ModelBuilder.MultipleNamespacesEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEntityInheritanceTree()
        {
            this.BasicRoundtripTest(ModelBuilder.EntityInheritanceTreeEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEntityContainerWithEntitySets()
        {
            this.BasicRoundtripTest(ModelBuilder.EntityContainerWithEntitySetsEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripAssociationIndependent()
        {
            this.BasicRoundtripTest(ModelBuilder.AssociationIndependentEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripAssociationFk()
        {
            this.BasicRoundtripTest(ModelBuilder.AssociationFkEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripAssociationFkWithNavigation()
        {
            this.BasicRoundtripTest(ModelBuilder.AssociationFkWithNavigationEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripAssociationCompositeFk()
        {
            this.BasicRoundtripTest(ModelBuilder.AssociationCompositeFkEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEntityContainerWithOperationImports()
        {
            this.BasicRoundtripTest(ModelBuilder.EntityContainerWithOperationImportsEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripFunctionWithAll()
        {
            this.BasicRoundtripTest(ModelBuilder.FunctionWithAllEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripModelWithAllConcepts()
        {
            this.BasicRoundtripTest(ModelBuilder.ModelWithAllConceptsEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripModelWithDefaultEnum()
        {
            this.BasicRoundtripTest(ModelBuilder.ModelWithDefaultEnumEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripAssociationOnDeleteModel()
        {
            this.BasicRoundtripTest(ModelBuilder.AssociationOnDeleteModelEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTaupoDefaultModel()
        {
            this.BasicRoundtripTest(ModelBuilder.TaupoDefaultModelEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripOneComplexWithAllPrimitiveProperty()
        {
            this.BasicRoundtripTest(ModelBuilder.OneComplexWithAllPrimitivePropertyEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripOneComplexWithAllSpatialProperty()
        {
            this.BasicRoundtripTest(ModelBuilder.OneComplexWithAllSpatialPropertyEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripValidNameCheck()
        {
            // The weird characters in this model cause issue with Approvals.
            this.BasicRoundtripTest(ModelBuilder.ValidNameCheckModelEdm(), skipBaseline:true);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripPropertyFacetsCollection()
        {
            this.BasicRoundtripTest(ModelBuilder.PropertyFacetsCollectionEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripComplexTypeAttributes()
        {
            this.BasicRoundtripTest(ModelBuilder.ComplexTypeAttributesEdm());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEnumsWithFlagsandUnderlyingInt64()
        {
            var directionType = new EdmEnumType("NS1", "Direction", EdmPrimitiveTypeKind.Int64, isFlags: true);
            directionType.AddMember("East", new EdmEnumMemberValue(0L));
            directionType.AddMember("West", new EdmEnumMemberValue(1L));
            directionType.AddMember("South", new EdmEnumMemberValue(2L));
            directionType.AddMember("North", new EdmEnumMemberValue(3L));

            this.BasicRoundtripTest(ModelBuilder.ModelWithEnumEdm(directionType));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEnumsWithValues()
        {
            var directionType = new EdmEnumType("NS1", "Direction");
            directionType.AddMember("East", new EdmEnumMemberValue(100L));
            directionType.AddMember("West", new EdmEnumMemberValue(1L));
            directionType.AddMember("South", new EdmEnumMemberValue(-2L));
            directionType.AddMember("North", new EdmEnumMemberValue(3L));

            this.BasicRoundtripTest(ModelBuilder.ModelWithEnumEdm(directionType));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEnumsWithUnderlyingValuesOtherThanInt32()
        {
            var underlyingTypes = new[]
            {
                EdmPrimitiveTypeKind.Byte, 
                EdmPrimitiveTypeKind.SByte, 
                EdmPrimitiveTypeKind.Int16,
                EdmPrimitiveTypeKind.Int64,
            };

            int counter = 0;
            var enumTypes = underlyingTypes.Select(t =>
            {
                var enumType = new EdmEnumType("NS1", "Direction" + counter++, t, false);
                enumType.AddMember("East", new EdmEnumMemberValue(0L));
                return (IEdmEnumType)enumType;
            });

            this.BasicRoundtripTest(ModelBuilder.ModelWithEnumEdm(enumTypes.ToArray()));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripMultipleSchemasWithDifferentNamespacesCyclicInvalid()
        {
            this.BasicRoundtripTest(FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripMultipleSchemasWithSameNamespace()
        {
            this.BasicRoundtripTest(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace(this.EdmVersion));
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelTestModel()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelBasicTest);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelAnnotationTestWithAnnotations()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelAnnotationTestWithAnnotations);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelAnnotationTestWithoutAnnotations()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelAnnotationTestWithoutAnnotations);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelWithOperationImport()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithFunctionImport);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataDefaultTestModel()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelDefaultModel);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelEmptyModel()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelEmptyModel);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelWithSingleEntityType()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithSingleEntityType);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelWithSingleComplexType()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithSingleComplexType);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelWithCollectionProperty()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithCollectionProperty);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelWithOpenType()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithOpenType);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTestBuildODataTestModelWithNamedStream()
        {
            this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithNamedStream);
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripCollectionTypesWithSimpleType()
        {
            this.BasicRoundtripTest(ModelBuilder.CollectionTypesWithSimpleType());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripCollectionTypes()
        {
            this.BasicRoundtripTest(ModelBuilder.CollectionTypes());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripTypeRefFacets()
        {
            this.BasicRoundtripTest(ModelBuilder.TypeRefFacets());
        }

        [TestMethod]
		[MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void RoundTripEntityContainerAttributes()
        {
            this.BasicRoundtripTest(ModelBuilder.EntityContainerAttributes());
        }
    }
}
