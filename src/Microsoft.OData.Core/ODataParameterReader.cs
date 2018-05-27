//---------------------------------------------------------------------
// <copyright file="ODataParameterReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System.Diagnostics.CodeAnalysis;
#if PORTABLELIB
    using System.Threading.Tasks;
#endif
    #endregion Namespaces

    /// <summary> Base class for OData parameter readers. </summary>
    public abstract class ODataParameterReader
    {
        /// <summary> Gets the current state of the reader. </summary>
        /// <returns> The current state of the reader. </returns>
        public abstract ODataParameterReaderState State
        {
            get;
        }

        /// <summary> Gets the name of the current parameter that is being read. </summary>
        /// <returns> The name of the current parameter that is being read. </returns>
        public abstract string Name
        {
            get;
        }

        /// <summary> Gets the value of the current parameter that is being read. </summary>
        /// <returns> The value of the current parameter that is being read. </returns>
        /// <remarks>
        /// This property returns a primitive value or null when State is ODataParameterReaderState.Value.
        /// This property returns null when State is ODataParameterReaderState.Resource, ODataParameterReaderState.ResourceSet or ODataParameterReaderState.Collection.
        /// </remarks>
        public abstract object Value
        {
            get;
        }

        /// <summary>
        /// This method creates an <see cref="ODataReader"/> to read the resource value when the state is ODataParameterReaderState.Resource.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Resource, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataReader"/> to read the resource value when the state is ODataParameterReaderState.Resource.</returns>
        public abstract ODataReader CreateResourceReader();

        /// <summary>
        /// This method creates an <see cref="ODataReader"/> to read the resource set value when the state is ODataParameterReaderState.ResourceSet.
        /// </summary>
        /// <remarks>
        /// When the state is ODataParameterReaderState.ResourceSet, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        /// <returns>Returns an <see cref="ODataReader"/> to read the resource set value when the state is ODataParameterReaderState.ResourceSet.</returns>
        public abstract ODataReader CreateResourceSetReader();

        /// <summary> Creates an <see cref="T:Microsoft.OData.ODataCollectionReader" /> to read the collection value when the state is ODataParameterReaderState.Collection. </summary>
        /// <returns>An <see cref="T:Microsoft.OData.ODataCollectionReader" /> to read the collection value when the state is ODataParameterReaderState.Collection.</returns>
        /// <remarks>
        /// When the state is ODataParameterReaderState.Collection, the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. Calling this method in any other state will cause an ODataException to be thrown.
        /// </remarks>
        public abstract ODataCollectionReader CreateCollectionReader();

        /// <summary> Reads the next parameter from the message payload. </summary>
        /// <returns>true if more items were read; otherwise false.</returns>
        public abstract bool Read();

#if PORTABLELIB
        /// <summary> Asynchronously reads the next item from the message payload. </summary>
        /// <returns>A task that when completed indicates whether more items were read.</returns>
        public abstract Task<bool> ReadAsync();
#endif
    }
}
