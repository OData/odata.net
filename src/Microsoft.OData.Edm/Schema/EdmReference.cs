//---------------------------------------------------------------------
// <copyright file="EdmReference.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents edmx:reference element in CSDL doc.
    /// </summary>
    public class EdmReference : IEdmReference
    {
        private Uri uri;
        private List<IEdmInclude> includes = new List<IEdmInclude>();
        private List<IEdmIncludeAnnotations> includeAnnotations = new List<IEdmIncludeAnnotations>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="uri">The Uri to load referenced model.</param>
        public EdmReference(Uri uri)
        {
            this.uri = uri;
        }

        /// <summary>
        /// Gets the Uri to load referenced model.
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uri;
            }
        }

        /// <summary>
        /// Gets the Includes for referenced model.
        /// </summary>
        public IEnumerable<IEdmInclude> Includes
        {
            get
            {
                return this.includes;
            }
        }

        /// <summary>
        /// Gets the IncludeAnnotations for referenced model.
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
