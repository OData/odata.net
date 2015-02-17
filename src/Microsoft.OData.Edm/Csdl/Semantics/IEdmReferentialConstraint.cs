//---------------------------------------------------------------------
// <copyright file="IEdmReferentialConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Csdl.Internal.CsdlSemantics
{
    /// <summary>
    /// Represents an EDM referential constraint in an association type.
    /// </summary>
    internal interface IEdmReferentialConstraint : IEdmElement
    {
        /// <summary>
        /// Gets the principal end of this referential constraint.
        /// </summary>
        IEdmAssociationEnd PrincipalEnd { get; }

        /// <summary>
        /// Gets the dependent properties of this referential constraint. (The principal properties of the constraint are the key of the principal end)
        /// </summary>
        IEnumerable<IEdmStructuralProperty> DependentProperties { get; }
    }
}
