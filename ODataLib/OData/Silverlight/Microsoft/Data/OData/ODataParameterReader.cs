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
    /// Base class for OData parameter readers.
    /// </summary>
    public abstract class ODataParameterReader
    {
        /// <summary>
        /// The current state of the reader.
        /// </summary>
        public abstract ODataParameterReaderState State
        {
            get;
        }

        /// <summary>
        /// The name of the current parameter that is being read.
        /// </summary>
        public abstract string Name
        {
            get;
        }

        /// <summary>
        /// The value of the current parameter that is being read.
        /// </summary>
        /// <remarks>
        /// This property returns a primitive value, an ODataComplexValue or null when State is ODataParameterReaderState.Value.
        /// This property returns null when State is ODataParameterReaderState.Entry, ODataParameterReaderState.Feed or ODataParameterReaderState.Collection.
        /// </remarks>
        public abstract object Value
        {
            get;
        }

#if SUPPORT_ENTITY_PARAMETER
        /// <summary>
        /// This method creates an <see cref="ODataReader"/> to read the entry value when the state is ODataParameterReaderState.Entry.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Entry, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataReader"/> to read the entry value when the state is ODataParameterReaderState.Entry.</returns>
        public abstract ODataReader CreateEntryReader();

        /// <summary>
        /// This method creates an <see cref="ODataReader"/> to read the feed value when the state is ODataParameterReaderState.Feed.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Feed, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataReader"/> to read the feed value when the state is ODataParameterReaderState.Feed.</returns>
        public abstract ODataReader CreateFeedReader();
#endif

        /// <summary>
        /// This method creates an <see cref="ODataCollectionReader"/> to read the collection value when the state is ODataParameterReaderState.Collection.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Collection, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataCollectionReader"/> to read the collection value when the state is ODataParameterReaderState.Collection.</returns>
        public abstract ODataCollectionReader CreateCollectionReader();

        /// <summary>
        /// Reads the next parameter from the message payload.
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
