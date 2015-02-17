//---------------------------------------------------------------------
// <copyright file="AssociationSet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Association set
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class AssociationSet : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the AssociationSet class without a name or a type.
        /// </summary>
        public AssociationSet()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssociationSet class with given name but without a type.
        /// </summary>
        /// <param name="name">Name of association set.</param>
        public AssociationSet(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the AssociationSet class with given name and type.
        /// </summary>
        /// <param name="name">Name of association set.</param>
        /// <param name="associationType">Type of association set elements.</param>
        public AssociationSet(string name, AssociationType associationType)
        {
            this.Name = name;
            this.AssociationType = associationType;
            this.Ends = new List<AssociationSetEnd>();
        }

        /// <summary>
        /// Gets or sets AssociationSet name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets type of association instances belonging to the set.
        /// </summary>
        public AssociationType AssociationType { get; set; }

        /// <summary>
        /// Gets a list of AssociationSet ends.
        /// </summary>
        public IList<AssociationSetEnd> Ends { get; private set; }

        /// <summary>
        /// Gets the entity container this set belongs to. If it is null, then the set has not been added to any container.
        /// </summary>
        public EntityContainer Container { get; internal set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.AssociationSet"/>.
        /// </summary>
        /// <param name="associationSetName">Name of the association set.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator AssociationSet(string associationSetName)
        {
            return new AssociationSetReference(associationSetName);
        }

        /// <summary>
        /// Add an AssociationSetEnd to Ends collection.
        /// </summary>
        /// <param name="end">association set end to be added</param>
        public void Add(AssociationSetEnd end)
        {
            this.Ends.Add(end);
        }

        /// <summary>
        /// Gets the other end of the association set.
        /// </summary>
        /// <param name="end">The end for which to get other end.</param>
        /// <returns>The other end of the association set.</returns>
        public AssociationSetEnd GetOtherEnd(AssociationSetEnd end)
        {
            return this.Ends.Where(e => e != end).Single();
        }
    }
}
