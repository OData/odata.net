//---------------------------------------------------------------------
// <copyright file="RelationshipType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Serves as an abstraction for associations in the EDM which includes
    /// navigation properties, association metadata, and dependent/principal information.
    /// </summary>
    public class RelationshipType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipType"/> class.
        /// </summary>
        /// <param name="associationSet">The <see cref="AssociationSet"/> to wrap.</param>
        internal RelationshipType(AssociationSet associationSet)
        {
            this.AssociationSet = associationSet;
            this.AssociationType = associationSet.AssociationType;
            this.ReferentialConstraint = this.AssociationType.ReferentialConstraint;
            this.IsIdentifyingRelationship = this.ReferentialConstraint != null &&
                                             this.ReferentialConstraint.DependentProperties.All(p => p.IsPrimaryKey);
            var sides = this.AssociationSet.Ends.Select(e => new RelationshipSide(e, this)).ToList();

            sides[0].OtherSide = sides[1];
            sides[1].OtherSide = sides[0];

            this.Sides = sides.AsReadOnly();

            this.DetermineMultiplicity();

            if (this.ReferentialConstraint != null)
            {
                this.DependentSide = this.Side(rs => rs.FromAssociationEnd == this.ReferentialConstraint.DependentAssociationEnd);
                this.PrincipalSide = this.Side(rs => rs.FromAssociationEnd == this.ReferentialConstraint.PrincipalAssociationEnd);

                this.DependentProperties = this.ReferentialConstraint.DependentProperties.ToList().AsReadOnly();
                this.PrincipalProperties = this.ReferentialConstraint.PrincipalProperties.ToList().AsReadOnly();
            }
            else
            {
                this.DependentProperties = new List<MemberProperty>().AsReadOnly();
                this.PrincipalProperties = new List<MemberProperty>().AsReadOnly();
            }
        }

        /// <summary>
        /// Gets the <see cref="AssociationSet"/> associated with this <see cref="RelationshipType"/>.
        /// </summary>
        public AssociationSet AssociationSet { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssociationType"/> associated with this <see cref="RelationshipType"/>.
        /// </summary>
        public AssociationType AssociationType { get; private set; }

        /// <summary>
        /// Gets a sequence of <see cref="MemberProperty"/> instances that serve as the dependent
        /// properties in the relationship. Returns an empty collection if this <see cref="RelationshipType"/>
        /// instance does not have a <see cref="ReferentialConstraint"/>.
        /// </summary>
        public ReadOnlyCollection<MemberProperty> DependentProperties { get; private set; }

        /// <summary>
        /// Gets the dependent <see cref="RelationshipSide"/> in this relationship. Returns null
        /// if this <see cref="RelationshipType"/> instance does not have a <see cref="ReferentialConstraint"/>.
        /// </summary>
        public RelationshipSide DependentSide { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RelationshipType"/> is an identifying relationship;
        /// that is, whether the dependent's foreign key(s) is(are) also part of the dependent's primary key(s).
        /// </summary>
        public bool IsIdentifyingRelationship { get; private set; }

        /// <summary>
        /// Gets the multiplicity pattern of this relationship, which describes
        /// the multiplicities on both sides.
        /// </summary>
        public MultiplicityPattern MultiplicityPattern { get; private set; }

        /// <summary>
        /// Gets a sequence of <see cref="MemberProperty"/> instances that serve as the principal
        /// properties in the relationship. Returns an empty collection if this <see cref="RelationshipType"/>
        /// instance does not have a <see cref="ReferentialConstraint"/>.
        /// </summary>
        public ReadOnlyCollection<MemberProperty> PrincipalProperties { get; private set; }

        /// <summary>
        /// Gets the principal <see cref="RelationshipSide"/> in this relationship. Returns null
        /// if this <see cref="RelationshipType"/> instance does not have a <see cref="ReferentialConstraint"/>.
        /// </summary>
        public RelationshipSide PrincipalSide { get; private set; }

        /// <summary>
        /// Gets the <see cref="ReferentialConstraint"/> in this relationship.
        /// </summary>
        public ReferentialConstraint ReferentialConstraint { get; private set; }

        /// <summary>
        /// Gets a collection containing the two sides of this relationship.
        /// </summary>
        public ReadOnlyCollection<RelationshipSide> Sides { get; private set; }

        /// <summary>
        /// Gets the single <see cref="RelationshipSide"/> that matches the
        /// specified predicate, if it exists.
        /// </summary>
        /// <param name="predicate">The predicate used to choose a specific side.</param>
        /// <returns>A <see cref="RelationshipSide"/> which matches the specified
        /// predicate. May return null.</returns>
        public RelationshipSide Side(Func<RelationshipSide, bool> predicate)
        {
            return this.Sides.SingleOrDefault(predicate);
        }

        private void DetermineMultiplicity()
        {
            var end1Multiplicity = this.AssociationType.Ends[0].Multiplicity;
            var end2Multiplicity = this.AssociationType.Ends[1].Multiplicity;

            if (end1Multiplicity == EndMultiplicity.Many)
            {
                if (end2Multiplicity == EndMultiplicity.Many)
                {
                    this.MultiplicityPattern = MultiplicityPattern.ManyToMany;
                }
                else if (end2Multiplicity == EndMultiplicity.One)
                {
                    this.MultiplicityPattern = MultiplicityPattern.ManyToOne;
                }
                else
                {
                    this.MultiplicityPattern = MultiplicityPattern.ManyToZeroOne;
                }
            }
            else if (end1Multiplicity == EndMultiplicity.One)
            {
                if (end2Multiplicity == EndMultiplicity.Many)
                {
                    this.MultiplicityPattern = MultiplicityPattern.ManyToOne;
                }
                else if (end2Multiplicity == EndMultiplicity.One)
                {
                    this.MultiplicityPattern = MultiplicityPattern.OneToOne;
                }
                else
                {
                    this.MultiplicityPattern = MultiplicityPattern.OneToZeroOne;
                }
            }
            else
            {
                if (end2Multiplicity == EndMultiplicity.Many)
                {
                    this.MultiplicityPattern = MultiplicityPattern.ManyToZeroOne;
                }
                else if (end2Multiplicity == EndMultiplicity.One)
                {
                    this.MultiplicityPattern = MultiplicityPattern.OneToZeroOne;
                }
                else
                {
                    this.MultiplicityPattern = MultiplicityPattern.ZeroOneToZeroOne;
                }
            }
        }
    }
}
