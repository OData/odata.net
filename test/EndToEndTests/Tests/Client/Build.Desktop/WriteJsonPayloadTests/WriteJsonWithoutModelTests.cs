//---------------------------------------------------------------------
// <copyright file="WriteJsonWithoutModelTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.OData.Edm;
    using Microsoft.OData.UriParser;

    /// <summary>
    /// Write various payload with/without EDM model in Atom/VerboseJosn/JsonLight formats.
    /// </summary>
    [TestClass]
    public class WritePayloadTests : EndToEndTestBase
    {
        protected const string NameSpace = "Microsoft.Test.OData.Services.AstoriaDefaultService.";

        protected List<string> mimeTypes = new List<string>()
        {
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata,
            MimeTypes.ApplicationJson + MimeTypes.ODataParameterNoMetadata,
        };

        public WritePayloadTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        public override void CustomTestInitialize()
        {
            WritePayloadHelper.CustomTestInitialize(this.ServiceUri);
        }


        /// <summary>
        /// Write a feed with multiple order entries. The feed/entry contains properties, navigation & association links, next link.
        /// </summary>
        [TestMethod]
        public void OrderFeedTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    outputWithModel = this.WriteAndVerifyOrderFeed(responseMessageWithModel, odataWriter, true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    outputWithoutModel = this.WriteAndVerifyOrderFeed(responseMessageWithoutModel, odataWriter, false,
                                                                      mimeType);
                }

                //var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                //outputWithoutModel = rex.Replace(outputWithoutModel, "");
                //WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");
                    var outputWithModel2 = rex.Replace(outputWithModel, "");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel2, outputWithoutModel2, mimeType);
                }
                else if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel2, mimeType);
                }
                else
                {
                    Assert.AreEqual(outputWithModel, outputWithoutModel, "NoMetadata with/out model should result in same output");
                }
            }
        }

        /// <summary>
        /// Write an expanded customer entry containing primitive, complex, collection of primitive/complex properties.
        /// </summary>
        [TestMethod]
        public void ExpandedCustomerEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutModel = this.WriteAndVerifyExpandedCustomerEntry(responseMessageWithoutModel,
                                                                                  odataWriter, false, mimeType);
                }
                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType);
                    outputWithModel = this.WriteAndVerifyExpandedCustomerEntry(responseMessageWithModel, odataWriter,
                                                                               false, mimeType);
                }

                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.associationLink\":\"[^\"]*\",");
                    var outputWithModel2 = rex.Replace(outputWithModel, "");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel2, outputWithoutModel2, mimeType);
                }
                else if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    var rex = new Regex("\"\\w*@odata.type\":\"#[\\w\\(\\)\\.]*\",");
                    var outputWithoutModel2 = rex.Replace(outputWithoutModel, "");
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel2, mimeType);
                }
                else
                {
                    Assert.AreEqual(outputWithModel, outputWithoutModel, "NoMetadata with/out model should result in same output");
                }
            }
        }

        /// <summary>
        /// Write and read an expanded customer entry containing primitive, complex, collection of primitive/complex properties with null values
        /// using omit-value=nulls Preference header.
        /// </summary>
        [TestMethod]
        public void ExpandedCustomerEntryOmitNullValuesTest()
        {
            // Repeat with full and minimal metadata.
            foreach (string mimeType in this.mimeTypes.GetRange(0, 2))
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() {ServiceRoot = this.ServiceUri};
                string rspRecvd = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                responseMessageWithModel.PreferenceAppliedHeader().OmitValues = ODataConstants.OmitValuesNulls;

                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings,
                        WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.CustomerSet,
                        WritePayloadHelper.CustomerType);
                    rspRecvd = this.WriteAndVerifyExpandedCustomerEntryWithSomeNullValues(
                        responseMessageWithModel, odataWriter, /* hasModel */false);
                    Assert.IsNotNull(rspRecvd);
                }
            }
        }

        // Test provided by Mike Pizzo as omit-values=nulls requirement test cases related to
        // dynamic properties and annotation-only properties.
        [TestMethod]
        public void Mikep_ReadWriteNullNavigationTest()
        {
            // set up model
            var model = new EdmModel();
            var addressType = model.AddComplexType("test", "Address");
            addressType.AddStructuralProperty("city", EdmPrimitiveTypeKind.String, true);
            var employeeType = model.AddEntityType("test", "Employee", null, false, true);
            var key = employeeType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, false);
            employeeType.AddKeys(key);
            employeeType.AddStructuralProperty("Title", EdmPrimitiveTypeKind.String, true);
            employeeType.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, true));

            employeeType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Manager",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    Target = employeeType
                }
            );
            var container = model.AddEntityContainer("test", "service");
            var employeeSet = container.AddEntitySet("employees", employeeType);

            // set up writer and reader
            Uri serviceUri = new Uri("http://test/");

            //            string expectedNulls = "{\"@odata.context\":\"" + serviceUri + "$metadata#employees(Name,Title,Dynamic,Address,Manager)/$entity\",\"Name\":\"Fred\",\"Title@test.annotation\":\"annotationValue\",\"Title\":null,\"Dynamic\":null,\"DynamicAnnotated@test.annotation\":\"annotationValue\",\"DynamicAnnotated\":null,\"Address\":null,\"Manager\":null}";
            string expectedNulls = @"{
                ""@odata.context"":""http://test/$metadata#employees(Name,Title,Dynamic,Address,Manager)/$entity"",
                ""Name"":""Fred"",
                ""Title@test.annotation"":""annotationValue"",""Title"":null,
                ""Dynamic"":null,
                ""DynamicAnnotated@test.annotation"":""annotationValue"",""DynamicAnnotated"":null,
                ""Address"":null,""Manager"":null
            }";
            expectedNulls = Regex.Replace(expectedNulls, @"\s*", string.Empty, RegexOptions.Multiline);
            Assert.IsTrue(expectedNulls.Contains(serviceUri.ToString()));

            //            string expectedNoNulls = "{\"@odata.context\":\"" + serviceUri + "$metadata#employees(Name,Title,Dynamic,Address,Manager)/$entity\",\"Name\":\"Fred\",\"Title@test.annotation\":\"annotationValue\",\"DynamicAnnotated@test.annotation\":\"annotationValue\"}";
            string expectedNoNulls = @"{
                ""@odata.context"":""http://test/$metadata#employees(Name,Title,Dynamic,Address,Manager)/$entity"",
                ""Name"":""Fred"",
                ""Title@test.annotation"":""annotationValue"",
                ""DynamicAnnotated@test.annotation"":""annotationValue""
            }";
            expectedNoNulls = Regex.Replace(expectedNoNulls, @"\s*", string.Empty, RegexOptions.Multiline);
            Assert.IsTrue(expectedNoNulls.Contains(serviceUri.ToString()));

            var writeSettings = new ODataMessageWriterSettings() { BaseUri = serviceUri };
            writeSettings.ODataUri = new ODataUriParser(model, serviceUri, new Uri("employees?$select=Name,Title,Dynamic,Address&$expand=Manager", UriKind.Relative)).ParseUri();

            foreach (bool omitNulls in new bool[] { false, true })
            {
                // validate writing a null navigation property value
                var stream = new MemoryStream();
                var responseMessage = new StreamResponseMessage(stream);
                if (omitNulls)
                {
                    responseMessage.SetHeader("Preference-Applied", "omit-values=nulls,odata.include-annotations=\"*\"");
                }
                else
                {
                    responseMessage.SetHeader("Preference-Applied", "odata.include-annotations=\"*\"");
                }
                var messageWriter = new ODataMessageWriter(responseMessage, writeSettings, model);
                var writer = messageWriter.CreateODataResourceWriter(employeeSet, employeeType);
                writer.WriteStart(new ODataResource
                {
                    Properties = new ODataProperty[]
                    {
                        new ODataProperty {Name = "Name", Value = "Fred" },
                        new ODataProperty {Name = "Title", Value = new ODataNullValue(),
                            InstanceAnnotations = new ODataInstanceAnnotation[] {
                                new ODataInstanceAnnotation("test.annotation", new ODataPrimitiveValue( "annotationValue" ))
                            }
                        },
                        new ODataProperty {Name = "Dynamic", Value = new ODataNullValue() },
                        new ODataProperty {Name = "DynamicAnnotated", Value = new ODataNullValue(),
                            InstanceAnnotations = new ODataInstanceAnnotation[] {
                                new ODataInstanceAnnotation("test.annotation", new ODataPrimitiveValue( "annotationValue" ))
                            }
                        }
                    }
                });

                // write address
                writer.WriteStart(new ODataNestedResourceInfo
                {
                    Name = "Address",
                    IsCollection = false
                });
                writer.WriteStart((ODataResource)null);
                writer.WriteEnd(); //address
                writer.WriteEnd(); //address nestedInfo

                // write manager
                writer.WriteStart(new ODataNestedResourceInfo
                {
                    Name = "Manager",
                    IsCollection = false
                });
                writer.WriteStart((ODataResource)null);
                writer.WriteEnd(); // manager resource
                writer.WriteEnd(); // manager nested info

                writer.WriteEnd(); // resource
                stream.Flush();

                // compare written stream to expected stream
                stream.Seek(0, SeekOrigin.Begin);
                var streamReader = new StreamReader(stream);
                Assert.AreEqual(omitNulls ? expectedNoNulls : expectedNulls, streamReader.ReadToEnd(), String.Format("Did not generate expected string when {0}omitting nulls", omitNulls ? "" : "not "));

                // validate reading back the stream
                var readSettings = new ODataMessageReaderSettings() { BaseUri = serviceUri, ReadUntypedAsString = false };
                stream.Seek(0, SeekOrigin.Begin);
                var messageReader = new ODataMessageReader(responseMessage, readSettings, model);
                var reader = messageReader.CreateODataResourceReader(employeeSet, employeeType);
                string reading = "Employee";
                bool addressReturned = false;
                bool managerReturned = false;
                ODataResource employeeResource = null;
                ODataResource managerResource = null;
                ODataResource addressResource = null;
                while (reader.Read())
                {
                    switch (reader.State)
                    {
                        case ODataReaderState.ResourceEnd:
                            switch (reading)
                            {
                                case "Manager":
                                    managerResource = reader.Item as ODataResource;
                                    managerReturned = true;
                                    break;
                                case "Address":
                                    addressResource = reader.Item as ODataResource;
                                    addressReturned = true;
                                    break;
                                case "Employee":
                                    employeeResource = reader.Item as ODataResource;
                                    break;
                            }
                            break;
                        case ODataReaderState.NestedResourceInfoStart:
                            reading = ((ODataNestedResourceInfo)reader.Item).Name;
                            break;
                        case ODataReaderState.NestedResourceInfoEnd:
                            reading = "Employee";
                            break;
                    }
                }

                if (!omitNulls)
                {
                    Assert.IsTrue(managerReturned, "Manager not returned when {0}omitting nulls", omitNulls ? "" : "not ");
                }
                Assert.IsNull(managerResource, "Manager resource not null when {0}omitting nulls", omitNulls ? "" : "not ");

                if (!omitNulls)
                {
                    Assert.IsTrue(addressReturned, "Address not returned when {0}omitting nulls", omitNulls ? "" : "not ");
                }
                Assert.IsNull(addressResource, "Address resource not null when {0}omitting nulls", omitNulls ? "" : "not ");

                Assert.IsNotNull(employeeResource, "Employee should not be null");
                var titleProperty = employeeResource.Properties.FirstOrDefault(p => p.Name == "Title");
                Assert.IsNotNull(titleProperty, "Title property should not be null");
                Assert.IsNull(titleProperty.Value, "Title property value should be null");
                var titleAnnotation = titleProperty.InstanceAnnotations.FirstOrDefault(a => a.Name == "test.annotation");
                Assert.IsNotNull(titleAnnotation, "Title property missing the test.annotation");
                Assert.AreEqual(((ODataPrimitiveValue)titleAnnotation.Value).Value.ToString(), "annotationValue");
                var dynamicProperty = employeeResource.Properties.FirstOrDefault(p => p.Name == "Dynamic");
                Assert.IsNotNull(dynamicProperty, "Dynamic property should not be null");
                Assert.IsNull(dynamicProperty.Value, "Dynamic property value should be null");
                var dynamicAnnotatedProperty = employeeResource.Properties.FirstOrDefault(p => p.Name == "DynamicAnnotated");
                Assert.IsNotNull(dynamicAnnotatedProperty, "DynamicAnnotated property should not be null");
                Assert.IsNull(dynamicAnnotatedProperty.Value, "DynamicAnnotated property value should be null");
                var dynamicAnnotation = dynamicAnnotatedProperty.InstanceAnnotations.FirstOrDefault(a => a.Name == "test.annotation");
                Assert.IsNotNull(dynamicAnnotation, "DynamicAnnotated property missing the test.annotation");
                Assert.AreEqual(((ODataPrimitiveValue)dynamicAnnotation.Value).Value.ToString(), "annotationValue");
            }
        }

        /// <summary>
        /// Write an entry containing stream, named stream
        /// </summary>
        [TestMethod]
        public void CarEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                responseMessageWithModel.PreferenceAppliedHeader().AnnotationFilter = "*";
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.CarSet, WritePayloadHelper.CarType);
                    outputWithModel = this.WriteAndVerifyCarEntry(responseMessageWithModel, odataWriter, true, mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                responseMessageWithoutModel.PreferenceAppliedHeader().AnnotationFilter = "*";
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutModel = this.WriteAndVerifyCarEntry(responseMessageWithoutModel, odataWriter, false,
                                                                     mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }

        /// <summary>
        /// Write a feed containing actions, derived type entry instance
        /// </summary>
        [TestMethod]
        public void PersonFeedTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterFullMetadata)
                {
                    continue;
                }

                var settings = new ODataMessageWriterSettings() { BaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType);
                    outputWithModel = this.WriteAndVerifyPersonFeed(responseMessageWithModel, odataWriter, false,
                                                                   mimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter();
                    outputWithoutModel = this.WriteAndVerifyPersonFeed(responseMessageWithoutModel, odataWriter, false,
                                                                       mimeType);
                }

                if (mimeType == MimeTypes.ApplicationJson + MimeTypes.ODataParameterMinimalMetadata)
                {
                    WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
                }
                else
                {
                    Assert.AreEqual(outputWithModel, outputWithoutModel, "NoMetadata with/out model should result in same output");
                }

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    Assert.IsTrue(outputWithoutModel.Contains(this.ServiceUri + "$metadata#Person\""));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // odata.type is included in json light payload only if entry typename is different than serialization info
                    Assert.IsFalse(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "Person\","), "odata.type Person");
                    Assert.IsTrue(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "Employee\","), "odata.type Employee");
                    Assert.IsTrue(outputWithoutModel.Contains("{\"@odata.type\":\"" + "#" + NameSpace + "SpecialEmployee\","), "odata.type SpecialEmployee");
                }
            }
        }

        /// <summary>
        /// Write an employee entry with/without ExpectedTypeName in serialization info
        /// </summary>
        [TestMethod]
        public void EmployeeEntryTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings() { BaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithTypeCast = null;
                string outputWithoutTypeCast = null;

                // employee entry as response of person(1)
                var responseMessageWithoutTypeCast = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutTypeCast.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutTypeCast, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutTypeCast = this.WriteAndVerifyEmployeeEntry(responseMessageWithoutTypeCast, odataWriter,
                                                                             false, mimeType);
                }

                // employee entry as response of person(1)/EmployeeTyeName, in this case the test sets ExpectedTypeName as Employee in Serialization info
                var responseMessageWithTypeCast = new StreamResponseMessage(new MemoryStream());
                responseMessageWithTypeCast.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithTypeCast, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithTypeCast = this.WriteAndVerifyEmployeeEntry(responseMessageWithTypeCast, odataWriter, true,
                                                                          mimeType);
                }

                if (mimeType.Contains(MimeTypes.ODataParameterMinimalMetadata) || mimeType.Contains(MimeTypes.ODataParameterFullMetadata))
                {
                    // expect type cast in odata.metadata if EntitySetElementTypeName != ExpectedTypeName
                    Assert.IsTrue(outputWithoutTypeCast.Contains(this.ServiceUri + "$metadata#Person/$entity"));
                    Assert.IsTrue(
                        outputWithTypeCast.Contains(this.ServiceUri + "$metadata#Person/" + NameSpace +
                                                    "Employee/$entity"));
                }

                if (mimeType.Contains(MimeTypes.ApplicationJsonLight))
                {
                    // write odata.type if entry TypeName != ExpectedTypeName
                    Assert.IsTrue(outputWithoutTypeCast.Contains("odata.type"));
                    Assert.IsFalse(outputWithTypeCast.Contains("odata.type"));
                }
            }
        }

        /// <summary>
        /// Write complex collection response
        /// </summary>
        [TestMethod]
        public void ComplexCollectionTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                string testMimeType = mimeType.Contains("xml") ? MimeTypes.ApplicationXml : mimeType;

                var settings = new ODataMessageWriterSettings() { BaseUri = this.ServiceUri };
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var responseMessageWithModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithModel.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(null, WritePayloadHelper.ContactDetailType);
                    outputWithModel = this.WriteAndVerifyCollection(responseMessageWithModel, odataWriter, true,
                                                                    testMimeType);
                }

                var responseMessageWithoutModel = new StreamResponseMessage(new MemoryStream());
                responseMessageWithoutModel.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceSetWriter(null, WritePayloadHelper.ContactDetailType);
                    outputWithoutModel = this.WriteAndVerifyCollection(responseMessageWithoutModel, odataWriter, false,
                                                                       testMimeType);
                }

                Assert.AreEqual(outputWithModel, outputWithoutModel);
            }
        }

        /// <summary>
        /// Write $ref response
        /// </summary>
        [TestMethod]
        public void LinksTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                if (mimeType.Equals(MimeTypes.ApplicationXml)) { continue; }
                string testMimeType = mimeType;
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                var responseMessage = new StreamResponseMessage(new MemoryStream());
                responseMessage.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessage, settings, WritePayloadHelper.Model))
                {
                    this.WriteAndVerifyLinks(responseMessage, messageWriter, testMimeType);
                }
            }
        }

        /// <summary>
        /// Write $ref response with a single link
        /// </summary>
        [TestMethod]
        public void SingleLinkTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                string testMimeType = mimeType.Contains("xml") ? MimeTypes.ApplicationXml : mimeType;

                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };

                var responseMessage = new StreamResponseMessage(new MemoryStream());
                responseMessage.SetHeader("Content-Type", testMimeType);
                using (var messageWriter = new ODataMessageWriter(responseMessage, settings, WritePayloadHelper.Model))
                {
                    this.WriteAndVerifySingleLink(responseMessage, messageWriter, testMimeType);
                }
            }
        }

        /// <summary>
        /// Write a request message with an entry
        /// </summary>
        [TestMethod]
        public void RequestMessageTest()
        {
            foreach (var mimeType in this.mimeTypes)
            {
                var settings = new ODataMessageWriterSettings();
                settings.ODataUri = new ODataUri() { ServiceRoot = this.ServiceUri };
                string outputWithModel = null;
                string outputWithoutModel = null;

                var requestMessageWithModel = new StreamRequestMessage(new MemoryStream(),
                                                                       new Uri(this.ServiceUri + "Order"), "POST");
                requestMessageWithModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(requestMessageWithModel, settings, WritePayloadHelper.Model))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                    outputWithModel = this.WriteAndVerifyRequestMessage(requestMessageWithModel, odataWriter,
                                                                        true, mimeType);
                }

                var requestMessageWithoutModel = new StreamRequestMessage(new MemoryStream(),
                                                                          new Uri(this.ServiceUri + "Order"), "POST");
                requestMessageWithoutModel.SetHeader("Content-Type", mimeType);
                using (var messageWriter = new ODataMessageWriter(requestMessageWithoutModel, settings))
                {
                    var odataWriter = messageWriter.CreateODataResourceWriter();
                    outputWithoutModel = this.WriteAndVerifyRequestMessage(requestMessageWithoutModel,
                                                                           odataWriter, false, mimeType);
                }

                WritePayloadHelper.VerifyPayloadString(outputWithModel, outputWithoutModel, mimeType);
            }
        }


        private string WriteAndVerifyOrderFeed(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                               bool hasModel, string mimeType)
        {
            var orderFeed = new ODataResourceSet()
            {
                Id = new Uri(this.ServiceUri + "Order"),
                NextPageLink = new Uri(this.ServiceUri + "Order?$skiptoken=-9"),
            };
            if (!hasModel)
            {
                orderFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
            }

            odataWriter.WriteStart(orderFeed);

            var orderEntry1 = WritePayloadHelper.CreateOrderEntry1(hasModel);
            odataWriter.WriteStart(orderEntry1);

            var orderEntry1Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-10)/Customer")
            };
            odataWriter.WriteStart(orderEntry1Navigation1);
            odataWriter.WriteEnd();

            var orderEntry1Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-10)/Login")
            };
            odataWriter.WriteStart(orderEntry1Navigation2);
            odataWriter.WriteEnd();

            // Finish writing orderEntry1.
            odataWriter.WriteEnd();

            var orderEntry2Wrapper = WritePayloadHelper.CreateOrderEntry2(hasModel);


            var orderEntry2Navigation1 = new ODataNestedResourceInfo()
            {
                Name = "Customer",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-9)/Customer")
            };

            var orderEntry2Navigation2 = new ODataNestedResourceInfo()
            {
                Name = "Login",
                IsCollection = false,
                Url = new Uri(this.ServiceUri + "Order(-9)/Login")
            };
            orderEntry2Wrapper.NestedResourceInfoWrappers = orderEntry2Wrapper.NestedResourceInfoWrappers.Concat(
                new[]
                {
                    new ODataNestedResourceInfoWrapper() { NestedResourceInfo = orderEntry1Navigation1 },
                    new ODataNestedResourceInfoWrapper() { NestedResourceInfo = orderEntry2Navigation2 }
                });

            ODataWriterHelper.WriteResource(odataWriter, orderEntry2Wrapper);

            // Finish writing the feed.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                Assert.IsNotNull(feed.NextPageLink, "feed.NextPageLink");
                verifyFeedCalled = true;
            };
            Action<ODataResource> verifyEntry = (entry) =>
            {
                if (entry.TypeName.Contains("Order"))
                {
                    Assert.AreEqual(2, entry.Properties.Count(), "entry.Properties.Count");
                }
                else
                {
                    Assert.IsTrue(entry.TypeName.Contains("ConcurrencyInfo"), "complex Property Concurrency should be read into ODataResource");
                }
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.IsTrue(navigation.Name == "Customer" || navigation.Name == "Login" || navigation.Name == "Concurrency", "navigation.Name");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private void WriteCustomerEntry( ODataWriter odataWriter, bool hasModel, bool withSomeNullValues)
        {
            ODataResourceWrapper customerEntry = WritePayloadHelper.CreateCustomerEntry(hasModel, withSomeNullValues);

            var loginFeed = new ODataResourceSet() { Id = new Uri(this.ServiceUri + "Customer(-9)/Logins") };
            if (!hasModel)
            {
                loginFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Login", NavigationSourceEntityTypeName = NameSpace + "Login", NavigationSourceKind = Microsoft.OData.Edm.EdmNavigationSourceKind.EntitySet });
            }

            var loginEntry = WritePayloadHelper.CreateLoginEntry(hasModel);


            // Setting null values for some navigation links, those null links won't be serialized
            // as odata.navigationlink annotations.
            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(
                WritePayloadHelper.CreateCustomerNavigationLinks(withSomeNullValues));
            customerEntry.NestedResourceInfoWrappers = customerEntry.NestedResourceInfoWrappers.Concat(
                new[]{  new ODataNestedResourceInfoWrapper()
                        {
                            NestedResourceInfo = new ODataNestedResourceInfo()
                            {
                                Name = "Logins",
                                IsCollection = true,
                                Url = new Uri(this.ServiceUri + "Customer(-9)/Logins")
                            },
                            NestedResourceOrResourceSet = new ODataResourceSetWrapper()
                            {
                                ResourceSet = loginFeed,
                                Resources = new List<ODataResourceWrapper>()
                                {
                                    new ODataResourceWrapper()
                                    {
                                        Resource = loginEntry,
                                        NestedResourceInfoWrappers = WritePayloadHelper.CreateLoginNavigationLinksWrapper(withSomeNullValues).ToList()
                                    }
                                }
                            }
                        }
                    });

            ODataWriterHelper.WriteResource(odataWriter, customerEntry);
        }

        private string WriteAndVerifyExpandedCustomerEntry(StreamResponseMessage responseMessage,
                                                           ODataWriter odataWriter, bool hasModel, string mimeType)
        {
            WriteCustomerEntry(odataWriter, hasModel, false);

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            int verifyEntryCalled = 0;
            bool verifyNavigationCalled = false;
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                verifyFeedCalled = true;
            };

            Action<ODataResource> verifyEntry = (entry) =>
            {
                if (entry.TypeName.Contains("Customer"))
                {
                    Assert.AreEqual(4, entry.Properties.Count());
                    verifyEntryCalled++;
                }

                if (entry.TypeName.Contains("Login"))
                {
                    Assert.AreEqual(2, entry.Properties.Count());
                    verifyEntryCalled++;
                }
            };

            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.IsNotNull(navigation.Name);
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType,
                                                   verifyFeed, verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyFeedCalled && verifyEntryCalled == 2 && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyExpandedCustomerEntryWithSomeNullValues(StreamResponseMessage responseMessage,
                                                   ODataWriter odataWriter, bool hasModel)
        {
            WriteCustomerEntry(odataWriter, hasModel, /* withSomeNullValues */true);

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            int verifyEntryCalled = 0;
            int verifyNullValuesCount = 0;
            bool verifyNavigationCalled = false;
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                verifyFeedCalled = true;
            };

            Action<ODataResource> verifyEntry = (entry) =>
            {
                if (entry.TypeName.Contains("Customer"))
                {
                    Assert.AreEqual(4, entry.Properties.Count());
                    verifyEntryCalled++;
                }

                if (entry.TypeName.Contains("Login"))
                {
                    Assert.AreEqual(2, entry.Properties.Count());
                    verifyEntryCalled++;
                }

                // Counting restored null property value during response de-serialization.
                verifyNullValuesCount += entry.Properties.Count(p => p.Value == null);
            };

            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.IsNotNull(navigation.Name);
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            stream.Seek(0, SeekOrigin.Begin);
            WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CustomerSet, WritePayloadHelper.CustomerType,
                                                verifyFeed, verifyEntry, verifyNavigation);
            Assert.IsTrue(verifyFeedCalled);
            Assert.IsTrue(verifyEntryCalled == 2);
            Assert.IsTrue(verifyNullValuesCount == 4);
            Assert.IsTrue(verifyNavigationCalled);

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyCarEntry(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                              bool hasModel, string mimeType)
        {
            var carEntry = WritePayloadHelper.CreateCarEntry(hasModel);

            odataWriter.WriteStart(carEntry);

            // Finish writing the entry.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyEntryCalled = false;
            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.AreEqual(4, entry.Properties.Count(), "entry.Properties.Count");
                Assert.IsNotNull(entry.MediaResource, "entry.MediaResource");
                Assert.IsTrue(entry.EditLink.AbsoluteUri.Contains("Car(11)"), "entry.EditLink");
                Assert.IsTrue(entry.ReadLink == null || entry.ReadLink.AbsoluteUri.Contains("Car(11)"), "entry.ReadLink");
                Assert.AreEqual(1, entry.InstanceAnnotations.Count, "entry.InstanceAnnotations.Count");

                verifyEntryCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.CarSet, WritePayloadHelper.CarType, null, verifyEntry,
                                                   null);
                Assert.IsTrue(verifyEntryCalled, "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyPersonFeed(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasModel, string mimeType)
        {
            var personFeed = new ODataResourceSet()
            {
                Id = new Uri(this.ServiceUri + "Person"),
                DeltaLink = new Uri(this.ServiceUri + "Person")
            };
            if (!hasModel)
            {
                personFeed.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Person", NavigationSourceEntityTypeName = NameSpace + "Person" });
            }

            odataWriter.WriteStart(personFeed);

            ODataResource personEntry = WritePayloadHelper.CreatePersonEntry(hasModel);
            odataWriter.WriteStart(personEntry);

            var personNavigation = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-5)/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(personNavigation);
            odataWriter.WriteEnd();

            // Finish writing personEntry.
            odataWriter.WriteEnd();

            ODataResource employeeEntry = WritePayloadHelper.CreateEmployeeEntry(hasModel);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation2);
            odataWriter.WriteEnd();

            // Finish writing employeeEntry.
            odataWriter.WriteEnd();

            ODataResource specialEmployeeEntry = WritePayloadHelper.CreateSpecialEmployeeEntry(hasModel);
            odataWriter.WriteStart(specialEmployeeEntry);

            var specialEmployeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation1);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation2);
            odataWriter.WriteEnd();

            var specialEmployeeNavigation3 = new ODataNestedResourceInfo()
            {
                Name = "Car",
                IsCollection = false,
                Url = new Uri("Person(-10)/" + NameSpace + "SpecialEmployee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(specialEmployeeNavigation3);
            odataWriter.WriteEnd();

            // Finish writing specialEmployeeEntry.
            odataWriter.WriteEnd();

            // Finish writing the feed.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyFeedCalled = false;
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;
            Action<ODataResourceSet> verifyFeed = (feed) =>
            {
                if (mimeType != MimeTypes.ApplicationAtomXml)
                {
                    Assert.IsTrue(feed.DeltaLink.AbsoluteUri.Contains("Person"));
                }
                verifyFeedCalled = true;
            };
            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.IsTrue(entry.EditLink.AbsoluteUri.EndsWith("Person(-5)") ||
                              entry.EditLink.AbsoluteUri.EndsWith("Person(-3)/" + NameSpace + "Employee") ||
                              entry.EditLink.AbsoluteUri.EndsWith("Person(-10)/" + NameSpace + "SpecialEmployee"));
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.IsTrue(navigation.Name == "PersonMetadata" || navigation.Name == "Manager" || navigation.Name == "Car");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(true, responseMessage, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, verifyFeed,
                                                   verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyFeedCalled && verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyEmployeeEntry(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasExpectedType, string mimeType)
        {

            ODataResource employeeEntry = WritePayloadHelper.CreateEmployeeEntry(false);
            ODataResourceSerializationInfo serializationInfo = new ODataResourceSerializationInfo()
            {
                NavigationSourceName = "Person",
                NavigationSourceEntityTypeName = NameSpace + "Person",
            };

            if (hasExpectedType)
            {
                serializationInfo.ExpectedTypeName = NameSpace + "Employee";
            }

            employeeEntry.SetSerializationInfo(serializationInfo);
            odataWriter.WriteStart(employeeEntry);

            var employeeNavigation1 = new ODataNestedResourceInfo()
            {
                Name = "PersonMetadata",
                IsCollection = true,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/PersonMetadata", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation1);
            odataWriter.WriteEnd();

            var employeeNavigation2 = new ODataNestedResourceInfo()
            {
                Name = "Manager",
                IsCollection = false,
                Url = new Uri("Person(-3)/" + NameSpace + "Employee" + "/Manager", UriKind.Relative)
            };
            odataWriter.WriteStart(employeeNavigation2);
            odataWriter.WriteEnd();

            // Finish writing employeeEntry.
            odataWriter.WriteEnd();

            // Some very basic verification for the payload.
            bool verifyEntryCalled = false;
            bool verifyNavigationCalled = false;

            Action<ODataResource> verifyEntry = (entry) =>
            {
                Assert.IsTrue(entry.EditLink.AbsoluteUri.Contains("Person"), "entry.EditLink");
                verifyEntryCalled = true;
            };
            Action<ODataNestedResourceInfo> verifyNavigation = (navigation) =>
            {
                Assert.IsTrue(navigation.Name == "PersonMetadata" || navigation.Name == "Manager", "navigation.Name");
                verifyNavigationCalled = true;
            };

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                WritePayloadHelper.ReadAndVerifyFeedEntryMessage(false, responseMessage, WritePayloadHelper.PersonSet, WritePayloadHelper.PersonType, null,
                                                   verifyEntry, verifyNavigation);
                Assert.IsTrue(verifyEntryCalled && verifyNavigationCalled,
                              "Verification action not called.");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyCollection(StreamResponseMessage responseMessage, ODataWriter odataWriter,
                                                bool hasModel, string mimeType)
        {
            var resourceSet = new ODataResourceSetWrapper()
            {
                ResourceSet = new ODataResourceSet
                {
                    Count = 12,
                    NextPageLink = new Uri("http://localhost")
                },
                Resources = new List<ODataResourceWrapper>()
                {
                    WritePayloadHelper.CreatePrimaryContactODataWrapper()
                }
            };

            if (!hasModel)
            {
                resourceSet.ResourceSet.SetSerializationInfo(new ODataResourceSerializationInfo()
                {
                    ExpectedTypeName = NameSpace + "ContactDetails"
                });
            }

            ODataWriterHelper.WriteResourceSet(odataWriter, resourceSet);

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);
                ODataReader reader = messageReader.CreateODataResourceSetReader(WritePayloadHelper.ContactDetailType);
                bool collectionRead = false;
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceSetEnd)
                    {
                        collectionRead = true;
                    }
                }

                Assert.IsTrue(collectionRead, "collectionRead");
                Assert.AreEqual(ODataReaderState.Completed, reader.State);
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyLinks(StreamResponseMessage responseMessage, ODataMessageWriter messageWriter, string mimeType)
        {
            var links = new ODataEntityReferenceLinks()
            {
                Links = new[]
                        {
                            new ODataEntityReferenceLink() {Url = new Uri(this.ServiceUri + "Order(-10)")},
                            new ODataEntityReferenceLink() {Url = new Uri(this.ServiceUri + "Order(-7)")},
                        },
                NextPageLink = new Uri(this.ServiceUri + "Customer(-10)/Orders/$ref?$skiptoken=-7")
            };

            messageWriter.WriteEntityReferenceLinks(links);

            Stream stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);

                ODataEntityReferenceLinks linksRead = messageReader.ReadEntityReferenceLinks();
                Assert.AreEqual(2, linksRead.Links.Count(), "linksRead.Links.Count");
                Assert.IsNotNull(linksRead.NextPageLink, "linksRead.NextPageLink");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifySingleLink(StreamResponseMessage responseMessage, ODataMessageWriter messageWriter, string mimeType)
        {
            var link = new ODataEntityReferenceLink() { Url = new Uri(this.ServiceUri + "Order(-10)") };

            messageWriter.WriteEntityReferenceLink(link);
            var stream = responseMessage.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };

                ODataMessageReader messageReader = new ODataMessageReader(responseMessage, settings, WritePayloadHelper.Model);

                ODataEntityReferenceLink linkRead = messageReader.ReadEntityReferenceLink();
                Assert.IsTrue(linkRead.Url.AbsoluteUri.Contains("Order(-10)"), "linkRead.Url");
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }

        private string WriteAndVerifyRequestMessage(StreamRequestMessage requestMessageWithoutModel,
                                                    ODataWriter odataWriter, bool hasModel, string mimeType)
        {
            var order = new ODataResource()
            {
                Id = new Uri(this.ServiceUri + "Order(-10)"),
                TypeName = NameSpace + "Order"
            };

            var orderP1 = new ODataProperty { Name = "OrderId", Value = -10 };
            var orderp2 = new ODataProperty { Name = "CustomerId", Value = 8212 };
            var orderp3 = new ODataProperty { Name = "Concurrency", Value = null };
            order.Properties = new[] { orderP1, orderp2, orderp3 };
            if (!hasModel)
            {
                order.SetSerializationInfo(new ODataResourceSerializationInfo() { NavigationSourceName = "Order", NavigationSourceEntityTypeName = NameSpace + "Order" });
                orderP1.SetSerializationInfo(new ODataPropertySerializationInfo() { PropertyKind = ODataPropertyKind.Key });
            }

            odataWriter.WriteStart(order);
            odataWriter.WriteEnd();

            Stream stream = requestMessageWithoutModel.GetStream();
            if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
            {
                stream.Seek(0, SeekOrigin.Begin);
                var settings = new ODataMessageReaderSettings() { BaseUri = this.ServiceUri };
                ODataMessageReader messageReader = new ODataMessageReader(requestMessageWithoutModel, settings,
                                                                          WritePayloadHelper.Model);
                ODataReader reader = messageReader.CreateODataResourceReader(WritePayloadHelper.OrderSet, WritePayloadHelper.OrderType);
                ODataResource entry = null;
                while (reader.Read())
                {
                    if (reader.State == ODataReaderState.ResourceEnd)
                    {
                        entry = reader.Item as ODataResource;
                    }
                }

                Assert.IsTrue(entry.Id.ToString().Contains("Order(-10)"), "entry.Id");
                Assert.AreEqual(2, entry.Properties.Count(), "entry.Properties.Count");
                Assert.AreEqual(ODataReaderState.Completed, reader.State);
            }

            return WritePayloadHelper.ReadStreamContent(stream);
        }
    }
}