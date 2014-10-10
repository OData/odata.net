//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation. All rights reserved.  
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//       http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

namespace Microsoft.OData.Core
{
    /// <summary> Enumeration of all possible states of an <see cref="T:Microsoft.OData.Core.ODataParameterReader" />. </summary>
    public enum ODataParameterReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state the Name and Value properties of the <see cref="ODataParameterReader"/> returns null.</remarks>
        Start,

        /// <summary>The reader read a primitive or a complex parameter.</summary>
        /// <remarks>In this state the Name property of the <see cref="ODataParameterReader"/> returns the name of the parameter
        /// and the Value property of the <see cref="ODataParameterReader"/> returns the value read (e.g. a primitive value, an ODataComplexValue or null).</remarks>
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
    }
}
