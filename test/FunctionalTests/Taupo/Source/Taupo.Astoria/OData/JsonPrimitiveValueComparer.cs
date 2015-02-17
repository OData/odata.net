//---------------------------------------------------------------------
// <copyright file="JsonPrimitiveValueComparer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.OData
{
    using System;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;

    /// <summary>
    /// The primitive comparer for Json
    /// </summary>
    public class JsonPrimitiveValueComparer : QueryScalarValueToClrValueComparerBase
    {
        /// <summary>
        /// Gets or sets the primitive converter to use
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IJsonPrimitiveConverter Converter { get; set; }
        
        /// <summary>
        /// Compares the given query scalar value to the given clr value, and throws a DataComparisonException if they dont match
        /// </summary>
        /// <param name="expected">expected query primitive value to compare</param>
        /// <param name="actual">actual CLR value</param>
        /// <param name="assert">The assertion handler to use</param>
        public override void Compare(QueryScalarValue expected, object actual, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(expected, "expected");

            if (expected.IsDynamicPropertyValue() && !(expected.Type is QueryClrSpatialType))
            {
                this.CompareDynamicValues(expected.Type, expected.Value, actual, assert);
            }
            else
            {
                base.Compare(expected, actual, assert);
            }
        }

        /// <summary>
        /// Compares the given clr value to the given query scalar value, and throws a DataComparisonException if they dont match
        /// </summary>
        /// <param name="expected">expected CLR value</param>
        /// <param name="actual">actual query primitive value to compare</param>
        /// <param name="assert">The assertion handler to use</param>
        public override void Compare(object expected, QueryScalarValue actual, AssertionHandler assert)
        {
            ExceptionUtilities.CheckArgumentNotNull(actual, "actual");

            if (actual.IsDynamicPropertyValue() && !(actual.Type is QueryClrSpatialType))
            {
                this.CompareDynamicValues(actual.Type, expected, actual.Value, assert);
            }
            else
            {
                base.Compare(expected, actual, assert);
            }
        }

        internal void CompareDynamicValues(QueryType queryType, object expected, object actual, AssertionHandler assert)
        {
            var expectedValue = expected;
            var actualValue = actual;

            if (expected is DateTime && actual is DateTime)
            {
                expectedValue = this.Converter.Normalize(expectedValue);
                actualValue = this.Converter.Normalize(actualValue);
            }

            var expectedString = this.Converter.SerializePrimitive(expectedValue);
            var actualString = this.Converter.SerializePrimitive(actualValue);

            base.Compare(
                new QueryClrPrimitiveType(typeof(string), queryType.EvaluationStrategy),
                expectedString,
                actualString,
                assert);
        }

        /// <summary>
        /// Compares the given values of the given type, and throws a DataComparisonException or AssertionFailedException if values don't match
        /// </summary>
        /// <param name="type">The expected type</param>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="assert">The assertion handler to use</param>
        protected override void Compare(QueryScalarType type, object expected, object actual, AssertionHandler assert)
        {
            expected = this.Converter.Normalize(expected);
            actual = this.Converter.Normalize(actual);

            base.Compare(type, expected, actual, assert);
        }
    }
}
