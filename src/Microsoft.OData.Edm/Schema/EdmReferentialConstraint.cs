//---------------------------------------------------------------------
// <copyright file="EdmReferentialConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents an EDM referential constraint on a navigation property.
    /// </summary>
    public class EdmReferentialConstraint : IEdmReferentialConstraint
    {
        private readonly IEnumerable<EdmReferentialConstraintPropertyPair> propertyPairs;

        /// <summary>
        /// Initializes a new instance of <see cref="EdmReferentialConstraint"/>.
        /// </summary>
        /// <param name="propertyPairs">The set of property pairs from the referential constraint.</param>
        public EdmReferentialConstraint(IEnumerable<EdmReferentialConstraintPropertyPair> propertyPairs)
        {
            EdmUtil.CheckArgumentNull(propertyPairs, "propertyPairs");
            this.propertyPairs = propertyPairs.ToList();
        }

        /// <summary>
        /// Gets the set of property pairs from the referential constraint. No particular ordering should be assumed.
        /// </summary>
        public IEnumerable<EdmReferentialConstraintPropertyPair> PropertyPairs
        {
            get { return this.propertyPairs; }
        }

        /// <summary>
        /// Creates a new <see cref="EdmReferentialConstraint"/> using the provided <paramref name="dependentProperties"/> and <paramref name="principalProperties"/> to form the pairs.
        /// </summary>
        /// <param name="dependentProperties">The dependent properties that participate in the referential constraint. Assumed to be in the correct order relative to the principal entity's properties.</param>
        /// <param name="principalProperties">The principal properties that participate in the referential constraint. Assumed to be in the correct order relative to the dependent entity's properties.</param>
        /// <returns>The newly created referential constraint.</returns>
        /// <exception cref="System.ArgumentException">Thrown if the number of dependent properties given does not match the number of key properties in the principal entity type.</exception>
        public static EdmReferentialConstraint Create(IEnumerable<IEdmStructuralProperty> dependentProperties, IEnumerable<IEdmStructuralProperty> principalProperties)
        {
            EdmUtil.CheckArgumentNull(dependentProperties, "dependentProperties");
            EdmUtil.CheckArgumentNull(principalProperties, "principalProperties");

            var dependentPropertyList = new List<IEdmStructuralProperty>(dependentProperties);
            var principalPropertyList = new List<IEdmStructuralProperty>(principalProperties);
            if (dependentPropertyList.Count != principalPropertyList.Count)
            {
                throw new ArgumentException(Strings.Constructable_DependentPropertyCountMustMatchNumberOfPropertiesOnPrincipalType(principalPropertyList.Count, dependentPropertyList.Count));
            }

            return new EdmReferentialConstraint(dependentPropertyList.Select((d, i) => new EdmReferentialConstraintPropertyPair(d, principalPropertyList[i])));
        }
    }
}