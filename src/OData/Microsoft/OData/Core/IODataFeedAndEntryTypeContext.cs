//   OData .NET Libraries ver. 6.9
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
