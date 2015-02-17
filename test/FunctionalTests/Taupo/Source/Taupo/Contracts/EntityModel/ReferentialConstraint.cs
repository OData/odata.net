//---------------------------------------------------------------------
// <copyright file="ReferentialConstraint.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Referential constraint
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class ReferentialConstraint : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the ReferentialConstraint class.
        /// </summary>
        public ReferentialConstraint() 
        {
            this.PrincipalProperties = new List<MemberProperty>();
            this.DependentProperties = new List<MemberProperty>();
        }

        /// <summary>
        /// Gets or sets the association end corresponding to the Principal
        /// </summary>
        public AssociationEnd PrincipalAssociationEnd { get; set; }

        /// <summary>
        /// Gets a list of the (pk) properties on principal entity type
        /// </summary>
        public IList<MemberProperty> PrincipalProperties { get; private set; }

        /// <summary>
        /// Gets or sets the association end corresponding to the Dependent
        /// </summary>
        public AssociationEnd DependentAssociationEnd { get; set; }

        /// <summary>
        /// Gets a list of the (fk) properties on dependent entity type
        /// </summary>
        public IList<MemberProperty> DependentProperties { get; private set; }

        /// <summary>
        /// Helper to easily establish Dependent (by properties)
        /// </summary>
        /// <param name="dependentEnd">the dependent end of the association</param>
        /// <param name="properties">the (fk) properties on Dependent</param>
        /// <returns>this RerentialConstraint</returns>
        public ReferentialConstraint WithDependentProperties(AssociationEnd dependentEnd, params MemberProperty[] properties)
        {
            this.DependentAssociationEnd = dependentEnd;
            
            foreach (MemberProperty p in properties)
            {
                this.DependentProperties.Add(p);
            }

            return this;
        }

        /// <summary>
        /// Helper to easily establish Principal (by properties)
        /// </summary>
        /// <param name="principalEnd">The principal end.</param>
        /// <param name="properties">the (pk) properties on Principal</param>
        /// <returns>this RerentialConstraint</returns>
        public ReferentialConstraint ReferencesPrincipalProperties(AssociationEnd principalEnd, params MemberProperty[] properties)
        {
            this.PrincipalAssociationEnd = principalEnd;
            
            foreach (MemberProperty p in properties)
            {
                this.PrincipalProperties.Add(p);
            }

            return this;
        }
    }
}
