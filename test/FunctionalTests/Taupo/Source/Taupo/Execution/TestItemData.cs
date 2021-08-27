//---------------------------------------------------------------------
// <copyright file="TestItemData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// A facade over <see cref="TestItem"/> that exposes only data
    /// about the item, instead of a way to execute it.
    /// </summary>
    public class TestItemData : MarshalByRefObject
    {
        private string stringRepresentation;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestItemData"/> class.
        /// </summary>
        /// <param name="testItem">The test item whose data is exposed.</param>
        public TestItemData(TestItem testItem)
        {
            ExceptionUtilities.CheckArgumentNotNull(testItem, "testItem");

            this.ExplorationSeed = testItem.ExplorationSeed;
            this.ExplorationKind = testItem.ExplorationKind;
            this.Children = testItem.Children.Select(ti => new TestItemData(ti, this)).ToList().AsReadOnly();
            this.Bugs = testItem.Bugs;
            this.IsTestCase = testItem is TestCase;
            this.IsVariation = testItem is VariationTestItem;
            this.Metadata = testItem.Metadata;
            this.TestItemType = testItem.GetType();
            this.stringRepresentation = testItem.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestItemData"/> class.
        /// </summary>
        /// <param name="testItem">The test item whose data is exposed.</param>
        /// <param name="parent">The parent of this <see cref="TestItemData"/>, if it exists.</param>
        public TestItemData(TestItem testItem, TestItemData parent) :
            this(testItem)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets all of the bugs associated with this <see cref="TestItemData"/>.
        /// </summary>
        public Collection<BugAttribute> Bugs { get; private set; }

        /// <summary>
        /// Gets collection of child items (test cases and variations) created dynamically based 
        /// on [TestCase] and [TestVariation] attributes.
        /// </summary>
        public ReadOnlyCollection<TestItemData> Children { get; private set; }

        /// <summary>
        /// Gets the seed to use when loading children with <see cref="TestMatrixAttribute"/>.
        /// </summary>
        public int ExplorationSeed { get; private set; }

        /// <summary>
        /// Gets the exploration kind to use when loading children with <see cref="TestMatrixAttribute"/>.
        /// If the value is null, exploration kind specified on the <see cref="TestMatrixAttribute"/> is used.
        /// If the value is not null it'll override the exploration kind specified on the <see cref="TestMatrixAttribute"/>.
        /// </summary>
        public TestMatrixExplorationKind? ExplorationKind { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TestItemData"/>
        /// represents a test case.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance represents a test case; otherwise, <c>false</c>.
        /// </value>
        public bool IsTestCase { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="TestItemData"/>
        /// represents a variation.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance represents a variation; otherwise, <c>false</c>.
        /// </value>
        public bool IsVariation { get; private set; }

        /// <summary>
        /// Gets the metadata for the test item.
        /// </summary>
        public TestItemMetadata Metadata { get; private set; }

        /// <summary>
        /// Gets the parent of this <see cref="TestItemData"/> instance.
        /// </summary>
        public TestItemData Parent { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type"/> of the test item.
        /// </summary>
        public Type TestItemType { get; private set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the <see cref="TestItemData"/> instances are equal. Otherwise, false.</returns>
        public static bool operator ==(TestItemData left, TestItemData right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the <see cref="TestItemData"/> instances are not equal. Otherwise, false.</returns>
        public static bool operator !=(TestItemData left, TestItemData right)
        {
            return !object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified <see cref="TestItemData"/> instance
        /// matches the specified <see cref="TestItem"/>. Matching, in this case,
        /// means that both objects have the same metadata, and all of their
        /// parents have the same metadata.
        /// </summary>
        /// <param name="testItemData">The <see cref="TestItemData"/> to compare with the <see cref="TestItem"/>.</param>
        /// <param name="other">The <see cref="TestItem"/> to consider.</param>
        /// <returns>
        /// true if this <see cref="TestItemData"/> matches the specified
        /// <see cref="TestItem"/>. Otherwise, false.
        /// </returns>
        public static bool Matches(TestItemData testItemData, TestItem other)
        {
            if (testItemData == null && other == null)
            {
                return true;
            }

            if ((testItemData == null) != (other == null))
            {
                return false;
            }

            if (testItemData.TestItemType != other.GetType())
            {
                return false;
            }

            if (!testItemData.Metadata.Equals(other.Metadata))
            {
                return false;
            }

            if (object.ReferenceEquals(testItemData.Parent, null) != (other.Parent == null))
            {
                return false;
            }

            if (testItemData.Parent != null)
            {
                return Matches(testItemData.Parent, other.Parent);
            }

            return true;
        }

        /// <summary>
        /// Returns a sequence containing this test item and all children (recursive, using pre-order walk).
        /// </summary>
        /// <returns>Sequence of <see cref="TestItem"/>.</returns>
        public IEnumerable<TestItemData> GetAllChildrenRecursive()
        {
            return new TestItemData[1] { this }
                .Concat(this.Children.SelectMany(c => c.GetAllChildrenRecursive()))
                .ToArray();
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the lifetime policy for this instance. This is the current lifetime service object for this instance if one exists; otherwise, a new lifetime service object initialized to the value of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        [SecurityCritical]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as TestItemData);
        }

        /// <summary>
        /// Determines whether the specified <see cref="TestItemData"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="TestItemData"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="TestItemData"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(TestItemData other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (this.GetType() != other.GetType())
            {
                return false;
            }

            if (object.ReferenceEquals(this.Parent, null) != object.ReferenceEquals(other.Parent, null))
            {
                return false;
            }

            if (object.ReferenceEquals(this.Parent, null))
            {
                return this.Metadata.Equals(other.Metadata);
            }

            return this.Metadata.Equals(other.Metadata) &&
                this.Parent.Equals(other.Parent);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int result = 17;
            result = (result * 37) + this.Metadata.GetHashCode();

            if (this.Parent != null)
            {
                result = (result * 37) + this.Parent.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Returns a string representation of the test item.
        /// </summary>
        /// <returns>Item Description</returns>
        public override string ToString()
        {
            return this.stringRepresentation;
        }
    }
}
