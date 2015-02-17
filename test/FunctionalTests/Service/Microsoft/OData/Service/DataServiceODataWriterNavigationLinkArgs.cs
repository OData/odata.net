//---------------------------------------------------------------------
// <copyright file="DataServiceODataWriterNavigationLinkArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Diagnostics;
    using Microsoft.OData.Core;

    /// <summary>
    /// Class that keeps track of the ODataNavigationLink and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterNavigationLinkArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterNavigationLinkArgs.
        /// </summary>
        /// <param name="navigationLink">Instance of ODataNavigationLink.</param>
        /// <param name="operationContext">Instance of DataServiceOperationContext.</param>
        public DataServiceODataWriterNavigationLinkArgs(
            ODataNavigationLink navigationLink,
            DataServiceOperationContext operationContext)
        {
            WebUtil.CheckArgumentNull(navigationLink, "navigationLink != null");
            Debug.Assert(operationContext != null, "navigationLink != null");
            this.NavigationLink = navigationLink;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the ODataNavigationLink instance that is going to be serialized.
        /// </summary>
        public ODataNavigationLink NavigationLink
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the instance of DataServiceOperationContext.
        /// </summary>
        public DataServiceOperationContext OperationContext
        {
            get;
            private set;
        }
    }
}
