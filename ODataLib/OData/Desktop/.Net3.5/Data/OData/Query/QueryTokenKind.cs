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
    /// Enumeration of kinds of query tokens.
    /// </summary>
    internal enum QueryTokenKind
    {
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
        EndPath = 7,

        /// <summary>
        /// The order by operation.
        /// </summary>
        OrderBy = 8,

        /// <summary>
        /// A query option.
        /// </summary>
        CustomQueryOption = 9,

        /// <summary>
        /// The Select query.
        /// </summary>
        Select = 10,

        /// <summary>
        /// The *.
        /// </summary>
        Star = 11,

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
        InnerPath = 16,

        /// <summary>
        /// type segment.
        /// </summary>
        DottedIdentifier = 17,

        /// <summary>
        /// Parameter token.
        /// </summary>
        RangeVariable = 18,

        /// <summary>
        /// All query.
        /// </summary>
        All = 19,

        /// <summary>
        /// ExpandTerm Token
        /// </summary>
        ExpandTerm = 20,

        /// <summary>
        /// FunctionParameterToken
        /// </summary>
        FunctionParameter = 21,

        /// <summary>
        /// FunctionParameterAlias
        /// </summary>
        FunctionParameterAlias = 22,

        /// <summary>
        /// RawFunctionParameterValue
        /// </summary>
        RawFunctionParameterValue = 23,
    }
}
