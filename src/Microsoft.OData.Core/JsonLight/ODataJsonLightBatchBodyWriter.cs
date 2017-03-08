//---------------------------------------------------------------------
// <copyright file="ODataJsonLightWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.JsonLight
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.OData.Edm;
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Core.Json;
    using Microsoft.OData.Core.Metadata;
    #endregion Namespaces

    /// <summary>
    /// Implementation of the ODataWriter for the JsonLight format with body attribute name for batch response.
    /// </summary>
    internal sealed class ODataJsonLightBatchBodyWriter : ODataJsonLightWriterCore
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonLightOutputContext">The output context to write to.</param>
        /// <param name="navigationSource">The navigation source we are going to write entities for.</param>
        /// <param name="entityType">The entity type for the entries in the feed to be written (or null if the entity set base type should be used).</param>
        /// <param name="writingFeed">true if the writer is created for writing a feed; false when it is created for writing an entry.</param>
        /// <param name="writingParameter">true if the writer is created for writing a parameter; false otherwise.</param>
        /// <param name="writingDelta">True if the writer is created for writing delta response; false otherwise.</param>
        /// <param name="listener">If not null, the writer will notify the implementer of the interface of relevant state changes in the writer.</param>
        internal ODataJsonLightBatchBodyWriter(
            ODataJsonLightOutputContext jsonLightOutputContext,
            IEdmNavigationSource navigationSource,
            IEdmEntityType entityType,
            bool writingFeed,
            bool writingParameter = false,
            bool writingDelta = false,
            IODataReaderWriterListener listener = null)
            : base(jsonLightOutputContext, navigationSource, entityType, writingFeed, writingParameter, writingDelta, listener)
        {
        }

        /// <summary>
        /// Starts writing a payload (called exactly once before anything else)
        /// </summary>
        protected override void StartPayload()
        {
            this.jsonWriter.WriteRawValue("\"body\" : ");
            this.jsonLightEntryAndFeedSerializer.WritePayloadStart();
        }
    }
}
