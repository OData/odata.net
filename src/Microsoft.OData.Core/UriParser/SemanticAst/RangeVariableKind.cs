//---------------------------------------------------------------------
// <copyright file="RangeVariableKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    /// <summary>
    /// Enumeration of the different kinds of RangeVariables.
    /// </summary>
    public static class RangeVariableKind
    {
        /// <summary>
        /// A range variable that refers to entity type or a complex.
        /// </summary>
        public const int Resource = 0;

        /// <summary>
        /// A range variable that refers to non-entity/complex types.
        /// </summary>
        public const int NonResource = 1;
    }
}