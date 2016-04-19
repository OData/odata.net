//---------------------------------------------------------------------
// <copyright file="RangeVariableKind.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Core.UriParser
{
    /// <summary>
    /// Enumeration of the different kinds of RangeVariables.
    /// </summary>
    public static class RangeVariableKind
    {
        /// <summary>
        /// A range variable that referrs to entity types.
        /// </summary>
        public const int Entity = 0;

        /// <summary>
        /// A range variable that referrs to non-entity types.
        /// </summary>
        public const int Nonentity = 1;
    }
}