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

namespace Microsoft.Data.OData.Query
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
        internal const string LinkSegment = "$links";

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

        /// <summary>A valid value to denote all-properties access.</summary>
        internal const string Star = "*";

        /// <summary>A top query option name.</summary>
        internal const string TopQueryOption = "$top";

        /// <summary>A inline-count query option name.</summary>
        internal const string InlineCountQueryOption = "$inlinecount";

        /// <summary>A format query option name.</summary>
        internal const string FormatQueryOption = "$format";
    }
}
