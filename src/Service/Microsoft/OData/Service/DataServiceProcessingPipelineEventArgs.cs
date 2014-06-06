//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Service
{
    #region Namespaces

    using System;
    using System.Diagnostics;

    #endregion Namespaces

    /// <summary>
    /// Event argument class for DataServiceProcessingPipeline events.
    /// </summary>
    public sealed class DataServiceProcessingPipelineEventArgs : EventArgs
    {
        /// <summary>
        /// Context for the operation which the current event is fired for.
        /// </summary>
        private readonly DataServiceOperationContext operationContext;

        /// <summary>
        /// Constructs a new instance of DataServicePipelineEventArgs object
        /// </summary>
        /// <param name="operationContext">Context for the operation which the current event is fired for.</param>
        internal DataServiceProcessingPipelineEventArgs(DataServiceOperationContext operationContext)
        {
            Debug.Assert(operationContext != null, "operationContext != null");
            this.operationContext = operationContext;
        }

        /// <summary>Gets the context of the operation that raised the event.</summary>
        /// <returns>A <see cref="T:Microsoft.OData.Service.DataServiceOperationContext" /> that is the operation context. </returns>
        public DataServiceOperationContext OperationContext
        {
            [DebuggerStepThrough]
            get { return this.operationContext; }
        }
    }
}
