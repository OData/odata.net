//---------------------------------------------------------------------
// <copyright file="VersioningListTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.Common;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

public class VersioningListTests : EdmLibTestCaseBase
{
    [Fact]
    public void Validate_IntegerVersioningListOperationsAcrossListTypes()
    {
        VersioningList<int> list1 = VersioningList<int>.Create();
        Assert.True(list1 is VersioningList<int>.EmptyVersioningList);
        TestList(list1, 0);

        VersioningList<int> list2 = list1.Add(0);
        Assert.True(list2 is VersioningList<int>.ArrayVersioningList);
        TestList(list2, 1);

        VersioningList<int> list3 = list1.Add(0);
        Assert.True(list3 is VersioningList<int>.ArrayVersioningList);
        TestList(list3, 1);

        VersioningList<int> list4 = list2.Add(10);
        Assert.True(list4 is VersioningList<int>.ArrayVersioningList);
        TestList(list4, 2);

        VersioningList<int> list5 = list4.Add(20);
        Assert.True(list5 is VersioningList<int>.ArrayVersioningList);
        TestList(list5, 3);

        VersioningList<int> list6 = list5.Add(30);
        Assert.True(list6 is VersioningList<int>.ArrayVersioningList);
        TestList(list6, 4);

        VersioningList<int> list7 = list6.Add(40);
        Assert.True(list7 is VersioningList<int>.ArrayVersioningList);
        TestList(list7, 5);

        VersioningList<int> list8 = list7.Add(50);
        Assert.True(list8 is VersioningList<int>.ArrayVersioningList);
        TestList(list8, 6);

        VersioningList<int> list9 = list8.Add(60);
        Assert.True(list9 is VersioningList<int>.ArrayVersioningList);
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
            Assert.Equal(index * 10, shrinker[0]);
            expectedCount--;
            shrinker = shrinker.RemoveAt(0);
            Assert.Equal(expectedCount, shrinker.Count);
        }

        shrinker = list9;
        expectedCount = list9.Count;
        for (int index = list9.Count - 1; index >= 0; index--)
        {
            Assert.Equal(index * 10, shrinker[index]);
            expectedCount--;
            shrinker = shrinker.RemoveAt(index);
            Assert.Equal(expectedCount, shrinker.Count);
        }

        TestList(list9, 7);
    }

    [Fact]
    public void Validate_StringVersioningListOperationsWithLargeData()
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
            Assert.Equal(a, s);
            count++;
        }

        Assert.Equal(1000, count);
    }

    private void TestList(VersioningList<int> list, int expectedCount)
    {
        int count = 0;
        foreach (int i in list)
        {
            Assert.Equal(count * 10, i);
            Assert.Equal(count * 10, list[count]);
            count++;
        }

        Assert.Equal(expectedCount, count);
    }

    #region Private Methods

    private static void TestList(VersioningList<int> list, int expectedCount, int skipIndex)
    {
        int count = 0;
        int skipped = 0;
        foreach (int i in list)
        {
            if (count == skipIndex)
            {
                skipped++;
            }
            Assert.Equal((count + skipped) * 10, i);
            Assert.Equal((count + skipped) * 10, list[count]);
            count++;
        }

        if (skipIndex == list.Count)
        {
            skipped++;
        }

        Assert.Equal(expectedCount, count + skipped);
    }

    #endregion
}
