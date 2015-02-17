//---------------------------------------------------------------------
// <copyright file="IQueryScalarValueToClrValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for comparing a query scalar value to a clr value
    /// </summary>
    [ImplementationSelector("QueryScalarValueToClrValueComparer", DefaultImplementation = "Default")]
    public interface IQueryScalarValueToClrValueComparer
    {
        /// <summary>
        /// Compares the given query scalar value to the given clr value, and throws a DataComparisonException if they dont match
        /// </summary>
        /// <param name="expected">expected query primitive value to compare</param>
        /// <param name="actual">actual CLR value</param>
        /// <param name="assert">The assertion handler to use</param>
        void Compare(QueryScalarValue expected, object actual, AssertionHandler assert);

        /// <summary>
        /// Compares the given clr value value to the given query scalar value, and throws a DataComparisonException if they dont match
        /// </summary>
        /// <param name="expected">expected CLR value</param>
        /// <param name="actual">actual query primitive value to compare</param>
        /// <param name="assert">The assertion handler to use</param>
        void Compare(object expected, QueryScalarValue actual, AssertionHandler assert);
    }
}
