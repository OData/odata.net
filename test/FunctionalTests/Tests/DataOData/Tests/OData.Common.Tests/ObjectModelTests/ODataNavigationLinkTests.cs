//---------------------------------------------------------------------
// <copyright file="ODataNavigationLinkTests.cs" company="Microsoft">
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
    /// Tests for the ODataNestedResourceInfo object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataNavigationLinkTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of ODataNestedResourceInfo.")]
        public void DefaultValuesTest()
        {
            ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo();
            this.Assert.IsNull(navigationLink.Name, "Expected null default value for property 'Name'.");
            this.Assert.IsNull(navigationLink.Url, "Expected null default value for property 'Url'.");
            this.Assert.IsNull(navigationLink.IsCollection, "Expected null default value for property 'IsCollection'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataNestedResourceInfo.")]
        public void PropertyGettersAndSettersTest()
        {
            string name = "ODataNestedResourceInfo";
            Uri url = new Uri("http://odatatest.org/");
            bool isCollection = true;

            ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo()
            {
                Name = name,
                Url = url,
                IsCollection = isCollection,
            };

            this.Assert.AreEqual(name, navigationLink.Name, "Expected equal name values.");
            this.Assert.AreSame(url, navigationLink.Url, "Expected reference equal values for property 'Url'.");
            this.Assert.AreEqual(isCollection, navigationLink.IsCollection, "Expected equal values for property 'IsCollection'");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataNestedResourceInfo navigationLink = new ODataNestedResourceInfo()
                {
                    Name = "NewLink",
                    Url = new Uri("http://odata.org"),
                    IsCollection = true,
                };

            navigationLink.Name = null;
            navigationLink.Url = null;
            navigationLink.IsCollection = null;

            this.Assert.IsNull(navigationLink.Name, "Expected null value for property 'Name'.");
            this.Assert.IsNull(navigationLink.Url, "Expected null value for property 'Url'.");
            this.Assert.IsNull(navigationLink.Url, "Expected null value for property 'IsCollection'.");
        }
    }
}
