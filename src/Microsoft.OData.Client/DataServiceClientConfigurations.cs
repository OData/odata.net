//---------------------------------------------------------------------
// <copyright file="DataServiceClientConfigurations.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System.Diagnostics;

    /// <summary>
    /// Configurations on the behavior of the Client.
    /// </summary>
    public class DataServiceClientConfigurations
    {
        /// <summary>
        /// Creates a data service client configurations class
        /// </summary>
        /// <param name="sender"> The sender for the Reading Atom event.</param>
        internal DataServiceClientConfigurations(object sender)
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
    }
}
