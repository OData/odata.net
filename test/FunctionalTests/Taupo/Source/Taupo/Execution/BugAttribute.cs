//---------------------------------------------------------------------
// <copyright file="BugAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Execution
{
    using System;

    /// <summary>
    /// Indicates that a test class or variation is associated with a bug from a particular bug location.
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public abstract class BugAttribute : Attribute, IEquatable<BugAttribute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BugAttribute"/> class.
        /// </summary>
        /// <param name="server">The server that holds the bug.</param>
        /// <param name="product">The product or project on the <paramref name="server"/> where the bug can be found.</param>
        /// <param name="bugId">The ID of the bug.</param>
        protected BugAttribute(string server, string product, int bugId)
        {
            this.Server = server;
            this.Product = product;
            this.BugId = bugId;
        }

        /// <summary>
        /// Gets the ID of the bug.
        /// </summary>
        public int BugId { get; private set; }

        /// <summary>
        /// Gets or sets additional information about this bug
        /// in relation to failing variations.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the product or project on the server in which the bug can be found.
        /// </summary>
        public string Product { get; private set; }

        /// <summary>
        /// Gets the server that holds the bug.
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        /// When implemented in a derived class, gets a unique identifier for this <see cref="T:System.Attribute"/>.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.Object"/> that is a unique identifier for the attribute.</returns>
        public override object TypeId
        {
            get
            {
                return Tuple.Create(this.GetType(), this.Server, this.Product, this.BugId);
            }
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">An System.Object to compare with this instance or null.</param>
        /// <returns>true if obj equals the type and value of this instance; otherwise, false.</returns>
        public sealed override bool Equals(object obj)
        {
            return this.Equals(obj as BugAttribute);
        }

        /// <summary>
        /// Returns a value that indicates whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="other">Another <see cref="BugAttribute"/> to compare with this instance or null.</param>
        /// <returns>true if other equals the type and value of this instance; otherwise, false.</returns>
        public virtual bool Equals(BugAttribute other)
        {
            if (other == null)
            {
                return false;
            }

            if (other.GetType() != this.GetType())
            {
                return false;
            }

            return this.BugId == other.BugId && this.Product == other.Product && this.Server == other.Server && this.Description == other.Description;
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

            result = (result * 37) + this.GetType().GetHashCode();
            result = (result * 37) + this.BugId.GetHashCode();

            if (this.Product != null)
            {
                result = (result * 37) + this.Product.GetHashCode();
            }

            if (this.Server != null)
            {
                result = (result * 37) + this.Server.GetHashCode();
            }

            if (this.Description != null)
            {
                result = (result * 37) + this.Description.GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        /// <remarks>
        /// This was made abstract to force implementers to use specific descriptions
        /// to give information to testers about what bug affects a test case or variation.
        /// </remarks>
        public abstract override string ToString();
    }
}
