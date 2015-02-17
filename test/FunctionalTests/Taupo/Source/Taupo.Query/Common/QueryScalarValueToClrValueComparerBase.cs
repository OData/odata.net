//---------------------------------------------------------------------
// <copyright file="QueryScalarValueToClrValueComparerBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Common
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// Base implementation of contract to compare a query scalar value to a clr value
    /// </summary>
    public abstract class QueryScalarValueToClrValueComparerBase : IQueryScalarValueToClrValueComparer
    {
        /// <summary>
        /// Compares the given query scalar value to the given clr value, and throws a DataComparisonException if they dont match
        /// </summary>
        /// <param name="expected">expected query primitive value to compare</param>
        /// <param name="actual">actual CLR value</param>
        /// <param name="assert">The assertion handler to use</param>
        public virtual void Compare(QueryScalarValue expected, object actual, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");
            ExceptionUtilities.CheckArgumentNotNull(assert, "assert");
            this.Compare(expected.Type, expected.Value, actual, assert);
        }

        /// <summary>
        /// Compares the given clr value to the given query scalar value, and throws a DataComparisonException if they dont match
        /// </summary>
        /// <param name="expected">expected CLR value</param>
        /// <param name="actual">actual query primitive value to compare</param>
        /// <param name="assert">The assertion handler to use</param>
        public virtual void Compare(object expected, QueryScalarValue actual, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(actual, "actual");
            ExceptionUtilities.CheckArgumentNotNull(assert, "assert");
            this.Compare(actual.Type, expected, actual.Value, assert);
        }

        /// <summary>
        /// Compares the given values of the given type, and throws a DataComparisonException or AssertionFailedException if values don't match
        /// </summary>
        /// <param name="type">The expected type</param>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="assert">The assertion handler to use</param>
        protected virtual void Compare(QueryScalarType type, object expected, object actual, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(type, "type");
            ExceptionUtilities.CheckArgumentNotNull(assert, "assert");

            if (expected == type.NullValue.Value)
            {
                assert.IsNull(actual, "Primitive value unexpectedly non-null");
            }
            else
            {
                assert.IsNotNull(actual, "Primitive value unexpectedly null");

                assert.AreEqual(expected.GetType(), actual.GetType(), EqualityComparer<Type>.Default, "Types did not match");

                var comparer = new DelegateBasedEqualityComparer<QueryScalarValue>((v1, v2) => v1.Type.EvaluationStrategy.Compare(v1, v2) == 0);
                assert.AreEqual(type.CreateValue(expected), type.CreateValue(actual), comparer, "Primitive value did not match");
            }
        }
    }
}
