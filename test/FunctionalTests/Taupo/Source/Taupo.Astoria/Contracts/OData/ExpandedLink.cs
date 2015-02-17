//---------------------------------------------------------------------
// <copyright file="ExpandedLink.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents an expanded link
    /// </summary>
    public class ExpandedLink : Link
    {
        /// <summary>
        /// Private storage for the expanded element so that the setter can validate the new value
        /// </summary>
        private ODataPayloadElement expanded = null;

        /// <summary>
        /// Initializes a new instance of the ExpandedLink class
        /// </summary>
        public ExpandedLink()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ExpandedLink class
        /// </summary>
        /// <param name="expanded">The expanded element</param>
        public ExpandedLink(ODataPayloadElement expanded)
            : base()
        {
            this.ExpandedElement = expanded;
        }

        /// <summary>
        /// Gets or sets the expanded element
        /// </summary>
        public ODataPayloadElement ExpandedElement
        {
            get
            {
                return this.expanded;
            }

            set
            {
                // Only allow entity sets or entities to be assigned
                if (value != null)
                {
                    switch (value.ElementType)
                    {
                        case ODataPayloadElementType.EntitySetInstance:
                        case ODataPayloadElementType.EntityInstance:
                            break;

                        default:
                            throw new TaupoInvalidOperationException("Expanded link cannot contain payload element of type: " + value.ElementType);
                    }
                }
                
                this.expanded = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the expanded element is a single entity
        /// </summary>
        public bool IsSingleEntity
        {
            get
            {
                return this.ExpandedElement.ElementType == ODataPayloadElementType.EntityInstance;
            }
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                if (this.ExpandedElement == null)
                {
                    return this.ElementType.ToString();
                }

                return this.ExpandedElement.StringRepresentation;
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
