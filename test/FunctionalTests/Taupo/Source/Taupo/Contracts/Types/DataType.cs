//---------------------------------------------------------------------
// <copyright file="DataType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Abstract data type with nullability information.
    /// </summary>
    public abstract class DataType : IEquatable<DataType>, IAnnotatable<IDataTypeAnnotation>
    {
        /// <summary>
        /// Initializes a new instance of the DataType class.
        /// </summary>
        /// <param name="isNullable">If set to <c>true</c> the type is nullable.</param>
        protected DataType(bool isNullable)
        {
            this.IsNullable = isNullable;
            this.Annotations = new List<IDataTypeAnnotation>();
        }

        /// <summary>
        /// Gets a list of all available Annotations on the DataType
        /// </summary>
        public IList<IDataTypeAnnotation> Annotations { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is nullable.
        /// </summary>
        /// <value>
        /// A value of <c>true</c> if this instance is nullable; otherwise, <c>false</c>.
        /// </value>
        public bool IsNullable { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as DataType;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Asserts that the given value compatible is compatible with the type and throws exception if it is not.
        /// </summary>
        /// <param name="value">The value.</param>
        public void AssertValueCompatible(object value)
        {
            if (value == null)
            {
                if (!this.IsNullable)
                {
                    throw new TaupoInvalidOperationException("Value null is not compatible with " + this + " because it is not nullable.");
                }

                return;
            }

            if (!this.IsValueCompatible(value))
            {
                throw new TaupoInvalidOperationException("Value " + ToStringConverter.ConvertObjectToString(value) + " is not compatible with " + this + ".");
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override abstract int GetHashCode();

        /// <summary>
        /// Determines whether the specified <see cref="DataType"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="DataType"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="DataType"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Equals(DataType other);

        /// <summary>
        /// Accepts the specified visitor by calling its Visit method.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="visitor">The visitor.</param>
        /// <returns>Visitor-specific value.</returns>
        public abstract TValue Accept<TValue>(IDataTypeVisitor<TValue> visitor);

        /// <summary>
        /// Determines whether the specified value is compatible with the type.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A value of <c>true</c> if the value is compatible with the type; otherwise <c>false</c>.
        /// </returns>
        protected abstract bool IsValueCompatible(object value);
    }
}
