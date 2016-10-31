//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Providers
{
    #region Namespaces
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Annotations;
    using Microsoft.Data.Edm.Library;
    using Microsoft.Data.Edm.Library.Annotations;
    using s = System.Data.Services;
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

        /// <summary>The dependent properties of the referential constraint.</summary>
        private ReadOnlyCollection<IEdmStructuralProperty> dependentProperties;

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
                if (this.partner == null)
                {
                    throw new InvalidOperationException(s.Strings.MetadataSerializer_NoResourceAssociationSetForNavigationProperty(this.name, this.declaringType.FullName()));
                }

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
        /// Gets whether this navigation property originates at the principal end of an association.
        /// </summary>
        public bool IsPrincipal 
        {
            get
            {
                return this.isPrinciple;
            }
        }

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
            this.dependentProperties = new ReadOnlyCollection<IEdmStructuralProperty>(properties);
        }
    }
}
