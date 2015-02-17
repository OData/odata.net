//---------------------------------------------------------------------
// <copyright file="RelationshipTypeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// An annotation applied to <see cref="AssociationSet"/>s, which caches the
    /// information retrieved by the <see cref="GlobalExtensionMethods.RelationshipType"/>
    /// extension method.
    /// </summary>
    internal sealed class RelationshipTypeAnnotation : Annotation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelationshipTypeAnnotation"/> class.
        /// </summary>
        /// <param name="relationshipType">The <see cref="RelationshipType"/> associated with the
        /// <see cref="AssociationSet"/> to which this <see cref="RelationshipTypeAnnotation"/> is applied.</param>
        public RelationshipTypeAnnotation(RelationshipType relationshipType)
        {
            ExceptionUtilities.Assert(relationshipType != null, "The Relationship argument to the RelationshipAnnotation constructor should never be null.");
            this.RelationshipType = relationshipType;
        }

        /// <summary>
        /// Gets the <see cref="RelationshipType"/> object cached for a particular <see cref="AssociationSet"/>.
        /// </summary>
        public RelationshipType RelationshipType { get; private set; }
    }
}
