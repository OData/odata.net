//---------------------------------------------------------------------
// <copyright file="IQueryStructuralValueToNamedValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// Contract for comparing a query structural value to a set of named clr values
    /// </summary>
    [ImplementationSelector("IQueryStructuralValueToNamedValueComparer", DefaultImplementation = "Default")]
    public interface IQueryStructuralValueToNamedValueComparer
    {
        /// <summary>
        /// Compares a queryStructualValue to namedValues
        /// Throws a DataComparisonException if values don't match
        /// </summary>
        /// <param name="queryStructuralValue">QueryStructural Value to compare</param>
        /// <param name="namedValues">NamedValues to compare</param>
        /// <param name="scalarComparer">The comparer to use for scalar values</param>
        void Compare(QueryStructuralValue queryStructuralValue, IEnumerable<NamedValue> namedValues, IQueryScalarValueToClrValueComparer scalarComparer);
    }
}
