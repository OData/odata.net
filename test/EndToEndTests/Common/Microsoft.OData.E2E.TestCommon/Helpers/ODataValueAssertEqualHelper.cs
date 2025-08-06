//---------------------------------------------------------------------
// <copyright file="ODataValueAssertEqualHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.E2E.TestCommon.Helpers;

public static class ODataValueAssertEqualHelper
{
    #region Util methods to AssertEqual ODataValues

    public static void AssertODataValueEqual(ODataValue expected, ODataValue actual)
    {
        ODataPrimitiveValue expectedPrimitiveValue = expected as ODataPrimitiveValue;
        ODataPrimitiveValue actualPrimitiveValue = actual as ODataPrimitiveValue;
        if (expectedPrimitiveValue != null && actualPrimitiveValue != null)
        {
            AssertODataPrimitiveValueEqual(expectedPrimitiveValue, actualPrimitiveValue);
        }
        else
        {
            ODataEnumValue expectedEnumValue = expected as ODataEnumValue;
            ODataEnumValue actualEnumValue = actual as ODataEnumValue;
            if (expectedEnumValue != null && actualEnumValue != null)
            {
                AssertODataEnumValueEqual(expectedEnumValue, actualEnumValue);
            }
            else
            {
                ODataCollectionValue expectedCollectionValue = (ODataCollectionValue)expected;
                ODataCollectionValue actualCollectionValue = (ODataCollectionValue)actual;
                AssertODataCollectionValueEqual(expectedCollectionValue, actualCollectionValue);
            }
        }
    }

    private static void AssertODataCollectionValueEqual(ODataCollectionValue expectedCollectionValue, ODataCollectionValue actualCollectionValue)
    {
        Assert.NotNull(expectedCollectionValue);
        Assert.NotNull(actualCollectionValue);
        Assert.Equal(expectedCollectionValue.TypeName, actualCollectionValue.TypeName);
        var expectedItemsArray = expectedCollectionValue.Items.OfType<object>().ToArray();
        var actualItemsArray = actualCollectionValue.Items.OfType<object>().ToArray();

        Assert.Equal(expectedItemsArray.Length, actualItemsArray.Length);
        for (int i = 0; i < expectedItemsArray.Length; i++)
        {
            var expectedOdataValue = expectedItemsArray[i] as ODataValue;
            var actualOdataValue = actualItemsArray[i] as ODataValue;
            if (expectedOdataValue != null && actualOdataValue != null)
            {
                AssertODataValueEqual(expectedOdataValue, actualOdataValue);
            }
            else
            {
                Assert.Equal(expectedItemsArray[i], actualItemsArray[i]);
            }
        }
    }

    public static void AssertODataPropertiesEqual(IEnumerable<ODataProperty> expectedProperties, IEnumerable<ODataProperty> actualProperties)
    {
        if (expectedProperties == null && actualProperties == null)
        {
            return;
        }

        Assert.NotNull(expectedProperties);
        Assert.NotNull(actualProperties);
        var expectedPropertyArray = expectedProperties.ToArray();
        var actualPropertyArray = actualProperties.ToArray();
        Assert.Equal(expectedPropertyArray.Length, actualPropertyArray.Length);
        for (int i = 0; i < expectedPropertyArray.Length; i++)
        {
            AssertODataPropertyEqual(expectedPropertyArray[i], actualPropertyArray[i]);
        }
    }

    public static void AssertODataPropertyEqual(ODataProperty expectedOdataProperty, ODataProperty actualOdataProperty)
    {
        Assert.NotNull(expectedOdataProperty);
        Assert.NotNull(actualOdataProperty);
        Assert.Equal(expectedOdataProperty.Name, actualOdataProperty.Name);
        AssertODataValueEqual(ToODataValue(expectedOdataProperty.Value), ToODataValue(actualOdataProperty.Value));
    }

    private static ODataValue ToODataValue(object value)
    {
        if (value == null)
        {
            return ODataNullValue.Instance;
        }

        var odataValue = value as ODataValue;
        if (odataValue != null)
        {
            return odataValue;
        }

        return new ODataPrimitiveValue(value);
    }

    private static void AssertODataPrimitiveValueEqual(ODataPrimitiveValue expectedPrimitiveValue, ODataPrimitiveValue actualPrimitiveValue)
    {
        Assert.NotNull(expectedPrimitiveValue);
        Assert.NotNull(actualPrimitiveValue);
        Assert.Equal(expectedPrimitiveValue.Value, actualPrimitiveValue.Value);
    }

    private static void AssertODataEnumValueEqual(ODataEnumValue expectedEnumValue, ODataEnumValue actualEnumValue)
    {
        Assert.NotNull(expectedEnumValue);
        Assert.NotNull(actualEnumValue);
        Assert.Equal(expectedEnumValue.Value, actualEnumValue.Value);
        Assert.Equal(expectedEnumValue.TypeName, actualEnumValue.TypeName);
    }

    #endregion Util methods to AssertEqual ODataValues

}
