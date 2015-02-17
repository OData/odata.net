//---------------------------------------------------------------------
// <copyright file="NamedItem.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Abstract class to represent item that has name
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public abstract class NamedItem : AnnotatedItem, INamedItem
    {
        /// <summary>
        /// Initializes a new instance of the NamedItem class.
        /// </summary>
        /// <param name="namespaceName">Item's namespace name.</param>
        /// <param name="name">Item's name.</param>
        protected NamedItem(string namespaceName, string name)
        {
            this.Name = name;
            this.NamespaceName = namespaceName;
        }

        /// <summary>
        /// Gets or sets item name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets item namespace.
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Gets the item's full name which includes NamespaceName and Name separated by dot. 
        /// </summary>
        public string FullName
        {
            get
            {
                string fullName = string.Empty;

                if (!string.IsNullOrEmpty(this.NamespaceName))
                {
                    fullName = this.NamespaceName + ".";
                }

                fullName += this.Name;

                return fullName;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int hashCode = 0x1234;

            if (this.Name != null)
            {
                hashCode ^= this.Name.GetHashCode();
            }

            if (this.NamespaceName != null)
            {
                hashCode ^= this.NamespaceName.GetHashCode();
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
            INamedItem other = obj as INamedItem;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Determines where the specified <see cref="INamedItem"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The other <see cref="INamedItem"/>.</param>
        /// <returns>A value of <c>true</c> if the specified <see cref="INamedItem"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public abstract bool Equals(INamedItem other);
    }
}
