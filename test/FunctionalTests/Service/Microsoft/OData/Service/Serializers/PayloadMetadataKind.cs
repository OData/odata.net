//---------------------------------------------------------------------
// <copyright file="PayloadMetadataKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Serializers
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