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
    /// <summary>
    /// Enumeration of all possible states of an <see cref="ODataReader" />.
    /// </summary>
    public enum ODataReaderState
    {
        /// <summary>The reader is at the start; nothing has been read yet.</summary>
        /// <remarks>In this state the Item property of the <see cref="ODataReader"/> returns null.</remarks>
        Start,

        /// <summary>The start of a feed has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns 
        /// an <see cref="ODataFeed"/> but no properties may be filled in until the FeedEnd state is reached.
        /// </remarks>
        FeedStart,

        /// <summary>The end of a feed has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns 
        /// an <see cref="ODataFeed"/> with all properties filled in.
        /// </remarks>
        FeedEnd,

        /// <summary>The start of an entry has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns 
        /// an <see cref="ODataEntry"/> but no properties may be filled in until the EntryEnd state is reached.
        /// </remarks>
        EntryStart,

        /// <summary>The end of an entry has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns 
        /// an <see cref="ODataEntry"/> with all properties filled in.
        /// </remarks>
        EntryEnd,

        /// <summary>The start of a navigation link has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns 
        /// an <see cref="ODataNavigationLink"/> but no properties may be filled in until the LinkEnd state is reached.
        /// </remarks>
        NavigationLinkStart,

        /// <summary>The end of a navigation link has been read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns 
        /// an <see cref="ODataNavigationLink"/> with all properties filled in.
        /// </remarks>
        NavigationLinkEnd,

        /// <summary>The reader has thrown an exception; nothing can be read from the reader anymore.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns null.
        /// </remarks>
        Exception,

        /// <summary>The reader has completed; nothing can be read anymore.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns null.
        /// </remarks>
        Completed,
    }
}
