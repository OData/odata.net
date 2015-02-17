//---------------------------------------------------------------------
// <copyright file="QueryUnresolvedScalarType.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using System;
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Unresolved query scalar type, used during query tree construction, must be replaced with a real QueryType before the tree can be used further. 
    /// Mainly used for unresolved constants.
    /// </summary>
    internal sealed class QueryUnresolvedScalarType : QueryScalarType
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="QueryUnresolvedScalarType"/> class from being created.
        /// </summary>
        private QueryUnresolvedScalarType()
            : base(DummyQueryEvaluationStrategy.Instance)
        {
        }
        
        /// <summary>
        /// Gets string representation of the given query type.
        /// </summary>
        public override string StringRepresentation
        {
            get 
            {
                return "UnresolvedPrimitive"; 
            }
        }

        /// <summary>
        /// Gets a value indicating whether this type is unresolved.
        /// </summary>
        public override bool IsUnresolved
        {
            get { return true; }
        }

        /// <summary>
        /// Gets an instance of QueryUnresolvedScalarType.
        /// </summary>
        internal static QueryUnresolvedScalarType Instance 
        {
            get { return Nested.Instance; } 
        }

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query type.</param>
        /// <returns>The result of visiting this query type.</returns>
        public override TResult Accept<TResult>(IQueryTypeVisitor<TResult> visitor)
        {
            throw new TaupoInvalidOperationException("Invalid Operation for unresolved type.");
        }

        /// <summary>
        /// Gets the non-strongly typed collection type for this type.
        /// </summary>
        /// <returns>Instance of <see cref="QueryCollectionType"/> which is a collection of this type.</returns>
        protected override QueryCollectionType CreateCollectionTypeInternal()
        {
            throw new TaupoNotSupportedException("Operation not supported for the unresolved type.");
        }

        /// <summary>
        /// Gets the non-strongly typed null value.
        /// </summary>
        /// <returns>Null value.</returns>
        protected override QueryValue GetNullValueInternal()
        {
            throw new TaupoNotSupportedException("Operation not supported for the unresolved type.");
        }

        /// <summary>
        /// Creates the non-strongly typed error value.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <returns>Created error value.</returns>
        protected override QueryValue CreateErrorValueInternal(QueryError evaluationError)
        {
            throw new TaupoNotSupportedException("Operation not supported for the unresolved type.");
        }

        /// <summary>
        /// Nested class for lazy instantiation
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Used for lazily instantiated singleton.")]
        private class Nested
        {
            internal static readonly QueryUnresolvedScalarType Instance = new QueryUnresolvedScalarType();
        }
    }
}
