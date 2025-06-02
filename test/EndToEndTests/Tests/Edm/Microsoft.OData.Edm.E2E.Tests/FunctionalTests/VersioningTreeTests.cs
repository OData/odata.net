// ---------------------------------------------------------------------
// <copyright file="VersioningTreeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.E2E.Tests.Common;

namespace Microsoft.OData.Edm.E2E.Tests.FunctionalTests;

/// <summary>
/// Tests for the VersioningTree class, which implements a versioned binary search tree.
/// </summary>
public class VersioningTreeTests : EdmLibTestCaseBase
{
    #region Tests basic insertion of key-value pairs into a VersioningTree and validates the structure of the tree.
    
    [Fact]
    public void Validate_BasicInsertionAndStructureInVersioningTree()
    {
        VersioningTree<int, int> tree = new VersioningTree<int, int>(100, 1000, null, null);

        tree = tree.SetKeyValue(90, 900, CompareIntegers);
        tree = tree.SetKeyValue(110, 1100, CompareIntegers);

        Assert.Equal(100, tree.Key);
        Assert.Equal(90, tree.LeftChild.Key);
        Assert.Equal(110, tree.RightChild.Key);

        tree = tree.SetKeyValue(80, 800, CompareIntegers);
        tree = tree.SetKeyValue(85, 850, CompareIntegers);

        Assert.Equal(90, tree.Key);
        Assert.Equal(100, tree.RightChild.Key);
        Assert.Equal(110, tree.RightChild.RightChild.Key);
        Assert.Equal(80, tree.LeftChild.Key);
        Assert.Equal(85, tree.LeftChild.RightChild.Key);

        var OneHundred = tree.RightChild;

        tree = tree.SetKeyValue(70, 700, CompareIntegers);
        tree = tree.SetKeyValue(60, 600, CompareIntegers);
        tree = tree.SetKeyValue(50, 500, CompareIntegers);
        tree = tree.SetKeyValue(80, 800, CompareIntegers);

        Assert.Equal(80, tree.Key);
        Assert.Equal(60, tree.LeftChild.Key);
        Assert.Equal(50, tree.LeftChild.LeftChild.Key);
        Assert.Equal(70, tree.LeftChild.RightChild.Key);
        Assert.Equal(90, tree.RightChild.Key);
        Assert.Equal(85, tree.RightChild.LeftChild.Key);
        Assert.Equal(100, tree.RightChild.RightChild.Key);
        Assert.Equal(110, tree.RightChild.RightChild.RightChild.Key);

        // Test that the 100 node has not been copied.
        Assert.Equal(OneHundred, tree.RightChild.RightChild);

        var Sixty = tree.LeftChild;

        tree = tree.SetKeyValue(96, 960, CompareIntegers);
        tree = tree.SetKeyValue(98, 980, CompareIntegers);

        Assert.Equal(90, tree.Key);
        Assert.Equal(80, tree.LeftChild.Key);
        Assert.Equal(60, tree.LeftChild.LeftChild.Key);
        Assert.Equal(50, tree.LeftChild.LeftChild.LeftChild.Key);
        Assert.Equal(70, tree.LeftChild.LeftChild.RightChild.Key);
        Assert.Equal(85, tree.LeftChild.RightChild.Key);
        Assert.Equal(100, tree.RightChild.Key);
        Assert.Equal(96, tree.RightChild.LeftChild.Key);
        Assert.Equal(98, tree.RightChild.LeftChild.RightChild.Key);
        Assert.Equal(110, tree.RightChild.RightChild.Key);

        // Test that the 60 node has not been copied.
        Assert.Equal(Sixty, tree.LeftChild.LeftChild);

        var Fifty = tree.LeftChild.LeftChild.LeftChild;
        OneHundred = tree.RightChild;

        tree = tree.SetKeyValue(65, 650, CompareIntegers);

        Assert.Equal(90, tree.Key);
        Assert.Equal(60, tree.LeftChild.Key);
        Assert.Equal(50, tree.LeftChild.LeftChild.Key);
        VerifyLeaf(tree.LeftChild.LeftChild);

        Assert.Equal(80, tree.LeftChild.RightChild.Key);
        Assert.Equal(70, tree.LeftChild.RightChild.LeftChild.Key);
        Assert.Equal(65, tree.LeftChild.RightChild.LeftChild.LeftChild.Key);
        Assert.Null(tree.LeftChild.RightChild.LeftChild.RightChild);
        Assert.Equal(85, tree.LeftChild.RightChild.RightChild.Key);
        Assert.Equal(100, tree.RightChild.Key);
        Assert.Equal(96, tree.RightChild.LeftChild.Key);
        Assert.Null(tree.RightChild.LeftChild.LeftChild);
        Assert.Equal(98, tree.RightChild.LeftChild.RightChild.Key);
        Assert.Equal(110, tree.RightChild.RightChild.Key);
        VerifyLeaf(tree.RightChild.RightChild);

        Assert.Equal(Fifty, tree.LeftChild.LeftChild);
        Assert.Equal(OneHundred, tree.RightChild);

        Sixty = tree.LeftChild;

        tree = tree.SetKeyValue(95, 950, CompareIntegers);
        tree = tree.SetKeyValue(94, 940, CompareIntegers);
        tree = tree.SetKeyValue(93, 930, CompareIntegers);
        tree = tree.SetKeyValue(93, 930, CompareIntegers);
        tree = tree.SetKeyValue(92, 920, CompareIntegers);
        tree = tree.SetKeyValue(91, 910, CompareIntegers);

        Assert.Equal(90, tree.Key);
        Assert.Equal(60, tree.LeftChild.Key);
        Assert.Equal(50, tree.LeftChild.LeftChild.Key);
        VerifyLeaf(tree.LeftChild.LeftChild);
        Assert.Equal(80, tree.LeftChild.RightChild.Key);
        Assert.Equal(70, tree.LeftChild.RightChild.LeftChild.Key);
        Assert.Equal(65, tree.LeftChild.RightChild.LeftChild.LeftChild.Key);
        VerifyLeaf(tree.LeftChild.RightChild.LeftChild.LeftChild);
        Assert.Null(tree.LeftChild.RightChild.LeftChild.RightChild);
        Assert.Equal(85, tree.LeftChild.RightChild.RightChild.Key);
        VerifyLeaf(tree.LeftChild.RightChild.RightChild);
        Assert.Equal(94, tree.RightChild.Key);
        Assert.Equal(92, tree.RightChild.LeftChild.Key);
        Assert.Equal(91, tree.RightChild.LeftChild.LeftChild.Key);
        VerifyLeaf(tree.RightChild.LeftChild.LeftChild);
        Assert.Equal(93, tree.RightChild.LeftChild.RightChild.Key);
        VerifyLeaf(tree.RightChild.LeftChild.RightChild);
        Assert.Equal(96, tree.RightChild.RightChild.Key);
        Assert.Equal(95, tree.RightChild.RightChild.LeftChild.Key);
        VerifyLeaf(tree.RightChild.RightChild.LeftChild);
        Assert.Equal(100, tree.RightChild.RightChild.RightChild.Key);
        Assert.Equal(98, tree.RightChild.RightChild.RightChild.LeftChild.Key);
        VerifyLeaf(tree.RightChild.RightChild.RightChild.LeftChild);
        Assert.Equal(110, tree.RightChild.RightChild.RightChild.RightChild.Key);
        VerifyLeaf(tree.RightChild.RightChild.RightChild.RightChild);

        Assert.Equal(Sixty, tree.LeftChild);

        tree = tree.Remove(60, CompareIntegers);
        tree = tree.Remove(92, CompareIntegers);
        tree = tree.Remove(91, CompareIntegers);
        tree = tree.Remove(98, CompareIntegers);
        tree = tree.Remove(85, CompareIntegers);
        tree = tree.Remove(110, CompareIntegers);
        tree = tree.Remove(93, CompareIntegers);
        tree = tree.Remove(50, CompareIntegers);
        tree = tree.Remove(96, CompareIntegers);
        tree = tree.Remove(70, CompareIntegers);
        tree = tree.Remove(94, CompareIntegers);

        Assert.Equal(90, tree.Key);
        Assert.Equal(80, tree.LeftChild.Key);
        Assert.Equal(65, tree.LeftChild.LeftChild.Key);
        VerifyLeaf(tree.LeftChild.LeftChild);
        Assert.Null(tree.LeftChild.RightChild);
        Assert.Equal(100, tree.RightChild.Key);
        Assert.Equal(95, tree.RightChild.LeftChild.Key);
        VerifyLeaf(tree.RightChild.LeftChild);
        Assert.Null(tree.RightChild.RightChild);

        tree = tree.Remove(90, CompareIntegers);
        tree = tree.Remove(65, CompareIntegers);
        tree = tree.Remove(100, CompareIntegers);
        tree = tree.Remove(95, CompareIntegers);
        tree = tree.Remove(80, CompareIntegers);

        Assert.Null(tree);
    }
    
