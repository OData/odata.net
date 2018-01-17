//---------------------------------------------------------------------
// <copyright file="EntityToSerializeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using FluentAssertions;

    [TestClass]
    public class EntityToSerializeTests
    {
        [TestMethod]
        public void EditLinkShouldIncludeTypeSegmentWhenConstructorArgumentIsTrue()
        {
            var entityToSerialize = CreateEntityToSerialize(true);
            entityToSerialize.SerializedKey.AbsoluteEditLink.ToString().Should().EndWith("TestNamespace.BaseType");
        }

        [TestMethod]
        public void EditLinkWithoutSuffixShouldNotIncludeTypeSegmentWhenConstructorArgumentIsTrue()
        {
            var entityToSerialize = CreateEntityToSerialize(true);
            entityToSerialize.SerializedKey.AbsoluteEditLinkWithoutSuffix.ToString().Should().NotContain("TestNamespace.BaseType");
        }

        [TestMethod]
        public void EditLinkShouldNotIncludeTypeSegmentWhenConstructorArgumentIsFalse()
        {
            var entityToSerialize = CreateEntityToSerialize(false);
            entityToSerialize.SerializedKey.AbsoluteEditLink.ToString().Should().NotContain("TestNamespace.BaseType");
        }

        private static EntityToSerialize CreateEntityToSerialize(bool shouldIncludeTypeSegment)
        {
            ResourceType baseType = new ResourceType(typeof(MyType), ResourceTypeKind.EntityType, null, "TestNamespace", "BaseType", /*isAbstract*/ false);
            baseType.AddProperty(new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, new ResourceType(typeof(int), ResourceTypeKind.Primitive, null, "int")));

            baseType.SetReadOnly();

            Uri serviceUri = new Uri("http://dummy");

            KeySerializer keySerializer = KeySerializer.Create(false);

            Func<ResourceProperty, object> getPropertyValue = p => "fakePropertyValue";
            return EntityToSerialize.Create(new MyType { ID = 42 }, baseType, "MySet", shouldIncludeTypeSegment, getPropertyValue, keySerializer, serviceUri);
        }

        private class MyType
        {
            public int ID { get; set; }
        }

    }
}
