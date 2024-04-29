//---------------------------------------------------------------------
// <copyright file="ODataJsonDeltaReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Json
{
    #region Namespaces

    using System.Diagnostics;
    using System.Threading.Tasks;
    using Microsoft.OData.Edm;

    #endregion Namespaces

    /// <summary>
    /// OData delta reader for the Json format.
    /// </summary>
    internal sealed class ODataJsonDeltaReader : ODataDeltaReader
    {
        #region Private Fields

        /// <summary>Underlying Json Reader</summary>
        private readonly ODataJsonReader underlyingReader;

        /// <summary>Whether the reader is reading nested content</summary>
        private int nestedLevel;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonInputContext">The input to read the payload from.</param>
        /// <param name="navigationSource">The navigation source we are going to read entities for.</param>
        /// <param name="expectedEntityType">The expected entity type for the resource to be read (in case of resource reader) or entries in the resource set to be read (in case of resource set reader).</param>
        public ODataJsonDeltaReader(
            ODataJsonInputContext jsonInputContext,
            IEdmNavigationSource navigationSource,
            IEdmEntityType expectedEntityType)
        {
            this.underlyingReader = new ODataJsonReader(
                jsonInputContext,
                navigationSource,
                expectedEntityType,
                readingResourceSet: true,
                readingParameter: false,
                readingDelta: true,
                listener: null);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the current state of the reader. </summary>
        /// <returns>The current state of the reader.</returns>
        public override ODataDeltaReaderState State
        {
            get
            {
                if (this.nestedLevel > 0 || this.underlyingReader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    return ODataDeltaReaderState.NestedResource;
                }

                switch (this.underlyingReader.State)
                {
                    case ODataReaderState.Start:
                        return ODataDeltaReaderState.Start;
                    case ODataReaderState.DeltaResourceSetStart:
                        return ODataDeltaReaderState.DeltaResourceSetStart;
                    case ODataReaderState.DeltaResourceSetEnd:
                        return ODataDeltaReaderState.DeltaResourceSetEnd;
                    case ODataReaderState.ResourceStart:
                        return ODataDeltaReaderState.DeltaResourceStart;
                    case ODataReaderState.ResourceEnd:
                        return ODataDeltaReaderState.DeltaResourceEnd;
                    case ODataReaderState.DeletedResourceEnd:
                        return ODataDeltaReaderState.DeltaDeletedEntry;
                    case ODataReaderState.DeltaDeletedLink:
                        return ODataDeltaReaderState.DeltaDeletedLink;
                    case ODataReaderState.DeltaLink:
                        return ODataDeltaReaderState.DeltaLink;
                    case ODataReaderState.Completed:
                        return ODataDeltaReaderState.Completed;

                    // These states are for reading nested resources
                    case ODataReaderState.EntityReferenceLink:
                    case ODataReaderState.Primitive:
                    case ODataReaderState.DeletedResourceStart:
                    case ODataReaderState.NestedResourceInfoStart:
                    case ODataReaderState.ResourceSetStart:
                    case ODataReaderState.ResourceSetEnd:
                    default:
                        Debug.Assert(true, "unexpected ReaderState for legacy reader");
                        return ODataDeltaReaderState.NestedResource;
                }
            }
        }

        /// <summary>Gets the current sub state of the reader. </summary>
        /// <returns>The current sub state of the reader.</returns>
        /// <remarks>
        /// The sub state is a complement to the current state if the current state itself is not enough to determine
        /// the real state of the reader. The sub state is only meaningful in NestedResourceInfo state.
        /// </remarks>
        public override ODataReaderState SubState
        {
            get
            {
                // Match legacy behavior
                if (this.nestedLevel == 1 && this.underlyingReader.State == ODataReaderState.NestedResourceInfoStart)
                {
                    return ODataReaderState.Start;
                }

                if (this.nestedLevel == 0 && this.underlyingReader.State == ODataReaderState.NestedResourceInfoEnd)
                {
                    return ODataReaderState.Completed;
                }

                return this.nestedLevel > 0 ?
                    this.underlyingReader.State : ODataReaderState.Start;
            }
        }

        /// <summary>Gets the most recent <see cref="Microsoft.OData.ODataItem" /> that has been read. </summary>
        /// <returns>The most recent <see cref="Microsoft.OData.ODataItem" /> that has been read.</returns>
        public override ODataItem Item
        {
            get
            {
                ODataDeletedResource deletedResource = this.underlyingReader.Item as ODataDeletedResource;
                if (deletedResource != null)
                {
                    return ODataDeltaDeletedEntry.GetDeltaDeletedEntry(deletedResource);
                }
                else
                {
                    return this.underlyingReader.Item;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary> Reads the next <see cref="Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public override bool Read()
        {
            // Translate state transitions to legacy states
            bool response = this.underlyingReader.Read();
            if (this.underlyingReader.State == ODataReaderState.DeletedResourceStart)
            {
                while ((response = this.underlyingReader.Read()) && this.underlyingReader.State != ODataReaderState.DeletedResourceEnd)
                {
                    // skip to end of a deleted resource
                    this.SetNestedLevel();
                }
            }

            this.SetNestedLevel();

            return response;
        }

        /// <summary> Asynchronously reads the next <see cref="Microsoft.OData.ODataItem" /> from the message payload. </summary>
        /// <returns>
        /// A task that represents the asynchronous read operation.
        /// The value of the TResult parameter contains true if more items were read; otherwise false.
        /// </returns>
        public override async Task<bool> ReadAsync()
        {
            bool result = await this.underlyingReader.ReadAsync()
                .ConfigureAwait(false);

            if (this.underlyingReader.State == ODataReaderState.DeletedResourceStart)
            {
                while ((result = await this.underlyingReader.ReadAsync().ConfigureAwait(false)) && this.underlyingReader.State != ODataReaderState.DeletedResourceEnd)
                {
                    // Skip to end of a deleted resource
                    this.SetNestedLevel();
                }
            }

            this.SetNestedLevel();

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary> Sets nested level following a successful read. </summary>
        private void SetNestedLevel()
        {
            if (this.underlyingReader.State == ODataReaderState.NestedResourceInfoStart)
            {
                this.nestedLevel++;
            }
            else if (this.underlyingReader.State == ODataReaderState.NestedResourceInfoEnd)
            {
                this.nestedLevel--;
            }
        }

#endregion
    }
}
