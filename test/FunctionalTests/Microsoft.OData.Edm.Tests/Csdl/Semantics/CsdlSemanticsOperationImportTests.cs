//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationImportTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
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
            var actionImport = new CsdlActionImport("Checkout", "FQ.NS.Checkout", "Nav1/Nav2" /*entitySet*/, null /*documentation*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container");

            var semanticSchema = CreateCsdlSemanticsSchema(csdlEntityContainer, action);
            var semanticAction = new CsdlSemanticsAction(semanticSchema, action);

            var csdlSemanticEntityContainer = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var semanticActionImport = new CsdlSemanticsActionImport(csdlSemanticEntityContainer, actionImport, semanticAction);
            semanticActionImport.Action.Should().NotBeNull();
            semanticActionImport.Action.Name.Should().Be("Checkout");
            var pathExpression = (IEdmPathExpression)semanticActionImport.EntitySet;
            var items = pathExpression.PathSegments.ToList();
            items[0].Should().Be("Nav1");
            items[1].Should().Be("Nav2");
        }

        [Fact]
        public void CsdlSemanticsActionImportPropertiesShouldBeInitializedCorrectly()
        {
            var action = CsdlBuilder.Action("Checkout");
            var actionImport = new CsdlActionImport("Checkout", "FQ.NS.Checkout", null, null /*documentation*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container");

            var semanticSchema = CreateCsdlSemanticsSchema(csdlEntityContainer, action);
            var semanticAction = new CsdlSemanticsAction(semanticSchema, action);

            var csdlSemanticEntityContainer = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var semanticActionImport = new CsdlSemanticsActionImport(csdlSemanticEntityContainer, actionImport, semanticAction);
            semanticActionImport.Action.Should().NotBeNull();
            semanticActionImport.Action.Name.Should().Be("Checkout");
            semanticActionImport.Container.Name.Should().Be("Container");
            semanticActionImport.Location().Should().Be(testLocation);
            semanticActionImport.ContainerElementKind.Should().Be(EdmContainerElementKind.ActionImport);
            semanticActionImport.EntitySet.Should().BeNull();
        }

        [Fact]
        public void CsdlSemanticsFunctionImportPropertiesShouldBeInitializedCorrectly()
        {
            // Added to ensure this is filtered out
            var function = CsdlBuilder.Function("GetStuff");
            
            var functionImport = new CsdlFunctionImport("GetStuff", "FQ.NS.GetStuff", null /*entitySet*/, true /*includeInServiceDocument*/, null /*documentation*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container");

            var semanticSchema = CreateCsdlSemanticsSchema(csdlEntityContainer, function);
            var semanticFunction = new CsdlSemanticsFunction(semanticSchema, function);

            var csdlSemanticEntityContainer = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var semanticActionImport = new CsdlSemanticsFunctionImport(csdlSemanticEntityContainer, functionImport, semanticFunction);
            semanticActionImport.Function.Should().NotBeNull();
            semanticActionImport.Function.Name.Should().Be("GetStuff");
            semanticActionImport.Container.Name.Should().Be("Container");
            semanticActionImport.Location().Should().Be(testLocation);
            semanticActionImport.ContainerElementKind.Should().Be(EdmContainerElementKind.FunctionImport);
            semanticActionImport.EntitySet.Should().BeNull();
            semanticActionImport.IncludeInServiceDocument.Should().BeTrue();
        }

        private static CsdlSemanticsSchema CreateCsdlSemanticsSchema(CsdlEntityContainer csdlEntityContainer, params CsdlOperation[] operations)
        {
            var csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, new Collection<CsdlProperty>(), new System.Collections.Generic.List<CsdlNavigationProperty>(), null, null);
            var schema = CsdlBuilder.Schema("FQ.NS", csdlOperations: operations, csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer }, csdlStructuredTypes: new CsdlStructuredType[] { csdlEntityType });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);
            var semanticSchema = new CsdlSemanticsSchema(new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>()), schema);
            return semanticSchema;
        }
    }
}
