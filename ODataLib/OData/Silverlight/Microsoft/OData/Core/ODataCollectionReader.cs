//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary>
    /// Base class for OData collection readers.
    /// </summary>
    public abstract class ODataCollectionReader
    {
        /// <summary>Gets the current state of the reader.</summary>
        /// <returns>The current state of the reader.</returns>
        public abstract ODataCollectionReaderState State
        {
            get;
        }

        /// <summary>Gets the most recent item that has been read.</summary>
        /// <returns>The most recent item that has been read.</returns>
        /// <remarks>
        /// This property returns an <see cref="ODataCollectionStart"/> when in state ODataCollectionReaderState.CollectionStart
        /// or ODataCollectionReaderState.CollectionEnd. It returns either a primitive value, an <see cref="ODataComplexValue"/> or 'null' when
        /// in state ODataCollectionReaderState.Value and 'null' in all other states.
        /// </remarks>
        public abstract object Item
        {
            get;
        }

        /// <summary>Reads the next item from the message payload. </summary>
        /// <returns>True if more items were read; otherwise false.</returns>
        public abstract bool Read();

#if ODATALIB_ASYNC
        /// <summary>Asynchronously reads the next item from the message payload.</summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        public abstract Task<bool> ReadAsync();
#endif
    }
}
