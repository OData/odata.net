//---------------------------------------------------------------------
// <copyright file="DataServiceODataWriterNestedResourceInfoArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Diagnostics;
    using Microsoft.OData;

    /// <summary>
    /// Class that keeps track of the ODataNestedResourceInfo and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterNestedResourceInfoArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterNestedResourceInfoArgs.
        /// </summary>
        /// <param name="navigationLink">Instance of ODataNestedResourceInfo.</param>
        /// <param name="operationContext">Instance of DataServiceOperationContext.</param>
        public DataServiceODataWriterNestedResourceInfoArgs(
            ODataNestedResourceInfo navigationLink,
            DataServiceOperationContext operationContext)
        {
            WebUtil.CheckArgumentNull(navigationLink, "navigationLink != null");
            Debug.Assert(operationContext != null, "navigationLink != null");
            this.NavigationLink = navigationLink;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the ODataNestedResourceInfo instance that is going to be serialized.
        /// </summary>
        public ODataNestedResourceInfo NavigationLink
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
