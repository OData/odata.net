//---------------------------------------------------------------------
// <copyright file="OperationModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class OperationModelTests : EdmLibTestCaseBase
{
    public OperationModelTests()
    {
        this.EdmVersion = EdmVersion.V40;
    }

    [Fact]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void OperationStandaloneTestRoundtripVerify()
    {
        var model = OperationTestModelBuilder.OperationStandAloneSchemasEdm();
        OperationTestModelBuilder.AddOperationImports(model as EdmModel);
        this.BasicRoundtripTest(model);
    }

    [Fact]
    public void OperationStandaloneTestFindMethodsVerify()
    {
        var model = OperationTestModelBuilder.OperationStandAloneSchemasEdm();
        OperationTestModelBuilder.AddOperationImports(model as EdmModel);
        this.BasicFindMethodsTest(model);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void OperationsWithReturnTypeOfPrimitiveDataTypeTestRoundtripVerify()
    {
        var model = OperationTestModelBuilder.OperationsWithReturnTypeOfPrimitiveDataTypeSchemasEdm(this.EdmVersion);
        OperationTestModelBuilder.AddOperationImports(model as EdmModel);
        this.BasicRoundtripTest(model);
    }

    [Fact]
    public void OperationsWithReturnTypeOfPrimitiveDataTestFindMethodsVerify()
    {
        var model = OperationTestModelBuilder.OperationsWithReturnTypeOfPrimitiveDataTypeSchemasEdm(this.EdmVersion);
        OperationTestModelBuilder.AddOperationImports(model as EdmModel);
        this.BasicFindMethodsTest(model);
    }

    [Fact]
    [MethodImplAttribute(MethodImplOptions.NoOptimization)]
    public void OperationsWith2ParametersTestRoundtripVerify()
    {
        var model = OperationTestModelBuilder.OperationsWith2ParametersSchemasEdm(this.EdmVersion);
        OperationTestModelBuilder.AddOperationImports(model as EdmModel);
        this.BasicRoundtripTest(model);
    }

    [Fact]
    public void OperationsWith2ParametersTestFindMethodsVerify()
    {
        var model = OperationTestModelBuilder.OperationsWith2ParametersSchemasEdm(this.EdmVersion);
        OperationTestModelBuilder.AddOperationImports(model as EdmModel);
        this.BasicFindMethodsTest(model);
    }

    [Fact]
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public void OperationsWithNamedStructuralDataTypeTestRoundtripVerify()
    {
        var testModel = OperationTestModelBuilder.OperationsWithNamedStructuralDataTypeSchemasEdm();
        OperationTestModelBuilder.AddOperationImports(testModel as EdmModel);
        this.BasicRoundtripTest(testModel);
    }

    [Fact]
    public void OperationsWithNamedStructuralDataTypeTestFindMethodsVerify()
    {
        var testModel = OperationTestModelBuilder.OperationsWithNamedStructuralDataTypeSchemasEdm();
        OperationTestModelBuilder.AddOperationImports(testModel as EdmModel);
        this.BasicFindMethodsTest(testModel);
    }
}
