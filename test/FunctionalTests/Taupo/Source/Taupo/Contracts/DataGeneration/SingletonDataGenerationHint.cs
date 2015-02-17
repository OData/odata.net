//---------------------------------------------------------------------
// <copyright file="SingletonDataGenerationHint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DataGeneration
{
    /// <summary>
    /// Base class for singleton data generation hints.
    /// </summary>
    public abstract class SingletonDataGenerationHint : DataGenerationHint
    {
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
            if (other == null)
            {
                return false;
            }

            return this.GetType() == other.GetType();
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }
    }
}
