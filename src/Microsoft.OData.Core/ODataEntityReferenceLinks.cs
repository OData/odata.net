//---------------------------------------------------------------------
// <copyright file="ODataEntityReferenceLinks.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;

    #endregion Namespaces

    /// <summary>
    /// Represents a collection of entity reference links (the result of a $ref query).
    /// Might include an inline count and a next link.
    /// </summary>
    public sealed class ODataEntityReferenceLinks : ODataAnnotatable
    {
        /// <summary>Gets or sets the optional inline count of the $ref collection.</summary>
        /// <returns>The optional inline count of the $ref collection.</returns>
        public long? Count
        {
            get;
            set;
        }

        /// <summary>Gets or sets the optional next link of the $ref collection.</summary>
        /// <returns>The optional next link of the $ref collection.</returns>
        public Uri NextPageLink
        {
            get;
            set;
        }

        /// <summary>Gets or sets the enumerable of <see cref="Microsoft.OData.ODataEntityReferenceLink" /> instances representing the links of the referenced entities.</summary>
        /// <returns>The enumerable of <see cref="Microsoft.OData.ODataEntityReferenceLink" /> instances.</returns>
        /// <remarks>These links should be usable to retrieve or modify the referenced entities.</remarks>
        public IEnumerable<ODataEntityReferenceLink> Links
        {
            get;
            set;
        }

        /// <summary>
        /// Collection of custom instance annotations.
        /// </summary>
        public ICollection<ODataInstanceAnnotation> InstanceAnnotations
        {
            get { return this.GetInstanceAnnotations(); }
            set { this.SetInstanceAnnotations(value); }
        }
    }
}
