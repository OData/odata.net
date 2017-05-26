//---------------------------------------------------------------------
// <copyright file="ODataServiceDocumentTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataServiceDocument object model class.
    /// </summary>
    [TestClass, TestCase]
    public class ODataServiceDocumentTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values of a service workspace.")]
        public void DefaultValuesTest()
        {
            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            this.Assert.IsNull(serviceDocument.EntitySets, "Expected null default value for property 'EntitySets'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a service workspace.")]
        public void PropertyGettersAndSettersTest()
        {
            ODataEntitySetInfo collection1 = new ODataEntitySetInfo();
            ODataEntitySetInfo collection2 = new ODataEntitySetInfo();
            ODataEntitySetInfo collection3 = new ODataEntitySetInfo();
            ODataEntitySetInfo[] collections = new ODataEntitySetInfo[] { collection1, collection2, collection3 };

            ODataServiceDocument serviceDocument = new ODataServiceDocument()
            {
                EntitySets = collections
            };

            this.Assert.AreSame(collections, serviceDocument.EntitySets, "Expected reference equal values for property 'EntitySets'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of a service workspace.")]
        public void PropertySettersNullTest()
        {
            ODataServiceDocument serviceDocument = new ODataServiceDocument();
            serviceDocument.EntitySets = null;

            this.Assert.IsNull(serviceDocument.EntitySets, "Expected null value for property 'EntitySets'.");
        }
    }
}
