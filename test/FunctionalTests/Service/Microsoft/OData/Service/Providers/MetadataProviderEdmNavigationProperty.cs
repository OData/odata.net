//---------------------------------------------------------------------
// <copyright file="MetadataProviderEdmNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Providers
{
    #region Namespaces

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;
    using s = Microsoft.OData.Service;
    #endregion Namespaces

    /// <summary>
    /// An <see cref="IEdmNavigationProperty"/> implementation supporting setting of the association end.
    /// </summary>
    internal sealed class MetadataProviderEdmNavigationProperty : EdmElement, IEdmNavigationProperty, IResourcePropertyBasedEdmProperty
    {
        /// <summary>The declaring type of the navigation property.</summary>
        private readonly IEdmEntityType declaringType;

        /// <summary>The name of the navigation property.</summary>
        private readonly string name;

        /// <summary>The type of the navigation property.</summary>
        private IEdmTypeReference type;

        /// <summary>The destination end of this navigation property.</summary>
        private IEdmNavigationProperty partner;

        /// <summary>The destination end of this navigation property.</summary>
        private bool isPrinciple;

        /// <summary>The on-delete action of the navigation property.</summary>
        private EdmOnDeleteAction deleteAction;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="declaringType">The declaring type of the navigation property.</param>
        /// <param name="property">The resource property to create the navigation property from.</param>
        /// <param name="type">The type of the navigation property.</param>
        /// <remarks>This constructor assumes that the entity set for this service operation has already be created.</remarks>
        internal MetadataProviderEdmNavigationProperty(EdmEntityType declaringType, ResourceProperty property, IEdmTypeReference type)
        {
            Debug.Assert(declaringType != null, "declaringType != null");
            Debug.Assert(property != null, "property != null");
            Debug.Assert(type != null, "type != null");

            this.declaringType = declaringType;
            this.name = property.Name;
            this.type = type;
            this.ResourceProperty = property;
        }

        /// <summary>
        /// The <see cref="ResourceProperty"/> this edm property was created from.
        /// </summary>
        public ResourceProperty ResourceProperty { get; private set; }

        /// <summary>
        /// Gets the destination end of this navigation property.
        /// </summary>
        public IEdmNavigationProperty Partner
        {
            get 
            {
                return this.partner; 
            }
        }

        /// <summary>
        /// Gets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        public EdmOnDeleteAction OnDelete 
        {
            get
            {
                return this.deleteAction;
            }
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
        /// Gets the type of this property.
        /// </summary>
        public IEdmTypeReference Type
        {
            get { return this.type; }
            internal set { this.type = value; }
        }

        /// <summary>
        /// Gets the type that this property belongs to.
        /// </summary>
        public IEdmStructuredType DeclaringType
        {
            get { return this.declaringType; }
        }

        /// <summary>
        /// Gets the name of this navigation property.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Matches the navigation property with its partner and fills in missing information.
        /// </summary>
        /// <param name="partnerProperty">The navigation property that corresponds to the opposite end of this navigation properties association.</param>
        /// <param name="isPrincipleEnd">A value indicating whether this navigation property is on the principle end of a referential constraint.</param>
        /// <param name="propertyDeleteAction">The action to execute on the deletion of this end of a bidirectional association.</param>
        internal void FixUpNavigationProperty(IEdmNavigationProperty partnerProperty, bool isPrincipleEnd, EdmOnDeleteAction propertyDeleteAction)
        {
            this.partner = partnerProperty;
            this.isPrinciple = isPrincipleEnd;
            this.deleteAction = propertyDeleteAction;
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
