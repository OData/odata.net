//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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
