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

namespace System.Data.OData.Query
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
        /// The root, the entire query.
        /// </summary>
        QueryDescriptor = 1,

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
    }
}
