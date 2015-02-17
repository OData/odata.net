//---------------------------------------------------------------------
// <copyright file="ODataFeedTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataFeed object model class.
    /// </summary>
    [TestClass, TestCase]
    public class ODataFeedTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of a feed.")]
        public void DefaultValuesTest()
        {
            ODataFeed feed = new ODataFeed();
            this.Assert.IsNull(feed.Count, "Expected null default value for property 'Count'.");
            this.Assert.IsNull(feed.NextPageLink, "Expected null default value for property 'NextPageLink'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a feed.")]
        public void PropertyGettersAndSettersTest()
        {
            int count = -2;
            Uri nextPageLink = new Uri("http://odatatest.org/page?id=2");

            ODataFeed feed = new ODataFeed()
            {
                Count = count,
                NextPageLink = nextPageLink
            };


            this.Assert.AreEqual(count, feed.Count, EqualityComparer<long?>.Default, "Expected equal Count values.");
            this.Assert.AreSame(nextPageLink, feed.NextPageLink, "Expected reference equal values for property 'NextPageLink'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a feed.")]
        public void PropertySettersNullTest()
        {
            ODataFeed feed = new ODataFeed();
            feed.Count = null;
            feed.NextPageLink = null;

            this.Assert.IsNull(feed.Count, "Expected null value for property 'Count'.");
            this.Assert.IsNull(feed.NextPageLink, "Expected null value for property 'NextPageLink'.");
        }
    }
}
