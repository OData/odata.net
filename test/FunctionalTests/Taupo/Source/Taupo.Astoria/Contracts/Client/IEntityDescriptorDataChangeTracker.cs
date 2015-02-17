//---------------------------------------------------------------------
// <copyright file="IEntityDescriptorDataChangeTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.Client
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Contract for tracking changes to entity descriptor data based on headers and payloads
    /// </summary>
    [ImplementationSelector("EntityDescriptorDataChangeTracker", DefaultImplementation = "Default")]
    public interface IEntityDescriptorDataChangeTracker
    {
        /// <summary>
        /// Gets or sets a value indicating whether to apply updates immediately. Should default to true.
        /// If false, changes should not be applied until the ApplyPendingUpdates methods are called.
        /// </summary>
        bool ApplyUpdatesImmediately { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether all updates should be ignored
        /// </summary>
        bool IgnoreAllUpdates { get; set; }

        /// <summary>
        /// Tracks a pending update to the given descriptor based on values read from response headers
        /// </summary>
        /// <param name="data">The descriptor data to update</param>
        /// <param name="headers">The headers that were read</param>
        void TrackUpdateFromHeaders(EntityDescriptorData data, IDictionary<string, string> headers);

        /// <summary>
        /// Tracks a pending update to the given descriptor based on values read from a response payload
        /// </summary>
        /// <param name="data">The descriptor data to update</param>
        /// <param name="payload">The payload that was read</param>
        /// <param name="baseUri">The context base uri</param>
        void TrackUpdateFromPayload(EntityDescriptorData data, EntityInstance payload, Uri baseUri);

        /// <summary>
        /// Applies all pending updates for the given entity descriptor data
        /// </summary>
        /// <param name="data">The descriptor data to update</param>
        void ApplyPendingUpdates(EntityDescriptorData data);

        /// <summary>
        /// Applies all pending entity descriptor updates
        /// </summary>
        void ApplyPendingUpdates();
    }
}
