//---------------------------------------------------------------------
// <copyright file="DataServiceODataWriterEntryArgs.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service
{
    using System.Diagnostics;
    using Microsoft.OData;

    /// <summary>
    /// Class that keeps track of the ODataResource, entity instance and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterEntryArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterEntryArgs
        /// </summary>
        /// <param name="entry">ODataResource instance.</param>
        /// <param name="entityInstance">Entity instance that is getting serialized.</param>
        /// <param name="operationContext">DataServiceOperationContext instance.</param>
        public DataServiceODataWriterEntryArgs(ODataResource entry, object entityInstance, DataServiceOperationContext operationContext)
        {
            Debug.Assert(operationContext != null, "operationContext != null");
            this.Entry = entry;
            this.Instance = entityInstance;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the ODataResource instance containing all the information
        /// that is going to be written in the wire.
        /// </summary>
        public ODataResource Entry
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the instance of the entity that is getting serialized.
        /// </summary>
        public object Instance
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
