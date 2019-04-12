//---------------------------------------------------------------------
// <copyright file="Scope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Various scope types for writer.
    /// </summary>
    internal enum ScopeType
    {
        /// <summary>
        /// Object scope.
        /// </summary>
        Object = 0,

        /// <summary>
        /// Array scope.
        /// </summary>
        Array = 1,
    }

    /// <summary>
    /// Class representing scope information.
    /// </summary>
    internal sealed class Scope
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">The type of the scope.</param>
        public Scope(ScopeType type)
        {
            ScopeType = type;
        }

        /// <summary>
        /// Gets/Sets the object count for this scope.
        /// </summary>
        public int ObjectCount { get; set; }

        /// <summary>
        /// Gets the scope type for this scope.
        /// </summary>
        public ScopeType ScopeType { get; private set; }

        /// <summary>
        /// Gets/Sets the value whether it is in previous array scope.
        /// </summary>
        public bool IsInArray { get; set; } = false;
    }
}