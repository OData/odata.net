//---------------------------------------------------------------------
// <copyright file="DataServiceTransportInfo.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Client
{
    using System.Diagnostics;

    /// <summary>
    /// Information required for creating the request message.
    /// </summary>
    internal class DataServiceTransportInfo
    {
        /// <summary>
        /// Creates an instance of DataServiceTransportInfo.
        /// </summary>
        /// <param name="context">DataServiceContext instance</param>
        internal DataServiceTransportInfo(DataServiceContext context)
        {
            Debug.Assert(context != null, "context != null");
            this.Context = context;
        }

        /// <summary>
        /// Gets the DataServiceContext instance.
        /// </summary>
        public DataServiceContext Context { get; private set; }
    }
}
