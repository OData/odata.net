//---------------------------------------------------------------------
// <copyright file="DataServiceClientConfigurations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System.Collections.Generic;
    using System.Diagnostics;

    /// <summary>
    /// Configurations on the behavior of the Client.
    /// </summary>
    public class DataServiceClientConfigurations
    {
        // default to null and lazy initialize it to avoid memory impact for majority case that does not use this.
        private IDictionary<string, object> properties = null;

        /// <summary>
        /// Creates a data service client configurations class
        /// </summary>
        /// <param name="sender"> The sender for the Reading Atom event.</param>
        public DataServiceClientConfigurations(object sender)
        {
            Debug.Assert(sender != null, "sender!= null");

            this.ResponsePipeline = new DataServiceClientResponsePipelineConfiguration(sender);
            this.RequestPipeline = new DataServiceClientRequestPipelineConfiguration();
        }

        /// <summary>
        /// Gets the response configuration pipeline.
        /// </summary>
        public DataServiceClientResponsePipelineConfiguration ResponsePipeline { get; private set; }

        /// <summary>
        /// Gets the request pipeline.
        /// </summary>
        public DataServiceClientRequestPipelineConfiguration RequestPipeline { get; private set; }

        /// <summary>
        /// A central location for sharing state between OData client handlers during the client process pipeline.
        /// </summary>
        public IDictionary<string, object> Properties
        {
            get
            {
                // lazy initialize it to avoid memory impact
                // no need to lock as DataServiceContext instance are not designed for concurrent access.
                if (this.properties == null)
                {
                    this.properties = new Dictionary<string, object>();
                }

                return this.properties;
            }
        }
    }
}
