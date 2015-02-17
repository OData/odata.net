//---------------------------------------------------------------------
// <copyright file="RelationshipSide.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    /// <summary>
    /// Represents one side of a <see cref="RelationshipType"/>, and contains multiple
    /// pieces of information, such as the multiplicity, the <see cref="FromNavigationProperty"/>
    /// represented by this side (if any), and the <see cref="FromEntityType"/>
    /// associated with this side of the relationship.
    /// </summary>
    public class RelationshipSide
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipSide"/> class.
        /// </summary>
        /// <param name="associationSetEnd">The <see cref="FromAssociationSetEnd"/> which describes this side of the relationship.</param>
        /// <param name="relationshipType">The parent <see cref="RelationshipType"/>.</param>
        internal RelationshipSide(AssociationSetEnd associationSetEnd, RelationshipType relationshipType)
        {
            this.FromAssociationSetEnd = associationSetEnd;
            this.FromAssociationEnd = associationSetEnd.AssociationEnd;
            this.FromEntitySet = associationSetEnd.EntitySet;
            this.FromEntityType = this.FromAssociationEnd.EntityType;
            this.FromDeleteBehavior = this.FromAssociationEnd.DeleteBehavior;
            this.FromMultiplicity = this.FromAssociationEnd.Multiplicity;
            this.FromNavigationProperty = this.FromAssociationEnd.FromNavigationProperty();
            this.FromRoleName = this.FromAssociationEnd.RoleName;

            this.RelationshipType = relationshipType;
        }

        /// <summary>
        /// Gets the <see cref="AssociationEnd"/> at this side of the relationship.
        /// </summary>
        public AssociationEnd FromAssociationEnd { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssociationSetEnd"/> at this side of the relationship.
        /// </summary>
        public AssociationSetEnd FromAssociationSetEnd { get; private set; }

        /// <summary>
        /// Gets the behavior taken by the product when the entity instance at
        /// this side of the relationship is deleted.
        /// </summary>
        public OperationAction FromDeleteBehavior { get; private set; }

        /// <summary>
        /// Gets the <see cref="EntitySet"/> that the <see cref="EntityType"/>
        /// on this side of the relationship is in.
        /// </summary>
        public EntitySet FromEntitySet { get; private set; }

        /// <summary>
        /// Gets the <see cref="EntityType"/> that participates in this side of
        /// the relationship.
        /// </summary>
        public EntityType FromEntityType { get; private set; }

        /// <summary>
        /// Gets the multiplicity at this side of the relationship.
        /// </summary>
        /// <remarks>
        /// This multiplicity specifies the "how many" that the other side
        /// has of this side. For example, if this side represented a Customer
        /// and the other side represents an Order in a 1:* relationship, this
        /// side's multiplicity would be One.
        /// </remarks>
        public EndMultiplicity FromMultiplicity { get; private set; }

        /// <summary>
        /// Gets the <see cref="NavigationProperty"/> that starts at this side
        /// of the relationship and navigates to the other <see cref="RelationshipSide"/>.
        /// </summary>
        public NavigationProperty FromNavigationProperty { get; private set; }

        /// <summary>
        /// Gets the name of the role that this side of the relationship plays.
        /// </summary>
        public string FromRoleName { get; private set; }

        /// <summary>
        /// Gets the other <see cref="RelationshipSide"/> in the same <see cref="RelationshipType"/>.
        /// </summary>
        public RelationshipSide OtherSide { get; internal set; }

        /// <summary>
        /// Gets the parent <see cref="RelationshipType"/> for this <see cref="RelationshipSide"/>.
        /// </summary>
        public RelationshipType RelationshipType { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssociationEnd"/> at the other side of the relationship.
        /// </summary>
        public AssociationEnd ToAssociationEnd
        {
            get { return this.OtherSide.FromAssociationEnd; }
        }

        /// <summary>
        /// Gets the <see cref="AssociationSetEnd"/> at the other side of the relationship.
        /// </summary>
        public AssociationSetEnd ToAssociationSetEnd
        {
            get { return this.OtherSide.FromAssociationSetEnd; }
        }

        /// <summary>
        /// Gets the behavior taken by the product when the entity instance at
        /// the other side of the relationship is deleted.
        /// </summary>
        public OperationAction ToDeleteBehavior
        {
            get { return this.OtherSide.FromDeleteBehavior; }
        }

        /// <summary>
        /// Gets the <see cref="EntitySet"/> that the <see cref="EntityType"/>
        /// on the other side of the relationship is in.
        /// </summary>
        public EntitySet ToEntitySet
        {
            get { return this.OtherSide.FromEntitySet; }
        }

        /// <summary>
        /// Gets the <see cref="EntityType"/> that participates in the other side of
        /// the relationship.
        /// </summary>
        public EntityType ToEntityType
        {
            get { return this.OtherSide.FromEntityType; }
        }

        /// <summary>
        /// Gets the multiplicity at the other side of the relationship.
        /// </summary>
        /// <remarks>
        /// This multiplicity specifies the "how many" that this side
        /// has of the other side. For example, if this side represented a Customer
        /// and the other side represents an Order in a 1:* relationship, this
        /// side's multiplicity would be Many.
        /// </remarks>
        public EndMultiplicity ToMultiplicity
        {
            get { return this.OtherSide.FromMultiplicity; }
        }

        /// <summary>
        /// Gets the <see cref="NavigationProperty"/> that starts at the other side
        /// of the relationship and navigates to the this <see cref="RelationshipSide"/>.
        /// </summary>
        public NavigationProperty ToNavigationProperty
        {
            get { return this.OtherSide.FromNavigationProperty; }
        }

        /// <summary>
        /// Gets the name of the role that the other side of the relationship plays.
        /// </summary>
        public string ToRoleName
        {
            get { return this.OtherSide.FromRoleName; }
        }
    }
}
