//---------------------------------------------------------------------
// <copyright file="IEdmEnumMemberValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// The EdmEnumMemberValue interface.
    /// </summary>
    public interface IEdmEnumMemberValue : IEdmElement
    {
        /// <summary>
        /// The value of enum member
        /// </summary>
        long Value { get; }
    }
}
