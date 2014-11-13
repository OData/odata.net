//   OData .NET Libraries ver. 6.8.1
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

using System.Collections.Generic;

namespace Microsoft.OData.Edm.Library
{
    /// <summary>
    /// Represents edmx:reference element in CSDL doc.
    /// </summary>
    public class EdmReference : IEdmReference
    {
        private string uri;
        private List<IEdmInclude> includes = new List<IEdmInclude>();
        private List<IEdmIncludeAnnotations> includeAnnotations = new List<IEdmIncludeAnnotations>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uri">The Uri to load referneced model.</param>
        public EdmReference(string uri)
        {
            this.uri = uri;
        }

        /// <summary>
        /// Gets the Uri to load referneced model.
        /// </summary>
        public string Uri
        {
            get
            {
                return this.uri;
            }
        }

        /// <summary>
        /// Gets the Includes for referneced model.
        /// </summary>
        public IEnumerable<IEdmInclude> Includes
        {
            get
            {
                return this.includes;
            }
        }

        /// <summary>
        /// Gets the IncludeAnnotations for referneced model.
        /// </summary>
        public IEnumerable<IEdmIncludeAnnotations> IncludeAnnotations
        {
            get
            {
                return this.includeAnnotations;
            }
        }

        /// <summary>
        /// Add include information.
        /// </summary>
        /// <param name="edmInclude">The IEdmInclude.</param>
        public void AddInclude(IEdmInclude edmInclude)
        {
            this.includes.Add(edmInclude);
        }

        /// <summary>
        /// Add IncludeAnnotations information.
        /// </summary>
        /// <param name="edmIncludeAnnotations">The IEdmIncludeAnnotations.</param>
        public void AddIncludeAnnotations(IEdmIncludeAnnotations edmIncludeAnnotations)
        {
            this.includeAnnotations.Add(edmIncludeAnnotations);
        }
    }
}
