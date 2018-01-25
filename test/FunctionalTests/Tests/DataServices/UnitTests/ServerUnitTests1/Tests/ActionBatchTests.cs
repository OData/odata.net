//---------------------------------------------------------------------
// <copyright file="ActionBatchTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using AstoriaUnitTests.Stubs;
using Microsoft.Test.ModuleCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using t = System.Data.Test.Astoria;

namespace AstoriaUnitTests.Tests.Server
{
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    [TestClass]
    public class ActionBatchTests
    {
        private static readonly Version V4 = new Version(4, 0);

        [Ignore] // Ignore this currently, it is for testing the WCF DS.
        // [TestCategory("Partition1"), TestMethod, Variation("Batch tests with actions in them")]
        public void BatchedActionTests()
        {
            #region Test Cases
            var testCases = new SimpleBatchTestCase[]
            {
                // One CS with one action, with an etag in its header, will error
               new SimpleBatchTestCase() {
                   
                    RequestPayload = new BatchInfo(new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive", new KeyValuePair<string,string>[]{new KeyValuePair<string, string>("If-Match", "W/\"2414\"")})))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 400 Bad Request
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""If-Match or If-None-Match HTTP headers cannot be specified for service actions.""}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 0,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with one top level action, with an etag in its header, will error
               new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelActionWithParam_Primitive", new KeyValuePair<string, string>[]{ new KeyValuePair<string, string>("If-None-Match", "W/\"41252\"")}, "\"value\": \"param1\"")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 400 Bad Request
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""If-Match or If-None-Match HTTP headers cannot be specified for service actions.""}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 0,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with a void action
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Void")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with a action returning a primitive
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // One CS with two actions, void and primitive
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Void")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 1,
                },
                // One CS with two actions, primitive and complex
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Complex")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#AstoriaUnitTests.Tests.Actions.ComplexType"",""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 2,
                },
                // One CS with a action with a primitive parameter
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelActionWithParam_Primitive", "\"value\": \"param1\"")))), 
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""param1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // Two CS, each with one action (primitive collection, complex collection).
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_PrimitiveCollection"))),
                        new Changeset(100, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_ComplexCollection")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Collection(Edm.String)"",""value"":[""value1"",""value2""]}
--changesetresponse--
--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 101

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)"",""value"":[{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null},{""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}]}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 2,
                },
                
                // Two CS, each with two actions, some are bound to an entity or entity set
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_PrimitiveCollection")),
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex"))),
                        new Changeset(100, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_ComplexCollection")),
                            new Operation(BatchRequestWritingUtils.GetActionText("Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Collection(Edm.String)"",""value"":[""value1"",""value2""]}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#AstoriaUnitTests.Tests.Actions.ComplexType"",""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}
--changesetresponse--
--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 101

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)"",""value"":[{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null},{""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}]}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 102

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 4,
                    ExpectedGetResultCalls = 4,
                },
                // One CS with an create entity and a top level action
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetPostJsonRequestText("Set2", "{\"ID\": 9091, \"Updated\": false}")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Complex")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 201 Created
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache
Location: http://host/Set2(9091)
ETag: W/""false""

