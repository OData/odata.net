//---------------------------------------------------------------------
// <copyright file="DSPResourceSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Stubs.DataServiceProvider
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.OData.Service.Providers;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Microsoft.Spatial;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using AstoriaUnitTests.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using t = System.Data.Test.Astoria;

    /// <summary>
    /// Supported formats
    /// </summary>
    public enum DSPResourceSerializerFormat
    {
        Atom,
        Json
    }

    /// <summary>
    /// Simple serializer to convert a DSPResource to the wire format. For generating payloads for insert/update tests.
    /// </summary>
    public abstract class DSPResourceSerializer : IDisposable
    {
        /// <summary>
        /// GML 'Point' element name in serializer's output.
        /// </summary>
        protected const string GmlPoint = "Point";

        /// <summary>
        /// GML 'LineString' element name in serializer's output.
        /// </summary>
        protected const string GmlLineString = "LineString";

        /// <summary>
        /// Edm type name for GML type GeographyLineString.
        /// </summary>
        protected const string Gml_Edm_GeographyLineStringName = "Edm.GeographyLineString";

        /// <summary>
        /// Edm type name for GML type GeographyPoint.
        /// </summary>
        protected const string Gml_Edm_GeographyPointName = "Edm.GeographyPoint";

        /// <summary>Element containing property values when 'content' is used for media link entries</summary>
        protected const string PropertiesElementName = "properties";

        /// <summary>
        /// Helper method which converts the mime type into the serialization format
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static DSPResourceSerializerFormat SerializerFormatFromMimeType(string mimeType)
        {
            if (string.Equals(mimeType, UnitTestsUtil.AtomFormat, StringComparison.OrdinalIgnoreCase))
            {
                return DSPResourceSerializerFormat.Atom;
            }
            else if (string.Equals(mimeType, UnitTestsUtil.MimeApplicationXml, StringComparison.OrdinalIgnoreCase))
            {
                return DSPResourceSerializerFormat.Atom;
            }
            else if (string.Equals(mimeType, UnitTestsUtil.MimeTextXml, StringComparison.OrdinalIgnoreCase))
            {
                return DSPResourceSerializerFormat.Atom;
            }
            else if (string.Equals(mimeType, UnitTestsUtil.JsonLightMimeType, StringComparison.OrdinalIgnoreCase))
            {
                return DSPResourceSerializerFormat.Json;
            }
            else
            {
                // default to Atom, same as our services
                return DSPResourceSerializerFormat.Atom;
            }
        }

        /// <summary>
        /// Serializes the specified entity resource
        /// </summary>
        /// <param name="entity">The entity resource to serialize.</param>
        /// <param name="format">The format in which to serialize.</param>
        /// <param name="output">The stream to serialize to.</param>
        /// <param name="encoding">The text encoding to use for the serialization.</param>
        public static void WriteEntity(DSPResource entity, DSPResourceSerializerFormat format, Stream output, Encoding encoding)
        {
            CreateSerializerAndRun(format, output, encoding, (serializer) => { serializer.WriteEntity(entity); });
        }

        /// <summary>
        /// Serializes the specified entity resource into a string
        /// </summary>
        /// <param name="entity">The entity resource to serialize.</param>
        /// <param name="format">The format in which to serialize.</param>
        /// <returns>The entity serialized as a string</returns>
        public static string WriteEntity(DSPResource entity, DSPResourceSerializerFormat format)
        {
            return CreateSerializerAndRunIntoString(format, (serializer) => { serializer.WriteEntity(entity); });
        }

        public static void WriteProperty(ResourceProperty resourceProperty, object value, DSPResourceSerializerFormat format, Stream output, Encoding encoding)
        {
            CreateSerializerAndRun(format, output, encoding, (serializer) => { serializer.WriteProperty(resourceProperty, value); });
        }

        public static string WriteProperty(ResourceProperty resourceProperty, object value, DSPResourceSerializerFormat format)
        {
            return CreateSerializerAndRunIntoString(format, (serializer) => { serializer.WriteProperty(resourceProperty, value); });
        }

        /// <summary>
        /// Serializes a DSPResource entity
        /// </summary>
        /// <param name="entity">DSPResource instance to serialize.</param>
        public void WriteEntity(DSPResource entity)
        {
            ResourceType type = entity.ResourceType;
            if (type.ResourceTypeKind != ResourceTypeKind.EntityType)
            {
                throw new NotSupportedException("Only EntityType is currently supported.");
            }

            this.StartEntry(entity, true);
            this.WriteEntryMetadata(entity);

            this.PreWriteProperties(entity);
            this.WriteProperties(entity);
            this.PostWriteProperties();

            this.EndEntry();
        }

        public void WriteProperty(ResourceProperty resourceProperty, object value)
        {
            this.StartNonEntityPayload();
            this.BeginWriteProperty(resourceProperty);
            this.WriteValue(resourceProperty.ResourceType, value);
            this.EndWriteProperty();
            this.EndNonEntityPayload();
        }

        private static void CreateSerializerAndRun(DSPResourceSerializerFormat format, Stream output, Encoding encoding, Action<DSPResourceSerializer> action)
        {
            DSPResourceSerializer serializer = null;
            if (format == DSPResourceSerializerFormat.Atom)
            {
                serializer = new DSPResourceAtomSerializer(output, encoding);
            }
            else
            {
                Debug.Assert(format == DSPResourceSerializerFormat.Json, "Only ATOM and JSON formats are supported.");
                serializer = new DSPResourceJsonSerializer(output, encoding);
            }

            using (serializer)
            {
                action(serializer);
            }
        }

        private static string CreateSerializerAndRunIntoString(DSPResourceSerializerFormat format, Action<DSPResourceSerializer> action)
        {
            using (MemoryStream output = new MemoryStream())
            {
                CreateSerializerAndRun(format, output, Encoding.UTF8, action);
                output.Position = 0;
                return (new StreamReader(output, Encoding.UTF8)).ReadToEnd();
            }
        }

        /// <summary>
        /// Writes the properties of a DSPResource.
        /// </summary>
        /// <param name="entry">DSPResource instance to serialize.</param>
        private void WriteProperties(DSPResource entry)
        {
            ResourceType type = entry.ResourceType;
            foreach (KeyValuePair<string, object> property in entry.Properties)
            {
                ResourceProperty resourceProperty = type.Properties.FirstOrDefault(p => p.Name == property.Key);
                this.BeginWriteProperty(resourceProperty);
                this.WriteValue(resourceProperty.ResourceType, property.Value);
                this.EndWriteProperty();
            }
        }

        protected void WriteValue(ResourceType resourceType, object value)
        {
            if (value == null)
            {
                this.WriteNullValue();
            }
            else
            {
                switch (resourceType.ResourceTypeKind)
                {
                    case ResourceTypeKind.Primitive:
                        this.WritePrimitiveValue(value);
                        break;
                    case ResourceTypeKind.ComplexType:
                        this.BeginComplexValue(resourceType);
                        this.WriteProperties((DSPResource)value);
                        this.EndComplexValue();
                        break;
                    case ResourceTypeKind.Collection:
                        CollectionResourceType CollectionResourceType = ((CollectionResourceType)resourceType);
                        this.BeginCollectionPropertyValue(CollectionResourceType);
                        foreach (object itemValue in ((IEnumerable)value))
                        {
                            this.WriteCollectionItemValue(CollectionResourceType.ItemType, itemValue);
                        }
                        this.EndCollectionPropertyValue();
                        break;
                    default:
                        throw new NotSupportedException("Unsupported value type for serialization.");
                }
            }
        }

        #region IDisposable

        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        protected abstract void StartEntry(DSPResource entry, bool isTopLevel);
        protected abstract void WriteEntryMetadata(DSPResource entry);
        protected abstract void EndEntry();

        protected abstract void StartNonEntityPayload();
        protected abstract void EndNonEntityPayload();

        protected abstract void PreWriteProperties(DSPResource entry);
        protected abstract void BeginWriteProperty(ResourceProperty resourceProperty);

        protected abstract void BeginComplexValue(ResourceType resourceType);
        protected abstract void EndComplexValue();

        protected abstract void BeginCollectionPropertyValue(CollectionResourceType CollectionResourceType);
        protected abstract void WriteCollectionItemValue(ResourceType itemType, object itemValue);
        protected abstract void EndCollectionPropertyValue();

        protected abstract void EndWriteProperty();
        protected abstract void PostWriteProperties();

        protected abstract void WriteNullValue();
        protected abstract void WritePrimitiveValue(object value);
    }

    /// <summary>
    /// Simple serializer to convert a DSPResource to Atom format. For generating payloads for insert/update tests.
    /// </summary>
    public class DSPResourceAtomSerializer : DSPResourceSerializer
    {
        /// <summary>XML element name to mark content element in Atom.</summary>
        private const string AtomContentElementName = "content";

        /// <summary>XML element name to mark entry element in Atom.</summary>
        private const string AtomEntryElementName = "entry";

        /// <summary> Schema Namespace prefix For xmlns.</summary>
        private const string XmlnsNamespacePrefix = "xmlns";

        /// <summary> Schema Namespace prefix For xml.</summary>
        private const string XmlNamespacePrefix = "xml";

        /// <summary> Schema Namespace Prefix For DataWeb.</summary>
        private const string DataWebNamespacePrefix = "d";

        /// <summary>XML attribute name to specify the type of the element.</summary>
        private const string AtomTypeAttributeName = "type";

        /// <summary>Atom attribute that indicates the actual location for an entry's content.</summary>
        private const string AtomContentSrcAttributeName = "src";


        /// <summary> Atom attribute which indicates the null value for the element.</summary>
        private const string AtomNullAttributeName = "null";

        /// <summary>'true' literal, as used in XML.</summary>
        private const string XmlTrueLiteral = "true";

        /// <summary>'adsm' - namespace prefix for DataWebMetadataNamespace.</summary>
        private const string DataWebMetadataNamespacePrefix = "m";

        /// <summary>'category' - XML element name for ATOM 'category' element for entries.</summary>
        private const string AtomCategoryElementName = "category";

        /// <summary>'term' - XML attribute name for ATOM 'term' attribute for categories.</summary>
        private const string AtomCategoryTermAttributeName = "term";

        /// <summary>'scheme' - XML attribute name for ATOM 'scheme' attribute for categories.</summary>
        private const string AtomCategorySchemeAttributeName = "scheme";

        /// <summary>'element' - XML element name for item in the collection property.</summary>
        private const string AtomCollectionItemElementName = "element";

        private const string GmlPosition = "pos";


        private const string GmlPrefix = "Gml";

        /// <summary>
        /// writer to serialize the payload.
        /// </summary>
        private XmlWriter writer;

        /// <summary>
        /// Creates an Atom serializer.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="encoding">Stream encoding.</param>
        public DSPResourceAtomSerializer(Stream output, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            writer = XmlWriter.Create(output, t.XmlUtil.CreateXmlWriterSettings(encoding));
        }

        public override void Dispose()
        {
            base.Dispose();
            writer.Close();
        }

        protected override void StartEntry(DSPResource entry, bool isTopLevel)
        {
            this.writer.WriteStartElement(AtomEntryElementName, UnitTestsUtil.AtomNamespace.NamespaceName);

            if (isTopLevel)
            {
                this.WriteDefaultNamespaces();
            }
        }

        private void WriteDefaultNamespaces()
        {
            this.writer.WriteAttributeString(
                DataWebNamespacePrefix,
                XNamespace.Xmlns.NamespaceName,
                UnitTestsUtil.DataNamespace.NamespaceName);

            this.writer.WriteAttributeString(
                DataWebMetadataNamespacePrefix,
                XNamespace.Xmlns.NamespaceName,
                UnitTestsUtil.MetadataNamespace.NamespaceName);
        }

        protected override void WriteEntryMetadata(DSPResource entry)
        {
            // <category term="type" scheme="odatascheme"/>
            this.writer.WriteStartElement(
                AtomCategoryElementName,
                UnitTestsUtil.AtomNamespace.NamespaceName);

            this.writer.WriteAttributeString(
                AtomCategoryTermAttributeName,
                entry.ResourceType.FullName);

            this.writer.WriteAttributeString(
                AtomCategorySchemeAttributeName,
                UnitTestsUtil.SchemeNamespace.NamespaceName);

            this.writer.WriteEndElement();
        }

        protected override void EndEntry()
        {
            this.writer.WriteEndElement();
        }

        protected override void StartNonEntityPayload()
        {
            // Do nothing
        }

        protected override void EndNonEntityPayload()
        {
            // Do nothing
        }

        protected override void PreWriteProperties(DSPResource entry)
        {
            // For MLEs, the <m:properties> element is outside of the content element and we can omit the content element.
            if (!entry.ResourceType.IsMediaLinkEntry)
            {
                // <content type="type">
                this.writer.WriteStartElement(
                    AtomContentElementName,
                    UnitTestsUtil.AtomNamespace.NamespaceName);

                this.writer.WriteAttributeString(
                    AtomTypeAttributeName,
                    UnitTestsUtil.MimeApplicationXml);
            }

            // <m:properties>
            this.writer.WriteStartElement(
                PropertiesElementName,
                UnitTestsUtil.MetadataNamespace.NamespaceName);
        }

        protected override void BeginWriteProperty(ResourceProperty metadata)
        {
            // <m:prop type="type">
            this.writer.WriteStartElement(
                metadata.Name,
                UnitTestsUtil.DataNamespace.NamespaceName);

            if (metadata.ResourceType != ResourceType.GetPrimitiveResourceType(typeof(string)))
            {
                this.writer.WriteAttributeString(
                    AtomTypeAttributeName,
                    UnitTestsUtil.MetadataNamespace.NamespaceName,
                    metadata.ResourceType.FullName);
            }
        }

        protected override void BeginComplexValue(ResourceType resourceType)
        {
            // Do nothing.
        }

        protected override void EndComplexValue()
        {
            // Do nothing.
        }

        protected override void BeginCollectionPropertyValue(CollectionResourceType CollectionResourceType)
        {
            // Do nothing
        }

        protected override void WriteCollectionItemValue(ResourceType itemType, object itemValue)
        {
            this.writer.WriteStartElement(AtomCollectionItemElementName, UnitTestsUtil.MetadataNamespace.NamespaceName);
            // No need to write item type as they are the same for each item for now

            this.WriteValue(itemType, itemValue);

            this.writer.WriteEndElement();
        }

        protected override void EndCollectionPropertyValue()
        {
            // Do nothing
        }

        protected override void EndWriteProperty()
        {
            // </m:prop/complexprop>
            this.writer.WriteEndElement();
        }

        protected override void PostWriteProperties()
        {
            // </m:properties>
            this.writer.WriteEndElement();

            // </content>
            this.writer.WriteEndElement();
        }

        protected override void WriteNullValue()
        {
            // m:null="true"
            this.writer.WriteAttributeString(
                AtomNullAttributeName,
                UnitTestsUtil.MetadataNamespace.NamespaceName,
                XmlTrueLiteral);
        }

        protected override void WritePrimitiveValue(object value)
        {
            WritePrimitiveXmlValue(this.writer, value);
        }

        private static void WritePrimitiveXmlValue(XmlWriter writer, object value)
        {
            Debug.Assert(value != null, "value != null");
            string result;

            // First see if this type can handle writing itself directly to Xml
            if (!TryWriteValueToXml(writer, value))
            {
                // Otherwise just convert to string and then write the value
                if (!TryXmlPrimitiveToString(value, out result))
                {
                    throw new InvalidOperationException("Cannot convert Value.");
                }

                writer.WriteString(result);
            }
        }

        private static bool TryWriteValueToXml(XmlWriter writer, object value)
        {
            Geography geography = value as Geography;
            if (geography != null)
            {
                WriteGeography(geography, writer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// This mimics what is in the product, but keeping this separate implementation so if the product changes this test will pick it up.
        /// </summary>
        /// <param name="geography"></param>
        /// <param name="writer"></param>
        private static void WriteGeography(Geography geography, XmlWriter writer)
        {
            Assert.IsNotNull(geography, "Can't write a null geography.");
            Assert.IsNotNull(writer, "XmlWriter is null.");

            Type type = geography.GetType();

            if (typeof(GeographyPoint).IsAssignableFrom(type))
            {
                writer.WriteStartElement(GmlPrefix, GmlPoint, UnitTestsUtil.GmlNamespace.NamespaceName);
                writer.WriteStartElement(GmlPrefix, GmlPosition, UnitTestsUtil.GmlNamespace.NamespaceName);
                writer.WriteValue(ToPointString((GeographyPoint)geography));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            else if (typeof(GeographyLineString).IsAssignableFrom(type))
            {
                writer.WriteStartElement(GmlPrefix, GmlLineString, UnitTestsUtil.GmlNamespace.NamespaceName);
                for (int i = 0; i < ((GeographyLineString)geography).Points.Count; ++i)
                {
                    writer.WriteElementString(GmlPrefix, GmlPosition, UnitTestsUtil.GmlNamespace.NamespaceName, ToPointString(((GeographyLineString)geography).Points[i]));
                }
                writer.WriteEndElement();
            }
            else
            {
                Assert.Fail("DSPResourceAtomSerializer does not support writing the Geography type '{0}'. If this is a valid type, the WriteGeography method needs to be updated.", type.Name);
            }
        }

        /// <summary>
        /// Convert an array of points into a string representation
        /// </summary>
        /// <param name="points">array of points</param>
        /// <returns>the string representation</returns>
        private static string ToPointString(params GeographyPoint[] points)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < points.Length; ++i)
            {
                sb.Append(points[i].Latitude);
                sb.Append(' ');
                sb.Append(points[i].Longitude);

                if (i != points.Length - 1)
                {
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }

        /// <summary>Converts the specified value to a serializable string for XML content.</summary>
        /// <param name="value">Non-null value to convert.</param>
        /// <param name="result">The specified value converted to a serializable string for XML content. </param>
        /// <returns>boolean value indicating conversion successful conversion</returns>
        private static bool TryXmlPrimitiveToString(object value, out string result)
        {
            Debug.Assert(value != null, "value != null");
            result = null;

            Type valueType = value.GetType();
            valueType = Nullable.GetUnderlyingType(valueType) ?? valueType;

            if (typeof(String) == valueType)
            {
                result = (string)value;
            }
            else if (typeof(Boolean) == valueType)
            {
                result = XmlConvert.ToString((bool)value);
            }
            else if (typeof(Byte) == valueType)
            {
                result = XmlConvert.ToString((byte)value);
            }
            else if (typeof(DateTime) == valueType)
            {
                result = XmlConvert.ToString((DateTime)value, XmlDateTimeSerializationMode.RoundtripKind);
            }
            else if (typeof(Decimal) == valueType)
            {
                result = XmlConvert.ToString((decimal)value);
            }
            else if (typeof(Double) == valueType)
            {
                result = XmlConvert.ToString((double)value);
            }
            else if (typeof(Guid) == valueType)
            {
                result = value.ToString();
            }
            else if (typeof(Int16) == valueType)
            {
                result = XmlConvert.ToString((Int16)value);
            }
            else if (typeof(Int32) == valueType)
            {
                result = XmlConvert.ToString((Int32)value);
            }
            else if (typeof(Int64) == valueType)
            {
                result = XmlConvert.ToString((Int64)value);
            }
            else if (typeof(SByte) == valueType)
            {
                result = XmlConvert.ToString((SByte)value);
            }
            else if (typeof(Single) == valueType)
            {
                result = XmlConvert.ToString((Single)value);
            }
            else if (typeof(byte[]) == valueType)
            {
                byte[] byteArray = (byte[])value;
                result = Convert.ToBase64String(byteArray);
            }
            else if (typeof(System.Data.Linq.Binary) == valueType)
            {
                // DEVNOTE: Client handles binary differently.
                return TryXmlPrimitiveToString(((System.Data.Linq.Binary)value).ToArray(), out result);
            }
            else if (typeof(System.Xml.Linq.XElement) == valueType)
            {
                result = ((System.Xml.Linq.XElement)value).ToString(System.Xml.Linq.SaveOptions.None);
            }
            else
            {
                result = null;
                return false;
            }

            Debug.Assert(result != null, "result != null");
            return true;
        }
    }

    /// <summary>
    /// Simple serializer to convert a DSPResource to Json format. For generating payloads for insert/update tests.
    /// </summary>
    public class DSPResourceJsonSerializer : DSPResourceSerializer
    {
        /// <summary>metadata element name in json payload.</summary>
        private const string JsonMetadataString = "__metadata";

        /// <summary>type element name in json payload.</summary>
        private const string JsonTypeString = "type";

        /// <summary>
        /// 'coordinates' element name in Serializer's output.
        /// </summary>
        private const string JsonCoOrdinatesString = "coordinates";

        /// <summary>
        /// 'crs' element name in Serializer's output.
        /// </summary>
        private const string JsonCrsString = "crs";

        /// <summary>
        /// 'EPSG' element template to form EPSG element's value name in Serializer's output.
        /// </summary>
        private const string JsonEPSGValueStringFormat = "EPSG:{0}";

        /// <summary>
        /// 'name' element name in Serializer's output.
        /// </summary>
        private const string JsonNameString = "name";

        /// <summary>
        /// writer to serialize the payload.
        /// </summary>
        private JsonWriter writer;

        /// <summary>
        /// Creates a Json serializer.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="encoding">Stream encoding.</param>
        public DSPResourceJsonSerializer(Stream output, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            writer = new JsonWriter(new StreamWriter(output, encoding));
        }

        public override void Dispose()
        {
            base.Dispose();
            writer.Flush();
        }

        protected override void StartEntry(DSPResource entry, bool isTopLevel)
        {
            // {
            this.writer.StartObjectScope();
        }

        /// <summary>
        /// Write metadata information for the entry.
        /// </summary>
        /// <param name="entry">Entry to write metadata for.</param>
        protected override void WriteEntryMetadata(DSPResource entry)
        {
            Debug.Assert(entry != null, "entry != null");

            // __metadata : { type: "Type" }
            if (entry.ResourceType != null)
            {
                this.writer.WriteName(JsonMetadataString);
                this.writer.StartObjectScope();
                this.writer.WriteName(JsonTypeString);
                this.writer.WriteValue(entry.ResourceType.FullName);
                this.writer.EndScope();
            }
        }

        protected override void EndEntry()
        {
            // }
            this.writer.EndScope();
        }

        protected override void StartNonEntityPayload()
        {
            // {
            this.writer.StartObjectScope();
        }

        protected override void EndNonEntityPayload()
        {
            // }
            this.writer.EndScope();
        }

        protected override void PreWriteProperties(DSPResource entry)
        {
            // Do nothing.
        }

        protected override void BeginWriteProperty(ResourceProperty metadata)
        {
            this.writer.WriteName(metadata.Name);
        }

        protected override void BeginComplexValue(ResourceType resourceType)
        {
            // {
            this.writer.StartObjectScope();

            // __metadata : { Type : "typename" }
            this.writer.WriteName(JsonMetadataString);
            this.writer.StartObjectScope();

            this.writer.WriteName(JsonTypeString);
            this.writer.WriteValue(resourceType.FullName);

            this.writer.EndScope();
        }

        protected override void EndComplexValue()
        {
            // }
            this.writer.EndScope();
        }

        protected override void BeginCollectionPropertyValue(CollectionResourceType CollectionResourceType)
        {
            // {
            this.writer.StartObjectScope();
            // __metadata : { Type : "typename" }
            this.writer.WriteName(JsonMetadataString);
            this.writer.StartObjectScope();

            this.writer.WriteName(JsonTypeString);
            this.writer.WriteValue(CollectionResourceType.FullName);

            this.writer.EndScope();

            // "results":
            this.writer.WriteDataArrayName();
            // [
            this.writer.StartArrayScope();
        }

        protected override void WriteCollectionItemValue(ResourceType itemType, object itemValue)
        {
            this.WriteValue(itemType, itemValue);
        }

        protected override void EndCollectionPropertyValue()
        {
            // ]
            this.writer.EndScope();
            // }
            this.writer.EndScope();
        }

        protected override void EndWriteProperty()
        {
            // Do nothing.
        }

        protected override void PostWriteProperties()
        {
            // Do nothing.
        }

        protected override void WriteNullValue()
        {
            this.writer.WriteValue((string)null);
        }

        /// <summary>
        /// Attempts to convert the specified primitive value to a serializable string.
        /// </summary>
        /// <param name="value">Non-null value to convert.</param>
        protected override void WritePrimitiveValue(object value)
        {
            WritePrimitiveJsonValue(this.writer, value);
        }

        /// <summary>
        /// Attempts to convert the specified primitive value to a serializable string.
        /// </summary>
        /// <param name="value">Non-null value to convert.</param>
        private static void WritePrimitiveJsonValue(JsonWriter writer, object value)
        {
            Debug.Assert(value != null, "value != null");

            Type valueType = value.GetType();
            if (typeof(String) == valueType)
            {
                writer.WriteValue((string)value);
            }
            else if (typeof(System.Xml.Linq.XElement) == valueType)
            {
                writer.WriteValue(((System.Xml.Linq.XElement)value).ToString(System.Xml.Linq.SaveOptions.None));
            }
            else if (typeof(SByte) == valueType)
            {
                writer.WriteValue((SByte)value);
            }
            else if (typeof(Boolean) == value.GetType())
            {
                writer.WriteValue((bool)value);
            }
            else if (typeof(Byte) == value.GetType())
            {
                writer.WriteValue((byte)value);
            }
            else if (typeof(DateTime) == value.GetType())
            {
                writer.WriteValue((DateTime)value);
            }
            else if (typeof(Decimal) == value.GetType())
            {
                writer.WriteValue((Decimal)value);
            }
            else if (typeof(Double) == value.GetType())
            {
                writer.WriteValue((Double)value);
            }
            else if (typeof(Guid) == value.GetType())
            {
                writer.WriteValue((Guid)value);
            }
            else if (typeof(Int16) == value.GetType())
            {
                writer.WriteValue((Int16)value);
            }
            else if (typeof(Int32) == value.GetType())
            {
                writer.WriteValue((Int32)value);
            }
            else if (typeof(Int64) == value.GetType())
            {
                writer.WriteValue((Int64)value);
            }
            else if (typeof(Single) == value.GetType())
            {
                writer.WriteValue((Single)value);
            }
            else if (typeof(byte[]) == value.GetType())
            {
                byte[] byteArray = (byte[])value;
                string result = Convert.ToBase64String(byteArray, Base64FormattingOptions.None);
                writer.WriteValue(result);
            }
            else if (!TryWriteGeography(value, writer))
            {
                Debug.Assert(typeof(System.Data.Linq.Binary) == value.GetType(), "typeof(Binary) == value.GetType() (" + value.GetType() + ")");
                WritePrimitiveJsonValue(writer, ((System.Data.Linq.Binary)value).ToArray());
            }
        }

        /// <summary>
        /// Tries to write out a possible geography value into json
        /// </summary>
        /// <param name="geographyValue">The geography value to serialize into Json</param>
        /// <param name="writer">The Json writer being used to write out a Json payload</param>
        /// <returns>true, if the value is a geography object that is supported, false otherwise.</returns>
        private static bool TryWriteGeography(object geographyValue, JsonWriter writer)
        {
            Assert.IsNotNull(geographyValue, "Can't write a null geography.");
            Assert.IsNotNull(writer, "JsonWriter is null.");

            Type type = geographyValue.GetType();

            if (typeof(GeographyPoint).IsAssignableFrom(type))
            {
                GeographyPoint point = (GeographyPoint)geographyValue;
                WriteGeographyPoint(point, writer);
                return true;
            }
            else if (typeof(GeographyLineString).IsAssignableFrom(type))
            {
                GeographyLineString lineString = (GeographyLineString)geographyValue;
                WriteGeographyLineString(lineString, writer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes out a GeographyLineString value in geojson format.
        /// {
        ///      "__metadata": {"type": "Edm.GeographyLineString"}, 
        ///      { "type" :"LineString",
        ///          "coordinates": { [-122.1202778,47.6741667] } ,
        ///          "crs":{"type":"name","properties":{"name":"EPSG:4326"}}
        ///      }
        /// }
        /// </summary>
        /// <param name="geographyLineStringValue">The geography value to serialize into Json</param>
        /// <param name="writer">The Json writer being used to write out a Json payload</param>
        private static void WriteGeographyLineString(GeographyLineString lineString, JsonWriter writer)
        {
            // { 
            writer.StartObjectScope();

            //  "__metadata":
            writer.WriteName(JsonMetadataString);
            //      {
            writer.StartObjectScope();

            // "type"
            writer.WriteName(JsonTypeString);

            // "Edm.GeographyLineString"
            writer.WriteValue(Gml_Edm_GeographyLineStringName);

            //      }
            writer.EndScope();

            //  "type":"LineString",
            writer.WriteName(JsonTypeString);
            writer.WriteValue(GmlLineString);

            //  "coordinates":
            writer.WriteName(JsonCoOrdinatesString);

            // [
            writer.StartArrayScope();
            foreach (var point in lineString.Points)
            {
                WritePointCoordinates(writer, point);
            }

            // ]
            writer.EndScope();

            // 	"crs": {"type": "name", "properties": {"name": "EPSG:4326"}}
            WriteCrsElement(writer, lineString.CoordinateSystem.EpsgId);

            // }
            writer.EndScope();
        }

        /// <summary>
        /// Writes out a GeographyPoint value in geojson format.
        /// {
        ///      "__metadata":{"type":"Edm.GeographyPoint"},
        ///      "type":"Point",
        ///      "coordinates":[Lattitude,Longitude],
        ///      "crs":{"type":"name","properties":{"name":"EPSG:EPSGValue"}}
        /// }
        /// </summary>
        /// <param name="geographyLineStringValue">The geography value to serialize into Json</param>
        /// <param name="writer">The Json writer being used to write out a Json payload</param>
        private static void WriteGeographyPoint(GeographyPoint point, JsonWriter writer)
        {
            // see http://geojson.org/geojson-spec.html#id9
            // {
            writer.StartObjectScope();

            //  "__metadata":
            writer.WriteName(JsonMetadataString);

            //      {
            writer.StartObjectScope();

            // "type"
            writer.WriteName(JsonTypeString);

            // "Edm.GeographyPoint"
            writer.WriteValue(Gml_Edm_GeographyPointName);

            //      }
            writer.EndScope();

            //  "type":"Point",
            writer.WriteName(JsonTypeString);
            writer.WriteValue(GmlPoint);

            //  "coordinates":[-122.1202778,47.6741667],
            writer.WriteName(JsonCoOrdinatesString);
            WritePointCoordinates(writer, point);

            // 	"crs": {"type": "name", "properties": {"name": "EPSG:4326"}}
            WriteCrsElement(writer, point.CoordinateSystem.EpsgId);

            // }
            writer.EndScope();
        }

        /// <summary>
        /// Writes out the Co-ordinate reference system element in JSON
        /// http://en.wikipedia.org/wiki/Coordinate_reference_system
        /// </summary>
        /// <param name="writer">The Json Writer being used to write out the results.</param>
        /// <param name="epsgId">the EpsgId value to serialize</param>
        private static void WriteCrsElement(JsonWriter writer, int? epsgId)
        {
            //  "crs"
            writer.WriteName(JsonCrsString);

            // { 
            writer.StartObjectScope();

            // "type"
            writer.WriteName(JsonTypeString);

            // :"name"
            writer.WriteValue(JsonNameString);

            // "properties"
            writer.WriteName(PropertiesElementName);

            // :    {
            writer.StartObjectScope();

            // "name":
            writer.WriteName(JsonNameString);

            string epsgidValue = epsgId.HasValue ? epsgId.Value.ToString() : "0";

            // "EPSG:<EpsgIdValue>"
            writer.WriteValue(String.Format(JsonEPSGValueStringFormat, epsgidValue));

            //      }
            writer.EndScope();

            //    }
            writer.EndScope();
        }

        /// <summary>
        /// Writes out a geography point's Co-ordinates in JSON
        /// </summary>
        /// <param name="writer">The Json Writer being used to write out the results.</param>
        /// <param name="point">The Point to serialize into JSON</param>
        private static void WritePointCoordinates(JsonWriter writer, GeographyPoint point)
        {
            writer.StartArrayScope();
            writer.WriteValue(point.Longitude);
            writer.WriteValue(point.Latitude);
            writer.EndScope();
        }
    }

    /// <summary>
    /// Json text writer
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Original writer is disposed of?")]
    internal sealed class JsonWriter
    {
        /// <summary> const tick value for caculating tick values</summary>
        internal static readonly long DatetimeMinTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        /// <summary> Json datetime format </summary>
        private const string JsonDateTimeFormat = @"\/Date({0})\/";

        /// <summary>Text used to start a data object wrapper in JSON.</summary>
        private const string JsonDataWrapper = "\"d\" : ";

        /// <summary>'true' literal constant</summary>
        private const string XmlTrueLiteral = "true";

        /// <summary>'false' literal constant</summary>
        private const string XmlFalseLiteral = "false";

        /// <summary>'results' header for arrays</summary>
        private const string JsonResultsName = "results";

        /// <summary> Writer to write text into </summary>
        private readonly IndentedTextWriter writer;

        /// <summary> scope of the json text - object, array, etc</summary>
        private readonly Stack<Scope> scopes;

        /// <summary>
        /// Creates a new instance of Json writer
        /// </summary>
        /// <param name="writer">writer to which text needs to be written</param>
        public JsonWriter(TextWriter writer)
        {
            this.writer = new IndentedTextWriter(writer);
            this.scopes = new Stack<Scope>();
        }

        /// <summary>
        /// Various scope types for Json writer
        /// </summary>
        private enum ScopeType
        {
            /// <summary> array scope </summary>
            Array = 0,

            /// <summary> object scope</summary>
            Object = 1
        }

        /// <summary>
        /// End the current scope
        /// </summary>
        public void EndScope()
        {
            if (this.scopes.Count == 0)
            {
                throw new InvalidOperationException("No active scope to end.");
            }

            this.writer.WriteLine();
            this.writer.Indent--;

            Scope scope = this.scopes.Pop();
            if (scope.Type == ScopeType.Array)
            {
                this.writer.Write("]");
            }
            else
            {
                this.writer.Write("}");
            }
        }

        /// <summary>
        /// Start the array scope
        /// </summary>
        public void StartArrayScope()
        {
            this.StartScope(ScopeType.Array);
        }

        /// <summary>
        /// Write the "d" wrapper text
        /// </summary>
        public void WriteDataWrapper()
        {
            this.writer.Write(JsonDataWrapper);
        }

        /// <summary>
        /// Write the "results" header for the data array
        /// </summary>
        public void WriteDataArrayName()
        {
            this.WriteName(JsonResultsName);
        }

        /// <summary>
        /// Start the object scope
        /// </summary>
        public void StartObjectScope()
        {
            this.StartScope(ScopeType.Object);
        }

        /// <summary>
        /// Write the name for the object property
        /// </summary>
        /// <param name="name">name of the object property </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Service.Serializers.IndentedTextWriter.WriteTrimmed(System.String)", Justification = "Literals are JSON standard elements, and should not be localized")]
        public void WriteName(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (this.scopes.Count == 0)
            {
                throw new InvalidOperationException("No active scope to write into.");
            }

            if (this.scopes.Peek().Type != ScopeType.Object)
            {
                throw new InvalidOperationException("Names can only be written into Object Scopes.");
            }

            Scope currentScope = this.scopes.Peek();
            if (currentScope.Type == ScopeType.Object)
            {
                if (currentScope.ObjectCount != 0)
                {
                    this.writer.WriteTrimmed(", ");
                }

                currentScope.ObjectCount++;
            }

            this.WriteCore(QuoteJScriptString(name), true /*quotes*/);
            this.writer.WriteTrimmed(": ");
        }

        /// <summary>
        /// Write the bool value
        /// </summary>
        /// <param name="value">bool value to be written</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Service.Serializers.JsonWriter.WriteCore(System.String,System.Boolean)", Justification = "Literals are JSON standard elements, and should not be localized")]
        public void WriteValue(bool value)
        {
            this.WriteCore(value ? XmlTrueLiteral : XmlFalseLiteral, /* quotes */ false);
        }

        /// <summary>
        /// Write the int value
        /// </summary>
        /// <param name="value">int value to be written</param>
        public void WriteValue(int value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), /* quotes */ false);
        }

        /// <summary>
        /// Write the float value
        /// </summary>
        /// <param name="value">float value to be written</param>
        public void WriteValue(float value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                this.WriteCore(value.ToString(null, CultureInfo.InvariantCulture), true /*quotes*/);
            }
            else
            {
                // float.ToString() supports a max scale of six,
                // whereas float.MinValue and float.MaxValue have 8 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                this.WriteCore(XmlConvert.ToString(value), /* quotes */ false);
            }
        }

        /// <summary>
        /// Write the short value
        /// </summary>
        /// <param name="value">short value to be written</param>
        public void WriteValue(short value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), /* quotes */ false);
        }

        /// <summary>
        /// Write the long value
        /// </summary>
        /// <param name="value">long value to be written</param>
        public void WriteValue(long value)
        {
            // Since Json only supports number, we need to convert long into string to prevent data loss
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), /* quotes */ true);
        }

        /// <summary>
        /// Write the double value
        /// </summary>
        /// <param name="value">double value to be written</param>
        public void WriteValue(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
                this.WriteCore(value.ToString(null, CultureInfo.InvariantCulture), true /*quotes*/);
            }
            else
            {
                // double.ToString() supports a max scale of 14,
                // whereas float.MinValue and float.MaxValue have 16 digits scale. Hence we need
                // to use XmlConvert in all other cases, except infinity
                this.WriteCore(XmlConvert.ToString(value), /* quotes */ false);
            }
        }

        /// <summary>
        /// Write the Guid value
        /// </summary>
        /// <param name="value">double value to be written</param>
        public void WriteValue(Guid value)
        {
            this.WriteCore(value.ToString(), /* quotes */ true);
        }

        /// <summary>
        /// Write the decimal value
        /// </summary>
        /// <param name="value">decimal value to be written</param>
        public void WriteValue(decimal value)
        {
            // Since Json doesn't have decimal support (it only has one data type - number),
            // we need to convert decimal to string to prevent data loss
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), /* quotes */ true);
        }

        /// <summary>
        /// Write the DateTime value
        /// </summary>
        /// <param name="dateTime">dateTime value to be written</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Service.Serializers.JsonWriter.WriteCore(System.String,System.Boolean)", Justification = "Using standard JSON datetime format which should not be localized")]
        public void WriteValue(DateTime dateTime)
        {
            // taken from the Atlas serializer
            // Never confuse atlas serialized strings with dates
            // Serialized date: "\/Date(123)\/"
            // sb.Append(@"""\/Date(");
            // sb.Append((datetime.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
            // sb.Append(@")\/""");
            switch (dateTime.Kind)
            {
                case DateTimeKind.Local:
                    dateTime = dateTime.ToUniversalTime();
                    break;
                case DateTimeKind.Unspecified:
                    dateTime = new DateTime(dateTime.Ticks, DateTimeKind.Utc);
                    break;
                case DateTimeKind.Utc:
                    break;
            }

            System.Diagnostics.Debug.Assert(dateTime.Kind == DateTimeKind.Utc, "dateTime.Kind == DateTimeKind.Utc");
            this.WriteCore(
                String.Format(
                    CultureInfo.InvariantCulture,
                    JsonWriter.JsonDateTimeFormat,
                    ((dateTime.Ticks - DatetimeMinTimeTicks) / 10000)),
                true);
        }

        /// <summary>
        /// Write the byte value
        /// </summary>
        /// <param name="value">byte value to be written</param>
        public void WriteValue(byte value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), /* quotes */ false);
        }

        /// <summary>
        /// Write the sbyte value
        /// </summary>
        /// <param name="value">sbyte value to be written</param>
        public void WriteValue(sbyte value)
        {
            this.WriteCore(value.ToString(CultureInfo.InvariantCulture), /* quotes */ false);
        }

        /// <summary>
        /// Write the string value
        /// </summary>
        /// <param name="s">string value to be written</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Service.Serializers.JsonWriter.WriteCore(System.String,System.Boolean)", Justification = "Literals are JSON standard elements, and should not be localized")]
        public void WriteValue(string s)
        {
            if (s == null)
            {
                this.WriteCore("null", /* quotes */ false);
            }
            else
            {
                this.WriteCore(QuoteJScriptString(s), /* quotes */ true);
            }
        }

        /// <summary>
        /// Clears all buffers for the current writer
        /// </summary>
        public void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Returns the string value with special characters escaped
        /// </summary>
        /// <param name="s">input string value</param>
        /// <returns>Returns the string value with special characters escaped.</returns>
        private static string QuoteJScriptString(string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.Empty;
            }

            StringBuilder b = null;
            int startIndex = 0;
            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                // Append the unhandled characters (that do not require special treament)
                // to the string builder when special characters are detected.
                if (c == '\r' || c == '\t' || c == '\"' ||
                    c == '\\' || c == '\n' || c < ' ' || c > 0x7F || c == '\b' || c == '\f')
                {
                    // Flush out the unescaped characters we've built so far.
                    if (b == null)
                    {
                        b = new StringBuilder(s.Length + 6);
                    }

                    if (count > 0)
                    {
                        b.Append(s, startIndex, count);
                    }

                    startIndex = i + 1;
                    count = 0;
                }

                switch (c)
                {
                    case '\r':
                        b.Append("\\r");
                        break;
                    case '\t':
                        b.Append("\\t");
                        break;
                    case '\"':
                        b.Append("\\\"");
                        break;
                    case '\\':
                        b.Append("\\\\");
                        break;
                    case '\n':
                        b.Append("\\n");
                        break;
                    case '\b':
                        b.Append("\\b");
                        break;
                    case '\f':
                        b.Append("\\f");
                        break;
                    default:
                        if ((c < ' ') || (c > 0x7F))
                        {
                            b.AppendFormat(CultureInfo.InvariantCulture, "\\u{0:x4}", (int)c);
                        }
                        else
                        {
                            count++;
                        }

                        break;
                }
            }

            string processedString = s;
            if (b != null)
            {
                if (count > 0)
                {
                    b.Append(s, startIndex, count);
                }

                processedString = b.ToString();
            }

            return processedString;
        }

        /// <summary>
        /// Write the string value with/without quotes
        /// </summary>
        /// <param name="text">string value to be written</param>
        /// <param name="quotes">put quotes around the value if this value is true</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Service.Serializers.IndentedTextWriter.WriteTrimmed(System.String)", Justification = "Literals are JSON standard elements, and should not be localized")]
        private void WriteCore(string text, bool quotes)
        {
            if (this.scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();
                if (currentScope.Type == ScopeType.Array)
                {
                    if (currentScope.ObjectCount != 0)
                    {
                        this.writer.WriteTrimmed(", ");
                    }

                    currentScope.ObjectCount++;
                }
            }

            if (quotes)
            {
                this.writer.Write('"');
            }

            this.writer.Write(text);
            if (quotes)
            {
                this.writer.Write('"');
            }
        }

        /// <summary>
        /// Start the scope given the scope type
        /// </summary>
        /// <param name="type">scope type</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.OData.Service.Serializers.IndentedTextWriter.WriteTrimmed(System.String)", Justification = "Literals are JSON standard elements, and should not be localized")]
        private void StartScope(ScopeType type)
        {
            if (this.scopes.Count != 0)
            {
                Scope currentScope = this.scopes.Peek();
                if ((currentScope.Type == ScopeType.Array) &&
                    (currentScope.ObjectCount != 0))
                {
                    this.writer.WriteTrimmed(", ");
                }

                currentScope.ObjectCount++;
            }

            Scope scope = new Scope(type);
            this.scopes.Push(scope);

            if (type == ScopeType.Array)
            {
                this.writer.Write("[");
            }
            else
            {
                this.writer.Write("{");
            }

            this.writer.Indent++;
            this.writer.WriteLine();
        }

        /// <summary>
        /// class representing scope information
        /// </summary>
        private sealed class Scope
        {
            /// <summary> keeps the count of the nested scopes </summary>
            private int objectCount;

            /// <summary> keeps the type of the scope </summary>
            private ScopeType type;

            /// <summary>
            /// Creates a new instance of scope type
            /// </summary>
            /// <param name="type">type of the scope</param>
            public Scope(ScopeType type)
            {
                this.type = type;
            }

            /// <summary>
            /// Get/Set the object count for this scope
            /// </summary>
            public int ObjectCount
            {
                get
                {
                    return this.objectCount;
                }

                set
                {
                    this.objectCount = value;
                }
            }

            /// <summary>
            /// Gets the scope type for this scope
            /// </summary>
            public ScopeType Type
            {
                get
                {
                    return this.type;
                }
            }
        }
    }

    /// <summary>Writes the Json text in indented format.</summary>
    /// <remarks>
    /// There are many more methods implemented in previous versions
    /// of this file to handle more type and newline cases.
    /// </remarks>
    internal sealed class IndentedTextWriter : TextWriter
    {
        /// <summary> writer to which Json text needs to be written</summary>
        private TextWriter writer;

        /// <summary> keeps track of the indentLevel</summary>
        private int indentLevel;

        /// <summary> keeps track of pending tabs</summary>
        private bool tabsPending;

        /// <summary> string representation of tab</summary>
        private string tabString;

        /// <summary>
        /// Creates a new instance of IndentedTextWriter over the given text writer
        /// </summary>
        /// <param name="writer">writer which IndentedTextWriter wraps</param>
        public IndentedTextWriter(TextWriter writer)
            : base(CultureInfo.InvariantCulture)
        {
            this.writer = writer;
            this.tabString = "    ";
        }

        /// <summary> Returns the Encoding for the given writer </summary>
        public override Encoding Encoding
        {
            get
            {
                return this.writer.Encoding;
            }
        }

        /// <summary> Returns the new line character </summary>
        public override string NewLine
        {
            get
            {
                return this.writer.NewLine;
            }
        }

        /// <summary> returns the current indent level </summary>
        public int Indent
        {
            get
            {
                return this.indentLevel;
            }

            set
            {
                Debug.Assert(value >= 0, "value >= 0");
                if (value < 0)
                {
                    value = 0;
                }

                this.indentLevel = value;
            }
        }

        /// <summary> Closes the underlying writer</summary>
        public override void Close()
        {
            // This is done to make sure we don't accidently close the underlying stream.
            // Since we don't own the stream, we should never close it.
            throw new NotImplementedException();
        }

        /// <summary> Clears all the buffer of the current writer </summary>
        public override void Flush()
        {
            this.writer.Flush();
        }

        /// <summary>
        /// Writes the given string value to the underlying writer
        /// </summary>
        /// <param name="s">string value to be written</param>
        public override void Write(string s)
        {
            this.OutputTabs();
            this.writer.Write(s);
        }

        /// <summary>
        /// Writes the given char value to the underlying writer 
        /// </summary>
        /// <param name="value">char value to be written</param>
        public override void Write(char value)
        {
            this.OutputTabs();
            this.writer.Write(value);
        }

        /// <summary>
        /// Writes the trimmed text if minimizeWhiteSpeace is set to true
        /// </summary>
        /// <param name="text">string value to be written</param>
        public void WriteTrimmed(string text)
        {
            this.Write(text);
        }

        /// <summary> Writes the tabs depending on the indent level </summary>
        private void OutputTabs()
        {
            if (this.tabsPending)
            {
                for (int i = 0; i < this.indentLevel; i++)
                {
                    this.writer.Write(this.tabString);
                }

                this.tabsPending = false;
            }
        }
    }
}
