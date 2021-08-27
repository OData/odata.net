//---------------------------------------------------------------------
// <copyright file="VersioningListTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VersioningListTests : EdmLibTestCaseBase
    {
        public VersioningListTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void IntegerVersioningLists()
        {
            VersioningList<int> list1 = VersioningList<int>.Create();
            Assert.IsTrue(list1 is VersioningList<int>.EmptyVersioningList, "List type");
            TestList(list1, 0);

            VersioningList<int> list2 = list1.Add(0);
            Assert.IsTrue(list2 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list2, 1);

            VersioningList<int> list3 = list1.Add(0);
            Assert.IsTrue(list3 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list3, 1);

            VersioningList<int> list4 = list2.Add(10);
            Assert.IsTrue(list4 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list4, 2);

            VersioningList<int> list5 = list4.Add(20);
            Assert.IsTrue(list5 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list5, 3);

            VersioningList<int> list6 = list5.Add(30);
            Assert.IsTrue(list6 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list6, 4);

            VersioningList<int> list7 = list6.Add(40);
            Assert.IsTrue(list7 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list7, 5);

            VersioningList<int> list8 = list7.Add(50);
            Assert.IsTrue(list8 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list8, 6);

            VersioningList<int> list9 = list8.Add(60);
            Assert.IsTrue(list9 is VersioningList<int>.ArrayVersioningList, "List type");
            TestList(list9, 7);

            TestList(list8, 6);
            TestList(list7, 5);
            TestList(list6, 4);
            TestList(list5, 3);
            TestList(list4, 2);
            TestList(list3, 1);
            TestList(list2, 1);
            TestList(list1, 0);

            for (int index = 0; index < list9.Count; index++)
            {
                VersioningList<int> list00 = list9.RemoveAt(index);
                TestList(list00, 7, index);
            }

            TestList(list9, 7);

            VersioningList<int> shrinker = list9;
            int expectedCount = list9.Count;
            for (int index = 0; index < list9.Count; index++)
            {
                Assert.AreEqual(index * 10, shrinker[0], "Shrinking value");
                expectedCount--;
                shrinker = shrinker.RemoveAt(0);
                Assert.AreEqual(expectedCount, shrinker.Count, "Shrinking count");
            }

            shrinker = list9;
            expectedCount = list9.Count;
            for (int index = list9.Count - 1; index >= 0; index--)
            {
                Assert.AreEqual(index * 10, shrinker[index], "Shrinking value");
                expectedCount--;
                shrinker = shrinker.RemoveAt(index);
                Assert.AreEqual(expectedCount, shrinker.Count, "Shrinking count");
            }

            TestList(list9, 7);
        }

        [TestMethod]
        public void StringVersioningLists()
        {
            VersioningList<string> list1 = VersioningList<string>.Create();
            string a = "";
            for (int i = 0; i < 1000; i++)
            {
                a = a + "a";
                list1 = list1.Add(a);
            }

            a = "";
            int count = 0;
            foreach (string s in list1)
            {
                a = a + "a";
                Assert.AreEqual(a, s, "Element value");
                count++;
            }

            Assert.AreEqual(1000, count, "Element count");
        }

        private void TestList(VersioningList<int> list, int expectedCount)
        {
            int count = 0;
            foreach (int i in list)
            {
                Assert.AreEqual(count * 10, i, "Element value");
                Assert.AreEqual(count * 10, list[count], "Element value");
                count++;
            }

            Assert.AreEqual(expectedCount, count, "List count");
        }

        private void TestList(VersioningList<int> list, int expectedCount, int skipIndex)
        {
            int count = 0;
            int skipped = 0;
            foreach (int i in list)
            {
                if (count == skipIndex)
                {
                    skipped++;
                }
                Assert.AreEqual((count + skipped) * 10, i, "Element value");
                Assert.AreEqual((count + skipped) * 10, list[count], "Element value");
                count++;
            }

            if (skipIndex == list.Count)
            {
                skipped++;
            }

            Assert.AreEqual(expectedCount, count + skipped, "List count");
        }
    }
}
