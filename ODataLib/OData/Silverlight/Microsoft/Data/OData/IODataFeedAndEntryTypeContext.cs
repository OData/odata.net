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
    using Microsoft.Data.OData.Evaluation;

    /// <summary>
    /// Interface used for substitutability, to answer basic questions regarding the type of the entry or feed.
    /// Metadata may come from a user-provided model or from the SetSerializationInfo() method on a feed or entry. The latter is considered the "no-model" case since only strings
    /// are provided, and there is no interconnectedness.  The goal of this interface is to provide a way to query the metadata information available on an entry or feed without
    /// needing to know where the metadata originated from.
    /// </summary>
    internal interface IODataFeedAndEntryTypeContext
    {
        /// <summary>
        /// The entity set name of the feed or entry.
        /// </summary>
        string EntitySetName { get; }

        /// <summary>
        /// The element type name of the entity set of the feed or entry.
        /// </summary>
        string EntitySetElementTypeName { get; }

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
        /// The Url convention to use for the entity set.
        /// </summary>
        UrlConvention UrlConvention { get; }
    }
}
