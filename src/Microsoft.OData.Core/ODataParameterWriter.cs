//---------------------------------------------------------------------
// <copyright file="ODataParameterWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces

    #if PORTABLELIB
    using System.Threading.Tasks;
#endif

    #endregion Namespaces

    /// <summary>Base class for OData collection writers.</summary>
    public abstract class ODataParameterWriter
    {
        /// <summary>Start writing a parameter payload.</summary>
        public abstract void WriteStart();

#if PORTABLELIB
        /// <summary>Asynchronously start writing a parameter payload.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteStartAsync();
#endif

        /// <summary>Start writing a value parameter.</summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write (null/ODataEnumValue/primitiveClrValue).</param>
        public abstract void WriteValue(string parameterName, object parameterValue);

#if PORTABLELIB
        /// <summary>Asynchronously start writing a value parameter.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <param name="parameterValue">The value of the parameter to write.</param>
        public abstract Task WriteValueAsync(string parameterName, object parameterValue);
#endif

        /// <summary>Creates an <see cref="T:Microsoft.OData.ODataCollectionWriter" /> to write the value of a collection parameter.</summary>
        /// <returns>The newly created <see cref="T:Microsoft.OData.ODataCollectionWriter" />.</returns>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        public abstract ODataCollectionWriter CreateCollectionWriter(string parameterName);

#if PORTABLELIB
        /// <summary>Asynchronously creates an <see cref="T:Microsoft.OData.ODataCollectionWriter" /> to write the value of a collection parameter.</summary>
        /// <returns>The asynchronously created <see cref="T:Microsoft.OData.ODataCollectionWriter" />.</returns>
        /// <param name="parameterName">The name of the collection parameter to write.</param>
        public abstract Task<ODataCollectionWriter> CreateCollectionWriterAsync(string parameterName);
#endif

        /// <summary> Creates an <see cref="T:Microsoft.OData.ODataWriter" /> to write a resource. </summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <returns>The created writer.</returns>
        public abstract ODataWriter CreateResourceWriter(string parameterName);

#if PORTABLELIB
        /// <summary>Asynchronously creates an <see cref="T:Microsoft.OData.ODataWriter" /> to  write a resource.</summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <returns>The asynchronously created <see cref="T:Microsoft.OData.ODataWriter" />.</returns>
        public abstract Task<ODataWriter> CreateResourceWriterAsync(string parameterName);
#endif

        /// <summary> Creates an <see cref="T:Microsoft.OData.ODataWriter" /> to write a resource set. </summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <returns>The created writer.</returns>
        public abstract ODataWriter CreateResourceSetWriter(string parameterName);

#if PORTABLELIB
        /// <summary>Asynchronously creates an <see cref="T:Microsoft.OData.ODataWriter" /> to  write a resource set.</summary>
        /// <param name="parameterName">The name of the parameter to write.</param>
        /// <returns>The asynchronously created <see cref="T:Microsoft.OData.ODataWriter" />.</returns>
        public abstract Task<ODataWriter> CreateResourceSetWriterAsync(string parameterName);
#endif

        /// <summary>Finish writing a parameter payload.</summary>
        public abstract void WriteEnd();

#if PORTABLELIB
        /// <summary>Asynchronously finish writing a parameter payload.</summary>
        /// <returns>A task instance that represents the asynchronous write operation.</returns>
        public abstract Task WriteEndAsync();
#endif

        /// <summary>Flushes the write buffer to the underlying stream.</summary>
        public abstract void Flush();

#if PORTABLELIB
        /// <summary>Asynchronously flushes the write buffer to the underlying stream.</summary>
        /// <returns>A task instance that represents the asynchronous operation.</returns>
        public abstract Task FlushAsync();
#endif
    }
}
