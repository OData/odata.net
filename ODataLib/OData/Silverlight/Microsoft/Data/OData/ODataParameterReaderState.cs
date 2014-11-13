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
    /// <summary> Enumeration of all possible states of an <see cref="T:Microsoft.Data.OData.ODataParameterReader" />. </summary>
    public enum ODataParameterReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state the Name and Value properties of the <see cref="ODataParameterReader"/> returns null.</remarks>
        Start,

        /// <summary>The reader read a primitive or a complex parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns the value read (e.g. a primitive value, an ODataComplexValue or null).</remarks>
        Value,

#if SUPPORT_ENTITY_PARAMETER
        /// <summary>The reader is reading an entry parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. The CreateEntryReader() method on the <see cref="ODataParameterReader"/>
        /// must be called to get the reader to read the entry value.</remarks>
        Entry,

        /// <summary>The reader is reading a feed parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns null. The CreateFeedReader() method on the <see cref="ODataParameterReader"/>
        /// must be called to get the reader to read the feed value.</remarks>
        Feed,
#endif

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
    }
}
