//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core
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

        /// <summary>An entity reference link was read.</summary>
        /// <remarks>
        /// In this state the Item property of the <see cref="ODataReader"/> returns
        /// an <see cref="ODataEntityReferenceLink"/> which is fully populated.
        /// Note that there's no End state for this item.
        /// </remarks>
        EntityReferenceLink,

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
