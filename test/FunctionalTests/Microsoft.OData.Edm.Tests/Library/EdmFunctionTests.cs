//---------------------------------------------------------------------
// <copyright file="EdmFunctionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Edm.Library;
using Microsoft.OData.Edm.Library.Expressions;
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
            test.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void EdmFunctionConstructorWithNullReturnTypeShouldNotThrow()
        {
            var edmFunction = new EdmFunction(defaultNamespaceName, checkout, this.boolType);
            edmFunction.Namespace.Should().Be(defaultNamespaceName);
            edmFunction.Name.Should().Be(checkout);
            edmFunction.ReturnType.Should().Be(this.boolType);
            edmFunction.IsComposable.Should().BeFalse();
        }

        [Fact]
        public void EdmFunctionConstructorShouldDefaultNonSpecifiedPropertiesCorrectly()
        {
            var edmFunction = new EdmFunction(defaultNamespaceName, checkout, this.boolType);
            edmFunction.Namespace.Should().Be(defaultNamespaceName);
            edmFunction.Name.Should().Be(checkout);
            edmFunction.ReturnType.Should().Be(this.boolType);
            edmFunction.EntitySetPath.Should().BeNull();
            edmFunction.IsBound.Should().BeFalse();
            edmFunction.SchemaElementKind.Should().Be(EdmSchemaElementKind.Function);
            edmFunction.IsComposable.Should().BeFalse();
        }

        [Fact]
        public void EdmFunctionConstructorShouldHaveSpecifiedConstructorValues()
        {
            var entitySetPath = new EdmPathExpression("Param1/Nav");
            var edmFunction = new EdmFunction(defaultNamespaceName, checkout, this.boolType, true, entitySetPath, true /*IsComposable*/);
            edmFunction.AddParameter(new EdmOperationParameter(edmFunction, "Param1", new EdmEntityTypeReference(personType, false)));
            edmFunction.Namespace.Should().Be(defaultNamespaceName);
            edmFunction.Name.Should().Be(checkout);
            edmFunction.ReturnType.Should().Be(this.boolType);
            edmFunction.EntitySetPath.Should().Be(entitySetPath);
            edmFunction.IsBound.Should().BeTrue();
            edmFunction.SchemaElementKind.Should().Be(EdmSchemaElementKind.Function);
            edmFunction.IsComposable.Should().BeTrue();
        }
    }
}
