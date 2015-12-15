//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsOperationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Csdl.CsdlSemantics;
using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Annotations;
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
            this.csdlEntityType = new CsdlEntityType("EntityType", null, false, false, false, null, Enumerable.Empty<CsdlProperty>(), Enumerable.Empty<CsdlNavigationProperty>(), null, null);

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
            var function = new CsdlFunction("Checkout", Enumerable.Empty<CsdlOperationParameter>(), new CsdlNamedTypeReference("Edm.String", false, testLocation), false /*isBound*/, null /*entitySetPath*/, true /*isComposable*/, null /*documentation*/, testLocation);
            var semanticFunction = new CsdlSemanticsFunction(this.semanticSchema, function);
            semanticFunction.IsBound.Should().BeFalse();
            semanticFunction.Location().Should().Be(testLocation);
            semanticFunction.Name.Should().Be("Checkout");
            semanticFunction.Namespace.Should().Be("FQ.NS");
            semanticFunction.ReturnType.Definition.Should().Be(EdmCoreModel.Instance.GetString(true).Definition);
            semanticFunction.EntitySetPath.Should().BeNull();
            semanticFunction.SchemaElementKind.Should().Be(EdmSchemaElementKind.Function);
            semanticFunction.IsComposable.Should().BeTrue();
        }

        [Fact]
        public void BoundCsdlSemanticsOperationPropertiesShouldBeCorrect()
        {
            var action = new CsdlAction(
                "Checkout",
                new CsdlOperationParameter[] { new CsdlOperationParameter("entity", new CsdlNamedTypeReference("FQ.NS.EntityType", false, testLocation), null, testLocation) }, 
                new CsdlNamedTypeReference("Edm.String", false, testLocation), 
                true /*isBound*/, 
                "entity/FakePath", 
                null /*documentation*/, 
                testLocation);

            var semanticAction = new CsdlSemanticsAction(this.semanticSchema, action);
            semanticAction.IsBound.Should().BeTrue();
            semanticAction.Location().Should().Be(testLocation);
            semanticAction.Name.Should().Be("Checkout");
            semanticAction.Namespace.Should().Be("FQ.NS");
            semanticAction.ReturnType.Definition.Should().Be(EdmCoreModel.Instance.GetString(true).Definition);
            semanticAction.EntitySetPath.Path.ToList()[0].Should().Be("entity");
            semanticAction.EntitySetPath.Path.ToList()[1].Should().Be("FakePath");
            semanticAction.SchemaElementKind.Should().Be(EdmSchemaElementKind.Action);
        }
    }
}
