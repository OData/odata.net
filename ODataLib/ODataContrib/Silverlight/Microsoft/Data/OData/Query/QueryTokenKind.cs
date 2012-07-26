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

namespace Microsoft.Data.OData.Query
{
    /// <summary>
    /// Enumeration of kinds of query tokens.
    /// </summary>
#if INTERNAL_DROP
    internal enum QueryTokenKind
#else
    public enum QueryTokenKind
#endif
    {
        /// <summary>
        /// An extension token, undefined by this library.
        /// </summary>
        Extension = 0,

        /// <summary>
        /// The segment of a query path.
        /// </summary>
        Segment = 2,

        /// <summary>
        /// The binary operator.
        /// </summary>
        BinaryOperator = 3,

        /// <summary>
        /// The unary operator.
        /// </summary>
        UnaryOperator = 4,

        /// <summary>
        /// The literal value.
        /// </summary>
        Literal = 5,

        /// <summary>
        /// The function call.
        /// </summary>
        FunctionCall = 6,

        /// <summary>
        /// The property access.
        /// </summary>
        PropertyAccess = 7,

        /// <summary>
        /// The order by operation.
        /// </summary>
        OrderBy = 8,

        /// <summary>
        /// A query option.
        /// </summary>
        QueryOption = 9,

        /// <summary>
        /// The Select query.
        /// </summary>
        Select = 10,

        /// <summary>
        /// The *.
        /// </summary>
        Star = 11,

        /// <summary>
        /// The $metadata, $count, $value segment.
        /// </summary>
        KeywordSegment = 12,

        /// <summary>
        /// The Expand query.
        /// </summary>
        Expand = 13,

        /// <summary>
        /// Type segment.
        /// </summary>
        TypeSegment = 14,

        /// <summary>
        /// Any query.
        /// </summary>
        Any = 15,

        /// <summary>
        /// Non root segment.
        /// </summary>
        NonRootSegment = 16,

        /// <summary>
        /// Cast segment.
        /// </summary>
        Cast = 17,

        /// <summary>
        /// Parameter token.
        /// </summary>
        Parameter = 18,

        /// <summary>
        /// All query.
        /// </summary>
        All = 19,
    }
}
