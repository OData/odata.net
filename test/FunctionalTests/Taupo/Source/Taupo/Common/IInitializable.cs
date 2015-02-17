//---------------------------------------------------------------------
// <copyright file="IInitializable.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Common
{
    /// <summary>
    /// Initializes the object.
    /// </summary>
    public interface IInitializable
    {
        /// <summary>
        /// Initializes this object (after construction and property assignment but before use).
        /// </summary>
        void Initialize();
    }
}