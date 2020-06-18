//---------------------------------------------------------------------
// <copyright file="CompositeKey.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Spatial
{
    using System;

    /// <summary>
    /// A key consisting of multiple fields
    /// </summary>
    /// <typeparam name="T1">The type of the first field.</typeparam>
    /// <typeparam name="T2">The type of the second field.</typeparam>
    internal class CompositeKey<T1, T2> : IEquatable<CompositeKey<T1, T2>>
    {
        /// <summary>
        /// The first field
        /// </summary>
        private readonly T1 first;

        /// <summary>
        /// The second field
        /// </summary>
        private readonly T2 second;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeKey&lt;T1, T2&gt;"/> class.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        public CompositeKey(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(CompositeKey<T1, T2> left, CompositeKey<T1, T2> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(CompositeKey<T1, T2> left, CompositeKey<T1, T2> right)
        {
            return !Equals(left, right);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(CompositeKey<T1, T2> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.first, this.first) && Equals(other.second, this.second);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            return Equals(obj as CompositeKey<T1, T2>);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.first.GetHashCode() * 397) ^ this.second.GetHashCode();
            }
        }
    }
}