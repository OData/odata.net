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
