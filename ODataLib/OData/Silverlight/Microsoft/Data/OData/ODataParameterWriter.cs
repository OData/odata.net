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
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
#if ODATALIB_ASYNC
    using System.Threading.Tasks;
#endif
    using Microsoft.Data.Edm;
    using Microsoft.Data.OData.Json;
    #endregion Namespaces

    /// <summary>Base class for OData collection writers.</summary>
    public abstract class ODataParameterWriter
    {
        /// <summary>Start writing a parameter payload.</summary>
        public abstract void WriteStart();

#if ODATALIB_ASYNC
        /// <summary>Asynchronously start writing a parameter payload.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync();
#endif

        /// <summary>Start writing a value parameter.</summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write.</param>
        public abstract void WriteValue(string parameterName, object parameterValue);

#if ODATALIB_ASYNC
        /// <summary>Asynchronously start writing a value parameter.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write.</param>
        public abstract Task WriteValueAsync(string parameterName, object parameterValue);
#endif

        /// <summary>Creates an <see cref="T:Microsoft.Data.OData.ODataCollectionWriter" /> to write the value of a collection parameter.</summary>
        /// <returns>The newly created <see cref="T:Microsoft.Data.OData.ODataCollectionWriter" />.</returns>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        public abstract ODataCollectionWriter CreateCollectionWriter(string parameterName);

#if ODATALIB_ASYNC
        /// <summary>Asynchronously creates an <see cref="T:Microsoft.Data.OData.ODataCollectionWriter" /> to write the value of a collection parameter.</summary>
        /// <returns>The asynchronously created <see cref="T:Microsoft.Data.OData.ODataCollectionWriter" />.</returns>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        public abstract Task<ODataCollectionWriter> CreateCollectionWriterAsync(string parameterName);
#endif

        /// <summary>Finish writing a parameter payload.</summary>
        public abstract void WriteEnd();

#if ODATALIB_ASYNC
        /// <summary>Asynchronously finish writing a parameter payload.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public abstract void Flush();

#if ODATALIB_ASYNC
        /// <summary>Asynchronously flushes the write buffer to the underlying stream.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
#endif
    }
}
