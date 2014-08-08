//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace System.Data.Services
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
        /// <returns>A <see cref="T:System.Data.Services.DataServiceOperationContext" /> that is the operation context. </returns>
        public DataServiceOperationContext OperationContext
        {
            [DebuggerStepThrough]
            get { return this.operationContext; }
        }
    }
}
