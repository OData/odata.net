//---------------------------------------------------------------------
// <copyright file="BatchPayload`2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Http;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Base class for $batch request and response payloads
    /// </summary>
    /// <typeparam name="TOperation">The type of individual operations the payload contains</typeparam>
    /// <typeparam name="TChangeset">The type of changesets the payload contains</typeparam>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Doesn't need to end in 'collection'")]
    public abstract class BatchPayload<TOperation, TChangeset> : ODataPayloadElement, IEnumerable
        where TChangeset : BatchChangeset<TOperation>
        where TOperation : IMimePart
    {
        private List<IMimePart> mimeData = new List<IMimePart>();

        /// <summary>
        /// Initializes a new instance of the BatchPayload class
        /// </summary>
        protected BatchPayload()
        {
        }
        
        /// <summary>
        /// Gets the changesets that are part of the batch payload.
        /// </summary>
        public IEnumerable<TChangeset> Changesets
        {
            get
            {
                return this.mimeData.OfType<TChangeset>();
            }
        }

        /// <summary>
        /// Gets all the parts of the batch payload. Includes both changesets and single operations.
        /// </summary>
        public IEnumerable<IMimePart> Parts
        {
            get
            {
                return this.mimeData.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets the individual operations of the batch payload that do not belong to a changeset.
        /// </summary>
        public IEnumerable<TOperation> Operations
        {
            get
            {
                return this.mimeData.Cast<IMimePart>().Except(this.Changesets.Cast<IMimePart>()).Cast<MimePartData<TOperation>>().Select(p => p.Body);
            }
        }

        /// <summary>
        /// Gets the string representation of the payload
        /// </summary>
        public override string StringRepresentation
        {
            get { return this.ElementType.ToString(); }
        }
        
        /// <summary>
        /// Adds a part to the batch payload
        /// </summary>
        /// <param name="part">The part to add</param>
        public void Add(MimePartData<TOperation> part)
        {
            this.mimeData.Add(part);
        }

        /// <summary>
        /// Adds a changeset to the batch payload
        /// </summary>
        /// <param name="changeset">The changeset to add</param>
        public void Add(TChangeset changeset)
        {
            this.mimeData.Add(changeset);
        }

        /// <summary>
        /// Always throws. The strongly typed properties should be used to enumerate the batch.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<TOperation> GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }

        /// <summary>
        /// Always throws. The strongly typed properties should be used to enumerate the batch.
        /// </summary>
        /// <returns>The enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}
