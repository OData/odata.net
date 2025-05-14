//---------------------------------------------------------------------
// <copyright file="VersioningDictionaryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;
public class VersioningDictionaryTests : EdmLibTestCaseBase
{
    [Fact]
    public void MapIntegerInsertionAndDeletion()
    {
        VersioningDictionary<int, int> map = VersioningDictionary<int, int>.Create(CompareIntegers);

        int value;

        Assert.True(map is VersioningDictionary<int, int>.EmptyVersioningDictionary, "Dictionary type");
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        // Test conversions back and forth between EmptyDictionary, OneKeyDictionary, and TwoKeyDictionary.

        map = map.Set(1, 10);
        Assert.True(map is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 10, "Key value");
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        map = map.Set(1, 100);
        Assert.True(map is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        map = map.Remove(1);
        Assert.True(map is VersioningDictionary<int, int>.EmptyVersioningDictionary, "Dictionary type");

        map = map.Set(1, 10);
        map = map.Set(2, 20);
        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 10, "Key value");
        Assert.Equal(map.Get(2), 20, "Key value");
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        map = map.Set(1, 100);
        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.Equal(map.Get(2), 20, "Key value");

        map = map.Set(2, 200);
        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.Equal(map.Get(2), 200, "Key value");

        VersioningDictionary<int, int> oneMap = map.Remove(1);
        Assert.True(oneMap is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
        Assert.Equal(oneMap.Get(2), 200, "Key value");

        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.Equal(map.Get(2), 200, "Key value");
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        oneMap = map.Remove(2);
        Assert.True(oneMap is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
        Assert.Equal(oneMap.Get(1), 100, "Key value");

        Assert.True(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.Equal(map.Get(2), 200, "Key value");

        // Test conversion to a TreeDictionary.

        map = map.Set(3, 30);
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.Equal(map.Get(2), 200, "Key value");
        Assert.Equal(map.Get(3), 30, "Key value");
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        map = map.Set(3, 300);
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
        Assert.Equal(map.Get(1), 100, "Key value");
        Assert.Equal(map.Get(2), 200, "Key value");
        Assert.Equal(map.Get(3), 300, "Key value");

        // Test that a significant number of keys does not force conversion to a HashKeyDictionary.

        for (int i = 1; i <= 100; i++)
        {
            map = map.Set(i, i * 10);
        }
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
        for (int i = 1; i <= 100; i++)
        {
            Assert.Equal(i * 10, map.Get(i), "Key value");
        }
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        // Test removing half the keys.

        for (int i = 1; i < 100; i += 2)
        {
            map = map.Remove(i);
        }
        Assert.True(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
        for (int i = 1; i < 100; i += 2)
        {
            Assert.False(map.TryGetValue(i, out value), "Odd key");
            Assert.Equal((i + 1) * 10, map.Get(i + 1), "Even key value");
        }

        // Test conversion to a HashTreeDictionary.

        for (int i = 1; i <= 10000; i++)
        {
            map = map.Set(i, i * 2);
        }
        Assert.True(map is VersioningDictionary<int, int>.HashTreeDictionary, "Dictionary type");
        for (int i = 1; i <= 10000; i++)
        {
            Assert.Equal(i * 2, map.Get(i), "Key value");
        }
        Assert.False(map.TryGetValue(0, out value), "Zero key");

        // Test removing half the keys.

        for (int i = 1; i < 10000; i += 2)
        {
            map = map.Remove(i);
        }
        Assert.True(map is VersioningDictionary<int, int>.HashTreeDictionary, "Dictionary type");
        for (int i = 1; i < 10000; i += 2)
        {
            Assert.False(map.TryGetValue(i, out value), "Odd key");
            Assert.Equal((i + 1) * 2, map.Get(i + 1), "Even key value");
        }
    }

    [Fact]
    public void MapStringInsertion()
    {
        VersioningDictionary<string, int> stringMap = VersioningDictionary<string, int>.Create(CompareStrings);
        stringMap = stringMap.Set("Fred", 1);
        stringMap = stringMap.Set("Wilma", 2);
        stringMap = stringMap.Set("Betty", 3);
        stringMap = stringMap.Set("Barney", 4);
        Assert.True(stringMap is VersioningDictionary<string, int>.TreeDictionary, "Dictionary type");
        Assert.Equal(stringMap.Get("Fred"), 1, "Key value");
        Assert.Equal(stringMap.Get("Barney"), 4, "Key value");
        Assert.Equal(stringMap.Get("Betty"), 3, "Key value");
        Assert.Equal(stringMap.Get("Wilma"), 2, "Key value");

        for (int i = 1; i < 1000; i++)
        {
            stringMap = stringMap.Set("Fred" + i, i + 1);
            stringMap = stringMap.Set("Wilma" + i, i + 2);
            stringMap = stringMap.Set("Betty" + i, i + 3);
            stringMap = stringMap.Set("Barney" + i, i + 4);
        }
        Assert.True(stringMap is VersioningDictionary<string, int>.HashTreeDictionary, "Dictionary type");
        Assert.Equal(stringMap.Get("Fred"), 1, "Key value");
        Assert.Equal(stringMap.Get("Barney"), 4, "Key value");
        Assert.Equal(stringMap.Get("Betty"), 3, "Key value");
        Assert.Equal(stringMap.Get("Wilma"), 2, "Key value");
        for (int i = 1; i < 1000; i++)
        {
            Assert.Equal(stringMap.Get("Fred" + i), i + 1, "Key value");
            Assert.Equal(stringMap.Get("Wilma" + i), i + 2, "Key value");
            Assert.Equal(stringMap.Get("Betty" + i), i + 3, "Key value");
            Assert.Equal(stringMap.Get("Barney" + i), i + 4, "Key value");
        }
    }

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
}
