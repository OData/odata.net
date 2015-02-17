//---------------------------------------------------------------------
// <copyright file="NavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Navigation property for navigating an association.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    [System.Diagnostics.DebuggerDisplay("NavigationProperty Name={this.Name} Association={this.Association.Name} From={this.FromAssociationEnd.RoleName} To={this.ToAssociationEnd.RoleName}")]
    public class NavigationProperty : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the NavigationProperty class without a name.
        /// </summary>
        public NavigationProperty()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NavigationProperty class with given name.
        /// </summary>
        /// <param name="name">Navigation property name</param>
        public NavigationProperty(string name)
            : this(name, null, null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NavigationProperty class.
        /// </summary>
        /// <param name="navigationPropertyName">Name of the navigation property.</param>
        /// <param name="association">The association.</param>
        /// <param name="fromAssociationEnd">From association end.</param>
        /// <param name="toAssociationEnd">To association end.</param>
        public NavigationProperty(string navigationPropertyName, AssociationType association, AssociationEnd fromAssociationEnd, AssociationEnd toAssociationEnd)
        {
            this.Name = navigationPropertyName;
            this.Association = association;
            this.FromAssociationEnd = fromAssociationEnd;
            this.ToAssociationEnd = toAssociationEnd;
        }

        /// <summary>
        /// Gets or sets navigation property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Association.
        /// </summary>
        public AssociationType Association { get; set; }

        /// <summary>
        /// Gets or sets the association end corresponding to "FromRole"
        /// </summary>
        public AssociationEnd FromAssociationEnd { get; set; }

        /// <summary>
        /// Gets or sets the association end corresponding to the "ToRole"
        /// </summary>
        public AssociationEnd ToAssociationEnd { get; set; }
    }
}
