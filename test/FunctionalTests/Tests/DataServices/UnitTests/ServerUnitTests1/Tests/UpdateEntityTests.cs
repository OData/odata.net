//---------------------------------------------------------------------
// <copyright file="UpdateEntityTests.cs" company="Microsoft">
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
    using AstoriaUnitTests.Stubs;
    using Microsoft.Test.ModuleCore;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ocs = AstoriaUnitTests.ObjectContextStubs;

    /// <summary>This class contains inner classes that can run as LTM tests.</summary>
    [TestModule]
    public partial class UnitTestModule1
    {
        /// <summary>This is a test class for update functionality on entities.</summary>
        [Ignore] // Remove Atom
        // [TestClass, TestCase] //  For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
        public class UpdateEntityTests
        {
            #region UpdateEntityFormatTestCase
            private sealed class UpdateEntityFormatTestCase
            {
                public string Payload { get; set; }
                public string ContentType { get; set; }
                public int? ExpectedErrorStatusCode { get; set; }
                public string RequestUri { get; set; }
                public Func<string, bool> OnlyForMethod { get; set; }
                public bool ValidateETag { get; set; }
                public Type ContextType { get; set; }
                public string ETagHeaderValue { get; set; }
                public string ExpectedExceptionMessage { get; set; }
                public override string ToString()
                {
                    return "Payload: " + this.Payload + ", ContentType: " + this.ContentType;
                }
            }
            #endregion

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateEntityFormatTest()
            {
                // this Uri is always invalid due to its length regardless of the UriKind used.
                string invalidUri = new string('a', 0x10000);

                var testCases = new[]
                {
                    #region [Astoria-ODataLib-Integration] Parsing of URLs on OData recognized places may fail, but Astoria server doesn't

                    // in server mode any link but navigation end entity reference link should be ignored
                    new UpdateEntityFormatTestCase
                    {
                        Payload = 
                        "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<link rel='edit' href='" + invalidUri + "' />" +
                            "<link rel='self' href='" + invalidUri + "' />" +
                            "<link rel='edit-media' href='" + invalidUri + "' />" +
                            "<link rel='bogus' href='" + invalidUri + "' />" +
                            "<link rel='http://www.iana.org/assignments/relation/edit' href='" + invalidUri + "' />" +
                            "<link rel='http://www.iana.org/assignments/relation/self' href='" + invalidUri + "' />" +
                            "<link rel='http://www.iana.org/assignments/relation/edit-media' href='" + invalidUri + "' />" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/edit-media/NamedStreamProp' href='" + invalidUri + "' />" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/mediaresource/' href='" + invalidUri + "' />" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/relatedlinks/abc' href='" + invalidUri + "' />" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/scheme/bogus/' href='" + invalidUri + "' />" +
                            "<content type='application/xml'>" +
                            "    <adsm:properties><ads:ID adsm:type='Edm.Int32'>43</ads:ID></adsm:properties>" +
                            "</content>" +
                        "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },

                    // invalid navigation link Uri shoud still be reported
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>43</ads:ID></adsm:properties></content>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/NavProp' href='" + invalidUri + "' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    
                    // invalid entity refefence link in $ref 
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<ref xmlns='http://docs.oasis-open.org/odata/ns/metadata' id='" + invalidUri + "' />",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCode = 500,
                        RequestUri = "/Customers(0)/Orders/$ref",
                        OnlyForMethod = (method) => method == "POST"
                    },
                    // invalid uris in content/@src (MLEs) should be ignored
                    // Note that the exception here is only because there is not a neat web service with BLOB support in the 
                    // test infrastructure. This exception happens after the entity was read and the @src attribute was processed.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = 
                        "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml' src='" + invalidUri + "'/>" +
                        "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400,
                        ExpectedExceptionMessage = "An entry with type 'AstoriaUnitTests.Stubs.Order' was found with a media resource, but this entity type is not a media link entry (MLE). When the type is not an MLE entity, the entry cannot have a media resource."
                    },                    
                    // invalid edit link
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { uri : '" + invalidUri +"' }}, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                    },
                    // invalid entity reference link in navigation property
                    new UpdateEntityFormatTestCase
                    {
                        Payload = string.Format(@"
                        {{ 
                            __metadata: 
                            {{ 
                                type: '{0}'
                            }}, 
                            Customer: {{ __metadata : {{ @odata.id : '{1}' }} }},
                            ID: 42
                        }}", typeof(Order).FullName, invalidUri),
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = method => method == "POST",
                        ExpectedErrorStatusCode = 500
                    },
                    // invalid entity reference link in collection
                    new UpdateEntityFormatTestCase
                    {
                        Payload = string.Format(@"
                        {{ 
                            __metadata: 
                            {{ 
                                type: '{0}'
                            }}, 
                            Orders : [ {{ __metadata : {{ @odata.id : '{1}' }} }} ],
                            ID: 42
                        }}", typeof(Customer).FullName, invalidUri),
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        RequestUri = "/Customers",
                        OnlyForMethod = method => method == "POST",
                        ExpectedErrorStatusCode = 500
                    },
                    // invalid top-level entity reference link
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ @odata.id : '" + invalidUri + "' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        RequestUri = "/Customers(0)/Orders/$ref",
                        OnlyForMethod = (method) => method == "POST"
                    },
                    new UpdateEntityFormatTestCase
                    {
                        Payload = string.Format(@"
                        {{ 
                            __metadata: 
                            {{ 
                                type: '{0}',
                                uri : null, 
                                media_src : '{1}',
                                edit_media : '{1}'
                            }}, 
                            Customer: {{ __deferred: {{ @odata.id: '{1}' }} }},
                            ID: 42
                        }}", typeof(Order).FullName, invalidUri),
                        OnlyForMethod = (method) => method == "POST",
                        ContentType = UnitTestsUtil.JsonLightMimeType
                    },

                    #endregion
                    
                    #region JSON entity top-level payloads
                    // Empty JSON entity
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{}",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = method => method != "POST", // POST requires an ID since 0 is already taken
                    },
                    // Invalid payload for JSON entity
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "42",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid payload for JSON entity
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "null",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid payload for JSON entity
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "[]",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    #endregion

                    #region JSON entity __metadata parsing
                    // Invalid __metadata property value
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: null }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid __metadata property value
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: [] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid __metadata type property value
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: null } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid __metadata type property value
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '' }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    // Invalid __metadata type property value
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: {} } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: 'TestModel.NonExistant' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: 'Edm.String' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Address).FullName + "' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: 'Collection(Edm.Int32)' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // uri has to be a string
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: 42 } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // uri has to be a string
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: {} } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid uri for top-level entity
                    // We decided to take a breaking change and start failing in this case. It used to ignore the invalid URI.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: 'http://foo:some:other' }, ID: 12345 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                    },
                    // For inheritance the type name must be specified
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method != "POST",
                        RequestUri = "/Customers(1)"
                    },
                    // For inheritance the type name must be specified
                    // ODataLib was fixed and reports missing type name as an annotation.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method == "POST",
                        RequestUri = "/Customers"
                    },
                    // For inheritance the type name must be specified
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method != "POST",
                        RequestUri = "/Customers(1)"
                    },
                    // For inheritance the type name must be specified
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '' } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method == "POST",
                        RequestUri = "/Customers"
                    },
                    // Multiple edit links
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: 'http://odata.org/edit1', uri: 'http://odata.org/edit2' }, ID: 4001 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    // Multiple edit links (first one is null)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: null, uri: 'http://odata.org/edit2' }, ID: 4002 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    // Multiple edit links (second one is null)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: 'http://odata.org/edit1', uri: null }, ID: 4003 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    // Multiple edit links (first one is invalid)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { @odata.id: 'http://this:is:invalid', uri: 'http://odata.org/edit1' }, ID: 4004 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    #endregion

                    #region JSON __deferred property parsing
                    // Bind with the deferred syntax will be ignored
                    // ODataLib now ignores __deferred properties inside WCF DS Server
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __deferred: { @odata.id: 'http://foo:some:other' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Bind with the deferred syntax will be ignored
                    // ODataLib now ignores __deferred properties inside WCF DS Server
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __deferred: { @odata.id: 'http://foo:some:other', foo: 'bar' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Bind with the deferred syntax will be ignored
                    // ODataLib now ignores __deferred properties inside WCF DS Server
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __deferred: { } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Bind with the deferred syntax to an open property will be ignored (it doesn't fail!)
                    // ODataLib now implements the backward compatible behavior when in WCF DS Server and ignores open properties with __deferred payload.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, CustomerOpen: { __deferred: { } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ContextType = typeof(CustomRowBasedOpenTypesContext),
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Bind with the deferred syntax to an undeclared property will fail
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, CustomerOpen: { __deferred: { } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                        ExpectedErrorStatusCode = 400
                    },
                    // Bind with the deferred syntax with more than one property will fail, since it's recognized as expanded entry instead of bind.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { foo: null, __deferred: { @odata.id: 'http://foo:some:other' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Bind with the deferred syntax inside an array will fail (POST)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __deferred: { @odata.id: 'http://foo:some:other' } } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        RequestUri = "/Customers",
                        OnlyForMethod = (method) => method == "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // Bind with the deferred syntax inside an array will fail (PATCH)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Customer).FullName + "' }, Orders: [ { __deferred: { @odata.id: 'http://foo:some:other' } } ], ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        RequestUri = "/Customers(0)",
                        ValidateETag = true,
                        OnlyForMethod = (method) => method != "POST" && method != "PUT",
                        ExpectedErrorStatusCode = 400
                    },
                    #endregion

                    #region JSON binding recognition
                    // Something which looks like a bind, but is not recognized as one will still fail due to wrong type names
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { } } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Something which looks like a bind, but is not recognized as one will still fail due to wrong type names (the uri is invalid, so it should fail with 500, but it's ignored in this case)
                    // The bind would be recognized, but type checks are made sooner and may fail (and those incorrectly not recognize the binding)
                    // uri should be ignored for expanded navigation properties.
                    // We decided we won't validate the type names in bindings.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { @odata.id: 'http://foo:some:other', type: 'Edm.String' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // Recognized as a bind later on, but the __metadata parsing doesn't think it's a bind and it validates the type name anyway
                    // We decided we won't validate the type names in bindings.
                    // ODL does recognize this a bind now, but it ignores the type, this is intentional relaxing change.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { @odata.id: '/Customers(0)', type: 'Edm.String' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // This is recognized as a bind, but the type name is still verified, fails due to invalid URI
                    // In JSON we will fail with 500 (as it used to) when finding invalid URI.
                    // ODL does recognize this a bind now, but it ignores the type, this is intentional relaxing change.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { @odata.id: 'http://foo:some:other', type: '" + typeof(Customer).FullName + "' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // This is recognized as a bind, but the type name is still verified
                    // ODL does recognize this a bind now, but it ignores the type, this is intentional relaxing change.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { @odata.id: '/Customers(0)', type: '" + typeof(Customer).FullName + "' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // The following is actually recognized as a bind later on (not in the __metadata parsing), and it fail resolving the uri - so 500
                    // In JSON we will fail with 500 (as it used to) when finding invalid URI.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { @odata.id: 'http://foo:some:other', foo: 'some' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    // The following is actually recognized as a bind later on (not in the __metadata parsing)
                    // ODL does recognize this as a bind now.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ __metadata: { type: '" + typeof(Order).FullName + "' }, Customer: { __metadata: { @odata.id: '/Customers(0)', foo: 'some' } }, ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "PUT",
                    },
                    #endregion

                    #region JSON property payloads
                    // Try to update a key property with correct value, should work (the value is ignored).
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "POST",
                    },
                    // Try to update a key property with null value (even though the actual underlying type is non-nullable), should work (the value is ignored).
                    // WCF DS allows null values for non-nullable properties in updates, we now use annotation on the property to instruct ODL
                    // to ignore the property if the value is null.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ ID: null }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "POST",
                    },
                    // Try to update a key property with wrong value, should work (the value is ignored).
                    // We decided to take a breaking change and fail in the case where the value is not convertible to the target type
                    // even though it would be later ignored.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ ID: \"string\" }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // Try to update a key property with wrong value, this one fails due to backward compat behavior
                    // JSON reader will not convert Boolean or DateTime values to the target type. Since in case of key property
                    // the value will be ignored later on, this test doesn't fail.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ ID: true }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = (method) => method != "POST",
                    },

                    // Send open collection property - should fail
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ ID: 42, OpenProperty: { __metadata: { type:'Collection(Edm.String)' }, results: [] } }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ContextType = typeof(CustomRowBasedOpenTypesContext),
                        ExpectedErrorStatusCode = 400
                    },

                    // Send a null complex property to EF provider - should fail with 500 (POST)
                    new UpdateEntityFormatTestCase{
                        Payload = "{ __metadata: { type:'AstoriaUnitTests.ObjectContextStubs.Types.Customer' }, ID: 42, Address: null }",
                        RequestUri = "/Customers",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ContextType = typeof(ocs.CustomObjectContext),
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method == "POST"
                    },
                    // Send a null complex property to EF provider - should fail with 500 (PUT, PATCH)
                    new UpdateEntityFormatTestCase{
                        Payload = "{ __metadata: { type:'AstoriaUnitTests.ObjectContextStubs.Types.Customer' }, Address: null }",
                        RequestUri = "/Customers(0)",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ContextType = typeof(ocs.CustomObjectContext),
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method != "POST",
                        ValidateETag = true
                    },
                    #endregion

                    #region ATOM entity type parsing
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.NonExistant'/></entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='Edm.String'/></entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Address).FullName + "'/></entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/></entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Invalid type name
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='Collection(Edm.Int32)'/></entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // The first category with the right scheme is used, others are ignored
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/schemE' term='TestModel.NonExistant'/>" +   // This is not the recognized scheme - wrong casing
                            "<category term='TestModel.NonExistant'/>" +   // This is not the recognized scheme - no scheme
                            "<category scheme='' term='TestModel.NonExistant'/>" +   // This is not the recognized scheme - empty scheme
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +   // This is the recognized scheme - this is the type name used
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.NonExistant'/>" +   // Only the first one is used, this one is ignored
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // The first category with the right scheme is used, others are ignored - even if the one recognized has no term, it is still used
                    // ODataLib now uses the first category with the right scheme - even if the term is missing.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/schemE' term='TestModel.NonExistant'/>" +   // This is not the recognized scheme - wrong casing
                            "<category term='TestModel.NonExistant'/>" +   // This is not the recognized scheme - no scheme
                            "<category scheme='' term='TestModel.NonExistant'/>" +   // This is not the recognized scheme - empty scheme
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +   // This is the recognized scheme - this is the type name used
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='TestModel.NonExistant'/>" +   // Only the first one is used, this one is ignored
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // When targeting entity set with inheritance, the type must be specified
                    // ODataLib was fixed and reports missing type name as an annotation.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        RequestUri = "/Customers",
                        OnlyForMethod = method => method == "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // When targeting entity set with inheritance, the type must be specified
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "></entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        RequestUri = "/Customers(0)",
                        OnlyForMethod = method => method != "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // When targeting entity set without inheritance, the type name can be missing (missing == not there, or empty)
                    // ODataLib now allows empty type names.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term=''/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // When targeting entity set without inheritance, the type name can be missing (missing == not there, or empty)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // When targeting entity set without inheritance, the type name can be missing (missing == not there, or empty)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Multiple self links
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>44</ads:ID></adsm:properties></content>" +
                            "<link rel='self' href='http://odata.org/self1' />" +
                            "<link rel='self' href='http://odata.org/self2' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Multiple self links (first one has no href)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>45</ads:ID></adsm:properties></content>" +
                            "<link rel='self' />" +
                            "<link rel='self' href='http://odata.org/self2' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Multiple self links (second one has no href)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>46</ads:ID></adsm:properties></content>" +
                            "<link rel='self' href='http://odata.org/self1' />" +
                            "<link rel='self' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Multiple edit links
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>47</ads:ID></adsm:properties></content>" +
                            "<link rel='edit' href='http://odata.org/edit1' />" +
                            "<link rel='edit' href='http://odata.org/edit2' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Multiple edit links (first one has no href)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>48</ads:ID></adsm:properties></content>" +
                            "<link rel='edit' />" +
                            "<link rel='edit' href='http://odata.org/edit2' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Multiple edit links (second one has no href)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>49</ads:ID></adsm:properties></content>" +
                            "<link rel='edit' href='http://odata.org/edit1' />" +
                            "<link rel='edit' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    #endregion

                    #region ATOM content element parsing
                    // Empty content element with no attributes
                    // ODataLib no treats missing or empty type attribute as text/plain (it doesn't allow text/plain itself though).
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST"  // POST requires a key property value to work correctly
                    },
                    // Empty content element with empty type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST"  // POST requires a key property value to work correctly
                    },
                    // Empty content (whitespace and comment) element with no attributes
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content>    <!--ignored--></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST",  // POST requires a key property value to work correctly
                        // This is a relaxing change, syndication API which was used in V2 would fail on this since it didn't allow comments in text nodes.
                        // With ODataLib we use the same logic to read text values of elements, which allows comments and such (still fails on elements though).
                    },
                    // Non-empty content with no type - should fails
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content>    <ads:foo/></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST",  // POST requires a key property value to work correctly
                        ExpectedErrorStatusCode = 400
                    },
                    // Empty content element with empty type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type=''></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST"  // POST requires a key property value to work correctly
                    }, 
                    // Content element with empty type attribute and only text content
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type=''>   foo   </content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST"  // POST requires a key property value to work correctly
                    },
                    // Empty content element with correct type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='application/xml' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST"  // POST requires a key property value to work correctly
                    },
                    // Empty content element with invalid type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='application/xml/foo' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST",  // POST requires a key property value to work correctly
                        ExpectedErrorStatusCode = 415
                    },
                    // Empty content element with application/atom+xml type attribute
                    // We decided to accept application/atom+xml even on the server.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='application/atom+xml' />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = method => method != "POST",  // POST requires a key property value to work correctly
                    },
                    // Content element with properties with no type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400  // Fails in syndication API as the value of the content element is not expected to contain elements.
                    },
                    // Content element with properties with empty type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type=''><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400  // Fails in syndication API as the value of the content element is not expected to contain elements.
                    },
                    // Content element with properties with invalid type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='application/atom/foo'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 415
                    },
                    // Content element with properties with valid type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Content element with properties with wrong type attribute
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                                "<content type='application/atom+xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    #endregion

                    #region ATOM property payloads
                    // Try to update a key property with correct value, should work (the value is ignored).
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = (method) => method != "POST",
                    },
                    // Try to update a key property with null value (even though the actual underlying type is non-nullable), should work (the value is ignored).
                    // WCF DS allows null values for non-nullable properties in updates, we now use annotation on the property to instruct ODL
                    // to ignore the property if the value is null.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32' adsm:null='true'/></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = (method) => method != "POST",
                    },
                    // Try to update a key property with wrong value, should work (the value is ignored).
                    // We decided to take a breaking change and fail in the case where the value is not convertible to the target type
                    // even though it would be later ignored.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID>true</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = (method) => method != "POST",
                        ExpectedErrorStatusCode = 400
                    },
                    // Send open collection property - should pass
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<content type='application/xml'><adsm:properties><ads:ID>42</ads:ID>" +
                            "<ads:OpenProperty adsm:type='Collection(Edm.String)'/>" +
                            "</adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ContextType = typeof(CustomRowBasedOpenTypesContext),
                    },
                    // Send a null complex property to EF provider - should fail with 500 (POST)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.ObjectContextStubs.Types.Customer'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID>42</ads:ID>" +
                            "<ads:Address adsm:null='true'/>" +
                            "</adsm:properties></content>" +
                            "</entry>",
                        RequestUri = "/Customers",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ContextType = typeof(ocs.CustomObjectContext),
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method == "POST"
                    },
                    // Send a null complex property to EF provider - should fail with 500 (PATCH)
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='#AstoriaUnitTests.ObjectContextStubs.Types.Customer'/>" +
                            "<content type='application/xml'><adsm:properties>" +
                            "<ads:Address adsm:null='true'/>" +
                            "</adsm:properties></content>" +
                            "</entry>",
                        RequestUri = "/Customers(0)",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ContextType = typeof(ocs.CustomObjectContext),
                        ExpectedErrorStatusCode = 500,
                        OnlyForMethod = (method) => method != "POST" && method != "PUT",
                        ValidateETag = true
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

                        if (testCase.OnlyForMethod != null && !testCase.OnlyForMethod(method)) return;
                        string requestUri = testCase.RequestUri ?? (method == "POST" ? "/Orders" : "/Orders(1)");
                        RunUpdateEntityFormatTestCase(
                            new OpenWebDataServiceDefinition() { DataServiceType = testCase.ContextType ?? typeof(CustomDataContext) },
                            testCase,
                            method,
                            requestUri);
                    });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateEntityBindingResourceSetAccessTest()
            {
                var testCases = new[]
                {
                    #region JSON
                    // Update to Orders in JSON should not fail, since there are no Customers anywhere.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Order).FullName + "', ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                    },
                    // Update to Orders in JSON which includes a bind to Customers should fail since we are touching the Customers set which is not visible to us.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Order).FullName + "', Customer: { @odata.type: '" + typeof(Customer).FullName + "', ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 400
                    },
                    #endregion

                    #region ATOM
                    // Update to Orders in ATOM should not fail, since there are no Customers anywhere.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                    },
                    // Astoria Server doesn't check resource set accessibility if a bind operation is done against it with ATOM payload.
                    //   We decided to fix this issue (since it's a security problem) and thus we should fail.
                    // Update to Orders in ATOM which includes a bind to Customers.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Customer' href='/Customers(0)'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400
                    },
                    // Astoria Server doesn't check resource set accessibility if a bind operation is done against it with ATOM payload.
                    //   We decided to fix this issue (since it's a security problem) and thus we should fail.
                    // Update to Orders in ATOM which includes a bind to Customers.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Customer' href=''/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 400,
                    },
                    // Astoria Server doesn't check resource set accessibility if a bind operation is done against it with ATOM payload.
                    //   We decided to fix this issue (since it's a security problem) and thus we should fail.
                    // Update to Orders in ATOM which includes a deep insert to Customers.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Order).FullName + "'/>" +
                            "<link rel='http://docs.oasis-open.org/odata/ns/related/Customer' href='/Customers(0)'><adsm:inline>" +
                                "<entry><category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                    "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>1042</ads:ID></adsm:properties></content>" +
                                "</entry>" + 
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
                    new string[] { "POST", "PATCH" },
                    (testCase, method) =>
                    {
                        if (testCase.OnlyForMethod != null && !testCase.OnlyForMethod(method)) return;
                        string requestUri = testCase.RequestUri ?? (method == "POST" ? "/Orders" : "/Orders(1)");
                        RunUpdateEntityFormatTestCase(
                            new OpenWebDataServiceDefinition()
                            {
                                DataServiceType = testCase.ContextType ?? typeof(CustomRowBasedOpenTypesContext),
                                EntitySetAccessRule = new Dictionary<string, Microsoft.OData.Service.EntitySetRights>
                                {
                                    { "Orders", Microsoft.OData.Service.EntitySetRights.All },
                                    { "Customers", Microsoft.OData.Service.EntitySetRights.None },
                                }
                            },
                            testCase,
                            method,
                            requestUri);
                    });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateEntityETagTest()
            {
                var testCases = new[]
                {
                    #region JSON
                    // JSON entity - causes ETag validation even if there are no properties on it - in JSON we always validation ETags
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        RequestUri = "/Customers(0)",
                        ValidateETag = true,
                        ETagHeaderValue = "W/\"wrongETag\"",
                        ExpectedErrorStatusCode = 412
                    },
                    #endregion

                    #region ATOM
                    // [Astoria-ODataLib-Integration] WCF DS Server doesn't check ETags if an ATOM payload entry has no content and no links (and it's a V1 entry)
                    // We decided to break the old behavior and always check the ETag.
                    // ATOM entity - the entity has no content and no links and it doesn't have MLE or EPM
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        RequestUri = "/Customers(0)",
                        ValidateETag = true,
                        ETagHeaderValue = "W/\"wrongETag\"",
                        ExpectedErrorStatusCode = 412,
                    },
                    // ATOM entity - if the entity has content, the ETag is validated
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<content type='application/xml'/>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        RequestUri = "/Customers(0)",
                        ValidateETag = true,
                        ETagHeaderValue = "W/\"wrongETag\"",
                        ExpectedErrorStatusCode = 412
                    },
                    // ATOM entity - if the entity has content, the ETag is validated
                    // ODataLib accepts content with no type attribute as if it was text/plain.
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<content />" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        RequestUri = "/Customers(0)",
                        ValidateETag = true,
                        ETagHeaderValue = "W/\"wrongETag\"",
                        ExpectedErrorStatusCode = 412
                    },
                    // ATOM entity - if the entity has any link element, the ETag is validated
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<link/>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        RequestUri = "/Customers(0)",
                        ValidateETag = true,
                        ETagHeaderValue = "W/\"wrongETag\"",
                        ExpectedErrorStatusCode = 412
                    },
                    #endregion
                };

                TestUtil.RunCombinations(
                    testCases,
                    new string[] { "PUT", "PATCH" },
                    (testCase, method) =>
                    {
                        if (testCase.OnlyForMethod != null && !testCase.OnlyForMethod(method)) return;
                        string requestUri = testCase.RequestUri ?? "/Orders(1)";
                        RunUpdateEntityFormatTestCase(
                            new OpenWebDataServiceDefinition() { DataServiceType = testCase.ContextType ?? typeof(CustomDataContext) },
                            testCase,
                            method,
                            requestUri);
                    });
            }

            #region ChangeInterceptorEntityFormatTestService
            public class ChangeInterceptorEntityFormatTestService : OpenWebDataService<CustomDataContext>
            {
                public static UpdateOperations LastUpdateOperation { get; set; }

                public static IDisposable CreateChangeScope() { return CustomDataContext.CreateChangeScope(); }

                [ChangeInterceptor("Customers")]
                public void CustomersChangeInterceptor(Customer customer, UpdateOperations operation)
                {
                    Assert.AreNotEqual(UpdateOperations.None, operation, "The operation reported to the change interceptor should never be None.");
                    Assert.AreEqual(UpdateOperations.None, LastUpdateOperation, "The change interceptor was called more than once.");
                    LastUpdateOperation = operation;
                }
            }
            #endregion

            #region ChangeInterceptorEntityFormatTestCase
            private sealed class ChangeInterceptorEntityFormatTestCase
            {
                public string Payload { get; set; }
                public string ContentType { get; set; }
                public Func<string, bool> OnlyForMethod { get; set; }
                public UpdateOperations ExpectedUpdateOperation { get; set; }

                public override string ToString()
                {
                    return this.Payload;
                }
            }
            #endregion

            [TestCategory("Partition2"), TestMethod, Variation]
            public void ChangeInterceptorEntityFormatTest()
            {
                var testCases = new ChangeInterceptorEntityFormatTestCase[]
                {
                    #region JSON
                    // Simple JSON Add - Add is reported
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "', ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = m => m == "POST",
                        ExpectedUpdateOperation = UpdateOperations.Add
                    },
                    // JSON Update with no properties - Change is reported for PUT
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = m => m == "PUT",
                        ExpectedUpdateOperation = UpdateOperations.Change
                    },
                    // We used to not fire the interceptor here, we decided we should do it always.
                    // JSON Update with no properties
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = m => m != "PUT" && m != "POST",
                        ExpectedUpdateOperation = UpdateOperations.Change
                    },
                    // JSON Update with key properties - Change is reported for all operations (even though nothing is actually updated)
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "', ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = m => m != "POST",
                        ExpectedUpdateOperation = UpdateOperations.Change
                    },
                    // JSON Update with only navigation property - Change is reported for all operations
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "', Orders: [] }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        OnlyForMethod = m => m != "POST" && m != "PUT",
                        ExpectedUpdateOperation = UpdateOperations.Change
                    },
                    #endregion

                    #region ATOM
                    // Simple ATOM Add - Add is reported
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = m => m == "POST",
                        ExpectedUpdateOperation = UpdateOperations.Add
                    },
                    // ATOM update with no properties - Change is reported
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = m => m != "POST",
                        ExpectedUpdateOperation = UpdateOperations.Change
                    },
                    // ATOM update with key properties - Change is reported
                    new ChangeInterceptorEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                            "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                            "<content type='application/xml'><adsm:properties><ads:ID adsm:type='Edm.Int32'>42</ads:ID></adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        OnlyForMethod = m => m != "POST",
                        ExpectedUpdateOperation = UpdateOperations.Change
                    },
                    #endregion
                };

                TestUtil.RunCombinations(
                    testCases,
                    new string[] { "POST", "PUT", "PATCH" },
                    (testCase, method) =>
                    {
                        if (testCase.OnlyForMethod != null && !testCase.OnlyForMethod(method)) return;
                        UpdateEntityFormatTestCase updateEntityTestCase = new UpdateEntityFormatTestCase
                        {
                            Payload = testCase.Payload,
                            ContentType = testCase.ContentType,
                            ValidateETag = true,
                            RequestUri = method == "POST" ? "/Customers" : "/Customers(0)"
                        };

                        ChangeInterceptorEntityFormatTestService.LastUpdateOperation = UpdateOperations.None;
                        RunUpdateEntityFormatTestCase(
                            new OpenWebDataServiceDefinition() { DataServiceType = typeof(ChangeInterceptorEntityFormatTestService) },
                            updateEntityTestCase,
                            method,
                            updateEntityTestCase.RequestUri);
                        Assert.AreEqual(testCase.ExpectedUpdateOperation, ChangeInterceptorEntityFormatTestService.LastUpdateOperation, "Wrong update operation reported to the change interceptor.");
                    });
            }

            #region UpdateEntityPropertyCallOrderTestContext
            public class UpdateEntityPropertyCallOrderTestContext : CustomRowBasedOpenTypesContext
            {
                public override void SetValue(object targetResource, string propertyName, object propertyValue)
                {
                    RowComplexType targetRow = targetResource as RowComplexType;
                    if (targetRow != null && targetRow.TypeName.Contains("Customer"))
                    {
                        if (targetRow.Properties.ContainsKey(propertyName))
                        {
                            if (propertyValue is string)
                            {
                                propertyValue = (string)targetRow.Properties[propertyName] + "_" + propertyValue;
                            }
                        }
                    }

                    base.SetValue(targetResource, propertyName, propertyValue);
                }
            }
            #endregion

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateEntityPropertyCallOrderTest()
            {
                var testCases = new[]
                {
                    #region JSON
                    // JSON payload order ID,Name
                    new
                    {
                        Payload = "{@odata.type: '" + typeof(Customer).FullName + "', ID: 42, Name: 'Bart' }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedPropertyNames = new string[] { "ID", "Name" },
                        ExpectedPropertyValues = new string[] { "42", "Bart" },
                    },
                    // JSON payload order Name,ID
                    new
                    {
                        Payload = "{ @odata.type: '" + typeof(Customer).FullName + "', Name: 'Bart', ID: 42 }",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedPropertyNames = new string[] { "Name", "ID" },
                        ExpectedPropertyValues = new string[] { "Bart", "42" },
                    },
                    #endregion

                    #region ATOM
                    // ATOM payload order ID,Name
                    new
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                "<content type='application/xml'><adsm:properties>" +
                                    "<ads:ID adsm:type='Edm.Int32'>42</ads:ID>" +
                                    "<ads:Name>Bart</ads:Name>" +
                                "</adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedPropertyNames = new string[] { "ID", "Name" },
                        ExpectedPropertyValues = new string[] { "42", "Bart" },
                    },
                    // ATOM payload order Name,ID
                    new
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                "<content type='application/xml'><adsm:properties>" +
                                    "<ads:Name>Bart</ads:Name>" +
                                    "<ads:ID adsm:type='Edm.Int32'>42</ads:ID>" +
                                "</adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedPropertyNames = new string[] { "Name", "ID" },
                        ExpectedPropertyValues = new string[] { "Bart", "42" },
                    },
                    // ATOM payload same property twice, the provider is called for both values in the payload order
                    // each payload property gets its own SetValue call.
                    new
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                "<content type='application/xml'><adsm:properties>" +
                                    "<ads:ID adsm:type='Edm.Int32'>42</ads:ID>" +
                                    "<ads:Name>Bart</ads:Name>" +
                                    "<ads:Name>Homer</ads:Name>" +
                                "</adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedPropertyNames = new string[] { "ID", "Name" },
                        ExpectedPropertyValues = new string[] { "42", "Bart_Homer" },
                    },
                    // ATOM payload same property twice and another in between, the provider is called for both values in the payload order
                    // each payload property gets its own SetValue call.
                    new
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + ">" +
                                "<category scheme='http://docs.oasis-open.org/odata/ns/scheme' term='" + typeof(Customer).FullName + "'/>" +
                                "<content type='application/xml'><adsm:properties>" +
                                    "<ads:Name>Bart</ads:Name>" +
                                    "<ads:ID adsm:type='Edm.Int32'>42</ads:ID>" +
                                    "<ads:Name>Homer</ads:Name>" +
                                "</adsm:properties></content>" +
                            "</entry>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedPropertyNames = new string[] { "Name", "ID" },
                        ExpectedPropertyValues = new string[] { "Bart_Homer", "42" },
                    },
                    #endregion
                };

                TestUtil.RunCombinations(
                    testCases,
                    testCase =>
                    {
                        using (UnitTestsUtil.CreateChangeScope(typeof(UpdateEntityPropertyCallOrderTestContext)))
                        {
                            UnitTestsUtil.SendRequestAndVerifyXPath(
                                testCase.Payload,
                                "/Customers",
                                null,
                                typeof(UpdateEntityPropertyCallOrderTestContext),
                                testCase.ContentType,
                                "POST",
                                new KeyValuePair<string, string>[] { },
                                true);

                            var customer = UpdateEntityPropertyCallOrderTestContext.customers.Where(c => (int)c.Properties["ID"] == 42).First();
                            var customerProperties = customer.Properties.Where(p => p.Key != "Address" && p.Key != "GuidValue").ToList();
                            string expected = string.Join(", ", testCase.ExpectedPropertyNames);
                            string actual = string.Join(", ", customerProperties.Select(p => p.Key));
                            Assert.AreEqual(expected, actual, "The list of property names differs.");

                            expected = string.Join(", ", testCase.ExpectedPropertyValues);
                            actual = string.Join(", ", customerProperties.Select(p => p.Value.ToString()));
                            Assert.AreEqual(expected, actual, "The list of property values differs.");
                        }
                    });
            }

            [TestCategory("Partition2"), TestMethod, Variation]
            public void UpdateSentToMetadataUrl()
            {
                var testCases = new[]
                {
                    // Previously in the JSON and XML formats cause asserts and eventually 500 (NRE or something like that).
                    //   We decided to fix this and correctly return 415
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "{}",
                        ContentType = UnitTestsUtil.JsonLightMimeType,
                        ExpectedErrorStatusCode = 415,
                    },
                    // Previously in the JSON and XML formats cause asserts and eventually 500 (NRE or something like that).
                    //   We decided to fix this and correctly return 415
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<ads:Property " + TestUtil.CommonPayloadNamespaces + "/>",
                        ContentType = UnitTestsUtil.MimeApplicationXml,
                        ExpectedErrorStatusCode = 415,
                    },
                    new UpdateEntityFormatTestCase
                    {
                        Payload = "<entry " + TestUtil.CommonPayloadNamespaces + "/>",
                        ContentType = UnitTestsUtil.AtomFormat,
                        ExpectedErrorStatusCode = 415,
                    },
                };

                TestUtil.RunCombinations(
                    testCases,
                    new string[] { "PUT", "PATCH" },
                    (testCase, method) =>
                    {
                        if (testCase.OnlyForMethod != null && !testCase.OnlyForMethod(method)) return;
                        string requestUri = testCase.RequestUri ?? "/$metadata";
                        RunUpdateEntityFormatTestCase(
                            new OpenWebDataServiceDefinition() { DataServiceType = testCase.ContextType ?? typeof(CustomDataContext) },
                            testCase,
                            method,
                            requestUri);
                    });
            }

            #region RunUpdateEntityFormatTestCase
            private static void RunUpdateEntityFormatTestCase(
                OpenWebDataServiceDefinition serviceDefinition,
                UpdateEntityFormatTestCase testCase,
                string method,
                string requestUri)
            {
                using (UnitTestsUtil.CreateChangeScope(serviceDefinition.DataServiceType))
                {
                    KeyValuePair<string, string>[] requestHeaders = null;
                    if (testCase.ValidateETag && method != "POST")
                    {
                        string newUri = UnitTestsUtil.ConvertUri(serviceDefinition.DataServiceType, requestUri);

                        // Get the etag and if its present, then pass to the request
                        var newFormat = newUri.Contains("$value") ? UnitTestsUtil.MimeTextPlain : testCase.ContentType;
                        string etag = testCase.ETagHeaderValue ?? UnitTestsUtil.GetETagFromResponse(serviceDefinition.DataServiceType, newUri, newFormat);
                        if (etag != null)
                        {
                            requestHeaders = new[] { new KeyValuePair<string, string>("If-Match", etag) };
                        }
                    }

                    Exception exception = TestUtil.RunCatching(delegate()
                    {
                        using (TestWebRequest request = serviceDefinition.CreateForInProcess())
                        {
                            request.RequestUriString = requestUri;
                            request.Accept = testCase.ContentType;
                            request.HttpMethod = method;
                            UnitTestsUtil.SetHeaderValues(request, requestHeaders);
                            request.SetRequestStreamAsText(testCase.Payload);
                            request.RequestContentType = testCase.ContentType;
                            request.SendRequest();
                        }
                    });

                    if (testCase.ExpectedErrorStatusCode == null)
                    {
                        Assert.IsNull(exception, "Unexpected exception occured: {0}", exception);
                        Assert.IsNull(testCase.ExpectedExceptionMessage);
                    }
                    else
                    {
                        UnitTestsUtil.VerifyTestException(exception, testCase.ExpectedErrorStatusCode.Value, testCase.ExpectedExceptionMessage);
                    }
                }
            }
            #endregion
        }
    }
}
