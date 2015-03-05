//---------------------------------------------------------------------
// <copyright file="UriQueryConstants.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

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

        /// <summary>An order by query option name.</summary>
        internal const string OrderByQueryOption = "$orderby";

        /// <summary>A select query option name.</summary>
        internal const string SelectQueryOption = "$select";

        /// <summary>An expand query option name.</summary>
        internal const string ExpandQueryOption = "$expand";

        /// <summary>A skip query option name.</summary>
        internal const string SkipQueryOption = "$skip";

        /// <summary>A skip token query option name.</summary>
        internal const string SkipTokenQueryOption = "$skipToken";

        /// <summary>A delta token query option name.</summary>
        internal const string DeltaTokenQueryOption = "$deltatoken";

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