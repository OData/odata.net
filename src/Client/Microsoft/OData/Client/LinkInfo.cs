//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

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
