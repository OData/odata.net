//---------------------------------------------------------------------
// <copyright file="ODataAtomPropertyAndValueDeserializerTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Core.Atom;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.Atom
{
    public class ODataAtomPropertyAndValueDeserializerTests
    {
        private readonly static EdmModel EdmModel;
        private readonly static EdmComplexType ComplexType;
        private static readonly IEdmStructuralProperty StringProperty;
        static ODataAtomPropertyAndValueDeserializerTests()
        {
            EdmModel = new EdmModel();
            ComplexType = new EdmComplexType("TestNamespace", "TestComplexType");
            StringProperty = ComplexType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            EdmModel.AddElement(ComplexType);
        }

        [Fact]
        public void TopLevelPropertyWithContentAndNullAttributeShouldBeNull()
        {
            // ODataLib ignores content of properties with m:null='true' - relaxing change from V2 server
            const string payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><adsm:value adsm:null='true' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">Foo</adsm:value>";
            var odataProperty = ReadStringPropertyUnderServerKnob(payload);
            odataProperty.Value.Should().BeNull();
        }

        [Fact]
        public void TopLevelPropertyWithElementContentAndNullAttributeShouldBeNull()
        {
            // ODataLib ignores content of properties with m:null='true' - relaxing change from V2 server
            const string payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?><adsm:value adsm:null='true' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\"><some/></adsm:value>";
            var odataProperty = ReadStringPropertyUnderServerKnob(payload);
            odataProperty.Value.Should().BeNull();
        }

        private static ODataProperty ReadStringPropertyUnderServerKnob(string payload)
        {
            MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(payload));
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings();
            settings.EnableODataServerBehavior();
            ODataAtomInputContext context = new ODataAtomInputContext(ODataFormat.Atom, memoryStream, Encoding.UTF8, settings, false /*readingResponse*/, true /*sync*/, EdmModel, null);
            var deserializer = new ODataAtomPropertyAndValueDeserializer(context);
            return deserializer.ReadTopLevelProperty(StringProperty, StringProperty.Type);
        }
    }
}
