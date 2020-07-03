//---------------------------------------------------------------------
// <copyright file="JsonWithoutTypeResolverTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.ClientWithoutTypeResolverTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.OData.Framework;
    using Microsoft.Test.OData.Framework.Client;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Services.TestServices.AstoriaDefaultServiceReference;
    using Xunit;
    using Xunit.Abstractions;

    public class JsonWithoutTypeResolverTests : EndToEndTestBase
    {
        public JsonWithoutTypeResolverTests(ITestOutputHelper helper)
            : base(ServiceDescriptors.AstoriaDefaultService, helper)
        {
        }

        [Fact]
        public void DerivedTypeFeedQuery()
        {
            var contextWrapper = this.CreateContext();

            var baseQueryResults = contextWrapper.Execute<PageView>(new Uri(this.ServiceUri.OriginalString + "/PageView/Microsoft.Test.OData.Services.AstoriaDefaultService.ProductPageView")).ToArray();

            var derivedQueryResults = contextWrapper.Execute<DiscontinuedProduct>(new Uri(this.ServiceUri.OriginalString + "Product/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct")).ToArray();
        }

        [Fact]
        public void ProjectionEntryQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/Customer(-9)?$select=Name,PrimaryContactInfo")).ToArray();
        }

        [Fact]
        public void ExpandEntryQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/Customer(-9)?$expand=Info")).ToArray();
        }

        [Fact]
        public void ExpandEntryQueryWithNestedSelect()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/Customer(-9)?$expand=Info($select=Information)")).ToArray();
        }

        [Fact]
        public void DerivedTypeExpandWithProjectionFeedQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryUri = new Uri(this.ServiceUri.OriginalString + "Product/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct?$expand=RelatedProducts($select=*),Detail($select=*),Reviews($select=*),Photos($select=*)");
            var queryResults = contextWrapper.Execute<DiscontinuedProduct>(queryUri).ToArray();
        }

        [Fact]
        public void BasePropertyQueryWithinDerivedType()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<int>(new Uri(this.ServiceUri.OriginalString + "/Product(-9)/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct/ProductId")).ToArray();
        }

        [Fact]
        public void ComplexPropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<ContactDetails>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo")).ToArray();
        }

        [Fact]
        public void NestedComplexPropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults1 = contextWrapper.Execute<Phone>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
            var queryResults2 = contextWrapper.Execute<Aliases>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/ContactAlias")).ToArray();
            var queryResults3 = contextWrapper.Execute<ICollection<string>>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
            var queryResults4 = contextWrapper.Execute<ICollection<string>>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/Microsoft.Test.OData.Services.AstoriaDefaultService.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
        }

        [Fact]
        public void CollectionOfComplexPropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Phone>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
        }

        [Fact]
        public void CollectionOfPrimitivePropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<ICollection<decimal>>(new Uri(this.ServiceUri.OriginalString + "/MappedEntityType(-10)/BagOfDecimals")).ToArray();
        }

        [Fact]
        public void ServiceOperationFeedQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResult = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/GetSpecificCustomer?Name='enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart'"), "GET", true).ToArray();
            Assert.Equal(1, queryResult.Count());
        }

        private DataServiceContextWrapper<DefaultContainer> CreateContext()
        {
            var context = this.CreateWrappedContext<DefaultContainer>();
            context.Format.UseJson();
            context.ResolveType = null;
            context.ResolveName = null;

            return context;
        }
    }
}
