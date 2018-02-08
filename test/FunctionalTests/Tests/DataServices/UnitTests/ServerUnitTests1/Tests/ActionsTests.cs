//---------------------------------------------------------------------
// <copyright file="ActionsTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Objects;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Providers;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Web;
using System.Xml;
using AstoriaUnitTests.Stubs;
using AstoriaUnitTests.Stubs.DataServiceProvider;
using Microsoft.Test.ModuleCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindModel;
using t = System.Data.Test.Astoria;
using Microsoft.OData.Client;

namespace AstoriaUnitTests.Tests.Server
{
    // For comment out test cases, see github: https://github.com/OData/odata.net/issues/868
    [TestClass]
    public class ActionTests
    {
        private Version V4 = new Version(4, 0);

        [Ignore]
        // [TestCategory("Partition1"), TestMethod, Variation("Invoke and validate service actions")]
        public void InvokeActionTest()
        {
            #region Test Cases

            // TODO: Fix this test cases for complex collection.
            const string expectedResponsePayload1 = "{\"@odata.context\":\"http://host/$metadata#Edm.String\",\"value\":\"entity1\"}";
            const string expectedResponsePayload2 = "{\"@odata.context\":\"http://host/$metadata#AstoriaUnitTests.Tests.Actions.ComplexType\",\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}";
            const string expectedResponsePayload3 = "{\"@odata.context\":\"http://host/$metadata#Collection(Edm.String)\",\"value\":[\"value1\",\"value2\"]}";
            const string expectedResponsePayload4 = "{\"@odata.context\":\"http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)\",\"value\":[{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null},{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}]}";
            const string expectedResponsePayload5 = "{\"@odata.context\":\"http://host/$metadata#Set/$entity\",\"@odata.etag\":\"W/\\\"true\\\"\",\"ID\":1,\"Updated\":true,\"PrimitiveProperty\":\"entity1\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}},\"PrimitiveCollectionProperty\":[\"value1\",\"value2\"],\"ComplexCollectionProperty\":[{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null},{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}],\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityWithPath\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollectionWithPath\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryableWithPath\":{}}";
            const string expectedResponsePayload6 = "{\"@odata.context\":\"http://host/$metadata#Set\",\"value\":[{\"@odata.etag\":\"W/\\\"true\\\"\",\"ID\":1,\"Updated\":true,\"PrimitiveProperty\":\"entity1\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}},\"PrimitiveCollectionProperty\":[\"value1\",\"value2\"],\"ComplexCollectionProperty\":[{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null},{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}],\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityWithPath\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollectionWithPath\":{},\"#AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryableWithPath\":{}}]}";
            const string expectedResponsePayload7 = "{\"@odata.context\":\"http://host/$metadata#Collection(Edm.String)\",\"value\":[]}";
            const string expectedResponsePayload8 = "{ \"value\" : null }";
            const string expectedResponsePayload9 = "{\"@odata.context\":\"http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)\",\"value\":[]}";
            const string expectedResponsePayload10 = "{\"@odata.context\":\"http://host/$metadata#Set\",\"value\":[]}";
            const string expectedResponsePayload11 = "{\"@odata.context\":\"http://host/$metadata#Edm.String\",\"value\":\"foo\"}";
            const string expectedResponsePayload12 = "{\"@odata.context\":\"http://host/$metadata#Collection(Edm.String)\",\"value\":[\"foo\"]}";
            const string expectedResponsePayload13 = "{\"@odata.context\":\"http://host/$metadata#Collection(Edm.String)\",\"value\":[\"foo\",\"bar\"]}";
            //const string expectedResponsePayload14 = "{\"@odata.context\":\"http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)\",\"value\":[{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}]}";
            //const string expectedResponsePayload15 = "{\"@odata.context\":\"http://host/$metadata#Collection(AstoriaUnitTests.Tests.Actions.ComplexType)\",\"value\":[{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}},{\"PrimitiveProperty\":null,\"ComplexProperty\":null}]}";
            const string expectedResponsePayload16 = "{\"@odata.context\":\"http://host/$metadata#Edm.String\",\"value\":\"foo325\"}";
            const string expectedResponsePayload17 = "{\"@odata.context\":\"http://host/$metadata#Collection(Edm.String)\",\"value\":[\"foo325\"]}";
            const string expectedResponsePayload18 = "{\"@odata.context\":\"http://host/$metadata#Collection(Edm.String)\",\"value\":[\"foo325\",\"bar325\"]}";

            const string requestPayload1 = "{ \"value\" : \"foo\" }";
            const string requestPayload2 = "{ \"value\" : [] }";
            const string requestPayload3 = "{ \"value\" : [\"foo\"] }";
            const string requestPayload4 = "{ \"value\" : [\"foo\",\"bar\"] }";
            const string requestPayload5 = "{ \"value\" : {\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}}";
            //const string requestPayload6 = "{ \"value\" : [{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}] }";
            //const string requestPayload7 = "{ \"value\" : [{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}},{\"PrimitiveProperty\":null,\"ComplexProperty\":null}] }";
            const string requestPayload8 = "{ \"value1\" : \"foo\", \"value2\" : 325 }";
            const string requestPayload9 = "{ \"value1\" : [], \"value2\" : 325 }";
            const string requestPayload10 = "{ \"value1\" : [\"foo\"], \"value2\" : 325 }";
            const string requestPayload11 = "{ \"value1\" : [\"foo\",\"bar\"], \"value2\" : 325 }";
            const string requestPayload12 = "{ \"value1\" : 111, \"value2\" : [] }";
            const string requestPayload13 = "{ \"value1\" : 111, \"value2\" : [\"foo\"] }";
            const string requestPayload14 = "{ \"value1\" : 111, \"value2\" : [\"foo\",\"bar\"] }";
            const string requestPayload15 = "{ \"value1\" : {\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}, \"value2\" : {\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}";
            //const string requestPayload16 = "{ \"value1\" : [], \"value2\" : 1 }";
            //const string requestPayload17 = "{ \"value1\" : [{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}], \"value2\" : 1 }";
            //const string requestPayload18 = "{ \"value1\" : [{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}},{\"PrimitiveProperty\":null,\"ComplexProperty\":null}], \"value2\" : 1 }";
            //const string requestPayload19 = "{ \"value1\" : [1], \"value2\" : [] }";
            //const string requestPayload20 = "{ \"value1\" : [1], \"value2\" : [{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}}] }";
            //const string requestPayload21 = "{ \"value1\" : [1], \"value2\" : [{\"PrimitiveProperty\":\"complex2\",\"ComplexProperty\":{\"PrimitiveProperty\":\"complex1\",\"ComplexProperty\":null}},{\"PrimitiveProperty\":null,\"ComplexProperty\":null}] }";

            var testCases = new[]
            {
                new {
                    RequestUri = "/TopLevelAction_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_Primitive_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_Complex_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_PrimitiveCollection_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_ComplexCollection_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_Entity_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_EntityCollection_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_EntityQueryable_Null",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_PrimitiveCollection_Empty",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_ComplexCollection_Empty",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload9,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelAction_EntityCollection_Empty",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload10,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelAction_EntityQueryable_Empty",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload10,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/TopLevelActionWithNullParam_Primitive",
                    RequestPayload = expectedResponsePayload8,
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithNullParam_PrimitiveCollection",
                    RequestPayload = expectedResponsePayload8,
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithNullParam_PrimitiveQueryable",
                    RequestPayload = expectedResponsePayload8,
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithNullParam_Complex",
                    RequestPayload = expectedResponsePayload8,
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithNullParam_ComplexCollection",
                    RequestPayload = expectedResponsePayload8,
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithNullParam_ComplexQueryable",
                    RequestPayload = expectedResponsePayload8,
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntity/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = "",
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = "",
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Complex",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_PrimitiveCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload3,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_ComplexCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload4,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload5,
                    StatusCode = 200,
                    ResponseETag = "W/\"true\"",
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityCollection",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                new {
                    RequestUri = "/GetEntities/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload6,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = true,
                },
                             new {
                    RequestUri = "/TopLevelActionWithParam_Primitive",
                    RequestPayload = requestPayload1,
                    ExpectedResponsePayload = expectedResponsePayload11,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithParam_PrimitiveCollection",
                    RequestPayload = requestPayload2,
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },

                new {
                    RequestUri = "/TopLevelActionWithParam_PrimitiveCollection",
                    RequestPayload = requestPayload3,
                    ExpectedResponsePayload = expectedResponsePayload12,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithParam_PrimitiveCollection",
                    RequestPayload = requestPayload4,
                    ExpectedResponsePayload = expectedResponsePayload13,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithParam_PrimitiveQueryable",
                    RequestPayload = requestPayload2,
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithParam_PrimitiveQueryable",
                    RequestPayload = requestPayload3,
                    ExpectedResponsePayload = expectedResponsePayload12,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/TopLevelActionWithParam_PrimitiveQueryable",
                    RequestPayload = requestPayload4,
                    ExpectedResponsePayload = expectedResponsePayload13,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },

                new {
                    RequestUri = "/TopLevelActionWithParam_Complex",
                    RequestPayload = requestPayload5,
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },

               // new {
               //     RequestUri = "/TopLevelActionWithParam_ComplexCollection",
               //     RequestPayload = requestPayload2,
               //     ExpectedResponsePayload = expectedResponsePayload9,
               //     StatusCode = 200,
               //     ResponseETag = default(string),
               //     ResponseVersion = V4,
               //     IsBindable = false,
               //     HasReturnSet = false,
               // },
               //new {
               //     RequestUri = "/TopLevelActionWithParam_ComplexCollection",
               //     RequestPayload = requestPayload6,
               //     ExpectedResponsePayload = expectedResponsePayload14,
               //     StatusCode = 200,
               //     ResponseETag = default(string),
               //     ResponseVersion = V4,
               //     IsBindable = false,
               //     HasReturnSet = false,
               // },
               // new {
               //     RequestUri = "/TopLevelActionWithParam_ComplexCollection",
               //     RequestPayload = requestPayload7,
               //     ExpectedResponsePayload = expectedResponsePayload15,
               //     StatusCode = 200,
               //     ResponseETag = default(string),
               //     ResponseVersion = V4,
               //     IsBindable = false,
               //     HasReturnSet = false,
               // },
               // new {
               //     RequestUri = "/TopLevelActionWithParam_ComplexQueryable",
               //     RequestPayload = requestPayload2,
               //     ExpectedResponsePayload = expectedResponsePayload9,
               //     StatusCode = 200,
               //     ResponseETag = default(string),
               //     ResponseVersion = V4,
               //     IsBindable = false,
               //     HasReturnSet = false,
               // },
               // new {
               //     RequestUri = "/TopLevelActionWithParam_ComplexQueryable",
               //     RequestPayload = requestPayload6,
               //     ExpectedResponsePayload = expectedResponsePayload14,
               //     StatusCode = 200,
               //     ResponseETag = default(string),
               //     ResponseVersion = V4,
               //     IsBindable = false,
               //     HasReturnSet = false,
               // },
               // new {
               //     RequestUri = "/TopLevelActionWithParam_ComplexQueryable",
               //     RequestPayload = requestPayload7,
               //     ExpectedResponsePayload = expectedResponsePayload15,
               //     StatusCode = 200,
               //     ResponseETag = default(string),
               //     ResponseVersion = V4,
               //     IsBindable = false,
               //     HasReturnSet = false,
               // },

                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive",
                    RequestPayload = requestPayload1,
                    ExpectedResponsePayload = expectedResponsePayload11,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection",
                    RequestPayload = requestPayload2,
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection",
                    RequestPayload = requestPayload3,
                    ExpectedResponsePayload = expectedResponsePayload12,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection",
                    RequestPayload = requestPayload4,
                    ExpectedResponsePayload = expectedResponsePayload13,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveQueryable",
                    RequestPayload = requestPayload2,
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveQueryable",
                    RequestPayload = requestPayload3,
                    ExpectedResponsePayload = expectedResponsePayload12,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveQueryable",
                    RequestPayload = requestPayload4,
                    ExpectedResponsePayload = expectedResponsePayload13,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Complex",
                    RequestPayload = requestPayload5,
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection",
                //    RequestPayload = requestPayload2,
                //    ExpectedResponsePayload = expectedResponsePayload9,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection",
                //    RequestPayload = requestPayload6,
                //    ExpectedResponsePayload = expectedResponsePayload14,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection",
                //    RequestPayload = requestPayload7,
                //    ExpectedResponsePayload = expectedResponsePayload15,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexQueryable",
                //    RequestPayload = requestPayload2,
                //    ExpectedResponsePayload = expectedResponsePayload9,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexQueryable",
                //    RequestPayload = requestPayload6,
                //    ExpectedResponsePayload = expectedResponsePayload14,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexQueryable",
                //    RequestPayload = requestPayload7,
                //    ExpectedResponsePayload = expectedResponsePayload15,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_Primitive",
                    RequestPayload = requestPayload8,
                    ExpectedResponsePayload = expectedResponsePayload16,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },

                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = requestPayload9,
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = requestPayload10,
                    ExpectedResponsePayload = expectedResponsePayload17,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },

                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = requestPayload11,
                    ExpectedResponsePayload = expectedResponsePayload18,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },

                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_PrimitiveQueryable",
                    RequestPayload = requestPayload12,
                    ExpectedResponsePayload = expectedResponsePayload7,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },

                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_PrimitiveQueryable",
                    RequestPayload = requestPayload13,
                    ExpectedResponsePayload = expectedResponsePayload12,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Primitive_PrimitiveQueryable",
                    RequestPayload = requestPayload14,
                    ExpectedResponsePayload = expectedResponsePayload13,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_Complex_Complex",
                    RequestPayload = requestPayload15,
                    ExpectedResponsePayload = expectedResponsePayload2,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection_Primitive",
                //    RequestPayload = requestPayload16,
                //    ExpectedResponsePayload = expectedResponsePayload9,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection_Primitive",
                //    RequestPayload = requestPayload17,
                //    ExpectedResponsePayload = expectedResponsePayload14,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_ComplexCollection_Primitive",
                //    RequestPayload = requestPayload18,
                //    ExpectedResponsePayload = expectedResponsePayload15,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_ComplexQueryable",
                //    RequestPayload = requestPayload19,
                //    ExpectedResponsePayload = expectedResponsePayload9,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_ComplexQueryable",
                //    RequestPayload = requestPayload20,
                //    ExpectedResponsePayload = expectedResponsePayload14,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                //new {
                //    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_ComplexQueryable",
                //    RequestPayload = requestPayload21,
                //    ExpectedResponsePayload = expectedResponsePayload15,
                //    StatusCode = 200,
                //    ResponseETag = default(string),
                //    ResponseVersion = V4,
                //    IsBindable = true,
                //    HasReturnSet = false,
                //},
                new {
                    RequestUri = "/Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnDerivedEntityCollection_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnDerivedEntityQueryable_Void",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = string.Empty,
                    StatusCode = 204,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = true,
                    HasReturnSet = false,
                },
             
                // The next two test cases are for namespace qualified and fully qualfied names.
                new {
                    RequestUri = "/ModelWithActions.TopLevelAction_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                },
                new {
                    RequestUri = "/AstoriaUnitTests.Tests.Actions.ModelWithActions.TopLevelAction_Primitive",
                    RequestPayload = default(string),
                    ExpectedResponsePayload = expectedResponsePayload1,
                    StatusCode = 200,
                    ResponseETag = default(string),
                    ResponseVersion = V4,
                    IsBindable = false,
                    HasReturnSet = false,
                }
            };
            #endregion Test Cases

            var entitySetRights = new EntitySetRights[]
            {
                EntitySetRights.None,
                EntitySetRights.ReadSingle,
                EntitySetRights.ReadMultiple,
                EntitySetRights.WriteAppend,
                EntitySetRights.WriteReplace,
                EntitySetRights.WriteDelete,
                EntitySetRights.WriteMerge,
                EntitySetRights.AllRead,
                EntitySetRights.AllWrite,
                EntitySetRights.All
            };

            List<string> failedTestUris = new List<string>();
            var versions = new Version[] { V4 };
            var service = ActionTests.ModelWithActions();
            t.TestUtil.RunCombinations(testCases, versions, versions, UnitTestsUtil.BooleanValues, entitySetRights, (testCase, dsv, mdsv, appendParanthesis, entitySetRight) =>
            {
                try
                {
                    using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                    using (OpenWebDataServiceHelper.ServiceActionAccessRule.Restore())
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                    {
                        { "Set", entitySetRight },
                        { "Set2", EntitySetRights.All },
                    };

                        request.HttpMethod = "POST";
                        string actionName = testCase.RequestUri.Split(new char[] { '/' }).Last();
                        request.RequestUriString = testCase.RequestUri + (appendParanthesis ? "()" : String.Empty);

                        request.Accept = UnitTestsUtil.JsonMimeType;
                        request.RequestVersion = dsv.ToString();
                        request.RequestMaxVersion = mdsv.ToString();
                        if (testCase.RequestPayload == null)
                        {
                            request.RequestContentLength = 0;
                        }
                        else
                        {
                            request.RequestContentType = UnitTestsUtil.JsonMimeType;
                            request.SetRequestStreamAsText(testCase.RequestPayload);
                        }

                        bool IDataServiceInvokable_Invoke_Called = false;
                        bool IDataServiceInvokable_GetResult_Called = false;
                        service.ActionProvider.ServiceActionInvokeCallback = (parameters) => { IDataServiceInvokable_Invoke_Called = true; };
                        service.ActionProvider.ServiceActionGetResultCallback = () => { IDataServiceInvokable_GetResult_Called = true; };

                        Exception e = t.TestUtil.RunCatching(request.SendRequest);
                        if (testCase.IsBindable && entitySetRight == EntitySetRights.None)
                        {
                            Assert.IsNotNull(e);
                            if (testCase.RequestUri.Contains("GetEntity"))
                            {
                                Assert.AreEqual("The operation 'GetEntity' has the resource set 'Set' that is not visible. The operation 'GetEntity' should be made hidden or the resource set 'Set' should be made visible.", e.InnerException.Message);
                                Assert.AreEqual(400, request.ResponseStatusCode);
                            }
                            else if (testCase.RequestUri.Contains("GetEntities"))
                            {
                                Assert.AreEqual("The operation 'GetEntities' has the resource set 'Set' that is not visible. The operation 'GetEntities' should be made hidden or the resource set 'Set' should be made visible.", e.InnerException.Message);
                                Assert.AreEqual(400, request.ResponseStatusCode);
                            }
                            else
                            {
                                Assert.AreEqual("Resource not found for the segment 'Set'.", e.InnerException.Message);
                                Assert.AreEqual(404, request.ResponseStatusCode);
                            }
                        }
                        else if (testCase.HasReturnSet && entitySetRight == EntitySetRights.None)
                        {
                            Assert.IsNotNull(e);
                            Assert.IsNotNull(e.InnerException);
                            Assert.AreEqual(string.Format("The operation '{0}' has the resource set 'Set' that is not visible. The operation '{0}' should be made hidden or the resource set 'Set' should be made visible.", actionName), e.InnerException.Message);
                            Assert.AreEqual(400, request.ResponseStatusCode);
                        }
                        else if (dsv < V4 && testCase.RequestPayload != null)
                        {
                            Assert.IsNotNull(e);
                            Assert.AreEqual(String.Format("The OData-Version '{0}' is too low for the request. The lowest supported version is '4.0'.", dsv), e.InnerException.Message);
                        }
                        else if (mdsv < testCase.ResponseVersion)
                        {
                            Assert.IsNotNull(e);
                            Assert.AreEqual(String.Format("The MaxDataServiceVersion '{0}' is too low for the response. The lowest supported version is '{1}'.", mdsv, testCase.ResponseVersion), e.InnerException.Message);
                        }
                        else
                        {
                            string response = request.GetResponseStreamAsText();
                            Assert.AreEqual(testCase.ExpectedResponsePayload, response);
                            Assert.AreEqual(request.ResponseStatusCode, testCase.StatusCode);

                            Assert.AreEqual(testCase.ResponseVersion.ToString() + ";", request.ResponseVersion);
                            Assert.AreEqual(testCase.ResponseETag, request.ResponseETag);

                            Assert.IsNull(e, "No exception expected but received one.");
                            Assert.IsTrue(IDataServiceInvokable_Invoke_Called, "IDataServiceInvokable.Invoke() was never called!");
                            if (!testCase.RequestUri.EndsWith("Void"))
                            {
                                Assert.IsTrue(IDataServiceInvokable_GetResult_Called, "IDataServiceInvokable.GetResult() was never called on a non-void action!");
                            }
                            else
                            {
                                Assert.IsFalse(IDataServiceInvokable_GetResult_Called, "IDataServiceInvokable.GetResult() was called on a void action!");
                            }
                        }
                    }

                    t.TestUtil.ClearMetadataCache();
                }
                catch (Exception)
                {
                    failedTestUris.Add(testCase.RequestUri);
                    throw;
                }

            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Verifies that the server will default an omitted nullable parameter value to null.")]
        public void InvokeActionWithOmittedNullableParameterTest()
        {
            var service = ActionTests.ModelWithActions();
            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.HttpMethod = "POST";
                request.RequestUriString = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive";

                request.Accept = UnitTestsUtil.JsonLightMimeType;
                request.RequestVersion = "4.0;";
                request.RequestMaxVersion = "4.0;";

                request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                request.SetRequestStreamAsText("{ \"value2\" : 123 }");

                bool IDataServiceInvokable_Invoke_Called = false;
                bool IDataServiceInvokable_GetResult_Called = false;
                service.ActionProvider.ServiceActionInvokeCallback = (parameters) =>
                {
                    Assert.AreEqual(3, parameters.Length);
                    Assert.IsTrue(parameters[0].GetType().FullName.StartsWith("AstoriaUnitTests.Stubs.DataServiceProvider.DSPLinqQuery"));
                    Assert.AreEqual(null, parameters[1]);
                    Assert.AreEqual(123, parameters[2]);
                    IDataServiceInvokable_Invoke_Called = true;
                };
                service.ActionProvider.ServiceActionGetResultCallback = () => { IDataServiceInvokable_GetResult_Called = true; };

                Exception e = t.TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(400, request.ResponseStatusCode);
                Assert.AreEqual(
                    "One or more parameters of the operation 'ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive' are missing from the request payload. The missing parameters are: value1.",
                    e.InnerException.InnerException.Message);
                Assert.IsFalse(IDataServiceInvokable_Invoke_Called);
                Assert.IsFalse(IDataServiceInvokable_GetResult_Called);
            }
        }

        [TestCategory("Partition1"), TestMethod, Variation("Service action error tests.")]
        public void InvokeActionNegativeTest()
        {
            #region testCases
            var testCases = new[]
            {
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void(1)",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void' cannot include key predicates, however it may end with empty parenthesis.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity/$ref",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/ResourceReferenceProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity/$ref",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Entity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable/$count",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_EntityQueryable' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive/$value",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Primitive' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity/ResourceSetReferenceProperty/$ref",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Entity' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/ResourceReferenceProperty/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnEntityCollection_Void' is not assignable from the result of the uri segment 'ResourceReferenceProperty'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/ResourceSetReferenceProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnSingleEntity_Void' is not assignable from the result of the uri segment 'ResourceSetReferenceProperty'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/PrimitiveProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void' in the request URI is not valid. The segment 'PrimitiveProperty' refers to a primitive property, function, or service operation, so the only supported value from the next segment is '$value'.",
                    StatusCode = 404,
                    StatusCodeWhenContentTypeNull = 404,
                },
                new {
                    RequestUri = "/Set(1)/ComplexProperty/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnSingleEntity_Void' is not assignable from the result of the uri segment 'ComplexProperty'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/PrimitiveCollectionProperty/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void()",
                    RequestPayload = default(string),
                    ErrorMsg = "Found an operation bound to a non-entity type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/ComplexCollectionProperty/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "Found an operation bound to a non-entity type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/ResourceReferenceProperty2/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnSingleEntity_Void' is not assignable from the result of the uri segment 'ResourceReferenceProperty2'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/ResourceSetReferenceProperty2/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnEntityCollection_Void' is not assignable from the result of the uri segment 'ResourceSetReferenceProperty2'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/TopLevelAction_Void()",
                    RequestPayload = default(string),
                    ErrorMsg = "Bad Request - Error in query syntax.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/TopLevelAction_Void()",
                    RequestPayload = default(string),
                    ErrorMsg = "Resource not found for the segment 'TopLevelAction_Void'.",
                    StatusCode = 404,
                    StatusCodeWhenContentTypeNull = 404,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()/Foo",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'TopLevelAction_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void()/Foo",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void()/Foo",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityCollection_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void()/Foo",
                    RequestPayload = default(string),
                    ErrorMsg = "The request URI is not valid. The segment 'AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$expand=ResourceReferenceProperty",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$filter=PrimitiveProperty eq \"foo\"",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$orderby=PrimitiveProperty",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$skip=1",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$top=1",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$count=true",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$select=ResourceReferenceProperty",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void()?$skiptoken=foo",
                    RequestPayload = default(string),
                    ErrorMsg = "Query options $select, $expand, $filter, $orderby, $count, $skip, $skiptoken and $top are not supported by this request method or cannot be applied to the requested resource.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void",
                    RequestPayload = "{ \"value\" : [\"foo\",\"bar\"] }",
                    ErrorMsg = "The parameter 'value' in the request payload is not a valid parameter for the operation 'TopLevelAction_Void'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 204,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = "{ \"value1\" : [], \"value2\" : 123, \"foo\" : \"bar\" }",
                    ErrorMsg = "The parameter 'foo' in the request payload is not a valid parameter for the operation 'ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = "{ \"value1\" : []}",
                    ErrorMsg = "One or more parameters of the operation 'ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive' are missing from the request payload. The missing parameters are: value2.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = "{}",
                    ErrorMsg = "One or more parameters of the operation 'ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive' are missing from the request payload. The missing parameters are: value1,value2.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = string.Empty,
                    ErrorMsg = "One or more parameters of the operation 'ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive' are missing from the request payload. The missing parameters are: value1,value2.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = "{ \"value1\" : 123, \"value2\" : [] }",
                    ErrorMsg = "When trying to read a null collection parameter value in JSON Light, a node of type 'PrimitiveValue' with the value '123' was read from the JSON reader; however, a primitive 'null' value was expected.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive",
                    RequestPayload = "{ \"value1\" : [], \"value2\" : 123, \"value1\" : [], \"value2\" : 123 }",
                    ErrorMsg = "Multiple parameters with the name 'value1' were found in the request payload.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set(4)/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnSingleDerivedEntity_Void' is not assignable from the result of the uri segment 'Set'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnDerivedEntityCollection_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnDerivedEntityCollection_Void' is not assignable from the result of the uri segment 'Set'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnDerivedEntityQueryable_Void",
                    RequestPayload = default(string),
                    ErrorMsg = "The binding parameter for 'ActionOnDerivedEntityQueryable_Void' is not assignable from the result of the uri segment 'Set'.",
                    StatusCode = 400,
                    StatusCodeWhenContentTypeNull = 400,
                },

                // Disabled tests due to Error "Service action 'actionName' 'requires a binding parameter" doesn't happen as ODataPathParser throws error earlier
                // We may simply want to make the ODataPathParser throw a smarter error message and remove this from the server.
                //new {
                //    RequestUri = "/ActionOnSingleEntity_Void",
                //    RequestPayload = default(string),
                //    ErrorMsg = "Service action 'ActionOnSingleEntity_Void' requires a binding parameter, but it was invoked unbound.",
                //    StatusCode = 400,
                //    StatusCodeWhenContentTypeNull = 400,
                //},
                //new {
                //    RequestUri = "/ActionOnEntityCollection_Void",
                //    RequestPayload = default(string),
                //    ErrorMsg = "Service action 'ActionOnEntityCollection_Void' requires a binding parameter, but it was invoked unbound.",
                //    StatusCode = 400,
                //    StatusCodeWhenContentTypeNull = 400,
                //},
                //new {
                //    RequestUri = "/ActionOnEntityQueryable_Void",
                //    RequestPayload = default(string),
                //    ErrorMsg = "Service action 'ActionOnEntityQueryable_Void' requires a binding parameter, but it was invoked unbound.",
                //    StatusCode = 400,
                //    StatusCodeWhenContentTypeNull = 400,
                //},
            };

            #endregion

            List<string> failedTests = new List<string>();

            var service = ActionTests.ModelWithActions();
            service.ForceVerboseErrors = true;
            var contentTypes = new string[] { UnitTestsUtil.JsonLightMimeType, null };

            t.TestUtil.RunCombinations(testCases, contentTypes, (testCase, contentType) =>
            {
                try
                {
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.HttpMethod = "POST";
                        request.RequestUriString = testCase.RequestUri;
                        request.RequestContentType = contentType;
                        request.SetRequestStreamAsText(testCase.RequestPayload ?? string.Empty);

                        Exception e = t.TestUtil.RunCatching(request.SendRequest);

                        int expectedStatusCode = contentType == null ? testCase.StatusCodeWhenContentTypeNull : testCase.StatusCode;
                        Assert.AreEqual(expectedStatusCode, request.ResponseStatusCode);
                        if (expectedStatusCode == 204)
                        {
                            return;
                        }

                        while (e.InnerException != null)
                        {
                            e = e.InnerException;
                        }

                        string expectedErrorMsg = testCase.ErrorMsg;
                        if (testCase.RequestPayload != null && contentType == null)
                        {
                            expectedErrorMsg = "Content-Type header value missing.";
                        }

                        Assert.AreEqual(expectedErrorMsg, e.Message);
                    }
                }
                catch (Exception)
                {
                    failedTests.Add(testCase.RequestUri);
                    throw;
                }
            });
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Variation("Makes sure in Json Light, always bindable actions are omitted in payload.")]
        public void JsonLightOmitAlwaysAvailableActionsInEntry()
        {
            DSPMetadata metadata = new DSPMetadata("ModelWithActions", "AstoriaUnitTests.Tests.Actions");

            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));

            metadata.AddResourceSet("Set", entityType);
            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            DSPActionProvider actionProvider = new DSPActionProvider();
            actionProvider.AddAction("TopLevelAction", ResourceType.GetPrimitiveResourceType(typeof(int)), null /*resultSet*/, null /*parameters*/, OperationParameterBindingKind.Never);
            actionProvider.AddAction("SometimesBindableAction", ResourceType.GetPrimitiveResourceType(typeof(int)), null /*resultSet*/, new[] { new ServiceActionParameter("binding", entityType) }, OperationParameterBindingKind.Sometimes);
            actionProvider.AddAction("AlwaysBindableAction", ResourceType.GetPrimitiveResourceType(typeof(int)), null /*resultSet*/, new[] { new ServiceActionParameter("binding", entityType) }, OperationParameterBindingKind.Always);

            DSPResource entity1 = new DSPResource(entityType);
            entity1.SetValue("ID", 1);
            context.GetResourceSetEntities("Set").Add(entity1);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                ActionProvider = actionProvider,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };

            var testCases = new[]
            {
                new
                {
                    RequestUri = "/Set",
                    Method = "POST",
                    Payload = "{\"ID\":2}",
                    ExpectedStatusCode = 201,
                },
                new
                {
                    RequestUri = "/Set",
                    Method = "GET",
                    Payload = default(string),
                    ExpectedStatusCode = 200,
                },
                new
                {
                    RequestUri = "/Set(1)",
                    Method = "GET",
                    Payload = default(string),
                    ExpectedStatusCode = 200,
                },
            };

            service.ForceVerboseErrors = true;
            t.TestUtil.RunCombinations(testCases, new[] { UnitTestsUtil.AtomFormat, UnitTestsUtil.JsonLightMimeType, UnitTestsUtil.JsonMimeType }, (testCase, format) =>
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = testCase.Method;
                    request.RequestUriString = testCase.RequestUri;
                    request.Accept = format;
                    if (testCase.Payload != null)
                    {
                        request.RequestContentType = UnitTestsUtil.JsonMimeType;
                        request.SetRequestStreamAsText(testCase.Payload);
                    }

                    Assert.IsNull(t.TestUtil.RunCatching(request.SendRequest));
                    Assert.AreEqual(testCase.ExpectedStatusCode, request.ResponseStatusCode);

                    string responsePayload = request.GetResponseStreamAsText();
                    if (format == UnitTestsUtil.JsonMimeType || format == UnitTestsUtil.JsonLightMimeType)
                    {
                        Assert.IsTrue(responsePayload.Contains("AstoriaUnitTests.Tests.Actions.SometimesBindableAction"), "Sometimes bindable actions should be in the JsonLight payload.");
                        Assert.IsFalse(responsePayload.Contains("AstoriaUnitTests.Tests.Actions.AlwaysBindableAction"), "Always bindable actions should be omitted from the JsonLight payload.");
                    }
                    else
                    {
                        Assert.IsTrue(responsePayload.Contains("AstoriaUnitTests.Tests.Actions.SometimesBindableAction"), "Sometimes bindable actions should be in the payload.");
                        Assert.IsTrue(responsePayload.Contains("AstoriaUnitTests.Tests.Actions.AlwaysBindableAction"), "Always bindable actions should be in the non-JsonLight payload.");
                    }
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Makes sure if there's a name collision between an action and a set or property, the set or property wins.")]
        public void ActionResolveOrderTests()
        {
            DSPMetadata metadata = new DSPMetadata("ModelWithActions", "AstoriaUnitTests.Tests.Actions");

            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            var barProperty = metadata.AddResourceSetReferenceProperty(entityType, "Bar", entityType);

            var set = metadata.AddResourceSet("Foo", entityType);

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceSetReference",
                new ResourceAssociationSetEnd(set, entityType, barProperty),
                new ResourceAssociationSetEnd(set, entityType, null)));

            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            DSPActionProvider actionProvider = new DSPActionProvider();
            actionProvider.AddAction("Foo", ResourceType.GetPrimitiveResourceType(typeof(int)), null /*resultSet*/, null /*parameters*/, OperationParameterBindingKind.Never);
            actionProvider.AddAction("Bar", ResourceType.GetPrimitiveResourceType(typeof(int)), null /*resultSet*/, new[] { new ServiceActionParameter("binding", entityType) }, OperationParameterBindingKind.Always);

            DSPResource entity1 = new DSPResource(entityType);
            entity1.SetValue("ID", 1);
            entity1.SetValue("Bar", new List<DSPResource>());

            context.GetResourceSetEntities("Foo").Add(entity1);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                ActionProvider = actionProvider,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };

            var testCases = new[]
            {
                new
                {
                    RequestUri = "/Foo",
                    Payload = "{ \"ID\" : 99 }",
                    ExpectedResponse = "{\"@odata.context\":\"http://host/$metadata#Foo/$entity\",\"ID\":99}",
                    ExpectedStatusCode = 201,
                },
                new
                {
                    RequestUri = "/Foo(1)/Bar",
                    Payload = "{ \"ID\" : 100 }",
                    ExpectedResponse = "{\"@odata.context\":\"http://host/$metadata#Foo/$entity\",\"ID\":100}",
                    ExpectedStatusCode = 201,
                }
            };

            service.ForceVerboseErrors = true;
            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = "POST";
                    request.RequestUriString = testCase.RequestUri;
                    request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.SetRequestStreamAsText(testCase.Payload);

                    t.TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(testCase.ExpectedStatusCode, request.ResponseStatusCode);
                    Assert.AreEqual(testCase.ExpectedResponse, request.GetResponseStreamAsText());
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Service action etag tests.")]
        public void ActionEtagErrorTests()
        {
            var testCases = new[]
            {
                new {
                    RequestUri = "/TopLevelAction_Void",
                    RequestPayload = default(string),
                    IfMatch = "W/\"foo\"",
                    IfNoneMatch = default(string),
                    ErrorMsg = "If-Match or If-None-Match HTTP headers cannot be specified for service actions.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void",
                    RequestPayload = default(string),
                    IfMatch = default(string),
                    IfNoneMatch = "W/\"bar\"",
                    ErrorMsg = "If-Match or If-None-Match HTTP headers cannot be specified for service actions.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/TopLevelAction_Void",
                    RequestPayload = default(string),
                    IfMatch = "W/\"foo\"",
                    IfNoneMatch = "W/\"bar\"",
                    ErrorMsg = "Both If-Match and If-None-Match HTTP headers cannot be specified at the same time. Please specify either one of the headers or none of them.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    IfMatch = "W/\"foo\"",
                    IfNoneMatch = default(string),
                    ErrorMsg = "If-Match or If-None-Match HTTP headers cannot be specified for service actions.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    IfMatch = default(string),
                    IfNoneMatch = "W/\"bar\"",
                    ErrorMsg = "If-Match or If-None-Match HTTP headers cannot be specified for service actions.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    RequestPayload = default(string),
                    IfMatch = "W/\"foo\"",
                    IfNoneMatch = "W/\"bar\"",
                    ErrorMsg = "Both If-Match and If-None-Match HTTP headers cannot be specified at the same time. Please specify either one of the headers or none of them.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void",
                    RequestPayload = default(string),
                    IfMatch = "W/\"foo\"",
                    IfNoneMatch = default(string),
                    ErrorMsg = "If-Match or If-None-Match HTTP headers cannot be specified for service actions.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void",
                    RequestPayload = default(string),
                    IfMatch = default(string),
                    IfNoneMatch = "W/\"bar\"",
                    ErrorMsg = "If-Match or If-None-Match HTTP headers cannot be specified for service actions.",
                    StatusCode = 400,
                },
                new {
                    RequestUri = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void",
                    RequestPayload = default(string),
                    IfMatch = "W/\"foo\"",
                    IfNoneMatch = "W/\"bar\"",
                    ErrorMsg = "Both If-Match and If-None-Match HTTP headers cannot be specified at the same time. Please specify either one of the headers or none of them.",
                    StatusCode = 400,
                },
            };

            List<string> failedRequestUris = new List<string>();
            var service = ActionTests.ModelWithActions();
            service.ForceVerboseErrors = true;
            t.TestUtil.RunCombinations(testCases, testCase =>
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    try
                    {
                        request.HttpMethod = "POST";
                        request.RequestUriString = testCase.RequestUri;
                        request.SetRequestStreamAsText(testCase.RequestPayload ?? string.Empty);
                        request.IfMatch = testCase.IfMatch;
                        request.IfNoneMatch = testCase.IfNoneMatch;

                        Exception e = t.TestUtil.RunCatching(request.SendRequest);

                        Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode);
                        while (e.InnerException != null)
                        {
                            e = e.InnerException;
                        }

                        string expectedErrorMsg = testCase.ErrorMsg;
                        Assert.AreEqual(expectedErrorMsg, e.Message);
                    }
                    catch (Exception)
                    {
                        failedRequestUris.Add(testCase.RequestUri);
                        throw;
                    }
                }
            });
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Variation("Tests $select on entities with actions.")]
        public void ActionSelectExpandTests()
        {
            var testCases = new[]
            {
                // $select test cases
                new {
                    RequestUri = "/Set?$select=*",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                        "not    (//adsm:action)"
                    },
                },
                new {
                    RequestUri = "/Set?$select=ID,AstoriaUnitTests.Tests.Actions.*",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties/ads:ID)",
                        "not    (//atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=12] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=11])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=*,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[count(adsm:action)=1] and not(//atom:entry[count(adsm:action)!=1]))",
                        "boolean(//atom:entry/adsm:action[@title='ActionOnSingleEntity_Void'] and not(//atom:entry/adsm:action[@title!='ActionOnSingleEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$select=ID,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties/ads:ID)",
                        "not    (//atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])",
                        "boolean(//atom:entry[count(adsm:action)=1] and not(//atom:entry[count(adsm:action)!=1]))",
                        "boolean(//atom:entry/adsm:action[@title='ActionOnSingleEntity_Void'] and not(//atom:entry/adsm:action[@title!='ActionOnSingleEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "not    (//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[count(adsm:action)=1] and not(//atom:entry[count(adsm:action)!=1]))",
                        "boolean(//atom:entry/adsm:action[@title='ActionOnSingleEntity_Void'] and not(//atom:entry/adsm:action[@title!='ActionOnSingleEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "not    (//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=1] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=0])",
                        "boolean(//atom:entry/adsm:action[@title='ActionOnSingleDerivedEntity_Void'] and not(//atom:entry/adsm:action[@title!='ActionOnSingleDerivedEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "not    (//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=2] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=1])",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and (adsm:action/@title='ActionOnSingleDerivedEntity_Void' or adsm:action/@title='ActionOnSingleEntity_Void')] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and adsm:action/@title='ActionOnSingleEntity_Void'])"
                    },
                },

                // $select + $expand test cases
                new {
                    RequestUri = "/Set?$select=*&$expand=ResourceSetReferenceProperty",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                    },
                },
                new {
                    RequestUri = "/Set?$select=*&$expand=ResourceSetReferenceProperty($select=*)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                        "not    (//adsm:action)"
                    },
                },
                new {
                    RequestUri = "/Set?$select=*&$expand=ResourceSetReferenceProperty",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                        "not    ((/atom:feed/atom:entry|/atom:entry)/adsm:action)",
                        "boolean((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=12] and (/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=11])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=ID,AstoriaUnitTests.Tests.Actions.*&$expand=ResourceSetReferenceProperty($select=ID,AstoriaUnitTests.Tests.Actions.*)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties/ads:ID)",
                        "not    (//atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=12] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=11])",
                    },
                },
                new {
                    RequestUri = "/Set?&$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.*)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                        "not    ((/atom:feed/atom:entry|/atom:entry)[count(adsm:action)=0])",
                        "boolean((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=12] and (/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=11])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=*,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void&$expand=ResourceSetReferenceProperty($select=*,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[count(adsm:action)=1] and not(//atom:entry[count(adsm:action)!=1]))",
                        "boolean(//atom:entry/adsm:action[@title='ActionOnSingleEntity_Void'] and not(//atom:entry/adsm:action[@title!='ActionOnSingleEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$expand=ResourceSetReferenceProperty($select=ID,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean((/atom:feed/atom:entry|/atom:entry)/atom:content/adsm:properties)",
                        "boolean((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties/ads:ID)",
                        "boolean((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry/atom:content/adsm:properties/*[local-name()!='ID'])",
                        "not    ((/atom:feed/atom:entry|/atom:entry)[count(adsm:action)=0])",
                        "not    ((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry[count(adsm:action)=1] and not((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry[count(adsm:action)!=1]))",
                        "not    ((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry/adsm:action[@title='ActionOnSingleEntity_Void'] and not((/atom:feed/atom:entry|/atom:entry)/atom:link/adsm:inline/atom:feed/atom:entry/adsm:action[@title!='ActionOnSingleEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "not    (//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=1] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=0])",
                        "boolean(//atom:entry/adsm:action[@title='ActionOnSingleDerivedEntity_Void'] and not(//atom:entry/adsm:action[@title!='ActionOnSingleDerivedEntity_Void']))"
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void)",
                    ErrorMessage = default(string),
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "not    (//atom:entry/atom:content/adsm:properties)",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=2] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and count(adsm:action)=1])",
                        "boolean(//atom:entry[atom:link[@rel='edit' and @href='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and (adsm:action/@title='ActionOnSingleDerivedEntity_Void' or adsm:action/@title='ActionOnSingleEntity_Void')] and //atom:entry[atom:link[@rel='edit' and @href!='Set(4)/AstoriaUnitTests.Tests.Actions.DerivedEntityType'] and adsm:action/@title='ActionOnSingleEntity_Void'])"
                    },
                },

                // Error cases
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.*Foo",
                    ErrorMessage = "Syntax error at position 32 in 'AstoriaUnitTests.Tests.Actions.*Foo'.",
                    StatusCode = 400,
                    XPaths = new string[] { },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.*.",
                    ErrorMessage = "Syntax error at position 32 in 'AstoriaUnitTests.Tests.Actions.*.'.",
                    StatusCode = 400,
                    XPaths = new string[] { },
                },
                new {
                    RequestUri = "/Set?$select=*,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void/Foo",
                    ErrorMessage = "The type 'AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void' is not defined in the model.",
                    StatusCode = 400,
                    XPaths = new string[] { },
                },
            };

            var service = ActionTests.ModelWithActions();
            service.ForceVerboseErrors = true;
            List<string> failingTests = new List<string>();
            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                try
                {
                    using (TestWebRequest request = service.CreateForInProcess())
                    {
                        request.HttpMethod = "GET";
                        request.RequestUriString = testCase.RequestUri;
                        request.Accept = "application/atom+xml,application/xml";

                        Exception e = t.TestUtil.RunCatching(request.SendRequest);

                        Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode);
                        if (testCase.ErrorMessage == null)
                        {
                            Assert.IsNull(e);
                            if (testCase.XPaths.Length > 0)
                            {
                                UnitTestsUtil.VerifyXPathExpressionResults(request.GetResponseStreamAsXDocument(), true, testCase.XPaths);
                            }
                        }
                        else
                        {
                            while (e.InnerException != null)
                            {
                                e = e.InnerException;
                            }

                            Assert.AreEqual(testCase.ErrorMessage, e.Message);
                        }
                    }
                }
                catch (Exception)
                {
                    failingTests.Add(testCase.RequestUri);
                    throw;
                }

            });
        }

        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Variation("Tests the next link with selected actions.")]
        public void ActionNextLinkWithSelectExpandTests()
        {
            var testCases = new[]
            {
                // $select test cases
                new {
                    RequestUri = "/Set?$select=ID,AstoriaUnitTests.Tests.Actions.*",
                    XPaths = new string[]
                    {
                        "boolean(/atom:feed/atom:link[@rel='next' and @href='http://host/Set?$select=ID,AstoriaUnitTests.Tests.Actions.*&$skiptoken=2'])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=*,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void",
                    XPaths = new string[]
                    {
                        "boolean(/atom:feed/atom:link[@rel='next' and @href='http://host/Set?$select=*,AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void&$skiptoken=2'])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void",
                    XPaths = new string[]
                    {
                        "boolean(/atom:feed/atom:link[@rel='next' and @href='http://host/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$skiptoken=2'])",
                    },
                },

                // $select + $expand test cases
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.*&$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID,AstoriaUnitTests.Tests.Actions.*)",
                    XPaths = new string[]
                    {
                        "boolean(//atom:link[@rel='next' and @href='http://host/Set(1)/ResourceSetReferenceProperty?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID,AstoriaUnitTests.Tests.Actions.*&$skiptoken=3'])",
                        "boolean(//atom:link[@rel='next' and @href='http://host/Set(2)/ResourceSetReferenceProperty?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID,AstoriaUnitTests.Tests.Actions.*&$skiptoken=4'])",
                        "boolean(/atom:feed/atom:link[@rel='next' and @href='http://host/Set?$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID,AstoriaUnitTests.Tests.Actions.*)&$select=AstoriaUnitTests.Tests.Actions.*&$skiptoken=2'])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.*&$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.*,AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID)",
                    XPaths = new string[]
                    {
                        "boolean(//atom:link[@rel='next' and @href='http://host/Set(1)/ResourceSetReferenceProperty?$select=AstoriaUnitTests.Tests.Actions.*,AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID&$skiptoken=3'])",
                        "boolean(//atom:link[@rel='next' and @href='http://host/Set(2)/ResourceSetReferenceProperty?$select=AstoriaUnitTests.Tests.Actions.*,AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID&$skiptoken=4'])",
                        "boolean(/atom:feed/atom:link[@rel='next' and @href='http://host/Set?$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.*,AstoriaUnitTests.Tests.Actions.DerivedEntityType/ID)&$select=AstoriaUnitTests.Tests.Actions.*&$skiptoken=2'])",
                    },
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void)",
                    XPaths = new string[]
                    {
                        "boolean(//atom:link[@rel='next' and @href='http://host/Set(1)/ResourceSetReferenceProperty?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$skiptoken=3'])",
                        "boolean(//atom:link[@rel='next' and @href='http://host/Set(2)/ResourceSetReferenceProperty?$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$skiptoken=4'])",
                        "boolean(/atom:feed/atom:link[@rel='next' and @href='http://host/Set?$expand=ResourceSetReferenceProperty($select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void)&$select=AstoriaUnitTests.Tests.Actions.ActionOnSingleEntity_Void,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.ActionOnSingleDerivedEntity_Void&$skiptoken=2'])",
                    },
                }
            };

            var service = ActionTests.ModelWithActions();
            service.ForceVerboseErrors = true;
            testCases = testCases.Skip(5).Take(1).ToArray();
            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                service.PageSizeCustomizer = (config, type) => config.SetEntitySetPageSize("*", 2);
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = "GET";
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestUriString = testCase.RequestUri;

                    Exception e = t.TestUtil.RunCatching(request.SendRequest);
                    Assert.IsNull(e);

                    Assert.AreEqual(200, request.ResponseStatusCode);
                    if (testCase.XPaths.Length > 0)
                    {
                        UnitTestsUtil.VerifyXPathExpressionResults(request.GetResponseStreamAsXDocument(), true, testCase.XPaths);
                    }
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Tests Accessibility of service actions in $metadata.")]
        public void ActionMetadataAuthorizationTest()
        {
            DSPMetadata metadata = new DSPMetadata("ModelWithActions", "AstoriaUnitTests.Tests.Actions");

            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));

            var entityType2 = metadata.AddEntityType("EntityType2", null, null, false);
            metadata.AddKeyProperty(entityType2, "ID", typeof(int));

            var resourceReferenceProperty2 = metadata.AddResourceReferenceProperty(entityType, "ResourceReferenceProperty2", entityType2);
            var resourceSetReferenceProperty2 = metadata.AddResourceSetReferenceProperty(entityType, "ResourceSetReferenceProperty2", entityType2);

            var set = metadata.AddResourceSet("Set", entityType);
            var set2 = metadata.AddResourceSet("Set2", entityType2);

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceReference2",
                new ResourceAssociationSetEnd(set, entityType, resourceReferenceProperty2),
                new ResourceAssociationSetEnd(set2, entityType2, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceSetReference2",
                new ResourceAssociationSetEnd(set, entityType, resourceSetReferenceProperty2),
                new ResourceAssociationSetEnd(set2, entityType2, null)));

            var complexParameterType = metadata.AddComplexType("ComplexParameterType", null, null, false);
            var complexParameterNestedType = metadata.AddComplexType("ComplexParameterNestedType", null, null, false);
            metadata.AddPrimitiveProperty(complexParameterNestedType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(complexParameterType, "ComplexProperty", complexParameterNestedType);

            var complexReturnType = metadata.AddComplexType("ComplexReturnType", null, null, false);
            var complexReturnNestedType = metadata.AddComplexType("ComplexReturnNestedType", null, null, false);
            metadata.AddPrimitiveProperty(complexReturnNestedType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(complexReturnType, "ComplexProperty", complexReturnNestedType);

            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            DSPActionProvider actionProvider = new DSPActionProvider();

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                ActionProvider = actionProvider,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };

            ResourceType entityCollectionType = ResourceType.GetEntityCollectionResourceType(entityType);
            ResourceType entityCollectionType2 = ResourceType.GetEntityCollectionResourceType(entityType2);
            ServiceActionParameter entityParam = new ServiceActionParameter("entity", entityType);
            ServiceActionParameter entityCollectionParam = new ServiceActionParameter("entityCollection", entityCollectionType);
            ServiceActionParameter complexParam = new ServiceActionParameter("complexParam", complexParameterType);

            var testCases = new[]
            {
                new
                {
                    ServiceAction = new ServiceAction("Action1", complexReturnType, OperationParameterBindingKind.Sometimes, new[] { entityParam }, null),
                    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'AstoriaUnitTests.Tests.Actions.EntityType', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'AstoriaUnitTests.Tests.Actions.EntityType' should be made visible.",
                    ErrorMsgWhenSet2Hidden = default(string),
                    ComplexParameterTypeVisible = false,
                    ComplexReturnTypeVisible = true,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", complexReturnType, OperationParameterBindingKind.Sometimes, new[] { entityCollectionParam }, null),
                    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)' should be made visible.",
                    ErrorMsgWhenSet2Hidden = default(string),
                    ComplexParameterTypeVisible = false,
                    ComplexReturnTypeVisible = true,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", entityType2, set2, OperationParameterBindingKind.Never, new[] { complexParam }),
                    ErrorMsgWhenSet1Hidden = default(string),
                    ErrorMsgWhenSet2Hidden = "The operation 'Action1' has the resource set 'Set2' that is not visible. The operation 'Action1' should be made hidden or the resource set 'Set2' should be made visible.",
                    ComplexParameterTypeVisible = true,
                    ComplexReturnTypeVisible = false,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", entityCollectionType2, set2 , OperationParameterBindingKind.Never, new[] { complexParam }),
                    ErrorMsgWhenSet1Hidden = default(string),
                    ErrorMsgWhenSet2Hidden = "The operation 'Action1' has the resource set 'Set2' that is not visible. The operation 'Action1' should be made hidden or the resource set 'Set2' should be made visible.",
                    ComplexParameterTypeVisible = true,
                    ComplexReturnTypeVisible = false,
                },
                // TODO: These are all invalid configurations bound actions can't have this error: need to add toplevel actions and validate this.
                //new
                //{
                //    ServiceAction = new ServiceAction("Action1", entityType2, OperationParameterBindingKind.Sometimes, new[] { entityParam }, new ResourceSetPathExpression("entity/ResourceReferenceProperty2")),
                //    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'AstoriaUnitTests.Tests.Actions.EntityType', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'AstoriaUnitTests.Tests.Actions.EntityType' should be made visible.",
                //    ErrorMsgWhenSet2Hidden = "The operation 'Action1' has the resource set 'Set2' that is not visible. The operation 'Action1' should be made hidden or the resource set 'Set2' should be made visible.",
                //    ComplexParameterTypeVisible = false,
                //    ComplexReturnTypeVisible = false,
                //},
                //new
                //{
                //    ServiceAction = new ServiceAction("Action1", entityCollectionType2, OperationParameterBindingKind.Sometimes, new[] { entityParam }, new ResourceSetPathExpression("entity/ResourceSetReferenceProperty2")),
                //    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'AstoriaUnitTests.Tests.Actions.EntityType', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'AstoriaUnitTests.Tests.Actions.EntityType' should be made visible.",
                //    ErrorMsgWhenSet2Hidden = "The operation 'Action1' has the resource set 'Set2' that is not visible. The operation 'Action1' should be made hidden or the resource set 'Set2' should be made visible.",
                //    ComplexParameterTypeVisible = false,
                //    ComplexReturnTypeVisible = false,
                //},
                //new
                //{
                //    ServiceAction = new ServiceAction("Action1", entityType2, OperationParameterBindingKind.Sometimes, new[] { entityCollectionParam }, new ResourceSetPathExpression("entityCollection/ResourceReferenceProperty2")),
                //    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)' should be made visible.",
                //    ErrorMsgWhenSet2Hidden = "The operation 'Action1' has the resource set 'Set2' that is not visible. The operation 'Action1' should be made hidden or the resource set 'Set2' should be made visible.",
                //    ComplexParameterTypeVisible = false,
                //    ComplexReturnTypeVisible = false,
                //},
                //new
                //{
                //    ServiceAction = new ServiceAction("Action1", entityCollectionType2, OperationParameterBindingKind.Sometimes, new[] { entityCollectionParam }, new ResourceSetPathExpression("entityCollection/ResourceSetReferenceProperty2")),
                //    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)' should be made visible.",
                //    ErrorMsgWhenSet2Hidden = "The operation 'Action1' has the resource set 'Set2' that is not visible. The operation 'Action8' should be made hidden or the resource set 'Set2' should be made visible.",
                //    ComplexParameterTypeVisible = false,
                //    ComplexReturnTypeVisible = false,
                //},
                new
                {
                    ServiceAction = new ServiceAction("Action1", entityType2, OperationParameterBindingKind.Sometimes, new[] { entityParam }, new ResourceSetPathExpression("entity/ResourceReferenceProperty2")),
                    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'AstoriaUnitTests.Tests.Actions.EntityType', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'AstoriaUnitTests.Tests.Actions.EntityType' should be made visible.",
                    ErrorMsgWhenSet2Hidden = "The service action 'Action1' has the resource set path expression 'entity/ResourceReferenceProperty2', but there is no visible resource set that can be reached from the binding parameter through the path expression. The service action should be made hidden or a resource set targeted by the path expression should be made visible.",
                    ComplexParameterTypeVisible = false,
                    ComplexReturnTypeVisible = false,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", entityCollectionType2, OperationParameterBindingKind.Sometimes, new[] { entityParam }, new ResourceSetPathExpression("entity/ResourceSetReferenceProperty2")),
                    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'AstoriaUnitTests.Tests.Actions.EntityType', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'AstoriaUnitTests.Tests.Actions.EntityType' should be made visible.",
                    ErrorMsgWhenSet2Hidden = "The service action 'Action1' has the resource set path expression 'entity/ResourceSetReferenceProperty2', but there is no visible resource set that can be reached from the binding parameter through the path expression. The service action should be made hidden or a resource set targeted by the path expression should be made visible.",
                    ComplexParameterTypeVisible = false,
                    ComplexReturnTypeVisible = false,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", entityType2, OperationParameterBindingKind.Sometimes, new[] { entityCollectionParam }, new ResourceSetPathExpression("entityCollection/ResourceReferenceProperty2")),
                    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)' should be made visible.",
                    ErrorMsgWhenSet2Hidden = "The service action 'Action1' has the resource set path expression 'entityCollection/ResourceReferenceProperty2', but there is no visible resource set that can be reached from the binding parameter through the path expression. The service action should be made hidden or a resource set targeted by the path expression should be made visible.",
                    ComplexParameterTypeVisible = false,
                    ComplexReturnTypeVisible = false,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", entityCollectionType2, OperationParameterBindingKind.Sometimes,  new[] { entityCollectionParam }, new ResourceSetPathExpression("entityCollection/ResourceSetReferenceProperty2")),
                    ErrorMsgWhenSet1Hidden = "The service action 'Action1' has the binding parameter of type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)', but there is no visible resource set for that type. The service action should be made hidden or a resource set for type 'Collection(AstoriaUnitTests.Tests.Actions.EntityType)' should be made visible.",
                    ErrorMsgWhenSet2Hidden = "The service action 'Action1' has the resource set path expression 'entityCollection/ResourceSetReferenceProperty2', but there is no visible resource set that can be reached from the binding parameter through the path expression. The service action should be made hidden or a resource set targeted by the path expression should be made visible.",
                    ComplexParameterTypeVisible = false,
                    ComplexReturnTypeVisible = false,
                },
                new
                {
                    ServiceAction = new ServiceAction("Action1", complexReturnType, null, OperationParameterBindingKind.Never, new[] { complexParam }),
                    ErrorMsgWhenSet1Hidden = default(string),
                    ErrorMsgWhenSet2Hidden = default(string),
                    ComplexParameterTypeVisible = true,
                    ComplexReturnTypeVisible = true,
                },
            };

            var entitySetRights = new EntitySetRights[]
            {
                EntitySetRights.None,
                EntitySetRights.ReadSingle,
                EntitySetRights.ReadMultiple,
                EntitySetRights.WriteAppend,
                EntitySetRights.WriteReplace,
                EntitySetRights.WriteDelete,
                EntitySetRights.WriteMerge,
                EntitySetRights.AllRead,
                EntitySetRights.AllWrite,
                EntitySetRights.All
            };

            var disableAllActions = new Dictionary<string, ServiceActionRights>() { { "*", ServiceActionRights.None } };
            var enableAllActions = new Dictionary<string, ServiceActionRights>() { { "*", ServiceActionRights.Invoke } };
            var enableActionsByName = new Dictionary<string, ServiceActionRights>() { { "Action1", ServiceActionRights.Invoke } };
            var serviceActionAccessRules = new[] { disableAllActions, enableAllActions, enableActionsByName };

            using (TestWebRequest request = service.CreateForInProcess())
            {
                t.TestUtil.RunCombinations(testCases, entitySetRights, entitySetRights, serviceActionAccessRules, (testCase, set1Rights, set2Rights, serviceActionAccessRule) =>
                {
                    service.ActionProvider = new DSPActionProvider();
                    service.ActionProvider.AddAction(testCase.ServiceAction);

                    using (OpenWebDataServiceHelper.EntitySetAccessRule.Restore())
                    using (OpenWebDataServiceHelper.ServiceActionAccessRule.Restore())
                    {
                        OpenWebDataServiceHelper.EntitySetAccessRule.Value = new Dictionary<string, EntitySetRights>()
                        {
                            { "Set", set1Rights },
                            { "Set2", set2Rights },
                        };

                        OpenWebDataServiceHelper.ServiceActionAccessRule.Value = serviceActionAccessRule;

                        request.RequestUriString = "/$metadata";
                        request.HttpMethod = "GET";
                        Exception e = t.TestUtil.RunCatching(request.SendRequest);

                        if (set2Rights == EntitySetRights.None && serviceActionAccessRule != disableAllActions && testCase.ServiceAction.ResourceSet != null && testCase.ErrorMsgWhenSet2Hidden != null)
                        {
                            Assert.IsNotNull(e, "Expecting exception but received none.");
                            Assert.AreEqual(testCase.ErrorMsgWhenSet2Hidden, e.InnerException.Message);
                        }
                        else if (set1Rights == EntitySetRights.None && serviceActionAccessRule != disableAllActions && testCase.ErrorMsgWhenSet1Hidden != null)
                        {
                            Assert.IsNotNull(e, "Expecting exception but received none.");
                            Assert.AreEqual(testCase.ErrorMsgWhenSet1Hidden, e.InnerException.Message);
                        }
                        else if (set2Rights == EntitySetRights.None && serviceActionAccessRule != disableAllActions && testCase.ServiceAction.ResultSetPathExpression != null && testCase.ErrorMsgWhenSet2Hidden != null)
                        {
                            Assert.IsNotNull(e, "Expecting exception but received none.");
                            Assert.AreEqual(testCase.ErrorMsgWhenSet2Hidden, e.InnerException.Message);
                        }
                        else
                        {
                            Assert.IsNull(e, "Exception received but expecting none.");
                            var xpaths = new List<string>();

                            if (serviceActionAccessRule != disableAllActions)
                            {
                                Assert.AreEqual("4.0;", request.ResponseVersion);

                                xpaths.Add("//csdl:Action[@Name='Action1' and not(@adsm:HttpMethod)]");
                                if (testCase.ComplexParameterTypeVisible)
                                {
                                    xpaths.Add("//csdl:ComplexType[@Name='ComplexParameterType']");
                                    xpaths.Add("//csdl:ComplexType[@Name='ComplexParameterNestedType']");
                                }
                                else
                                {
                                    xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexParameterType']))");
                                    xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexParameterNestedType']))");
                                }

                                if (testCase.ComplexReturnTypeVisible)
                                {
                                    xpaths.Add("//csdl:ComplexType[@Name='ComplexReturnType']");
                                    xpaths.Add("//csdl:ComplexType[@Name='ComplexReturnNestedType']");
                                }
                                else
                                {
                                    xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexReturnType']))");
                                    xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexReturnNestedType']))");
                                }
                            }
                            else
                            {
                                Assert.AreEqual("4.0;", request.ResponseVersion);

                                xpaths.Add("boolean(not(//csdl:Action[@Name='Action1']))");
                                xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexParameterType']))");
                                xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexParameterNestedType']))");
                                xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexReturnType']))");
                                xpaths.Add("boolean(not(//csdl:ComplexType[@Name='ComplexReturnNestedType']))");
                            }

                            UnitTestsUtil.VerifyXPaths(request.GetResponseStream(), UnitTestsUtil.MimeApplicationXml, xpaths.ToArray());
                        }
                    }

                    t.TestUtil.ClearMetadataCache();
                });
            }
        }

        [TestCategory("Partition1"), TestMethod, Variation("Verify that we can call GetQueryStringValue(headerName) correctly in an IDataServiceActionProvider method.")]
        public void InvokeActionQueryStringHeaderTest()
        {
            var service = ActionTests.ModelWithActions();
            service.ForceVerboseErrors = true;

            using (TestWebRequest request = service.CreateForInProcess())
            {
                request.HttpMethod = "POST";
                request.RequestUriString = "/Set/AstoriaUnitTests.Tests.Actions.ActionOnEntityQueryable_Void?Query-String-Header-Force-Error=yes";
                request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                request.SetRequestStreamAsText(string.Empty);

                Exception e = t.TestUtil.RunCatching(request.SendRequest);
                Assert.AreEqual(418, request.ResponseStatusCode);
                Assert.IsTrue(request.GetResponseStreamAsText().Contains("User code threw a Query-String-Header-Force-Error exception."));
            }
        }

        internal static DSPServiceDefinition ModelWithActions()
        {
            DSPMetadata metadata = new DSPMetadata("ModelWithActions", "AstoriaUnitTests.Tests.Actions");

            var complexType = metadata.AddComplexType("ComplexType", null, null, false);
            metadata.AddPrimitiveProperty(complexType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(complexType, "ComplexProperty", complexType);

            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType, "Updated", typeof(bool), etag: true);
            metadata.AddPrimitiveProperty(entityType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(entityType, "ComplexProperty", complexType);
            metadata.AddCollectionProperty(entityType, "PrimitiveCollectionProperty", ResourceType.GetPrimitiveResourceType(typeof(string)));
            metadata.AddCollectionProperty(entityType, "ComplexCollectionProperty", complexType);
            var resourceReferenceProperty = metadata.AddResourceReferenceProperty(entityType, "ResourceReferenceProperty", entityType);
            var resourceSetReferenceProperty = metadata.AddResourceSetReferenceProperty(entityType, "ResourceSetReferenceProperty", entityType);

            var derivedEntityType = metadata.AddEntityType("DerivedEntityType", null, entityType, false);

            var entityType2 = metadata.AddEntityType("EntityType2", null, null, false);
            metadata.AddKeyProperty(entityType2, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType2, "Updated", typeof(bool), etag: true);

            var resourceReferenceProperty2 = metadata.AddResourceReferenceProperty(entityType, "ResourceReferenceProperty2", entityType2);
            var resourceSetReferenceProperty2 = metadata.AddResourceSetReferenceProperty(entityType, "ResourceSetReferenceProperty2", entityType2);

            var set = metadata.AddResourceSet("Set", entityType);
            var set2 = metadata.AddResourceSet("Set2", entityType2);

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceReference",
                new ResourceAssociationSetEnd(set, entityType, resourceReferenceProperty),
                new ResourceAssociationSetEnd(set, entityType, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceSetReference",
                new ResourceAssociationSetEnd(set, entityType, resourceSetReferenceProperty),
                new ResourceAssociationSetEnd(set, entityType, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceReference2",
                new ResourceAssociationSetEnd(set, entityType, resourceReferenceProperty2),
                new ResourceAssociationSetEnd(set2, entityType2, null)));

            metadata.AddResourceAssociationSet(new ResourceAssociationSet(
                "ResourceSetReference2",
                new ResourceAssociationSetEnd(set, entityType, resourceSetReferenceProperty2),
                new ResourceAssociationSetEnd(set2, entityType2, null)));

            var complexParameterType = metadata.AddComplexType("ComplexParameterType", null, null, false);
            var complexParameterNestedType = metadata.AddComplexType("ComplexParameterNestedType", null, null, false);
            metadata.AddPrimitiveProperty(complexParameterNestedType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(complexParameterType, "ComplexProperty", complexParameterNestedType);

            var complexReturnType = metadata.AddComplexType("ComplexReturnType", null, null, false);
            var complexReturnNestedType = metadata.AddComplexType("ComplexReturnNestedType", null, null, false);
            metadata.AddPrimitiveProperty(complexReturnNestedType, "PrimitiveProperty", typeof(string));
            metadata.AddComplexProperty(complexReturnType, "ComplexProperty", complexReturnNestedType);

            metadata.AddServiceOperation("GetEntity", ServiceOperationResultKind.QueryWithSingleResult, entityType, set, "GET", null);
            metadata.AddServiceOperation("GetEntities", ServiceOperationResultKind.QueryWithMultipleResults, entityType, set, "GET", null);

            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            DSPActionProvider actionProvider = new DSPActionProvider();

            ActionTests.SetupActions(actionProvider, context);

            DSPResource complex1 = new DSPResource(complexType);
            complex1.SetValue("PrimitiveProperty", "complex1");
            DSPResource complex2 = new DSPResource(complexType);
            complex2.SetValue("PrimitiveProperty", "complex2");
            complex2.SetValue("ComplexProperty", complex1);

            DSPResource entity1 = new DSPResource(entityType);
            entity1.SetValue("ID", 1);
            entity1.SetValue("Updated", false);
            entity1.SetValue("PrimitiveProperty", "entity1");
            entity1.SetValue("ComplexProperty", complex2);
            entity1.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity1.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });

            DSPResource entity2 = new DSPResource(entityType);
            entity2.SetValue("ID", 2);
            entity2.SetValue("Updated", false);
            entity2.SetValue("PrimitiveProperty", "entity2");
            entity2.SetValue("ComplexProperty", complex2);
            entity2.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity2.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });

            DSPResource entity3 = new DSPResource(entityType);
            entity3.SetValue("ID", 3);
            entity3.SetValue("Updated", false);
            entity3.SetValue("PrimitiveProperty", "entity3");
            entity3.SetValue("ComplexProperty", complex2);
            entity3.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity3.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });

            DSPResource entity4 = new DSPResource(derivedEntityType);
            entity4.SetValue("ID", 4);
            entity4.SetValue("Updated", false);
            entity4.SetValue("PrimitiveProperty", "entity4");
            entity4.SetValue("ComplexProperty", complex2);
            entity4.SetValue("PrimitiveCollectionProperty", new string[] { "value1", "value2" });
            entity4.SetValue("ComplexCollectionProperty", new[] { complex1, complex2 });

            entity1.SetValue("ResourceReferenceProperty", entity2);
            entity2.SetValue("ResourceReferenceProperty", entity3);
            entity3.SetValue("ResourceReferenceProperty", entity4);
            entity4.SetValue("ResourceReferenceProperty", entity1);

            entity1.SetValue("ResourceSetReferenceProperty", new[] { entity2, entity3 });
            entity2.SetValue("ResourceSetReferenceProperty", new[] { entity3, entity4 });
            entity3.SetValue("ResourceSetReferenceProperty", new[] { entity4, entity1 });
            entity4.SetValue("ResourceSetReferenceProperty", new[] { entity1, entity2 });

            entity1.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);
            entity2.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);
            entity3.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);
            entity4.SetValue("ResourceSetReferenceProperty2", new DSPResource[0]);

            context.GetResourceSetEntities("Set").Add(entity1);
            context.GetResourceSetEntities("Set").Add(entity2);
            context.GetResourceSetEntities("Set").Add(entity3);
            context.GetResourceSetEntities("Set").Add(entity4);

            context.ServiceOperations["GetEntity"] = (args) => (new[] { entity1 }).AsQueryable();
            context.ServiceOperations["GetEntities"] = (args) => (new[] { entity1, entity2, entity3, entity4 }).AsQueryable();

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                ActionProvider = actionProvider,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };

            return service;
        }

        private static ResourceType GetResourceTypeFromType(DSPMetadata metadata, Type type, string itemTypeName)
        {
            ResourceType resourceType;
            Type itemType = type;
            if (!type.IsPrimitive && type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type))
            {
                itemType = type.GetGenericArguments()[0];
            }

            if (itemType == typeof(DSPResource))
            {
                metadata.TryResolveResourceType(itemTypeName, out resourceType);
            }
            else
            {
                resourceType = ResourceType.GetPrimitiveResourceType(itemType);
            }

            if (type != itemType)
            {
                if (resourceType.ResourceTypeKind == ResourceTypeKind.EntityType)
                {
                    resourceType = ResourceType.GetEntityCollectionResourceType(resourceType);
                }
                else
                {
                    resourceType = ResourceType.GetCollectionResourceType(resourceType);
                }
            }

            return resourceType;
        }

        private static MethodInfo[] GetActionsFromContextType()
        {
            return typeof(ActionContext).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Where(m => m.GetCustomAttributes(typeof(DSPActionAttribute), true).FirstOrDefault() != null).ToArray();
        }

        internal static void SetupActions(DSPActionProvider actionProvider, DSPContext context)
        {
            ActionContext aContext = new ActionContext(context);
            foreach (MethodInfo method in ActionTests.GetActionsFromContextType())
            {
                actionProvider.AddAction(method, method.IsStatic ? null : aContext);
            }
        }

        private class ActionContext
        {
            private DSPContext context;

            public ActionContext(DSPContext context)
            {
                this.context = context;
            }

            #region Top Level Actions
            [DSPActionAttribute(OperationParameterBindingKind.Never)]
            public void TopLevelAction_Void()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never)]
            public string TopLevelAction_Primitive()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (string)resource.GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public DSPResource TopLevelAction_Complex()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (DSPResource)resource.GetValue("ComplexProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never)]
            public IEnumerable<string> TopLevelAction_PrimitiveCollection()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (IEnumerable<string>)resource.GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public IEnumerable<DSPResource> TopLevelAction_ComplexCollection()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return (IEnumerable<DSPResource>)resource.GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public DSPResource TopLevelAction_Entity()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return resource;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IEnumerable<DSPResource> TopLevelAction_EntityCollection()
            {
                DSPResource resource = (DSPResource)this.context.GetResourceSetEntities("Set").First();
                resource.SetValue("Updated", true);
                return new List<DSPResource>() { resource };
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IQueryable<DSPResource> TopLevelAction_EntityQueryable()
            {
                return this.TopLevelAction_EntityCollection().AsQueryable();
            }
            #endregion Top Level Actions

            #region Top level actions returning null values
            [DSPActionAttribute(OperationParameterBindingKind.Never)]
            public string TopLevelAction_Primitive_Null()
            {
                return null;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public DSPResource TopLevelAction_Complex_Null()
            {
                return null;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never)]
            public IEnumerable<string> TopLevelAction_PrimitiveCollection_Null()
            {
                return null;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public IEnumerable<DSPResource> TopLevelAction_ComplexCollection_Null()
            {
                return null;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public DSPResource TopLevelAction_Entity_Null()
            {
                return null;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IEnumerable<DSPResource> TopLevelAction_EntityCollection_Null()
            {
                return null;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IQueryable<DSPResource> TopLevelAction_EntityQueryable_Null()
            {
                return null;
            }
            #endregion Top level actions returning null values

            #region Top level actions returning empty collections
            [DSPActionAttribute(OperationParameterBindingKind.Never)]
            public IEnumerable<string> TopLevelAction_PrimitiveCollection_Empty()
            {
                return new string[0];
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType")]
            public IEnumerable<DSPResource> TopLevelAction_ComplexCollection_Empty()
            {
                return new DSPResource[0];
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IEnumerable<DSPResource> TopLevelAction_EntityCollection_Empty()
            {
                return new DSPResource[0];
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSet = "Set")]
            public IQueryable<DSPResource> TopLevelAction_EntityQueryable_Empty()
            {
                return this.TopLevelAction_EntityCollection_Empty().AsQueryable();
            }
            #endregion Top level actions returning empty collections

            #region Top level actions expecting null parameter
            [DSPActionAttribute(OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "" })]
            public string TopLevelActionWithNullParam_Primitive(string value)
            {
                if (value != null)
                {
                    throw new DataServiceException(418, "Expect parameter to be null.");
                }

                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "" })]
            public IEnumerable<string> TopLevelActionWithNullParam_PrimitiveCollection(IEnumerable<string> value)
            {
                if (value != null)
                {
                    throw new DataServiceException(418, "Expect parameter to be null.");
                }

                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "" })]
            public IQueryable<string> TopLevelActionWithNullParam_PrimitiveQueryable(IQueryable<string> value)
            {
                if (value != null)
                {
                    throw new DataServiceException(418, "Expect parameter to be null.");
                }

                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public DSPResource TopLevelActionWithNullParam_Complex(DSPResource value)
            {
                if (value != null)
                {
                    throw new DataServiceException(418, "Expect parameter to be null.");
                }

                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public IEnumerable<DSPResource> TopLevelActionWithNullParam_ComplexCollection(IEnumerable<DSPResource> value)
            {
                if (value != null)
                {
                    throw new DataServiceException(418, "Expect parameter to be null.");
                }

                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public IQueryable<DSPResource> TopLevelActionWithNullParam_ComplexQueryable(IQueryable<DSPResource> value)
            {
                if (value != null)
                {
                    throw new DataServiceException(418, "Expect parameter to be null.");
                }

                return value;
            }
            #endregion Top level actions expecting null parameter

            #region Action on single entity
            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static void ActionOnSingleEntity_Void(DSPResource resource)
            {
                resource.SetValue("Updated", true);
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static string ActionOnSingleEntity_Primitive(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (string)resource.GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnSingleEntity_Complex(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (DSPResource)resource.GetValue("ComplexProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<string> ActionOnSingleEntity_PrimitiveCollection(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (IEnumerable<string>)resource.GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnSingleEntity_ComplexCollection(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (IEnumerable<DSPResource>)resource.GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource/ResourceReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnSingleEntity_Entity(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return resource;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnSingleEntity_EntityCollection(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return new List<DSPResource>() { resource };
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnSingleEntity_EntityQueryable(DSPResource resource)
            {
                return ActionOnSingleEntity_EntityCollection(resource).AsQueryable();
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource/ResourceReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnSingleEntity_EntityWithPath(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (DSPResource)resource.GetValue("ResourceReferenceProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnSingleEntity_EntityCollectionWithPath(DSPResource resource)
            {
                resource.SetValue("Updated", true);
                return (DSPResource[])resource.GetValue("ResourceSetReferenceProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resource/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnSingleEntity_EntityQueryableWithPath(DSPResource resource)
            {
                return ActionOnSingleEntity_EntityCollectionWithPath(resource).AsQueryable();
            }
            #endregion Action on single entity

            #region Action on entity collection
            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static void ActionOnEntityCollection_Void(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static string ActionOnEntityCollection_Primitive(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (string)resources.First().GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityCollection_Complex(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (DSPResource)resources.First().GetValue("ComplexProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<string> ActionOnEntityCollection_PrimitiveCollection(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<string>)((DSPResource)resources.First()).GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollection_ComplexCollection(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<DSPResource>)((DSPResource)resources.First()).GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityCollection_Entity(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return resources.First();
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollection_EntityCollection(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return new[] { resources.First() };
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnEntityCollection_EntityQueryable(IEnumerable<DSPResource> resources)
            {
                return ActionOnEntityCollection_EntityCollection(resources).AsQueryable();
            }
            #endregion Action on entity collection

            #region Action on entity queryable
            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static void ActionOnEntityQueryable_Void(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static string ActionOnEntityQueryable_Primitive(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (string)resources.First().GetValue("PrimitiveProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityQueryable_Complex(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (DSPResource)resources.First().GetValue("ComplexProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<string> ActionOnEntityQueryable_PrimitiveCollection(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<string>)((DSPResource)resources.First()).GetValue("PrimitiveCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityQueryable_ComplexCollection(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return (IEnumerable<DSPResource>)((DSPResource)resources.First()).GetValue("ComplexCollectionProperty");
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static DSPResource ActionOnEntityQueryable_Entity(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return resources.First();
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IEnumerable<DSPResource> ActionOnEntityQueryable_EntityCollection(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }

                return new[] { resources.First() };
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType", ReturnSetPath = "resources/ResourceSetReferenceProperty", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType" })]
            public static IQueryable<DSPResource> ActionOnEntityQueryable_EntityQueryable(IQueryable<DSPResource> resources)
            {
                return ActionOnEntityQueryable_EntityCollection(resources).AsQueryable();
            }
            #endregion Action on entity queryable

            #region Top Level Action with Single Parameter
            [DSPActionAttribute(OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "" })]
            public string TopLevelActionWithParam_Primitive(string value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "" })]
            public IEnumerable<string> TopLevelActionWithParam_PrimitiveCollection(IEnumerable<string> value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ParameterTypeNames = new string[] { "" })]
            public IQueryable<string> TopLevelActionWithParam_PrimitiveQueryable(IQueryable<string> value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public DSPResource TopLevelActionWithParam_Complex(DSPResource value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public IEnumerable<DSPResource> TopLevelActionWithParam_ComplexCollection(IEnumerable<DSPResource> value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Never, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public IQueryable<DSPResource> TopLevelActionWithParam_ComplexQueryable(IQueryable<DSPResource> value)
            {
                return value;
            }
            #endregion Top Level Action with Single Parameter

            #region Bindable Action with Single Parameter
            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String" })]
            public static string ActionOnEntityCollectionWithParam_Primitive(IQueryable<DSPResource> resource, string value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String" })]
            public static IEnumerable<string> ActionOnEntityCollectionWithParam_PrimitiveCollection(IQueryable<DSPResource> resource, IEnumerable<string> value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String" })]
            public static IQueryable<string> ActionOnEntityCollectionWithParam_PrimitiveQueryable(IQueryable<DSPResource> resource, IQueryable<string> value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static DSPResource ActionOnEntityCollectionWithParam_Complex(IQueryable<DSPResource> resource, DSPResource value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollectionWithParam_ComplexCollection(IQueryable<DSPResource> resource, IEnumerable<DSPResource> value)
            {
                return value;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static IQueryable<DSPResource> ActionOnEntityCollectionWithParam_ComplexQueryable(IQueryable<DSPResource> resource, IQueryable<DSPResource> value)
            {
                return value;
            }
            #endregion Bindable Action with Single Parameter

            #region Bindable Action with Two Parameters
            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String", "System.Int32" })]
            public static string ActionOnEntityCollectionWithParam_Primitive_Primitive(IQueryable<DSPResource> resource, string value1, int value2)
            {
                return value1 + value2;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.String", "System.Int32" })]
            public static IEnumerable<string> ActionOnEntityCollectionWithParam_PrimitiveCollection_Primitive(IQueryable<DSPResource> resource, IEnumerable<string> value1, int value2)
            {
                if (value1 == null)
                {
                    throw new DataServiceException(400, "value1 must not be null.");
                }

                foreach (var v in value1)
                {
                    yield return v + value2;
                }
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.Int32", "System.String" })]
            public static IQueryable<string> ActionOnEntityCollectionWithParam_Primitive_PrimitiveQueryable(IQueryable<DSPResource> resource, int value1, IQueryable<string> value2)
            {
                if (value1 == 0)
                {
                    throw new DataServiceException(400, "value1 must not be 0");
                }

                return value2;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static DSPResource ActionOnEntityCollectionWithParam_Complex_Complex(IQueryable<DSPResource> resource, DSPResource value1, DSPResource value2)
            {
                if (value1 == null)
                {
                    throw new DataServiceException(400, "value1 cannot be null.");
                }

                if (value2 == null)
                {
                    throw new DataServiceException(400, "value2 cannot be null.");
                }

                return value1;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "AstoriaUnitTests.Tests.Actions.ComplexType", "System.Int32" })]
            public static IEnumerable<DSPResource> ActionOnEntityCollectionWithParam_ComplexCollection_Primitive(IQueryable<DSPResource> resource, IEnumerable<DSPResource> value1, int value2)
            {
                if (value2 == 0)
                {
                    throw new DataServiceException(400, "value2 must not be 0");
                }

                return value1;
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.ComplexType", ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.EntityType", "System.Int32", "AstoriaUnitTests.Tests.Actions.ComplexType" })]
            public static IQueryable<DSPResource> ActionOnEntityCollectionWithParam_PrimitiveCollection_ComplexQueryable(IQueryable<DSPResource> resource, IEnumerable<int> value1, IQueryable<DSPResource> value2)
            {
                if (value1.Count() == 0)
                {
                    throw new DataServiceException(400, "value1 must contain elements");
                }

                return value2;
            }
            #endregion Bindable Action with Two Parameters

            #region Actions binding to derived type

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.DerivedEntityType" })]
            public static void ActionOnSingleDerivedEntity_Void(DSPResource resource)
            {
                resource.SetValue("Updated", true);
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.DerivedEntityType" })]
            public static void ActionOnDerivedEntityCollection_Void(IEnumerable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }
            }

            [DSPActionAttribute(OperationParameterBindingKind.Sometimes, ParameterTypeNames = new string[] { "AstoriaUnitTests.Tests.Actions.DerivedEntityType" })]
            public static void ActionOnDerivedEntityQueryable_Void(IQueryable<DSPResource> resources)
            {
                foreach (var resource in resources)
                {
                    resource.SetValue("Updated", true);
                }
            }

            #endregion Actions binding to derived type
        }

        private class EFActions
        {
            private ObjectContext objectContext;

            public EFActions(ObjectContext objectContext)
            {
                this.objectContext = objectContext;
            }

            [DSPAction(OperationParameterBindingKind.Sometimes, ReturnSetPath = "c", ParameterTypeNames = new string[] { "NorthwindModel.Customers", "System.String" })]
            public Customers UpdateAddress(Customers c, string newAddress)
            {
                ObjectStateEntry ose;
                if (!this.objectContext.ObjectStateManager.TryGetObjectStateEntry(c, out ose))
                {
                    this.objectContext.Attach(c);
                }

                c.Address = newAddress;
                this.objectContext.ApplyCurrentValues("Customers", c);
                return c;
            }

            [DSPAction(OperationParameterBindingKind.Sometimes, ReturnSetPath = "cs", ParameterTypeNames = new string[] { "NorthwindModel.Customers", "System.String" })]
            public IEnumerable<Customers> UpdateFirst2Addresses(IQueryable<Customers> cs, string newAddress)
            {
                List<Customers> customers = new List<Customers>();
                foreach (Customers c in cs.Take(2))
                {
                    ObjectStateEntry ose;
                    if (!this.objectContext.ObjectStateManager.TryGetObjectStateEntry(c, out ose))
                    {
                        this.objectContext.Attach(c);
                    }

                    customers.Add(c);
                    c.Address = newAddress;
                    this.objectContext.ApplyCurrentValues("Customers", c);
                }

                return customers;
            }

            [DSPAction(OperationParameterBindingKind.Never, ReturnSet = "Customers", ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType")]
            public IQueryable<Customers> IQueryableCustomers()
            {
                List<Customers> customers = new List<Customers>();
                for (int i = 0; i < 2; ++i)
                {
                    Customers c = new Customers();
                    c.CustomerID = "ID_" + i;
                    customers.Add(c);
                }

                return customers.AsQueryable<Customers>();
            }

            [DSPAction(OperationParameterBindingKind.Always, ReturnSetPath = "customers", ParameterTypeNames = new string[] { "NorthwindModel.Customers" }, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType")]
            public IQueryable<Customers> IQueryableCustomersWithBinding(IQueryable<Customers> customers)
            {
                return customers.Take(2);
            }

            [DSPAction(OperationParameterBindingKind.Always, ReturnSetPath = "customers", ParameterTypeNames = new string[] { "NorthwindModel.Customers" }, ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType")]
            public IEnumerable<Customers> IEnumerableCustomersWithBinding(IQueryable<Customers> customers)
            {
                return customers.ToList<Customers>().Take(2).ToList();
            }

            [DSPAction(OperationParameterBindingKind.Never, ReturnSet = "Customers", ReturnElementTypeName = "AstoriaUnitTests.Tests.Actions.EntityType")]
            public IEnumerable<Customers> IEnumerableCustomers()
            {
                List<Customers> customers = new List<Customers>();
                for (int i = 0; i < 2; ++i)
                {
                    Customers c = new Customers();
                    c.CustomerID = "ID_" + i;
                    customers.Add(c);
                }

                return customers;
            }
        }

        private class MyDSPActionProvider : DSPActionProvider
        {
            public static Action<DataServiceOperationContext> OperationContextTestMethod;

            public override bool TryResolveServiceAction(DataServiceOperationContext operationContext, string serviceActionName, out ServiceAction serviceAction)
            {
                if (MyDSPActionProvider.OperationContextTestMethod != null)
                {
                    MyDSPActionProvider.OperationContextTestMethod(operationContext);
                }

                return base.TryResolveServiceAction(operationContext, serviceActionName, out serviceAction);
            }
        }

        public class EFActionDataService : NorthwindTempDbServiceBase<NorthwindContext>, IServiceProvider
        {
            public static new void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);

                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.DataServiceBehavior.AcceptProjectionRequests = true;
                config.UseVerboseErrors = true;
            }

            [WebGet]
            public IQueryable<NorthwindModel.Customers> GetFirstTwoCustomers()
            {
                var context = this.CurrentDataSource;
                return context.CreateObjectSet<NorthwindModel.Customers>().Take(2);
            }

            [WebGet]
            [SingleResult]
            public IQueryable<NorthwindModel.Customers> GetCustomer(string customerName)
            {
                var context = this.CurrentDataSource;
                return context.CreateObjectSet<NorthwindModel.Customers>().Where(c => c.CustomerID == customerName).Take(1);
            }

            [WebInvoke]
            [SingleResult]
            public IQueryable<NorthwindModel.Customers> GetCustomerUsingInvoke()
            {
                var context = this.CurrentDataSource;
                return context.CreateObjectSet<NorthwindModel.Customers>().Take(1);
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                if (serviceType == typeof(IDataServiceActionProvider))
                {
                    var actionProvider = new MyDSPActionProvider();
                    var actionInstance = new EFActions((ObjectContext)this.CurrentDataSource);

                    foreach (MethodInfo method in typeof(EFActions).GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(m => m.GetCustomAttributes(typeof(DSPActionAttribute), false).Any()))
                    {
                        actionProvider.AddAction(method, actionInstance);
                    }

                    return actionProvider;
                }

                return null;
            }
        }

        public class EFActionDataServiceWithPaging : EFActionDataService
        {
            public static new void InitializeService(DataServiceConfiguration config)
            {
                config.SetEntitySetAccessRule("*", EntitySetRights.All);
                config.SetServiceOperationAccessRule("*", ServiceOperationRights.All);
                config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);

                config.DataServiceBehavior.MaxProtocolVersion = ODataProtocolVersion.V4;
                config.DataServiceBehavior.AcceptProjectionRequests = true;
                config.UseVerboseErrors = true;
                config.SetEntitySetPageSize("Customers", 1);
            }
        }

        [TestCategory("Partition1"), TestMethod, Variation("Invoke and validate EF service actions")]
        public void EFActionTest()
        {
            var testCases = new[]
            {
                new {
                    RequestUri = "/Customers('ALFKI')/NorthwindModel.UpdateAddress",
                    ValidateUri = "/Customers('ALFKI')",
                    RequestPayload = "{ \"newAddress\" : \"Updated Address\" }",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers/$entity\",\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Updated Address\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}}",
                    StatusCode = 200,
                },
                new {
                    RequestUri = "/Customers/NorthwindModel.UpdateFirst2Addresses",
                    ValidateUri = "/Customers?$top=2",
                    RequestPayload = "{ \"newAddress\" : \"Updated Address\" }",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Updated Address\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Updated Address\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },
                new {
                    RequestUri = "/GetFirstTwoCustomers/NorthwindModel.UpdateFirst2Addresses",
                    ValidateUri = "/Customers?$top=2",
                    RequestPayload = "{ \"newAddress\" : \"Updated Address 12_30\" }",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Updated Address 12_30\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Updated Address 12_30\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },
                new {
                    RequestUri = "/GetCustomer/NorthwindModel.UpdateAddress?customerName='ALFKI'",
                    ValidateUri = "/Customers('ALFKI')",
                    RequestPayload = "{ \"newAddress\" : \"Updated Address SingleCustomer\" }",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers/$entity\",\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Updated Address SingleCustomer\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}}",
                    StatusCode = 200,
                },
            };

            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                using (NorthwindDefaultTempDbService.SetupNorthwind())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(EFActionDataService);
                    request.Accept = "application/atom+xml,application/xml";
                    request.HttpMethod = "POST";
                    request.RequestUriString = testCase.RequestUri;

                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    if (testCase.RequestPayload == null)
                    {
                        request.RequestContentLength = 0;
                    }
                    else
                    {
                        request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                        request.SetRequestStreamAsText(testCase.RequestPayload);
                    }

                    request.SendRequest();
                    string response = request.GetResponseStreamAsText();
                    Assert.AreEqual(testCase.ExpectedResponsePayload, response);

                    // Verify that the change is persisted.
                    request.HttpMethod = "GET";
                    request.RequestUriString = testCase.ValidateUri;
                    request.RequestContentType = null;
                    request.RequestContentLength = 0;

                    request.SendRequest();
                    response = request.GetResponseStreamAsText();
                    Assert.AreEqual(testCase.ExpectedResponsePayload, response);
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Invoke and validate EF service actions with paging")]
        public void EFActionTestWithPaging()
        {
            var testCases = new[]
            {
                // A bindable action which returns IEnumerable. Expected: Paging should be discarded.
                new {
                    RequestUri = "/Customers/NorthwindModel.IEnumerableCustomersWithBinding",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Avda. de la Constituci\\u00f3n 2222\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },
  
                // A bindable action which returns IQueryable. Expected: Paging should be discarded.
                new {
                    RequestUri = "/Customers/NorthwindModel.IQueryableCustomersWithBinding",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Avda. de la Constituci\\u00f3n 2222\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },
  
                // A top level action which returns IQueryable. Expected: Paging should be discarded.
                new {
                    RequestUri = "/IQueryableCustomers",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ID_0\",\"CompanyName\":null,\"ContactName\":null,\"ContactTitle\":null,\"Address\":null,\"City\":null,\"Region\":null,\"PostalCode\":null,\"Phone\":null,\"Fax\":null,\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ID_1\",\"CompanyName\":null,\"ContactName\":null,\"ContactTitle\":null,\"Address\":null,\"City\":null,\"Region\":null,\"PostalCode\":null,\"Phone\":null,\"Fax\":null,\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },

                // A top level action which returns IEnumerable. Expected: Paging should be discarded.
                new {
                    RequestUri = "/IEnumerableCustomers",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ID_0\",\"CompanyName\":null,\"ContactName\":null,\"ContactTitle\":null,\"Address\":null,\"City\":null,\"Region\":null,\"PostalCode\":null,\"Phone\":null,\"Fax\":null,\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ID_1\",\"CompanyName\":null,\"ContactName\":null,\"ContactTitle\":null,\"Address\":null,\"City\":null,\"Region\":null,\"PostalCode\":null,\"Phone\":null,\"Fax\":null,\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },

                // Action composed from a service operation and returns IEnumerable.
                new {
                    RequestUri = "/GetFirstTwoCustomers/NorthwindModel.IEnumerableCustomersWithBinding",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Avda. de la Constituci\\u00f3n 2222\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },       
   
                // Action composed from a service operation and returns IQueryable
                new {
                    RequestUri = "/GetFirstTwoCustomers/NorthwindModel.IQueryableCustomersWithBinding",
                    ExpectedResponsePayload = "{\"@odata.context\":\"http://host/$metadata#Customers\",\"value\":[{\"CustomerID\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\",\"ContactName\":\"Maria Anders\",\"ContactTitle\":\"Sales Representative\",\"Address\":\"Obere Str. 57\",\"City\":\"Berlin\",\"Region\":null,\"PostalCode\":\"12209\",\"Phone\":\"030-0074321\",\"Fax\":\"030-0076545\",\"#NorthwindModel.UpdateAddress\":{}},{\"CustomerID\":\"ANATR\",\"CompanyName\":\"Ana Trujillo Emparedados y helados\",\"ContactName\":\"Ana Trujillo\",\"ContactTitle\":\"Owner\",\"Address\":\"Avda. de la Constituci\\u00f3n 2222\",\"City\":\"M\\u00e9xico D.F.\",\"Region\":null,\"PostalCode\":\"05021\",\"Phone\":\"(5) 555-4729\",\"Fax\":\"(5) 555-3745\",\"#NorthwindModel.UpdateAddress\":{}}]}",
                    StatusCode = 200,
                },
            };

            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                using (NorthwindDefaultTempDbService.SetupNorthwind())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(EFActionDataService);
                    request.HttpMethod = "POST";
                    request.RequestUriString = testCase.RequestUri;

                    request.Accept = UnitTestsUtil.JsonLightMimeType;
                    request.RequestContentLength = 0;
                    request.SendRequest();

                    string response = request.GetResponseStreamAsText();
                    Assert.AreEqual(testCase.ExpectedResponsePayload, response);
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Invoke and validate EF service actions")]
        public void EFProviderNegativeTests()
        {
            using (t.TestUtil.RestoreStaticMembersOnDispose(typeof(MyDSPActionProvider)))
            using (NorthwindDefaultTempDbService.SetupNorthwind())
            using (TestWebRequest request = TestWebRequest.CreateForInProcess())
            {
                request.ServiceType = typeof(EFActionDataService);
                request.HttpMethod = "GET";
                request.RequestUriString = "/Customers('ALFKI')";
                request.RequestContentType = null;
                request.RequestContentLength = 0;

                MyDSPActionProvider.OperationContextTestMethod = (operationContext) =>
                {
                    //
                    // Query provider
                    //
                    IDataServiceQueryProvider queryProvider = (IDataServiceQueryProvider)operationContext.GetService(typeof(IDataServiceQueryProvider));
                    Assert.IsNotNull(queryProvider);
                    Assert.IsNotNull(queryProvider.CurrentDataSource);

                    Exception e = t.TestUtil.RunCatching<ArgumentNullException>(() => queryProvider.GetQueryRootForResourceSet(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: container", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => queryProvider.GetResourceType(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => queryProvider.GetPropertyValue(null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: target", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => queryProvider.GetPropertyValue(new object(), null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resourceProperty", e.Message);

                    e = t.TestUtil.RunCatching<NotImplementedException>(() => queryProvider.GetOpenPropertyValue(null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The method or operation is not implemented.", e.Message);

                    e = t.TestUtil.RunCatching<NotImplementedException>(() => queryProvider.GetOpenPropertyValues(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The method or operation is not implemented.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => queryProvider.InvokeServiceOperation(null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: serviceOperation", e.Message);

                    //
                    // Metadata provider
                    //
                    IDataServiceMetadataProvider metadataProvider = (IDataServiceMetadataProvider)operationContext.GetService(typeof(IDataServiceMetadataProvider));
                    Assert.IsNotNull(metadataProvider);

                    Assert.AreEqual("NorthwindModel", metadataProvider.ContainerNamespace);
                    Assert.AreEqual("NorthwindContext", metadataProvider.ContainerName);
                    Assert.AreEqual(26, metadataProvider.ResourceSets.Count());
                    Assert.AreEqual(26, metadataProvider.Types.Count());
                    Assert.AreEqual(0, metadataProvider.ServiceOperations.Count());

                    ResourceSet set;
                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.TryResolveResourceSet(null, out set));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: name", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.TryResolveResourceSet("", out set));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: name", e.Message);

                    ResourceType type;
                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.TryResolveResourceType(null, out type));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: name", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.TryResolveResourceType("", out type));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: name", e.Message);

                    ServiceOperation operation;
                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.TryResolveServiceOperation(null, out operation));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: name", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.TryResolveServiceOperation("", out operation));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: name", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.GetDerivedTypes(null).Count());
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resourceType", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => metadataProvider.GetDerivedTypes(new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "abc", "pqr", false)).Count());
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The given resource type instance for the type 'abc.pqr' is not known to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.HasDerivedTypes(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resourceType", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => metadataProvider.HasDerivedTypes(new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "abc", "pqr", false)));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The given resource type instance for the type 'abc.pqr' is not known to the metadata provider.", e.Message);

                    ResourceSet customerSet;
                    metadataProvider.TryResolveResourceSet("Customers", out customerSet);
                    Assert.IsNotNull(customerSet);

                    ResourceType customerType = customerSet.ResourceType;
                    Assert.IsNotNull(customerType);

                    ResourceProperty orderProperty = customerType.Properties.Single(p => p.Name == "Orders");
                    ResourceType orderType = orderProperty.ResourceType;
                    Assert.IsNotNull(orderType);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.GetResourceAssociationSet(null, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resourceSet", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.GetResourceAssociationSet(customerSet, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resourceType", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentNullException>(() => metadataProvider.GetResourceAssociationSet(customerSet, customerType, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resourceProperty", e.Message);

                    ResourceSet fooSet = new ResourceSet("foo", customerType);
                    e = t.TestUtil.RunCatching<ArgumentException>(() => metadataProvider.GetResourceAssociationSet(fooSet, customerType, orderProperty));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The member with identity 'foo' does not exist in the metadata collection.\r\nParameter name: identity", e.Message);

                    ResourceType barType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "", "bar", false);
                    e = t.TestUtil.RunCatching<ArgumentException>(() => metadataProvider.GetResourceAssociationSet(customerSet, barType, orderProperty));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The member with identity 'bar' does not exist in the metadata collection.\r\nParameter name: identity", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => metadataProvider.GetResourceAssociationSet(customerSet, orderType, orderProperty));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource property 'Orders' must be a navigation property on the resource type 'NorthwindModel.Orders'.", e.Message);

                    ResourceProperty addressProperty = customerType.Properties.Single(p => p.Name == "Address");
                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => metadataProvider.GetResourceAssociationSet(customerSet, customerType, addressProperty));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource property 'Address' must be a navigation property on the resource type 'NorthwindModel.Customers'.", e.Message);

                    ResourceProperty myOrdersProperty = new ResourceProperty("Orders", ResourcePropertyKind.ResourceSetReference, orderType);
                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => metadataProvider.GetResourceAssociationSet(customerSet, customerType, myOrdersProperty));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource type 'NorthwindModel.Customers' must contain the resource property instance 'Orders'.", e.Message);

                    //
                    // Update Provider
                    //
                    IDataServiceUpdateProvider2 updateProvider = (IDataServiceUpdateProvider2)operationContext.GetService(typeof(IDataServiceUpdateProvider2));
                    Assert.IsNotNull(updateProvider);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.CreateResource(null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: fullTypeName", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.CreateResource(null, ""));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: fullTypeName", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.CreateResource(null, "foo"));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("A resource type named 'foo' does not exist in the metadata.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.CreateResource(null, "NorthwindModel.Customers"));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("A complex resource type is expected, however the resource type 'NorthwindModel.Customers' is of type kind 'EntityType'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.GetResource(null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: query", e.Message);

                    e = t.TestUtil.RunCatching<InvalidCastException>(() => updateProvider.GetResource(new string[0].AsQueryable(), null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Unable to cast object of type 'System.Linq.EnumerableQuery`1[System.String]' to type 'System.Data.Objects.ObjectQuery'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.ResetResource(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resource", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.ResetResource(new object()));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The clr type 'System.Object' is an unknown resource type to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetValue(null, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: targetResource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetValue(new object(), null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetValue(new object(), "", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetValue(new object(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The clr type 'System.Object' is an unknown resource type to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetValue(new Customers(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource type 'NorthwindModel.Customers' does not define a property that is named 'foo'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.GetValue(null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: targetResource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.GetValue(new object(), null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.GetValue(new object(), ""));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.GetValue(new object(), "foo"));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The clr type 'System.Object' is an unknown resource type to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.GetValue(new Customers(), "foo"));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource type 'NorthwindModel.Customers' does not define a property that is named 'foo'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.DeleteResource(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.ResolveResource(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetReference(null, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: targetResource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetReference(new object(), null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetReference(new object(), "", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetReference(new object(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The clr type 'System.Object' is an unknown resource type to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetReference(new Customers(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource type 'NorthwindModel.Customers' does not define a property that is named 'foo'.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetReference(new Customers(), "Address", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The property 'Address' must be a navigation property defined on the resource type 'NorthwindModel.Customers'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.AddReferenceToCollection(null, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: targetResource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.AddReferenceToCollection(new object(), null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.AddReferenceToCollection(new object(), "", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.AddReferenceToCollection(new object(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The clr type 'System.Object' is an unknown resource type to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.AddReferenceToCollection(new Customers(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource type 'NorthwindModel.Customers' does not define a property that is named 'foo'.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.AddReferenceToCollection(new Customers(), "Address", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The property 'Address' must be a navigation property defined on the resource type 'NorthwindModel.Customers'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.RemoveReferenceFromCollection(null, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: targetResource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.RemoveReferenceFromCollection(new object(), null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.RemoveReferenceFromCollection(new object(), "", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null or empty.\r\nParameter name: propertyName", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.RemoveReferenceFromCollection(new object(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The clr type 'System.Object' is an unknown resource type to the metadata provider.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.RemoveReferenceFromCollection(new Customers(), "foo", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The resource type 'NorthwindModel.Customers' does not define a property that is named 'foo'.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.RemoveReferenceFromCollection(new Customers(), "Address", null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The property 'Address' must be a navigation property defined on the resource type 'NorthwindModel.Customers'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetConcurrencyValues(null, null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: resource", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.SetConcurrencyValues(new object(), null, null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: concurrencyValues", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetConcurrencyValues(new object(), false, new KeyValuePair<string, object>[0]));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("If-None-Match HTTP header cannot be specified for update and delete operations.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetConcurrencyValues(new object(), null, new KeyValuePair<string, object>[0]));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Since entity type 'System.Object' has one or more etag properties, If-Match HTTP header must be specified for DELETE/PUT operations on this type.", e.Message);

                    e = t.TestUtil.RunCatching<InvalidOperationException>(() => updateProvider.SetConcurrencyValues(new object(), true, new KeyValuePair<string, object>[0]));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("The ObjectStateManager does not contain an ObjectStateEntry with a reference to an object of type 'System.Object'.", e.Message);

                    e = t.TestUtil.RunCatching<ArgumentException>(() => updateProvider.ScheduleInvokable(null));
                    Assert.IsNotNull(e);
                    Assert.AreEqual("Value cannot be null.\r\nParameter name: invokable", e.Message);
                };

                request.SendRequest();
            }
        }

        [TestCategory("Partition1"), TestMethod, Variation("Invoke and validate EF service actions")]
        public void EFProviderNegativeTests2()
        {
            var testCases = new[]
            {  
                // Action composed with a service operation using WebInvoke.
                new {
                    RequestUri = "/GetCustomerUsingInvoke/UpdateAddress",
                    StatusCode = 400,
                    ErrorMessage = "The request URI is not valid. The segment 'GetCustomerUsingInvoke' must be the last segment in the URI because it is one of the following: $ref, $batch, $count, $value, $metadata, a named media resource, an action, a noncomposable function, an action import, a noncomposable function import, an operation with void return type, or an operation import with void return type.",
                    HttpMethod = "POST",
                },

                // [Regression Test] A "POST" request for a service operation using WebGet
                new {
                    RequestUri = "/GetFirstTwoCustomers",
                    StatusCode = 405,
                    ErrorMessage = "Method Not Allowed",
                    HttpMethod = "POST",
                },
            };

            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                using (NorthwindDefaultTempDbService.SetupNorthwind())
                using (TestWebRequest request = TestWebRequest.CreateForInProcess())
                {
                    request.ServiceType = typeof(EFActionDataService);
                    request.HttpMethod = testCase.HttpMethod;
                    request.RequestUriString = testCase.RequestUri;
                    request.Accept = "application/atom+xml,application/xml";
                    request.RequestContentLength = 0;

                    Exception e = t.TestUtil.RunCatching(request.SendRequest);

                    Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode);

                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                    }

                    Assert.AreEqual(testCase.ErrorMessage, e.Message);
                }
            });
        }

        #region Open Type Actions Tests

        private static DSPServiceDefinition ModelWithActionsAndOpenType()
        {
            DSPMetadata metadata = new DSPMetadata("ModelWithActions", "AstoriaUnitTests.Tests.Actions");

            var entityType = metadata.AddEntityType("EntityType", null, null, false);
            var derivedOpenType = metadata.AddEntityType("DerivedOpenType", null, entityType, false);
            derivedOpenType.IsOpenType = true;
            var derivedEntityType = metadata.AddEntityType("DerivedEntityType", null, entityType, false);

            metadata.AddKeyProperty(entityType, "ID", typeof(int));
            metadata.AddPrimitiveProperty(entityType, "EntityTypeCollidingName", typeof(string));
            metadata.AddPrimitiveProperty(derivedEntityType, "DerivedEntityTypeCollidingName", typeof(int));
            metadata.AddPrimitiveProperty(derivedOpenType, "DerivedOpenTypeCollidingName", typeof(bool));

            var set = metadata.AddResourceSet("Set", entityType);
            metadata.SetReadOnly();

            DSPContext context = new DSPContext();
            DSPActionProvider actionProvider = new DSPActionProvider();

            actionProvider.AddAction("EntityTypeCollidingName", null, null, new ServiceActionParameter[] { new ServiceActionParameter("binding", entityType) }, null, EntityTypeCollidingNameMethodInfo);
            actionProvider.AddAction("DerivedEntityTypeCollidingName", null, null, new ServiceActionParameter[] { new ServiceActionParameter("binding", derivedEntityType) }, null, DerivedEntityTypeCollidingNameMethodInfo);
            actionProvider.AddAction("DerivedOpenTypeCollidingName", null, null, new ServiceActionParameter[] { new ServiceActionParameter("binding", derivedOpenType) }, null, DerivedOpenTypeCollidingNameMethodInfo);
            actionProvider.AddAction("CollidingOpenProperty", null, null, new ServiceActionParameter[] { new ServiceActionParameter("binding", derivedOpenType) }, null, CollidingOpenPropertyMethodInfo);

            DSPResource entity1 = new DSPResource(entityType);
            entity1.SetValue("ID", 1);
            entity1.SetValue("EntityTypeCollidingName", "Abcpqr");

            DSPResource entity2 = new DSPResource(derivedEntityType);
            entity2.SetValue("ID", 2);
            entity2.SetValue("DerivedEntityTypeCollidingName", 123);

            DSPResource entity3 = new DSPResource(derivedOpenType);
            entity3.SetValue("ID", 3);
            entity3.SetValue("DerivedOpenTypeCollidingName", true);
            entity3.SetValue("OpenProperty", "Open value 1");
            entity3.SetValue("CollidingOpenProperty", "Open value 2");

            context.GetResourceSetEntities("Set").Add(entity1);
            context.GetResourceSetEntities("Set").Add(entity2);
            context.GetResourceSetEntities("Set").Add(entity3);

            DSPServiceDefinition service = new DSPServiceDefinition()
            {
                Metadata = metadata,
                CreateDataSource = (m) => context,
                ForceVerboseErrors = true,
                MediaResourceStorage = new DSPMediaResourceStorage(),
                SupportNamedStream = true,
                Writable = true,
                ActionProvider = actionProvider,
                DataServiceBehavior = new OpenWebDataServiceDefinition.OpenWebDataServiceBehavior() { IncludeRelationshipLinksInResponse = true },
            };

            return service;
        }

        private static MethodInfo EntityTypeCollidingNameMethodInfo = typeof(ActionTests).GetMethod("EntityTypeCollidingName", BindingFlags.NonPublic | BindingFlags.Static);
        private static void EntityTypeCollidingName(DSPResource binding)
        {
            Assert.AreEqual(binding.GetValue("ID"), 1);
        }

        private static MethodInfo DerivedEntityTypeCollidingNameMethodInfo = typeof(ActionTests).GetMethod("DerivedEntityTypeCollidingName", BindingFlags.NonPublic | BindingFlags.Static);
        private static void DerivedEntityTypeCollidingName(DSPResource binding)
        {
            Assert.AreEqual(binding.GetValue("ID"), 2);
        }

        private static MethodInfo DerivedOpenTypeCollidingNameMethodInfo = typeof(ActionTests).GetMethod("DerivedOpenTypeCollidingName", BindingFlags.NonPublic | BindingFlags.Static);
        private static void DerivedOpenTypeCollidingName(DSPResource binding)
        {
            Assert.AreEqual(binding.GetValue("ID"), 3);
        }

        private static MethodInfo CollidingOpenPropertyMethodInfo = typeof(ActionTests).GetMethod("CollidingOpenProperty", BindingFlags.NonPublic | BindingFlags.Static);
        private static void CollidingOpenProperty(DSPResource binding)
        {
            Assert.AreEqual(binding.GetValue("ID"), 3);
        }
        [Ignore] // Remove Atom
        // [TestCategory("Partition1"), TestMethod, Variation("Service action and open type positive tests.")]
        public void ActionOpenTypeSelectTests()
        {
            var testCases = new[]
            {
                new {
                    RequestUri = "/Set?$select=EntityTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:EntityTypeCollidingName)=3)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.EntityTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='EntityTypeCollidingName' and contains(@target, 'AstoriaUnitTests.Tests.Actions.EntityTypeCollidingName')])=3)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=EntityTypeCollidingName,AstoriaUnitTests.Tests.Actions.EntityTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:EntityTypeCollidingName)=3)",
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='EntityTypeCollidingName' and contains(@target, 'AstoriaUnitTests.Tests.Actions.EntityTypeCollidingName')])=3)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/DerivedEntityTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:DerivedEntityTypeCollidingName)=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.DerivedEntityTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='DerivedEntityTypeCollidingName' and contains(@target, 'AstoriaUnitTests.Tests.Actions.DerivedEntityTypeCollidingName')])=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedEntityType/DerivedEntityTypeCollidingName,AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.DerivedEntityTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:DerivedEntityTypeCollidingName)=1)",
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='DerivedEntityTypeCollidingName' and contains(@target, 'AstoriaUnitTests.Tests.Actions.DerivedEntityTypeCollidingName')])=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedOpenType/DerivedOpenTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:DerivedOpenTypeCollidingName)=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='DerivedOpenTypeCollidingName' and contains(@target, 'AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName')])=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedOpenType/DerivedOpenTypeCollidingName,AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:DerivedOpenTypeCollidingName)=1)",
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='DerivedOpenTypeCollidingName' and contains(@target, 'AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName')])=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedOpenType/CollidingOpenProperty",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:CollidingOpenProperty)=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.CollidingOpenProperty",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='CollidingOpenProperty' and contains(@target, 'AstoriaUnitTests.Tests.Actions.CollidingOpenProperty')])=1)",
                    },
                    HttpMethod = "GET",
                },
                new {
                    RequestUri = "/Set?$select=AstoriaUnitTests.Tests.Actions.DerivedOpenType/CollidingOpenProperty,AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.CollidingOpenProperty",
                    StatusCode = 200,
                    XPaths = new string[]
                    {
                        "boolean(count(/atom:feed/atom:entry/atom:content/adsm:properties/ads:CollidingOpenProperty)=1)",
                        "boolean(count(/atom:feed/atom:entry/adsm:action[@title='CollidingOpenProperty' and contains(@target, 'AstoriaUnitTests.Tests.Actions.CollidingOpenProperty')])=1)",
                    },
                    HttpMethod = "GET",
                },
            };

            var service = ActionTests.ModelWithActionsAndOpenType();
            service.ForceVerboseErrors = true;
            t.TestUtil.RunCombinations(testCases, testCase =>
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = testCase.HttpMethod;
                    request.RequestUriString = testCase.RequestUri;
                    request.Accept = "application/atom+xml,application/xml";

                    Exception e = t.TestUtil.RunCatching(request.SendRequest);
                    UnitTestsUtil.VerifyXPathExpressionResults(request.GetResponseStreamAsXDocument(), true, testCase.XPaths);
                    Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode);
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Service action and open type positive tests.")]
        public void ActionOpenTypePositiveTest()
        {
            var testCases = new[]
            {
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.EntityTypeCollidingName",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(1)/AstoriaUnitTests.Tests.Actions.EntityTypeCollidingName",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.DerivedEntityTypeCollidingName",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.DerivedEntityType/AstoriaUnitTests.Tests.Actions.DerivedEntityTypeCollidingName",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.CollidingOpenProperty",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.CollidingOpenProperty",
                    StatusCode = 204,
                    ExpectedResponsePayload = string.Empty,
                    HttpMethod = "POST",
                },
            };

            var service = ActionTests.ModelWithActionsAndOpenType();
            service.ForceVerboseErrors = true;

            t.TestUtil.RunCombinations(testCases, testCase =>
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = testCase.HttpMethod;
                    request.RequestUriString = testCase.RequestUri;

                    Exception e = t.TestUtil.RunCatching(request.SendRequest);
                    Assert.AreEqual(testCase.ExpectedResponsePayload, request.GetResponseStreamAsText());
                    Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode);
                }
            });
        }

        [TestCategory("Partition1"), TestMethod, Variation("Service action and open type negative tests.")]
        public void ActionOpenTypeNegativeTest()
        {
            var testCases = new[]
            {
                new {
                    RequestUri = "/Set(1)/EntityTypeCollidingName",
                    RequestPayload = default(string),
                    ErrorMsg = "The URI 'http://host/Set(1)/EntityTypeCollidingName' is not valid for POST operation. For POST operations, the URI must refer to a service operation or an entity set.",
                    StatusCode = 405,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(2)/AstoriaUnitTests.Tests.Actions.DerivedEntityType/DerivedEntityTypeCollidingName",
                    RequestPayload = default(string),
                    ErrorMsg = "The URI 'http://host/Set(2)/AstoriaUnitTests.Tests.Actions.DerivedEntityType/DerivedEntityTypeCollidingName' is not valid for POST operation. For POST operations, the URI must refer to a service operation or an entity set.",
                    StatusCode = 405,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/DerivedOpenTypeCollidingName",
                    RequestPayload = default(string),
                    ErrorMsg = "The URI 'http://host/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/DerivedOpenTypeCollidingName' is not valid for POST operation. For POST operations, the URI must refer to a service operation or an entity set.",
                    StatusCode = 405,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/CollidingOpenProperty",
                    RequestPayload = default(string),
                    ErrorMsg = "Open navigation properties are not supported on OpenTypes. Property name: 'CollidingOpenProperty'.",
                    StatusCode = 400,
                    HttpMethod = "POST",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.DerivedOpenTypeCollidingName",
                    RequestPayload = default(string),
                    ErrorMsg = "Method Not Allowed",
                    StatusCode = 405,
                    HttpMethod = "PUT",
                },
                new {
                    RequestUri = "/Set(3)/AstoriaUnitTests.Tests.Actions.DerivedOpenType/AstoriaUnitTests.Tests.Actions.CollidingOpenProperty",
                    RequestPayload = default(string),
                    ErrorMsg = "Method Not Allowed",
                    StatusCode = 405,
                    HttpMethod = "PUT",
                },
            };

            var service = ActionTests.ModelWithActionsAndOpenType();
            service.ForceVerboseErrors = true;
            t.TestUtil.RunCombinations(testCases, (testCase) =>
            {
                using (TestWebRequest request = service.CreateForInProcess())
                {
                    request.HttpMethod = testCase.HttpMethod;
                    request.RequestUriString = testCase.RequestUri;
                    request.RequestContentType = UnitTestsUtil.JsonLightMimeType;
                    request.SetRequestStreamAsText(testCase.RequestPayload ?? string.Empty);

                    Exception e = t.TestUtil.RunCatching(request.SendRequest);
                    while (e.InnerException != null)
                    {
                        e = e.InnerException;
                    }

                    Assert.AreEqual(testCase.ErrorMsg, e.Message);
                    Assert.AreEqual(testCase.StatusCode, request.ResponseStatusCode);
                }
            });
        }

        #endregion Open Type Actions Tests
    }
}
