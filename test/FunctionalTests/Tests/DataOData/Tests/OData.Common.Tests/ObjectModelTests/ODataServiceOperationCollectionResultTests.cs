//---------------------------------------------------------------------
// <copyright file="ODataServiceOperationCollectionResultTests.cs" company="Microsoft">
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
    /// Tests for the ODataCollectionStart object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataCollectionStartTests : ODataTestCase
    {
        [TestMethod, Variation(Description = "Test the default values and setter and getter of ODataCollectionStart.")]
        public void DefaultValuesAndPropertiesGetterAndSetterTest()
        {
            ODataCollectionStart odataCollectionStart = new ODataCollectionStart();
            this.Assert.IsNull(odataCollectionStart.Name, "Expected null default value for property 'Name'.");
        }

        [TestMethod, Variation(Description = "Tests setting ODataCollectionStart properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataCollectionStart odataCollectionStart = new ODataCollectionStart()
                {
                    Name = "DummyName"
                };
            odataCollectionStart.Name = null;
            this.Assert.IsNull(odataCollectionStart.Name, "Expected null value for property 'Name'.");
        }
    }
}
