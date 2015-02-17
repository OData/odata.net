//---------------------------------------------------------------------
// <copyright file="QueryValue.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Query.Contracts
{
    using Microsoft.Test.Taupo.Common;

    /// <summary>
    /// Result of a query evaluation.
    /// </summary>
    public abstract class QueryValue : AnnotatedQueryItem
    {
        /// <summary>
        /// Initializes a new instance of the QueryValue class.
        /// </summary>
        /// <param name="evaluationError">The evaluation error.</param>
        /// <param name="evaluationStrategy">The evaluation strategy.</param>
        protected QueryValue(QueryError evaluationError, IQueryEvaluationStrategy evaluationStrategy)
            : base()
        {
            ExceptionUtilities.CheckArgumentNotNull(evaluationStrategy, "evaluationStrategy");

            this.EvaluationStrategy = evaluationStrategy;
            this.EvaluationError = evaluationError;
        }
        
        /// <summary>
        /// Gets or sets the evaluation error.
        /// </summary>
        /// <value>The error.</value>
        public QueryError EvaluationError { get; set; }

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = "Type is nice and short, ValueType would be confusing here.")]
        public QueryType Type
        {
            get { return this.GetTypeInternal(); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is null.
        /// </summary>
        public abstract bool IsNull { get; }

        /// <summary>
        /// Gets the evaluation strategy.
        /// </summary>
        /// <value>The evaluation strategy.</value>
        protected IQueryEvaluationStrategy EvaluationStrategy { get; private set; }

        /// <summary>
        /// Casts a <see cref="QueryValue"/> to a <see cref="QueryType"/>. The cast will return the value type cast to the new type.
        /// </summary>
        /// <param name="type">The type for the cast operation.</param>
        /// <returns><see cref="QueryValue"/> which is cast to the appropriate type</returns>
        public abstract QueryValue Cast(QueryType type);

        /// <summary>
        /// Checks if a <see cref="QueryValue"/> is of a particular <see cref="QueryType"/>. This operation will return a true if the value is of the specified type.
        /// </summary>
        /// <param name="type">The type for the IsOf operation.</param>
        /// <param name="performExactMatch">Determines if an exact match needs to be performed.</param>
        /// <returns>A <see cref="QueryValue"/> containing true or false depending on whether the value is of the specified type or not.</returns>
        public abstract QueryValue IsOf(QueryType type, bool performExactMatch);

        /// <summary>
        /// Converts the <see cref="QueryValue"/> to a particular <see cref="QueryType"/>.
        /// </summary>
        /// <param name="type">The type for the As operation.</param>
        /// <returns>The <see cref="QueryValue"/> converted to the specified type if successful. Returns null if this operation fails.</returns>
        public abstract QueryValue TreatAs(QueryType type);

        /// <summary>
        /// The Accept method used to support the double-dispatch visitor pattern with a visitor that returns a result.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the visitor.</typeparam>
        /// <param name="visitor">The visitor that is visiting this query value.</param>
        /// <returns>The result of visiting this query value.</returns>
        public abstract TResult Accept<TResult>(IQueryValueVisitor<TResult> visitor);

        /// <summary>
        /// Gets the type of the value.
        /// </summary>
        /// <returns>Type of the value.</returns>
        protected abstract QueryType GetTypeInternal();
    }
}