    #endregion

    #region Tests insertion of key-value pairs in ascending order and validates the height and values of the tree.
    
    [Fact]
    public void Validate_AscendingOrderInsertionInVersioningTree()
    {
        const int treeSize = 10000;

        VersioningTree<int, int> tree = new VersioningTree<int, int>(0, 0, null, null);

        for (int i = 1; i < treeSize; i++)
        {
            tree = tree.SetKeyValue(i, i * 10, CompareIntegers);
        }

        Assert.Equal(18, tree.Height);

        for (int i = 0; i < treeSize; i++)
        {
            Assert.Equal(i * 10, tree.GetValue(i, CompareIntegers));
        }

        for (int i = 1; i < treeSize; i++)
        {
            tree = tree.SetKeyValue(i, i * 2, CompareIntegers);
        }

        Assert.Equal(18, tree.Height);

        for (int i = 0; i < treeSize; i++)
        {
            Assert.Equal(i * 2, tree.GetValue(i, CompareIntegers));
        }
    }
    
    #endregion

    #region Tests insertion of key-value pairs in descending order and validates the height and values of the tree.

    [Fact]
    public void Validate_DescendingOrderInsertionInVersioningTree()
    {
        const int treeSize = 10000;

        VersioningTree<int, int> tree = new VersioningTree<int, int>(0, 0, null, null);

        for (int i = treeSize - 1; i > 0; i--)
        {
            tree = tree.SetKeyValue(i, i * 10, CompareIntegers);
        }

        Assert.Equal(20, tree.Height);

        for (int i = 0; i < treeSize; i++)
        {
            Assert.Equal(i * 10, tree.GetValue(i, CompareIntegers));
        }

        for (int i = treeSize - 1; i > 0; i--)
        {
            tree = tree.SetKeyValue(i, i * 2, CompareIntegers);
        }

        Assert.Equal(20, tree.Height);

        for (int i = 0; i < treeSize; i++)
        {
            Assert.Equal(i * 2, tree.GetValue(i, CompareIntegers));
        }
    }
    
    #endregion

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

    private void VerifyLeaf(VersioningTree<int, int> tree)
    {
        Assert.Null(tree.LeftChild);
        Assert.Null(tree.RightChild);
    }
}
