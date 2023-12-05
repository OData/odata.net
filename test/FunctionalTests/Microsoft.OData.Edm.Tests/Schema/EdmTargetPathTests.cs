//---------------------------------------------------------------------
// <copyright file="EdmTargetPathTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Library
{
    public class EdmTargetPathTests
    {
        [Fact]
        public void InitializeEdmTargetPathWithNull()
        {
            Action parseAction = () => new EdmTargetPath(null);

            Assert.Throws<ArgumentNullException>(parseAction);
        }

        [Fact]
        public void InitializeEdmTargetPath_FirstSegmentIsNotContainer()
        {
            // Arrange
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Name").FirstOrDefault();

            // Act & Assert
            Action action = () => new EdmTargetPath(customer, nameProperty);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal(Strings.TargetPath_FirstSegmentMustBeIEdmEntityContainer, exception.Message);
        }

        [Fact]
        public void InitializeEdmTargetPath_SecondSegmentIsNotContainerElement()
        {
            // Arrange
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntityType customer = new EdmEntityType("NS", "Customer");

            // Act & Assert
            Action action = () => new EdmTargetPath(container,customer);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal(Strings.TargetPath_SecondSegmentMustBeIEdmEntityContainerElement, exception.Message);
        }

        [Fact]
        public void InitializeEdmTargetPath_WithNullElement()
        {
            // Arrange
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntityType customer = new EdmEntityType("NS", "Customer");

            // Act & Assert
            Action action = () => new EdmTargetPath(container, null, customer);

            ArgumentException exception = Assert.Throws<ArgumentException>(action);
            Assert.Equal(Strings.TargetPath_SegmentsMustNotContainNullSegment, exception.Message);
        }

        [Fact]
        public void TwoEqualEdmTargetPaths_ReturnsTrue()
        {
            // Arrange
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);

            // Act & Assert
            EdmTargetPath targetPath1 = new EdmTargetPath(container, entitySet, customer);
            EdmTargetPath targetPath2 = new EdmTargetPath(container, entitySet, customer);

            Assert.True(targetPath1.Equals(targetPath2));
        }

        [Fact]
        public void TwoUnEqualEdmTargetPaths_ReturnsFalse()
        {
            // Arrange
            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            EdmEntityType derivedCustomer = new EdmEntityType("NS", "DerivedCustomer", customer);
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);

            // Act & Assert
            EdmTargetPath targetPath1 = new EdmTargetPath(container, entitySet, customer);
            EdmTargetPath targetPath2 = new EdmTargetPath(container, entitySet, derivedCustomer);

            Assert.False(targetPath1.Equals(targetPath2));

            EdmTargetPath targetPath3 = new EdmTargetPath(container, entitySet, customer);
            EdmTargetPath targetPath4 = new EdmTargetPath(container, entitySet);

            Assert.False(targetPath3.Equals(targetPath4));
        }
    }
}
