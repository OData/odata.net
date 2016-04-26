//---------------------------------------------------------------------
// <copyright file="UnaryOperatorKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    #region Namespaces
    #endregion Namespaces

    /// <summary>
    /// Enumeration of binary operators.
    /// </summary>
    public enum UnaryOperatorKind
    {
        /// <summary>
        /// The unary - operator.
        /// </summary>
        Negate = 0,

        /// <summary>
        /// The not operator.
        /// </summary>
        Not = 1,
    }
}