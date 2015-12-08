//---------------------------------------------------------------------
// <copyright file="EdmActionImportTests.cs" company="Microsoft">
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
    public class EdmActionImportTests
    {
        private IEdmPrimitiveTypeReference boolType;
        private EdmEntityContainer entityContainer;
        private IEdmEntityType personType;

        public EdmActionImportTests()
        {
            this.boolType = EdmCoreModel.Instance.GetBoolean(false);
            this.entityContainer = new EdmEntityContainer("DefaultNamespace", "Container");
            this.personType = new EdmEntityType("DefaultNamespace", "Person");
        }

        [Fact]
        public void EdmActionImportConstructorShouldDefaultNonSpecifiedPropertiesCorrectly()
        {
            var edmAction = new EdmAction("DefaultNamespace", "Checkout", this.boolType);
            var edmActionImport = new EdmActionImport(this.entityContainer, "CheckoutImport", edmAction, null);
            edmActionImport.Name.Should().Be("CheckoutImport");
            edmActionImport.EntitySet.Should().BeNull();
            edmActionImport.Container.Should().Be(this.entityContainer);
            edmActionImport.Action.Should().Be(edmAction);
        }

        [Fact]
        public void EdmActionImportConstructorShouldHaveSpecifiedConstructorValues()
        {
            var actionEntitySetPath = new EdmPathExpression("Param1/Nav");
            var edmAction = new EdmAction("DefaultNamespace", "Checkout", this.boolType, true, actionEntitySetPath);
            edmAction.AddParameter(new EdmOperationParameter(edmAction, "Param1", new EdmEntityTypeReference(personType, true)));
            
            var actionImportEntitySetPath = new EdmPathExpression("Param1/Nav2");
            var edmActionImport = new EdmActionImport(this.entityContainer, "checkoutImport", edmAction, actionImportEntitySetPath);
            edmActionImport.Name.Should().Be("checkoutImport");
            edmActionImport.Container.Should().Be(this.entityContainer);
            edmActionImport.EntitySet.Should().Be(actionImportEntitySetPath);
            edmActionImport.Action.Should().Be(edmAction);
        }

        [Fact]
        public void EdmActionImportConstructorWithNullActionShouldThrowArgmentException()
        {
            Action test = ()=> new EdmActionImport(this.entityContainer, "checkoutImport", (IEdmAction)null, null);
            test.ShouldThrow<ArgumentNullException>().WithMessage("Value cannot be null.\r\nParameter name: action");
        }
    }
}
