//---------------------------------------------------------------------
// <copyright file="ODataPropertyTests.cs" company="Microsoft">
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
    /// Tests for the ODataProperty object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataPropertyTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values of ODataProperty.")]
        public void DefaultValuesTest()
        {
            ODataProperty property = new ODataProperty();
            this.Assert.IsNull(property.Name, "Expected null default value for property 'Name'.");
            this.Assert.IsNull(property.Value, "Expected null default value for property 'Value'.");
        }

        [TestMethod, Variation(Description = "Test the property setters and getters of ODataProperty.")]
        public void PropertyGettersAndSettersTest()
        {
            string name1 = "ODataPrimitiveProperty";
            object value1 = "Hello world";

            ODataProperty primitiveProperty = new ODataProperty()
            {
                Name = name1,
                Value = value1,
            };

            this.Assert.AreEqual(name1, primitiveProperty.Name, "Expected equal name values.");
            this.Assert.AreSame(value1, primitiveProperty.Value, "Expected reference equal values for property 'Value'.");

            string name3 = "ODataCollectionProperty";
            ODataCollectionValue value3 = new ODataCollectionValue()
            {
                Items = new object[] { 1, 2, 3 }
            };

            ODataProperty multiValueProperty = new ODataProperty()
            {
                Name = name3,
                Value = value3,
            };

            this.Assert.AreEqual(name3, multiValueProperty.Name, "Expected equal name values.");
            this.Assert.AreSame(value3, multiValueProperty.Value, "Expected reference equal values for property 'Value'.");
        }

        [TestMethod, Variation(Description = "Test setting properties to null.")]
        public void PropertySettersNullTest()
        {
            ODataProperty property1 = new ODataProperty()
            {
                Name = "ODataPrimitiveProperty",
                Value = "Hello world",
            };

            property1.Name = null;
            property1.Value = null;

            this.Assert.IsNull(property1.Name, "Expected null value for property 'Name'.");
            this.Assert.IsNull(property1.Value, "Expected null value for property 'Value'.");
        }
    }
}
