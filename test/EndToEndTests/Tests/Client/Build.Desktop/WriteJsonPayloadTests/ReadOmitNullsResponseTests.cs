//---------------------------------------------------------------------
// <copyright file="ReadOmitNullsResponseTest.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.WriteJsonPayloadTests
{
    using System;
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
    /// Generate response by writing various payload with/without omit-value=nulls preference,
    /// and read the response on client side.
    /// Based on sample test provided by Mike Pizzo as omit-values=nulls requirement test cases
    /// related to dynamic properties and annotation-only properties.
    /// </summary>
    [TestClass]
    public class ReadOmitNullsResponseTest : EndToEndTestBase
    {
        public ReadOmitNullsResponseTest()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        public override void CustomTestInitialize()
        {
            WritePayloadHelper.CustomTestInitialize(this.ServiceUri);
        }



        [TestMethod]
        public void ReadWriteNullExpandedNavigationTest_omitNulls_projectedEntity()
        {
            // Expanded entity with projection.
            ReadWriteNullExpandedNavigationTest_omitNulls( /*nestedSelect*/true);
        }

        [TestMethod]
        public void ReadWriteNullExpandedNavigationTest_omitNulls()
        {
            // Expanded entity without projection.
            ReadWriteNullExpandedNavigationTest_omitNulls( /*nestedSelect*/false);
        }

        [TestMethod]
        public void ReadWriteNullExpandedNavigationTest_notOmitNulls()
        {
            // set up model
            EdmModel model = null;
            EdmEntityType employeeType = null;
            EdmEntitySet employeeSet = null;

            SetupModel(out model, out employeeType, out employeeSet);

            Uri serviceUri = new Uri("http://test/");

            string expectedNulls = @"{
            ""@odata.context"":""http://test/$metadata#employees(Name,Title,Dynamic,Address,Manager(),Friend())/$entity"",
            ""Name"":""Fred"",
            ""Title@test.annotation"":""annotationValue"",""Title"":null,
            ""Dynamic"":null,
            ""DynamicAnnotated@test.annotation"":""annotationValue"",""DynamicAnnotated"":null,
            ""Address"":null,""Manager"":null,
            ""Friend"": {""Name"":""FriendOfFred""}
        }";
            expectedNulls = Regex.Replace(expectedNulls, @"\s*", string.Empty, RegexOptions.Multiline);
            Assert.IsTrue(expectedNulls.Contains(serviceUri.ToString()));

            var writeSettings = new ODataMessageWriterSettings() { BaseUri = serviceUri };
            writeSettings.ODataUri = new ODataUriParser(model, serviceUri, new Uri("employees?$select=Name,Title,Dynamic,Address&$expand=Manager,Friend", UriKind.Relative)).ParseUri();

            // validate writing a null navigation property value
            var stream = new MemoryStream();
            var responseMessage = new StreamResponseMessage(stream);
            responseMessage.SetHeader("Preference-Applied", "odata.include-annotations=\"*\"");
            var messageWriter = new ODataMessageWriter(responseMessage, writeSettings, model);
            var writer = messageWriter.CreateODataResourceWriter(employeeSet, employeeType);

            WriteResponse(writer);

            stream.Flush();

            // compare written stream to expected stream
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);
            Assert.AreEqual(expectedNulls, streamReader.ReadToEnd(), "Did not generate expected string when not omitting nulls");

            // validate reading back the stream
            var readSettings = new ODataMessageReaderSettings() { BaseUri = serviceUri, ReadUntypedAsString = false };
            stream.Seek(0, SeekOrigin.Begin);
            var messageReader = new ODataMessageReader(responseMessage, readSettings, model);
            var reader = messageReader.CreateODataResourceReader(employeeSet, employeeType);
            string reading = null;
            bool managerReturned = false;
            bool addressReturned = false;
            int employeeEntityCount = 0;
            ODataResource employeeResource = null;
            ODataResource managerResource = null;
            ODataResource friendResource = null;
            ODataResource addressResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        switch (reading)
                        {
                            case "Manager":
                                // Primitive null value for nested resource "Manager" is reported here.
                                managerResource = reader.Item as ODataResource;
                                managerReturned = true;
                                break;
                            case "Address":
                                // Primitive null value for nested complex value "Address" is reported here.
                                addressResource = reader.Item as ODataResource;
                                addressReturned = true;
                                break;
                            case "Employee":
                                ++employeeEntityCount;

                                // Nested resource "Friend" reading is completed before the reading of top level resource "Employee".
                                if (employeeEntityCount == 1)
                                {
                                    friendResource = reader.Item as ODataResource;
                                }
                                else if (employeeEntityCount == 2)
                                {
                                    employeeResource = reader.Item as ODataResource;
                                }
                                else
                                {
                                    Assert.Fail($"Employee resource entry count is {employeeEntityCount}, should not be more than 2.");
                                }
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

            Assert.IsTrue(employeeEntityCount == 2, $"employeeEntityCount: Expected: 2, Actual: {employeeEntityCount}");

            Assert.IsTrue(addressReturned, "Address is not returned but it should be.");
            Assert.IsNull(addressResource, "Address resource is not null but it should be.");

            Assert.IsTrue(managerReturned, "Manager is not returned but it should be, because payload has its value of null.");
            Assert.IsNull(managerResource, "Manager resource is not null but it should be, because payload has its value of null.");

            Assert.IsNotNull(friendResource, "Friend resource should not be null.");

            VerifyAdditionalProperties(employeeResource);
        }

        [TestMethod]
        public void ReadWriteNullNavigationTest_omitNulls()
        {
            // set up model
            EdmModel model = null;
            EdmEntityType employeeType = null;
            EdmEntitySet employeeSet = null;

            SetupModel(out model, out employeeType, out employeeSet);

            Uri serviceUri = new Uri("http://test/");

            // Requirement: Omit null value for nested resource if omit-values=nulls is specified.
            string expectedNoNulls = @"{
            ""@odata.context"":""http://test/$metadata#employees(Name,Title,Dynamic,Address,Manager,Friend())/$entity"",
            ""Name"":""Fred"",
            ""Title@test.annotation"":""annotationValue"",
            ""DynamicAnnotated@test.annotation"":""annotationValue"",
            ""Friend"": {""Name"":""FriendOfFred""}
        }";

            expectedNoNulls = Regex.Replace(expectedNoNulls, @"\s*", string.Empty, RegexOptions.Multiline);
            Assert.IsTrue(expectedNoNulls.Contains(serviceUri.ToString()));

            var writeSettings = new ODataMessageWriterSettings() { BaseUri = serviceUri };
            writeSettings.ODataUri = new ODataUriParser(model, serviceUri, new Uri("employees?$select=Name,Title,Dynamic,Address,Manager&$expand=Friend", UriKind.Relative)).ParseUri();

            // validate writing a null navigation property value
            var stream = new MemoryStream();
            var responseMessage = new StreamResponseMessage(stream);
            responseMessage.SetHeader("Preference-Applied", "omit-values=nulls,odata.include-annotations=\"*\"");

            var messageWriter = new ODataMessageWriter(responseMessage, writeSettings, model);
            var writer = messageWriter.CreateODataResourceWriter(employeeSet, employeeType);

            WriteResponse(writer);

            stream.Flush();

            // compare written stream to expected stream
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);
            Assert.AreEqual(expectedNoNulls, streamReader.ReadToEnd(), "Did not generate expected string when omitting nulls");

            // validate reading back the stream
            var readSettings = new ODataMessageReaderSettings() { BaseUri = serviceUri, ReadUntypedAsString = false };
            stream.Seek(0, SeekOrigin.Begin);
            var messageReader = new ODataMessageReader(responseMessage, readSettings, model);
            var reader = messageReader.CreateODataResourceReader(employeeSet, employeeType);
            string reading = null;
            bool addressReturned = false;
            bool managerReturned = false;
            int employeeEntityCount = 0;
            ODataResource employeeResource = null;
            ODataResource friendResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        switch (reading)
                        {
                            case "Address":
                                addressReturned = true;
                                break;
                            case "Manager":
                                managerReturned = true;
                                break;
                            case "Employee":
                                ++employeeEntityCount;

                                // Nested resource "Friend" is completed first, and the top level resource "Employee" is completed later.
                                if (employeeEntityCount == 1)
                                {
                                    friendResource = reader.Item as ODataResource;
                                }
                                else if (employeeEntityCount == 2)
                                {
                                    employeeResource = reader.Item as ODataResource;
                                }
                                else
                                {
                                    Assert.Fail($"Employee resource entry count is {employeeEntityCount}, should not be more than 2.");
                                }
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

            Assert.IsTrue(employeeEntityCount == 2, $"employeeEntityCount: Expected: 2, Actual: {employeeEntityCount}");
            Assert.IsFalse(addressReturned, "Address is returned but it should not be.");
            Assert.IsFalse(managerReturned, "Manager is returned but it should not be, because it is navigation link and its value is omitted in payload.");
            Assert.IsNotNull(friendResource, "Friend resource is null but it should not be.");
            VerifyAdditionalProperties(employeeResource);
            Assert.IsTrue(employeeResource.Properties != null);
            Assert.AreEqual(employeeResource.Properties.Count(), 5);
            Assert.IsTrue(employeeResource.Properties.Any(p => p.Name.Equals("Address") && (p.Value == null)));
            Assert.IsFalse(employeeResource.Properties.Any(p => p.Name.Equals("Manager")));
        }

        private void ReadWriteNullExpandedNavigationTest_omitNulls(bool nestedSelect)
        {
            // set up model
            EdmModel model = null;
            EdmEntityType employeeType = null;
            EdmEntitySet employeeSet = null;

            SetupModel(out model, out employeeType, out employeeSet);

            Uri serviceUri = new Uri("http://test/");

            // Requirement: Omit null value for nested resource if omit-values=nulls is specified.
            string expectedNoNulls = nestedSelect
                ? @"{
            ""@odata.context"":""http://test/$metadata#employees(Name,Title,Dynamic,Address,Manager(),Friend(Name,Title))/$entity"",
            ""Name"":""Fred"",
            ""Title@test.annotation"":""annotationValue"",
            ""DynamicAnnotated@test.annotation"":""annotationValue"",
            ""Friend"": {""Name"":""FriendOfFred""}
            }"
                : @"{
            ""@odata.context"":""http://test/$metadata#employees(Name,Title,Dynamic,Address,Manager(),Friend())/$entity"",
            ""Name"":""Fred"",
            ""Title@test.annotation"":""annotationValue"",
            ""DynamicAnnotated@test.annotation"":""annotationValue"",
            ""Friend"": {""Name"":""FriendOfFred""}
            }";

            expectedNoNulls = Regex.Replace(expectedNoNulls, @"\s*", string.Empty, RegexOptions.Multiline);
            Assert.IsTrue(expectedNoNulls.Contains(serviceUri.ToString()));

            var writeSettings = new ODataMessageWriterSettings() { BaseUri = serviceUri };

            Uri uri = nestedSelect
                ? new Uri("employees?$select=Name,Title,Dynamic,Address&$expand=Manager,Friend($select=Name,Title)", UriKind.Relative)
                : new Uri("employees?$select=Name,Title,Dynamic,Address&$expand=Manager,Friend", UriKind.Relative);
            writeSettings.ODataUri = new ODataUriParser(model, serviceUri, uri).ParseUri();

            // validate writing a null navigation property value
            var stream = new MemoryStream();
            var responseMessage = new StreamResponseMessage(stream);
            responseMessage.SetHeader("Preference-Applied", "omit-values=nulls,odata.include-annotations=\"*\"");

            var messageWriter = new ODataMessageWriter(responseMessage, writeSettings, model);
            var writer = messageWriter.CreateODataResourceWriter(employeeSet, employeeType);

            WriteResponse(writer);

            stream.Flush();

            // compare written stream to expected stream
            stream.Seek(0, SeekOrigin.Begin);
            var streamReader = new StreamReader(stream);
            Assert.AreEqual(expectedNoNulls, streamReader.ReadToEnd(), "Did not generate expected string when omitting nulls");

            // validate reading back the stream
            var readSettings = new ODataMessageReaderSettings() { BaseUri = serviceUri, ReadUntypedAsString = false };
            stream.Seek(0, SeekOrigin.Begin);
            var messageReader = new ODataMessageReader(responseMessage, readSettings, model);
            var reader = messageReader.CreateODataResourceReader(employeeSet, employeeType);
            string reading = null;
            bool addressReturned = false;
            bool managerReturned = false;
            int employeeEntityCount = 0;
            ODataResource employeeResource = null;
            ODataResource friendResource = null;
            while (reader.Read())
            {
                switch (reader.State)
                {
                    case ODataReaderState.ResourceEnd:
                        switch (reading)
                        {
                            case "Address":
                                addressReturned = true;
                                break;
                            case "Manager":
                                managerReturned = true;
                                break;
                            case "Employee":
                            case "Friend":
                                ++employeeEntityCount;

                                // Nested resource "Friend" is completed first, and the top level resource "Employee" is completed later.
                                if (employeeEntityCount == 1)
                                {
                                    friendResource = reader.Item as ODataResource;
                                }
                                else if (employeeEntityCount == 2)
                                {
                                    employeeResource = reader.Item as ODataResource;
                                }
                                else
                                {
                                    Assert.Fail($"Employee resource entry count is {employeeEntityCount}, should not be more than 2.");
                                }
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

            Assert.IsTrue(employeeEntityCount == 2, $"employeeEntityCount: Expected: 2, Actual: {employeeEntityCount}");
            Assert.IsFalse(addressReturned, "Address is returned but it should not be.");
            Assert.IsFalse(managerReturned, "Manager is returned but it should not be, because value is omitted in payload.");

            Assert.IsNotNull(friendResource, "Friend resource is null but it should not be.");
            Assert.IsTrue(friendResource.Properties.Count() == (nestedSelect ? 2 : 3));
            Assert.IsTrue(friendResource.Properties.Any(p => (p.Name.Equals("Name", StringComparison.Ordinal) && p.Value.Equals("FriendOfFred"))));
            Assert.IsTrue(friendResource.Properties.Any(p => (p.Name.Equals("Title", StringComparison.Ordinal) && p.Value == null)));
            Assert.IsTrue(nestedSelect
                ? !friendResource.Properties.Any(p => p.Name.Equals("Address", StringComparison.Ordinal))
                : friendResource.Properties.Any(p => (p.Name.Equals("Address", StringComparison.Ordinal) && p.Value == null)));


            Assert.IsNotNull(employeeResource);
            VerifyAdditionalProperties(employeeResource);
            Assert.IsTrue(employeeResource.Properties != null && employeeResource.Properties.Any(p => p.Name.Equals("Manager") && (p.Value == null)),
                "Expanded null entity should be restored if omit-values=nulls.");
        }

        private void SetupModel(out EdmModel model, out EdmEntityType employeeType, out EdmEntitySet employeeSet)
        {
            // set up model
            model = new EdmModel();
            var addressType = model.AddComplexType("test", "Address");
            addressType.AddStructuralProperty("city", EdmPrimitiveTypeKind.String, true);
            employeeType = model.AddEntityType("test", "Employee", null, false, true);
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

            employeeType.AddUnidirectionalNavigation(
                new EdmNavigationPropertyInfo
                {
                    Name = "Friend",
                    TargetMultiplicity = EdmMultiplicity.ZeroOrOne,
                    Target = employeeType
                }
            );

            var container = model.AddEntityContainer("test", "service");

            employeeSet = container.AddEntitySet("employees", employeeType);
        }

        private void WriteResponse(ODataWriter writer)
        {
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
            writer.WriteEnd();
            writer.WriteEnd(); // manager nested info

            // write friend
            writer.WriteStart(new ODataNestedResourceInfo
            {
                Name = "Friend",
                IsCollection = false
            });
            writer.WriteStart(new ODataResource
            {
                Properties = new ODataProperty[]
                {
                    new ODataProperty { Name = "Name", Value = "FriendOfFred" }
                }
            });
            writer.WriteEnd();
            writer.WriteEnd(); // friend nested info

            writer.WriteEnd(); // resource
        }

        private void VerifyAdditionalProperties(ODataResource employeeResource)
        {
            Assert.IsNotNull(employeeResource, "Employee should not be null");
            Assert.IsNotNull(employeeResource.Properties.FirstOrDefault(p => p.Name.Equals("Name", StringComparison.Ordinal)));
            var titleProperty = employeeResource.Properties.FirstOrDefault(p => p.Name.Equals("Title", StringComparison.Ordinal));
            Assert.IsNotNull(titleProperty, "Title property should not be null");
            Assert.IsNull(titleProperty.Value, "Title property value should be null");
            var titleAnnotation = titleProperty.InstanceAnnotations.FirstOrDefault(a => a.Name.Equals("test.annotation", StringComparison.Ordinal));
            Assert.IsNotNull(titleAnnotation, "Title property missing the test.annotation");
            Assert.AreEqual(((ODataPrimitiveValue)titleAnnotation.Value).Value.ToString(), "annotationValue");
            var dynamicProperty = employeeResource.Properties.FirstOrDefault(p => p.Name.Equals("Dynamic", StringComparison.Ordinal));
            Assert.IsNotNull(dynamicProperty, "Dynamic property should not be null");
            Assert.IsNull(dynamicProperty.Value, "Dynamic property value should be null");
            var dynamicAnnotatedProperty = employeeResource.Properties.FirstOrDefault(p => p.Name.Equals("DynamicAnnotated", StringComparison.Ordinal));
            Assert.IsNotNull(dynamicAnnotatedProperty, "DynamicAnnotated property should not be null");
            Assert.IsNull(dynamicAnnotatedProperty.Value, "DynamicAnnotated property value should be null");
            var dynamicAnnotation = dynamicAnnotatedProperty.InstanceAnnotations.FirstOrDefault(a => a.Name.Equals("test.annotation", StringComparison.Ordinal));
            Assert.IsNotNull(dynamicAnnotation, "DynamicAnnotated property missing the test.annotation");
            Assert.AreEqual(((ODataPrimitiveValue)dynamicAnnotation.Value).Value.ToString(), "annotationValue");
        }
    }
}


