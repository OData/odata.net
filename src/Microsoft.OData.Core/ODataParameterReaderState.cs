//---------------------------------------------------------------------
// <copyright file="ODataParameterReaderState.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    /// <summary> Enumeration of all possible states of an <see cref="Microsoft.OData.ODataParameterReader" />. </summary>
    public enum ODataParameterReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state the Name and Value properties of the <see cref="ODataParameterReader"/> returns null.</remarks>
        Start,

        /// <summary>The reader read a primitive or a complex parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns the value read (e.g. a primitive value, or null).</remarks>
        Value,

        /// <summary>The reader is reading a collection parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. The CreateCollectionReader() method on the <see cref="ODataParameterReader"/>
        /// must be called to get the reader to read the collection value.</remarks>
        Collection,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>In this state the Name and Value properties of the <see cref="ODataReader"/> return null.</remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>In this state the Name and Value properties of the <see cref="ODataParameterReader"/> return null.</remarks>
        Completed,

        /// <summary>The reader is reading a resource parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. The CreateResourceReader() method on the <see cref="ODataParameterReader"/>
        /// must be called to get the reader to read the resource value.</remarks>
        Resource,

        /// <summary>The reader is reading a resource set parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. The CreateResourceSetReader() method on the <see cref="ODataParameterReader"/>
        /// must be called to get the reader to read the resource set value.</remarks>
        ResourceSet,
    }
}
