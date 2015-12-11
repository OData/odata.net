﻿//---------------------------------------------------------------------
// <copyright file="IODataFeedAndEntryTypeContext.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core
{
    using Microsoft.OData.Core.Evaluation;
    using Microsoft.OData.Edm;

    /// <summary>
    /// Interface used for substitutability, to answer basic questions regarding the type of the entry or feed.
    /// Metadata may come from a user-provided model or from the SetSerializationInfo() method on a feed or entry. The latter is considered the "no-model" case since only strings
    /// are provided, and there is no interconnectedness.  The goal of this interface is to provide a way to query the metadata information available on an entry or feed without
    /// needing to know where the metadata originated from.
    /// </summary>
    internal interface IODataFeedAndEntryTypeContext
    {
        /// <summary>
        /// The navigation source name of the feed or entry.
        /// </summary>
        string NavigationSourceName { get; }

        /// <summary>
        /// The entity type name of the navigation source of the feed or entry.
        /// </summary>
        string NavigationSourceEntityTypeName { get; }

        /// <summary>
        /// The full type name of the navigation source of the feed or entry.
        /// </summary>
        string NavigationSourceFullTypeName { get; }

        /// <summary>
        /// The kind of the navigation source of the feed or entry.
        /// </summary>
        EdmNavigationSourceKind NavigationSourceKind { get; }

        /// <summary>
        /// The expected entity type name of the entry.
        /// For example, in the request URI 'http://example.com/Service.svc/People/Namespace.VIP_Person', the expected entity type is Namespace.VIP_Person.
        /// (The entity set element type name in this example may be Person, and the actual entity type of a particular entity might be a type more derived than VIP_Person)
        /// </summary>
        string ExpectedEntityTypeName { get; }

        /// <summary>
        /// true if the entry is an MLE, false otherwise.
        /// </summary>
        bool IsMediaLinkEntry { get; }

        /// <summary>
        /// The flag we use to identify if the current entry is from a navigation property with collection type or not.
        /// </summary>
        bool IsFromCollection { get; }

        /// <summary>
        /// The Url convention to use for the entity set.
        /// </summary>
        UrlConvention UrlConvention { get; }
    }
}