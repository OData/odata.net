//---------------------------------------------------------------------
// <copyright file="PayloadGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.Data.Test.Astoria.Util;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using AstoriaUnitTests.Data;
    using Microsoft.OData;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class PayloadBuilder
    {
        private List<PropertyPayloadBuilder> properties;
        private List<OperationPayloadBuilder> operations;

        public string[] OpenProperties { get; set; }
        public string TypeName { get; set; }
        public string Id { get; set; }
        public string Metadata { get; set; }
        public string Uri { get; set; }
        public bool IsComplex { get; set; }

        public IEnumerable<PropertyPayloadBuilder> Properties
        {
            get
            {
                return this.properties;
            }

            set
            {
                this.properties = new List<PropertyPayloadBuilder>(value);
            }
        }

        public List<OperationPayloadBuilder> Operations
        {
            get
            {
                if (this.operations == null)
                {
                    this.operations = new List<OperationPayloadBuilder>();
                }

                return this.operations;
            }

            set
            {
                this.operations = value;
            }
        }

        public PayloadBuilder AddProperty(string name, object value)
        {
            if (value is PayloadBuilder)
            {
                this.AddProperty(name, value, ((PayloadBuilder)value).IsComplex ? PayloadBuilderPropertyKind.Complex : PayloadBuilderPropertyKind.EntityReference);
            }
            else if (value is PayloadBuilder[])
            {
                this.AddProperty(name, value, PayloadBuilderPropertyKind.EntityCollection);
            }
            else if (value != null && value.GetType().IsEnum)
            {
                this.AddProperty(name, value, PayloadBuilderPropertyKind.Enum);
            }
            else
            {
                this.AddProperty(name, value, PayloadBuilderPropertyKind.Primitive);
            }

            return this;
        }

        public PayloadBuilder AddComplexProperty(string name, PayloadBuilder propertyPayloadBuilder)
        {
            propertyPayloadBuilder.IsComplex = true;
            this.AddProperty(name, propertyPayloadBuilder, PayloadBuilderPropertyKind.Complex);
            return this;
        }

        public PayloadBuilder AddCollectionProperty(string name, string collectionItemEdmTypeName, ICollection value)
        {
            this.AddProperty(new CollectionPropertyPayloadBuilder(name, collectionItemEdmTypeName, value));
            return this;
        }

        public PayloadBuilder AddNavigationReferenceProperty(string name, PayloadBuilder propertyPayloadBuilder)
        {
            if (propertyPayloadBuilder != null)
            {
                propertyPayloadBuilder.IsComplex = false;
            }
            this.AddProperty(name, propertyPayloadBuilder, PayloadBuilderPropertyKind.EntityReference);
            return this;
        }

        public PayloadBuilder AddNavigationCollectionProperty(string name, PayloadBuilder[] propertyPayloadBuilder)
        {
            this.AddProperty(name, propertyPayloadBuilder, PayloadBuilderPropertyKind.EntityCollection);
            return this;
        }

        public PayloadBuilder SetOpenProperties(params string[] openPropertyNames)
        {
            this.OpenProperties = openPropertyNames;
            return this;
        }

        public PayloadBuilder AddAction(string metadata, string title, string target)
        {
            this.AddOperation(metadata, title, target, PayloadBuilderPropertyKind.Action);
            return this;
        }

        public PayloadBuilder AddFunction(string metadata, string title, string target)
        {
            this.AddOperation(metadata, title, target, PayloadBuilderPropertyKind.Function);
            return this;
        }

        private void AddOperation(string metadata, string title, string target, PayloadBuilderPropertyKind operationKind)
        {
            this.Operations.Add(new OperationPayloadBuilder(metadata, title, target, operationKind));
        }

        private void AddProperty(string name, object value, PayloadBuilderPropertyKind payloadBuilderPropertyKind)
        {
            this.AddProperty(new PropertyPayloadBuilder(name, value, payloadBuilderPropertyKind));
        }

        private void AddProperty(PropertyPayloadBuilder propertyPayloadBuilder)
        {
            if (this.properties == null)
            {
                this.properties = new List<PropertyPayloadBuilder>();
            }

            this.properties.Add(propertyPayloadBuilder);
        }
    }

    public class PayloadGeneratorSettings
    {
        public PayloadGeneratorSettings()
        {
            this.IncludeWhitespaceInJson = true;
            this.QuotePropertyNamesInJson = false;
        }

        public bool IncludeWhitespaceInJson { get; set; }
        public bool QuotePropertyNamesInJson { get; set; }
    }

    public abstract class PayloadGenerator
    {
        private int depth = 0;

        protected int Depth { get { return this.depth; } }
        protected PayloadGeneratorSettings Settings { get; private set; }

        protected PayloadGenerator(PayloadGeneratorSettings settings)
        {
            Assert.IsNotNull(settings, "PayloadGeneratorSettings must be non-null.");
            this.Settings = settings;
        }

        public static string Generate(PayloadBuilder payloadBuilder, string format)
        {
            ODataFormat oDataFormat = null;
            if (format.StartsWith(UnitTestsUtil.JsonLightMimeType))
            {
                oDataFormat = ODataFormat.Json;
            }
            //else if (format.StartsWith(UnitTestsUtil.AtomFormat) ||
            //         format.StartsWith(UnitTestsUtil.MimeApplicationXml))
            //{
            //    oDataFormat = ODataFormat.Atom;
            //}
            //[Lianw] Remove Atom 
            else if (format.StartsWith(UnitTestsUtil.MimeApplicationXml))
            {
                oDataFormat = ODataFormat.Metadata;
            }
            else
            {
                Assert.Fail(String.Format("Invalid format specified - {0}", format));
            }

            return Generate(payloadBuilder, oDataFormat);
        }

        public static string Generate(PayloadBuilder payloadBuilder, ODataFormat format, PayloadGeneratorSettings settings = null)
        {
            if (settings == null)
            {
                settings = new PayloadGeneratorSettings();
            }

            if (payloadBuilder == null)
            {
                return null;
            }

            PayloadGenerator payloadGenerator = null;
            if (format == ODataFormat.Json)
            {
                payloadGenerator = new JsonLightPayloadGenerator(settings);
            }
            //else if (format == ODataFormat.Atom)
            //{
            //    payloadGenerator = new AtomPayloadGenerator(settings);
            //}
            else
            {
                Assert.Fail(String.Format("Payload generation not implemented for {0}", format));
            }

            string payload;
            if (PayloadGenerator.IsEntityBindingPayload(payloadBuilder))
            {
                payload = payloadGenerator.GenerateLinkPayload(payloadBuilder);
            }
            else
            {
                payload = payloadGenerator.Generate(payloadBuilder);
            }

            return payload;
        }

        public abstract string Generate(PayloadBuilder payloadBuilder);
        public abstract string GenerateLinkPayload(PayloadBuilder payloadBuilder);

        protected void IncrementDepth() { this.depth++; }
        protected void DecrementDepth() { this.depth--; }

        protected static bool IsEntityBindingPayload(PayloadBuilder builder)
        {
            return !builder.IsComplex && !String.IsNullOrEmpty(builder.Uri) && builder.Properties == null;
        }

        protected static bool IsEntityInsertPayload(PayloadBuilder builder)
        {
            return !builder.IsComplex && builder.Properties != null;
        }
    }

    public abstract class JsonPayloadGenerator : PayloadGenerator
    {
        protected StringBuilder stringBuilder = new StringBuilder();
        private bool first = false;
        private bool arrayContainsValues = true;

        public JsonPayloadGenerator(PayloadGeneratorSettings settings)
            : base(settings)
        {
        }

        public override sealed string Generate(PayloadBuilder payloadBuilder)
        {
            this.GenerateJson(payloadBuilder, topLevel: true);
            return stringBuilder.ToString();
        }

        public string GenerateLiteral(CollectionPropertyPayloadBuilder collectionBuilder)
        {
            this.WriteCollectionValue(collectionBuilder, writeCollectionTypeName: true);
            return stringBuilder.ToString();
        }

        public string GenerateLiteral(PayloadBuilder payloadBuilder)
        {
            this.GenerateJson(payloadBuilder);
            return stringBuilder.ToString();
        }

        protected abstract void GenerateJson(PayloadBuilder payloadBuilder, bool topLevel = false);

        protected abstract void WriteTypeMetadata(string typeName);

        protected abstract string CollectionWrapperPropertyName { get; }

        protected void WriteStartObject()
        {
            this.AppendTabs(stringBuilder);
            stringBuilder.Append("{");
            this.WriteNewLine();
            this.IncrementDepth();
            this.first = true;
            this.arrayContainsValues = false;
        }

        protected void WriteStartArray()
        {
            this.AppendTabs(stringBuilder);
            stringBuilder.Append("[");
            this.WriteNewLine();
            this.IncrementDepth();
            this.first = true;
            this.arrayContainsValues = true;
        }

        protected void WriteKey(string key)
        {
            Assert.IsFalse(arrayContainsValues, "firstInArray - key cannot be first in array");
            if (!this.first)
            {
                stringBuilder.Append(",");
                this.WriteNewLine();
            }

            this.AppendTabs(stringBuilder);

            if (this.Settings.QuotePropertyNamesInJson)
            {
                key = this.GetQuotedString(key);
            }

            stringBuilder.Append(key);
            stringBuilder.Append(":");
            this.first = false;
        }

        protected void WriteValue(string value)
        {
            if (arrayContainsValues)
            {
                if (!first)
                {
                    stringBuilder.Append(",");
                    this.WriteNewLine();
                }

                this.AppendTabs(stringBuilder);
            }

            stringBuilder.Append(value);
            first = false;
        }

        protected void WriteKeyValuePair(string key, string value)
        {
            this.WriteKey(key);
            this.WriteValue(value);
        }

        protected void WriteEndObject()
        {
            this.DecrementDepth();
            this.WriteNewLine();
            this.AppendTabs(stringBuilder);
            stringBuilder.Append("}");
        }

        protected void WriteEndArray()
        {
            this.DecrementDepth();
            this.WriteNewLine();
            this.AppendTabs(stringBuilder);
            stringBuilder.Append("]");
            this.arrayContainsValues = false;
        }

        protected void WriteCollectionValue(CollectionPropertyPayloadBuilder propertyBuilder, bool writeCollectionTypeName)
        {
            var collectionItemEdmTypeName = propertyBuilder.CollectionEdmTypeName;
            var collectionItems = (ICollection)propertyBuilder.Value;

            // In some cases we don't want to write the type name even if it's specified. E.g. we do write it with JSON Verbose even for collections inside of other objects
            // but with JSON Light we don't. So in that case the test case would specify the type name but the JSON Light generator would explicitly request it not to be written.
            bool wrapInComplexType = !String.IsNullOrEmpty(collectionItemEdmTypeName) && writeCollectionTypeName;
            if (wrapInComplexType)
            {
                this.WriteStartObject();
                this.WriteTypeMetadata(collectionItemEdmTypeName);
                this.WriteKey(this.CollectionWrapperPropertyName);
            }

            this.WriteStartArray();
            bool firstElement = true;
            foreach (object coll in collectionItems)
            {
                PayloadBuilder sp = coll as PayloadBuilder;
                if (sp != null)
                {
                    if (!firstElement)
                    {
                        this.WriteValue(",");
                    }
                    this.GenerateJson(sp);
                    firstElement = false;
                }
                else if (coll != null && coll.GetType().IsEnum)
                {
                    this.WriteValue(JsonPrimitiveTypesUtil.PrimitiveToString(coll.ToString(), null));
                }
                else
                {
                    this.WriteValue(JsonPrimitiveTypesUtil.PrimitiveToString(coll, null));
                }
            }

            this.WriteEndArray();

            if (wrapInComplexType)
            {
                this.WriteEndObject();
            }
        }

        protected string GetQuotedString(string value)
        {
            return "\"" + value + "\"";
        }

        protected string GetPrefixTypeName(string typeName)
        {
            if (typeName == null || EdmCoreModel.Instance.FindDeclaredType(typeName) != null)
            {
                return typeName;
            }
            else
            {
                return '#' + typeName;
            }
        }

        private const string CollectionTypeQualifier = "Collection";
        protected string RemoveEdmPrefixFromTypeName(string typeName)
        {
            string itemTypeName = GetCollectionItemTypeName(typeName);
            if (itemTypeName == null)
            {
                // This is not a collection type
                IEdmSchemaType edmType = EdmCoreModel.Instance.FindDeclaredType(typeName);
                if (edmType != null)
                {
                    return edmType.ShortQualifiedName();
                }
            }
            else
            {
                // This is a collection type
                IEdmSchemaType edmType = EdmCoreModel.Instance.FindDeclaredType(itemTypeName);
                if (edmType != null)
                {
                    return CollectionTypeQualifier + String.Format("({0})", edmType.ShortQualifiedName());
                }
            }

            return typeName;
        }

        private static string GetCollectionItemTypeName(string typeName)
        {
            int collectionTypeQualifierLength = CollectionTypeQualifier.Length;

            // to be recognized as a collection wireTypeName must not be null, has to start with "Collection(" and end with ")" and must not be "Collection()"
            if (typeName != null &&
                typeName.StartsWith(CollectionTypeQualifier + "(", StringComparison.Ordinal) &&
                typeName[typeName.Length - 1] == ')' &&
                typeName.Length != collectionTypeQualifierLength + 2)
            {
                return typeName.Substring(collectionTypeQualifierLength + 1, typeName.Length - (collectionTypeQualifierLength + 2));
            }

            return null;
        }

        private void WriteNewLine()
        {
            if (this.Settings.IncludeWhitespaceInJson)
            {
                this.stringBuilder.Append(Environment.NewLine);
            }
        }

        private void AppendTabs(StringBuilder payload)
        {
            if (this.Settings.IncludeWhitespaceInJson)
            {
                for (int i = 0; i < this.Depth; i++)
                {
                    payload.Append("\t");
                }
            }
        }
    }

    public class JsonLightPayloadGenerator : JsonPayloadGenerator
    {
        public JsonLightPayloadGenerator(PayloadGeneratorSettings settings)
            : base(settings)
        {
        }

        public sealed override string GenerateLinkPayload(PayloadBuilder payloadBuilder)
        {
            this.WriteStartObject();
            this.WriteKeyValuePair("@odata.id", this.GetQuotedString(payloadBuilder.Uri));
            this.WriteEndObject();
            return stringBuilder.ToString();
        }

        protected override void GenerateJson(PayloadBuilder builder, bool topLevel = false)
        {
            if (builder.IsComplex && topLevel)
            {
                this.GeneratePropertyPayload(builder);
                return;
            }

            this.WriteStartObject();

            if (!String.IsNullOrEmpty(builder.Metadata))
            {
                this.WriteKeyValuePair("@odata.context", this.GetQuotedString(builder.Metadata));
            }

            if (!String.IsNullOrEmpty(builder.TypeName))
            {
                this.WriteTypeMetadata(builder.TypeName);
            }

            if (!builder.IsComplex && !String.IsNullOrEmpty(builder.Uri))
            {
                this.WriteKeyValuePair("@odata.editLink", this.GetQuotedString(builder.Uri));
            }

            // Write the operations
            foreach (var operation in builder.Operations)
            {
                this.WriteKey(this.GetQuotedString(operation.Metadata));

                this.WriteStartObject();

                if (!String.IsNullOrEmpty(operation.Title))
                {
                    this.WriteKeyValuePair("title", this.GetQuotedString(operation.Title));
                }

                if (!String.IsNullOrEmpty(operation.Target))
                {
                    this.WriteKeyValuePair("target", this.GetQuotedString(operation.Target));
                }

                this.WriteEndObject();
            }

            if (builder.Properties != null)
            {
                foreach (var pi in builder.Properties)
                {
                    // For open properties, we need to write the type name first as property annotation
                    if (builder.OpenProperties != null && builder.OpenProperties.Exists(n => n == pi.Name) && pi.Value != null)
                    {
                        PayloadGenerator payloadGenerator = pi.Value as PayloadGenerator;
                        if (payloadGenerator == null)
                        {
                            // Write the type annotation
                            // TODO: make this work for byte[] and System.Linq.Binary types
                            string typeName = pi.Value.GetType().ToString().Replace("System", "Edm");
                            this.WriteKeyValuePair(pi.Name + "@odata.type", this.GetQuotedString(GetPrefixTypeName(RemoveEdmPrefixFromTypeName(typeName))));
                        }
                    }

                    if (pi.PropertyKind == PayloadBuilderPropertyKind.Primitive || pi.Value == null)
                    {
                        // Write primitive property value
                        this.WriteKeyValuePair(pi.Name, JsonPrimitiveTypesUtil.PrimitiveToString(pi.Value, null));
                    }
                    else if (pi.PropertyKind == PayloadBuilderPropertyKind.Enum)
                    {
                        // Write enum property value
                        this.WriteKeyValuePair(pi.Name, pi.Value + "");
                    }
                    else
                    {
                        PayloadBuilder structuredPropertyValue = pi.Value as PayloadBuilder;
                        if (structuredPropertyValue != null)
                        {
                            if (IsEntityBindingPayload(structuredPropertyValue))
                            {
                                this.WriteKeyValuePair(pi.Name + "@odata.bind", this.GetQuotedString(structuredPropertyValue.Uri));
                            }
                            else
                            {
                                this.WriteKey(pi.Name);
                                this.GenerateJson(structuredPropertyValue);
                            }
                        }
                        else if (pi.PropertyKind == PayloadBuilderPropertyKind.Collection)
                        {
                            var collectionPropertyPayloadBuilder = (CollectionPropertyPayloadBuilder)pi;
                            if (!String.IsNullOrEmpty(collectionPropertyPayloadBuilder.CollectionEdmTypeName))
                            {
                                this.WriteKeyValuePair(pi.Name + "@odata.type", this.GetQuotedString(GetPrefixTypeName(RemoveEdmPrefixFromTypeName(collectionPropertyPayloadBuilder.CollectionEdmTypeName))));
                            }
                            this.WriteKey(pi.Name);
                            this.WriteCollectionValue(collectionPropertyPayloadBuilder, writeCollectionTypeName: false);
                        }
                        else
                        {
                            IEnumerable<PayloadBuilder> payloads = pi.Value as IEnumerable<PayloadBuilder>;
                            if (payloads != null)
                            {
                                var bindingPayloads = payloads.Where(p => IsEntityBindingPayload(p)).ToList();
                                if (bindingPayloads.Count > 0)
                                {
                                    this.WriteKey(pi.Name + "@odata.bind");
                                    this.WriteStartArray();
                                    foreach (var url in bindingPayloads)
                                    {
                                        this.WriteValue(this.GetQuotedString(url.Uri));
                                    }

                                    this.WriteEndArray();
                                }

                                var insertPayloads = payloads.Where(p => IsEntityInsertPayload(p)).ToList();
                                if (insertPayloads.Count > 0)
                                {
                                    this.WriteKey(pi.Name);
                                    this.WriteStartArray();
                                    bool first = true;
                                    foreach (var insert in insertPayloads)
                                    {
                                        if (!first)
                                        {
                                            this.WriteValue(",");
                                        }

                                        this.GenerateJson(insert);
                                        first = false;
                                    }

                                    this.WriteEndArray();
                                }
                            }
                        }
                    }
                }
            }

            this.WriteEndObject();
        }

        protected override void WriteTypeMetadata(string typeName)
        {
            this.WriteKeyValuePair("@odata.type", this.GetQuotedString(GetPrefixTypeName(RemoveEdmPrefixFromTypeName(typeName))));
        }

        protected override string CollectionWrapperPropertyName
        {
            get { return "value"; }
        }

        private void GeneratePropertyPayload(PayloadBuilder builder)
        {
            Assert.IsTrue(builder.Properties.Count() == 1, "There must be exactly one property specified");
            var property = builder.Properties.Single();

            var propertyValue = property.Value as PayloadBuilder;
            if (propertyValue != null)
            {
                this.GenerateJson(propertyValue);
            }
            else
            {
                this.WriteStartObject();

                // For open properties, we need to write the type name first as property annotation
                if (builder.OpenProperties != null && builder.OpenProperties.Exists(n => n == property.Name) && property.Value != null)
                {
                    PayloadGenerator payloadGenerator = property.Value as PayloadGenerator;
                    if (payloadGenerator == null)
                    {
                        // Write the type annotation
                        // TODO: make this work for byte[] and System.Linq.Binary types
                        string typeName = property.Value.GetType().ToString().Replace("System", "Edm");
                        this.WriteKeyValuePair("@odata.type", this.GetQuotedString(GetPrefixTypeName(RemoveEdmPrefixFromTypeName(typeName))));
                    }
                }

                // TODO: Change the payload of null top-level properties #645
                if (property.PropertyKind == PayloadBuilderPropertyKind.Primitive || property.Value == null)
                {
                    // Write primitive property value
                    this.WriteKeyValuePair("value", JsonPrimitiveTypesUtil.PrimitiveToString(property.Value, null));
                }

                this.WriteEndObject();
            }
        }
    }

    public class JsonVerbosePayloadGenerator : JsonPayloadGenerator
    {
        public JsonVerbosePayloadGenerator(PayloadGeneratorSettings settings)
            : base(settings)
        {
        }

        public sealed override string GenerateLinkPayload(PayloadBuilder payloadBuilder)
        {
            this.WriteStartObject();
            this.WriteKeyValuePair("uri", this.GetQuotedString(payloadBuilder.Uri));
            this.WriteEndObject();
            return stringBuilder.ToString();
        }

        protected override void GenerateJson(PayloadBuilder builder, bool topLevel = false)
        {
            this.WriteStartObject();
            this.WriteMetadata(builder.TypeName, builder.Uri);

            if (builder.Properties != null)
            {
                foreach (var pi in builder.Properties)
                {
                    if (pi.Value == null || pi.PropertyKind == PayloadBuilderPropertyKind.Primitive)
                    {
                        this.WriteKeyValuePair(pi.Name, JsonPrimitiveTypesUtil.PrimitiveToString(pi.Value, null));
                    }
                    else
                    {
                        this.WriteKey(pi.Name);
                        PayloadBuilder structuredPayload = pi.Value as PayloadBuilder;
                        if (structuredPayload != null)
                        {
                            this.GenerateJson(structuredPayload);
                        }
                        else if (pi.PropertyKind == PayloadBuilderPropertyKind.Collection)
                        {
                            this.WriteCollectionValue((CollectionPropertyPayloadBuilder)pi, writeCollectionTypeName: true);
                        }
                        else
                        {
                            PayloadBuilder[] collectionProperties = pi.Value as PayloadBuilder[];
                            this.WriteStartArray();
                            bool firstElement = true;
                            foreach (var element in collectionProperties)
                            {
                                if (!firstElement) this.WriteValue(",");
                                this.GenerateJson(element);
                                firstElement = false;
                            }

                            this.WriteEndArray();
                        }
                    }
                }
            }

            this.WriteEndObject();
        }

        protected override void WriteTypeMetadata(string typeName)
        {
            this.WriteMetadata(typeName, null);
        }

        protected override string CollectionWrapperPropertyName
        {
            get { return "results"; }
        }

        private void WriteMetadata(string typeName, string uri)
        {
            if (!String.IsNullOrEmpty(typeName) || !String.IsNullOrEmpty(uri))
            {
                this.WriteKey("__metadata");
                this.WriteStartObject();

                if (!String.IsNullOrEmpty(typeName))
                {
                    this.WriteKeyValuePair("type", this.GetQuotedString(typeName));
                }

                if (!String.IsNullOrEmpty(uri))
                {
                    this.WriteKeyValuePair("uri", this.GetQuotedString(uri));
                }

                this.WriteEndObject();
            }
        }
    }

    public class AtomPayloadGenerator : PayloadGenerator
    {
        public AtomPayloadGenerator(PayloadGeneratorSettings settings)
            : base(settings)
        {
        }

        public override string GenerateLinkPayload(PayloadBuilder payloadBuilder)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter writer = CreateXmlWriterAndWriteProcessingInstruction(stringBuilder))
            {
                writer.WriteStartElement("ref", AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace);
                writer.WriteAttributeString("id", payloadBuilder.Uri);
                writer.WriteEndElement();
            }

            return stringBuilder.ToString();
        }

        public override string Generate(PayloadBuilder payloadBuilder)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter writer = CreateXmlWriterAndWriteProcessingInstruction(stringBuilder))
            {
                this.GeneratePayload(writer, payloadBuilder);
            }

            return stringBuilder.ToString();
        }

        private void GeneratePayload(XmlWriter writer, PayloadBuilder builder)
        {
            if (builder.IsComplex)
            {
                GeneratePropertiesPayload(writer, builder);
                return;
            }

            writer.WriteStartElement("entry", AtomUpdatePayloadBuilder.AtomXmlNamespace);
            WriteCommonNamespaces(writer);

            if (!String.IsNullOrEmpty(builder.Id))
            {
                writer.WriteStartElement("id", AtomUpdatePayloadBuilder.AtomXmlNamespace);
                writer.WriteValue(builder.Id);
                writer.WriteEndElement();
            }

            if (!String.IsNullOrEmpty(builder.TypeName))
            {
                writer.WriteStartElement("category", AtomUpdatePayloadBuilder.AtomXmlNamespace);
                writer.WriteAttributeString("term", builder.TypeName);
                writer.WriteAttributeString("scheme", AtomUpdatePayloadBuilder.DataWebSchemeNamespace);
                writer.WriteEndElement();
            }

            var navProperties = new List<PropertyPayloadBuilder>();
            if (builder.Properties != null && builder.Properties.Count() != 0)
            {
                writer.WriteStartElement("content", AtomUpdatePayloadBuilder.AtomXmlNamespace);
                writer.WriteAttributeString("type", "application/xml");
                writer.WriteStartElement("properties", AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace);
                navProperties = this.GeneratePropertiesPayload(writer, builder);
                writer.WriteEndElement(); //properties
                writer.WriteEndElement(); //content
            }

            // write the nav properties
            foreach (var pi in navProperties)
            {
                PayloadBuilder referenceNavProperty = pi.Value as PayloadBuilder;
                if (pi.PropertyKind == PayloadBuilderPropertyKind.EntityReference || referenceNavProperty != null)
                {
                    writer.WriteStartElement("link", AtomUpdatePayloadBuilder.AtomXmlNamespace);

                    writer.WriteAttributeString("rel", AtomUpdatePayloadBuilder.DataWebRelatedXmlNamespace + pi.Name);
                    writer.WriteAttributeString("title", pi.Name);

                    // If there is only uri element specified, then its a binding case and we need to emit
                    // an href attribute, otherwise it might be deep insert and we need to specify the whole entity
                    if ((pi.PropertyKind == PayloadBuilderPropertyKind.EntityReference && pi.Value == null) || IsEntityBindingPayload(referenceNavProperty))
                    {
                        writer.WriteAttributeString("href", pi.Value == null ? string.Empty : referenceNavProperty.Uri);
                    }
                    else
                    {
                        writer.WriteStartElement("inline", AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace);
                        this.GeneratePayload(writer, referenceNavProperty);
                        writer.WriteEndElement(); // inline
                    }

                    writer.WriteEndElement(); // link
                }
                else
                {
                    PayloadBuilder[] navCollectionProperty = (PayloadBuilder[])pi.Value;

                    foreach (var bindProperty in navCollectionProperty.Where(p => IsEntityBindingPayload(p)))
                    {
                        writer.WriteStartElement("link", AtomUpdatePayloadBuilder.AtomXmlNamespace);

                        writer.WriteAttributeString("rel", AtomUpdatePayloadBuilder.DataWebRelatedXmlNamespace + pi.Name);
                        writer.WriteAttributeString("title", pi.Name);
                        writer.WriteAttributeString("href", bindProperty.Uri);
                        writer.WriteEndElement(); // link
                    }

                    var insertPayloads = navCollectionProperty.Where(p => IsEntityInsertPayload(p)).ToList();
                    if (insertPayloads.Count > 0)
                    {
                        writer.WriteStartElement("link", AtomUpdatePayloadBuilder.AtomXmlNamespace);

                        writer.WriteAttributeString("rel", AtomUpdatePayloadBuilder.DataWebRelatedXmlNamespace + pi.Name);
                        writer.WriteAttributeString("title", pi.Name);

                        writer.WriteStartElement("inline", AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace);
                        writer.WriteStartElement("feed", AtomUpdatePayloadBuilder.AtomXmlNamespace);

                        foreach (var element in insertPayloads)
                        {
                            this.GeneratePayload(writer, element);
                        }

                        writer.WriteEndElement(); //feed
                        writer.WriteEndElement(); //inline
                        writer.WriteEndElement(); // link
                    }
                }
            }

            // Write the operations
            foreach (var operation in builder.Operations)
            {
                var operationKind = operation.PropertyKind == PayloadBuilderPropertyKind.Action ? "action" : "function";
                writer.WriteStartElement(operationKind, AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace);

                if (operation.Title != null)
                {
                    writer.WriteAttributeString("title", operation.Title);
                }

                if (operation.Metadata != null)
                {
                    writer.WriteAttributeString("metadata", operation.Metadata);
                }

                if (operation.Target != null)
                {
                    writer.WriteAttributeString("target", operation.Target);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement(); // entry
        }

        private List<PropertyPayloadBuilder> GeneratePropertiesPayload(XmlWriter writer, PayloadBuilder builder)
        {
            List<PropertyPayloadBuilder> navProperties = new List<PropertyPayloadBuilder>();
            foreach (var pi in builder.Properties)
            {
                if (pi.PropertyKind == PayloadBuilderPropertyKind.Primitive)
                {
                    writer.WriteStartElement(pi.Name, AtomUpdatePayloadBuilder.DataWebXmlNamespace);
                    if (pi.Value == null)
                    {
                        writer.WriteAttributeString("null", AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace, "true");
                    }
                    else
                    {
                        writer.WriteValue(TypeData.XmlValueFromObject(pi.Value));
                    }
                    writer.WriteEndElement();
                }
                else
                {
                    PayloadBuilder structuredPropertyPayload = pi.Value as PayloadBuilder;
                    if (structuredPropertyPayload != null)
                    {
                        if (structuredPropertyPayload.IsComplex)
                        {
                            writer.WriteStartElement(pi.Name, AtomUpdatePayloadBuilder.DataWebXmlNamespace);
                            if (!String.IsNullOrEmpty(structuredPropertyPayload.TypeName))
                            {
                                writer.WriteAttributeString("type", AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace, structuredPropertyPayload.TypeName);
                            }

                            PayloadBuilder p = (PayloadBuilder)pi.Value;
                            this.GeneratePayload(writer, structuredPropertyPayload);
                            writer.WriteEndElement();
                        }
                        else
                        {
                            navProperties.Add(pi);
                        }
                    }
                    else
                    {
                        navProperties.Add(pi);
                    }
                }
            }

            return navProperties;
        }

        internal static XmlWriter CreateXmlWriterAndWriteProcessingInstruction(StringBuilder builder)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CheckCharacters = false;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.Indent = true;
            settings.NewLineHandling = NewLineHandling.Entitize;

            XmlWriter writer = XmlWriter.Create(builder, settings);
            writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"");
            return writer;
        }

        private static void WriteCommonNamespaces(XmlWriter writer)
        {
            writer.WriteAttributeString("xml", "base", null, "/");
            writer.WriteAttributeString("xmlns", "ads", null, AtomUpdatePayloadBuilder.DataWebXmlNamespace);
            writer.WriteAttributeString("xmlns", "adsm", null, AtomUpdatePayloadBuilder.DataWebMetadataXmlNamespace);
        }
    }

    public class PropertyPayloadBuilder
    {
        internal string Name { get; private set; }
        internal object Value { get; private set; }
        internal PayloadBuilderPropertyKind PropertyKind { get; private set; }

        internal PropertyPayloadBuilder(string name, object value, PayloadBuilderPropertyKind propertyKind)
        {
            this.Name = name;
            this.Value = value;
            this.PropertyKind = propertyKind;
        }
    }

    public class CollectionPropertyPayloadBuilder : PropertyPayloadBuilder
    {
        internal string CollectionEdmTypeName { get; private set; }

        public CollectionPropertyPayloadBuilder(string name, string collectionItemEdmTypeName, object value)
            : base(name, value, PayloadBuilderPropertyKind.Collection)
        {
            string collectionEdmTypeName = null;
            if (!String.IsNullOrEmpty(collectionItemEdmTypeName))
            {
                collectionEdmTypeName = String.Format("Collection({0})", collectionItemEdmTypeName);
            }

            this.CollectionEdmTypeName = collectionEdmTypeName;
        }
    }

    public class OperationPayloadBuilder
    {
        public string Metadata { get; private set; }
        public string Title { get; private set; }
        public string Target { get; private set; }
        public PayloadBuilderPropertyKind PropertyKind { get; private set; }

        internal OperationPayloadBuilder(string metadata, string title, string target, PayloadBuilderPropertyKind operationKind)
        {
            this.Metadata = metadata;
            this.Title = title;
            this.Target = target;

            Assert.IsTrue(operationKind == PayloadBuilderPropertyKind.Action || operationKind == PayloadBuilderPropertyKind.Function, "OperationPayloadBuilder only supports Actions and Functions.");
            this.PropertyKind = operationKind;
        }
    }

    public enum PayloadBuilderPropertyKind
    {
        Enum,
        Primitive,
        Complex,
        EntityReference,
        EntityCollection,
        Collection,
        Action,
        Function
    }
}
