//---------------------------------------------------------------------
// <copyright file="NullPropertyInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// Represents a null property instance that may be primitive, complex, or a navigation
    /// </summary>
    public class NullPropertyInstance : PropertyInstance
    {
        /// <summary>
        /// Initializes a new instance of the NullPropertyInstance class
        /// </summary>
        public NullPropertyInstance()
            : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NullPropertyInstance class
        /// </summary>
        /// <param name="name">The property's name</param>
        /// <param name="fullTypeName">The property's type name</param>
        public NullPropertyInstance(string name, string fullTypeName)
            : base(name)
        {
            this.FullTypeName = fullTypeName;
        }

        /// <summary>
        /// Gets or sets the type name for the null property
        /// </summary>
        public string FullTypeName { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.Name, this.ElementType);
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
