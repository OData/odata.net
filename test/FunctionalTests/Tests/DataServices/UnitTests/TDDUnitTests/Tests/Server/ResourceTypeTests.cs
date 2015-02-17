//---------------------------------------------------------------------
// <copyright file="ResourceTypeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests.Server
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.IO;
    using Microsoft.Spatial;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ResourceTypeTests
    {
        private Version v4 = new Version(4, 0);
        [TestMethod]
        public void VerifyVersionOfEntityWithGeographyPropertyIsV3()
        {
            ResourceType rt = new ResourceType(typeof(IDictionary<string, string>), ResourceTypeKind.EntityType, null, "TestNamespace", "TestEntity", false);
            var keyProperty = new ResourceProperty("ID", ResourcePropertyKind.Primitive | ResourcePropertyKind.Key, ResourceType.GetPrimitiveResourceType(typeof(int))) {CanReflectOnInstanceTypeProperty = false };
            var geographyProperty = new ResourceProperty("GeographyProperty", ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(Geography))) { CanReflectOnInstanceTypeProperty = false };
            rt.AddProperty(keyProperty);
            rt.AddProperty(geographyProperty);
            rt.SetReadOnly();

            Assert.AreEqual(v4, rt.MetadataVersion, "MetadataVersion must be 4.0");
            Assert.AreEqual(MetadataEdmSchemaVersion.Version4Dot0, rt.SchemaVersion, "Schema version must be 4.0");
        }

        [TestMethod]
        public void VerifyVersionOfGeographyResourceTypeIsV3()
        {
            ResourceType rt = ResourceType.GetPrimitiveResourceType(typeof(Geography));
            Assert.AreEqual(v4, rt.MetadataVersion, "MetadataVersion must be 4.0");
            Assert.AreEqual(MetadataEdmSchemaVersion.Version4Dot0, rt.SchemaVersion, "Schema version must be 4.0");
        }

        [TestMethod]
        public void VerifyVersionOfGeometryResourceTypeIsV3()
        {
            ResourceType rt = ResourceType.GetPrimitiveResourceType(typeof(Geometry));
            Assert.AreEqual(v4, rt.MetadataVersion, "MetadataVersion must be 4.0");
            Assert.AreEqual(MetadataEdmSchemaVersion.Version4Dot0, rt.SchemaVersion, "Schema version must be 4.0");
        }

        [TestMethod]
        public void VerifyVersionOfCollectionResourceTypeIsV3()
        {
            ResourceType rt = ResourceType.GetCollectionResourceType(ResourceType.GetPrimitiveResourceType(typeof(int)));
            Assert.AreEqual(v4, rt.MetadataVersion, "MetadataVersion must be 4.0");
            Assert.AreEqual(MetadataEdmSchemaVersion.Version4Dot0, rt.SchemaVersion, "Schema version must be 4.0");
        }

        [TestMethod]
        public void VerifyVersionOfStreamResourceTypeShouldBe1MetadataAnd3ForSchemaVersion()
        {
            ResourceType rt = ResourceType.GetPrimitiveResourceType(typeof(Stream));
            Assert.AreEqual(v4, rt.MetadataVersion, "MetadataVersion must be 4.0");
            Assert.AreEqual(MetadataEdmSchemaVersion.Version4Dot0, rt.SchemaVersion, "Schema version must be 4.0");
        }
    }
}
