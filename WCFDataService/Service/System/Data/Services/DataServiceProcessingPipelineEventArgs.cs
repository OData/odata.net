//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

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
