//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntityContainerTests.cs" company="Microsoft">
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
    /// Unit tests for CsdlSemanticsEntityContainer
    /// </summary>
    public class CsdlSemanticsEntityContainerTests
    {
        private readonly CsdlLocation testLocation = new CsdlLocation(1, 2);

        [Fact]
        public void EnsureFunctionImportFunctionPropertyIsUnresolvedFunction()
        {
            var functionImport = new CsdlFunctionImport("GetStuff", "FQ.NS.GetStuff", null /*entitySet*/, true /*includeInServiceDocument*/, null /*documentation*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container", operationImports: new CsdlOperationImport[] {functionImport});
            var schema = CsdlBuilder.Schema("FQ.NS", csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);
            
            var semanticSchema = new CsdlSemanticsSchema(new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>()), schema);

            CsdlSemanticsEntityContainer container = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var imports = container.OperationImports().ToList();
            imports.Should().HaveCount(1);
            var csdlFunctionImport = (IEdmFunctionImport)imports[0];
            csdlFunctionImport.Name.Should().Be("GetStuff");
            csdlFunctionImport.Operation.GetType().Should().Be(typeof(UnresolvedFunction));
            var errors = csdlFunctionImport.Operation.As<BadElement>().Errors.ToList();
            errors.Should().HaveCount(1);
            errors.First().ErrorMessage.Should().Be(Strings.Bad_UnresolvedOperation("FQ.NS.GetStuff"));
            csdlFunctionImport.Container.Name.Should().Be("Container");
            csdlFunctionImport.Location().Should().Be(testLocation);
            csdlFunctionImport.ContainerElementKind.Should().Be(EdmContainerElementKind.FunctionImport);
            csdlFunctionImport.EntitySet.Should().BeNull();
            csdlFunctionImport.IncludeInServiceDocument.Should().BeTrue();
            csdlFunctionImport.Function.IsComposable.Should().BeFalse();
        }

        [Fact]
        public void EnsureActionImportActionPropertyIsUnresolvedAction()
        {
            var actionImport = new CsdlActionImport("Action", "FQ.NS.Action", null /*entitySet*/, null /*documentation*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container", operationImports: new CsdlOperationImport[] { actionImport });
            var schema = CsdlBuilder.Schema("FQ.NS", csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);
            var semanticSchema = new CsdlSemanticsSchema(new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>()), schema);
            
            CsdlSemanticsEntityContainer container = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var imports = container.OperationImports().ToList();
            imports.Should().HaveCount(1);
            imports[0].Name.Should().Be("Action");
            imports[0].Operation.GetType().Should().Be(typeof(UnresolvedAction));
            var errors = imports[0].Operation.As<BadElement>().Errors.ToList();
            errors.Should().HaveCount(1);
            errors.First().ErrorMessage.Should().Be(Strings.Bad_UnresolvedOperation("FQ.NS.Action"));
            imports[0].Container.Name.Should().Be("Container");
            imports[0].Location().Should().Be(testLocation);
            imports[0].ContainerElementKind.Should().Be(EdmContainerElementKind.ActionImport);
            imports[0].EntitySet.Should().BeNull();
        }
    }
}
