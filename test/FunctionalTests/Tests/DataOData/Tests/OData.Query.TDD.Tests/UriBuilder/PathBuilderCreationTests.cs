//---------------------------------------------------------------------
// <copyright file="PathBuilderCreationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Query.TDD.Tests.UriBuilder
{
    using System;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.OData.Core.UriBuilder;
    using Microsoft.OData.Core.UriParser;
    using Microsoft.Spatial;
    using Microsoft.Test.OData.Query.TDD.Tests.TestUtilities;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Expressions;
    using Microsoft.OData.Core.UriParser.Semantic;

    [TestClass]
    public class PathBuilderCreationTests : UriBuilderTestBase
    {
        [TestMethod]
        public void BuildPathWithFunctionImport()
        {
            ODataUri odataUri = new ODataUri();
            odataUri.ServiceRoot = new Uri("http://gobbledygook/");
            IEdmOperationImport functionImport = HardCodedTestModel.TestModel.EntityContainer.FindOperationImports("GetPet1").Single();
            IEdmEntitySetReferenceExpression reference = functionImport.EntitySet as IEdmEntitySetReferenceExpression;
            OperationSegmentParameter[] parameters = new OperationSegmentParameter[] { new OperationSegmentParameter("id", new ConstantNode(1, "1")) };
            odataUri.Path = new ODataPath(new OperationImportSegment(
                new IEdmOperationImport[] { functionImport },
                reference.ReferencedEntitySet,
                parameters));
            ODataUriBuilder odataUriBuilder = new ODataUriBuilder(ODataUrlConventions.Default, odataUri);
            Uri actual = odataUriBuilder.BuildUri();
            Assert.AreEqual(new Uri("http://gobbledygook/GetPet1(id=1)"), actual);
        }
    }
}
