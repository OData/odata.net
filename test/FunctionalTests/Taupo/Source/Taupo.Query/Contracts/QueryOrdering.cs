//---------------------------------------------------------------------
// <copyright file="QueryOrdering.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Represents ordering for a given collection (list of sort expressions along with ascending/descending information).
    /// </summary>
    public class QueryOrdering
    {
        private List<Func<object, QueryScalarValue>> selectors = new List<Func<object, QueryScalarValue>>();
        private List<bool> areDescending = new List<bool>();

        /// <summary>
        /// Gets the list of odering key selectors.
        /// </summary>
        internal IEnumerable<Func<object, QueryScalarValue>> Selectors
        {
            get
            {
                return this.selectors.AsEnumerable();
            }
        }

        /// <summary>
        /// Gets the list of sort directions
        /// </summary>
        internal IEnumerable<bool> AreDescending
        {
            get
            {
                return this.areDescending.AsEnumerable();
            }
        }

        /// <summary>
        /// Adds an ascending sort to the list of sort orders.
        /// </summary>
        /// <param name="selector">Value selector for the sort operation.</param>
        /// <returns>This instance (suitable for chaining calls together).</returns>
        public QueryOrdering Ascending(Func<object, QueryScalarValue> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            this.selectors.Add(selector);
            this.areDescending.Add(false);
            return this;
        }

        /// <summary>
        /// Adds an descending sort to the list of sort orders.
        /// </summary>
        /// <param name="selector">Value selector for the sort operation.</param>
        /// <returns>This instance (suitable for chaining calls together).</returns>
        public QueryOrdering Descending(Func<object, QueryScalarValue> selector)
        {
            ExceptionUtilities.CheckArgumentNotNull(selector, "selector");

            this.selectors.Add(selector);
            this.areDescending.Add(true);
            return this;
        }

        /// <summary>
        /// Applies the ordering to the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>Ordered collection.</returns>
        public IEnumerable<object> Apply(IEnumerable<object> source)
        {
            ExceptionUtilities.CheckArgumentNotNull(source, "source");

            if (this.areDescending.Count == 0)
            {
                return source;
            }

            IOrderedEnumerable<object> result;

            if (this.areDescending[0])
            {
                result = source.OrderByDescending(this.selectors[0], QueryScalarValue.Comparer);
            }
            else
            {
                result = source.OrderBy(this.selectors[0], QueryScalarValue.Comparer);
            }

            for (int i = 1; i < this.selectors.Count; ++i)
            {
                if (this.areDescending[i])
                {
                    result = result.ThenByDescending(this.selectors[i], QueryScalarValue.Comparer);
                }
                else
                {
                    result = result.ThenBy(this.selectors[i], QueryScalarValue.Comparer);
                }
            }

            return result;
        }
    }
}
