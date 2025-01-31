﻿//---------------------------------------------------------------------
// <copyright file="SingletonODataValueAssertEqualHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Xunit;

namespace Microsoft.OData.Client.E2E.Tests.SingletonTests.Tests;

public static class SingletonODataValueAssertEqualHelper
{
    #region Util methods to AssertEqual ODataValues

    public static void AssertODataValueEqual(ODataValue expected, ODataValue actual)
    {
        if (expected is ODataPrimitiveValue expectedPrimitiveValue && actual is ODataPrimitiveValue actualPrimitiveValue)
        {
            AssertODataPrimitiveValueEqual(expectedPrimitiveValue, actualPrimitiveValue);
        }
        else
        {
            if (expected is ODataEnumValue expectedEnumValue && actual is ODataEnumValue actualEnumValue)
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
            if (expectedItemsArray[i] is ODataValue expectedOdataValue && actualItemsArray[i] is ODataValue actualOdataValue)
            {
                AssertODataValueEqual(expectedOdataValue, actualOdataValue);
            }
            else
            {
                Assert.Equal(expectedItemsArray[i], actualItemsArray[i]);
            }
        }
    }

    public static void AssertODataPropertiesEqual(IEnumerable<ODataPropertyInfo> expectedProperties, IEnumerable<ODataPropertyInfo> actualProperties)
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

    public static void AssertODataPropertyAndResourceEqual(ODataResource expectedOdataProperty, ODataResource actualOdataProperty)
    {
        Assert.NotNull(expectedOdataProperty);
        Assert.NotNull(actualOdataProperty);
        AssertODataValueAndResourceEqual(expectedOdataProperty, actualOdataProperty);
    }

    public static void AssertODataValueAndResourceEqual(ODataResource expected, ODataResource actual)
    {
        Assert.Equal(expected.TypeName, actual.TypeName);
        AssertODataPropertiesEqual(expected.Properties, actual.Properties);
    }

    public static void AssertODataPropertyEqual(ODataPropertyInfo expectedPropertyInfo, ODataPropertyInfo actualPropertyInfo)
    {
        Assert.NotNull(expectedPropertyInfo);
        Assert.NotNull(actualPropertyInfo);
        Assert.Equal(expectedPropertyInfo.Name, actualPropertyInfo.Name);

        var expectedProperty = expectedPropertyInfo as ODataProperty;
        var actualProperty = actualPropertyInfo as ODataProperty;

        Assert.NotNull(expectedProperty);
        Assert.NotNull(actualProperty);

        AssertODataValueEqual(ToODataValue(expectedProperty.Value), ToODataValue(actualProperty.Value));
    }

    private static ODataValue ToODataValue(object value)
    {
        if (value == null)
        {
            return new ODataNullValue();
        }

        if (value is ODataValue odataValue)
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
