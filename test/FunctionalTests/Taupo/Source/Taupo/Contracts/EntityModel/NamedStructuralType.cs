//---------------------------------------------------------------------
// <copyright file="NamedStructuralType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Abstract class to represent structural type which has name
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public abstract class NamedStructuralType : StructuralType, INamedItem
    {
        /// <summary>
        /// Initializes a new instance of the NamedStructuralType class.
        /// </summary>
        /// <param name="namespaceName">Type's namespace name.</param>
        /// <param name="name">Type's name.</param>
        protected NamedStructuralType(string namespaceName, string name)
        {
            this.Name = name;
            this.NamespaceName = namespaceName;
        }

        /// <summary>
        /// Gets or sets type name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets type namespace.
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Gets the type's full name which includes NamespaceName and Name separated by dot. 
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
        /// Gets the model this type belongs to. If it is null, then the type has not been added to a model.
        /// </summary>
        public EntityModelSchema Model { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is abstract (instances of it cannot be created)
        /// </summary>
        public bool IsAbstract { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is open (can have more properties than declared)
        /// </summary>
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets all properties, including the ones defined in all its BaseTypes.
        /// </summary>
        public IEnumerable<MemberProperty> AllProperties
        {
            get
            {
                if (this.GetBaseTypeInternal() == null)
                {
                    return this.Properties;
                }
                else
                {
                    return this.GetBaseTypeInternal().AllProperties.Concat(this.Properties);
                }
            }
        }

        /// <summary>
        /// Gets the name of the base type (for debugging purposes)
        /// </summary>
        /// <value>The name of the base type.</value>
        internal string BaseTypeName
        {
            get
            {
                if (this.GetBaseTypeInternal() == null)
                {
                    return string.Empty;
                }
                else
                {
                    return this.GetBaseTypeInternal().FullName;
                }
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

        /// <summary>
        /// Gets the base type from which this type derives from.
        /// </summary>
        /// <returns>The base type of this type.</returns>
        protected abstract NamedStructuralType GetBaseTypeInternal();
    }
}
