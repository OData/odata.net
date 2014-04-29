//---------------------------------------------------------------------
// <copyright file="DataServiceTransportInfo.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
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
#if !ASTORIA_LIGHT
        /// <summary>
        /// Creates an instance of DataServiceTransportInfo.
        /// </summary>
        /// <param name="context">DataServiceContext instance</param>
        internal DataServiceTransportInfo(DataServiceContext context)
        {
            Debug.Assert(context != null, "context != null");
            this.Context = context;
        }
#else
        /// <summary>
        /// Creates an instance of DataServiceTransportInfo.
        /// </summary>
        /// <param name="context">DataServiceContext instance.</param>
        /// <param name="httpStack">HttpStack to use for silverlight.</param>
        internal DataServiceTransportInfo(DataServiceContext context, HttpStack httpStack)
        {
            Debug.Assert(context != null, "context != null");
            this.Context = context;
            this.HttpStack = httpStack;
        }
#endif

        /// <summary>
        /// Gets the DataServiceContext instance.
        /// </summary>
        public DataServiceContext Context { get; private set; }

#if ASTORIA_LIGHT
        /// <summary>
        /// Gets the HttpStack to use while creating the request message.
        /// </summary>
        public HttpStack HttpStack { get; private set; }
#endif
    }
}
