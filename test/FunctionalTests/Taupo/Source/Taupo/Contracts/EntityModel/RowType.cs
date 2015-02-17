//---------------------------------------------------------------------
// <copyright file="RowType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;
    
    /// <summary>
    /// RowType (type)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class RowType : StructuralType, IEquatable<RowType>
    {
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 0x1234;

            foreach (var p in this.Properties)
            {
                hashCode ^= p.PropertyType.GetHashCode();
            }

            return hashCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as RowType;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="RowType"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="RowType"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="RowType"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(RowType other)
        {
            if (this.Properties.Count != other.Properties.Count)
            {
                return false;
            }

            for (int i = 0; i < this.Properties.Count; i++)
            {
                if (!this.Properties[i].PropertyType.Equals(other.Properties[i].PropertyType))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
