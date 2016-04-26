//---------------------------------------------------------------------
// <copyright file="ODataComplexValueTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Tests.ObjectModelTests
{
    #region Namespaces
    using System.Collections.Generic;
    using Microsoft.OData;
    using Microsoft.Test.Taupo.Execution;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion Namespaces

    /// <summary>
    /// Tests for the ODataComplexValue object model type.
    /// </summary>
    [TestClass, TestCase]
    public class ODataComplexValueTests : ODataTestCase
    {

        [TestMethod, Variation(Description = "Test the default values and setter and getter of ODataComplexValue property 'Properties'.")]
        public void DefaultValuesAndPropertiesGetterAndSetterTest()
        {
            ODataComplexValue odataComplexValue = new ODataComplexValue();
            this.Assert.IsNull(odataComplexValue.Properties, "Expected null default value for property 'Properties'.");
            List<ODataProperty> properties = new List<ODataProperty>();
            odataComplexValue.Properties = properties;
            this.Assert.AreSame(odataComplexValue.Properties, properties, "Expected reference equal values for property 'Properties'.");
        }

        [TestMethod, Variation(Description = "Tests setting to null ODataComplexValue properties.")]
        public void PropertySettersNullTest()
        {
            ODataComplexValue odataComplexValue = new ODataComplexValue()
                {
                    Properties = new List<ODataProperty>()
                };
            odataComplexValue.Properties = null;
            this.Assert.IsNull(odataComplexValue.Properties, "Expected null value for property 'Properties'.");
        }
    }
}
