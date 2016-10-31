//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace System.Data.Services.Client
{
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
