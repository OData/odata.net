//---------------------------------------------------------------------
// <copyright file="PropertyAndValueAtomReaderIntegrationTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.IntegrationTests.Reader.Atom
{
    public class PropertyAndValueAtomReaderIntegrationTests
    {
        [Fact]
        public void ReadingPayloadInt64SingleDoubleDecimalWithoutSuffix()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);
            entityType.AddStructuralProperty("FloatId", EdmPrimitiveTypeKind.Single, false);
            entityType.AddStructuralProperty("DoubleId", EdmPrimitiveTypeKind.Double, false);
            entityType.AddStructuralProperty("DecimalId", EdmPrimitiveTypeKind.Decimal, false);
            
            EdmComplexType complexType = new EdmComplexType("NS", "MyTestComplexType");
            complexType.AddStructuralProperty("CLongId", EdmPrimitiveTypeKind.Int64, false);
            complexType.AddStructuralProperty("CFloatId", EdmPrimitiveTypeKind.Single, false);
            complexType.AddStructuralProperty("CDoubleId", EdmPrimitiveTypeKind.Double, false);
            complexType.AddStructuralProperty("CDecimalId", EdmPrimitiveTypeKind.Decimal, false);
            model.AddElement(complexType);
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);
            entityType.AddStructuralProperty("ComplexProperty", complexTypeRef);

            entityType.AddStructuralProperty("LongNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt64(false))));
            entityType.AddStructuralProperty("FloatNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetSingle( false))));
            entityType.AddStructuralProperty("DoubleNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDouble(false))));
            entityType.AddStructuralProperty("DecimalNumbers", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetDecimal(false))));
            model.AddElement(entityType);
            
            // Payload with an entry and an inner feed, where the inner feed has a delta link.
            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id/>
                    <title/>
                    <updated>2013-01-22T01:09:20Z</updated>
                    <author>
                        <name/>
                    </author>
                    <content type=""application/xml"">
                    <m:properties>
                       <d:LongId>12</d:LongId>
                       <d:FloatId>34.98</d:FloatId>
                       <d:DoubleId>56.01</d:DoubleId>
                       <d:DecimalId>78.62</d:DecimalId>
                       <d:ComplexProperty> 
                         <d:CLongId m:type=""Int64"">1</d:CLongId>
                         <d:CFloatId m:type=""Single"">1.0</d:CFloatId>
                         <d:CDoubleId m:type=""Double"">-1.0</d:CDoubleId>
                         <d:CDecimalId m:type=""Decimal"">0.0</d:CDecimalId>
                       </d:ComplexProperty>
                       <d:LongNumbers m:type=""Collection(Int64)"">
                         <m:element>-1</m:element>
                         <m:element>-9223372036854775808</m:element>
                         <m:element>9223372036854775807</m:element>
                       </d:LongNumbers>
                       <d:FloatNumbers m:type=""Collection(Single)"">
                         <m:element>-1.0</m:element>
                         <m:element>-3.40282347E+38</m:element>
                         <m:element>3.40282347E+38</m:element>
                         <m:element>INF</m:element>
                         <m:element>-INF</m:element>
                         <m:element>NaN</m:element>
                       </d:FloatNumbers>
                       <d:DoubleNumbers m:type=""Collection(Double)"">
                         <m:element>1.0</m:element>
                         <m:element>-1.7976931348623157E+308</m:element>
                         <m:element>1.7976931348623157E+308</m:element>
                         <m:element>INF</m:element>
                         <m:element>-INF</m:element>
                         <m:element>NaN</m:element>
                       </d:DoubleNumbers>
                       <d:DecimalNumbers m:type=""Collection(Decimal)"">
                         <m:element>-1.0</m:element>
                         <m:element>-79228162514264337593543950335</m:element>
                         <m:element>79228162514264337593543950335</m:element>
                       </d:DecimalNumbers>
                     </m:properties> 
                    </content>
                </entry>
                ";

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "LongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(12L, "value should be in correct type.");
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(34.98f, "value should be in correct type.");
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(56.01d, "value should be in correct type.");
            entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(78.62m, "value should be in correct type.");
            
            var complextProperty = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "ComplexProperty", StringComparison.OrdinalIgnoreCase)).Value as ODataComplexValue;
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CLongId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1L, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CFloatId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(1.0F, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDoubleId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(-1.0D, "value should be in correct type.");
            complextProperty.Properties.FirstOrDefault(s => string.Equals(s.Name, "CDecimalId", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo(0.0M, "value should be in correct type.");

            var longCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "LongNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            longCollection.Items.Should().Contain(-1L).And.Contain(long.MinValue).And.Contain(long.MaxValue);
            var floatCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "FloatNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            floatCollection.Items.Should().Contain(-1F).And.Contain(float.MinValue).And.Contain(float.MaxValue).And.Contain(float.PositiveInfinity).And.Contain(float.NegativeInfinity).And.Contain(float.NaN);
            var doubleCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DoubleNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            doubleCollection.Items.Should().Contain(1.0D).And.Contain(double.MinValue).And.Contain(double.MaxValue).And.Contain(double.PositiveInfinity).And.Contain(double.NegativeInfinity).And.Contain(double.NaN);
            var decimalCollection = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "DecimalNumbers", StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            decimalCollection.Items.Should().Contain(-1.0M).And.Contain(decimal.MinValue).And.Contain(decimal.MaxValue);
        }

        [Fact]
        public void ReadNullableCollectionValueAtom()
        {
            EdmModel model = new EdmModel();
            EdmEntityType entityType = new EdmEntityType("NS", "MyTestEntity");
            EdmStructuralProperty key = entityType.AddStructuralProperty("LongId", EdmPrimitiveTypeKind.Int64, false);
            entityType.AddKeys(key);

            entityType.AddStructuralProperty(
                "NullableIntNumbers",
                new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetInt32(true))));
            model.AddElement(entityType);

            // Payload with an entry and an inner feed, where the inner feed has a delta link.
            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id/>
                    <title/>
                    <updated>2013-01-22T01:09:20Z</updated>
                    <author>
                        <name/>
                    </author>
                    <content type=""application/xml"">
                    <m:properties>
                       <d:NullableIntNumbers m:type=""Collection(Int32)"">
                         <m:element>0</m:element>
                         <m:element m:null=""true""></m:element>
                         <m:element>1</m:element>
                         <m:element>2</m:element>
                       </d:NullableIntNumbers>
                     </m:properties> 
                    </content>
                </entry>";

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            var intCollection = entry.Properties.FirstOrDefault(
                s => string.Equals(
                    s.Name,
                    "NullableIntNumbers",
                    StringComparison.OrdinalIgnoreCase)).Value.As<ODataCollectionValue>();
            var list = new List<int?>();
            foreach (var val in intCollection.Items)
            {
                list.Add(val as int?);
            }

            Assert.Equal(0, list[0]);
            Assert.Null(list[1]);
            Assert.Equal(1, (int)list[2]);
            Assert.Equal(2, (int)list[3]);
        }

        [Fact]
        public void ReadingPayloadOpenComplexTypeAtom()
        {
            EdmModel model = new EdmModel();

            EdmEntityType entityType = new EdmEntityType("NS", "Person");
            entityType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);

            EdmComplexType complexType = new EdmComplexType("NS", "OpenAddress", null, false, true);
            complexType.AddStructuralProperty("CountryRegion", EdmPrimitiveTypeKind.String, false);
            EdmComplexTypeReference complexTypeRef = new EdmComplexTypeReference(complexType, true);

            entityType.AddStructuralProperty("Address", complexTypeRef);

            model.AddElement(complexType);
            model.AddElement(entityType);

            const string payload = @"
                <entry xmlns=""http://www.w3.org/2005/Atom"" xmlns:d=""http://docs.oasis-open.org/odata/ns/data"" xmlns:m=""http://docs.oasis-open.org/odata/ns/metadata"">
                    <id/>
                    <title/>
                    <updated>2013-01-22T01:09:20Z</updated>
                    <author>
                        <name/>
                    </author>
                    <content type=""application/xml"">
                    <m:properties>
                       <d:Id>0</d:Id>
                       <d:Address> 
                        <d:CountryRegion m:type=""String"">China</d:CountryRegion>
                        <d:City m:type=""String"">Shanghai</d:City>
                       </d:Address>
                     </m:properties> 
                    </content>
                </entry>
                ";

            ODataEntry entry = null;

            this.ReadEntryPayload(model, payload, entityType, reader => { entry = entry ?? reader.Item as ODataEntry; });
            Assert.NotNull(entry);

            var address = entry.Properties.FirstOrDefault(s => string.Equals(s.Name, "Address", StringComparison.OrdinalIgnoreCase)).Value as ODataComplexValue;
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "CountryRegion", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("China", "value should be in correct type.");
            address.Properties.FirstOrDefault(s => string.Equals(s.Name, "City", StringComparison.OrdinalIgnoreCase)).Value.ShouldBeEquivalentTo("Shanghai", "value should be in correct type.");
        }
        
        private void ReadEntryPayload(EdmModel userModel, string payload, IEdmEntityType entityType, Action<ODataReader> action)
        {
            var message = new InMemoryMessage() { Stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)) };
            message.SetHeader("Content-Type", "application/atom+xml;type=entry");
            var readerSettings = new ODataMessageReaderSettings { DisableMessageStreamDisposal = false, EnableAtom = true};
            using (var msgReader = new ODataMessageReader((IODataResponseMessage)message, readerSettings, userModel))
            {
                var reader = msgReader.CreateODataEntryReader(entityType);
                while (reader.Read())
                {
                    action(reader);
                }
            }
        }
    }
}
