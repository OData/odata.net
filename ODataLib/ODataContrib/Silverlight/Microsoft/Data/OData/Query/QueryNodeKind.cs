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
    /// Enumeration of kinds of query nodes.
    /// </summary>
#if INTERNAL_DROP
    internal enum QueryNodeKind
#else
    public enum QueryNodeKind
#endif
    {
        /// <summary>
        /// An extension node, undefined by this library.
        /// </summary>
        Extension = 0,

        /// <summary>
        /// The query descriptor.
        /// </summary>
        QueryDescriptor = 1,

        /// <summary>
        /// The entity set node.
        /// </summary>
        EntitySet = 2,

        /// <summary>
        /// The key lookup on a collection.
        /// </summary>
        KeyLookup = 3,

        /// <summary>
        /// The constant value.
        /// </summary>
        Constant = 4,

        /// <summary>
        /// The convert of primitive type.
        /// </summary>
        Convert = 5,

        /// <summary>
        /// Service operation returning a composable collection.
        /// </summary>
        CollectionServiceOperation = 6,

        /// <summary>
        /// Service operation returning a composable single value.
        /// </summary>
        SingleValueServiceOperation = 7,

        /// <summary>
        /// Service operation which returns a value which can't be composed.
        /// </summary>
        UncomposableServiceOperation = 8,

        /// <summary>
        /// Filter operation on a collection.
        /// </summary>
        Filter = 9,

        /// <summary>
        /// Parameter node used in expressions.
        /// </summary>
        Parameter = 10,

        /// <summary>
        /// The skip on a collection.
        /// </summary>
        Skip = 11,

        /// <summary>
        /// The top on a collection.
        /// </summary>
        Top = 12,

        /// <summary>
        /// Parameter node used to represent a binary operator.
        /// </summary>
        BinaryOperator = 13,

        /// <summary>
        /// Parameter node used to represent a unary operator.
        /// </summary>
        UnaryOperator = 14,

        /// <summary>
        /// Property access.
        /// </summary>
        PropertyAccess = 15,

        /// <summary>
        /// Order-By.
        /// </summary>
        OrderBy = 16,

        /// <summary>
        /// Function call returning a single value.
        /// </summary>
        SingleValueFunctionCall = 17,

        /// <summary>
        /// Custom query option.
        /// </summary>
        CustomQueryOption = 18,
    }
}
