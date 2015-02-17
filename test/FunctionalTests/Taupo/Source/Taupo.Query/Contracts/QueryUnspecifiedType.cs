//---------------------------------------------------------------------
// <copyright file="QueryUnspecifiedType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// For use exclusively by untyped null values.
    /// </summary>
    public sealed class QueryUnspecifiedType : QueryScalarType
    {
        /// <summary>
        /// Prevents a default instance of the QueryUnspecifiedType class from being created.
        /// </summary>
        private QueryUnspecifiedType()
            : base(DummyQueryEvaluationStrategy.Instance)
        {
        }

        /// <summary>
        /// Gets the instance of the QueryUnspecifiedType class.
        /// </summary>
        public static QueryUnspecifiedType Instance
        {
            get
            {
                return Nested.Instance;
            }
        }

        /// <summary>
        /// Gets string representation of the given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get 
            {
                return "Unspecified"; 
            }
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            throw new TaupoInvalidOperationException("Invalid Operation for unspecified type.");
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            throw new TaupoNotSupportedException("Operation not supported for the unspecified type.");
        }

        /// <summary>
        /// Nested class for lazy instantiation
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used for lazily instantiated singleton.")]
        private class Nested
        {
            internal static readonly QueryUnspecifiedType Instance = new QueryUnspecifiedType();
        }
    }
}
