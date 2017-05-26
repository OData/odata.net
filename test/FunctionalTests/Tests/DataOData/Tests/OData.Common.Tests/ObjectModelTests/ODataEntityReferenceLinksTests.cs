//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinksTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataEntityReferenceLinks object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataEntityReferenceLinksTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of an entity reference links instance.")]
        public void DefaultValuesTest()
        {
            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks();
            this.Assert.IsNull(entityReferenceLinks.Count, "Expected null default value for property 'Count'.");
            this.Assert.IsNull(entityReferenceLinks.NextPageLink, "Expected null default value for property 'NextPageLink'.");
            this.Assert.IsNull(entityReferenceLinks.Links, "Expected null default value for property 'Links'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of an entity reference links instance.")]
        public void PropertyGettersAndSettersTest()
        {
            ODataEntityReferenceLink link1 = new ODataEntityReferenceLink { Url = new Uri("http://odatalib.org/entityreferencelink1") };
            ODataEntityReferenceLink link2 = new ODataEntityReferenceLink { Url = new Uri("http://odatalib.org/entityreferencelink2") };
            ODataEntityReferenceLink link3 = new ODataEntityReferenceLink { Url = new Uri("http://odatalib.org/entityreferencelink3") };
            ODataEntityReferenceLink[] links = new ODataEntityReferenceLink[] { link1, link2, link3 };

            int inlineCount = 3;

            Uri nextLink = new Uri("http://odatalib.org/nextlink");

            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks()
            {
                Count = inlineCount,
                NextPageLink = nextLink,
                Links = links,
            };

            this.Assert.AreEqual(inlineCount, entityReferenceLinks.Count, "Expected equal values for property 'Count'.");
            this.Assert.AreEqual(nextLink, entityReferenceLinks.NextPageLink, "Expected reference equal values for property 'NextPageLink'.");
            VerificationUtils.VerifyEnumerationsAreEqual(
                links, 
                entityReferenceLinks.Links,
                (first, second, assert) => assert.AreSame(first, second, "Expected reference equal values for entity reference links."),
                (link) => link.Url.OriginalString,
                this.Assert);
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of an entity reference links instance when setting null values.")]
        public void PropertySettersNullTest()
        {
            ODataEntityReferenceLinks entityReferenceLinks = new ODataEntityReferenceLinks()
            {
                Count = null,
                NextPageLink = null,
                Links = null,
            };

            this.Assert.IsNull(entityReferenceLinks.Count, "Expected null default value for property 'Count'.");
            this.Assert.IsNull(entityReferenceLinks.NextPageLink, "Expected null default value for property 'NextPageLink'.");
            this.Assert.IsNull(entityReferenceLinks.Links, "Expected null default value for property 'Links'.");
        }
    }
}
