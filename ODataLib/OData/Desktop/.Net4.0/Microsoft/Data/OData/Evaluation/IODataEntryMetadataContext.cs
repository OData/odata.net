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

namespace Microsoft.Data.OData.Evaluation
{
    using System.Collections.Generic;
    using Microsoft.Data.Edm;

    /// <summary>
    /// Interface used for substitutability of the metadata-centric responsibilities of an entry.
    /// Metadata may come from a user-provided model or from the SetSerializationInfo() method on a feed or entry. The latter is considered the "no-model" case since only strings
    /// are provided, and there is no interconnectedness. The goal of this interface is to provide a way to query the metadata information available on an entry without
    /// needing to know where the metadata originated from.
    /// </summary>
    internal interface IODataEntryMetadataContext
    {
        /// <summary>
        /// The entry instance.
        /// </summary>
        ODataEntry Entry { get; }

        /// <summary>
        /// The context object to answer basic questions regarding the type of the entry.
        /// </summary>
        IODataFeedAndEntryTypeContext TypeContext { get; }

        /// <summary>
        /// The actual entity type of the entry, i.e. ODataEntry.TypeName.
        /// </summary>
        string ActualEntityTypeName { get; }

        /// <summary>
        /// The key property name and value pairs of the entry.
        /// </summary>
        ICollection<KeyValuePair<string, object>> KeyProperties { get; }

        /// <summary>
        /// The ETag property name and value pairs of the entry.
        /// </summary>
        IEnumerable<KeyValuePair<string, object>> ETagProperties { get; }

        /// <summary>
        /// The selected navigation properties.
        /// </summary>
        IEnumerable<IEdmNavigationProperty> SelectedNavigationProperties { get; }

        /// <summary>
        /// The selected stream properties.
        /// </summary>
        IDictionary<string, IEdmStructuralProperty> SelectedStreamProperties { get; }

        /// <summary>
        /// The selected always bindable operations.
        /// </summary>
        IEnumerable<IEdmFunctionImport> SelectedAlwaysBindableOperations { get; }
    }
}
