//---------------------------------------------------------------------
// <copyright file="AssociationEnd.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;

    /// <summary>
    /// Association (type) End
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationEnd : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the AssociationEnd class without a RoleName.
        /// </summary>
        public AssociationEnd()
            : this(null)
        { 
        }

        /// <summary>
        /// Initializes a new instance of the AssociationEnd class with a given RoleName.
        /// </summary>
        /// <param name="roleName">The RoleName for the association end</param>
        public AssociationEnd(string roleName)
        {           
            this.RoleName = roleName;
            this.DeleteBehavior = OperationAction.None;
        }

        /// <summary>
        /// Initializes a new instance of the AssociationEnd class with a given RoleName, EntityType, and multiplicity
        /// </summary>
        /// <param name="roleName">The RoleName for the association end</param>
        /// <param name="type">The EntityType this association end refers to</param>
        /// <param name="multiplicity">Mulitipliciy of this association end</param>
        public AssociationEnd(string roleName, EntityType type, EndMultiplicity multiplicity)
            : this(roleName)
        {
            this.EntityType = type;
            this.Multiplicity = multiplicity;
        }

        /// <summary>
        /// Initializes a new instance of the AssociationEnd class with a given RoleName, EntityType, multiplicity and OnDeleteAction
        /// </summary>
        /// <param name="roleName">The RoleName for the association end</param>
        /// <param name="type">The EntityType this association end refers to</param>
        /// <param name="multiplicity">Mulitipliciy of this association end</param>
        /// <param name="onDeleteAction">OnDelete value</param>
        public AssociationEnd(string roleName, EntityType type, EndMultiplicity multiplicity, OperationAction onDeleteAction)
            : this(roleName, type, multiplicity)
        {
            this.DeleteBehavior = onDeleteAction;
        }

        /// <summary>
        /// Gets or sets the role name of the association end.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// Gets or sets the EntityType of this association end.
        /// </summary>
        public EntityType EntityType { get; set; }

        /// <summary>
        /// Gets or sets the Multiplicity of this association end.
        /// </summary>
        public EndMultiplicity Multiplicity { get; set; }

        /// <summary>
        /// Gets or sets the OnDeleteAction Value of this association end.
        /// </summary>
        public OperationAction DeleteBehavior { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.AssociationEnd"/>.
        /// </summary>
        /// <param name="roleName">Name of the role.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator AssociationEnd(string roleName)
        {
            return new AssociationEndReference(roleName);
        }
    }
}
