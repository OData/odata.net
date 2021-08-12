//---------------------------------------------------------------------
// <copyright file="CsdlSemanticsEntityContainerTests.cs" company="Microsoft">
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
    /// Unit tests for CsdlSemanticsEntityContainer
    /// </summary>
    public class CsdlSemanticsEntityContainerTests
    {
        private readonly CsdlLocation testLocation = new CsdlLocation(1, 2);

        [Fact]
        public void EnsureFunctionImportFunctionPropertyIsUnresolvedFunction()
        {
            var functionImport = new CsdlFunctionImport("GetStuff", "FQ.NS.GetStuff", null /*entitySet*/, true /*includeInServiceDocument*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container", operationImports: new CsdlOperationImport[] {functionImport});
            var schema = CsdlBuilder.Schema("FQ.NS", csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);

            var semanticSchema = new CsdlSemanticsSchema(new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>()), schema);

            CsdlSemanticsEntityContainer container = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var imports = container.OperationImports().ToList();
            var csdlFunctionImport = Assert.Single(imports) as IEdmFunctionImport;
            Assert.Equal("GetStuff", csdlFunctionImport.Name);
            Assert.IsType<UnresolvedFunction>(csdlFunctionImport.Operation);
            var errors = (csdlFunctionImport.Operation as BadElement).Errors.ToList();
            var error = Assert.Single(errors);
            Assert.Equal(Strings.Bad_UnresolvedOperation("FQ.NS.GetStuff"), error.ErrorMessage);
            Assert.Equal("Container", csdlFunctionImport.Container.Name);
            Assert.Equal(testLocation, csdlFunctionImport.Location());
            Assert.Equal(EdmContainerElementKind.FunctionImport, csdlFunctionImport.ContainerElementKind);
            Assert.Null(csdlFunctionImport.EntitySet);
            Assert.True(csdlFunctionImport.IncludeInServiceDocument);
            Assert.False(csdlFunctionImport.Function.IsComposable);
        }

        [Fact]
        public void EnsureActionImportActionPropertyIsUnresolvedAction()
        {
            var actionImport = new CsdlActionImport("Action", "FQ.NS.Action", null /*entitySet*/, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container", operationImports: new CsdlOperationImport[] { actionImport });
            var schema = CsdlBuilder.Schema("FQ.NS", csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);
            var semanticSchema = new CsdlSemanticsSchema(new CsdlSemanticsModel(csdlModel, new EdmDirectValueAnnotationsManager(), Enumerable.Empty<IEdmModel>()), schema);
            
            CsdlSemanticsEntityContainer container = new CsdlSemanticsEntityContainer(semanticSchema, csdlEntityContainer);
            var imports = container.OperationImports().ToList();
            Assert.Single(imports);
            Assert.Equal("Action", imports[0].Name);
            Assert.IsType<UnresolvedAction>(imports[0].Operation);
            var errors = (imports[0].Operation as BadElement).Errors.ToList();
            var error = Assert.Single(errors);
            Assert.Equal(Strings.Bad_UnresolvedOperation("FQ.NS.Action"), error.ErrorMessage);
            Assert.Equal("Container", imports[0].Container.Name);
            Assert.Equal(testLocation, imports[0].Location());
            Assert.Equal(EdmContainerElementKind.ActionImport, imports[0].ContainerElementKind);
            Assert.Null(imports[0].EntitySet);
        }

        [Fact]
        public void NavigationPropertyBindingsReturned()
        {
            // arrange
            var entitySet1 = new CsdlEntitySet("EntitySet1", "unknown", new[] { 
                new CsdlNavigationPropertyBinding("foo", "bar", testLocation)
            }, testLocation);
            var singleton1 = new CsdlSingleton("Singleton", "unknown", new[] {
                new CsdlNavigationPropertyBinding("foo", "bar", testLocation)
            }, testLocation);
            var csdlEntityContainer = CsdlBuilder.EntityContainer("Container", entitySets: new [] { entitySet1 }, singletons: new[] { singleton1 } );
            var schema = CsdlBuilder.Schema("FQ.NS", csdlEntityContainers: new CsdlEntityContainer[] { csdlEntityContainer });
            var csdlModel = new CsdlModel();
            csdlModel.AddSchema(schema);
            
            // act
            var container = (IEdmEntityContainer)csdlEntityContainer;
            var bindings = container.GetNavigationPropertyBindings().ToList();

            // assert
            Assert.Equal(2, bindings.Count);
        }
    }
}
