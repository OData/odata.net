//---------------------------------------------------------------------
// <copyright file="PrimitiveDataTypeFacet.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    /// <summary>
    /// Base class for all facets of <see cref="PrimitiveDataType"/>.
    /// </summary>
    public abstract class PrimitiveDataTypeFacet
    {
        /// <summary>
        /// Gets a value indicating whether this instance is volatile.
        /// </summary>
        /// <value>
        /// A value <c>true</c> if this instance is volatile; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>Facets which are volatile are not preserved when extending primitive types with non-volatile facets.</remarks>
        protected internal virtual bool IsVolatile
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public abstract override string ToString();
    }
}
