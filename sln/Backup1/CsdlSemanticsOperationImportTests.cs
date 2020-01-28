//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationImportTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Semantics
{
    public class CsdlSemanticsOperationImportTests
    {
        private readonly CsdlLocation testLocation;

        public CsdlSemanticsOperationImportTests()
        {
            this.testLocation = new CsdlLocation(1, 3);
        }

        [Fact]
        public void EnsureEntitySetResolvesToEdmPathExpression()
        {
            var action = CsdlBuilder.Action("Checkout");
            var actionImport = new CsdlActionImport("Checkout", "FQ.NS.Checkout", "Nav1/Nav2" /*entitySet*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container");

            var semanticSchema = CreateCsdlSemanticsSchema(csdlEntityContainer, action);
            var semanticAction = new CsdlSemanticsAction(semanticSchema, action);

            var csdlSemanticEntityContainer = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var semanticActionImport = new CsdlSemanticsActionImport(csdlSemanticEntityContainer, actionImport, semanticAction);
            Assert.NotNull(semanticActionImport.Action);
            Assert.Equal("Checkout", semanticActionImport.Action.Name);
            var pathExpression = (IEdmPathExpression)semanticActionImport.EntitySet;
            var items = pathExpression.PathSegments.ToList();
            Assert.Equal("Nav1", items[0]);
            Assert.Equal("Nav2", items[1]);
        }

        [Fact]
        public void CsdlSemanticsActionImportPropertiesShouldBeInitializedCorrectly()
        {
            var action = CsdlBuilder.Action("Checkout");
            var actionImport = new CsdlActionImport("Checkout", "FQ.NS.Checkout", null, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container");

            var semanticSchema = CreateCsdlSemanticsSchema(csdlEntityContainer, action);
            var semanticAction = new CsdlSemanticsAction(semanticSchema, action);

            var csdlSemanticEntityContainer = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var semanticActionImport = new CsdlSemanticsActionImport(csdlSemanticEntityContainer, actionImport, semanticAction);
            Assert.NotNull(semanticActionImport.Action);
            Assert.Equal("Checkout", semanticActionImport.Action.Name);
            Assert.Equal("Container", semanticActionImport.Container.Name);
            Assert.Equal(testLocation, semanticActionImport.Location());
            Assert.Equal(EdmContainerElementKind.ActionImport, semanticActionImport.ContainerElementKind);
            Assert.Null(semanticActionImport.EntitySet);
        }

        [Fact]
        public void CsdlSemanticsFunctionImportPropertiesShouldBeInitializedCorrectly()
        {
            // Added to ensure this is filtered out
            var function = CsdlBuilder.Function("GetStuff");
            
            var functionImport = new CsdlFunctionImport("GetStuff", "FQ.NS.GetStuff", null /*entitySet*/, true /*includeInServiceDocument*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container");

            var semanticSchema = CreateCsdlSemanticsSchema(csdlEntityContainer, function);
            var semanticFunction = new CsdlSemanticsFunction(semanticSchema, function);

            var csdlSemanticEntityContainer = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var semanticActionImport = new CsdlSemanticsFunctionImport(csdlSemanticEntityContainer, functionImport, semanticFunction);
            Assert.NotNull(semanticActionImport.Function);
            Assert.Equal("GetStuff", semanticActionImport.Function.Name);
            Assert.Equal("Container", semanticActionImport.Container.Name);
            Assert.Equal(testLocation, semanticActionImport.Location());
            Assert.Equal(EdmContainerElementKind.FunctionImport, semanticActionImport.ContainerElementKind);
            Assert.Null(semanticActionImport.EntitySet);
            Assert.True(semanticActionImport.IncludeInServiceDocument);
        }

        private static CsdlSemanticsSchema CreateCsdlSemanticsSchema(CsdlEntityContainer csdlEntityContainer, params CsdlOperation[] operations)
        {
            var csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, new Collection<CsdlProperty>(), new System.Collections.Generic.List<CsdlNavigationProperty>(), null);
            var schema = CsdlBuilder.Schema("FQ.NS", csdlOperations: operations, csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer }, csdlStructuredTypes: new CsdlStructuredType[] { csdlEntityType });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);
            var semanticSchema = new CsdlSemanticsSchema(new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>()), schema);
            return semanticSchema;
        }
    }
}
