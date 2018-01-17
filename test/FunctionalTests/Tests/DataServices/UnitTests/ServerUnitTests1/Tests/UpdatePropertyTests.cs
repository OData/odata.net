//---------------------------------------------------------------------
// <copyright file="UpdatePropertyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.Data.Test.Astoria;
    using System.Linq;
    using System.Xml.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using efpoco = AstoriaUnitTests.EFFK;
    using ocs = AstoriaUnitTests.ObjectContextStubs;

    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    /// <summary>This class contains inner classes that can run as LTM tests.</summary>
    [TestModule]
    public partial class UnitTestModule1
    {
        /// <summary>This is a test class for update functionality on properties.</summary>
        [TestClass, TestCase]
        public class UpdatePropertyTests
        {
            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutError_PropertyValueOnMissingEntity()
            {
                string jsonPayload = "{ value : 'Foo' }";

                UpdateTests.VerifyInvalidRequestForVariousProviders(jsonPayload, "/Customers(-1)/Name", UnitTestsUtil.JsonLightMimeType, "PUT", 404);
            }

            private class UpdatePutPropertyTestCase
            {
                public string Payload { get; set; }
                public string ContentType { get; set; }
                public string ExpectedXPath { get; set; }
                public int? ExpectedErrorStatusCodeForDeclaredProperty { get; set; }
                public int? ExpectedErrorStatusCodeForOpenProperty { get; set; }
                public string RequestUri { get; set; }
                public bool DisableETagVerification { get; set; }
                public override string ToString()
                {
                    return "Payload: " + this.Payload + ", ContentType: " + this.ContentType;
                }
            }

            public class UpdatePropertyOpenTypeContext : CustomRowBasedOpenTypesContext
            {
                public override void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    RowComplexType target = targetResource as RowComplexType;
                    if (target != null)
                    {
                        if (IsCustomerInstance(target))
                        {
                            target.Properties["GuidValue"] = System.Guid.NewGuid();
                        }
                    }

                    target.Properties[propertyName] = propertyValue;
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutPrimitiveProperty()
            {
                var testCases = new[]
                {
                    // Valid property payload in JSON
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ value : 'Foo' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedXPath = "Object/value[text()='Foo']"
                    },
                    // Valid property payload in XML
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\">Foo</ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedXPath = "/adsm:value[text()='Foo']"
                    },
                    // Wrong value type in JSON
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ value : 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                    },
                    // Wrong property name in JSON
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ WrongName : 'Foo' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        // JSON on the other hand fails this because it thinks that the value should be a complex value (if the name of property doesn't match)
                        // and will fail later on when the value doesn't have a type name.
                        // ODataLib can now parse complex values without property wrappers, and fails here because of the missing type name
                        ExpectedErrorStatusCodeForOpenProperty = 400,
                    },
                    // Wrong property name in JSON with null value
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ WrongName : null }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        // JSON on the other hand fails this because it thinks that the value should be a complex value (if the name of property doesn't match)
                        // and will fail later on when the value doesn't have a type name.
                        // ODataLib can now parse complex values without property wrappers, and fails here because of the missing type name
                        ExpectedErrorStatusCodeForOpenProperty = 400,
                    },
                    //// Correct property name in JSON Light
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ @odata.type: 'Edm.String', value : 'Foo' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    // JSON Missing '
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ value : 'Foo }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400,
                    },
                    // JSON not object
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "'Foo'",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400,
                    },
                    // JSON Two top-level properties
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ value : 'Foo', Another : 'Bar' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400,
                    },
                    // XML missing end element
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\">Foo",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400
                    },
                    // XML wrong namespace
                    // Server ignores the namespace when reading property payload in XML.
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data_wrong\">Foo</ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCodeForDeclaredProperty = null,
                        ExpectedErrorStatusCodeForOpenProperty = null
                    },
                    // XML wrong namespace
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name xmlns:ads=\"http://docs.oasis-open.org/odata/ns/metadata\">Foo</ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCodeForDeclaredProperty = null,
                        ExpectedErrorStatusCodeForOpenProperty = null
                    },
                    // No XML element
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "Foo",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400
                    },
                    // XML m:null with wrong value
                    // Server fails with 500 in this case.
                    // ODL will throw FormatException and server won't wrap it.
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name adsm:null='some' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\"></ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCodeForDeclaredProperty = 500,
                        ExpectedErrorStatusCodeForOpenProperty = 500
                    },
                    // XML with complex type
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name adsm:type='" + typeof(Address).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">Foo</ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        // ODataLib fails on different type kinds - Astoria completely ignores the payload type
                        ExpectedErrorStatusCodeForDeclaredProperty = null,
                        // [ODataLib Integration] Updating a complex property without specifying any of its sub-properties now fails
                        ExpectedErrorStatusCodeForOpenProperty = null
                    },
                    // XML with entity type
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name adsm:type='" + typeof(Customer).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">Foo</ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        // ODataLib fails on different type kinds - Astoria completely ignores the payload type
                        ExpectedErrorStatusCodeForDeclaredProperty = null,
                        // Open properties of entity type are not supported
                        ExpectedErrorStatusCodeForOpenProperty = 400
                    },
                    // XML with non-existant type
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name adsm:type='TestModel.NonExistant' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">Foo</ads:Name>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCodeForDeclaredProperty = null,
                        // Open properties must have a valid type name
                        ExpectedErrorStatusCodeForOpenProperty = 400
                    },
                    // JSON update for a key property - fails
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "{ value: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        RequestUri = "/Customers(1)/ID",
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400
                    },
                    // XML update for a key property - fails
                    new UpdatePutPropertyTestCase
                    {
                        Payload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                                  "<ads:Name xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">42</ads:Name>",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        RequestUri = "/Customers(1)/ID",
                        ExpectedErrorStatusCodeForDeclaredProperty = 400,
                        ExpectedErrorStatusCodeForOpenProperty = 400
                    },
                };

                TestUtil.RunCombinations(
                    testCases,
                    new bool[] { false, true },
                    new string[] { "PUT", "PATCH" },
                    (testCase, openProperty, method) =>
                    {
                        RunUpdatePutPropertyTestCase(testCase, openProperty, method, "/Customers(1)/Name");
                    });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutNonStringPrimitiveProperty()
            {
                PayloadBuilder payloadBuilder = new PayloadBuilder() { IsComplex = true }.AddProperty("DollarAmount", 9.95);

                string[] atomXPath = new string[] { "/adsm:value[number(text()) = 9.95]" };

                string[] jsonLiteXPath = new string[] {
                    String.Format("{0}[value[number(text()) = 9.95]]", JsonValidator.ObjectString) };

                var jsonLiteOpenTypesXPath = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>("/Orders(100)/DollarAmount", new string[] { String.Format("{0}[value[number(text()) = 9.95]]", JsonValidator.ObjectString) })
                };

                UpdateTests.DoUpdatesForVariousProviders("PUT", "/Orders(100)/DollarAmount", UnitTestsUtil.MimeApplicationXml, payloadBuilder, atomXPath, false);
                UpdateTests.DoUpdatesForVariousProviders("PATCH", "/Orders(100)/DollarAmount", UnitTestsUtil.MimeApplicationXml, payloadBuilder, atomXPath, false);
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("PUT", "/Orders(100)/DollarAmount", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteXPath, false);
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("PATCH", "/Orders(100)/DollarAmount", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteXPath, false);

                UpdateTests.CustomProviderRequest(typeof(CustomRowBasedOpenTypesContext), "/Orders(100)/DollarAmount", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteOpenTypesXPath, "PUT", false);
                UpdateTests.CustomProviderRequest(typeof(CustomRowBasedOpenTypesContext), "/Orders(100)/DollarAmount", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteOpenTypesXPath, "PATCH", false);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutComplexProperty()
            {
                var testCases = new[]
                    {
                        // JSON Normal payload with the property wrapper
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{@odata.type: '" + typeof(Address).FullName + "', City: 'Redmond'}",
                            ContentType = UnitTestsUtil.JsonLightMimeType
                        },
                        // JSON Normal payload without type name
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{ City: 'Redmond' }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                        },
                        // JSON Normal payload without type name
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{City: 'Redmond', State: 'WA'}",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                        },
                        // JSON Payload 
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{@odata.type: '" + typeof(Address).FullName + "', City: 'Redmond' }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                        },
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{ WrongAddress: \"Foo\" }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            ExpectedErrorStatusCodeForDeclaredProperty = 400,
                            // In this case the payload is read as a top-level complex value without a property wrapper, and thus will fail since the value doesn't have a type
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // Empty object
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{ }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                        },
                        // JSON payload with wrong type name
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{@odata.type: null, City: 'Redmond'}",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            ExpectedErrorStatusCodeForDeclaredProperty = 400,
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // JSON payload with wrong type name
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{@odata.type: 42, City: 'Redmond'}",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            ExpectedErrorStatusCodeForDeclaredProperty = 400,
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // JSON payload with multiple type properties, the first one is completely ignored
                        // WCF DS Server ignores duplicate properties in JSON payloads.
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{ @odata.type: null, @odata.type: '" + typeof(Address).FullName + "'}",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            ExpectedErrorStatusCodeForDeclaredProperty = 400,
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // JSON payload with wrong type name
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{ type: 'Edm.String' City: 'Redmond' }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            ExpectedErrorStatusCodeForDeclaredProperty = null,
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // JSON payload with wrong type name (primitive)
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{@odata.type: 'Edm.String', City: 'Redmond' }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            // It works for declared property because the type from the payload is ignored
                            ExpectedErrorStatusCodeForDeclaredProperty = null,
                            // It will fail for open property because the type is not a complex type
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // JSON payload with wrong type name (non-existant)
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{@odata.type: 'TestModel.NonExistant', City: 'Redmond'}",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                            // ODataLib in the Lax mode will ignore the type in this case and use the expected one.
                            ExpectedErrorStatusCodeForDeclaredProperty = null,
                            // It will fail because the type didn't resolve, but it must for open properties
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // XML payload with type
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "<ads:Address adsm:type='" + typeof(Address).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\"" +
                                        "><ads:City>Redmond</ads:City></ads:Address>",
                            ContentType = UnitTestsUtil.MimeApplicationXml,
                        },
                        // XML payload with entity type
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "<ads:Address adsm:type='" + typeof(Customer).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\"" +
                                        "><ads:City>Duvall</ads:City></ads:Address>",
                            ContentType = UnitTestsUtil.MimeApplicationXml,
                            // Metadata type should be used if exists (i.e. not an open property)
                            ExpectedErrorStatusCodeForDeclaredProperty = null,
                            // This fails because open properties cannot have entity types.
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                        // XML payload with non existant type
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "<ads:Address adsm:type='TestModel.NonExistant' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\"" +
                                        "><ads:City>Duvall</ads:City></ads:Address>",
                            ContentType = UnitTestsUtil.MimeApplicationXml,
                            // If the property is declared the payload type is ignored and the content is read using the expected type
                            ExpectedErrorStatusCodeForDeclaredProperty = null,
                            // This fails because open property values must have type.
                            ExpectedErrorStatusCodeForOpenProperty = 400
                        },
                    };

                TestUtil.RunCombinations(
                    testCases,
                    new bool[] { true },
                    new string[] { "PUT", "PATCH" },
                    (testCase, openProperty, method) =>
                    {
                        // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                        // Convert the previous json verbose payloads to json light then remove this condition
                        if (testCase.ContentType == UnitTestsUtil.JsonLightMimeType)
                        {
                            return;
                        }
                        RunUpdatePutPropertyTestCase(testCase, openProperty, method, "/Customers(1)/Address");
                    });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutOpenCollectionProperty()
            {
                var testCases = new[]
                    {
                        // JSON Normal collection with type
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "{\"@odata.type\":\"Collection(Edm.String)\", \"value\": [] }",
                            ContentType = UnitTestsUtil.JsonLightMimeType,
                        },
                        // XML Normal collection with type
                        new UpdatePutPropertyTestCase
                        {
                            Payload = "<ads:Address adsm:type='Collection(Edm.String)' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\"" +
                                        "></ads:Address>",
                            ContentType = UnitTestsUtil.MimeApplicationXml,
                        },
                    };

                TestUtil.RunCombinations(
                    testCases,
                    new string[] { "PUT", "PATCH" },
                    (testCase, method) =>
                    {
                        RunUpdatePutPropertyTestCase(testCase, true, method, "/Customers(1)/Address");
                    });
            }

            public class UpdateComplexPropertyCallOrderTestContext : CustomRowBasedOpenTypesContext
            {
                public override void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    RowComplexType targetRow = targetResource as RowComplexType;
                    if (targetRow != null && targetRow.TypeName.Contains("Address"))
                    {
                        if (targetRow.Properties.ContainsKey(propertyName))
                        {
                            propertyValue = (string)targetRow.Properties[propertyName] + "_" + propertyValue;
                        }
                    }

                    base.SetValue(targetResource, propertyName, propertyValue);
                }
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateComplexPropertyCallOrderTest()
            {
                var testCases = new[]
                {
                    // JSON payload order State,City
                    new
                    {
                        Payload = "{ State: 'WA', City: 'Redmond' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedPropertyNames = new string[] { "State", "City" },
                        ExpectedPropertyValues = new string[] { "WA", "Redmond" },
                    },
                    // JSON payload order City,State
                    new
                    {
                        Payload = "{ City: 'Redmond', State: 'WA' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedPropertyNames = new string[] { "City", "State" },
                        ExpectedPropertyValues = new string[] { "Redmond", "WA" },
                    },
                    // XML payload order City,State
                    new
                    {
                        Payload = "<ads:Address adsm:type='" + typeof(Address).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                    "<ads:City>Redmond</ads:City>" +
                                    "<ads:State>WA</ads:State>" +
                                  "</ads:Address>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedPropertyNames = new string[] { "City", "State" },
                        ExpectedPropertyValues = new string[] { "Redmond", "WA" },
                    },
                    // XML payload order State,City
                    new
                    {
                        Payload = "<ads:Address adsm:type='" + typeof(Address).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                    "<ads:State>WA</ads:State>" +
                                    "<ads:City>Redmond</ads:City>" +
                                  "</ads:Address>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedPropertyNames = new string[] { "State", "City" },
                        ExpectedPropertyValues = new string[] { "WA", "Redmond" },
                    },
                    // XML payload same property twice, the provider is called for both values in the payload order
                    // each payload property gets its own SetValue call.
                    new
                    {
                        Payload = "<ads:Address adsm:type='" + typeof(Address).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                    "<ads:City>Redmond</ads:City>" +
                                    "<ads:City>Seattle</ads:City>" +
                                  "</ads:Address>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedPropertyNames = new string[] { "City" },
                        ExpectedPropertyValues = new string[] { "Redmond_Seattle" },
                    },
                    // XML payload same property twice and another in between, the provider is called for both values in the payload order
                    // each payload property gets its own SetValue call.
                    new
                    {
                        Payload = "<ads:Address adsm:type='" + typeof(Address).FullName + "' xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\">" +
                                    "<ads:City>Redmond</ads:City>" +
                                    "<ads:State>WA</ads:State>" +
                                    "<ads:City>Seattle</ads:City>" +
                                  "</ads:Address>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedPropertyNames = new string[] { "City", "State" },
                        ExpectedPropertyValues = new string[] { "Redmond_Seattle", "WA" },
                    },
                };

                TestUtil.RunCombinations(
                    testCases,
                    new bool[] { false, true },
                    (testCase, inEntity) =>
                    {
                        using (UnitTestsUtil.CreateChangeScope(typeof(UpdateComplexPropertyCallOrderTestContext)))
                        {
                            string contentType = testCase.ContentType;
                            string payload = testCase.Payload;
                            string requestUri;
                            KeyValuePair<string, string>[] headers = null;
                            if (inEntity)
                            {
                                requestUri = "/Customers";
                                headers = new KeyValuePair<string, string>[0];

                                if (contentType == UnitTestsUtil.JsonLightMimeType)
                                {
                                    // Cut off the surrounding object record
                                    payload = payload.Substring(1, payload.Length - 2);

                                    // Add it as a property into an entity
                                    payload = "{ @odata.type: '" + typeof(Customer).FullName + "', ID: 42, Address: {  @odata.type: '" + typeof(Address).FullName + "'," + payload + "} }";
                                }
                                else
                                {
                                    payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                            "<content type='application/xml'><adsm:properties>" +
                                                "<ads:ID>42</ads:ID>" +
                                                payload +
                                            "</adsm:properties></content>" +
                                        "</entry>";

                                    contentType = UnitTestsUtil.AtomFormat;
                                }
                            }
                            else
                            {
                                if (contentType == UnitTestsUtil.JsonLightMimeType)
                                {
                                    // Cut off the surrounding object record
                                    payload = payload.Substring(1, payload.Length - 2);

                                    // Add it as a property into an entity
                                    payload = "{  @odata.type: '" + typeof(Address).FullName + "'," + payload + " }";
                                }

                                requestUri = "/Customers(0)/Address";
                                string etag = UnitTestsUtil.GetETagFromResponse(typeof(UpdateComplexPropertyCallOrderTestContext), requestUri, testCase.ContentType);
                                headers = new KeyValuePair<string, string>[] { new KeyValuePair<string, string>("If-Match", etag) };
                            }

                            UnitTestsUtil.SendRequestAndVerifyXPath(
                                payload,
                                requestUri,
                                null,
                                typeof(UpdateComplexPropertyCallOrderTestContext),
                                contentType,
                                inEntity ? "POST" : "PUT",
                                headers,
                                true);

                            var customer = UpdateComplexPropertyCallOrderTestContext.customers.Where(c => (int)c.Properties["ID"] == (inEntity ? 42 : 0)).First();
                            var address = (RowComplexType)customer.Properties["Address"];
                            string expected = string.Join(", ", testCase.ExpectedPropertyNames);
                            string actual = string.Join(", ", address.Properties.Select(p => p.Key));
                            Assert.AreEqual(expected, actual, "The list of property names differs.");

                            expected = string.Join(", ", testCase.ExpectedPropertyValues);
                            actual = string.Join(", ", address.Properties.Select(p => p.Value.ToString()));
                            Assert.AreEqual(expected, actual, "The list of property values differs.");
                        }
                    });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateMergeComplexProperty()
            {
                PayloadBuilder payloadBuilder = new PayloadBuilder() { IsComplex = true }
                    .AddComplexProperty("Address", new PayloadBuilder() { TypeName = typeof(Address).FullName }
                        .AddProperty("StreetAddress", "new street address")
                        .AddProperty("City", "Redmond")
                        .AddProperty("State", "Washington"));

                string[] atomXPaths = new string[] {
                   "adsm:value[ads:StreetAddress='new street address' and ads:City='Redmond'" +
                    "and ads:State='Washington']" };

                string[] jsonLiteXPaths = new string[] {
                    String.Format("{0}[StreetAddress='new street address' and City='Redmond' and State='Washington']",
                        JsonValidator.ObjectString) };

                UpdateTests.DoUpdatesForVariousProviders("PATCH", "/Customers(1)/Address", UnitTestsUtil.MimeApplicationXml, payloadBuilder, atomXPaths, true);
                UpdateTests.DoUpdatesForVariousProviders("PATCH", "/Customers(1)/Address", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteXPaths, true);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateReplaceComplexProperty()
            {
                string uri = "/Customers(1)/Address";
                PayloadBuilder payloadBuilder = new PayloadBuilder() { IsComplex = true }
                    .AddProperty("Address", new PayloadBuilder() { TypeName = typeof(Address).FullName, IsComplex = true }
                        .AddProperty("City", "Charlotte")
                        .AddProperty("State", "North Carolina"));

                var atomXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] { "adsm:value[ads:StreetAddress='' and ads:City='Charlotte' and ads:State='North Carolina' and ads:PostalCode='']"
                    })};

                var atomOpenTypesXPath = new KeyValuePair<string, string[]>[] {
                new KeyValuePair<string, string[]>(uri,
                        new string[] { "adsm:value[ads:City='Charlotte' and ads:State='North Carolina']",
                                       "count(/adsm:value/*)=4"
                        })};

                var jsonLiteXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] {
                            String.Format("{0}[StreetAddress='' and City='Charlotte' and State='North Carolina' and PostalCode='']", JsonValidator.ObjectString)
                        })};

                var jsonLiteOpenTypesXPath = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(uri,
                        new string[] {
                            String.Format("{0}[City='Charlotte' and State='North Carolina']", JsonValidator.ObjectString),
                            String.Format("count(/{0}/*)=5", JsonValidator.ObjectString) // counting odata.metadata element
                        })};

                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("PUT", uri, UnitTestsUtil.MimeApplicationXml, payloadBuilder, atomXPaths, true);
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("PUT", uri, UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteXPaths, true);

                UpdateTests.CustomProviderRequest(typeof(CustomRowBasedOpenTypesContext), uri, UnitTestsUtil.MimeApplicationXml, payloadBuilder, atomOpenTypesXPath, "PUT", true);
                UpdateTests.CustomProviderRequest(typeof(CustomRowBasedOpenTypesContext), uri, UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteOpenTypesXPath, "PUT", true);
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutComplexPropertyToNull()
            {
                string jsonPayload1 = "{ \"@odata.type\":\"#AstoriaUnitTests.Stubs.CustomerWithBirthday\", \"Address\": null }";
                // string jsonPayload2 = "{ \"@odata.type\":\"#AstoriaUnitTests.Stubs.Hidden.Customer\", \"Address\": null }";

                string[] jsonXPath = new string[] {
                    String.Format("{0}/Address[@IsNull='true']", JsonValidator.ObjectString) };

                string atomPayload = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>" +
                    "<ads:Address xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\" xmlns:adsm=\"http://docs.oasis-open.org/odata/ns/metadata\" adsm:null='true' />";
                string[] atomXPath = new string[] { "/adsm:value[@adsm:null='true']" };

                TestUtil.RunCombinations(
                    // POCO providers don't have the Customer/Address property
                    UnitTestsUtil.ProviderTypes.Where(p => p != typeof(efpoco.CustomObjectContextPOCO) && p != typeof(efpoco.CustomObjectContextPOCOProxy)),
                    (providerType) =>
                    {
                        if (providerType == typeof(ocs.CustomObjectContext))
                        {
                            // EF provider fails with 500 since it doesn't allow null complex values
                            UnitTestsUtil.VerifyInvalidRequest(jsonPayload1, "/Customers(0)", providerType, UnitTestsUtil.JsonLightMimeType, "PUT", 400);
                            UnitTestsUtil.VerifyInvalidRequest(atomPayload, "/Customers(0)/Address", providerType, UnitTestsUtil.MimeApplicationXml, "PUT", 500);
                        }
                        else
                        {
                            UpdateTests.CustomProviderRequest(providerType, "/Customers(1)/", UnitTestsUtil.JsonLightMimeType, jsonPayload1, jsonXPath, "PUT", true);
                            UpdateTests.CustomProviderRequest(providerType, "/Customers(1)/Address", UnitTestsUtil.MimeApplicationXml, atomPayload, atomXPath, "PUT", true);
                        }
                    });
            }

            [Ignore]
            // TODO: Change the payload of null top-level properties #645
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateMergePrimitivePropertyToNull()
            {
                var payloadBuilder = new PayloadBuilder() { IsComplex = false }
                    .AddProperty("value", null);

                string[] jsonLiteXPath = new string[] { String.Format("{0}", JsonValidator.ValueString) };

                UpdateTests.DoUpdatesForVariousProviders(
                    UnitTestsUtil.ProviderTypes.Where(providerType => providerType != typeof(EFFK.CustomObjectContextPOCO)
                        && providerType != typeof(EFFK.CustomObjectContextPOCOProxy)),
                    "PATCH",
                    "/Customers(1)/Name",
                    UnitTestsUtil.JsonLightMimeType,
                    payloadBuilder,
                    new KeyValuePair<string, string[]>[] { new KeyValuePair<string, string[]>("/Customers(1)/Name", jsonLiteXPath) },
                    true);
            }

            #region PrimitiveDataTypesContext
            public class PrimitiveDataTypesEntity
            {
                public int ID { get; set; }

                public XElement XElementProperty { get; set; }
                public System.Data.Linq.Binary BinaryProperty { get; set; }
            }

            public class PrimitiveDataTypesContext : IUpdatable
            {
                public static IDisposable CreateChangeScope() { return null; }

                public IQueryable<PrimitiveDataTypesEntity> Entities
                {
                    get
                    {
                        return new[]
                        {
                            new PrimitiveDataTypesEntity
                            {
                                ID = 1,
                                XElementProperty = new XElement("root"),
                                BinaryProperty = new System.Data.Linq.Binary(new byte[] { (byte)'a', (byte)'b', (byte)'c' })
                            }
                        }.AsQueryable();
                    }
                }

                #region IUpdatable Members

                public object CreateResource(string containerName, string fullTypeName)
                {
                    throw new NotImplementedException();
                }

                public object GetResource(IQueryable query, string fullTypeName)
                {
                    foreach (var value in query) { return value; }
                    return null;
                }

                public object ResetResource(object resource)
                {
                    throw new NotImplementedException();
                }

                public void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    var property = targetResource.GetType().GetProperty(propertyName);
                    if (propertyValue != null)
                    {
                        Assert.AreSame(property.PropertyType, propertyValue.GetType(), "Value of type {0} is set to a property of type {1}", propertyValue.GetType(), property.PropertyType);
                    }

                    targetResource.GetType().GetProperty(propertyName).SetValue(targetResource, propertyValue, null);
                }

                public object GetValue(object targetResource, string propertyName)
                {
                    throw new NotImplementedException();
                }

                public void SetReference(object targetResource, string propertyName, object propertyValue)
                {
                    throw new NotImplementedException();
                }

                public void AddReferenceToCollection(object targetResource, string propertyName, object resourceToBeAdded)
                {
                    throw new NotImplementedException();
                }

                public void RemoveReferenceFromCollection(object targetResource, string propertyName, object resourceToBeRemoved)
                {
                    throw new NotImplementedException();
                }

                public void DeleteResource(object targetResource)
                {
                    throw new NotImplementedException();
                }

                public void SaveChanges()
                {
                }

                public object ResolveResource(object resource)
                {
                    return resource;
                }

                public void ClearChanges()
                {
                    throw new NotImplementedException();
                }

                #endregion
            }
            #endregion

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutPrimitiveValueDataTypes()
            {
                var testCases = new[]
                {
                    new
                    {
                        PropertyName = "XElementProperty",
                        Payload = "<root />",
                        ContentType = UnitTestsUtil.MimeTextPlain,
                    },
                    new
                    {
                        PropertyName = "BinaryProperty",
                        Payload = "abc",
                        ContentType = UnitTestsUtil.MimeApplicationOctetStream,
                    }
                };

                TestUtil.RunCombinations(
                    testCases,
                    testCase =>
                    {
                        UpdateTests.DoPrimitiveValueUpdates(testCase.Payload, "/Entities(1)/" + testCase.PropertyName + "/$value", null, typeof(PrimitiveDataTypesContext), testCase.ContentType, 0);
                    });
            }
            [Ignore] // Remove Atom
            // [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutPrimitivePropertyDataTypes()
            {
                var testCases = new[]
                {
                    new
                    {
                        PropertyName = "XElementProperty",
                        JsonValue = "'<root2 />'",
                        XmlValue = "&lt;root2 /&gt;",
                    },
                    new
                    {
                        PropertyName = "BinaryProperty",
                        JsonValue = "'YWJk'",
                        XmlValue = "YWJk",
                    }
                };

                TestUtil.RunCombinations(
                    testCases,
                    testCase =>
                    {
                        UnitTestsUtil.SendRequestAndVerifyXPath(
                            "{ \"value\": " + testCase.JsonValue + "}",
                            "/Entities(1)/" + testCase.PropertyName,
                            new KeyValuePair<string, string[]>[0],
                            typeof(PrimitiveDataTypesContext),
                            UnitTestsUtil.JsonLightMimeType,
                            "PUT");
                        UnitTestsUtil.SendRequestAndVerifyXPath(
                            "<ads:" + testCase.PropertyName + " xmlns:ads=\"http://docs.oasis-open.org/odata/ns/data\">" + testCase.XmlValue + "</ads:" + testCase.PropertyName + ">",
                            "/Entities(1)/" + testCase.PropertyName,
                            new KeyValuePair<string, string[]>[0],
                            typeof(PrimitiveDataTypesContext),
                            UnitTestsUtil.MimeApplicationXml,
                            "PUT");
                    });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutPrimitiveValue()
            {
                string payload = "Foo";
                string[] xPath = new string[] { String.Format("{0}[text()='Foo']", JsonValidator.ValueString) };

                UpdateTests.DoPrimitiveValueUpdatesForVariousProvider(payload, "/Customers(1)/Name/$value", xPath, UnitTestsUtil.MimeTextPlain);

                // Wrong content types => 415
                UpdateTests.VerifyInvalidRequestForVariousProviders(payload, "/Customers(1)/Name/$value", "mime/type", "PUT", 415);
                UpdateTests.VerifyInvalidRequestForVariousProviders(payload, "/Customers(1)/Name/$value", UnitTestsUtil.MimeApplicationOctetStream, "PUT", 415);

                // Invalid values => 400
                UpdateTests.VerifyInvalidRequestForVariousProviders("foo", "/Orders(0)/DollarAmount/$value", UnitTestsUtil.MimeTextPlain, "PUT", 400);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutComplexPrimitiveValue()
            {
                string payload = "New York";
                string[] xPath = new string[] { String.Format("{0}[text()='New York']", JsonValidator.ValueString) };

                UpdateTests.DoPrimitiveValueUpdatesForVariousProvider(payload, "/Customers(1)/Address/City/$value", xPath, UnitTestsUtil.MimeTextPlain);
                UpdateTests.VerifyInvalidRequestForVariousProviders(payload, "/Customers(1)/Address/City/$value", "mime/type", "PUT", 415);
                UpdateTests.VerifyInvalidRequestForVariousProviders(payload, "/Customers(1)/Address/City/$value", UnitTestsUtil.MimeApplicationOctetStream, "PUT", 415);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutPrimitiveValueLarge()
            {
                const int length = 5000;
                string payload = new string('a', length);
                string[] xPath = new string[] { String.Format("{0}[text()='" + payload + "']", JsonValidator.ValueString) };

                // Skip EDM, which has constraints on length.
                string uri = "/Customers(1)/Name/$value";
                string contentType = UnitTestsUtil.MimeTextPlain;
                UpdateTests.DoPrimitiveValueUpdatesForVariousProvider(payload, uri, xPath, contentType, 0);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutBinaryPrimitiveValue()
            {
                TypedCustomDataContext<AllTypes>.ClearValues();
                TypedCustomDataContext<AllTypes>.PreserveChanges = true;
                EventHandler handler = new EventHandler((sender, e) =>
                {
                    var context = (TypedCustomDataContext<AllTypes>)sender;
                    var theValue = new AllTypes();

                    theValue.ID = 1;
                    theValue.BinaryType = new byte[] { 1, 2, 3, 4 };

                    context.SetValues(new object[] { theValue });
                });

                TypedCustomDataContext<AllTypes>.ValuesRequested += handler;
                try
                {
                    string payload = "12345";
                    UpdateTests.DoPrimitiveValueUpdates(payload, "/Values(1)/BinaryType/$value",
                        new string[] { String.Format("{0}[text()='Foo']",
                                        JsonValidator.ValueString)
                                 },
                       typeof(TypedCustomDataContext<AllTypes>), UnitTestsUtil.MimeApplicationOctetStream, 0);
                    UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(1)/BinaryType/$value", typeof(TypedCustomDataContext<AllTypes>), "mime/type", "PUT", 415);
                    UnitTestsUtil.VerifyInvalidRequest(payload, "/Values(1)/BinaryType/$value", typeof(TypedCustomDataContext<AllTypes>), UnitTestsUtil.MimeTextPlain, "PUT", 415);
                }
                finally
                {
                    TypedCustomDataContext<AllTypes>.ValuesRequested -= handler;
                }

                TypedCustomDataContext<AllTypes>.PreserveChanges = false;
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutBinaryPrimitiveValueWithFixedContentLength()
            {
                TypedCustomDataContext<AllTypes>.ClearValues();
                TypedCustomDataContext<AllTypes>.PreserveChanges = true;
                EventHandler handler = new EventHandler((sender, e) =>
                {
                    var context = (TypedCustomDataContext<AllTypes>)sender;
                    var theValue = new AllTypes();

                    theValue.ID = 1;
                    theValue.BinaryType = new byte[] { 1, 2, 3, 4 };

                    context.SetValues(new object[] { theValue });
                });

                TypedCustomDataContext<AllTypes>.ValuesRequested += handler;
                try
                {
                    string payLoad = "12345";
                    UpdateTests.DoPrimitiveValueUpdates(payLoad, "/Values(1)/BinaryType/$value",
                        new string[] { String.Format("{0}[text()='Foo']",
                                        JsonValidator.ValueString)
                                 },
                       typeof(TypedCustomDataContext<AllTypes>), UnitTestsUtil.MimeApplicationOctetStream, 5);
                }
                finally
                {
                    TypedCustomDataContext<AllTypes>.ValuesRequested -= handler;
                }

                TypedCustomDataContext<AllTypes>.PreserveChanges = false;
            }

            private static void RunUpdatePutPropertyTestCase(UpdatePutPropertyTestCase testCase, bool openProperty, string method, string requestUri)
            {
                if (testCase.RequestUri != null)
                {
                    requestUri = testCase.RequestUri;
                }

                if (openProperty)
                {
                    if (testCase.ExpectedErrorStatusCodeForOpenProperty.HasValue)
                    {
                        VerifyInvalidPutPropertyTestCase(
                            typeof(UpdatePropertyOpenTypeContext),
                            testCase.Payload,
                            testCase.ContentType,
                            method,
                            requestUri,
                            testCase.ExpectedErrorStatusCodeForOpenProperty.Value,
                            testCase.DisableETagVerification);
                    }
                    else
                    {
                        UpdateTests.CustomProviderRequest(
                            typeof(UpdatePropertyOpenTypeContext),
                            requestUri,
                            testCase.ContentType,
                            testCase.Payload,
                            testCase.ExpectedXPath == null ? null : new string[] { testCase.ExpectedXPath },
                            method,
                            !testCase.DisableETagVerification);
                    }
                }
                else
                {
                    if (testCase.ExpectedErrorStatusCodeForDeclaredProperty.HasValue)
                    {
                        VerifyInvalidPutPropertyTestCase(
                            typeof(CustomDataContext),
                            testCase.Payload,
                            testCase.ContentType,
                            method,
                            requestUri,
                            testCase.ExpectedErrorStatusCodeForDeclaredProperty.Value,
                            testCase.DisableETagVerification);
                    }
                    else
                    {
                        UpdateTests.DoUpdatesForVariousProviders(
                            method,
                            requestUri,
                            testCase.ContentType,
                            testCase.Payload,
                            testCase.ExpectedXPath == null ? (new KeyValuePair<string, string[]>[0]) : (new KeyValuePair<string, string[]>[1] { new KeyValuePair<string, string[]>(requestUri, new string[] { testCase.ExpectedXPath }) }),
                            !testCase.DisableETagVerification,
                            includeOpenTypesProvider: false);
                    }
                }
            }

            private static void VerifyInvalidPutPropertyTestCase(
                Type contextType,
                string payload,
                string contentType,
                string method,
                string requestUri,
                int expectedStatusCode,
                bool disableETagVerification)
            {
                using (UnitTestsUtil.CreateChangeScope(contextType))
                {
                    KeyValuePair<string, string>[] requestHeaders = null;
                    if (!disableETagVerification)
                    {
                        string newUri = UnitTestsUtil.ConvertUri(contextType, requestUri);

                        // Get the etag and if its present, then pass to the PUT request
                        var newFormat = newUri.Contains("$value") ? UnitTestsUtil.MimeTextPlain : contentType;
                        string etag = UnitTestsUtil.GetETagFromResponse(contextType, newUri, newFormat);
                        if (etag != null)
                        {
                            requestHeaders = new[] { new KeyValuePair<string, string>("If-Match", etag) };
                        }
                    }

                    UnitTestsUtil.VerifyInvalidRequest(
                        payload,
                        requestUri,
                        contextType,
                        contentType,
                        method,
                        expectedStatusCode,
                        requestHeaders);
                }
            }
        }
    }
}
