//---------------------------------------------------------------------
// <copyright file="UpdateBindingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria;
    using System.Linq;
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>This class contains inner classes that can run as LTM tests.</summary>
    [TestModule]
    public partial class UnitTestModule1
    {
        /// <summary>This is a test class for update functionality on bindings.</summary>
        [Ignore] // Remove Atom
        // [TestClass, TestCase] // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
        public class UpdateBindingTests
        {
            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateBindToCollection_SimpleScenario()
            {
                var payloadBuilder = new PayloadBuilder() { Uri = "/Orders(0) "};

                var atomUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)/$ref",
                        new string[] { "/adsm:ref[@id='http://host/Orders(0)']" }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)",
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(Order).FullName + 
                            "' and atom:id='http://host/Orders(0)' and atom:content/adsm:properties[ads:ID='0' and ads:DollarAmount='20.1']]" })
                };

                var jsonLiteUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)/$ref",
                        new string[] { String.Format("/{0}[odata.id='http://host/Orders(0)']", JsonValidator.ObjectString) }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)",
                        new string[] { String.Format("/{0}[ID=0 and DollarAmount=20.1]",
                                       JsonValidator.ObjectString) })
                };

                UnitTestsUtil.DoInsertsForVariousProviders("/Customers(1)/Orders/$ref", UnitTestsUtil.AtomFormat, payloadBuilder, atomUriAndXPaths, false/*verifyETag*/);
                UnitTestsUtil.DoInsertsForVariousProviders("/Customers(1)/Orders/$ref", UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteUriAndXPaths, false/*verifyETag*/);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateBindToCollection()
            {
                // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                // Convert the below payload to json light and uncomment the combinations running

                /*string[] jsonPayloads = new string[]
                {
                    // Additional properties are ignored
                    "{ some: 'value', uri: '/Orders(0)', other: [] }",
                    // [Astoria-ODataLib-Integration] ODataLib fails if $ref singleton in JSON has more than one uri property, Astoria server uses the last one.
                    // ODataLib now ignores the duplicate properties in WCF DS Server mode and thus this payload works
                    // Multiple uri properties - last value is used (note that the first one is invalid since its pointing to non-existing entity set, so it is not used)
                    "{ uri: '/NonExistant(0)', uri: '/Orders(0)' }",
                    // [Astoria-ODataLib-Integration] ODataLib fails if $ref singleton in JSON has more than one uri property, Astoria server uses the last one.
                    // ODataLib now ignores the duplicate properties in WCF DS Server mode and thus this payload works
                    // Multiple uri properties - last value is used (the first one is completely invalid, but its ignored anyway)
                    "{ uri: null, uri: '/Orders(0)' }",
                    // [Astoria-ODataLib-Integration] ODataLib fails if $ref singleton in JSON has more than one uri property, Astoria server uses the last one.
                    // ODataLib now ignores the duplicate properties in WCF DS Server mode and thus this payload works
                    // Multiple uri properties - last value is used (the first one is completely invalid, but its ignored anyway)
                    "{ uri: {}, uri: '/Orders(0)' }",
                    // [Astoria-ODataLib-Integration] ODataLib fails if $ref singleton in JSON has more than one uri property, Astoria server uses the last one.
                    // ODataLib now ignores the duplicate properties in WCF DS Server mode and thus this payload works
                    // Multiple uri properties - last value is used (the first one is completely invalid, but its ignored anyway)
                    "{ uri: [], uri: '/Orders(0)' }",
                    // [Astoria-ODataLib-Integration] Astoria server uses weird algorithm for combining base and relative URIs from payloads
                    "{ uri: 'Orders(0)' }",
                    // The host in a URI processed by server deserializer is ignored.
                    // Host is ignored in the URI
                    "{ uri: 'http://odata.org/Orders(0)' }",
                };
                var jsonUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)/$ref",
                        new string[] { String.Format("/{0}[uri='http://host/Orders(0)']", JsonValidator.ObjectString) }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)",
                        new string[] { String.Format("/{0}[{1}/type='{2}' and {1}/uri='http://host/Orders(0)' and ID=0 and DollarAmount=20.1]",
                                       JsonValidator.ObjectString,
                                       JsonValidator.Metadata,
                                       typeof(Order).FullName) })
                };
                TestUtil.RunCombinations(jsonPayloads, jsonPayload =>
                {
                    UpdateTests.DoInsertsForVariousProviders("/Customers(1)/$ref/Orders", UnitTestsUtil.JsonLightMimeType, jsonPayload, jsonUriAndXPaths, false/*verifyETag#1#);
                });*/

                string[] atomPayloads = new string[]
                {
                    "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id ='/Orders(0)' />",
                    // Comments and PIs around the ref element are ignored
                    "  <!--some comment-->  <ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Orders(0)' /> \t <?value?>",
                    // Comments and insignificant whitespace are ignored in the ref
                    "<!--some--><ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Orders(0)'/><!--other-->  \r <!--comment-->",
                    // [Astoria-ODataLib-Integration] Astoria server uses weird algorithm for combining base and relative URIs from payloads
                    "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='Orders(0)' />",
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // xml:base is ignored
                    "<ref xml:base='http://odata.org/invalid/' xmlns='http://docs.oasis-open.org/odata/ns/metadata' id ='/Orders(0)' />",
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // xml:base is ignored even if it's invalid on its own (top level xml:base should be absolute URI)
                    "<ref xml:base='invalid' xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Orders(0)' />",
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // xml:base is ignored even if it's invalid on its own
                    // For V3 spec compliance we should not ignore xml:base on the readers
                    //"<ref xml:base='http://invalid:some:test' xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Orders(0)' />",
                    // The host in a URI processed by server deserializer is ignored.
                    // Host is ignored in the URI
                    "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id ='http://odata.org/Orders(0)' />",

                    // [Astoria-ODataLib-Integration] Server treats m:null on $ref uri element as null value and fail.
                    // We decided it's OK to not read the m:null and thus this payload is a valid $link request.
                    "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:null='true' id='/Orders(0)' />",
                    "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:null='invalid' id='/Orders(0)' />",
                };
                var atomUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)/$ref",
                        new string[] { "/adsm:ref[@id='http://host/Orders(0)']" }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/Orders(0)",
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(Order).FullName + 
                            "' and atom:id='http://host/Orders(0)' and atom:content/adsm:properties[ads:ID='0' and ads:DollarAmount='20.1']]" })
                };
                TestUtil.RunCombinations(atomPayloads, atomPayload =>
                {
                    UpdateTests.DoInsertsForVariousProviders("/Customers(1)/Orders/$ref", UnitTestsUtil.AtomFormat, atomPayload, atomUriAndXPaths, false/*verifyETag*/);
                });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateBindToReference_SimpleScenario()
            {
                string uri = "/Customers(2)/BestFriend/$ref";

                var payloadBuilder = new PayloadBuilder() { Uri = "/Customers(0)" };

                var atomUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/BestFriend/$ref",
                        new string[] { "/adsm:ref[@id='http://host/Customers(0)']" }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/BestFriend",
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(0)' and atom:content/adsm:properties[ads:ID='0']]" }),
                };

                var jsonLiteUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/BestFriend/$ref",
                        new string[] { String.Format("/{0}[odata.id='http://host/Customers(0)']", JsonValidator.ObjectString) }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/BestFriend",
                        new string[] { String.Format("/{0}[ID=0]", 
                                    JsonValidator.ObjectString) })
                };

                UpdateTests.DoUpdatesForVariousProviders("PATCH", uri, UnitTestsUtil.AtomFormat, payloadBuilder, atomUriAndXPaths, false, verifyResponsePreference: false);
                UpdateTests.DoUpdatesForVariousProviders("PATCH", uri, UnitTestsUtil.JsonLightMimeType, payloadBuilder, jsonLiteUriAndXPaths, false, verifyResponsePreference: false);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateBindToReference()
            {
                const string uri = "/Customers(2)/BestFriend/$ref";

                // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                // Convert the payloads to json light then uncomment the test running
               /* string[] jsonPayloads = new string[]
                {
                    // Additional properties are ignored
                    "{ some: 'value', uri: '/Customers(0)', other: [] }",
                    // [Astoria-ODataLib-Integration] ODataLib fails if $ref singleton in JSON has more than one uri property, Astoria server uses the last one.
                    // Multiple uri properties - last value is used (note that the first one is invalid since its pointing to non-existing entity set, so it is not used)
                    "{ uri: '/NonExistant(0)', uri: '/Customers(0)' }",
                    // [Astoria-ODataLib-Integration] Astoria server uses weird algorithm for combining base and relative URIs from payloads
                    "{ uri: 'Customers(0)' }",
                    // The host in a URI processed by server deserializer is ignored.
                    // Host is ignored in the URI
                    "{ uri: 'http://odata.org/Customers(0)' }",
                };
                var jsonUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/BestFriend/$ref",
                        new string[] { String.Format("/{0}[uri='http://host/Customers(0)']", JsonValidator.ObjectString) }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/BestFriend",
                        new string[] { String.Format("/{0}[{1}/type='{2}' and {1}/uri='http://host/Customers(0)' and ID=0]", 
                                    JsonValidator.ObjectString, 
                                    JsonValidator.Metadata, 
                                    typeof(Customer).FullName) })
                };
                TestUtil.RunCombinations(jsonPayloads, jsonPayload =>
                {
                    UpdateTests.DoUpdatesForVariousProviders("MERGE", uri, UnitTestsUtil.JsonLightMimeType, jsonPayload, jsonUriAndXPaths, false, verifyResponsePreference: false);
                });*/

                string[] atomPayloads = new string[]
                {
                    // The uri element can be from both 'd' or 'm' namespace
                    //"<ref xmlns='http://docs.oasis-open.org/odata/ns/data'>/Customers(0) />",
                    // Comment and PIs around the uri element are ignored
                    //"  <!--some comment-->  <ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' /Customers(0) /> \t <?value?>",
                    // [Astoria-ODataLib-Integration] Astoria server uses weird algorithm for combining base and relative URIs from payloads
                    //"<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' Customers(0) />",
                    //"<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata'    <!--comment-->/Customers(0) />",
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // xml:base is ignored
                    //"<ref xml:base='http://odata.org/invalid/' xmlns='http://docs.oasis-open.org/odata/ns/metadata'>/Customers(0) />",
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // xml:base is ignored even if it's invalid on its own (top level xml:base should be absolute URI)
                    //"<ref xml:base='invalid' xmlns='http://docs.oasis-open.org/odata/ns/metadata'>/Customers(0) />",
                    // ODataLib now ignores xml:base when running in WCF DS Server
                    // xml:base is ignored even if it's invalid on its own
                    //"<ref xml:base='http://invalid:some:test' xmlns='http://docs.oasis-open.org/odata/ns/metadata'>/Customers(0) />",
                    // The host in a URI processed by server deserializer is ignored.
                    // Host is ignored in the URI
                    //"<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' http://odata.org/Customers(0) />",

                    // [Astoria-ODataLib-Integration] Server treats m:null on $ref uri element as null value and fail.
                    // We decided it's OK to not read the m:null and thus this payload is a valid $link request.
                    //"<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:null='true'>/Customers(0) />",
                    //"<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' xmlns:m='http://docs.oasis-open.org/odata/ns/metadata' m:null='test'>/Customers(0) />",
                };
                var atomUriAndXPaths = new KeyValuePair<string, string[]>[] {
                    new KeyValuePair<string, string[]>(
                        "/Customers(1)/BestFriend/$ref",
                        new string[] { "/adsm:ref[@id='http://host/Customers(0)']" }),
                    new KeyValuePair<string, string[]>(
                        "/Customers(2)/BestFriend",
                        new string[] { "/atom:entry[atom:category/@term='#" + typeof(Customer).FullName + "' and atom:id='http://host/Customers(0)' and atom:content/adsm:properties[ads:ID='0']]" }),
                };
                TestUtil.RunCombinations(atomPayloads, atomPayload =>
                {
                    UpdateTests.DoUpdatesForVariousProviders("PATCH", uri, UnitTestsUtil.AtomFormat, atomPayload, atomUriAndXPaths, false, verifyResponsePreference: false);
                });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutReferencePropertyToNull()
            {
                string uri = "/Customers(1)/BestFriend/$ref";
                string[] xPath = new string[] { "404" };
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("DELETE", uri, UnitTestsUtil.AtomFormat, null, xPath, false, verifyResponsePreference: false);
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("DELETE", uri, UnitTestsUtil.JsonLightMimeType, null, xPath, false, verifyResponsePreference: false);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdatePutUnbindResourceFromManyEndOfRelationship()
            {
                string uri = "/Customers(1)/Orders(1)/$ref";
                string[] xPath = new string[] { "404" };
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("DELETE", uri, UnitTestsUtil.AtomFormat, null, xPath, false, verifyResponsePreference: false);
                UpdateTests.DoUpdatesForVariousProvidersWithOpenMissing("DELETE", uri, UnitTestsUtil.JsonLightMimeType, null, xPath, false, verifyResponsePreference: false);
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void VerifyInvalidScenarios()
            {
                string[] invalidPostUris = new string[] {
                    "/Customers(1)/BestFriend/$ref",
                    "/Customers(1)/Orders(1)/$ref" };

                string[] invalidUpdateUris = new string[] {
                    "/Customers(1)/Orders/$ref",
                    "/Customers(1)/Orders(1)/$ref" };

                string[] invalidDeleteUris = new string[] {
                    "/Customers(1)/Orders/$ref"
                };

                foreach (string uri in invalidPostUris)
                {
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.AtomFormat, "POST", 405);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.JsonLightMimeType, "POST", 405);
                }

                foreach (string uri in invalidUpdateUris)
                {
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.AtomFormat, "PUT", 405);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.JsonLightMimeType, "PUT", 405);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.AtomFormat, "PATCH", 405);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.JsonLightMimeType, "PATCH", 405);
                }

                foreach (string uri in invalidDeleteUris)
                {
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.AtomFormat, "DELETE", 400);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(null, uri, UnitTestsUtil.JsonLightMimeType, "DELETE", 400);
                }

                // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                // Convert the payloads to json light then uncomment the test running
                /*string[] invalidJsonPayloads = new[]
                    {
                        // Missing '
                        "{ odata.id: '/Customers(0) }",
                        // Empty URI - invalid
                        "{ odata.id: '' }",
                        // Null URI - invalid
                        "{ odata.id: null }",
                        // URI - non-string - primitive
                        "{ odata.id: 42 }",
                        // URI - non-string - object
                        "{ odata.id: { uri: 'Customers(0)' } }",
                        // URI - non-string - array
                        "{ odata.id: [ 'Customer(0)' ] }",
                        // Empty object
                        "{ }",
                        // Object without uri property
                        "{ odata.id: 'Customer(0)', test: 'Customer(0)' }",
                        // Non-object - primitive
                        "42",
                        // Non-object - null
                        "null",
                        // Non-object - array
                        "[]",
                    };
               TestUtil.RunCombinations(invalidJsonPayloads, invalidJsonPayload =>
                {
                    UpdateTests.VerifyInvalidRequestForVariousProviders(invalidJsonPayload, "/Customers(2)/BestFriend/$ref", UnitTestsUtil.JsonLightMimeType, "MERGE", 400);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(invalidJsonPayload, "/Customers(2)/BestFriend/$ref", UnitTestsUtil.JsonLightMimeType, "PATCH", 400);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(invalidJsonPayload, "/Customers(2)/Orders/$ref", UnitTestsUtil.JsonLightMimeType, "POST", 400);
                });*/

                string[] invalidXmlPayloads = new[]
                    {
                        // Missing end element
                        "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Customers(0)'>",
                        // Empty element - empty uri - wrong
                        "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id=''/>",
                        // Empty content - empty uri - wrong
                        "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id=''></ref>",
                        // [Astoria-ODataLib-Integration] Astoria server fails on empty URIs from $link payloads
                        // Empty content - empty uri - wrong
                        "<ref xml:base='/Customers(1)' xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='' />",

                        // Wrong name
                        "<Ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Customers(0)' />",
                        "<Ref xmlns='http://docs.oasis-open.org/odata/ns/data' id='/Customers(0)'> />",
                        // Wrong namespace
                        "<ref xmlns='http://docs.oasis-open.org/odata/ns/data_wrong' id='/Customers(0)' />",

                        // Mixed content is invalid
                        "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Customers(0)'<some/> />",
                    };
                TestUtil.RunCombinations(invalidXmlPayloads, invalidXmlPayload =>
                {
                    UpdateTests.VerifyInvalidRequestForVariousProviders(invalidXmlPayload, "/Customers(2)/BestFriend/$ref", UnitTestsUtil.MimeApplicationXml, "PATCH", 400);
                    UpdateTests.VerifyInvalidRequestForVariousProviders(invalidXmlPayload, "/Customers(2)/Orders/$ref", UnitTestsUtil.MimeApplicationXml, "POST", 400);
                });

                // Significant whitespaces are considered part of the value - thus the below payload is invalid (points to invalid entity set "Order  rs(0)")
                string nonExistingXmlPayload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' xml:space='preserve' id='/Orders'/><!--some--><!--other-->  \r <!--comment-->";
                UpdateTests.VerifyInvalidRequestForVariousProviders(nonExistingXmlPayload, "/Customers(2)/BestFriend/$ref", UnitTestsUtil.MimeApplicationXml, "PATCH", 400);
                UpdateTests.VerifyInvalidRequestForVariousProviders(nonExistingXmlPayload, "/Customers(2)/Orders/$ref", UnitTestsUtil.MimeApplicationXml, "POST", 400);

                string validJsonPayload = "{ @odata.id: '/Customers(0)' }";
                string validXmlPayload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='/Customers(0)' />";
                UpdateTests.VerifyInvalidRequestForVariousProviders(validJsonPayload, "/Customers(-1)/BestFriend/$ref", UnitTestsUtil.JsonLightMimeType, "PATCH", 404);
                UpdateTests.VerifyInvalidRequestForVariousProviders(validXmlPayload, "/Customers(-1)/BestFriend/$ref", UnitTestsUtil.MimeApplicationXml, "PATCH", 404);
                UpdateTests.VerifyInvalidRequestForVariousProviders(validJsonPayload, "/Customers(-1)/Orders/$ref", UnitTestsUtil.JsonLightMimeType, "POST", 404);
                UpdateTests.VerifyInvalidRequestForVariousProviders(validXmlPayload, "/Customers(-1)/Orders/$ref", UnitTestsUtil.MimeApplicationXml, "POST", 404);
            }

            private class UpdateBindResultTestCase
            {
                public string Payload { get; set; }
                public string ContentType { get; set; }
                public int[] ExpectedOrderIds { get; set; }
                public int? ExpectedBestFriendId { get; set; }
                public bool HasExpandedEntry { get; set; }
                public Action<Customer> Verification { get; set; }
                public int? ExpectedErrorStatusCode { get; set; }
                public Func<string, bool> OnlyForMethod { get; set; }
                public override string ToString()
                {
                    return this.Payload;
                }
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateBindResultTest()
            {
                var testCases = new[]
                {
                    #region Collection navigation property in JSON
                    // Empty array in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[0]
                    },
                    // Single ref in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { uri: '/Orders(0)' } } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 0 }
                    },
                    // Single ref in JSON pointing to a collection
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { uri: '/Orders' } } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Multiple refs in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { uri: '/Orders(0)' } }, { __metadata: { uri: '/Orders(1)' } } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 0, 1 }
                    },
                    // Single expanded entry in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201 } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 4201 },
                        HasExpandedEntry = true
                    },
                    // Multiple expanded entries in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201 }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4202 } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 4201, 4202 },
                        HasExpandedEntry = true
                    },
                    // Single expanded null entry in JSON in a collection
                    // We took a breaking change and don't allow null items in expanded feeds in JSON.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ null ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        HasExpandedEntry = true
                    },
                    // Expanded null for a collection - should fail
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: null }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        HasExpandedEntry = true
                    },
                    // Mix of expanded and refs in JSON (expanded first)
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201 }, { __metadata: { uri: '/Orders(1)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4202 }, { __metadata: { uri: '/Orders(0)' } } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 4201, 1, 4202, 0 },
                        HasExpandedEntry = true
                    },
                    // Mix of expanded and refs in JSON (ref first)
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { uri: '/Orders(1)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201 }, { __metadata: { uri: '/Orders(0)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4202 } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 1, 4201, 0, 4202 },
                        HasExpandedEntry = true
                    },
                    // Multiple nav. props of the same name, only the last value is used
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { uri: '/Orders(0)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4211 }, { __metadata: { uri: '/Orders(1)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4212 } ]," +
                            "Orders: [ { __metadata: { uri: '/Orders(1)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201 }, { __metadata: { uri: '/Orders(0)' } }, { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4202 } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 1, 4201, 0, 4202 },
                        HasExpandedEntry = true
                    },
                    // Bind with anything after __metadata will fail for PATCH with 400, even if the uri is wrong
                    // [Astoria-ODataLib-Integration] Server doesn't recognize __deferred as a JSON binding, it needs an expanded entry with uri instead.
                    // [Astoria-ODataLib-Integration] Astoria JSON ignores uri for expanded entries in update payloads
                    // We decided to take a breaking change and this will fail due to the invalid URI.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __metadata: { uri: 'http://test:some:other' }, ID: 42 } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method != "POST" && method != "PUT",
                    },
                    // Bind with anything after __metadata in POST is recognized as expanded entry and the uri is ignored
                    // [Astoria-ODataLib-Integration] Server doesn't recognize __deferred as a JSON binding, it needs an expanded entry with uri instead.
                    // [Astoria-ODataLib-Integration] Astoria JSON ignores uri for expanded entries in update payloads
                    // We decided to take a breaking change this will fail due to the invalud URI.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __metadata: { uri: 'http://test:some:other' }, ID: 42 } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method == "POST",
                    },
                    // Bind with just __metadata in POST is recognized as bind and thus fails on invalid uri
                    // Inconsistency in handling invalid URI failures - in JSON we will fail with 500.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __metadata: { uri: 'http://test:some:other' } } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method == "POST",
                    },
                    // PATCH does not allow deep inserts, they require binding only. The below is not recognized as binding (Since there's no uri), so it fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __metadata: { } } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method != "POST" && method != "PUT",
                    },
                    // This is a valid binding in a PATCH
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __metadata: { uri: '/Orders(0)' } } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "POST" && method != "PUT",
                        ExpectedOrderIds = new int[] { 0 }
                    },
                    // Bind to collection with a singleton fails
                    // uri should be ignored for expanded navigation properties.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: { __metadata: { uri: 'http://test:some:other' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                    },
                    #endregion

                    #region Singleton navigation property in JSON
                    // null in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "BestFriend: null }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = null
                    },
                    // ref in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "BestFriend: { __metadata: { uri: '/Customers(0)' } } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = 0
                    },
                    // expanded in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "BestFriend: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4201 } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = 4201,
                        HasExpandedEntry = true
                    },
                    // duplicate in JSON - the last value is used only
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "BestFriend: { __metadata: { uri: '/Customers(1)' } }, " +
                            "BestFriend: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4201 } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = 4201,
                        HasExpandedEntry = true
                    },
                    // Bind with null uri will fail
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, BestFriend: { __metadata: { uri: null } } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                    },
                    // Invalid uri in a binding fails with 500
                    // We decided to keep failing with 500 in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, BestFriend: { __metadata: { uri: 'http://test:some:other' } } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                    },
                    // Bind in a PUT is not allowed in JSON in V2
                    // We decided to allow bindings in PUTs in JSON.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, BestFriend: { __metadata: { uri: '/Customers(0)' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = 0,
                    },
                    // Bind to singleton with a collection fails
                    // uri should be ignored for expanded navigation properties.
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, BestFriend: [ { __metadata: { uri: 'http://test:some:other' } } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                    },
                    // Bind to a singleton with feed URI fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, BestFriend: { __metadata: { uri: '/Customers' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    #endregion

                    #region Both Collection and Singleton navigation property in JSON
                    // empty and null in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [], " +
                            "BestFriend: null }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[0],
                        ExpectedBestFriendId = null
                    },
                    // refs in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { uri: '/Orders(0)' } } ], " +
                            "BestFriend: { __metadata: { uri: '/Customers(0)' } } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 0 },
                        ExpectedBestFriendId = 0
                    },
                    // duplicates in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4202 } ], " +
                            "BestFriend: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4201 }, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4203 } ], " +
                            "BestFriend: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4204 } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 4203 },
                        ExpectedBestFriendId = 4204,
                        HasExpandedEntry = true
                    },
                    #endregion

                    #region Ref inside a deep bind JSON
                    // Single expanded entry in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "BestFriend: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4201, " +
                            "Orders: [ { __metadata: { uri: 'Orders(0)' } } ] } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = 4201,
                        HasExpandedEntry = true,
                        Verification = customer => Assert.AreEqual(0, customer.BestFriend.Orders.First().ID, "The bind should have been applied.")
                    },
                    // Collection expanded entry in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201, " +
                            "Customer: { __metadata: { uri: 'Customers(0)' } } } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 4201 },
                        HasExpandedEntry = true,
                        Verification = customer => Assert.AreEqual(0, customer.Orders.First().Customer.ID, "The bind should have been applied.")
                    },
                    #endregion

                    #region Deep bind inside a deep bind JSON
                    // Single expanded entry in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "BestFriend: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4201, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 1043 } ] } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedBestFriendId = 4201,
                        HasExpandedEntry = true,
                        Verification = customer => Assert.AreEqual(1043, customer.BestFriend.Orders.First().ID, "The bind should have been applied.")
                    },
                    // Collection expanded entry in JSON
                    new UpdateBindResultTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 42, " +
                            "Orders: [ { __metadata: { type: '" + typeof(Order).FullName + "' }, ID: 4201, " +
                            "Customer: { __metadata: { type: '" + typeof(Customer).FullName + "' }, ID: 4202 } } ] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedOrderIds = new int[] { 4201 },
                        HasExpandedEntry = true,
                        Verification = customer => Assert.AreEqual(4202, customer.Orders.First().Customer.ID, "The bind should have been applied.")
                    },
                    #endregion

                    #region Collection navigation property in XML
                    // No href and no body in XML -> the link is effectively ignored
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' />" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Empty href and no body in XML -> fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href=''/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400  // Fails because collection property can't be set to null (weird error message, but it will work)
                    },
                    // Entry is not allowed for singleton
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<entry/>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Ref in XML with empty type is OK - empty type is ignored
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type=''/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML with wrong media type is invalid
                    // ODataLib parses the mime type and fails if the parsing fails.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='app lication/atom+xml'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 415
                    },
                    // Ref in XML with missing slash in type is invalid
                    // ODataLib parses the mime type and fails if the parsing fails.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application atom+xml'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 415
                    },
                    // Ref in XML with wrong media sub type is invalid
                    // ODataLib parses the mime type and fails if the parsing fails.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom xml'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 415
                    },
                    // Ref in XML without semicolon before parameters in type is invalid
                    // ODataLib parses the mime type and fails if the parsing fails.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml type=entry'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 415
                    },
                    // Ref in XML with any type but application/atom+xml (was failing in WCF DS, now it is ignored)
                    // ODataLib ignores links with wrong mime types, this is a relacing change for server (no data corruption).
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+test;type=entry'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[0]
                    },
                    // Ref in XML with any type but application/atom+xml (was failing in WCF DS, now it works, relaxin change)
                    // ODataLib uses case insensitive comparison of mime types (as per spec), so this link is recognized.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='applicatioN/atom+Xml;type=entry'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML with parameter type being empty is OK - the type parameter is ignored
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml;type='/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML without parameter type is OK
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML with invalid parameter type (was failing with WCF DS, now it works and the link is recognized, relaxing change)
                    // ODataLib ignores invalid type parameters and uses the link as if there was no type parameter
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml;type=invalid'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML with multiple type parameters uses the first parameter only
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml;type=feed;type=test'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML with entry type should not fail for a collection property
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml;type=entry'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML which points to a feed - this is in fact ignored if the nav. prop in question is a collection
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[0]
                    },
                    // Ref in XML which points to a feed of wrong type - this is in fact ignored if the nav. prop in question is a collection
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Customers'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[0]
                    },
                    // Ref in XML with feed type should not fail for a collection property
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml;type=feed'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Ref in XML with wrong type (different casing) (was failing with WCF DS, now works with ODL, relaxing change)
                    // ODataLib uses case insensitive comparison, so it recognizes the link in this case.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)' type='application/atom+xml;type=Feed'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1 }
                    },
                    // Empty expanded inline (null)
                    // We decided to relax the behavior and treat empty inline as an empty feed (client does that).
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline /></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[0],
                        HasExpandedEntry = true
                    },
                    // Empty expanded feed
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<feed/>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[0],
                        HasExpandedEntry = true
                    },
                    // Two feeds in one inline - doesn't fail - this is a bug where we read one more node than wanted
                    // We decided to break and don't allow multiple ATOM elements in an expanded navigation property.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<feed/><feed/>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400,
                        HasExpandedEntry = true
                    },
                    // Second element in inline has feed in it - this will work - bug
                    // We decided to break and don't allow multiple ATOM elements in an expanded navigation property.
                    // Since this payload has feed and test elements in ATOM namespace it should fail.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<feed/><test><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                                "</feed></test>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400,
                        HasExpandedEntry = true
                    },
                    // Second element in inline has feed in it - this will work - bug
                    // We decided to break and don't allow multiple ATOM elements in an expanded navigation property.
                    // Since this payload has only feed ATOM element, and then non-ATOM element, it should succeed (empty feed is used)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<feed/><ads:test><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                                "</feed></ads:test>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[0],
                        HasExpandedEntry = true
                    },
                    // Expanded feed with one entry in XML (the href is ignored)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1042 },
                        HasExpandedEntry = true
                    },
                    // Two expanded feeds in one inline in XML - this fails because it tries to read the second entry as if without a feed
                    // Used to fail and with ODL fails as well, since two elements from ATOM are not allowed in inline.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<feed><entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry></feed>" + 
                                "<feed><entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry></feed>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Two expanded feeds in one inline, second wrapped in any element in XML - this actuall works
                    // We decided to break and don't allow multiple ATOM elements in an expanded navigation property.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'><adsm:inline>" +
                                "<feed><entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry></feed>" + 
                                "<test><feed><entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry></feed></test>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400,
                        HasExpandedEntry = true
                    },
                    // Expanded feed with one entry in XML (the empty href is ignored)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href=''><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1042 },
                        HasExpandedEntry = true
                    },
                    // Expanded feed with one entry in XML (the missing href is ignored) with feed type
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=feed'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1042 },
                        HasExpandedEntry = true
                    },
                    // Expanded feed with one entry in XML - with entry type, should fail
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' type='application/atom+xml;type=entry'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Expanded feed with two entries in XML
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1042, 1043 },
                        HasExpandedEntry = true
                    },
                    // Expanded feed with entry and a ref in XML
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 1042, 1 },
                        HasExpandedEntry = true
                    },
                    // Two expanded feeds and two refs in XML
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders' href='Orders(1)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Orders'><adsm:inline><feed>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</feed></adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedOrderIds = new int[] { 0, 1042, 1, 1043 },
                        HasExpandedEntry = true
                    },
                    #endregion

                    #region Singleton navigation property in XML
                    // No href in XML - the link is ignored (using a previous link to set the property to non-null value to verify the behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' />" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 0
                    },
                    // Empty href in XML -> set null (using a previous link to set the property to non-null value to verify the behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href=''/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = null
                    },
                    // Empty inline in XML is recognized as deep insert and thus it won't work in updates
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(1)'>" +
                                "<adsm:inline/>" +
                            "</link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // Empty inline in XML (valid href is ignored) -> set null (using a previous link to set the property to non-null value to verify the behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(1)'>" +
                                "<adsm:inline/>" +
                            "</link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = null,
                        HasExpandedEntry = true
                    },
                    // Empty inline in XML (empty href is ignored) -> set null (using a previous link to set the property to non-null value to verify the behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href=''>" +
                                "<adsm:inline/>" +
                            "</link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = null,
                        HasExpandedEntry = true
                    },
                    // Empty inline in XML (no href is ignored) -> set null (using a previous link to set the property to non-null value to verify the behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='/Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend'>" +
                                "<adsm:inline/>" +
                            "</link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = null,
                        HasExpandedEntry = true
                    },
                    // Empty inline in XML invalid href still fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='http://an:invalid:uri'>" +
                                "<adsm:inline/>" +
                            "</link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Ref in XML
                    // Binding in XML is allowed in all methods (including PUT)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1
                    },
                    // Ref in XML for non-existing property should fail
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/NonExisting' href='Customers(1)'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Ref in XML for non-entity property should fail
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Name' href='Customers(1)'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Ref in XML with wrong type should fail for a singleton property
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)' type='application/atom+xml;type=feed'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Ref in XML with wrong type (different casing) (was failing with WCF DS, now works with ODL, relaxing change)
                    // ODataLib uses case insensitive comparison, so it recognizes the link in this case.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)' type='application/atom+xml;type=Entry'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1
                    },
                    // 2 refs in XML - the second one wins, but this is provider implemented
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1
                    },
                    // Ref pointing to a feed - fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Expanded entry in XML (the href is ignored)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1042,
                        HasExpandedEntry = true
                    },
                    // Expanded entry in XML (missing href is ignored)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1042,
                        HasExpandedEntry = true
                    },
                    // Expanded entry in XML (empty href is ignored)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href=''><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1042,
                        HasExpandedEntry = true
                    },
                    // Expanded entry in XML fails in updates
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // Two expanded entries in XML - both are created and the second one is used (that is a provider implemented behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1043,
                        HasExpandedEntry = true,
                        Verification = customer => Assert.IsTrue(CustomDataContext.customers.Any(c => c.ID == 1042), "Even though the first deep insert is not used, the entity should still get created.")
                    },
                    // Two expanded entries in two inlines in one link in XML - fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline>" +
                            "<adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Two expanded entries in one inline in one link in XML - fails
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Two expanded entries in one inline in one link, second wrapped in any element in XML - works
                    // We decided to break WCF DS and fail on multiple ATOM elements in inline.
                    //  In this case there's entry and test elements from ATOM namespace - so it should fail.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                                "<test><entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry></test>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400,
                        HasExpandedEntry = true
                    },
                    // Two expanded entries in one inline in one link, second wrapped in any element in XML - works
                    // We decided to break WCF DS and fail on multiple ATOM elements in inline.
                    //  In this case there's entry in ATOM namespace and then non-ATOM element, so we read the entry just fine.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                                "<ads:test><entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry></ads:test>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1042,
                        HasExpandedEntry = true
                    },
                    // One ref and one expanded in XML - the second one is used (that is a provider implemented behavior)
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(0)'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1043</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = 1043,
                        HasExpandedEntry = true,
                    },
                    // Binding with invalid URI fails with 400 (in XML)
                    // We decided to make XML fail with 400 for invalid URIs
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='http://an:invalid:uri' />" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Binding with invalid URI fails with 400 (in XML) (Expanded case)
                    // We decided to make XML fail with 400 for invalid URIs
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='http://an:invalid:uri'><adsm:inline>" +
                                "<entry/>" +
                            "</adms:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Non atom element in expand fails
                    // We decided to relax the original server behavior here and ignore any non-atom elements inside the inline element.
                    // this means it looks like an empty inline, which wins over the href and means a null value.
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<adsm:nonatom/>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedBestFriendId = null,
                        HasExpandedEntry = true
                    },
                    // Only feed or entry are allowed in expansion
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<link/>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Feed is not allowed for singleton
                    new UpdateBindResultTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/BestFriend' href='Customers(1)'><adsm:inline>" +
                                "<feed/>" +
                            "</adsm:inline></link>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    #endregion
                };

                TestUtil.RunCombinations(
                    testCases,
                    new string[] { "POST", "PUT", "PATCH" },
                    (testCase, method) =>
                    {
                        // TODO: Fix places where we've lost JsonVerbose coverage to add JsonLight
                        // Convert the previous json verbose payloads to json light then remove this condition
                        if (testCase.ContentType == UnitTestsUtil.JsonLightMimeType)
                        {
                            return;
                        }

                        if (method == "PUT" && testCase.ContentType == UnitTestsUtil.JsonLightMimeType && testCase.OnlyForMethod == null)
                        {
                            // Binding in PUT in JSON fails always.
                            return;
                        }

                        if (testCase.OnlyForMethod != null && !testCase.OnlyForMethod(method))
                        {
                            return;
                        }

                        using (UnitTestsUtil.CreateChangeScope(typeof(CustomDataContext)))
                        {
                            int? expectedStatusCode = testCase.ExpectedErrorStatusCode;
                            if (method != "POST" && testCase.HasExpandedEntry)
                            {
                                expectedStatusCode = 400;
                            }

                            string etag = null;
                            if (method != "POST")
                            {
                                etag = UnitTestsUtil.GetETagFromResponse(typeof(CustomDataContext), "/Customers(0)", testCase.ContentType);
                            }

                            Exception exception = TestUtil.RunCatching(() =>
                                {
                                    using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                                    {
                                        request.DataServiceType = typeof(CustomDataContext);
                                        request.RequestUriString = method == "POST" ? "/Customers" : "/Customers(0)";
                                        request.Accept = testCase.ContentType;
                                        request.HttpMethod = method;
                                        request.IfMatch = etag;
                                        request.SetRequestStreamAsText(testCase.Payload);
                                        request.RequestContentType = testCase.ContentType;
                                        request.SendRequest();
                                    }
                                });

                            if (expectedStatusCode == null)
                            {
                                Assert.IsNull(exception, "The request should have succeeded. Error: {0}", exception);

                                // Customer(0) already has Order(0) and Order(100) connected to it, so for inplace updates those will remain. For resets (insert or replace) they start over.
                                int[] expectedOrderIds =
                                    (method == "POST" || method == "PUT") ? testCase.ExpectedOrderIds :
                                    new int[] { 0, 100 }.Concat(testCase.ExpectedOrderIds == null ? new int[0] : testCase.ExpectedOrderIds).ToArray();

                                Customer customer = CustomDataContext.customers.Single(c => c.ID == (method == "POST" ? 42 : 0));
                                string[] orderIds = customer.Orders.Select(o => o.ID.ToString()).ToArray();
                                Assert.AreEqual(
                                    expectedOrderIds == null ? "" : string.Join(", ", expectedOrderIds.Select(i => i.ToString())),
                                    string.Join(", ", orderIds),
                                    "Orders don't match.");
                                Assert.AreEqual(
                                    testCase.ExpectedBestFriendId,
                                    customer.BestFriend == null ? null : (int?)customer.BestFriend.ID,
                                    "BestFriend doesn't match.");

                                if (testCase.Verification != null)
                                {
                                    testCase.Verification(customer);
                                }
                            }
                            else
                            {
                                UnitTestsUtil.VerifyTestException(exception, expectedStatusCode.Value, null);
                            }
                        }
                    });
            }
        }
    }
}
