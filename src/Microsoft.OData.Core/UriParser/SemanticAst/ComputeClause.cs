//---------------------------------------------------------------------
// <copyright file="ComputeClause.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.UriParser
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The result of parsing a $compute query option.
    /// </summary>
    public sealed class ComputeClause
    {
        /// <summary>
        /// The computed properties and operations.
        /// </summary>
        private ReadOnlyCollection<ComputeExpression> computedItems;

        /// <summary>
        /// Constructs a <see cref="ComputeClause"/> from the given parameters.
        /// </summary>
        /// <param name="computedItems">The computed properties and operations.</param>
        public ComputeClause(IEnumerable<ComputeExpression> computedItems)
        {
            this.computedItems = computedItems != null ?
                new ReadOnlyCollection<ComputeExpression>(computedItems.ToList()) :
                new ReadOnlyCollection<ComputeExpression>(new List<ComputeExpression>());
        }

        /// <summary>
        /// Gets the computed properties and operations.
        /// </summary>
        public IEnumerable<ComputeExpression> ComputedItems
        {
            get
            {
                return this.computedItems.AsEnumerable();
            }
        }
    }
}
