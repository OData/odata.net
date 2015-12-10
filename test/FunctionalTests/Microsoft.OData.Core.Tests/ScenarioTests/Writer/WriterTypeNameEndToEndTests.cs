//---------------------------------------------------------------------
// <copyright file="WriterTypeNameEndToEndTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Xunit;

namespace Microsoft.OData.Core.Tests.ScenarioTests.Writer
{
    /// <summary>
    /// These tests baseline the end-to-end behavior of when type names are written on the wire, 
    /// based on the format and metadata level along with whether the AutoComputePayloadMetadataInJson 
    /// flag is set on the message writer settings. These tests are not meant to be exhaustive, but
    /// should catch major end-to-end problems. The unit tests for the individual components are more extensive.
    /// </summary>
    public class WriterTypeNameEndToEndTests : IDisposable
    {
        private Lazy<ODataMessageWriter> messageWriter;
        private ODataMessageWriterSettings settings;
        private Lazy<string> writerOutput;
        private static readonly Uri ServiceDocumentUri = new Uri("http://odata.org/");

        private const string jsonLightNoMetadata = "application/json;odata.metadata=none";
        private const string jsonLightMinimalMetadata = "application/json;odata.metadata=minimal";
        private const string jsonLightFullMetadata = "application/json;odata.metadata=full";
        private const string atom = "application/atom+xml";

        public WriterTypeNameEndToEndTests()
        {
            var model = new EdmModel();
            var type = new EdmEntityType("TestModel", "TestEntity", /* baseType */ null, /* isAbstract */ false, /* isOpen */ true);
            var keyProperty = type.AddStructuralProperty("DeclaredInt16", EdmPrimitiveTypeKind.Int16);
            type.AddKeys(new[] { keyProperty });

            // Note: DerivedPrimitive is declared as a Geography, but its value below will be set to GeographyPoint, which is derived from Geography.
            type.AddStructuralProperty("DerivedPrimitive", EdmPrimitiveTypeKind.Geography);
            var container = new EdmEntityContainer("TestModel", "Container");
            var set = container.AddEntitySet("Set", type);
            model.AddElement(type);
            model.AddElement(container);

            var writerStream = new MemoryStream();
            this.settings = new ODataMessageWriterSettings();
            this.settings.SetServiceDocumentUri(ServiceDocumentUri);

            // Make the message writer and entry writer lazy so that individual tests can tweak the settings before the message writer is created.
            this.messageWriter = new Lazy<ODataMessageWriter>(() =>
                new ODataMessageWriter(
                    (IODataResponseMessage)new InMemoryMessage { Stream = writerStream },
                    this.settings,
                    model));

            var entryWriter = new Lazy<ODataWriter>(() => this.messageWriter.Value.CreateODataEntryWriter(set, type));

            var valueWithAnnotation = new ODataPrimitiveValue(45);
            valueWithAnnotation.SetAnnotation(new SerializationTypeNameAnnotation { TypeName = "TypeNameFromSTNA" });

            var propertiesToWrite = new List<ODataProperty>
            {
                new ODataProperty
                {
                    Name = "DeclaredInt16", Value = (Int16)42
                }, 
                new ODataProperty
                {
                    Name = "UndeclaredDecimal", Value = (Decimal)4.5
                }, 
                new ODataProperty
                {
                    // Note: value is more derived than the declared type.
                    Name = "DerivedPrimitive", Value = Microsoft.Spatial.GeographyPoint.Create(42, 45)
                },
                new ODataProperty()
                {
                    Name = "PropertyWithSTNA", Value = valueWithAnnotation
                }
            };

            this.writerOutput = new Lazy<string>(() =>
            {
                entryWriter.Value.WriteStart(new ODataEntry { Properties = propertiesToWrite });
                entryWriter.Value.WriteEnd();
                entryWriter.Value.Flush();
                writerStream.Seek(0, SeekOrigin.Begin);
                return new StreamReader(writerStream).ReadToEnd();
            });
        }

