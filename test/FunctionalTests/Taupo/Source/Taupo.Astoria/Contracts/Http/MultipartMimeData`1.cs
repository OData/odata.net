//---------------------------------------------------------------------
// <copyright file="MultipartMimeData`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Http
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Data structure for representing a multipart MIME body
    /// </summary>
    /// <typeparam name="TPart">The type of the individual mime parts</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "The name should not have 'Collection' suffix.")]
    public class MultipartMimeData<TPart> : IMimePart, IEnumerable<TPart> where TPart : IMimePart
    {
        private List<TPart> parts = new List<TPart>();

        /// <summary>
        /// Initializes a new instance of the MultipartMimeData class
        /// </summary>
        public MultipartMimeData()
        {
            this.Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the boundary that seperates the parts of the MIME body
        /// </summary>
        public string Boundary
        {
            get
            {
                string boundary;
                if (this.TryGetMimeBoundary(out boundary))
                {
                    return boundary;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the headers for the MIME part
        /// </summary>
        public IDictionary<string, string> Headers { get; private set; }

        /// <summary>
        /// Adds a part to this multipart mime body
        /// </summary>
        /// <param name="part">The part to add</param>
        public virtual void Add(TPart part)
        {
            this.parts.Add(part);
        }

        /// <summary>
        /// Returns the enumerator for this multipart mime body
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<TPart> GetEnumerator()
        {
            return this.parts.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator for this multipart mime body
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TPart>)this).GetEnumerator();
        }
    }
}
