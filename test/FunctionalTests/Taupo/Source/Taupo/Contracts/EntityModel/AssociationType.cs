//---------------------------------------------------------------------
// <copyright file="AssociationType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Association (Type)
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationType : NamedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the AssociationType class without name.
        /// </summary>
        public AssociationType()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssociationType class with given name and null namespace.
        /// </summary>
        /// <param name="name">Name of the association</param>
        public AssociationType(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssociationType class with given namespace and name.
        /// </summary>
        /// <param name="namespaceName">Namespace of the association</param>
        /// <param name="name">Name of the association</param>
        public AssociationType(string namespaceName, string name)
            : base(namespaceName, name)
        {
            this.Ends = new List<AssociationEnd>();
        }

        /// <summary>
        /// Gets association ends of the Association Type
        /// </summary>
        public IList<AssociationEnd> Ends { get; private set; }

        /// <summary>
        /// Gets or sets ReferentialConstraint
        /// </summary>
        public ReferentialConstraint ReferentialConstraint { get; set; }

        /// <summary>
        /// Gets the model this type belongs to. If it is null, then the type has not been added to a model.
        /// </summary>
        public EntityModelSchema Model { get; internal set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.AssociationType"/>.
        /// </summary>
        /// <param name="associationTypeName">Name of the association type.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator AssociationType(string associationTypeName)
        {
            return new AssociationTypeReference(associationTypeName);
        }

        /// <summary>
        /// Add an AssociationEnd to the <see cref="Ends"/> collection.
        /// </summary>
        /// <param name="end">the AssociationEnd to be added</param>
        public void Add(AssociationEnd end)
        {
            this.Ends.Add(end);
        }

        /// <summary>
        /// Adds the specified referential constraint.
        /// </summary>
        /// <param name="referentialConstraint">The referential constraint.</param>
        public void Add(ReferentialConstraint referentialConstraint)
        {
            if (this.ReferentialConstraint != null)
            {
                throw new TaupoInvalidOperationException("This association set already has a referential constraint.");
            }

            this.ReferentialConstraint = referentialConstraint;
        }

        /// <summary>
        /// Gets the other end of the association type.
        /// </summary>
        /// <param name="end">The end for which to get other end.</param>
        /// <returns>The other end of the association type.</returns>
        public AssociationEnd GetOtherEnd(AssociationEnd end)
        {
            return this.Ends.Where(e => e != end).Single();
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
            var otherAssociation = other as AssociationType;
            if (otherAssociation == null)
            {
                return false;
            }

            return (this.Name == otherAssociation.Name) && (this.NamespaceName == otherAssociation.NamespaceName);
        }
    }
}
