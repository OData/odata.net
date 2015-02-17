//---------------------------------------------------------------------
// <copyright file="AssociationSetEnd.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;

    /// <summary>
    /// Association set end
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationSetEnd : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the AssociationSetEnd class.
        /// </summary>
        public AssociationSetEnd()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssociationSetEnd class.
        /// </summary>
        /// <param name="associationEnd">The association end.</param>
        /// <param name="entitySet">The entity set.</param>
        public AssociationSetEnd(AssociationEnd associationEnd, EntitySet entitySet)
        {
            this.EntitySet = entitySet;
            this.AssociationEnd = associationEnd;
        }

        /// <summary>
        /// Gets or sets the EntitySet that this association set end refers to.
        /// </summary>
        public EntitySet EntitySet { get; set; }

        /// <summary>
        /// Gets or sets the corresponding association (type) end 
        /// </summary>
        public AssociationEnd AssociationEnd { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.AssociationSetEnd"/>.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator AssociationSetEnd(string roleName)
        {
            return new AssociationSetEndReference(roleName);
        }
    }
}
