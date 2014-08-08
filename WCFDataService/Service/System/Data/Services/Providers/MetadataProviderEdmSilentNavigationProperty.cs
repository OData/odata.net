//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Library;

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

        /// <summary>The dependent properties of the referential constraint.</summary>
        private ReadOnlyCollection<IEdmStructuralProperty> dependentProperties;

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
                    this.type = new EdmCollectionTypeReference(new EdmCollectionType(new EdmEntityTypeReference(this.partner.DeclaringEntityType(), false)), false);
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
        /// Gets whether this navigation property originates at the principal end of an association.
        /// </summary>
        public bool IsPrincipal
        {
            get { return this.partner.DependentProperties != null; }
        }

        /// <summary>
        /// Gets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the dependent properties of this navigation property, returning null if this is the principal end or if there is no referential constraint.
        /// </summary>
        public IEnumerable<IEdmStructuralProperty> DependentProperties
        {
            get
            {
                return this.dependentProperties;
            }
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
            this.dependentProperties = new ReadOnlyCollection<IEdmStructuralProperty>(properties);
        }
    }
}
