//---------------------------------------------------------------------
// <copyright file="EdmActionImportTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
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
            Assert.Equal("CheckoutImport", edmActionImport.Name);
            Assert.Null(edmActionImport.EntitySet);
            Assert.Same(this.entityContainer, edmActionImport.Container);
            Assert.Same(edmAction, edmActionImport.Action);
        }

        [Fact]
        public void EdmActionImportConstructorShouldHaveSpecifiedConstructorValues()
        {
            var actionEntitySetPath = new EdmPathExpression("Param1/Nav");
            var edmAction = new EdmAction("DefaultNamespace", "Checkout", this.boolType, true, actionEntitySetPath);
            edmAction.AddParameter(new EdmOperationParameter(edmAction, "Param1", new EdmEntityTypeReference(personType, true)));
            
            var actionImportEntitySetPath = new EdmPathExpression("Param1/Nav2");
            var edmActionImport = new EdmActionImport(this.entityContainer, "checkoutImport", edmAction, actionImportEntitySetPath);
            Assert.Equal("checkoutImport", edmActionImport.Name);
            Assert.Same(this.entityContainer, edmActionImport.Container);
            Assert.Same(actionImportEntitySetPath, edmActionImport.EntitySet);
            Assert.Same(edmAction, edmActionImport.Action);
        }

        [Fact]
        public void EdmActionImportConstructorWithNullActionShouldThrowArgmentException()
        {
            Action test = ()=> new EdmActionImport(this.entityContainer, "checkoutImport", (IEdmAction)null, null);
            Assert.Throws<ArgumentNullException>("action", test);
        }
    }
}
