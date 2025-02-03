//---------------------------------------------------------------------
// <copyright file="EdmFunctionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmFunctionTests
    {
        private IEdmPrimitiveTypeReference boolType;
        private IEdmEntityType personType;
        private static string defaultNamespaceName = "DefaultNamespace";
        private static string checkout = "Checkout";

        public EdmFunctionTests()
        {
            this.boolType = EdmCoreModel.Instance.GetBoolean(false);
            this.personType = new EdmEntityType(defaultNamespaceName, "Person");
        }

        [Fact]
        public void EdmFunctionShouldThrowIfReturnTypeIsNull()
        {
            Action test = () => new EdmFunction(defaultNamespaceName, checkout, null);
            Assert.Throws<ArgumentNullException>("returnType", test);
        }

        [Fact]
        public void EdmFunctionConstructorWithNullReturnTypeShouldNotThrow()
        {
            var edmFunction = new EdmFunction(defaultNamespaceName, checkout, this.boolType);
            Assert.Equal(defaultNamespaceName, edmFunction.Namespace);
            Assert.Equal(checkout, edmFunction.Name);
            Assert.Same(this.boolType, edmFunction.ReturnType);
            Assert.False(edmFunction.IsComposable);
        }

        [Fact]
        public void EdmFunctionConstructorShouldDefaultNonSpecifiedPropertiesCorrectly()
        {
            var edmFunction = new EdmFunction(defaultNamespaceName, checkout, this.boolType);
            Assert.Equal(defaultNamespaceName, edmFunction.Namespace);
            Assert.Equal(checkout, edmFunction.Name);
            Assert.Same(this.boolType, edmFunction.ReturnType);
            Assert.Null(edmFunction.EntitySetPath);
            Assert.False(edmFunction.IsBound);
            Assert.Equal(EdmSchemaElementKind.Function, edmFunction.SchemaElementKind);
            Assert.False(edmFunction.IsComposable);
        }

        [Fact]
        public void EdmFunctionConstructorShouldHaveSpecifiedConstructorValues()
        {
            var entitySetPath = new EdmPathExpression("Param1/Nav");
            var edmFunction = new EdmFunction(defaultNamespaceName, checkout, this.boolType, true, entitySetPath, true /*IsComposable*/);
            edmFunction.AddParameter(new EdmOperationParameter(edmFunction, "Param1", new EdmEntityTypeReference(personType, false)));
            Assert.Equal(defaultNamespaceName, edmFunction.Namespace);
            Assert.Equal(checkout, edmFunction.Name);
            Assert.Same(this.boolType, edmFunction.ReturnType);
            Assert.Same(entitySetPath, edmFunction.EntitySetPath);
            Assert.True(edmFunction.IsBound);
            Assert.Equal(EdmSchemaElementKind.Function, edmFunction.SchemaElementKind);
            Assert.True(edmFunction.IsComposable);

            Assert.NotNull(edmFunction.Return);
            Assert.Same(edmFunction.ReturnType, edmFunction.Return.Type);
        }
    }
}
