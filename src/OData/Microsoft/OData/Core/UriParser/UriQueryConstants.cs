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

namespace Microsoft.OData.Core.UriParser
{
    /// <summary>
    /// Constant values related to the URI query syntax.
    /// </summary>
    internal static class UriQueryConstants
    {
        /// <summary>A segment name in a URI that indicates metadata is being requested.</summary>
        internal const string MetadataSegment = "$metadata";

        /// <summary>A segment name in a URI that indicates a plain primitive value is being requested.</summary>
        internal const string ValueSegment = "$value";

        /// <summary>A segment name in a URI that indicates batch is being requested.</summary>
        internal const string BatchSegment = "$batch";

        /// <summary>A segment name in a URI that indicates that this is an entity reference link operation.</summary>
        internal const string RefSegment = "$ref";

        /// <summary>A segment name in a URI that indicates that this is a count operation.</summary>
        internal const string CountSegment = "$count";

        /// <summary>A filter query option name.</summary>
        internal const string FilterQueryOption = "$filter";

        /// <summary>An apply query option name.</summary>
        internal const string ApplyQueryOption = "$apply";

        /// <summary>An aggregate transformation</summary>
        internal const string AggregateTransformation = "aggregate";

        /// <summary>A group-by transformation</summary>
        internal const string GroupbyTransformation = "groupby";

        /// <summary>A filter transformation</summary>
        internal const string FilterTransformation = "filter";

        /// <summary>An order by query option name.</summary>
        internal const string OrderByQueryOption = "$orderby";

        /// <summary>A select query option name.</summary>
        internal const string SelectQueryOption = "$select";

        /// <summary>An expand query option name.</summary>
        internal const string ExpandQueryOption = "$expand";

        /// <summary>A skip query option name.</summary>
        internal const string SkipQueryOption = "$skip";

        /// <summary>An entity id query option name. </summary>
        internal const string IdQueryOption = "$id";

        /// <summary>A valid value to denote all-properties access.</summary>
        internal const string Star = "*";

        /// <summary>A top query option name.</summary>
        internal const string TopQueryOption = "$top";

        /// <summary>A count query option name.</summary>
        internal const string CountQueryOption = "$count";

        /// <summary>A format query option name.</summary>
        internal const string FormatQueryOption = "$format";

        /// <summary>A search query option name.</summary>
        internal const string SearchQueryOption = "$search";
    }
}
