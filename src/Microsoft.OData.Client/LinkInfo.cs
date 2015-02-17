//---------------------------------------------------------------------
// <copyright file="LinkInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;

    /// <summary>Encapsulates information about a link, or relationship, between entities.</summary>
    public sealed class LinkInfo
    {
        /// <summary>navigation URI to the related entity.</summary>
        private Uri navigationLink;

        /// <summary>association URI to the related entity.</summary>
        private Uri associationLink;

        /// <summary>the navigation property name</summary>
        private string name;

        /// <summary>
        /// Creates a LinkInfo with a given properyName
        /// </summary>
        /// <param name="propertyName">the name of the navigation property</param>
        internal LinkInfo(String propertyName)
        {
            Debug.Assert(!String.IsNullOrEmpty(propertyName), "!String.IsNullOrEmpty(propertyName)");
            this.name = propertyName;
        }

        /// <summary>Gets the name of the link.</summary>
        /// <returns>The name of the link.</returns>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>Gets the URI that is the navigation property representation of the link.</summary>
        /// <returns>The navigation link URI.</returns>
        public Uri NavigationLink
        {
            get
            {
                return this.navigationLink;
            }

            internal set
            {
                Debug.Assert(value == null || value.IsAbsoluteUri, "navigation link must be absolute uri");
                this.navigationLink = value;
            }
        }

        /// <summary>Gets the URI that is the association link.</summary>
        /// <returns>The URI of the association link.</returns>
        public Uri AssociationLink
        {
            get
            {
                return this.associationLink;
            }

            internal set
            {
                Debug.Assert(value == null || value.IsAbsoluteUri, "association link must be absolute uri");
                this.associationLink = value;
            }
        }
    }
}
