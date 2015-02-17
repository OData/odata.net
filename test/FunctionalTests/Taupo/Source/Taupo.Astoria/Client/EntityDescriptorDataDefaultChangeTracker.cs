//---------------------------------------------------------------------
// <copyright file="EntityDescriptorDataDefaultChangeTracker.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Client
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Astoria.Contracts.Client;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Default implementation of the entity descriptor data change tracker
    /// </summary>
    [ImplementationName(typeof(IEntityDescriptorDataChangeTracker), "Default")]
    public class EntityDescriptorDataDefaultChangeTracker : IEntityDescriptorDataChangeTracker
    {
        private List<HeaderUpdate> headerUpdates = new List<HeaderUpdate>();
        private List<PayloadUpdate> payloadUpdates = new List<PayloadUpdate>();
        private bool applyUpdatesImmediately = true;
        private bool ignoreAllUpdates = false;
        
        /// <summary>
        /// Gets or sets a value indicating whether to apply updates immediately. Should default to true.
        /// If false, changes should not be applied until the ApplyPendingUpdates methods are called.
        /// </summary>
        public bool ApplyUpdatesImmediately
        {
            get
            {
                return this.applyUpdatesImmediately;
            }

            set
            {
                ExceptionUtilities.Assert(this.headerUpdates.Count == 0, "Cannot change ApplyUpdatesImmediately while there are pending header updates");
                ExceptionUtilities.Assert(this.payloadUpdates.Count == 0, "Cannot change ApplyUpdatesImmediately while there are pending payload updates");
                this.applyUpdatesImmediately = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether all updates should be ignored
        /// </summary>
        public bool IgnoreAllUpdates
        {
            get
            {
                return this.ignoreAllUpdates;
            }

            set
            {
                ExceptionUtilities.Assert(this.headerUpdates.Count == 0, "Cannot change IgnoreAllUpdates while there are pending header updates");
                ExceptionUtilities.Assert(this.payloadUpdates.Count == 0, "Cannot change IgnoreAllUpdates while there are pending payload updates");
                this.ignoreAllUpdates = value;
            }
        }

        /// <summary>
        /// Tracks a pending update to the given descriptor based on values read from a response payload
        /// </summary>
        /// <param name="data">The descriptor data to update</param>
        /// <param name="payload">The payload that was read</param>
        /// <param name="baseUri">The base uri of the context</param>
        public void TrackUpdateFromPayload(EntityDescriptorData data, EntityInstance payload, Uri baseUri)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            var update = new PayloadUpdate() { Descriptor = data, Payload = payload, BaseUri = baseUri };
            if (!this.IgnoreAllUpdates)
            {
                if (this.ApplyUpdatesImmediately)
                {
                    Apply(update);
                }
                else
                {
                    this.payloadUpdates.Add(update);
                }
            }
        }

        /// <summary>
        /// Tracks a pending update to the given descriptor based on values read from response headers
        /// </summary>
        /// <param name="data">The descriptor data to update</param>
        /// <param name="headers">The headers that were read</param>
        public void TrackUpdateFromHeaders(EntityDescriptorData data, IDictionary<string, string> headers)
        {
            ExceptionUtilities.CheckArgumentNotNull(data, "data");
            var update = new HeaderUpdate() { Descriptor = data, Headers = headers };
           
            if (!this.IgnoreAllUpdates)
            {
                if (this.ApplyUpdatesImmediately)
                {
                    Apply(update);
                }
                else
                {
                    this.headerUpdates.Add(update);
                }
            }
        }

        /// <summary>
        /// Applies all pending updates for the given entity descriptor data
        /// </summary>
        /// <param name="data">The descriptor data to update</param>
        public void ApplyPendingUpdates(EntityDescriptorData data)
        {
            foreach (var update in this.headerUpdates.Where(u => u.Descriptor == data).ToList())
            {
                ExceptionUtilities.Assert(!this.ApplyUpdatesImmediately, "Should not have any pending header updates when ApplyUpdatesImmediately is true");
                Apply(update);
                this.headerUpdates.Remove(update);
            }

            foreach (var update in this.payloadUpdates.Where(u => u.Descriptor == data).ToList())
            {
                ExceptionUtilities.Assert(!this.ApplyUpdatesImmediately, "Should not have any pending payload updates when ApplyUpdatesImmediately is true");
                Apply(update);
                this.payloadUpdates.Remove(update);
            }
        }

        /// <summary>
        /// Applies all pending entity descriptor updates
        /// </summary>
        public void ApplyPendingUpdates()
        {
            if (this.headerUpdates.Count > 0)
            {
                ExceptionUtilities.Assert(!this.ApplyUpdatesImmediately, "Should not have any pending header updates when ApplyUpdatesImmediately is true");
                this.headerUpdates.ForEach(u => Apply(u));
                this.headerUpdates.Clear();
            }

            if (this.payloadUpdates.Count > 0)
            {
                ExceptionUtilities.Assert(!this.ApplyUpdatesImmediately, "Should not have any pending payload updates when ApplyUpdatesImmediately is true");
                this.payloadUpdates.ForEach(u => Apply(u));
                this.payloadUpdates.Clear();
            }
        }

        private static void Apply(PayloadUpdate update)
        {
            update.Descriptor.UpdateFromPayload(update.Payload, update.BaseUri);
        }

        private static void Apply(HeaderUpdate update)
        {
            update.Descriptor.UpdateFromHeaders(update.Headers);
        }

        /// <summary>
        /// Data structure for a pending payload update
        /// </summary>
        private class PayloadUpdate
        {
            /// <summary>
            /// Gets or sets the descriptor
            /// </summary>
            public EntityDescriptorData Descriptor { get; set; }

            /// <summary>
            /// Gets or sets the payload
            /// </summary>
            public EntityInstance Payload { get; set; }

            /// <summary>
            /// Gets or sets the base uri
            /// </summary>
            public Uri BaseUri { get; set; }
        }

        /// <summary>
        /// Data structure for a pending header update
        /// </summary>
        private class HeaderUpdate
        {
            /// <summary>
            /// Gets or sets the descriptor
            /// </summary>
            public EntityDescriptorData Descriptor { get; set; }
            
            /// <summary>
            /// Gets or sets the headers
            /// </summary>
            public IDictionary<string, string> Headers { get; set; }
        }
    }
}
