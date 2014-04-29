//   OData .NET Libraries
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 

//   Licensed under the Apache License, Version 2.0 (the ""License""); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0 

//   THIS CODE IS PROVIDED ON AN  *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABLITY OR NON-INFRINGEMENT. 

//   See the Apache Version 2.0 License for specific language governing permissions and limitations under the License.

namespace Microsoft.OData.Core.UriParser.TreeNodeKinds
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Enumeration of binary operators.
    /// </summary>
    public enum BinaryOperatorKind
    {
        /// <summary>
        /// The logical or operator.
        /// </summary>
        Or = 0,

        /// <summary>
        /// The logical and operator.
        /// </summary>
        And = 1,

        /// <summary>
        /// The eq operator.
        /// </summary>
        Equal = 2,

        /// <summary>
        /// The ne operator.
        /// </summary>
        NotEqual = 3,

        /// <summary>
        /// The gt operator.
        /// </summary>
        GreaterThan = 4,

        /// <summary>
        /// The ge operator.
        /// </summary>
        GreaterThanOrEqual = 5,

        /// <summary>
        /// The lt operator.
        /// </summary>
        LessThan = 6,

        /// <summary>
        /// The le operator.
        /// </summary>
        LessThanOrEqual = 7,

        /// <summary>
        /// The add operator.
        /// </summary>
        Add = 8,

        /// <summary>
        /// The sub operator.
        /// </summary>
        Subtract = 9,

        /// <summary>
        /// The mul operator.
        /// </summary>
        Multiply = 10,

        /// <summary>
        /// The div operator.
        /// </summary>
        Divide = 11,

        /// <summary>
        /// The mod operator.
        /// </summary>
        Modulo = 12,

        /// <summary>
        /// The has operator.
        /// </summary>
        Has = 13,
    }
}
