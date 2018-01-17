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
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class JsonWithoutTypeResolverTests : EndToEndTestBase
    {
        public JsonWithoutTypeResolverTests()
            : base(ServiceDescriptors.AstoriaDefaultService)
        {
        }

        [TestMethod]
        public void DerivedTypeFeedQuery()
        {
            var contextWrapper = this.CreateContext();

            var baseQueryResults = contextWrapper.Execute<PageView>(new Uri(this.ServiceUri.OriginalString + "/PageView/Microsoft.Test.OData.Services.AstoriaDefaultService.ProductPageView")).ToArray();

            var derivedQueryResults = contextWrapper.Execute<DiscontinuedProduct>(new Uri(this.ServiceUri.OriginalString + "Product/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct")).ToArray();
        }

        [TestMethod]
        public void ProjectionEntryQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/Customer(-9)?$select=Name,PrimaryContactInfo")).ToArray();
        }

        [TestMethod]
        public void ExpandEntryQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/Customer(-9)?$expand=Info")).ToArray();
        }

        [TestMethod]
        public void DerivedTypeExpandWithProjectionFeedQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryUri = new Uri(this.ServiceUri.OriginalString + "Product/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct?$expand=RelatedProducts($select=*),Detail($select=*),Reviews($select=*),Photos($select=*)");
            var queryResults = contextWrapper.Execute<DiscontinuedProduct>(queryUri).ToArray();
        }

        [TestMethod]
        public void BasePropertyQueryWithinDerivedType()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<int>(new Uri(this.ServiceUri.OriginalString + "/Product(-9)/Microsoft.Test.OData.Services.AstoriaDefaultService.DiscontinuedProduct/ProductId")).ToArray();
        }

        [TestMethod]
        public void ComplexPropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<ContactDetails>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo")).ToArray();
        }

        [TestMethod]
        public void NestedComplexPropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults1 = contextWrapper.Execute<Phone>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
            var queryResults2 = contextWrapper.Execute<Aliases>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/ContactAlias")).ToArray();
            var queryResults3 = contextWrapper.Execute<ICollection<string>>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
            var queryResults4 = contextWrapper.Execute<ICollection<string>>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/Microsoft.Test.OData.Services.AstoriaDefaultService.Customer/PrimaryContactInfo/ContactAlias/AlternativeNames")).ToArray();
        }

        [TestMethod]
        public void CollectionOfComplexPropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<Phone>(new Uri(this.ServiceUri.OriginalString + "/Customer(-10)/PrimaryContactInfo/MobilePhoneBag")).ToArray();
        }

        [TestMethod]
        public void CollectionOfPrimitivePropertyQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResults = contextWrapper.Execute<ICollection<decimal>>(new Uri(this.ServiceUri.OriginalString + "/MappedEntityType(-10)/BagOfDecimals")).ToArray();
        }

        [TestMethod]
        public void ServiceOperationFeedQuery()
        {
            var contextWrapper = this.CreateContext();
            var queryResult = contextWrapper.Execute<Customer>(new Uri(this.ServiceUri.OriginalString + "/GetSpecificCustomer?Name='enumeratetrademarkexecutionbrfalsenesteddupoverflowspacebarseekietfbeforeobservedstart'"), "GET", true).ToArray();
            Assert.AreEqual(1, queryResult.Count(), "Expected a single Customer return");
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
