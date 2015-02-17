//---------------------------------------------------------------------
// <copyright file="NavigationPropertyInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents a navigation property
    /// </summary>
    public class NavigationPropertyInstance : PropertyInstance
    {
        /// <summary>
        /// Private storage for value
        /// </summary>
        private ODataPayloadElement currentValue;

        /// <summary>
        /// Initializes a new instance of the NavigationPropertyInstance class
        /// </summary>
        public NavigationPropertyInstance()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NavigationPropertyInstance class
        /// Will automatically wrap entity sets and entity instances in expanded links
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="value">The property value</param>
        public NavigationPropertyInstance(string name, ODataPayloadElement value)
            : base(name)
        {
            if (value != null)
            {
                if (value.ElementType == ODataPayloadElementType.EntityInstance || value.ElementType == ODataPayloadElementType.EntitySetInstance)
                {
                    value = new ExpandedLink() { ExpandedElement = value };
                }
            }

            this.Value = value;
        }

        /// <summary>
        /// Initializes a new instance of the NavigationPropertyInstance class
        /// </summary>
        /// <param name="name">The property name</param>
        /// <param name="value">The navigation link value</param>
        /// <param name="associationLink">The ralationship link value</param>
        public NavigationPropertyInstance(string name, ODataPayloadElement value, DeferredLink associationLink)
            : this(name, value)
        {
            this.AssociationLink = associationLink;
        }

        /// <summary>
        /// Gets or sets the value of the navigation property
        /// </summary>
        public ODataPayloadElement Value
        {
            get
            {
                return this.currentValue;
            }

            set
            {
                // Only allow deferred links, deferred link collections, and expanded links to be assigned
                if (value != null)
                {
                    switch (value.ElementType)
                    {
                        case ODataPayloadElementType.LinkCollection:
                        case ODataPayloadElementType.DeferredLink:
                        case ODataPayloadElementType.ExpandedLink:
                            break;

                        default:
                            throw new TaupoInvalidOperationException("Navigation property cannot be assigned a value of type: " + value.ElementType);
                    }

                    this.currentValue = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the property relationship link value
        /// </summary>
        public DeferredLink AssociationLink { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the navigation property's value is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                ExceptionUtilities.CheckObjectNotNull(this.Value, "Cannot check IsExpanded when value is null");
                return this.Value.ElementType == ODataPayloadElementType.ExpandedLink;
            }
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.Value == null)
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}:null", this.Name);
                }
                else
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Name, this.Value.StringRepresentation);
                }
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        /// <returns>The result of visiting this expression.</returns>
        public override TResult Accept<TResult>(IODataPayloadElementVisitor<TResult> visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that does not return a result.
        /// </summary>
        /// <param name="visitor">The visitor that is visiting this element.</param>
        public override void Accept(IODataPayloadElementVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
