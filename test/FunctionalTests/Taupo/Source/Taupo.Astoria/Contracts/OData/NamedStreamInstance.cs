//---------------------------------------------------------------------
// <copyright file="NamedStreamInstance.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Globalization;

    /// <summary>
    /// Represents a named stream instance
    /// </summary>
    public class NamedStreamInstance : PropertyInstance
    {   
        /// <summary>
        /// Initializes a new instance of the NamedStreamInstance class.  strictly used for code coverage and unit tests
        /// </summary>
        public NamedStreamInstance()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NamedStreamInstance class
        /// </summary>
        /// <param name="name">the name of stream</param>
        public NamedStreamInstance(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Gets or sets the content type for the edit link of the stream
        /// </summary>
        public string EditLinkContentType { get; set; }

        /// <summary>
        /// Gets or sets content type for the source link of the stream
        /// </summary>
        public string SourceLinkContentType { get; set; }

        /// <summary>
        /// Gets or sets the the source link for the stream
        /// </summary>
        public string SourceLink { get; set; }

        /// <summary>
        /// Gets or sets the edit link for the stream
        /// </summary>
        public string EditLink { get; set; }

        /// <summary>
        /// Gets or sets the etag of the named stream
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}:NamedStream", this.Name);
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
