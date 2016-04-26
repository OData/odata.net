//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinkTests.cs" company="Microsoft">
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
    /// Tests for the ODataEntityReferenceLink object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataEntityReferenceLinkTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of an entity reference link.")]
        public void DefaultValuesTest()
        {
            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink();
            this.Assert.IsNull(entityReferenceLink.Url, "Expected null default value for property 'Url'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of an entity reference link.")]
        public void PropertyGettersAndSettersTest()
        {
            Uri link = new Uri("http://odatalib.org/entityreferencelink");

            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink()
            {
                Url = link,
            };

            this.Assert.AreSame(link, entityReferenceLink.Url, "Expected reference equal values for property 'Url'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of an entity reference link when setting null values.")]
        public void PropertySettersNullTest()
        {
            ODataEntityReferenceLink entityReferenceLink = new ODataEntityReferenceLink()
            {
                Url = null,
            };

            this.Assert.IsNull(entityReferenceLink.Url, "Expected null value for property 'Url'.");
        }
    }
}
