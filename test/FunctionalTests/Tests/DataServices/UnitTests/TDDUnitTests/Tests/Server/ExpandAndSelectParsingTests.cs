//---------------------------------------------------------------------
// <copyright file="ExpandAndSelectParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using AstoriaUnitTests.TDD.Tests.Server.Simulators;
using AstoriaUnitTests.Tests.Server.Simulators;
using FluentAssertions;
using Microsoft.OData.UriParser;
using Microsoft.OData.Service;
using Microsoft.OData.Service.Caching;
using Microsoft.OData.Service.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ErrorStrings = Microsoft.OData.Service.Strings;

namespace AstoriaUnitTests.TDD.Tests.Server
{
    [TestClass]
    public class ExpandAndSelectParsingTests
    {
        private DataServiceSimulator service;
        private DataServiceHostSimulator host;
        private readonly RequestDescription requestDescription;

        public ExpandAndSelectParsingTests()
        {
            this.requestDescription = new RequestDescription(Microsoft.OData.Service.RequestTargetKind.Resource, RequestTargetSource.EntitySet, new Uri("http://fake.org/"));
            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "Fake", "Type", false) { CanReflectOnInstanceType = false, IsOpenType = true };
            resourceType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            var resourceSet = new ResourceSet("FakeSet", resourceType);
            resourceSet.SetReadOnly();

            this.requestDescription.LastSegmentInfo.TargetResourceType = resourceType;
            this.requestDescription.LastSegmentInfo.TargetResourceSet = ResourceSetWrapper.CreateForTests(resourceSet);
        }

        private void InitWithQueryOptions(string selectQueryOption, string expandQueryOption)
        {
            string queryOption = string.Empty;
            if (selectQueryOption != null && expandQueryOption != null)
            {
                queryOption = "?$select=" + selectQueryOption + "&$expand=" + expandQueryOption;
            }
            else if (selectQueryOption != null)
            {
                queryOption = "?$select=" + selectQueryOption;
            }
            else
            {
                queryOption = "?$expand=" + expandQueryOption;
            }

            this.host = new DataServiceHostSimulator
            {
                AbsoluteRequestUri = new Uri("http://fake.org/FakeSet" + queryOption),
                AbsoluteServiceUri = new Uri("http://fake.org/"),
                RequestHttpMethod = "GET",
                RequestVersion = "2.0",
            };

            if (selectQueryOption != null)
            {
                this.host.SetQueryStringItem("$select", selectQueryOption);
            }

            if (expandQueryOption != null)
            {
                this.host.SetQueryStringItem("$expand", expandQueryOption);
            }

            DataServiceProviderSimulator provider = new DataServiceProviderSimulator
            {
                ContainerName = "SelectTestContainer",
                ContainerNamespace = "SelectTestNamespace"
            };

            var resourceType = new ResourceType(typeof(object), ResourceTypeKind.EntityType, null, "SelectTestNamespace", "Fake", false) { CanReflectOnInstanceType = false, IsOpenType = true };
            resourceType.AddProperty(new ResourceProperty("Id", ResourcePropertyKind.Key | ResourcePropertyKind.Primitive, ResourceType.GetPrimitiveResourceType(typeof(int))) { CanReflectOnInstanceTypeProperty = false });
            var resourceSet = new ResourceSet("FakeSet", resourceType);
            resourceSet.SetReadOnly();

            provider.AddResourceSet(resourceSet);

            DataServiceConfiguration configuration = new DataServiceConfiguration(provider);
            configuration.SetEntitySetAccessRule("*", EntitySetRights.All);
            this.service = new DataServiceSimulator
            {
                OperationContext = new DataServiceOperationContext(this.host),
                Configuration = configuration,
            };

            DataServiceStaticConfiguration staticConfiguration = new DataServiceStaticConfiguration(this.service.Instance.GetType(), provider);
            IDataServiceProviderBehavior providerBehavior = DataServiceProviderBehavior.CustomDataServiceProviderBehavior;

            this.service.Provider = new DataServiceProviderWrapper(
                new DataServiceCacheItem(
                    configuration,
                    staticConfiguration),
                provider,
                provider,
                this.service,
                false);

            this.service.ProcessingPipeline = new DataServiceProcessingPipeline();
            this.service.Provider.ProviderBehavior = providerBehavior;
            this.service.ActionProvider = DataServiceActionProviderWrapper.Create(this.service);
            this.service.OperationContext.InitializeAndCacheHeaders(service);
        }

        [TestMethod]
        public void SelectParsingWithNull()
        {
            this.AssertNothingSelected(null);
        }

        [TestMethod]
        public void SelectParsingWithEmpty()
        {
            this.AssertNothingSelected(string.Empty);
        }

        [TestMethod]
        public void SelectParsingWithWhitespace()
        {
            this.AssertNothingSelected("   ");
        }

        [TestMethod]
        public void SelectParsingWithSimplePropertyName()
        {
            this.ParseOnePropertyPath<PathSelectItem>("Id");
        }

        [TestMethod]
        public void SelectParsingWithOpenPropertyName()
        {
            this.ParseOnePropertyPath<PathSelectItem>("foo");
        }

        [TestMethod]
        public void SelectParsingWithWildcard()
        {
            this.ParseOnePropertyPath<WildcardSelectItem>("*");
        }

        [TestMethod]
        public void SelectParsingWithFullyQualifiedWildcard()
        {
            this.ParseOnePropertyPath<NamespaceQualifiedWildcardSelectItem>("SelectTestNamespace.*");
        }

        [TestMethod]
        public void SelectParsingShouldFailIfConfiguredToBlockProjection()
        {
            this.InitWithQueryOptions("foo", "");
            this.service.Configuration.DataServiceBehavior.AcceptProjectionRequests = false;
            Action parse = () => new ExpandAndSelectParseResult(this.requestDescription, this.service);
            parse.ShouldThrow<DataServiceException>().WithMessage(ErrorStrings.DataServiceConfiguration_ProjectionsNotAccepted);
        }

        private ExpandAndSelectParseResult ParseExpandAndSelect(string selectValue = null, string expandValue = null)
        {
            this.InitWithQueryOptions(selectValue, expandValue);
            return new ExpandAndSelectParseResult(this.requestDescription, this.service);
        }

        private void ParseOnePropertyPath<TSelection>(string segment) where TSelection : SelectItem
        {
            var testSubject = this.ParseExpandAndSelect(segment);
            var paths = testSubject.Clause.SelectedItems;
            paths.Should().HaveCount(1).And.OnlyContain(c => c is TSelection);
        }

        private void AssertNothingSelected(string value)
        {
            var testSubject = this.ParseExpandAndSelect(value);
            testSubject.HasSelect.Should().BeFalse();
        }
    }
}