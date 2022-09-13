//---------------------------------------------------------------------
// <copyright file="VersioningTreeTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalTests
{
    using Microsoft.OData.Edm;
    using Microsoft.Test.OData.Utils.Metadata;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VersioningTreeTests : EdmLibTestCaseBase
    {
        public VersioningTreeTests()
        {
            this.EdmVersion = EdmVersion.V40;
        }

        [TestMethod]
        public void VersioningTreeBasicInsertion()
        {
            VersioningTree<int, int> tree = new VersioningTree<int, int>(100, 1000, null, null);

            tree = tree.SetKeyValue(90, 900, CompareIntegers);
            tree = tree.SetKeyValue(110, 1100, CompareIntegers);

            VerifyKey(tree, 100);
            VerifyKey(tree.LeftChild, 90);
            VerifyKey(tree.RightChild, 110);

            tree = tree.SetKeyValue(80, 800, CompareIntegers);
            tree = tree.SetKeyValue(85, 850, CompareIntegers);

            VerifyKey(tree, 90);
            VerifyKey(tree.RightChild, 100);
            VerifyKey(tree.RightChild.RightChild, 110);
            VerifyKey(tree.LeftChild, 80);
            VerifyKey(tree.LeftChild.RightChild, 85);

            var OneHundred = tree.RightChild;

            tree = tree.SetKeyValue(70, 700, CompareIntegers);
            tree = tree.SetKeyValue(60, 600, CompareIntegers);
            tree = tree.SetKeyValue(50, 500, CompareIntegers);
            tree = tree.SetKeyValue(80, 800, CompareIntegers);

            VerifyKey(tree, 80);
            VerifyKey(tree.LeftChild, 60);
            VerifyKey(tree.LeftChild.LeftChild, 50);
            VerifyKey(tree.LeftChild.RightChild, 70);
            VerifyKey(tree.RightChild, 90);
            VerifyKey(tree.RightChild.LeftChild, 85);
            VerifyKey(tree.RightChild.RightChild, 100);
            VerifyKey(tree.RightChild.RightChild.RightChild, 110);

            // Test that the 100 node has not been copied.
            Assert.AreEqual(OneHundred, tree.RightChild.RightChild, "Tree nodes EQ");

            var Sixty = tree.LeftChild;

            tree = tree.SetKeyValue(96, 960, CompareIntegers);
            tree = tree.SetKeyValue(98, 980, CompareIntegers);

            VerifyKey(tree, 90);
            VerifyKey(tree.LeftChild, 80);
            VerifyKey(tree.LeftChild.LeftChild, 60);
            VerifyKey(tree.LeftChild.LeftChild.LeftChild, 50);
            VerifyKey(tree.LeftChild.LeftChild.RightChild, 70);
            VerifyKey(tree.LeftChild.RightChild, 85);
            VerifyKey(tree.RightChild, 100);
            VerifyKey(tree.RightChild.LeftChild, 96);
            VerifyKey(tree.RightChild.LeftChild.RightChild, 98);
            VerifyKey(tree.RightChild.RightChild, 110);

            // Test that the 60 node has not been copied.
            Assert.AreEqual(Sixty, tree.LeftChild.LeftChild, "Tree nodes EQ");

            var Fifty = tree.LeftChild.LeftChild.LeftChild;
            OneHundred = tree.RightChild;

            tree = tree.SetKeyValue(65, 650, CompareIntegers);

            VerifyKey(tree, 90);
            VerifyKey(tree.LeftChild, 60);
            VerifyKey(tree.LeftChild.LeftChild, 50);
            VerifyLeaf(tree.LeftChild.LeftChild);
            VerifyKey(tree.LeftChild.RightChild, 80);
            VerifyKey(tree.LeftChild.RightChild.LeftChild, 70);
            VerifyKey(tree.LeftChild.RightChild.LeftChild.LeftChild, 65);
            Assert.IsNull(tree.LeftChild.RightChild.LeftChild.RightChild, "Null child");
            VerifyKey(tree.LeftChild.RightChild.RightChild, 85);
            VerifyKey(tree.RightChild, 100);
            VerifyKey(tree.RightChild.LeftChild, 96);
            Assert.IsNull(tree.RightChild.LeftChild.LeftChild, "Null child");
            VerifyKey(tree.RightChild.LeftChild.RightChild, 98);
            VerifyKey(tree.RightChild.RightChild, 110);
            VerifyLeaf(tree.RightChild.RightChild);

            Assert.AreEqual(Fifty, tree.LeftChild.LeftChild, "Tree nodes EQ");
            Assert.AreEqual(OneHundred, tree.RightChild, "Tree nodes EQ");

            Sixty = tree.LeftChild;

            tree = tree.SetKeyValue(95, 950, CompareIntegers);
            tree = tree.SetKeyValue(94, 940, CompareIntegers);
            tree = tree.SetKeyValue(93, 930, CompareIntegers);
            tree = tree.SetKeyValue(93, 930, CompareIntegers);
            tree = tree.SetKeyValue(92, 920, CompareIntegers);
            tree = tree.SetKeyValue(91, 910, CompareIntegers);

            VerifyKey(tree, 90);
            VerifyKey(tree.LeftChild, 60);
            VerifyKey(tree.LeftChild.LeftChild, 50);
            VerifyLeaf(tree.LeftChild.LeftChild);
            VerifyKey(tree.LeftChild.RightChild, 80);
            VerifyKey(tree.LeftChild.RightChild.LeftChild, 70);
            VerifyKey(tree.LeftChild.RightChild.LeftChild.LeftChild, 65);
            VerifyLeaf(tree.LeftChild.RightChild.LeftChild.LeftChild);
            Assert.IsNull(tree.LeftChild.RightChild.LeftChild.RightChild, "Null child");
            VerifyKey(tree.LeftChild.RightChild.RightChild, 85);
            VerifyLeaf(tree.LeftChild.RightChild.RightChild);
            VerifyKey(tree.RightChild, 94);
            VerifyKey(tree.RightChild.LeftChild, 92);
            VerifyKey(tree.RightChild.LeftChild.LeftChild, 91);
            VerifyLeaf(tree.RightChild.LeftChild.LeftChild);
            VerifyKey(tree.RightChild.LeftChild.RightChild, 93);
            VerifyLeaf(tree.RightChild.LeftChild.RightChild);
            VerifyKey(tree.RightChild.RightChild, 96);
            VerifyKey(tree.RightChild.RightChild.LeftChild, 95);
            VerifyLeaf(tree.RightChild.RightChild.LeftChild);
            VerifyKey(tree.RightChild.RightChild.RightChild, 100);
            VerifyKey(tree.RightChild.RightChild.RightChild.LeftChild, 98);
            VerifyLeaf(tree.RightChild.RightChild.RightChild.LeftChild);
            VerifyKey(tree.RightChild.RightChild.RightChild.RightChild, 110);
            VerifyLeaf(tree.RightChild.RightChild.RightChild.RightChild);

            Assert.AreEqual(Sixty, tree.LeftChild, "Tree nodes EQ");

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

            VerifyKey(tree, 90);
            VerifyKey(tree.LeftChild, 80);
            VerifyKey(tree.LeftChild.LeftChild, 65);
            VerifyLeaf(tree.LeftChild.LeftChild);
            Assert.IsNull(tree.LeftChild.RightChild, "Null child");
            VerifyKey(tree.RightChild, 100);
            VerifyKey(tree.RightChild.LeftChild, 95);
            VerifyLeaf(tree.RightChild.LeftChild);
            Assert.IsNull(tree.RightChild.RightChild, "Null child");

            tree = tree.Remove(90, CompareIntegers);
            tree = tree.Remove(65, CompareIntegers);
            tree = tree.Remove(100, CompareIntegers);
            tree = tree.Remove(95, CompareIntegers);
            tree = tree.Remove(80, CompareIntegers);

            Assert.IsNull(tree, "Null tree");
        }

        [TestMethod]
        public void VersioningTreeAscendingInsertion()
        {
            const int treeSize = 10000;

            VersioningTree<int, int> tree = new VersioningTree<int, int>(0, 0, null, null);

            for (int i = 1; i < treeSize; i++)
            {
                tree = tree.SetKeyValue(i, i * 10, CompareIntegers);
            }

            VerifyHeight(tree, 18);

            for (int i = 0; i < treeSize; i++)
            {
                Assert.AreEqual(i * 10, tree.GetValue(i, CompareIntegers), "Node value");
            }

            for (int i = 1; i < treeSize; i++)
            {
                tree = tree.SetKeyValue(i, i * 2, CompareIntegers);
            }

            VerifyHeight(tree, 18);

            for (int i = 0; i < treeSize; i++)
            {
                Assert.AreEqual(i * 2, tree.GetValue(i, CompareIntegers), "Node value");
            }
        }

        [TestMethod]
        public void VersioningTreeDescendingInsertion()
        {
            const int treeSize = 10000;

            VersioningTree<int, int> tree = new VersioningTree<int, int>(0, 0, null, null);

            for (int i = treeSize - 1; i > 0; i--)
            {
                tree = tree.SetKeyValue(i, i * 10, CompareIntegers);
            }

            VerifyHeight(tree, 20);

            for (int i = 0; i < treeSize; i++)
            {
                Assert.AreEqual(i * 10, tree.GetValue(i, CompareIntegers), "Node value");
            }

            for (int i = treeSize - 1; i > 0; i--)
            {
                tree = tree.SetKeyValue(i, i * 2, CompareIntegers);
            }

            VerifyHeight(tree, 20);

            for (int i = 0; i < treeSize; i++)
            {
                Assert.AreEqual(i * 2, tree.GetValue(i, CompareIntegers), "Node value");
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

        private void VerifyHeight(VersioningTree<int, int> tree, int expectedHeight)
        {
            Assert.AreEqual(expectedHeight, tree.Height, "Node height");
        }

        private void VerifyKey(VersioningTree<int, int> tree, int expectedKey)
        {
            Assert.AreEqual(expectedKey, tree.Key, "Node key");
        }

        private void VerifyLeaf(VersioningTree<int, int> tree)
        {
            Assert.IsNull(tree.LeftChild, "Leaf child");
            Assert.IsNull(tree.RightChild, "Leaf child");
        }
    }
}
