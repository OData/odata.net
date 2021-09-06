//---------------------------------------------------------------------
// <copyright file="VersioningDictionaryTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VersioningDictionaryTests : EdmLibTestCaseBase
    {
        public VersioningDictionaryTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void MapIntegerInsertionAndDeletion()
        {
            VersioningDictionary<int, int> map = VersioningDictionary<int, int>.Create(CompareIntegers);

            int value;

            Assert.IsTrue(map is VersioningDictionary<int, int>.EmptyVersioningDictionary, "Dictionary type");
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            // Test conversions back and forth between EmptyDictionary, OneKeyDictionary, and TwoKeyDictionary.

            map = map.Set(1, 10);
            Assert.IsTrue(map is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 10, "Key value");
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            map = map.Set(1, 100);
            Assert.IsTrue(map is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            map = map.Remove(1);
            Assert.IsTrue(map is VersioningDictionary<int, int>.EmptyVersioningDictionary, "Dictionary type");

            map = map.Set(1, 10);
            map = map.Set(2, 20);
            Assert.IsTrue(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 10, "Key value");
            Assert.AreEqual(map.Get(2), 20, "Key value");
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            map = map.Set(1, 100);
            Assert.IsTrue(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.AreEqual(map.Get(2), 20, "Key value");

            map = map.Set(2, 200);
            Assert.IsTrue(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.AreEqual(map.Get(2), 200, "Key value");

            VersioningDictionary<int, int> oneMap = map.Remove(1);
            Assert.IsTrue(oneMap is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
            Assert.AreEqual(oneMap.Get(2), 200, "Key value");

            Assert.IsTrue(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.AreEqual(map.Get(2), 200, "Key value");
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            oneMap = map.Remove(2);
            Assert.IsTrue(oneMap is VersioningDictionary<int, int>.OneKeyDictionary, "Dictionary type");
            Assert.AreEqual(oneMap.Get(1), 100, "Key value");

            Assert.IsTrue(map is VersioningDictionary<int, int>.TwoKeyDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.AreEqual(map.Get(2), 200, "Key value");

            // Test conversion to a TreeDictionary.

            map = map.Set(3, 30);
            Assert.IsTrue(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.AreEqual(map.Get(2), 200, "Key value");
            Assert.AreEqual(map.Get(3), 30, "Key value");
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            map = map.Set(3, 300);
            Assert.IsTrue(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
            Assert.AreEqual(map.Get(1), 100, "Key value");
            Assert.AreEqual(map.Get(2), 200, "Key value");
            Assert.AreEqual(map.Get(3), 300, "Key value");

            // Test that a significant number of keys does not force conversion to a HashKeyDictionary.

            for (int i = 1; i <= 100; i++)
            {
                map = map.Set(i, i * 10);
            }
            Assert.IsTrue(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
            for (int i = 1; i <= 100; i++)
            {
                Assert.AreEqual(i * 10, map.Get(i), "Key value");
            }
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            // Test removing half the keys.

            for (int i = 1; i < 100; i += 2)
            {
                map = map.Remove(i);
            }
            Assert.IsTrue(map is VersioningDictionary<int, int>.TreeDictionary, "Dictionary type");
            for (int i = 1; i < 100; i += 2)
            {
                Assert.IsFalse(map.TryGetValue(i, out value), "Odd key");
                Assert.AreEqual((i + 1) * 10, map.Get(i + 1), "Even key value");
            }

            // Test conversion to a HashTreeDictionary.

            for (int i = 1; i <= 10000; i++)
            {
                map = map.Set(i, i * 2);
            }
            Assert.IsTrue(map is VersioningDictionary<int, int>.HashTreeDictionary, "Dictionary type");
            for (int i = 1; i <= 10000; i++)
            {
                Assert.AreEqual(i * 2, map.Get(i), "Key value");
            }
            Assert.IsFalse(map.TryGetValue(0, out value), "Zero key");

            // Test removing half the keys.

            for (int i = 1; i < 10000; i += 2)
            {
                map = map.Remove(i);
            }
            Assert.IsTrue(map is VersioningDictionary<int, int>.HashTreeDictionary, "Dictionary type");
            for (int i = 1; i < 10000; i += 2)
            {
                Assert.IsFalse(map.TryGetValue(i, out value), "Odd key");
                Assert.AreEqual((i + 1) * 2, map.Get(i + 1), "Even key value");
            }
        }

        [TestMethod]
        public void MapStringInsertion()
        {
            VersioningDictionary<string, int> stringMap = VersioningDictionary<string, int>.Create(CompareStrings);
            stringMap = stringMap.Set("Fred", 1);
            stringMap = stringMap.Set("Wilma", 2);
            stringMap = stringMap.Set("Betty", 3);
            stringMap = stringMap.Set("Barney", 4);
            Assert.IsTrue(stringMap is VersioningDictionary<string, int>.TreeDictionary, "Dictionary type");
            Assert.AreEqual(stringMap.Get("Fred"), 1, "Key value");
            Assert.AreEqual(stringMap.Get("Barney"), 4, "Key value");
            Assert.AreEqual(stringMap.Get("Betty"), 3, "Key value");
            Assert.AreEqual(stringMap.Get("Wilma"), 2, "Key value");

            for (int i = 1; i < 1000; i++)
            {
                stringMap = stringMap.Set("Fred" + i, i + 1);
                stringMap = stringMap.Set("Wilma" + i, i + 2);
                stringMap = stringMap.Set("Betty" + i, i + 3);
                stringMap = stringMap.Set("Barney" + i, i + 4);
            }
            Assert.IsTrue(stringMap is VersioningDictionary<string, int>.HashTreeDictionary, "Dictionary type");
            Assert.AreEqual(stringMap.Get("Fred"), 1, "Key value");
            Assert.AreEqual(stringMap.Get("Barney"), 4, "Key value");
            Assert.AreEqual(stringMap.Get("Betty"), 3, "Key value");
            Assert.AreEqual(stringMap.Get("Wilma"), 2, "Key value");
            for (int i = 1; i < 1000; i++)
            {
                Assert.AreEqual(stringMap.Get("Fred" + i), i + 1, "Key value");
                Assert.AreEqual(stringMap.Get("Wilma" + i), i + 2, "Key value");
                Assert.AreEqual(stringMap.Get("Betty" + i), i + 3, "Key value");
                Assert.AreEqual(stringMap.Get("Barney" + i), i + 4, "Key value");
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
}
