//---------------------------------------------------------------------
// <copyright file="ComplexType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Complex Type
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    [System.Diagnostics.DebuggerDisplay("ComplexType: {this.NamespaceName}.{this.Name} Properties={this.Properties.Count}")]
    public class ComplexType : NamedStructuralType
    {
        /// <summary>
        /// Initializes a new instance of the ComplexType class.
        /// </summary>
        public ComplexType()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexType class with given name and null namespace.
        /// </summary>
        /// <param name="name">Complex type name</param>
        public ComplexType(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ComplexType class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Complex type namespace</param>
        /// <param name="name">Complex type name</param>
        public ComplexType(string namespaceName, string name)
            : base(namespaceName, name)
        {
        }

        /// <summary>
        /// Gets or sets the ComplexType that this complex type derives from.
        /// </summary>
        public ComplexType BaseType { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.ComplexType"/>.
        /// </summary>
        /// <param name="complexTypeName">Name of the complex type.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator ComplexType(string complexTypeName)
        {
            return new ComplexTypeReference(complexTypeName);
        }

        /// <summary>
        /// Determines whether the specified <see cref="INamedItem"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="INamedItem"/> to compare with this instance.</param>
        /// <returns>
        /// A value of <c>true</c> if the specified <see cref="INamedItem"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(INamedItem other)
        {
            var otherComplex = other as ComplexType;
            if (otherComplex == null)
            {
                return false;
            }

            return (this.Name == otherComplex.Name) && (this.NamespaceName == otherComplex.NamespaceName);
        }

        /// <summary>
        /// Determines whether the complex type is or derives from the specified complex type.
        /// </summary>
        /// <param name="complexType">Complex Type to check against.</param>
        /// <returns>
        /// A value of <c>true</c> if this complex type is or derives from the specified complex type; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKindOf(ComplexType complexType)
        {
            for (ComplexType thisType = this; thisType != null; thisType = thisType.BaseType)
            {
                if (thisType == complexType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the root <see cref="ComplexType"/> in the type hierarchy.
        /// </summary>
        /// <returns>The root <see cref="ComplexType"/>.</returns>
        public ComplexType GetRootType()
        {
            ComplexType root = this;
            while (root.BaseType != null)
            {
                root = root.BaseType;
            }

            return root;
        }

        /// <summary>
        /// Gets the base type from which this type derives from.
        /// </summary>
        /// <returns>The base type of this type.</returns>
        protected override NamedStructuralType GetBaseTypeInternal()
        {
            return this.BaseType;
        }
    }
}
