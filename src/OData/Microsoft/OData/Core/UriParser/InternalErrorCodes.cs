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
    /// An enumeration that lists the internal errors.
    /// </summary>
    internal enum InternalErrorCodes
    {
        /// <summary>Unreachable codepath in TypePromotionUtils.GetFunctionSignatures(BinaryOperatorKind), unrecognized kind of binary operator.</summary>
        TypePromotionUtils_GetFunctionSignatures_Binary_UnreachableCodepath,

        /// <summary>Unreachable codepath in TypePromotionUtils.GetFunctionSignatures(UnaryOperatorKind), unrecognized kind of unary operator.</summary>
        TypePromotionUtils_GetFunctionSignatures_Unary_UnreachableCodepath,

        /// <summary>Unreachable codepath in UriQueryExpressionParser.ParseComparison</summary>
        /// <remarks>Was a new binary operator keyword without adding it to the switch in the ParseComparison?</remarks>
        UriQueryExpressionParser_ParseComparison,

        /// <summary>Unreachable codepath in UriPrimitiveTypeParser.TryUriStringToPrimitive</summary>
        /// <remarks>Unsupported type was asked to be parsed.</remarks>
        UriPrimitiveTypeParser_TryUriStringToPrimitive,

        /// <summary>Unreachable codepath in QueryNodeUtils.BinaryOperatorResultType, unrecognized kind of binary operator.</summary>
        QueryNodeUtils_BinaryOperatorResultType_UnreachableCodepath,
    }
}
