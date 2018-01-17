//---------------------------------------------------------------------
// <copyright file="IEdmFunction.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm
{
    /// <summary>
    /// Represents an EDM function.
    /// </summary>
    public interface IEdmFunction : IEdmOperation
    {
        /// <summary>
        /// Gets a value indicating whether this instance is composable.
        /// </summary>
        bool IsComposable { get; }
    }
}