        public void Dispose()
        {
            if (messageWriter.IsValueCreated)
            {
                messageWriter.Value.Dispose();
            }
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInMinimalMetadataWhenKnobIsOff()
        {
            this.settings.SetContentType(jsonLightMinimalMetadata, null);
            this.settings.AutoComputePayloadMetadataInJson = false;
            this.writerOutput.Value.Should()
                .NotContain("DeclaredInt16@odata.type")
                .And.Contain("UndeclaredDecimal@odata.type\":\"#Decimal\"")
                .And.Contain("DerivedPrimitive@odata.type\":\"#GeographyPoint\"")
                .And.Contain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInFullMetadataWhenKnobIsOff()
        {
            this.settings.SetContentType(jsonLightFullMetadata, null);
            this.settings.AutoComputePayloadMetadataInJson = false;
            this.writerOutput.Value.Should()
                .NotContain("DeclaredInt16@odata.type")
                .And.Contain("UndeclaredDecimal@odata.type\":\"#Decimal\"")
                .And.Contain("DerivedPrimitive@odata.type\":\"#GeographyPoint\"")
                .And.Contain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInNoMetadataWhenKnobIsOff()
        {
            this.settings.SetContentType(jsonLightNoMetadata, null);
            this.settings.AutoComputePayloadMetadataInJson = false;
            this.writerOutput.Value.Should()
                .NotContain("DeclaredInt16@odata.type")
                .And.Contain("UndeclaredDecimal@odata.type\":\"#Decimal\"")
                .And.Contain("DerivedPrimitive@odata.type\":\"#GeographyPoint\"")
                .And.Contain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInAtomWhenKnobIsOff()
        {
            this.settings.SetContentType(atom, null);
            this.settings.EnableAtom = true;
            this.settings.AutoComputePayloadMetadataInJson = false;
            this.writerOutput.Value.Should()
                .Contain("d:DeclaredInt16 m:type=\"Int16\"")
                .And.Contain("d:UndeclaredDecimal m:type=\"Decimal\"")
                .And.Contain("d:DerivedPrimitive m:type=\"GeographyPoint\"")
                .And.Contain("d:PropertyWithSTNA m:type=\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInMinimalMetadataWhenKnobIsSet()
        {
            this.settings.SetContentType(jsonLightMinimalMetadata, null);
            this.settings.AutoComputePayloadMetadataInJson = true;
            this.writerOutput.Value.Should()
                .NotContain("DeclaredInt16@odata.type")
                .And.Contain("UndeclaredDecimal@odata.type\":\"#Decimal\"")
                .And.Contain("DerivedPrimitive@odata.type\":\"#GeographyPoint\"")
                .And.Contain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInNoMetadataWhenKnobIsSet()
        {
            this.settings.SetContentType(jsonLightNoMetadata, null);
            this.settings.AutoComputePayloadMetadataInJson = true;
            this.writerOutput.Value.Should()
                .NotContain("DeclaredInt16@odata.type")
                .And.NotContain("UndeclaredDecimal@odata.type")
                .And.NotContain("DerivedPrimitive@odata.type")
                .And.NotContain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInNoMetadataWhenKnobIsSetWithJsonP()
        {
            this.settings.SetContentType(jsonLightNoMetadata, null);
            this.settings.JsonPCallback = "callback";
            this.settings.AutoComputePayloadMetadataInJson = true;
            this.writerOutput.Value.Should()
                .NotContain("DeclaredInt16@odata.type")
                .And.NotContain("UndeclaredDecimal@odata.type")
                .And.NotContain("DerivedPrimitive@odata.type")
                .And.NotContain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInFullMetadataWhenKnobIsSet()
        {
            this.settings.SetContentType(jsonLightFullMetadata, null);
            this.settings.AutoComputePayloadMetadataInJson = true;
            this.writerOutput.Value.Should()
                .Contain("DeclaredInt16@odata.type\":\"#Int16\"")
                .And.Contain("UndeclaredDecimal@odata.type\":\"#Decimal\"")
                .And.Contain("DerivedPrimitive@odata.type\":\"#GeographyPoint\"")
                .And.Contain("PropertyWithSTNA@odata.type\":\"#TypeNameFromSTNA\"");
        }

        [Fact]
        public void TypeNameShouldBeWrittenCorrectlyInAtomWhenKnobIsSet()
        {
            this.settings.SetContentType(atom, null);
            this.settings.EnableAtom = true;
            this.settings.AutoComputePayloadMetadataInJson = true;
            this.writerOutput.Value.Should()
                .Contain("d:DeclaredInt16 m:type=\"Int16\"")
                .And.Contain("d:UndeclaredDecimal m:type=\"Decimal\"")
                .And.Contain("d:DerivedPrimitive m:type=\"GeographyPoint\"")
                .And.Contain("d:PropertyWithSTNA m:type=\"#TypeNameFromSTNA\"");
        }
    }
}
