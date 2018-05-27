//---------------------------------------------------------------------
// <copyright file="ODataEntitySetInfoTests.cs" company="Microsoft">
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
    /// Tests for the ODataEntitySetInfo object model class.
    /// </summary>
    [TestClass, TestCase]
    public class ODataEntitySetInfoTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of a resource collection.")]
        public void DefaultValuesTest()
        {
            ODataEntitySetInfo entitySetInfo = new ODataEntitySetInfo();
            this.Assert.IsNull(entitySetInfo.Url, "Expected null default value for property 'Url'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a ODataEntitySetInfo.")]
        public void PropertyGettersAndSettersTest()
        {
            Uri url = new Uri("MyResourceCollection", UriKind.Relative);

            ODataEntitySetInfo entitySetInfo = new ODataEntitySetInfo()
            {
                Url = url
            };

            this.Assert.AreSame(url, entitySetInfo.Url, "Expected reference equal values for property 'Url'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a resource collection.")]
        public void PropertySettersNullTest()
        {
            ODataEntitySetInfo entitySetInfo = new ODataEntitySetInfo();
            entitySetInfo.Url = null;

            this.Assert.IsNull(entitySetInfo.Url, "Expected null value for property 'Url'.");
        }
    }
}
