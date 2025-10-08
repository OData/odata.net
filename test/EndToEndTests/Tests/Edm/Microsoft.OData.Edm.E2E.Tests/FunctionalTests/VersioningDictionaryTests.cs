//---------------------------------------------------------------------
// <copyright file="VersioningDictionaryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.Common;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

/// <summary>
/// This class contains tests that validate the functionality of the VersioningDictionary class.
/// </summary>
public class VersioningDictionaryTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_IntegerKeyInsertionAndDeletionAcrossDictionaryTypes()
    {
        VersioningDictionary<int, int> map = VersioningDictionary<int, int>.Create(CompareIntegers);


        Assert.True(map is VersioningDictionary<int, int>.EmptyVersioningDictionary);
        Assert.False(map.TryGetValue(0, out int value));

        // Test conversions back and forth between EmptyDictionary, OneKeyDictionary, and TwoKeyDictionary.

        map = map.Set(1, 10);
        Assert.True(map is VersioningDictionary<int, int>.OneKeyDictionary);
        Assert.Equal(10, map.Get(1));
        Assert.False(map.TryGetValue(0, out value));

        map = map.Set(1, 100);
        Assert.True(map is VersioningDictionary<int, int>.OneKeyDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.False(map.TryGetValue(0, out value));

        map = map.Remove(1);
        Assert.True(map is VersioningDictionary<int, int>.EmptyVersioningDictionary);

        map = map.Set(1, 10);
        map = map.Set(2, 20);
        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary);
        Assert.Equal(10, map.Get(1));
        Assert.Equal(20, map.Get(2));
        Assert.False(map.TryGetValue(0, out value));

        map = map.Set(1, 100);
        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.Equal(20, map.Get(2));

        map = map.Set(2, 200);
        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.Equal(200, map.Get(2));

        VersioningDictionary<int, int> oneMap = map.Remove(1);
        Assert.True(oneMap is VersioningDictionary<int, int>.OneKeyDictionary);
        Assert.Equal(200, oneMap.Get(2));

        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.Equal(200, map.Get(2));
        Assert.False(map.TryGetValue(0, out value));

        oneMap = map.Remove(2);
        Assert.True(oneMap is VersioningDictionary<int, int>.OneKeyDictionary);
        Assert.Equal(100, oneMap.Get(1));

        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.Equal(200, map.Get(2));

        // Test conversion to a TreeDictionary.

        map = map.Set(3, 30);
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.Equal(200, map.Get(2));
        Assert.Equal(30, map.Get(3));
        Assert.False(map.TryGetValue(0, out value));

        map = map.Set(3, 300);
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary);
        Assert.Equal(100, map.Get(1));
        Assert.Equal(200, map.Get(2));
        Assert.Equal(300, map.Get(3));

        // Test that a significant number of keys does not force conversion to a HashKeyDictionary.

        for (int i = 1; i <= 100; i++)
        {
            map = map.Set(i, i * 10);
        }
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary);

        for (int i = 1; i <= 100; i++)
        {
            Assert.Equal(i * 10, map.Get(i));
        }
        Assert.False(map.TryGetValue(0, out value));

        // Test removing half the keys.

        for (int i = 1; i < 100; i += 2)
        {
            map = map.Remove(i);
        }
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary);
        for (int i = 1; i < 100; i += 2)
        {
            Assert.False(map.TryGetValue(i, out value));
            Assert.Equal((i + 1) * 10, map.Get(i + 1));
        }

        // Test conversion to a HashTreeDictionary.

        for (int i = 1; i <= 10000; i++)
        {
            map = map.Set(i, i * 2);
        }
        Assert.True(map is VersioningDictionary<int, int>.HashTreeDictionary);
        for (int i = 1; i <= 10000; i++)
        {
            Assert.Equal(i * 2, map.Get(i));
        }
        Assert.False(map.TryGetValue(0, out value));

        // Test removing half the keys.

        for (int i = 1; i < 10000; i += 2)
        {
            map = map.Remove(i);
        }
        Assert.True(map is VersioningDictionary<int, int>.HashTreeDictionary);
        for (int i = 1; i < 10000; i += 2)
        {
            Assert.False(map.TryGetValue(i, out value));
            Assert.Equal((i + 1) * 2, map.Get(i + 1));
        }
    }

    [Fact]
    public void Validate_StringKeyInsertionAcrossDictionaryTypes()
    {
        VersioningDictionary<string, int> stringMap = VersioningDictionary<string, int>.Create(CompareStrings);
        stringMap = stringMap.Set("Fred", 1);
        stringMap = stringMap.Set("Wilma", 2);
        stringMap = stringMap.Set("Betty", 3);
        stringMap = stringMap.Set("Barney", 4);
        Assert.True(stringMap is VersioningDictionary<string, int>.TreeDictionary);
        Assert.Equal(1, stringMap.Get("Fred"));
        Assert.Equal(4, stringMap.Get("Barney"));
        Assert.Equal(3, stringMap.Get("Betty"));
        Assert.Equal(2, stringMap.Get("Wilma"));

        for (int i = 1; i < 1000; i++)
        {
            stringMap = stringMap.Set("Fred" + i, i + 1);
            stringMap = stringMap.Set("Wilma" + i, i + 2);
            stringMap = stringMap.Set("Betty" + i, i + 3);
            stringMap = stringMap.Set("Barney" + i, i + 4);
        }
        Assert.True(stringMap is VersioningDictionary<string, int>.HashTreeDictionary);
        Assert.Equal(1, stringMap.Get("Fred"));
        Assert.Equal(4, stringMap.Get("Barney"));
        Assert.Equal(3, stringMap.Get("Betty"));
        Assert.Equal(2, stringMap.Get("Wilma"));
        for (int i = 1; i < 1000; i++)
        {
            Assert.Equal(i + 1, stringMap.Get("Fred" + i));
            Assert.Equal(i + 2, stringMap.Get("Wilma" + i));
            Assert.Equal(i + 3, stringMap.Get("Betty" + i));
            Assert.Equal(i + 4, stringMap.Get("Barney" + i));
        }
    }

    #region Private Methods

    private static int CompareIntegers(int x, int y)
    {
        if (x < y)
        {
            return -1;
        }
        if (x > y)
        {
            return 1;
        }
        return 0;
    }

    private static int CompareStrings(string x, string y)
    {
        return x.CompareTo(y);
    }

    #endregion
}
