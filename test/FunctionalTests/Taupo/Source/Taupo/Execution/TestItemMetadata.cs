//---------------------------------------------------------------------
// <copyright file="TestItemMetadata.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;
    
    /// <summary>
    /// Metadata for <see cref="TestItem" />.
    /// </summary>
    [Serializable]
    public class TestItemMetadata : IEquatable<TestItemMetadata>
    {
        /// <summary>
        /// Gets or sets id of the test item
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets name of the test item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets description of the test item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets owner of the test item
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Gets or sets priority of the test item
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the reason this test item is skipped. Only used when <see cref="SkipUntil"/> is set.
        /// </summary>
        public string SkipReason { get; set; }

        /// <summary>
        /// Gets or sets the date until which this test item should be skipped. After this date,
        /// the test item will throw an exception.
        /// </summary>
        public DateTime? SkipUntil { get; set; }

        /// <summary>
        /// Gets or sets version of the test item
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the <see cref="TestItemMetadata"/> instances are equal. Otherwise, false.</returns>
        public static bool operator ==(TestItemMetadata left, TestItemMetadata right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the <see cref="TestItemMetadata"/> instances are not equal. Otherwise, false.</returns>
        public static bool operator !=(TestItemMetadata left, TestItemMetadata right)
        {
            return !object.Equals(left, right);
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
            return this.Equals(obj as TestItemMetadata);
        }

        /// <summary>
        /// Determines whether the specified <see cref="TestItemMetadata"/> is equal to this instance.  
        /// </summary>
        /// <param name="other">The <see cref="TestItemMetadata"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="TestItemMetadata"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(TestItemMetadata other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id == other.Id && this.Name == other.Name && this.Description == other.Description;
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
            result = (37 * result) + this.Id.GetHashCode();
            result = (37 * result) + this.Name.GetHashCode();
            result = (37 * result) + this.Description.GetHashCode();

            return result;
        }
    }
}
