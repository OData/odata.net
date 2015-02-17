//---------------------------------------------------------------------
// <copyright file="DataGenerationHint`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Base class for data generation hints that have a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public abstract class DataGenerationHint<TValue> : DataGenerationHint
    {
        private int? hashCode;
        
        /// <summary>
        /// Initializes a new instance of the DataGenerationHint class.
        /// </summary>
        /// <param name="value">The value of the hint.</param>
        protected DataGenerationHint(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value of this hint.
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DataGenerationHint);
        }

        /// <summary>
        /// Determines whether this data generation hint is equal to other data generation hint.
        /// </summary>
        /// <param name="other">The other data generation hint.</param>
        /// <returns>True if this <see cref="DataGenerationHint"/> is equal to the <paramref name="other"/>, false otherwise.</returns>
        public override bool Equals(DataGenerationHint other)
        {
            if (other == null || other.GetType() != this.GetType())
            {
                return false;
            }

            DataGenerationHint<TValue> otherHintWithValue = (DataGenerationHint<TValue>)other;

            return ValueComparer.Instance.Equals(this.Value, otherHintWithValue.Value);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            if (!this.hashCode.HasValue)
            {
                this.hashCode = this.GetType().GetHashCode() ^ ValueComparer.Instance.GetHashCode(this.Value);
            }

            return this.hashCode.Value;
        }

        /// <summary>
        /// Returns a string representation of this <see cref="DataGenerationHint"/>, for use in debugging and logging.
        /// </summary>
        /// <returns>String representation of this <see cref="DataGenerationHint"/>.</returns>
        public override string ToString()
        {
            return this.GetType().Name + " = " + this.Value;
        }
    }
}
