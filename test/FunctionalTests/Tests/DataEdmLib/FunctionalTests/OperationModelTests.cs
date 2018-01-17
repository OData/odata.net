//---------------------------------------------------------------------
// <copyright file="OperationModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
#if !SILVERLIGHT
    using ApprovalTests.Reporters;
#endif
    using System.Runtime.CompilerServices;
    using EdmLibTests.FunctionalUtilities;
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Common;
    using Microsoft.Test.OData.Utils.Metadata;
#if SILVERLIGHT
    using Microsoft.Silverlight.Testing;
#endif
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
#if !SILVERLIGHT
    [UseReporter(typeof(LoggingReporter))]
    [DeploymentItem("FunctionalTests")]
#endif
    public class OperationModelTests : EdmLibTestCaseBase
    {
        public OperationModelTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void OperationStandaloneTestRoundtripVerify()
        {
            var model = OperationTestModelBuilder.OperationStandAloneSchemasEdm();
            OperationTestModelBuilder.AddOperationImports(model as EdmModel);
            this.BasicRoundtripTest(model);
        }

        [TestMethod]
        public void OperationStandaloneTestFindMethodsVerify()
        {
            var model = OperationTestModelBuilder.OperationStandAloneSchemasEdm();
            OperationTestModelBuilder.AddOperationImports(model as EdmModel);
            this.BasicFindMethodsTest(model);
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void OperationsWithReturnTypeOfPrimitiveDataTypeTestRoundtripVerify()
        {
            var model = OperationTestModelBuilder.OperationsWithReturnTypeOfPrimitiveDataTypeSchemasEdm(this.EdmVersion);
            OperationTestModelBuilder.AddOperationImports(model as EdmModel);
            this.BasicRoundtripTest(model);
        }

        [TestMethod]
        public void OperationsWithReturnTypeOfPrimitiveDataTestFindMethodsVerify()
        {
            var model = OperationTestModelBuilder.OperationsWithReturnTypeOfPrimitiveDataTypeSchemasEdm(this.EdmVersion);
            OperationTestModelBuilder.AddOperationImports(model as EdmModel);
            this.BasicFindMethodsTest(model);
        }

        [TestMethod]
        [MethodImplAttribute(MethodImplOptions.NoOptimization)]
        public void OperationsWith2ParametersTestRoundtripVerify()
        {
            var model = OperationTestModelBuilder.OperationsWith2ParametersSchemasEdm(this.EdmVersion);
            OperationTestModelBuilder.AddOperationImports(model as EdmModel);
            this.BasicRoundtripTest(model);
        }

        [TestMethod]
        public void OperationsWith2ParametersTestFindMethodsVerify()
        {
            var model = OperationTestModelBuilder.OperationsWith2ParametersSchemasEdm(this.EdmVersion);
            OperationTestModelBuilder.AddOperationImports(model as EdmModel);
            this.BasicFindMethodsTest(model);
        }

        [TestMethod]
        [MethodImpl(MethodImplOptions.NoOptimization)]
        public void OperationsWithNamedStructuralDataTypeTestRoundtripVerify()
        {
            var testModel = OperationTestModelBuilder.OperationsWithNamedStructuralDataTypeSchemasEdm();
            OperationTestModelBuilder.AddOperationImports(testModel as EdmModel);
            this.BasicRoundtripTest(testModel);
        }

        [TestMethod]
        public void OperationsWithNamedStructuralDataTypeTestFindMethodsVerify()
        {
            var testModel = OperationTestModelBuilder.OperationsWithNamedStructuralDataTypeSchemasEdm();
            OperationTestModelBuilder.AddOperationImports(testModel as EdmModel);
            this.BasicFindMethodsTest(testModel);
        }
    }
}
