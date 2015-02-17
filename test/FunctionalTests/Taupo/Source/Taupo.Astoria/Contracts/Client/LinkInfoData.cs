//---------------------------------------------------------------------
// <copyright file="LinkInfoData.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents data for the LinkInfo (from Microsoft.OData.Client namespace).
    /// </summary>
    public sealed class LinkInfoData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkInfoData"/> class.
        /// </summary>
        /// <param name="name">The name of the link property</param>
        internal LinkInfoData(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Gets or sets the name of the link property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the navigation link
        /// </summary>
        public Uri NavigationLink { get; set; }

        /// <summary>
        /// Gets or sets the relationship link
        /// </summary>
        public Uri RelationshipLink { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "{{ LinkInfo {{ Name = {0}, Navigation = {1}, Relationship = {2} }}}}",
                this.Name,
                this.NavigationLink,
                this.RelationshipLink);
        }

        /// <summary>
        /// Returns another LinkInfoData with equivalent values but no references to the current instance
        /// </summary>
        /// <returns>A cloned LinkInfoData</returns>
        public LinkInfoData Clone()
        {
            var clone = new LinkInfoData(this.Name);

            if (this.NavigationLink != null)
            {
                clone.NavigationLink = new Uri(this.NavigationLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            if (this.RelationshipLink != null)
            {
                clone.RelationshipLink = new Uri(this.RelationshipLink.OriginalString, UriKind.RelativeOrAbsolute);
            }

            return clone;
        }
    }
}