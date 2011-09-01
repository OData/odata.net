//   Copyright 2011 Microsoft Corporation
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

namespace Microsoft.Data.OData
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
        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public abstract ODataCollectionReaderState State
        {
            get;
        }

        /// <summary>
        /// The most recent item that has been read.
        /// This property returns an <see cref="ODataCollectionResult"/> when in state ODataCollectionReaderState.CollectionStart
        /// or ODataCollectionReaderState.CollectionEnd. It returns either a primitive value, an <see cref="ODataComplexValue"/> or 'null' when
        /// in state ODataCollectionReaderState.Value and 'null' in all other states.
        /// </summary>
        public abstract object Item
        {
            get;
        }

        /// <summary>
        /// Reads the next item from the message payload.
        /// </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public abstract bool Read();

#if ODATALIB_ASYNC
        /// <summary>
        /// Asynchronously reads the next item from the message payload.
        /// </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        [SuppressMessage("Microsoft.MSInternal", "CA908:AvoidTypesThatRequireJitCompilationInPrecompiledAssemblies", Justification = "API design calls for a bool being returned from the task here.")]
        public abstract Task<bool> ReadAsync();
#endif
    }
}
