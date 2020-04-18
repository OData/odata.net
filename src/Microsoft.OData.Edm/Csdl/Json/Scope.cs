//---------------------------------------------------------------------
// <copyright file="Scope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Edm.Csdl.Json
{
    /// <summary>
    /// Various scope types for JSON writer.
    /// </summary>
    internal enum ScopeType
    {
        /// <summary>
        /// Root scope - the top-level of the JSON content.
        /// </summary>
        /// <remarks>This scope is only once on the stack and that is at the bottom, always.
        /// It's used to track the fact that only one top-level value is allowed.</remarks>
        Root,

        /// <summary>
        /// Array scope - inside an array.
        /// </summary>
        /// <remarks>This scope is pushed when [ is found and is active before the first and between the elements in the array.
        /// Between the elements it's active when the parser is in front of the comma, the parser is never after comma as then
        /// it always immediately processed the next token.</remarks>
        Array,

        /// <summary>
        /// Object scope - inside the object (but not in a property value).
        /// </summary>
        /// <remarks>This scope is pushed when { is found and is active before the first and between the properties in the object.
        /// Between the properties it's active when the parser is in front of the comma, the parser is never after comma as then
        /// it always immediately processed the next token.</remarks>
        Object,

        /// <summary>
        /// Property scope - after the property name and colon and throughout the value.
        /// </summary>
        /// <remarks>This scope is pushed when a property name and colon is found.
        /// The scope remains on the stack while the property value is parsed, but once the property value ends, it's immediately removed
        /// so that it doesn't appear on the stack after the value (ever).</remarks>
        Property,
    }

    /// <summary>
    /// Class representing JSON writer scope information.
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
        /// Gets the scope type for this scope.
        /// </summary>
        public ScopeType ScopeType { get; }

        /// <summary>
        /// Gets/Sets the object count for this scope.
        /// </summary>
        public int ObjectCount { get; set; }
    }
}
