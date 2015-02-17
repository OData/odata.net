//---------------------------------------------------------------------
// <copyright file="NamedStreamAtomLinkMetadataAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Common.Annotations
{
    #region Namespaces
    using System.Globalization;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    #endregion Namespaces

    /// <summary>
    /// Represents the atom:link values that may be ascribed to a named stream link.
    /// </summary>
    public class NamedStreamAtomLinkMetadataAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Gets or sets the link's href value.
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Gets or sets the link's hreflang value.
        /// </summary>
        public string HrefLang { get; set; }

        /// <summary>
        /// Gets or sets the link's length value.
        /// </summary>
        public string Length { get; set; }

        /// <summary>
        /// Gets or sets the link's relation value.
        /// </summary>
        public string Relation { get; set; }
        
        /// <summary>
        /// Gets or sets the link's title value.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the link's type value.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation.
        /// </summary>
        /// <param name="other">The annotation to compare to.</param>
        /// <returns>True if the annotations are equivalent, false otherwise.</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            return this.CastAndCheckEquality<NamedStreamAtomLinkMetadataAnnotation>(
                other,
                (a) => a.Href == this.Href && 
                       a.HrefLang == this.HrefLang && 
                       a.Length == this.Length && 
                       a.Relation == this.Relation && 
                       a.Title == this.Title && 
                       a.Type == this.Type);
        }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier.
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "Link: Href={0} HrefLang={1} Length={2} Relation={3} Title={4} Type={5}",
                    this.Href ?? "<null>",
                    this.HrefLang ?? "<null>",
                    this.Length ?? "<null>",
                    this.Relation ?? "<null>",
                    this.Title ?? "<null>",
                    this.Type ?? "<null>");
            }
        }

        /// <summary>
        /// Creates a clone of the annotation.
        /// </summary>
        /// <returns>A clone of the annotation.</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            return new NamedStreamAtomLinkMetadataAnnotation
            {
                Href = this.Href,
                HrefLang = this.HrefLang,
                Length = this.Length,
                Relation = this.Relation,
                Title = this.Title,
                Type = this.Type,
            };
        }
    }
}
