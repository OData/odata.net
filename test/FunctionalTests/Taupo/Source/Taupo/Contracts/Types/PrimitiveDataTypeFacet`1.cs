//---------------------------------------------------------------------
// <copyright file="PrimitiveDataTypeFacet`1.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.Types
{
    using System;

    /// <summary>
    /// Facet of a <see cref="PrimitiveDataType"/> parameterized with a single value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public abstract class PrimitiveDataTypeFacet<TValue> : PrimitiveDataTypeFacet
    {
        /// <summary>
        /// Initializes a new instance of the PrimitiveDataTypeFacet class.
        /// </summary>
        /// <param name="value">The value of the facet.</param>
        protected PrimitiveDataTypeFacet(TValue value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value facet.
        /// </summary>
        /// <value>The facet value.</value>
        public TValue Value { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            var simpleName = this.GetType().Name;
            if (simpleName.EndsWith("Facet", StringComparison.Ordinal))
            {
                simpleName = simpleName.Substring(0, simpleName.Length - 5);
            }

            return simpleName + "=" + this.Value;
        }
    }
}
