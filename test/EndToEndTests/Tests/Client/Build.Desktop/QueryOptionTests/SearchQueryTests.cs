//---------------------------------------------------------------------
// <copyright file="SearchQueryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.QueryOptionTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.OData;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Xunit;

    public class SearchQueryTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        public SearchQueryTests()
            : base(ServiceDescriptors.ODataWCFServiceDescriptor)
        {
        }

        private QueryOptionTestsHelper TestsHelper
        {
            get
            {
                return new QueryOptionTestsHelper(ServiceBaseUri, Model);
            }
        }

        #region test method
        [Fact]
        public void SearchTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataResource> details = this.TestsHelper.QueryFeed("ProductDetails?$search=(drink OR snack) AND (suger OR sweet) AND NOT \"0\"", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(2, details.Count);
                }

                details = this.TestsHelper.QueryFeed("ProductDetails?$search=NOT (drink OR snack)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(0, details.Count);
                }

                //Implicit AND
                details = this.TestsHelper.QueryFeed("ProductDetails?$search=snack sweet", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(1, details.Count);
                    Assert.Equal("sweet snack", details.First().Properties.Single(p => p.Name == "Description").Value);
                }

                details = this.TestsHelper.QueryFeed("ProductDetails?$search=snack NOT sweet", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(2, details.Count);
                }

                //Priority of AND NOT OR
                details = this.TestsHelper.QueryFeed("ProductDetails?$search=snack OR drink AND soft AND NOT \"0\"", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(4, details.Count);
                }
            }
        }

        [Fact]
        public void SearchCombinedWithQueryOptionTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataResource> details = this.TestsHelper.QueryFeed("ProductDetails?$filter=contains(Description,'drink')&$search=suger OR spicy NOT \"0\"&$select=Description", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(2, details.Count);
                    foreach (var detail in details)
                    {
                        Assert.Equal(1, detail.Properties.Count());
                        var description = detail.Properties.Single(p => p.Name == "Description").Value as string;
                        Assert.True(description.Contains("drink"));
                    }
                }

                List<ODataResource> entries = this.TestsHelper.QueryFeed("ProductDetails?$search=suger OR sweet&$orderby=ProductName&$expand=Reviews", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(7, entries.Count);
                    var productDetails = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.Equal("Candy", productDetails.First().Properties.Single(p=>p.Name == "ProductName").Value);
                    var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
                    Assert.Equal(4, reviews.Count);
                }
            }
        }

        #endregion
    }
}
