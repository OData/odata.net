﻿//---------------------------------------------------------------------
// <copyright file="RequestInfoTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Client
{
    using System;
    using Microsoft.OData.Client;
    using FluentAssertions;
    using Microsoft.OData.Edm;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Collections.Generic;

    /// <summary>
    /// TODO: test the rest of RequestInfo
    /// </summary>
    [TestClass]
    public class RequestInfoTests
    {
        private ClientEdmModel clientEdmModel;
        private EntityDescriptor entityDescriptor;
        private DataServiceContext ctx;
        private RequestInfo testSubject;
        private EdmEntitySet serverEntitySet;
        private string serverTypeName;

        [TestInitialize]
        public void Init()
        {
            this.clientEdmModel = new ClientEdmModel(ODataProtocolVersion.V4);
            this.entityDescriptor = new EntityDescriptor(this.clientEdmModel) { Entity = new Customer() };

            var serverType = new EdmEntityType("FQ.NS", "MyServerType");
            serverType.AddUnidirectionalNavigation(new EdmNavigationPropertyInfo { Name = "Navigation", Target = serverType, TargetMultiplicity = EdmMultiplicity.ZeroOrOne });
            this.serverTypeName = ((IEdmSchemaElement)serverType).FullName();
            var serverContainer = new EdmEntityContainer("FQ.NS", "MyContainer");
            this.serverEntitySet = serverContainer.AddEntitySet("MyEntitySet", serverType);

            var serverModel = new EdmModel();
            serverModel.AddElement(serverType);
            serverModel.AddElement(serverContainer);

            this.ctx = new DataServiceContext(new Uri("http://temp.org/"), ODataProtocolVersion.V4, this.clientEdmModel);
            this.ctx.Format.UseJson(serverModel);
            this.testSubject = new RequestInfo(ctx);
        }

        [TestMethod]
        public void RequestInfoShouldFindServerTypeNameFromSet()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.EntitySetName = this.serverEntitySet.Name;
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(this.serverTypeName);
        }

        [TestMethod]
        public void RequestInfoShouldNotBeAbleToInferServerTypeNameIfNoSetOrParentIsPresent()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.EntitySetName = null;
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(null);
        }

        [TestMethod]
        public void RequestInfoShouldNotBeAbleToInferServerTypeNameIfSetDoesNotExist()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.EntitySetName = "FakeSet";
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(null);
        }

        [TestMethod]
        public void RequestInfoShouldBeAbleToInferServerTypeNameFromContainingNavigation()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.ParentForInsert = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = this.serverTypeName, Entity = new Customer() };
            this.entityDescriptor.ParentPropertyForInsert = "Navigation";
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(this.serverTypeName);
        }

        [TestMethod]
        public void RequestInfoShouldNotBeAbleToInferServerTypeNameFromContainingNavigationIfParentHasNoType()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.ParentForInsert = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = null, Entity = new Customer() };
            this.entityDescriptor.ParentPropertyForInsert = "Navigation";
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(null);
        }

        [TestMethod]
        public void RequestInfoShouldNotBeAbleToInferServerTypeNameFromContainingNavigationIfParentTypeIsntFound()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.ParentForInsert = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = "FakeType", Entity = new Customer() };
            this.entityDescriptor.ParentPropertyForInsert = "FakeNavigation";
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(null);
        }

        [TestMethod]
        public void RequestInfoShouldNotBeAbleToInferServerTypeNameFromContainingNavigationIfPropertyDoesNotExist()
        {
            this.entityDescriptor.ServerTypeName.Should().BeNull();
            this.entityDescriptor.ParentForInsert = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = this.serverTypeName, Entity = new Customer() };
            this.entityDescriptor.ParentPropertyForInsert = "FakeNavigation";
            testSubject.InferServerTypeNameFromServerModel(this.entityDescriptor).Should().Be(null);
        }

        [TestMethod]
        public void RequestInfoShouldCreateTunneledDeleteRequestMessageDeleteMethodAndDeleteInHttpXMethodHeader()
        {
            bool previousPostTunnelingValue = ctx.UsePostTunneling;
            ctx.UsePostTunneling = true;
            HeaderCollection headersCollection = new HeaderCollection();
            var descriptor = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = this.serverTypeName, Entity = new Customer() };
            var buildingRequestArgs = new BuildingRequestEventArgs("DELETE", new Uri("http://localhost/fakeService.svc/"), headersCollection, descriptor, HttpStack.Auto);

            var requestMessage = (HttpWebRequestMessage)testSubject.CreateRequestMessage(buildingRequestArgs);

            requestMessage.GetHeader(XmlConstants.HttpXMethod).Should().Be("DELETE");
            requestMessage.GetHeader(XmlConstants.HttpContentLength).Should().Be("0");
            requestMessage.GetHeader(XmlConstants.HttpContentType).Should().BeNull();
            requestMessage.Method.Should().Be("DELETE");
            requestMessage.HttpWebRequest.Method.Should().Be("POST");
            
            // undoing change so this is applicable only for this test.
            ctx.UsePostTunneling = previousPostTunnelingValue;
        }

        [TestMethod]
        public void RequestInfoShouldCreateTunneledPatchRequestMessagePostMethodAndPatchInHttpXMethodHeader()
        {
            bool previousPostTunnelingValue = ctx.UsePostTunneling;
            ctx.UsePostTunneling = true;
            HeaderCollection headersCollection = new HeaderCollection();
            var descriptor = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = this.serverTypeName, Entity = new Customer() };
            var buildingRequestArgs = new BuildingRequestEventArgs("PATCH", new Uri("http://localhost/fakeService.svc/"), headersCollection, descriptor, HttpStack.Auto);

            var requestMessage = (HttpWebRequestMessage)testSubject.CreateRequestMessage(buildingRequestArgs);

            requestMessage.GetHeader(XmlConstants.HttpXMethod).Should().Be("PATCH");
            requestMessage.Method.Should().Be("PATCH");
            requestMessage.HttpWebRequest.Method.Should().Be("POST");

            // undoing change so this is applicable only for this test.
            ctx.UsePostTunneling = previousPostTunnelingValue;
        }

        [TestMethod]
        public void PostTunnelingDeleteRequestShouldNotHaveContentTypeHeader()
        {
            bool previousPostTunnelingValue = ctx.UsePostTunneling;
            ctx.UsePostTunneling = true;
            HeaderCollection headersCollection = new HeaderCollection();
            var descriptor = new EntityDescriptor(this.clientEdmModel) { ServerTypeName = this.serverTypeName, Entity = new Customer() };
            var buildingRequestArgs = new BuildingRequestEventArgs("DELETE", new Uri("http://localhost/fakeService.svc/"), headersCollection, descriptor, HttpStack.Auto);

            ctx.Configurations.RequestPipeline.OnMessageCreating = (args) =>
            {
                buildingRequestArgs.Headers.Keys.Should().NotContain(XmlConstants.HttpContentType);
                return new HttpWebRequestMessage(args);
            };

            testSubject.CreateRequestMessage(buildingRequestArgs);

            // undoing change so this is applicable only for this test.
            ctx.UsePostTunneling = previousPostTunnelingValue;
            ctx.Configurations.RequestPipeline.OnMessageCreating = null;
        }
    }
}