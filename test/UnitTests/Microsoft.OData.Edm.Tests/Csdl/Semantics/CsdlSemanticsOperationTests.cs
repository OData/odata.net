//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl.Semantics
{
    /// <summary>
    /// The logic for Actions and Functions Semantic classes is almost all (minus iscomposable) in the Operation class. Hence testing these classes here.
    /// </summary>
    public class CsdlSemanticsOperationTests
    {
        private readonly CsdlSemanticsSchema semanticSchema;
        private readonly CsdlEntityType csdlEntityType;
        private readonly CsdlLocation testLocation;
        public CsdlSemanticsOperationTests()
        {
            this.csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), Enumerable.Empty<CsdlNavigationProperty>(), null);

            var csdlSchema = CsdlBuilder.Schema("FQ.NS", csdlStructuredTypes: new[] { this.csdlEntityType });
            
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(csdlSchema);
            var semanticModel = new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>());
            this.semanticSchema = new CsdlSemanticsSchema(semanticModel, csdlSchema);
            this.testLocation = new CsdlLocation(1, 3);
        }

        [Fact]
        public void NonBoundCsdlSemanticsOperationPropertiesShouldBeCorrect()
        {
            var function = new CsdlFunction(
                "Checkout",
                Enumerable.Empty<CsdlOperationParameter>(),
                new CsdlOperationReturn(new CsdlNamedTypeReference("Edm.String", false, testLocation), testLocation),
                false /*isBound*/,
                null /*entitySetPath*/,
                true /*isComposable*/,
                testLocation);

            var semanticFunction = new CsdlSemanticsFunction(this.semanticSchema, function);
            Assert.False(semanticFunction.IsBound);
            Assert.Equal(testLocation, semanticFunction.Location());
            Assert.Equal("Checkout", semanticFunction.Name);
            Assert.Equal("FQ.NS", semanticFunction.Namespace);
            Assert.Equal("Edm.String", semanticFunction.ReturnType.Definition.FullTypeName());
            Assert.Null(semanticFunction.EntitySetPath);
            Assert.Equal(EdmSchemaElementKind.Function, semanticFunction.SchemaElementKind);
            Assert.True(semanticFunction.IsComposable);
        }

        [Fact]
        public void BoundCsdlSemanticsOperationPropertiesShouldBeCorrect()
        {
            var action = new CsdlAction(
                "Checkout",
                new CsdlOperationParameter[] { new CsdlOperationParameter("entity", new CsdlNamedTypeReference("FQ.NS.EntityType", false, testLocation), testLocation) }, 
                new CsdlOperationReturn(new CsdlNamedTypeReference("Edm.String", false, testLocation), testLocation),
                true /*isBound*/,
                "entity/FakePath",
                testLocation);

            var semanticAction = new CsdlSemanticsAction(this.semanticSchema, action);
            Assert.True(semanticAction.IsBound);
            Assert.Equal(testLocation, semanticAction.Location());
            Assert.Equal("Checkout", semanticAction.Name);
            Assert.Equal("FQ.NS", semanticAction.Namespace);
            Assert.Equal("Edm.String", semanticAction.ReturnType.Definition.FullTypeName());
            Assert.Equal("entity", semanticAction.EntitySetPath.PathSegments.ToList()[0]);
            Assert.Equal("FakePath", semanticAction.EntitySetPath.PathSegments.ToList()[1]);
            Assert.Equal(EdmSchemaElementKind.Action, semanticAction.SchemaElementKind);
        }
    }
}
