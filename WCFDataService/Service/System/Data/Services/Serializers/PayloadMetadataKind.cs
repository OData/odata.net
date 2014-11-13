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

namespace System.Data.Services.Serializers
{
    /// <summary>
    /// Container class for a set of enumerations for payload metadatada.
    /// </summary>
    internal static class PayloadMetadataKind
    {
        /// <summary>
        /// Enumeration of payload metadata kinds for navigation links.
        /// </summary>
        internal enum Navigation
        {
            /// <summary>
            /// The 'Url' property of a navigation link.
            /// </summary>
            Url,

            /// <summary>
            /// The 'AssociationLinkUrl' property of a navigation link.
            /// </summary>
            AssociationLinkUrl
        }

        /// <summary>
        /// Enumeration of payload metadata kinds for feeds.
        /// </summary>
        internal enum Feed
        {
            /// <summary>
            /// The 'Id' property of a feed.
            /// </summary>
            Id
        }

        /// <summary>
        /// Enumeration of payload metadata kinds for entries.
        /// </summary>
        internal enum Entry
        {
            /// <summary>
            /// The 'Id' property of an entry.
            /// </summary>
            Id,

            /// <summary>
            /// The 'TypeName' property of an entry.
            /// </summary>
            TypeName,

            /// <summary>
            /// The 'EditLink' property of an entry.
            /// </summary>
            EditLink,

            /// <summary>
            /// The 'ETag' property of an entry.
            /// </summary>
            ETag,
        }

        /// <summary>
        /// Enumeration of payload metadata kinds for association links.
        /// </summary>
        internal enum Association
        {
            /// <summary>
            /// The 'Url' property of an association link.
            /// </summary>
            Url,
        }

        /// <summary>
        /// Enumeration of payload metadata kinds for streams.
        /// </summary>
        internal enum Stream
        {
            /// <summary>
            /// The 'EditLink' property of a stream.
            /// </summary>
            EditLink,

            /// <summary>
            /// The 'ReadLink' property of a stream.
            /// </summary>
            ReadLink,

            /// <summary>
            /// The 'ContentType' property of a stream.
            /// </summary>
            ContentType,

            /// <summary>
            /// The 'ETag' property of a stream.
            /// </summary>
            ETag
        }

        /// <summary>
        /// Enumeration of payload metadata kinds for actions/functions.
        /// </summary>
        internal enum Operation
        {
            /// <summary>
            /// The 'Title' property of an operation.
            /// </summary>
            Title,

            /// <summary>
            /// The 'Target' property of an operation.
            /// </summary>
            Target
        }
    }
}
