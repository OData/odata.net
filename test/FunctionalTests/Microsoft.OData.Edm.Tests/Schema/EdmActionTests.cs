//---------------------------------------------------------------------
// <copyright file="EdmActionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmActionTests
    {
        private IEdmPrimitiveTypeReference boolType;
        private IEdmEntityType personType;
        private static string defaultNamespaceName = "DefaultNamespace";
        private static string checkout = "Checkout";

        public EdmActionTests()
        {
            this.boolType = EdmCoreModel.Instance.GetBoolean(false);
            this.personType = new EdmEntityType(defaultNamespaceName, "Person");
        }

        [Fact]
        public void EdmActionConstructorWithNullReturnTypeShouldNotThrow()
        {
            var edmAction = new EdmAction(defaultNamespaceName, checkout, null);
            Assert.Equal(defaultNamespaceName, edmAction.Namespace);
            Assert.Equal(checkout, edmAction.Name);
            Assert.Null(edmAction.ReturnType);
        }

        [Fact]
        public void EdmActionConstructorShouldDefaultNonSpecifiedPropertiesCorrectly()
        {
            var edmAction = new EdmAction(defaultNamespaceName, checkout, this.boolType);
            Assert.Equal(defaultNamespaceName, edmAction.Namespace);
            Assert.Equal(checkout, edmAction.Name);
            Assert.Same(this.boolType, edmAction.ReturnType);
            Assert.Null(edmAction.EntitySetPath);
            Assert.False(edmAction.IsBound);
            Assert.Equal(EdmSchemaElementKind.Action, edmAction.SchemaElementKind);
        }

        [Fact]
        public void EdmActionConstructorShouldHaveSpecifiedConstructorValues()
        {
            var entitySetPath = new EdmPathExpression("Param1/Nav");
            var edmAction = new EdmAction(defaultNamespaceName, checkout, this.boolType, true, entitySetPath);
            edmAction.AddParameter(new EdmOperationParameter(edmAction, "Param1", new EdmEntityTypeReference(personType, false)));
            Assert.Equal(defaultNamespaceName, edmAction.Namespace);
            Assert.Equal(checkout, edmAction.Name);
            Assert.Same(this.boolType, edmAction.ReturnType);
            Assert.Same(entitySetPath, edmAction.EntitySetPath);
            Assert.True(edmAction.IsBound);
            Assert.Equal(EdmSchemaElementKind.Action, edmAction.SchemaElementKind);

            Assert.NotNull(edmAction.Return);
            Assert.Same(edmAction.ReturnType, edmAction.Return.Type);
        }
    }
}
