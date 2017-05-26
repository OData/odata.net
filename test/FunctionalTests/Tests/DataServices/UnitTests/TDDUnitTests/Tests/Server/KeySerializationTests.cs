//---------------------------------------------------------------------
// <copyright file="KeySerializationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server
{
    using System;
    using FluentAssertions;
    using Microsoft.OData;
    using Microsoft.OData.Service.Providers;
    using Microsoft.OData.Service.Serializers;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class KeySerializationTests
    {
        private ResourceProperty keyProperty1;
        private ResourceProperty keyProperty2;
        private KeySerializer defaultSerializer;
        private KeySerializer segmentSerializer;
        private readonly Uri serviceBaseUri = new Uri("http://odata.org/");
        private ResourceProperty[] singleKeyProperty;
        private ResourceProperty[] compositeKey;

        [TestInitialize]
        public void Init()
        {
            this.keyProperty1 = new ResourceProperty("Key1", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(string)));
            this.keyProperty2 = new ResourceProperty("Key2", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int)));

            this.defaultSerializer = KeySerializer.Create(false);
            this.segmentSerializer = KeySerializer.Create(true);

            this.singleKeyProperty = new[] { this.keyProperty1 };
            this.compositeKey = new[] { this.keyProperty1, this.keyProperty2 };
        }

        [TestMethod]
        public void DefaultSerializerShouldNotWritePropertyNameForSingleKey()
        {
            LazySerializedEntityKey.Create(this.defaultSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => "foo", null).RelativeEditLink.Should().Be("Fake('foo')");
        }

        [TestMethod]
        public void DefaultSerializerShouldWritePropertyNamesForCompositeKey()
        {
            LazySerializedEntityKey.Create(this.defaultSerializer, this.serviceBaseUri, "Fake", this.compositeKey, p => "foo", null).RelativeEditLink.Should().Be("Fake(Key1='foo',Key2='foo')");
        }

        [TestMethod]
        public void SegmentSerializerShouldWriteSlash()
        {
            LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => "foo", null).RelativeEditLink.Should().Be("Fake/foo");
        }

        [TestMethod]
        public void SegmentSerializerShouldUseParenthesesForCompositeKey()
        {
            LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.compositeKey, p => "foo", null).RelativeEditLink.Should().Be("Fake(Key1='foo',Key2='foo')");
        }

        [TestMethod]
        public void SegmentSerializerShouldUseSegmentsForIdentity()
        {
            LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => "foo", null).Identity.Should().Be("http://odata.org/Fake/foo");
        }

        [TestMethod]
        public void DefaultSerializerShouldIncludeSuffixOnEditLink()
        {
            SerializedEntityKey testSubject = LazySerializedEntityKey.Create(this.defaultSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => 1, "FakeTypeSegment");
            testSubject.RelativeEditLink.Should().Be("Fake(1)/FakeTypeSegment");
            testSubject.AbsoluteEditLink.Should().Be("http://odata.org/Fake(1)/FakeTypeSegment");
        }

        [TestMethod]
        public void SegmentSerializerShouldIncludeSuffixOnEditLink()
        {
            SerializedEntityKey testSubject = LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => 1, "FakeTypeSegment");
            testSubject.RelativeEditLink.Should().Be("Fake/1/FakeTypeSegment");
            testSubject.AbsoluteEditLink.Should().Be("http://odata.org/Fake/1/FakeTypeSegment");
        }

        [TestMethod]
        public void DefaultSerializerShouldNotIncludeSuffixOnIdentity()
        {
            LazySerializedEntityKey.Create(this.defaultSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => 1, "FakeTypeSegment").Identity.Should().Be("http://odata.org/Fake(1)");
        }

        [TestMethod]
        public void SegmentSerializerShouldNotIncludeSuffixOnIdentity()
        {
            LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => 1, "FakeTypeSegment").Identity.Should().Be("http://odata.org/Fake/1");
        }

        [TestMethod]
        public void SegmentSerializerShouldNotWriteTypeMarkersOrQuotes()
        {
            LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => "foo", null).RelativeEditLink.Should().Be("Fake/foo");
            LazySerializedEntityKey.Create(this.segmentSerializer, this.serviceBaseUri, "Fake", this.singleKeyProperty, p => 1.0M, null).RelativeEditLink.Should().Be("Fake/1.0");
        }
    }
}
