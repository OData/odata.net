//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmSilentNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Represents a navigation property synthesized for an association end that does not have a corresponding navigation property.
    /// </summary>
    internal class MetadataProviderEdmSilentNavigationProperty : EdmElement, IEdmNavigationProperty
    {
        /// <summary>The destination end of this navigation property.</summary>
        private readonly IEdmNavigationProperty partner;

        /// <summary>The type of the navigation property.</summary>
        private readonly IEdmTypeReference type;

        /// <summary>The on-delete action of the navigation property.</summary>
        private readonly EdmOnDeleteAction deleteAction;

        /// <summary>The name of this navigation property.</summary>
        private readonly string name;

        /// <summary>
        /// Creates a new Silent partner for a navigation property
        /// </summary>
        /// <param name="partnerProperty">The navigation property this is a silent partner of.</param>
        /// <param name="propertyDeleteAction">The on delete action for this side of the association</param>
        /// <param name="multiplicity">The multiplicity of this side of the association.</param>
        /// <param name="name">The name of this navigation property.</param>
        public MetadataProviderEdmSilentNavigationProperty(IEdmNavigationProperty partnerProperty, EdmOnDeleteAction propertyDeleteAction, EdmMultiplicity multiplicity, string name)
        {
            this.partner = partnerProperty;
            this.deleteAction = propertyDeleteAction;
            this.name = name;
            switch (multiplicity)
            {
                case EdmMultiplicity.One:
                    this.type = new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false);
                    break;
                case EdmMultiplicity.ZeroOrOne:
                    this.type = new EdmEntityTypeReference(this.partner.DeclaringEntityType(), true);
                    break;
                case EdmMultiplicity.Many:
                    this.type = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false)));
                    break;
            }
        }

        /// <summary>
        /// Gets the destination end of this navigation property.
        /// </summary>
        public IEdmNavigationProperty Partner
        {
            get { return this.partner; }
        }

        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        public EdmOnDeleteAction OnDelete
        {
            get { return this.deleteAction; }
        }

        /// <summary>
        /// Gets the referential constraint for this navigation, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        public IEdmReferentialConstraint ReferentialConstraint { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the kind of this property.
        /// </summary>
        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.partner.ToEntityType(); }
        }

        /// <summary>
        /// Gets the name of this element.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Dependent properties of this navigation property.
        /// </summary>
        /// <param name="properties">The dependent properties</param>
        internal void SetDependentProperties(IList<IEdmStructuralProperty> properties)
        {
            this.ReferentialConstraint = EdmReferentialConstraint.Create(properties, this.ToEntityType().Key());
        }
    }
}
