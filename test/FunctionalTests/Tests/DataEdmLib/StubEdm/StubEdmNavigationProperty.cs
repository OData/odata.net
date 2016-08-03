//---------------------------------------------------------------------
// <copyright file="StubEdmNavigationProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.StubEdm
{
    using Microsoft.OData.Edm;

    /// <summary>
    /// Stub implementation of EdmNavigationProperty
    /// </summary>
    public class StubEdmNavigationProperty : StubEdmElement, IEdmNavigationProperty
    {
        private IEdmNavigationProperty partner;

        /// <summary>
        /// Initializes a new instance of the StubEdmNavigationProperty class.
        /// </summary>
        /// <param name="name">the name of the navigation property</param>
        public StubEdmNavigationProperty(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the navigation target is contained inside the navigation source.
        /// </summary>
        public bool ContainsTarget { get; set; }

        /// <summary>
        /// Gets or sets the type
        /// </summary>
        public IEdmTypeReference Type { get; set; }

        /// <summary>
        /// Gets or sets the declaring type
        /// </summary>
        public IEdmStructuredType DeclaringType { get; set; }

        /// <summary>
        /// Gets the property kind
        /// </summary>
        public EdmPropertyKind PropertyKind
        {
            get { return EdmPropertyKind.Navigation; }
        }

        /// <summary>
        /// Gets or sets the Partner navigation property.
        /// </summary>
        public IEdmNavigationProperty Partner
        {
            get
            {
                return this.partner;
            }

            set
            {
                this.partner = value;
                if (value.Partner != this)
                {
                    ((StubEdmNavigationProperty)value).Partner = this;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to execute on the deletion of this end of a bidirectional association.
        /// </summary>
        public EdmOnDeleteAction OnDelete { get; set; }

        /// <summary>
        /// Gets or sets the referential constraint for this navigation property.
        /// </summary>
        public IEdmReferentialConstraint ReferentialConstraint { get; set; }
    }
}
