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
    using System.Diagnostics;
    using Microsoft.Data.OData;

    /// <summary>
    /// Class that keeps track of the ODataEntry, entity instance and other information
    /// that we need to provide to the service author when they choose to provide their own
    /// instance of ODataWriter.
    /// </summary>
    public sealed class DataServiceODataWriterEntryArgs
    {
        /// <summary>
        /// Creates a new instance of DataServiceODataWriterEntryArgs
        /// </summary>
        /// <param name="entry">ODataEntry instance.</param>
        /// <param name="entityInstance">Entity instance that is getting serialized.</param>
        /// <param name="operationContext">DataServiceOperationContext instance.</param>
        public DataServiceODataWriterEntryArgs(ODataEntry entry, object entityInstance, DataServiceOperationContext operationContext)
        {
            Debug.Assert(operationContext != null, "operationContext != null");
            this.Entry = entry;
            this.Instance = entityInstance;
            this.OperationContext = operationContext;
        }

        /// <summary>
        /// Gets the ODataEntry instance containing all the information
        /// that is going to be written in the wire.
        /// </summary>
        public ODataEntry Entry
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
