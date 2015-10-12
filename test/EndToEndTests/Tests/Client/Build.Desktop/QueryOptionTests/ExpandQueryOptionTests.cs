﻿//---------------------------------------------------------------------
//  <copyright file="ExpandQueryOptionTests.cs" company="Microsoft">
//       Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
//  </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.QueryOptionTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Core;
    using Microsoft.Test.OData.Services.TestServices;
    using Microsoft.Test.OData.Tests.Client.Common;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpandQueryOptionTests : ODataWCFServiceTestsBase<Microsoft.Test.OData.Services.TestServices.ODataWCFServiceReference.InMemoryEntities>
    {
        public ExpandQueryOptionTests()
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

        #region Test Method

        [TestMethod]
        public void ExpandBasicQueryOptionTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                // $top
                List<ODataEntry> entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($top=3)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(3, details.Count);
                }

                // $skip
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($skip=2)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(3, details.Count);
                }

                // $orderby
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($orderby=Description desc)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual("suger soft drink", details.First().Properties.Single(p => p.Name == "Description").Value);
                }

                // $filter
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($filter=Description eq 'spicy snack')", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(5, details.First().Properties.Single(p => p.Name == "ProductDetailID").Value);
                }

                // $count
                var feed = this.TestsHelper.QueryInnerFeed("Products(5)?$expand=Details($count=true)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata) && !mimeType.Contains(MimeTypes.ApplicationAtomXml))
                {
                    Assert.AreEqual(5, feed.Count);
                }

                // $search
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($search=(spicy OR suger) AND NOT \"0\" )", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(2, details.Count);
                }

                // with $ref option
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details/$ref", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(5, details.Count);
                    entries.First().Id.ToString().Contains("ProductDetailID=2");
                }

                // Nested option on $ref
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details/$ref($orderby=Description desc)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(5, details.Count);
                    entries.First().Id.ToString().Contains("ProductDetailID=3");
                }
            }
        }

        [TestMethod]
        public void ExpandMiscQueryOptionsTest()
        {
            foreach (var mimeType in mimeTypes)
            {
                List<ODataEntry> entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($orderby=Description;$skip=2;$top=1;$select=Description)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    var details = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductDetails"));
                    Assert.AreEqual(1, details.Count);
                    Assert.AreEqual(1, details.First().Properties.Count());
                    Assert.AreEqual("fitness drink!", details.First().Properties.Single(p => p.Name == "Description").Value);
                }

                // Nested $expand
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($expand=Reviews($filter=contains(Comment,'good');$select=Comment))", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(8, entries.Count);
                    var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
                    Assert.AreEqual(2, reviews.Count);
                    Assert.AreEqual(1, reviews.First().Properties.Count());
                    Assert.AreEqual("Not so good as other brands", reviews.First().Properties.Single(p => p.Name == "Comment").Value);
                }

                // Nested $expand with $ref
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($expand=Reviews/$ref)", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(9, entries.Count);
                    var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
                    Assert.AreEqual(3, reviews.Count);
                }

                // Nested $search
                entries = this.TestsHelper.QueryEntries("Products(5)?$expand=Details($expand=Reviews($filter=contains(Comment, 'good'));$search=snack \"Cheese-flavored\")", mimeType);
                if (!mimeType.Contains(MimeTypes.ODataParameterNoMetadata))
                {
                    Assert.AreEqual(4, entries.Count);
                    var reviews = entries.FindAll(e => e.Id.AbsoluteUri.Contains("ProductReviews"));
                    Assert.AreEqual(2, reviews.Count);
                    Assert.AreEqual("Not so good as other brands", reviews.First().Properties.Single(p => p.Name == "Comment").Value);
                }
            }
        }

        #endregion

        
    }
}
