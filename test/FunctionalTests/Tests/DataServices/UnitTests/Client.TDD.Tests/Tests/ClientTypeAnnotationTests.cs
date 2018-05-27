//---------------------------------------------------------------------
// <copyright file="ClientTypeAnnotationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Client;
    using Microsoft.OData.Client.Metadata;
    using System.Linq;
    using Microsoft.Spatial;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ClientTypeAnnotationTests
    {
        private ClientTypeAnnotation typeWithUnserializableProperties;
        private ClientTypeAnnotation typeWithPropertiesInStrangeOrder;

        [TestInitialize]
        public void Init()
        {
            var clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.typeWithUnserializableProperties = new ClientTypeAnnotation(new EdmEntityType("NS", "Type1"), typeof(EntityWithUnserializableProperties), "NS.Type1", clientEdmModel);
            this.typeWithPropertiesInStrangeOrder = new ClientTypeAnnotation(new EdmEntityType("NS", "Type2"), typeof(EntityWithPropertiesInStrangeOrder), "NS.Type2", clientEdmModel);
        }

        [TestMethod]
        public void EnsureGeographyTypeIsVersion3()
        {
            VerifyIsVersion4(typeof(EntityTypeWithGeographyProperty));
        }

        [TestMethod]
        public void EnsureGeometryIsVersion3()
        {
            VerifyIsVersion4(typeof(EntityTypeWithGeometryProperty));
        }

        [TestMethod]
        public void ClientTypeAnnotationShouldNotIncludeDictionaryInPropertiesToSerialize()
        {
            this.typeWithUnserializableProperties.PropertiesToSerialize().Should().NotContain(p => p.PropertyName == "Dictionary");
        }

        [TestMethod]
        public void ClientTypeAnnotationShouldNotIncludeMediaPropertiesInPropertiesToSerialize()
        {
            this.typeWithUnserializableProperties.PropertiesToSerialize().Should().NotContain(p => p.PropertyName == "Media");
        }

        [TestMethod]
        public void ClientTypeAnnotationShouldNotIncludeStreamPropertiesInPropertiesToSerialize()
        {
            this.typeWithUnserializableProperties.PropertiesToSerialize().Should().NotContain(p => p.PropertyName == "Stream");
        }

        [TestMethod]
        public void ClientTypeAnnotationShouldNotIncludeMimeTypePropertiesInPropertiesToSerialize()
        {
            this.typeWithUnserializableProperties.PropertiesToSerialize().Should().NotContain(p => p.PropertyName == "MimeType");
        }

        [TestMethod]
        public void ClientTypeAnnotationShouldReturnPropertiesToSerializeInOrder()
        {
            List<string> properties = this.typeWithPropertiesInStrangeOrder.PropertiesToSerialize().Select(p => p.PropertyName).ToList();
            List<string> sortedProperties = properties.ToList();
            sortedProperties.Sort(StringComparer.Ordinal);
            properties.Should().ContainInOrder(sortedProperties);
        }

        private static void VerifyIsVersion4(Type type)
        {
            ClientEdmModel model = new ClientEdmModel(ODataProtocolVersion.V4);
            IEdmType edmType = model.GetOrCreateEdmType(type);
            var typeAnnotation = model.GetClientTypeAnnotation(edmType);
            Assert.AreEqual(typeAnnotation.GetMetadataVersion(), new Version(4, 0), "Type with geography point must have 4.0 version");
        }

        public class EntityTypeWithGeographyProperty
        {
            public int ID { get; set; }
            public GeographyPoint GeographyPoint { get; set; }
        }

        public class EntityTypeWithGeometryProperty
        {
            public int ID { get; set; }
            public GeometryPoint GeometryPoint { get; set; }
        }

        [MediaEntryAttribute("Media")]
        [MimeTypeProperty("Media", "MimeType")]
        public class EntityWithUnserializableProperties
        {
            public int ID { get; set; }
            public Dictionary<string, object> Dictionary { get; set; }
            public byte[] Media { get; set; }
            public string MimeType { get; set; }
            public DataServiceStreamLink Stream { get; set; }
        }

        public class EntityWithPropertiesInStrangeOrder
        {
            public int ID { get; set; }
            public string B { get; set; }
            public string A { get; set; }
            public string C { get; set; }
            public string E { get; set; }
            public string D { get; set; }
            public string X { get; set; }
            public string F { get; set; }
        }
    }
}