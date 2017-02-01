//---------------------------------------------------------------------
// <copyright file="PropertyCacheHandlerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.Spatial;
using Xunit;

namespace Microsoft.OData.Tests
{
    /// <summary>
    /// This is a test class for PropertyCacheHandle and is intended
    /// to contain all PropertyCacheHandle Unit Tests
    ///</summary>
    public class PropertyCacheHandlerTests
    {
        [Fact]
        public void PropertyHandlerGetProperty()
        {
            // Model with a single entity type
            EdmModel model = new EdmModel();
            const string defaultNamespaceName = "Test";
            var int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);

            // Create a complext type.
            var complexType = new EdmComplexType(defaultNamespaceName, "ComplexType");
            complexType.AddStructuralProperty("IntProp", int32TypeRef);
            complexType.AddStructuralProperty("StringProp", EdmCoreModel.Instance.GetString(isNullable: false));
            complexType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            model.AddElement(complexType);

            // Create an entity with a complex type property.
            var singleComplexPropertyEntityType = new EdmEntityType(defaultNamespaceName, "SingleComplexPropertyEntityType");
            singleComplexPropertyEntityType.AddKeys(singleComplexPropertyEntityType.AddStructuralProperty("ID", int32TypeRef));
            singleComplexPropertyEntityType.AddStructuralProperty("ComplexProp", new EdmComplexTypeReference(complexType, isNullable: true));
            model.AddElement(singleComplexPropertyEntityType);

            // Create a property handler and enter a resource set scope.
            PropertyCacheHandler handler = new PropertyCacheHandler();
            handler.EnterResourceSetScope(singleComplexPropertyEntityType, 0);

            // Create a PropertySerializationInfo for ComplexProp.IntProp
            var info1 = handler.GetProperty("IntProp", complexType);
            info1.Should().NotBeNull();

            // Get a second PropertySerializationInfo for ComplexProp.IntProp; it should be the same.
            PropertySerializationInfo info2 = handler.GetProperty("IntProp", complexType);
            info2.Should().NotBeNull();
            info2.Should().BeSameAs(info1);
        }

        [Fact]
        public void PropertyHandlerGetPropertyNullOwningType()
        {
            // Model with a single entity type
            EdmModel model = new EdmModel();
            const string defaultNamespaceName = "Test";
            var int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);

            // Create an entity with a complex type property.
            var singleComplexPropertyEntityType = new EdmEntityType(defaultNamespaceName, "SingleComplexPropertyEntityType");
            singleComplexPropertyEntityType.AddKeys(singleComplexPropertyEntityType.AddStructuralProperty("ID", int32TypeRef));
            model.AddElement(singleComplexPropertyEntityType);

            // Create a property handler and enter a resource set scope.
            PropertyCacheHandler handler = new PropertyCacheHandler();
            handler.EnterResourceSetScope(singleComplexPropertyEntityType, 0);

            // Create a PropertySerializationInfo for ComplexProp.IntProp
            var info1 = handler.GetProperty("IntProp", null);
            info1.Should().NotBeNull();

            // Get a second PropertySerializationInfo for ComplexProp.IntProp; it should be the same.
            PropertySerializationInfo info2 = handler.GetProperty("IntProp", null);
            info2.Should().NotBeNull();
            info2.Should().BeSameAs(info1);
        }

        [Fact]
        public void PropertyHandlerGetPropertyNameCollision()
        {
            // Model with a single entity type
            EdmModel model = new EdmModel();
            const string defaultNamespaceName = "Test";
            var int32TypeRef = EdmCoreModel.Instance.GetInt32(isNullable: false);

            // Create a complext types.
            var complexType1 = new EdmComplexType(defaultNamespaceName, "ComplexType1");
            complexType1.AddStructuralProperty("Prop1", int32TypeRef);
            model.AddElement(complexType1);

            var complexType2 = new EdmComplexType(defaultNamespaceName, "ComplexType2");
            complexType1.AddStructuralProperty("Prop1", EdmCoreModel.Instance.GetString(isNullable: false));
            model.AddElement(complexType2);

            // Create an entity with a complex type property.
            var singleComplexPropertyEntityType = new EdmEntityType(defaultNamespaceName, "SingleComplexPropertyEntityType");
            singleComplexPropertyEntityType.AddKeys(singleComplexPropertyEntityType.AddStructuralProperty("ID", int32TypeRef));
            singleComplexPropertyEntityType.AddStructuralProperty("ComplexProp1", new EdmComplexTypeReference(complexType1, isNullable: true));
            singleComplexPropertyEntityType.AddStructuralProperty("ComplexProp2", new EdmComplexTypeReference(complexType2, isNullable: true));
            model.AddElement(singleComplexPropertyEntityType);

            // Create a property handler and enter a resource set scope.
            PropertyCacheHandler handler = new PropertyCacheHandler();
            handler.EnterResourceSetScope(singleComplexPropertyEntityType, 0);

            // Create a PropertySerializationInfo for ComplexProp1.Prop1
            var info1 = handler.GetProperty("Prop1", complexType1);
            info1.Should().NotBeNull();

            // Create a PropertySerializationInfo for ComplexProp2.Prop1; they shoudl be different.
            var info2 = handler.GetProperty("Prop1", complexType2);
            info2.Should().NotBeNull();
            info2.Should().NotBeSameAs(info1);
        }
    }
}
