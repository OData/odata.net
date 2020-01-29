//---------------------------------------------------------------------
// <copyright file="IEdmReferentialConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an EDM referential constraint on a navigation property.
    /// </summary>
    public interface IEdmReferentialConstraint
    {
        /// <summary>
        /// Gets the set of property pairs from the referential constraint. No particular ordering should be assumed.
        /// </summary>
        IEnumerable<EdmReferentialConstraintPropertyPair> PropertyPairs { get; }
    }

    /// <summary>
    /// Represents a pair of properties as part of a referential constraint.
    /// </summary>
    public class EdmReferentialConstraintPropertyPair
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EdmReferentialConstraintPropertyPair"/>.
        /// </summary>
        /// <param name="dependentProperty">The local or dependent property in the referential constraint property pair. A null value is not allowed.</param>
        /// <param name="principalProperty">The foreign or principal property in the referential constraint property pair. A null value is not allowed.</param>
        /// <exception cref="System.ArgumentNullException">Thrown if either of the properties given are null.</exception>
        public EdmReferentialConstraintPropertyPair(IEdmStructuralProperty dependentProperty, IEdmStructuralProperty principalProperty)
        {
            EdmUtil.CheckArgumentNull(dependentProperty, "dependentProperty");
            EdmUtil.CheckArgumentNull(principalProperty, "principalProperty");

            this.DependentProperty = dependentProperty;
            this.PrincipalProperty = principalProperty;
        }

        /// <summary>
        /// The local or dependent property in the referential constraint property pair. Will never be null.
        /// </summary>
        public IEdmStructuralProperty DependentProperty { get; private set; }

        /// <summary>
        /// The foreign or principal property in the referential constraint property pair. Will never be null.
        /// </summary>
        public IEdmStructuralProperty PrincipalProperty { get; private set; }
    }
}