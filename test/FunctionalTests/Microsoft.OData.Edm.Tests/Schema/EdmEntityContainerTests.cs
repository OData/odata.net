//---------------------------------------------------------------------
// <copyright file="EdmEntityContainerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmEntityContainerTests
    {
        [Fact]
        public void EdmModelAddEntityContainerTest()
        {
            var model = new EdmModel();
            Assert.Equal(
                model.AddEntityContainer("NS", "Container"),
                model.FindEntityContainer("NS.Container"));
        }

        #region AddActionImport Api tests.
        [Fact]
        public void EnsureActionImportIsAddedWithActionSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var actionImport = container.AddActionImport(action);

            Assert.Same(action, actionImport.Action);
            Assert.Equal(action.Name, actionImport.Name);
            Assert.Same(actionImport, container.Elements.ToArray()[0]);
        }

        [Fact]
        public void EnsureActionImportIsAddedAndWithCorrectSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var actionImport = container.AddActionImport("OtherName", action);

            Assert.Same(action, actionImport.Action);
            Assert.Equal("OtherName", actionImport.Name);
            Assert.Same(actionImport, container.Elements.ToArray()[0]);
        }

        [Fact]
        public void EnsureActionImportIsAddedAndWithCorrectEntitySetExpression()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var entitySetExpression = new EdmPathExpression("EntitySet");
            var actionImport = container.AddActionImport("OtherName", action, entitySetExpression);

            Assert.Same(action, actionImport.Action);
            Assert.Equal("OtherName", actionImport.Name);
            Assert.Same(entitySetExpression, actionImport.EntitySet);
            Assert.Same(actionImport, container.Elements.ToArray()[0]);
        }
        #endregion

        #region AddFunctionImport Api tests.
        [Fact]
        public void EnsureFunctionImportIsAddedWithActionSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmFunction function = new EdmFunction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var functionImport = container.AddFunctionImport(function);

            Assert.Same(function, functionImport.Function);
            Assert.Equal(function.Name, functionImport.Name);
            Assert.Same(functionImport, container.Elements.ToArray()[0]);
        }

        [Fact]
        public void EnsureFunctionImportIsAddedAndWithCorrectSuppliedName()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmFunction function = new EdmFunction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var functionImport = container.AddFunctionImport("OtherName", function);

            Assert.Same(function, functionImport.Function);
            Assert.Equal("OtherName", functionImport.Name);
            Assert.Same(functionImport, container.Elements.ToArray()[0]);
        }

        [Fact]
        public void EnsureFunctionImportIsAddedAndWithCorrectEntitySetExpression()
        {
            EdmEntityContainer container = new EdmEntityContainer("Default", "Container");
            EdmAction action = new EdmAction("DS", "TestAction", EdmCoreModel.Instance.GetBoolean(false));
            var entitySetExpression = new EdmPathExpression("EntitySet");
            var actionImport = container.AddActionImport("OtherName", action, entitySetExpression);

            Assert.Same(action, actionImport.Action);
            Assert.Equal("OtherName", actionImport.Name);
            Assert.Same(entitySetExpression, actionImport.EntitySet);
            Assert.Same(actionImport, container.Elements.ToArray()[0]);
        }
        #endregion

        #region AddEntitySet tests

        [Fact]
        public void EnsureAddedEntitySetHasAllPropertiesSet()
        {
            EdmModel model = new EdmModel();
            EdmEntityContainer container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);
            EdmEntityType entityType = model.AddEntityType("NS", "Entity");

            EdmEntitySet entitySetDefault = container.AddEntitySet("DefaultEntities", entityType);
            EdmEntitySet entitySetIncludeInServiceDocumentTrue = container.AddEntitySet("IncludeInServiceDocumentTrueEntities", entityType, true);
            EdmEntitySet entitySetIncludeInServiceDocumentFalse = container.AddEntitySet("IncludeInServiceDocumentFalseEntities", entityType, false);

            Assert.NotNull(entitySetDefault.Container);
            Assert.NotNull(entitySetIncludeInServiceDocumentTrue.Container);
            Assert.NotNull(entitySetIncludeInServiceDocumentFalse.Container);

            Assert.NotNull(entitySetDefault.Type);
            Assert.NotNull(entitySetIncludeInServiceDocumentTrue.Type);
            Assert.NotNull(entitySetIncludeInServiceDocumentFalse.Type);

            Assert.NotNull(entitySetDefault.Path);
            Assert.NotNull(entitySetIncludeInServiceDocumentTrue.Path);
            Assert.NotNull(entitySetIncludeInServiceDocumentFalse.Path);

            Assert.Same(entitySetDefault.Container, container);
            Assert.Same(entitySetIncludeInServiceDocumentTrue.Container, container);
            Assert.Same(entitySetIncludeInServiceDocumentFalse.Container, container);

            Assert.Same(entitySetDefault.Type.AsElementType(), entityType);
            Assert.Same(entitySetIncludeInServiceDocumentTrue.Type.AsElementType(), entityType);
            Assert.Same(entitySetIncludeInServiceDocumentFalse.Type.AsElementType(), entityType);

            Assert.Equal("DefaultEntities", entitySetDefault.Path.Path);
            Assert.Equal("IncludeInServiceDocumentTrueEntities", entitySetIncludeInServiceDocumentTrue.Path.Path);
            Assert.Equal("IncludeInServiceDocumentFalseEntities", entitySetIncludeInServiceDocumentFalse.Path.Path);

            Assert.True(entitySetDefault.IncludeInServiceDocument);
            Assert.True(entitySetIncludeInServiceDocumentTrue.IncludeInServiceDocument);
            Assert.False(entitySetIncludeInServiceDocumentFalse.IncludeInServiceDocument);
        }

        #endregion
    }
}
