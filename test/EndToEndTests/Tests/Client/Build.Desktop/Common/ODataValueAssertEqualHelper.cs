//---------------------------------------------------------------------
// <copyright file="ODataValueAssertEqualHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Tests.Client.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class ODataValueAssertEqualHelper
    {
        #region Util methods to AssertAreEqual ODataValues

        public static void AssertODataValueAreEqual(ODataValue expected, ODataValue actual)
        {
            ODataPrimitiveValue expectedPrimitiveValue = expected as ODataPrimitiveValue;
            ODataPrimitiveValue actualPrimitiveValue = actual as ODataPrimitiveValue;
            if (expectedPrimitiveValue != null && actualPrimitiveValue != null)
            {
                AssertODataPrimitiveValueAreEqual(expectedPrimitiveValue, actualPrimitiveValue);
            }
            else
            {
                ODataComplexValue expectedComplexValue = expected as ODataComplexValue;
                ODataComplexValue actualComplexValue = actual as ODataComplexValue;
                if (expectedComplexValue != null && actualComplexValue != null)
                {
                    AssertODataComplexValueAreEqual(expectedComplexValue, actualComplexValue);
                }
                else
                {
                    ODataEnumValue expectedEnumValue = expected as ODataEnumValue;
                    ODataEnumValue actualEnumValue = actual as ODataEnumValue;
                    if (expectedEnumValue != null && actualEnumValue != null)
                    {
                        AssertODataEnumValueAreEqual(expectedEnumValue, actualEnumValue);
                    }
                    else
                    {
                        ODataCollectionValue expectedCollectionValue = (ODataCollectionValue)expected;
                        ODataCollectionValue actualCollectionValue = (ODataCollectionValue)actual;
                        AssertODataCollectionValueAreEqual(expectedCollectionValue, actualCollectionValue);
                    }
                }
            }
        }

        private static void AssertODataCollectionValueAreEqual(ODataCollectionValue expectedCollectionValue, ODataCollectionValue actualCollectionValue)
        {
            Assert.IsNotNull(expectedCollectionValue);
            Assert.IsNotNull(actualCollectionValue);
            Assert.AreEqual(expectedCollectionValue.TypeName, actualCollectionValue.TypeName);
            var expectedItemsArray = expectedCollectionValue.Items.OfType<object>().ToArray();
            var actualItemsArray = actualCollectionValue.Items.OfType<object>().ToArray();

            Assert.AreEqual(expectedItemsArray.Length, actualItemsArray.Length);
            for (int i = 0; i < expectedItemsArray.Length; i++)
            {
                var expectedOdataValue = expectedItemsArray[i] as ODataValue;
                var actualOdataValue = actualItemsArray[i] as ODataValue;
                if (expectedOdataValue != null && actualOdataValue != null)
                {
                    AssertODataValueAreEqual(expectedOdataValue, actualOdataValue);
                }
                else
                {
                    Assert.AreEqual(expectedItemsArray[i], actualItemsArray[i]);
                }
            }
        }

        private static void AssertODataComplexValueAreEqual(ODataComplexValue expectedComplexValue, ODataComplexValue actualComplexValue)
        {
            Assert.IsNotNull(expectedComplexValue);
            Assert.IsNotNull(actualComplexValue);
            Assert.AreEqual(expectedComplexValue.TypeName, actualComplexValue.TypeName);
            AssertODataPropertiesAreEqual(expectedComplexValue.Properties, actualComplexValue.Properties);
        }

        public static void AssertODataPropertiesAreEqual(IEnumerable<ODataProperty> expectedProperties, IEnumerable<ODataProperty> actualProperties)
        {
            if (expectedProperties == null && actualProperties == null)
            {
                return;
            }

            Assert.IsNotNull(expectedProperties);
            Assert.IsNotNull(actualProperties);
            var expectedPropertyArray = expectedProperties.ToArray();
            var actualPropertyArray = actualProperties.ToArray();
            Assert.AreEqual(expectedPropertyArray.Length, actualPropertyArray.Length);
            for (int i = 0; i < expectedPropertyArray.Length; i++)
            {
                AssertODataPropertyAreEqual(expectedPropertyArray[i], actualPropertyArray[i]);
            }
        }

        public static void AssertODataPropertyAreEqual(ODataProperty expectedOdataProperty, ODataProperty actualOdataProperty)
        {
            Assert.IsNotNull(expectedOdataProperty);
            Assert.IsNotNull(actualOdataProperty);
            Assert.AreEqual(expectedOdataProperty.Name, actualOdataProperty.Name);
            AssertODataValueAreEqual(ToODataValue(expectedOdataProperty.Value), ToODataValue(actualOdataProperty.Value));
        }

        private static ODataValue ToODataValue(object value)
        {
            if (value == null)
            {
                return new ODataNullValue();
            }

            var odataValue = value as ODataValue;
            if (odataValue != null)
            {
                return odataValue;
            }

            return new ODataPrimitiveValue(value);
        }

        private static void AssertODataPrimitiveValueAreEqual(ODataPrimitiveValue expectedPrimitiveValue, ODataPrimitiveValue actualPrimitiveValue)
        {
            Assert.IsNotNull(expectedPrimitiveValue);
            Assert.IsNotNull(actualPrimitiveValue);
            Assert.AreEqual(expectedPrimitiveValue.Value, actualPrimitiveValue.Value);
        }

        private static void AssertODataEnumValueAreEqual(ODataEnumValue expectedEnumValue, ODataEnumValue actualEnumValue)
        {
            Assert.IsNotNull(expectedEnumValue);
            Assert.IsNotNull(actualEnumValue);
            Assert.AreEqual(expectedEnumValue.Value, actualEnumValue.Value);
            Assert.AreEqual(expectedEnumValue.TypeName, actualEnumValue.TypeName);
        }
        #endregion Util methods to AssertAreEqual ODataValues

    }
}
