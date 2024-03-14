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

    public class SearchQueryTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>, IDisposable
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
                    Assert.Empty(details);
                }

                //Implicit AND
                details = this.TestsHelper.QueryFeed("ProductDetails?$search=snack sweet", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Single(details);
                    Assert.Equal("sweet snack", Assert.IsType<ODataProperty>(details.First().Properties.Single(p => p.Name == "Description")).Value);
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
                        Assert.Single(detail.Properties);
                        var description = Assert.IsType<string>(Assert.IsType<ODataProperty>(detail.Properties.Single(p => p.Name == "Description")).Value);
                        Assert.Contains("drink", description);
                    }
                }

                List<ODataResource> entries = this.TestsHelper.QueryFeed("ProductDetails?$search=suger OR sweet&$orderby=ProductName&$expand=Reviews", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.Equal(7, entries.Count);
                    var productDetails = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.Equal("Candy", Assert.IsType<ODataProperty>(productDetails.First().Properties.Single(p=>p.Name == "ProductName")).Value);
                    var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
                    Assert.Equal(4, reviews.Count);
                }
            }
        }

        #endregion

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
