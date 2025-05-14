//---------------------------------------------------------------------
// <copyright file="RoundTripTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class RoundTripTests : EdmLibTestCaseBase
{
    [Fact]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void RoundTripOneEntityWithAllPrimitiveProperty()
    {
        this.BasicRoundtripTest(ModelBuilder.OneEntityWithAllPrimitivePropertyEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripOneEntityWithAllSpatialProperty()
    {
        this.BasicRoundtripTest(ModelBuilder.OneEntityWithAllSpatialPropertyEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripOneComplexWithOneProperty()
    {
        this.BasicRoundtripTest(ModelBuilder.OneComplexWithOnePropertyEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripOneComplexWithNestedComplex()
    {
        this.BasicRoundtripTest(ModelBuilder.OneComplexWithNestedComplexEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripMultipleNamespaces()
    {
        this.BasicRoundtripTest(ModelBuilder.MultipleNamespacesEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripEntityInheritanceTree()
    {
        this.BasicRoundtripTest(ModelBuilder.EntityInheritanceTreeEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripEntityContainerWithEntitySets()
    {
        this.BasicRoundtripTest(ModelBuilder.EntityContainerWithEntitySetsEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripAssociationIndependent()
    {
        this.BasicRoundtripTest(ModelBuilder.AssociationIndependentEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripAssociationFk()
    {
        this.BasicRoundtripTest(ModelBuilder.AssociationFkEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripAssociationFkWithNavigation()
    {
        this.BasicRoundtripTest(ModelBuilder.AssociationFkWithNavigationEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripAssociationCompositeFk()
    {
        this.BasicRoundtripTest(ModelBuilder.AssociationCompositeFkEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripEntityContainerWithOperationImports()
    {
        this.BasicRoundtripTest(ModelBuilder.EntityContainerWithOperationImportsEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripFunctionWithAll()
    {
        this.BasicRoundtripTest(ModelBuilder.FunctionWithAllEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripModelWithAllConcepts()
    {
        this.BasicRoundtripTest(ModelBuilder.ModelWithAllConceptsEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripModelWithDefaultEnum()
    {
        this.BasicRoundtripTest(ModelBuilder.ModelWithDefaultEnumEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripAssociationOnDeleteModel()
    {
        this.BasicRoundtripTest(ModelBuilder.AssociationOnDeleteModelEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTaupoDefaultModel()
    {
        this.BasicRoundtripTest(ModelBuilder.TaupoDefaultModelEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripOneComplexWithAllPrimitiveProperty()
    {
        this.BasicRoundtripTest(ModelBuilder.OneComplexWithAllPrimitivePropertyEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripOneComplexWithAllSpatialProperty()
    {
        this.BasicRoundtripTest(ModelBuilder.OneComplexWithAllSpatialPropertyEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripValidNameCheck()
    {
        // The weird characters in this model cause issue with Approvals.
        this.BasicRoundtripTest(ModelBuilder.ValidNameCheckModelEdm(), skipBaseline: true);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripPropertyFacetsCollection()
    {
        this.BasicRoundtripTest(ModelBuilder.PropertyFacetsCollectionEdm());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripComplexTypeAttributes()
    {
        this.BasicRoundtripTest(ModelBuilder.ComplexTypeAttributesEdm());
    }

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripMultipleSchemasWithDifferentNamespacesCyclicInvalid()
    {
        this.BasicRoundtripTest(FindMethodsTestModelBuilder.MultipleSchemasWithDifferentNamespacesCyclicInvalid());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripMultipleSchemasWithSameNamespace()
    {
        this.BasicRoundtripTest(FindMethodsTestModelBuilder.MultipleSchemasWithSameNamespace(this.EdmVersion));
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelTestModel()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelBasicTest);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelAnnotationTestWithAnnotations()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelAnnotationTestWithAnnotations);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelAnnotationTestWithoutAnnotations()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelAnnotationTestWithoutAnnotations);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelWithOperationImport()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithFunctionImport);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataDefaultTestModel()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelDefaultModel);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelEmptyModel()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelEmptyModel);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelWithSingleEntityType()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithSingleEntityType);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelWithSingleComplexType()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithSingleComplexType);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelWithCollectionProperty()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithCollectionProperty);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelWithOpenType()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithOpenType);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTestBuildODataTestModelWithNamedStream()
    {
        this.BasicRoundtripTest(ODataTestModelBuilder.ODataTestModelWithNamedStream);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripCollectionTypesWithSimpleType()
    {
        this.BasicRoundtripTest(ModelBuilder.CollectionTypesWithSimpleType());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripCollectionTypes()
    {
        this.BasicRoundtripTest(ModelBuilder.CollectionTypes());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripTypeRefFacets()
    {
        this.BasicRoundtripTest(ModelBuilder.TypeRefFacets());
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void RoundTripEntityContainerAttributes()
    {
        this.BasicRoundtripTest(ModelBuilder.EntityContainerAttributes());
    }
}
