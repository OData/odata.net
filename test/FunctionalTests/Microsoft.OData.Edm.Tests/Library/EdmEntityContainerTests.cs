//---------------------------------------------------------------------
// <copyright file="EdmEntityContainerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmEntityContainerTests
    {
        #region AddActionImport Api tests.
        [Fact]
        public void EnsureActionImportIsAddedWithActionSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var actionImport = container.AddActionImport(action);

            actionImport.Action.Should().Be(action);
            actionImport.Name.Should().Be(action.Name);
            container.Elements.ToArray()[0].Should().Be(actionImport);
        }

        [Fact]
        public void EnsureActionImportIsAddedAndWithCorrectSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var actionImport = container.AddActionImport("OtherName", action);

            actionImport.Action.Should().Be(action);
            actionImport.Name.Should().Be("OtherName");
            container.Elements.ToArray()[0].Should().Be(actionImport);
        }

        [Fact]
        public void EnsureActionImportIsAddedAndWithCorrectEntitySetExpression()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var edmEntitySet = new EdmEntitySet(container, "EntitySet", new EdmEntityType("DS", "TestEntity"));
            var entitySetExpression = new EdmEntitySetReferenceExpression(edmEntitySet);
            var actionImport = container.AddActionImport("OtherName", action, entitySetExpression);

            actionImport.Action.Should().Be(action);
            actionImport.Name.Should().Be("OtherName");
            actionImport.EntitySet.Should().Be(entitySetExpression);
            container.Elements.ToArray()[0].Should().Be(actionImport);
        }
        #endregion

        #region AddFunctionImport Api tests.
        [Fact]
        public void EnsureFunctionImportIsAddedWithActionSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmFunction function = new EdmFunction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var functionImport = container.AddFunctionImport(function);

            functionImport.Function.Should().Be(function);
            functionImport.Name.Should().Be(function.Name);
            container.Elements.ToArray()[0].Should().Be(functionImport);
        }

        [Fact]
        public void EnsureFunctionImportIsAddedAndWithCorrectSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmFunction function = new EdmFunction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var functionImport = container.AddFunctionImport("OtherName", function);

            functionImport.Function.Should().Be(function);
            functionImport.Name.Should().Be("OtherName");
            container.Elements.ToArray()[0].Should().Be(functionImport);
        }

        [Fact]
        public void EnsureFunctionImportIsAddedAndWithCorrectEntitySetExpression()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var edmEntitySet = new EdmEntitySet(container, "EntitySet", new EdmEntityType("DS", "TestEntity"));
            var entitySetExpression = new EdmEntitySetReferenceExpression(edmEntitySet);
            var functionImport = container.AddActionImport("OtherName", action, entitySetExpression);

            functionImport.Action.Should().Be(action);
            functionImport.Name.Should().Be("OtherName");
            functionImport.EntitySet.Should().Be(entitySetExpression);
            container.Elements.ToArray()[0].Should().Be(functionImport);
        }
        #endregion
    }
}
