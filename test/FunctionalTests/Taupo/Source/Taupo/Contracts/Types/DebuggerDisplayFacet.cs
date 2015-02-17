//---------------------------------------------------------------------
// <copyright file="DebuggerDisplayFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Specifies a debugger display string which will be used for the primitive type.
    /// </summary>
    public class DebuggerDisplayFacet : PrimitiveDataTypeFacet<string>
    {
        /// <summary>
        /// Initializes a new instance of the DebuggerDisplayFacet class.
        /// </summary>
        /// <param name="debuggerDisplay">The string to be displayed in the debugger.</param>
        public DebuggerDisplayFacet(string debuggerDisplay)
            : base(debuggerDisplay)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this instance is volatile.
        /// </summary>
        /// <value>
        /// A value <c>true</c> if this instance is volatile; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Facets which are volatile are not preserved when extending primitive types with non-volatile facets.</remarks>
        protected internal override bool IsVolatile
        {
            get { return true; }
        }
    }
}