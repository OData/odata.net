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
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataResourceSet object model class.
    /// </summary>
    [TestClass, TestCase]
    public class ODataFeedTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of a resourceCollection.")]
        public void DefaultValuesTest()
        {
            ODataResourceSet resourceCollection = new ODataResourceSet();
            this.Assert.IsNull(resourceCollection.Count, "Expected null default value for property 'Count'.");
            this.Assert.IsNull(resourceCollection.NextPageLink, "Expected null default value for property 'NextPageLink'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a resourceCollection.")]
        public void PropertyGettersAndSettersTest()
        {
            int count = -2;
            Uri nextPageLink = new Uri("http://odatatest.org/page?id=2");

            ODataResourceSet resourceCollection = new ODataResourceSet()
            {
                Count = count,
                NextPageLink = nextPageLink
            };


            this.Assert.AreEqual(count, resourceCollection.Count, EqualityComparer<long?>.Default, "Expected equal Count values.");
            this.Assert.AreSame(nextPageLink, resourceCollection.NextPageLink, "Expected reference equal values for property 'NextPageLink'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a resourceCollection.")]
        public void PropertySettersNullTest()
        {
            ODataResourceSet resourceCollection = new ODataResourceSet();
            resourceCollection.Count = null;
            resourceCollection.NextPageLink = null;

            this.Assert.IsNull(resourceCollection.Count, "Expected null value for property 'Count'.");
            this.Assert.IsNull(resourceCollection.NextPageLink, "Expected null value for property 'NextPageLink'.");
        }
    }
}