{""@odata.context"":""http://host/$metadata#Set2/$entity"",""@odata.etag"":""W/\""false\"""",""ID"":9091,""Updated"":false}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#AstoriaUnitTests.Tests.Actions.ComplexType"",""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // One CS with an create entity and then an action on the newly created entity (new one is the binding parameter)
                // Dissallowed, the creation fizzles and we only get the 400 from the action
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetPostJsonRequestText("Set", "{\"@odata.type\": \"AstoriaUnitTests.Tests.Actions.EntityType\", \"ID\": 9092, \"Updated\": false, \"PrimitiveProperty\": \"str\", \"ComplexProperty\": null, \"PrimitiveCollectionProperty\": [ \"string1\", \"string2\" ], \"ComplexCollectionProperty\":[]}", "1")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("$1/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 400 Bad Request
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""Batched service action 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive' cannot be invoked because it was bound to an entity created in the same changeset.""}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 0,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with an create entity in a deeper navigation and then an action on the newly created entity (new one is the binding parameter)
                // Dissallowed, the creation fizzles and we only get the 400 from the action
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetPostJsonRequestText("Set(1)/ResourceSetReferenceProperty", "{\"@odata.type\": \"AstoriaUnitTests.Tests.Actions.EntityType\", \"ID\": 9092, \"Updated\": false, \"PrimitiveProperty\": \"str\", \"ComplexProperty\": null, \"PrimitiveCollectionProperty\": [ \"string1\", \"string2\" ], \"ComplexCollectionProperty\":[]}", "1")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("$1/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 400 Bad Request
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""Batched service action 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive' cannot be invoked because it was bound to an entity created in the same changeset.""}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 0,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with an action that returns a string and then another action that tries to use it as a paremeter by cross referencing
                // Cross-referencing doesn't work like that; the $1 should be processed literally (so it technically works)
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelActionWithParam_Primitive", "\"value\": \"$1\"")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""$1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 2,
                },
                // Two CS: 1 with an OK action, the other with is bad. The good changeset should have it's changes persisted
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive"))),
                        new Changeset(100, new Operation(BatchRequestWritingUtils.GetActionText("Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_Primitive", "\"value1\": \"abc\"", "\"value3\": \"def\"")))),
                    ExpectedResponsePayloadContains = new String[] {
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 101

HTTP/1.1 400 Bad Request
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""An error occurred while processing this request."",""innererror"":{""message"":""The parameter 'value3' in the request payload is not a valid parameter for the operation 'ActionOnEntityCollectionWithParam_Primitive_Primitive'."",""type"":""Microsoft.OData.ODataException"",""stacktrace"":""",

@"""}}}
--changesetresponse--
--batchresponse--
"
                    },
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // One CS, lots of OK actions
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Void")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Complex")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_PrimitiveCollection")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_ComplexCollection")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive_Null")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Complex_Null")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_PrimitiveCollection_Null")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_ComplexCollection_Null")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#AstoriaUnitTests.Tests.Actions.ComplexType"",""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 4

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Collection(Edm.String)"",""value"":[""value1"",""value2""]}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 5

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)"",""value"":[{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null},{""PrimitiveProperty"":""complex2"",""ComplexProperty"":{""PrimitiveProperty"":""complex1"",""ComplexProperty"":null}}]}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 6

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 7

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 8

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 9

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 9,
                    ExpectedGetResultCalls = 8,
                },
                // One CS with the same action multiple times in a row
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")),
                            new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 2,
                },
                // Two CS with same action in each
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive"))),
                        new Changeset(100, new Operation(BatchRequestWritingUtils.GetActionText("TopLevelAction_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 101

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 2,
                },            
                // One CS with multiple actions bound to the same entity
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void")),
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity1""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 2,
                    ExpectedGetResultCalls = 1,
                },  
                // Changed error message of test due to change in behavior, Error "Service action 'actionName' 'requires a binding parameter" doesn't happen as ODataPathParser throws error earlier
                // One CS with one action missing its binding parameter
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 404 Not Found
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""Resource not found for the segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive'.""}}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 0,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with one action missing a non-binding nullable parameter
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive")))),
                    ExpectedResponsePayloadContains = new string[] { 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;


--changesetresponse--
--batchresponse--
" 
                    },
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },

                // One CS with one action missing a non-binding parameter
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive", "\"value1\" : []", "\"value2\" : 123", "\"value1\" : []", "\"value2\" : 123")))),
                    ExpectedResponsePayloadContains = new string[] { 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 400 Bad Request
X-Content-Type-Options: nosniff
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""error"":{""code"":"""",""message"":""An error occurred while processing this request."",""innererror"":{""message"":""Multiple parameters with the name 'value1' were found in the request payload."",""type"":""Microsoft.OData.ODataException"",""stacktrace""",
@"""}}}
--changesetresponse--
--batchresponse--
"
                    },
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 0,
                    ExpectedGetResultCalls = 0,
                },
                // One CS with an create entity and then an action on the newly created entity using direct ID
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetPostJsonRequestText("Set", "{\"@odata.type\": \"AstoriaUnitTests.Tests.Actions.EntityType\", \"ID\": 1234, \"Updated\": false, \"PrimitiveProperty\": \"str\", \"ComplexProperty\": null, \"PrimitiveCollectionProperty\": [ \"string1\", \"string2\" ], \"ComplexCollectionProperty\": []}", "1")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(1234)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 201 Created
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache
Location: http://host/Set(1234)
ETag: W/""true""

{""@odata.context"":""http://host/$metadata#Set/$entity"",""@odata.etag"":""W/\""true\"""",""ID"":1234,""Updated"":true,""PrimitiveProperty"":""str"",""ComplexProperty"":null,""PrimitiveCollectionProperty"":[""string1"",""string2""],""ComplexCollectionProperty"":[],""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityWithPath"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollectionWithPath"":{},""#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryableWithPath"":{}}
--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""str""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // One CS with a PUT to an entity and then an action on something unrelated
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetPutJsonRequestText("Set(1)", "{\"@odata.type\": \"AstoriaUnitTests.Tests.Actions.EntityType\", \"ID\": 1, \"Updated\": false, \"PrimitiveProperty\": \"strFromPut1\", \"ComplexProperty\": null, \"PrimitiveCollectionProperty\":[ \"string1\", \"string2\" ], \"ComplexCollectionProperty\": []}")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(2)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;
ETag: W/""false""


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""entity2""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // One CS with a PUT update to an entity and then an action on the same entity
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, 
                            new Operation(BatchRequestWritingUtils.GetPutJsonRequestText("Set(1)", "{\"@odata.type\": \"AstoriaUnitTests.Tests.Actions.EntityType\", \"ID\": 1, \"Updated\": false, \"PrimitiveProperty\": \"strFromPut2\", \"ComplexProperty\": null, \"PrimitiveCollectionProperty\":[ \"string1\", \"string2\" ], \"ComplexCollectionProperty\":[]}")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;
ETag: W/""true""


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""strFromPut2""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // Two CS: 1 with a PUT update to an entity and then a second with an action on the same entity
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0, new Operation(BatchRequestWritingUtils.GetPutJsonRequestText("Set(1)", "{\"@odata.type\": \"AstoriaUnitTests.Tests.Actions.EntityType\", \"ID\": 1, \"Updated\": false, \"PrimitiveProperty\": \"strFromPut3\", \"ComplexProperty\": null, \"PrimitiveCollectionProperty\":[ \"string1\", \"string2\" ], \"ComplexCollectionProperty\": []}"))),
                        new Changeset(100,new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;
ETag: W/""false""


--changesetresponse--
--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 101

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""strFromPut3""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
                // One CS with a PUT update to 1 property of an entity and then an action on the same entity
                new SimpleBatchTestCase() {
                    RequestPayload = new BatchInfo(
                        new Changeset(0,
                            new Operation(BatchRequestWritingUtils.GetPutJsonRequestText("Set(1)/PrimitiveProperty", "{\"value\": \"strFromPut4\"}")), 
                            new Operation(BatchRequestWritingUtils.GetActionText("Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive")))),
                    ExpectedResponsePayloadExact = 
@"--batchresponse
Content-Type: multipart/mixed; boundary=changesetresponse

--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
X-Content-Type-Options: nosniff
Cache-Control: no-cache
OData-Version: 4.0;
ETag: W/""true""


--changesetresponse
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 200 OK
OData-Version: 4.0;
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
X-Content-Type-Options: nosniff
Cache-Control: no-cache

{""@odata.context"":""http://host/$metadata#Edm.String"",""value"":""strFromPut4""}
--changesetresponse--
--batchresponse--
",
                    ResponseStatusCode = 202,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    RequestDataServiceVersion = V4,
                    RequestMaxDataServiceVersion = V4,
                    ExpectedInvokeCalls = 1,
                    ExpectedGetResultCalls = 1,
                },
            };

            #endregion

            List<object> failedTestCases = new List<object>();
            var service = ActionTests.ModelWithActions();
            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                try
                {
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.HttpMethod = "POST";
                        request.RequestUriString = "/$batch";
                        request.Accept = UnitTestsUtil.MimeMultipartMixed;
                        request.RequestVersion = testCase.RequestDataServiceVersion.ToString();
                        request.RequestMaxVersion = testCase.RequestMaxDataServiceVersion.ToString();
                        request.ForceVerboseErrors = true;
                        if (testCase.RequestPayload == null)
                        {
                            request.RequestContentLength = 0;
                        }
                        else
                        {
                            const string boundary = "batch-set";
                            request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                            request.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(testCase.RequestPayload, boundary));
                        }

                        int IDataServiceInvokable_Invoke_Calls = 0;
                        int IDataServiceInvokable_GetResult_Calls = 0;
                        service.ActionProvider.ServiceActionInvokeCallback = (parameters) => { IDataServiceInvokable_Invoke_Calls++; };
                        service.ActionProvider.ServiceActionGetResultCallback = () => { IDataServiceInvokable_GetResult_Calls++; };

                        Exception e = t.TestUtil.RunCatching(request.SendRequest);
                        string response = request.GetResponseStreamAsText();

                        Assert.AreEqual(request.ResponseStatusCode, testCase.ResponseStatusCode);

                        Assert.AreEqual(testCase.ResponseVersion.ToString(), request.ResponseVersion);
                        Assert.AreEqual(testCase.ResponseETag, request.ResponseETag);

                        // strip off the guid from the boundries for comparison purposes
                        Regex regex1 = new Regex(@"batchresponse_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}");
                        response = regex1.Replace(response, "batchresponse");
                        Regex regex2 = new Regex(@"changesetresponse_\w{8}-\w{4}-\w{4}-\w{4}-\w{12}");
                        response = regex2.Replace(response, "changesetresponse");

                        // Make exact comparision if given that, or otherwise do a series of str contains
                        if (testCase.ExpectedResponsePayloadExact != null)
                        {
                            Assert.AreEqual(testCase.ExpectedResponsePayloadExact, response);
                        }
                        else
                        {
                            foreach (string str in testCase.ExpectedResponsePayloadContains)
                            {
                                Assert.IsTrue(response.Contains(str), string.Format("The response:\r\n{0}\r\nDoes not contain the string:\r\n{1}.", response, str));
                            }
                        }

                        Assert.IsNull(e, "No exception expected but received one.");
                        Assert.AreEqual(testCase.ExpectedInvokeCalls, IDataServiceInvokable_Invoke_Calls);
                        Assert.AreEqual(testCase.ExpectedGetResultCalls, IDataServiceInvokable_GetResult_Calls);
                    }
                }
                catch (Exception)
                {
                    failedTestCases.Add(testCase);
                    throw;
                }

            });
        }

        // [TestCategory("Partition1"), TestMethod, Variation("Verify that $metadata in batch passes the right operation context instance.")]
        public void BatchedActionMetadataTest()
        {
            var service = ActionTests.ModelWithActions();

            StringBuilder payloadBuilder = new StringBuilder();
            payloadBuilder.AppendLine("GET /$metadata HTTP/1.1");
            payloadBuilder.AppendLine("Host: host");
            payloadBuilder.AppendLine("Accept: " + UnitTestsUtil.MimeApplicationXml);

            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.HttpMethod = "POST";
                request.RequestUriString = "/$batch";
                request.Accept = UnitTestsUtil.MimeMultipartMixed;

                string queryPayload = payloadBuilder.ToString();
                var batchInfo = new BatchInfo(new BatchQuery(new Operation(queryPayload)));

                const string boundary = "batch-set";
                request.RequestContentType = String.Format("{0}; boundary={1}", UnitTestsUtil.MimeMultipartMixed, boundary);
                request.SetRequestStreamAsText(BatchRequestWritingUtils.GetBatchText(batchInfo, boundary));

                request.SendRequest();
            }
        }
    }
}
