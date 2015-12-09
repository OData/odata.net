//---------------------------------------------------------------------
// <copyright file="CommonWritingValidationScenarioTests.cs" company="Microsoft">
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
    public class CommonWritingValidationScenarioTests
    {
        [Fact]
        public void WriteEntryWithFakeTypeShouldFailWithUnrecognizedTypeName()
        {
            IEdmEntitySet entitySet = null;
            var model = CreatePersonModel(out entitySet);

            foreach (string contentType in new string[] { "application/json", "application/atom+xml" })
            {
                var messageWriter = CreateODataMessageWriter(model, contentType);
                var odataWriter = messageWriter.CreateODataEntryWriter();
                Action test = () =>
                {
                    var entry = new ODataEntry() { Properties = new List<ODataProperty>(new ODataProperty[] { new ODataProperty() { Name = "Id", Value = 1 } }) };
                    entry.TypeName = "DefaultNamespace.FakeType";
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                };

                test.ShouldThrow<ODataException>().WithMessage(Strings.ValidationUtils_UnrecognizedTypeName("DefaultNamespace.FakeType"));
            }
        }

        [Fact]
        public void WriteEntryWithoutTypeNameWithMetadataOnWriterShouldWriteOnJsonLightFailOnOtherFormats()
        {
            IEdmEntitySet entitySet = null;
            var model = CreatePersonModel(out entitySet);

            foreach (string contentType in new string[] { "application/json", "application/atom+xml" })
            {
                var messageWriter = CreateODataMessageWriter(model, contentType);
                var odataWriter = messageWriter.CreateODataEntryWriter(null, entitySet.EntityType());
                Action test = () =>
                {
                    var entry = new ODataEntry();
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                };

                test.ShouldNotThrow();
            }
        }

        [Fact]
        public void WriteEntryWithNoTypeNameAndNoWriterMetadataShouldFailWithMissingTypeInformation()
        {
            IEdmEntitySet entitySet = null;
            var model = CreatePersonModel(out entitySet);

            foreach (string contentType in new string[] { "application/json", "application/atom+xml" })
            {
                var messageWriter = CreateODataMessageWriter(model, contentType);
                var odataWriter = messageWriter.CreateODataEntryWriter();
                Action test = () =>
                {
                    var entry = new ODataEntry() { Properties = new List<ODataProperty>(new ODataProperty[] { new ODataProperty() { Name = "Id", Value = 1 } }) };
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                };

                test.ShouldThrow<ODataException>().WithMessage(Strings.WriterValidationUtils_MissingTypeNameWithMetadata);
            }
        }

        [Fact]
        public void WriteEntryOnODataFormatsWithoutSpecifingModelOrUsingEdmCoreModel()
        {
            foreach (IEdmModel model in new IEdmModel[] { null, EdmCoreModel.Instance })
            {
                foreach (string contentType in new string[] { "application/json", "application/atom+xml" })
                {
                    string currentContentType = contentType;
                    var currentModel = model;
                    Action test = () =>
                    {
                        var messageWriter = CreateODataMessageWriter(currentModel, currentContentType);
                        var odataWriter = messageWriter.CreateODataEntryWriter();
                        var entry = new ODataEntry() { Properties = new List<ODataProperty>(new ODataProperty[] { new ODataProperty() { Name = "Id", Value = 1 } }) };
                        odataWriter.WriteStart(entry);
                        odataWriter.WriteEnd();
                    };

                    test.ShouldNotThrow();
                }
            }
        }

        [Fact]
        public void WriteEntryWithCollectionOfTypeDefinitionShouldWork()
        {
            IEdmEntitySet entitySet = null;
            var model = CreatePersonModelWithTypeDefinition(out entitySet);

            foreach (string contentType in new string[] { "application/json" })
            {
                var messageWriter = CreateODataMessageWriter(model, contentType);
                var odataWriter = messageWriter.CreateODataEntryWriter(null, entitySet.EntityType());
                Action test = () =>
                {
                    var entry = new ODataEntry()
                    {
                        Properties = new[]
                        {
                            new ODataProperty()
                            {
                                Name = "Id",
                                Value = 1
                            },
                            new ODataProperty()
                            {
                                Name = "Numbers",
                                Value = new ODataCollectionValue() {Items = new[] { (UInt32)1, UInt32.MaxValue, UInt32.MinValue } }
                            }
                        }
                    };
                    odataWriter.WriteStart(entry);
                    odataWriter.WriteEnd();
                };

                test.ShouldNotThrow();
            }
        }

        private static EdmModel CreatePersonModel(out IEdmEntitySet entitySet)
        {
            var model = new EdmModel();
            var container = new EdmEntityContainer("DefaultNamespace", "Container");
            var personType = new EdmEntityType("DefaultNamespace", "Person");
            var keyProp = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddKeys(keyProp);
            entitySet = container.AddEntitySet("People", personType);
            model.AddElement(container);
            model.AddElement(personType);
            return model;
        }

        private static EdmModel CreatePersonModelWithTypeDefinition(out IEdmEntitySet entitySet)
        {
            var model = new EdmModel();
            var uint32 = model.GetUInt32("DefaultNamespace", false);
            var container = new EdmEntityContainer("DefaultNamespace", "Container");
            var personType = new EdmEntityType("DefaultNamespace", "Person");
            var keyProp = personType.AddStructuralProperty("Id", EdmPrimitiveTypeKind.Int32);
            personType.AddStructuralProperty("Numbers", new EdmCollectionTypeReference(new EdmCollectionType(uint32)));
            personType.AddKeys(keyProp);
            entitySet = container.AddEntitySet("People", personType);
            model.AddElement(container);
            model.AddElement(personType);
            return model;
        }

        private static ODataMessageWriter CreateODataMessageWriter(IEdmModel model, string contentType)
        {
            var settings = new ODataMessageWriterSettings { DisableMessageStreamDisposal = true, Version = ODataVersion.V4, EnableAtom = true };
            settings.SetServiceDocumentUri(new Uri("http://example.com"));

            var message = new InMemoryMessage();
            message.Stream = new MemoryStream();
            message.SetHeader("Accept", contentType);
            message.SetHeader("Content-Type", contentType);
            message.SetHeader("OData-Version", "4.0");
            if (model == null)
            {
                return new ODataMessageWriter((IODataRequestMessage)message, settings);
            }
            else
            {
                return new ODataMessageWriter((IODataRequestMessage)message, settings, model);
            }
        }
    }
}
